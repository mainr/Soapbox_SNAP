#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// Copyright (C) 2009-2015 SoapBox Automation, All Rights Reserved.
/// Contact: SoapBox Automation Licencing (license@soapboxautomation.com)
/// 
/// This file is part of SoapBox Snap.
/// 
/// SoapBox Snap is free software: you can redistribute it and/or modify it
/// under the terms of the GNU General Public License as published by the 
/// Free Software Foundation, either version 3 of the License, or 
/// (at your option) any later version.
/// 
/// SoapBox Snap is distributed in the hope that it will be useful, but 
/// WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU General Public License for more details.
/// 
/// You should have received a copy of the GNU General Public License along
/// with SoapBox Snap. If not, see <http://www.gnu.org/licenses/>.
/// </header>
#endregion
        
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using SoapBox.Core;
using SoapBox.Snap;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Compression;
using System.IO;
using SoapBox.Protocol.Automation;
using System.Windows.Forms;
using SoapBox.Protocol.Base;
using SoapBox.Utilities;
using System.Windows;
using System.Reflection;

namespace SoapBox.Snap.Application
{
    /// <summary>
    /// Implments both the Solution Tool Pad, and the SolutionService instance
    /// </summary>
    [Export(Services.Solution.SolutionService, typeof(ISolutionService))]
    [Export(SoapBox.Core.ExtensionPoints.Workbench.Pads, typeof(IPad))]
    [Export(CompositionPoints.Workbench.Pads.SolutionPad, typeof(SolutionPad))]
    [Pad(Name = Extensions.Workbench.Pads.SolutionPad)]
    public class SolutionPad : AbstractPad, ISolutionService, IPartImportsSatisfiedNotification
    {
        public const string SOLUTION_EXTENSION = "snp";
        public const string EMPTY_SOLUTION = "Empty." + SOLUTION_EXTENSION;

        private Dictionary<string, string> m_filters = new Dictionary<string, string>();

        public SolutionPad()
        {
            m_filters.Add(SOLUTION_EXTENSION, Resources.Strings.Solution_Extension_Description);

            ID = Extensions.Workbench.Pads.SolutionPad;
            Name = Extensions.Workbench.Pads.SolutionPad;
            Title = Resources.Strings.Solution_Pad_Title;
        }

        [Import(SoapBox.Core.Services.FileDialog.FileDialogService, typeof(IFileDialogService))]
        private Lazy<IFileDialogService> fileDialogService { get; set; }

        [Import(SoapBox.Core.Services.Messaging.MessagingService, typeof(IMessagingService))]
        private Lazy<IMessagingService> messagingService { get; set; }

        [Import(CompositionPoints.Workbench.Pads.SolutionPad_.RootSolutionItem, typeof(RootSolutionItem))]
        private RootSolutionItem rootSolutionItem { get; set; }

        [ImportMany(ExtensionPoints.Workbench.Pads.SolutionPad.SolutionItems, typeof(ISolutionItem))]
        private IEnumerable<ISolutionItem> solutionItems { get; set; }

        [Import(SoapBox.Core.Services.Layout.LayoutManager, typeof(ILayoutManager))]
        private Lazy<ILayoutManager> layoutManager { get; set; }

        public void OnImportsSatisfied()
        {
            layoutManager.Value.Unloading += new EventHandler(layoutManager_Unloading);
            rootSolutionItem.Edited += new EditedHandler(rootSolutionItem_Edited);
            Collection<ISolutionItem> topItems = new Collection<ISolutionItem>();
            topItems.Add(rootSolutionItem);
            Items = topItems;
        }

        void layoutManager_Unloading(object sender, EventArgs e)
        {
            if (dirty)
            {
                var result = messagingService.Value.ShowDialog(Resources.Strings.Solution_Pad_Closing_SaveMessage,
                    Resources.Strings.Solution_Pad_Closing_SaveTitle, MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    SaveExecute();
                }
            }
        }

        void rootSolutionItem_Edited(INodeWrapper sender, NodeBase oldNode, NodeBase newNode)
        {
            dirty = true;
            m_CloseEnabled.SetCondition(true);
        }

        #region " Items "
        /// <summary>
        /// Solution Items Tree.
        /// </summary>
        public IEnumerable<ISolutionItem> Items
        {
            get
            {
                return m_Items;
            }
            protected set
            {
                if (m_Items != value)
                {
                    m_Items = value;
                    NotifyPropertyChanged(m_ItemsArgs);
                }
            }
        }
        private IEnumerable<ISolutionItem> m_Items;
        static readonly PropertyChangedEventArgs m_ItemsArgs =
            NotifyPropertyChangedHelper.CreateArgs<SolutionPad>(o => o.Items);

        #endregion

        public event EventHandler SolutionLoaded = delegate { };

        #region " Solution "

        private void setSolution(NodeSolution value, bool isDirty, bool isCloseEnabled)
        {
            if (rootSolutionItem.Solution != value)
            {
                rootSolutionItem.Solution = value;
                dirty = isDirty || m_requireSaving.Count > 0;
                m_CloseEnabled.SetCondition(isCloseEnabled);
            }
        }
        #endregion

