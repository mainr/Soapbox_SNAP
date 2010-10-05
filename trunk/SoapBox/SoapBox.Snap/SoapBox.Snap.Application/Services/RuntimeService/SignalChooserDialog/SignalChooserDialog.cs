#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// Copyright (C) 2009 SoapBox Automation Inc., All Rights Reserved.
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
using System.Windows;
using System.ComponentModel.Composition;
using SoapBox.Core;
using SoapBox.Protocol.Base;
using System.ComponentModel;
using SoapBox.Utilities;
using SoapBox.Protocol.Automation;
using System.Collections.ObjectModel;

namespace SoapBox.Snap.Application
{
    /// <summary>
    /// ViewModel for the Signal Chooser Dialog
    /// </summary>
    [Export(CompositionPoints.Workbench.Dialogs.SignalChooserDialog, typeof(SignalChooserDialog))]
    public class SignalChooserDialog : AbstractViewModel
    {
        public SignalChooserDialog()
        {
        }

        [Import(SoapBox.Snap.Services.Solution.RuntimeService, typeof(IRuntimeService))]
        private IRuntimeService runtimeService { get; set; }

        [Import(SoapBox.Core.CompositionPoints.Host.MainWindow)]
        private Lazy<Window> mainWindowExport { get; set; }

        private void setDefaultValues()
        {
            SignalId = OriginalSignalIn.SignalId;
            Literal = OriginalSignalIn.Literal;
            if (OriginalSignalIn.SignalId != null)
            {
                SignalSelected = true;
            }
            else if (OriginalSignalIn.Literal != null)
            {
                LiteralSelected = true;
            }
        }

        private FieldDataType.DataTypeEnum m_dataTypefilter = FieldDataType.DataTypeEnum.ANY;

        /// <summary>
        /// Displays the Dialog as modal
        /// </summary>
        /// <returns>Signal ID</returns>
        public NodeSignalIn ShowDialog(INodeWrapper requester, NodeSignalIn originalSignalIn)
        {
            m_dataTypefilter = originalSignalIn.CompatibleTypes.DataType;
            OriginalSignalIn = originalSignalIn;
            setDefaultValues();
            NodeItem = requester;
            Window dlg = new SignalChooserDialogView();
            dlg.Owner = mainWindowExport.Value;
            dlg.DataContext = this;
            dlg.ShowDialog();
            if (SignalSelected)
            {
                return NodeSignalIn.BuildWith(OriginalSignalIn.DataType, OriginalSignalIn.CompatibleTypes, SignalId);
            }
            else
            {
                return NodeSignalIn.BuildWith(OriginalSignalIn.DataType, OriginalSignalIn.CompatibleTypes, Literal);
            }
        }

        #region " OriginalSignalIn "
        public NodeSignalIn OriginalSignalIn
        {
            get
            {
                return m_OriginalSignalIn;
            }
            set
            {
                if (m_OriginalSignalIn != value)
                {
                    m_OriginalSignalIn = value;
                    NotifyPropertyChanged(m_OriginalSignalInArgs);
                }
            }
        }
        private NodeSignalIn m_OriginalSignalIn = null;
        private static readonly PropertyChangedEventArgs m_OriginalSignalInArgs =
            NotifyPropertyChangedHelper.CreateArgs<SignalChooserDialog>(o => o.OriginalSignalIn);
        private static string m_OriginalSignalInName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalChooserDialog>(o => o.OriginalSignalIn);
        #endregion

        #region " SignalId "
        public FieldGuid SignalId
        {
            get
            {
                return m_SignalId;
            }
            set
            {
                if (m_SignalId != value)
                {
                    m_SignalId = value;
                    NotifyPropertyChanged(m_SignalIdArgs);
                }
            }
        }
        private FieldGuid m_SignalId = null;
        private static readonly PropertyChangedEventArgs m_SignalIdArgs =
            NotifyPropertyChangedHelper.CreateArgs<SignalChooserDialog>(o => o.SignalId);
        private static string m_SignalIdName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalChooserDialog>(o => o.SignalId);
        #endregion

        #region " SignalSelected "
        public bool SignalSelected
        {
            get
            {
                return m_SignalSelected;
            }
            set
            {
                if (m_SignalSelected_PreventLoop == false)
                {
                    m_SignalSelected_PreventLoop = true;
                    if (m_SignalSelected != value)
                    {
                        m_SignalSelected = value;
                        NotifyPropertyChanged(m_SignalSelectedArgs);
                        NotifyPropertyChanged(m_LiteralSelectedArgs);
                    }
                    m_SignalSelected_PreventLoop = false;
                }
            }
        }
        private bool m_SignalSelected = false;
        private static readonly PropertyChangedEventArgs m_SignalSelectedArgs =
            NotifyPropertyChangedHelper.CreateArgs<SignalChooserDialog>(o => o.SignalSelected);
        private static string m_SignalSelectedName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalChooserDialog>(o => o.SignalSelected);

        private bool m_SignalSelected_PreventLoop = false;
        #endregion

        #region " Literal "
        public FieldConstant Literal
        {
            get
            {
                return m_Literal;
            }
            set
            {
                if (m_Literal != value)
                {
                    m_Literal = value;
                    NotifyPropertyChanged(m_LiteralArgs);
                    if (m_Literal == null)
                    {
                        LiteralText = string.Empty;
                    }
                    else
                    {
                        LiteralText = m_Literal.Value.ToString();
                    }
                }
            }
        }
        private FieldConstant m_Literal = null;
        private static readonly PropertyChangedEventArgs m_LiteralArgs =
            NotifyPropertyChangedHelper.CreateArgs<SignalChooserDialog>(o => o.Literal);
        private static string m_LiteralName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalChooserDialog>(o => o.Literal);
        #endregion

        #region " LiteralSelected "
        public bool LiteralSelected
        {
            get
            {
                return !m_SignalSelected;
            }
            set
            {
                SignalSelected = !value;
            }
        }
         private static readonly PropertyChangedEventArgs m_LiteralSelectedArgs =
            NotifyPropertyChangedHelper.CreateArgs<SignalChooserDialog>(o => o.LiteralSelected);
        private static string m_LiteralSelectedName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalChooserDialog>(o => o.LiteralSelected);
        #endregion