        #region " SolutionFileName "

        public string SolutionFileName
        {
            get
            {
                return m_SolutionFilename;
            }
            private set
            {
                if (m_SolutionFilename != value)
                {
                    m_SolutionFilename = value;
                }
            }
        }
        private string m_SolutionFilename = null;

        #endregion

        #region " dirty "

        private bool dirty
        {
            get
            {
                return m_dirty;
            }
            set
            {
                if (m_dirty != value)
                {
                    m_dirty = value;
                    m_SaveEnabled.SetCondition(dirty);
                }
            }
        }
        private bool m_dirty = false;

        #endregion

        #region " New "

        public void NewExecute()
        {
            if (dirty)
            {
                // give the user a chance to save
                DialogResult result = messagingService.Value.ShowDialog(
                    Resources.Strings.Solution_Confirm_DoYouWantToSave,
                    Resources.Strings.Solution_Confirm_DoYouWantToSave_Title,
                    MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Cancel)
                {
                    return;
                }
                else if (result == DialogResult.Yes)
                {
                    SaveExecute();
                }
            }
            var newSolutionFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), EMPTY_SOLUTION);
            OpenExecute(newSolutionFile, true);
        }

        public ICondition NewEnabled
        {
            get
            {
                return m_NewEnabled;
            }
        }
        private readonly ConcreteCondition m_NewEnabled =
            new ConcreteCondition(true);

        #endregion

        #region " Open "

        public void OpenExecute()
        {
            OpenExecute(null, false);
        }

        public void OpenExecute(string openFileName, bool readOnly)
        {
            if (dirty)
            {
                // give the user a chance to save
                DialogResult result = messagingService.Value.ShowDialog(
                    Resources.Strings.Solution_Confirm_DoYouWantToSave,
                    Resources.Strings.Solution_Confirm_DoYouWantToSave_Title,
                    MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Cancel)
                {
                    return;
                }
                else if (result == DialogResult.Yes)
                {
                    SaveExecute();
                }
            }
            bool addExtension = true;
            bool checkFileExists = true;
            bool checkPathExists = true;
            string fileName;
            if (openFileName == null)
            {
                fileName = fileDialogService.Value.OpenFileDialog(
                    SOLUTION_EXTENSION, @"c:\", m_filters,
                    Resources.Strings.Solution_OpenFileDialog_Title,
                    addExtension, checkFileExists, checkPathExists);
            }
            else
            {
                fileName = openFileName;
            }
            if (fileName != null)
            {
                try
                {
                    using (Stream src = File.OpenRead(fileName))
                    {
                        using (Stream dest = new MemoryStream())
                        {
                            try
                            {
                                Decompress(src, dest);
                                dest.Seek(0, SeekOrigin.Begin); // after Decompress, pointer is at the end
                                using (StreamReader reader = new StreamReader(dest, Encoding.Unicode))
                                {
                                    string xml = reader.ReadToEnd();
                                    NodeSolution readSolution = NodeBase.NodeFromXML(xml, null) as NodeSolution;
                                    setSolution(readSolution, false, true);
                                    if (!readOnly)
                                    {
                                        SolutionFileName = fileName;
                                    }
                                    else
                                    {
                                        SolutionFileName = null;
                                    }
                                    layoutManager.Value.ShowPad(this);
                                    SolutionLoaded(this, new EventArgs());
                                    layoutManager.Value.RestoreLayout(readSolution.Layout.ToString());
                                }
                            }
                            catch (InvalidDataException)
                            {
                                messagingService.Value.ShowMessage(
                                    Resources.Strings.Solution_Open_InvalidDataException,
                                    Resources.Strings.Solution_Open_InvalidDataException_Title);
                            }
                        }
                    }
                }
                catch (PathTooLongException)
                {
                    messagingService.Value.ShowMessage(
                        Resources.Strings.Solution_Open_PathTooLongException,
                        Resources.Strings.Solution_Open_PathTooLongException_Title);
                }
                catch (DirectoryNotFoundException)
                {
                    messagingService.Value.ShowMessage(
                        Resources.Strings.Solution_Open_DirectoryNotFoundException,
                        Resources.Strings.Solution_Open_DirectoryNotFoundException_Title);
                }
                catch (UnauthorizedAccessException)
                {
                    messagingService.Value.ShowMessage(
                        Resources.Strings.Solution_Open_UnauthorizedAccessException,
                        Resources.Strings.Solution_Open_UnauthorizedAccessException_Title);
                }
                catch (FileNotFoundException)
                {
                    messagingService.Value.ShowMessage(
                        Resources.Strings.Solution_Open_FileNotFoundException,
                        Resources.Strings.Solution_Open_FileNotFoundException_Title);
                }
            }
        }

        public ICondition OpenEnabled
        {
            get
            {
                return m_OpenEnabled;
            }
        }
        private readonly ConcreteCondition m_OpenEnabled =
            new ConcreteCondition(true);

        #endregion

        #region " Close "

        public void CloseExecute()
        {
            if (dirty)
            {
                // give the user a chance to save
                DialogResult result = messagingService.Value.ShowDialog(
                    Resources.Strings.Solution_Confirm_DoYouWantToSave,
                    Resources.Strings.Solution_Confirm_DoYouWantToSave_Title,
                    MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Cancel)
                {
                    return;
                }
                else if (result == DialogResult.Yes)
                {
                    SaveExecute();
                }
            }
            setSolution(null, false, false);
            SolutionFileName = null;
            layoutManager.Value.CloseAllDocuments();
            layoutManager.Value.HideAllPads();
            SolutionLoaded(this, EventArgs.Empty);
        }

        public ICondition CloseEnabled
        {
            get
            {
                return m_CloseEnabled;
            }
        }
        private readonly ConcreteCondition m_CloseEnabled =
            new ConcreteCondition(false);

        #endregion

        #region " Save "

        public void SaveExecute()
        {
            if (SolutionFileName == null)
            {
                SaveAsExecute();
            }
            else
            {
                Saving(this, new EventArgs());

                // Grab the layout blob from the layout Manager so we can restore to this layout
                string layoutBlob = layoutManager.Value.SaveLayout();
                FieldLayout layout = new FieldLayout(string.Empty);
                if (FieldLayout.CheckSyntax(layoutBlob))
                {
                    layout = new FieldLayout(layoutBlob);
                }

                // save to existing (or new) file
                using (Stream dest = File.Create(SolutionFileName))
                {
                    byte[] byteArray = Encoding.Unicode.GetBytes(rootSolutionItem.Solution.SetLayout(layout).ToXml());
                    using (MemoryStream src = new MemoryStream(byteArray))
                    {
                        Compress(src, dest);
                        dirty = m_requireSaving.Count > 0;
                    }
                }
            }
        }

        public ICondition SaveEnabled
        {
            get
            {
                return m_SaveEnabled;
            }
        }
        private readonly ConcreteCondition m_SaveEnabled =
            new ConcreteCondition(false);

        #endregion

        #region " SaveAs "

        public void SaveAsExecute()
        {
            bool addExtension = true;
            bool checkFileExists = false;
            bool checkPathExists = true;
            string fileName = fileDialogService.Value.SaveFileDialog(
                SOLUTION_EXTENSION, @"c:\", m_filters,
                Resources.Strings.Solution_OpenFileDialog_Title,
                addExtension, checkFileExists, checkPathExists);
            if (fileName != null)
            {
                SolutionFileName = fileName;
                SaveExecute();
            }
        }

        public ICondition SaveAsEnabled
        {
            get
            {
                return m_SaveEnabled;
            }
        }

        #endregion

        #region " Saving "
        public event EventHandler Saving = delegate { };
        private readonly List<object> m_requireSaving = new List<object>();

        public void RequiresSave(object sender)
        {
            if (!m_requireSaving.Contains(sender))
            {
                m_requireSaving.Add(sender);
                dirty = true;
            }
        }

        public void NoLongerRequiresSave(object sender)
        {
            if (m_requireSaving.Contains(sender))
            {
                m_requireSaving.Remove(sender);
                dirty = m_requireSaving.Count > 0;
            }
        }
        #endregion

        #region " COMPRESSION "

        private static void Compress(Stream src, Stream dest)
        {
            using (GZipStream output = new GZipStream(dest, CompressionMode.Compress))
            {
                CopyInToOut(src, output);
            }
        }

        private static void Decompress(Stream src, Stream dest)
        {
            using (GZipStream input = new GZipStream(src, CompressionMode.Decompress))
            {
                CopyInToOut(input, dest);
            }
        }

        private static void CopyInToOut(Stream input, Stream output)
        {
            int n = 0;
            byte[] bytes = new byte[4096];
            while ((n = input.Read(bytes, 0, bytes.Length)) != 0)
            {
                output.Write(bytes, 0, n);
            }
        }

        #endregion

        #region " FindItemByNodeId "
        /// <summary>
        /// Finds the ISolutionItem in the solution tree that
        /// corresponds to the given ID, if any
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public INodeWrapper FindItemByNodeId(Guid nodeId)
        {
            var fGuid = new FieldGuid(nodeId);
            return findItemByNodeId(rootSolutionItem, fGuid);
        }

        /// <summary>
        /// Implements a depth first search
        /// </summary>
        private INodeWrapper findItemByNodeId(INodeWrapper currentItem, FieldGuid fGuid)
        {
            if (currentItem == null)
            {
                return null;
            }
            else if (currentItem.Node.ID == fGuid)
            {
                return currentItem;
            }
            else
            {
                foreach (var item in currentItem.Items)
                {
                    var foundItem = findItemByNodeId(item, fGuid);
                    if (foundItem != null)
                    {
                        return foundItem;
                    }
                }
                return null;
            }
        }
        #endregion
    }

}