        #region " LiteralText "
        public string LiteralText
        {
            get
            {
                return m_LiteralText;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_LiteralTextName);
                }
                if (m_LiteralText != value)
                {
                    m_LiteralText = value;
                    NotifyPropertyChanged(m_LiteralTextArgs);
                }
            }
        }
        private string m_LiteralText = string.Empty;
        private static readonly PropertyChangedEventArgs m_LiteralTextArgs =
            NotifyPropertyChangedHelper.CreateArgs<SignalChooserDialog>(o => o.LiteralText);
        private static string m_LiteralTextName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalChooserDialog>(o => o.LiteralText);
        #endregion

        #region " OK Button "
        public IControl OKButton
        {
            get
            {
                if (m_OKButton == null)
                {
                    m_OKButton = new CommitChangesButton(this);
                }
                return m_OKButton;
            }
        }
        private IControl m_OKButton = null;

        private class CommitChangesButton : AbstractButton
        {
            public CommitChangesButton(SignalChooserDialog dlg)
            {
                m_SignalChooserDialog = dlg;
            }

            private SignalChooserDialog m_SignalChooserDialog = null;

            protected override void Run()
            {
                // Don't need to do anything with SignalId, the tree does the work of filling in SignalId
                if (m_SignalChooserDialog.LiteralSelected)
                {
                    // do a validation 
                    if (FieldConstant.CheckSyntax(m_SignalChooserDialog.OriginalSignalIn.DataType.DataType, m_SignalChooserDialog.LiteralText))
                    {
                        m_SignalChooserDialog.Literal = new FieldConstant(m_SignalChooserDialog.OriginalSignalIn.DataType.DataType, m_SignalChooserDialog.LiteralText);
                    }
                    else
                    {
                        m_SignalChooserDialog.setDefaultValues();
                    }
                }
            }
        }
        #endregion

        #region " Cancel Button "
        public IControl CancelButton
        {
            get
            {
                if (m_CancelButton == null)
                {
                    m_CancelButton = new CancelChangesButton(this);
                }
                return m_CancelButton;
            }
        }
        private IControl m_CancelButton = null;

        private class CancelChangesButton : AbstractButton
        {
            public CancelChangesButton(SignalChooserDialog dlg)
            {
                m_SignalChooserDialog = dlg;
            }

            private SignalChooserDialog m_SignalChooserDialog = null;

            protected override void Run()
            {
                m_SignalChooserDialog.setDefaultValues();
            }
        }
        #endregion

        #region " SignalTree "
        public AbstractTreeView SignalTree
        {
            get
            {
                return m_SignalTree;
            }
        }
        private AbstractTreeView m_SignalTree = null;

        private class SignalTreeClass : AbstractTreeView
        {
            public SignalTreeClass(SignalChooserDialog dlg)
            {
                var tpl = dlg.runtimeService.FindParentPageAndRuntimeApp(dlg.NodeItem);
                NodePage pg = tpl.Item1;
                NodeRuntimeApplication rta = tpl.Item2;
                Items = BuildSignalTree(dlg, rta, pg, dlg.SignalId, dlg.m_dataTypefilter);
            }

            private Collection<SignalTreeItem> BuildSignalTree(SignalChooserDialog dlg, NodeBase n, 
                    NodePage pg, FieldGuid selectedSignalId, FieldDataType.DataTypeEnum dataTypeFilter)
            {
                var items = new Collection<SignalTreeItem>();

                foreach (var child in n.ChildCollection)
                {
                    var nPageCollection = child as NodePageCollection;
                    var nPage = child as NodePage;
                    var nInstructionGroup = child as NodeInstructionGroup;
                    var nInstruction = child as NodeInstruction;
                    var nSignal = child as NodeSignal;
                    var nDeviceConfiguration = child as NodeDeviceConfiguration;
                    var nDriver = child as NodeDriver;
                    var nDevice = child as NodeDevice;
                    var nDiscreteInput = child as NodeDiscreteInput;
                    var nAnalogInput = child as NodeAnalogInput;
                    var nStringInput = child as NodeStringInput;

                    // the following logic sets one or the other, or neither
                    SignalTreeItem item = null;
                    NodeBase searchChildren = null;
                    bool sort = false;
                    var adjustedChild = child;

                    if (nPageCollection != null)
                    {
                        item = new SignalTreeItem(dlg, nPageCollection.PageCollectionName.ToString(), null);
                    }
                    else if (nPage != null)
                    {
                        var pgToUse = nPage;
                        if (pg != null && nPage.PageId == pg.PageId)
                        {
                            pgToUse = pg;
                            adjustedChild = pg;
                        }
                        item = new SignalTreeItem(dlg, pgToUse.PageName.ToString(), null);
                        sort = true;
                    }
                    else if (nInstructionGroup != null || nInstruction != null || nDiscreteInput != null || nAnalogInput != null || nStringInput != null)
                    {
                        searchChildren = adjustedChild;
                    }
                    else if (nSignal != null)
                    {
                        if (nSignal.DataType.IsOfType(dataTypeFilter))
                        {
                            item = new SignalTreeItem(dlg, nSignal.SignalName.ToString(), nSignal);
                            if (nSignal.SignalId == selectedSignalId)
                            {
                                item.IsSelected = true;
                            }
                        }
                    }
                    else if (nDeviceConfiguration != null)
                    {
                        item = new SignalTreeItem(dlg, Resources.Strings.Solution_Pad_DeviceConfigurationItem_Header, null);
                    }
                    else if (nDriver != null)
                    {
                        item = new SignalTreeItem(dlg, nDriver.DriverName.ToString(), null);
                    }
                    else if (nDevice != null)
                    {
                        item = new SignalTreeItem(dlg, nDevice.DeviceName.ToString(), null);
                    }

                    if (searchChildren != null)
                    {
                        var childItems = BuildSignalTree(dlg, searchChildren, pg, selectedSignalId, dataTypeFilter);
                        if(childItems != null)
                        {
                            foreach (var childItem in childItems)
                            {
                                items.Add(childItem);
                            }
                        }
                    }

                    if (item != null)
                    {
                        items.Add(item);
                        var childItems = BuildSignalTree(dlg, adjustedChild, pg, selectedSignalId, dataTypeFilter);
                        if (childItems != null)
                        {
                            if (sort)
                            {
                                var sorted = from c in childItems orderby c.Text select c;
                                childItems = new Collection<SignalTreeItem>();
                                foreach (var c in sorted)
                                {
                                    childItems.Add(c);
                                }
                            }
                            // make sure to have this branch of the tree expanded if the selected node is somewhere down there
                            if (childItems.Count((SignalTreeItem ti) => ti.IsSelected) > 0
                                || childItems.Count((SignalTreeItem ti) => ti.IsExpanded) > 0)
                            {
                                item.IsExpanded = true;
                            }

                            item.SetItems(childItems);
                        }
                    }
                }

                if (items.Count > 0)
                {
                    return items;
                }
                else
                {
                    return null;
                }
            }

            private class SignalTreeItem : AbstractTreeViewItem
            {
                public SignalTreeItem(SignalChooserDialog dlg, string text, NodeSignal sig)
                {
                    if (dlg == null)
                    {
                        throw new ArgumentNullException("dlg");
                    }
                    m_dlg = dlg;
                    Text = text;
                    m_NodeSignal = sig;
                }

                private SignalChooserDialog m_dlg = null;
                private NodeSignal m_NodeSignal = null;

                protected override void OnIsSelectedChanged()
                {
                    base.OnIsSelectedChanged();
                    if (IsSelected)
                    {
                        if (m_NodeSignal != null)
                        {
                            m_dlg.SignalId = m_NodeSignal.SignalId;
                        }
                    }
                }

                public void SetItems(Collection<SignalTreeItem> value)
                {
                    Items = value;
                }
            }
        }
        #endregion

        #region " NodeItem "
        public INodeWrapper NodeItem
        {
            get
            {
                return m_NodeItem;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_NodeItemName);
                }
                if (m_NodeItem != value)
                {
                    m_NodeItem = value;
                    NotifyPropertyChanged(m_NodeItemArgs);
                }
                m_SignalTree = new SignalTreeClass(this);
            }
        }
        private INodeWrapper m_NodeItem = null;
        private static readonly PropertyChangedEventArgs m_NodeItemArgs =
            NotifyPropertyChangedHelper.CreateArgs<SignalChooserDialog>(o => o.NodeItem);
        private static string m_NodeItemName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalChooserDialog>(o => o.NodeItem);
        #endregion


    }
}
