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
using System.ComponentModel.Composition;
using SoapBox.Protocol.Automation;
using SoapBox.Protocol.Base;
using SoapBox.Core;
using System.Windows;
using System.Collections.ObjectModel;
using System.Threading;

namespace SoapBox.Snap.Application
{
    [Export(Services.Solution.RuntimeService, typeof(IRuntimeService))]
    class RuntimeService : IRuntimeService, IPartImportsSatisfiedNotification
    {

        [Import(SoapBox.Core.Services.Layout.LayoutManager, typeof(ILayoutManager))]
        private ILayoutManager layoutManager { get; set; }

        [Import(SoapBox.Core.Services.Messaging.MessagingService, typeof(IMessagingService))]
        private IMessagingService messagingService { get; set; }

        [Import(SoapBox.Snap.CompositionPoints.Workbench.Dialogs.SignalChooserDialog, typeof(SignalChooserDialog))]
        private SignalChooserDialog signalChooserDialog { get; set; }

        [Import(SoapBox.Snap.CompositionPoints.Workbench.Dialogs.GetConstantDialog, typeof(GetConstantDialog))]
        private GetConstantDialog getConstantDialog { get; set; }

        [Import(SoapBox.Core.CompositionPoints.Host.MainWindow, typeof(Window))]
        private Lazy<Window> mainWindow { get; set; }

        public void OnImportsSatisfied()
        {
            ThreadPool.QueueUserWorkItem(
                o => ThreadStart()
                );
        }

        void ThreadStart()
        {
            while (true)
            {
                Thread.Sleep(50);
                refreshWatchedValues();
            }
        }

        #region IRuntimeService Members

        private Tuple<PageEditor.PageEditorItem, PageItem, RuntimeApplicationItem> FindParentPageEditorItemPageItemAndRuntimeAppItem(INodeWrapper requester)
        {
            PageEditor.PageEditorItem pageEditorItem;
            if (requester is PageEditor.PageEditorItem)
            {
                pageEditorItem = requester as PageEditor.PageEditorItem;
            }
            else
            {
                pageEditorItem = requester.FindAncestorOfType(typeof(PageEditor.PageEditorItem)) as PageEditor.PageEditorItem;
            }
            PageItem pageItem;
            if (requester is PageItem)
            {
                pageItem = requester as PageItem;
            }
            else
            {
                pageItem = requester.FindAncestorOfType(typeof(PageItem)) as PageItem;
            }
            RuntimeApplicationItem runtimeAppItem;
            if (pageEditorItem == null)
            {
                // we're outside of a page editor
                runtimeAppItem = requester.FindAncestorOfType(typeof(RuntimeApplicationItem)) as RuntimeApplicationItem;
            }
            else
            {
                // inside a page editor, we have to do some fancy footwork
                var pageEditor = pageEditorItem.PageEditorParent;
                if (pageEditor.PageItemParent != null)
                {
                    pageItem = pageEditor.PageItemParent;
                    runtimeAppItem = pageItem.FindAncestorOfType(typeof(RuntimeApplicationItem)) as RuntimeApplicationItem;
                }
                else
                {
                    runtimeAppItem = null;
                }
            }
            return new Tuple<PageEditor.PageEditorItem, PageItem, RuntimeApplicationItem>(pageEditorItem, pageItem, runtimeAppItem);
        }

        private RuntimeApplicationItem FindParentRuntimeApplicationItem(INodeWrapper requester)
        {
            return FindParentPageEditorItemPageItemAndRuntimeAppItem(requester).Item3;
        }

        public Tuple<NodePage, NodeRuntimeApplication> FindParentPageAndRuntimeApp(INodeWrapper requester)
        {
            var tpl = FindParentPageEditorItemPageItemAndRuntimeAppItem(requester);
            PageEditor.PageEditorItem pageEditorItem = tpl.Item1;
            PageItem pageItem = tpl.Item2;
            RuntimeApplicationItem runtimeAppItem = tpl.Item3;
            NodePage pg = null;
            NodeRuntimeApplication rta = null;
            if (pageEditorItem != null)
            {
                pg = pageEditorItem.WorkingCopy;
            }
            else if (pageItem != null)
            {
                pg = pageItem.Page;
            }
            if (runtimeAppItem != null)
            {
                rta = runtimeAppItem.RuntimeApplication;
            }
            return new Tuple<NodePage, NodeRuntimeApplication>(pg, rta);
        }

        /// <summary>
        /// Searches the runtime application for a signal matching the given signalId
        /// </summary>
        public Tuple<string,NodeSignal> FindSignal(INodeWrapper requester, FieldGuid signalId)
        {
            var tpl = FindParentPageAndRuntimeApp(requester);
            NodePage pg = tpl.Item1;
            NodeRuntimeApplication rta = tpl.Item2;
            if (rta != null)
            {
                var edits = new Dictionary<NodePage,NodePage>();

                if (pg != null)
                {
                    // Searches the local page first
                    var tryLocal = FindSignalAndName(pg, string.Empty, pg, signalId, edits);
                    if (tryLocal != null)
                    {
                        return tryLocal;
                    }
                }

                // Make a list of edited page copies
                foreach (var d in layoutManager.Documents)
                {
                    var pageEditor = d as PageEditor;
                    if (pageEditor != null)
                    {
                        edits.Add(pageEditor.PageItemParent.Page, pageEditor.EditorRoot.WorkingCopy);
                    }
                }

                return FindSignalAndName(rta, string.Empty, pg, signalId, edits);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a dictionary of signal names, and those signals, in a depth
        /// first search of the runtime application.  For those signals within
        /// the same page, it has no prefix, but for those in other pages or
        /// in the device configuration, it adds a directory-like prefix.
        /// </summary>
        public Dictionary<string, NodeSignal> SignalList(INodeWrapper requester, FieldDataType.DataTypeEnum dataTypeFilter)
        {
            var dict = new Dictionary<string, NodeSignal>();
            var tpl = FindParentPageAndRuntimeApp(requester);
            NodePage pg = tpl.Item1;
            NodeRuntimeApplication rta = tpl.Item2;
            if (rta != null)
            {
                if (pg != null)
                {
                    // Search the local page first
                    SignalsDepthFirst(pg, string.Empty, dict, pg, dataTypeFilter);
                } 
                SignalsDepthFirst(rta, string.Empty, dict, pg, dataTypeFilter);
            }
            return dict;
        }

        public NodeSignalIn SignalDialog(INodeWrapper requester, NodeSignalIn defaultSignalIn)
        {
            if (DisconnectDialog(requester))
            {
                return signalChooserDialog.ShowDialog(requester, defaultSignalIn);
            }
            else
            {
                return defaultSignalIn;
            }
        }

        #region SignalChanged event
        /// <summary>
        /// Fired when a signal changes.  Passes the signal
        /// that changed as a parameter of the event.
        /// </summary>
        public event SignalChangedHandler SignalChanged
        {
            add
            {
                lock (m_signalChanged_Lock)
                {
                    m_signalChanged += value;
                }
            }
            remove
            {
                lock (m_signalChanged_Lock)
                {
                    m_signalChanged -= value;
                }
            }
        }
        private event SignalChangedHandler m_signalChanged;
        private readonly object m_signalChanged_Lock = new object();

        public void NotifySignalChanged(NodeSignal signal)
        {
            if (signal != null)
            {
                SignalChangedHandler evt;
                lock (m_signalChanged_Lock)
                {
                    evt = m_signalChanged;
                }
                // Fire the event on the UI thread
                mainWindow.Value.Dispatcher.BeginInvoke(new Action(delegate() 
                {
                    if (evt != null)
                    {
                        evt(signal);
                    }
                }));
            }
        }
        #endregion

        #region ValuesChanged event
        /// <summary>
        /// Fired when any values have changed 
        /// (and GUI controls should update their state)
        /// </summary>
        public event ValueChangedHandler ValueChanged
        {
            add
            {
                lock (m_valuesChanged_Lock)
                {
                    m_valueChanged += value;
                }
            }
            remove
            {
                lock (m_valuesChanged_Lock)
                {
                    m_valueChanged -= value;
                }
            }
        }
        private event ValueChangedHandler m_valueChanged;
        private readonly object m_valuesChanged_Lock = new object();

        public void NotifyValueChanged(NodeSignal signal, object value)
        {
            if (signal != null)
            {
                ValueChangedHandler evt;
                lock (m_valuesChanged_Lock)
                {
                    evt = m_valueChanged;
                }
                // Fire the event on the UI thread
                mainWindow.Value.Dispatcher.BeginInvoke(new Action(delegate()
                {
                    if (evt != null)
                    {
                        evt(signal, value);
                    }
                }));
            }
        }
        #endregion

        public FieldConstant GetConstant(FieldDataType.DataTypeEnum dataType, FieldConstant defaultConstant)
        {
            return getConstantDialog.ShowDialog(dataType, defaultConstant);
        }

        public bool Connected(INodeWrapper requester)
        {
            if(requester == null)
            {
                return false;
            }
            var runtimeApplicationItem = FindParentRuntimeApplicationItem(requester);
            if (runtimeApplicationItem != null)
            {
                return runtimeApplicationItem.Connected;
            }
            else
            {
                // we're not inside a runtime app?
                return false;
            }
        }

        public bool DisconnectDialog(INodeWrapper requester)
        {
            if (!Connected(requester))
            {
                return true;
            }
            else
            {
                if (messagingService.ShowDialog(Resources.Strings.RuntimeService_Disconnect_Message, Resources.Strings.RuntimeService_Disconnect_Title,
                    System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes) 
                {
                    var runtimeApplicationItem = FindParentRuntimeApplicationItem(requester);
                    if (runtimeApplicationItem != null)
                    {
                        runtimeApplicationItem.Disconnect();
                        return true;
                    }
                    else
                    {
                        // we're not inside a runtime app?
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public void RegisterValueWatcher(INodeWrapper nodeItem, NodeSignal signal)
        {
            if (nodeItem == null)
            {
                throw new ArgumentNullException("nodeItem");
            }
            if (signal == null)
            {
                throw new ArgumentNullException("signal");
            }
            var rta = FindParentRuntimeApplicationItem(nodeItem);
            if (rta != null)
            {
                lock (m_valueWatchers_Lock)
                {
                    if (!m_valueWatchers.ContainsKey(rta))
                    {
                        m_valueWatchers.Add(rta, new Dictionary<NodeSignal, int>());
                    }
                    if (!m_valueWatchers[rta].ContainsKey(signal))
                    {
                        m_valueWatchers[rta].Add(signal,1);
                        m_rtaLookup[nodeItem] = rta;
                    }
                    else
                    {
                        m_valueWatchers[rta][signal] += 1;
                    }
                }
            }
        }

        public void DeregisterValueWatcher(INodeWrapper nodeItem, NodeSignal signal)
        {
            if (nodeItem == null)
            {
                throw new ArgumentNullException("nodeItem");
            }
            if (signal == null)
            {
                throw new ArgumentNullException("signal");
            }
            var rta = FindParentRuntimeApplicationItem(nodeItem);
            if (rta == null)
            {
                lock (m_valueWatchers_Lock)
                {
                    if (m_rtaLookup.ContainsKey(nodeItem))
                    {
                        rta = m_rtaLookup[nodeItem];
                    }
                }
            }
            if (rta != null)
            {
                lock (m_valueWatchers_Lock)
                {
                    if (m_valueWatchers.ContainsKey(rta))
                    {
                        if (m_valueWatchers[rta].ContainsKey(signal))
                        {
                            m_valueWatchers[rta][signal] -= 1;
                            if (m_valueWatchers[rta][signal] <= 0)
                            {
                                m_rtaLookup.Remove(nodeItem);
                                m_valueWatchers[rta].Remove(signal);
                                if (m_valueWatchers[rta].Count == 0)
                                {
                                    m_valueWatchers.Remove(rta);
                                }
                            }
                        }
                    }
                }
            }
        }
        private readonly Dictionary<INodeWrapper, RuntimeApplicationItem> m_rtaLookup =
            new Dictionary<INodeWrapper, RuntimeApplicationItem>();
        private readonly Dictionary<RuntimeApplicationItem, Dictionary<NodeSignal,int>> m_valueWatchers =
            new Dictionary<RuntimeApplicationItem, Dictionary<NodeSignal, int>>();
        private readonly object m_valueWatchers_Lock = new object();

        private void refreshWatchedValues()
        {
            lock (m_valueWatchers_Lock)
            {
                foreach (var rta in m_valueWatchers.Keys)
                {
                    if (rta.Runtime != null && rta.Connected) // the Runtime property on rta is protected by a lock, so it's threadsafe
                    {
                        rta.Runtime.ReadSignalValues(new ReadOnlyCollection<NodeSignal>(m_valueWatchers[rta].Keys.ToList()));
                    }
                }
            }
        }
        #endregion

        #region SignalsDepthFirst
        private static void SignalsDepthFirst(NodeBase fromNode, string header, Dictionary<string, NodeSignal> dict, NodePage localRoot, 
            FieldDataType.DataTypeEnum dataTypeFilter)
        {
            const string SEPARATOR = "/";

            string prevHeader;
            if (header.Length == 0)
            {
                prevHeader = string.Empty;
            }
            else
            {
                prevHeader = header + SEPARATOR;
            }

            var thisSig = fromNode as NodeSignal;
            if(thisSig != null)
            {
                if (thisSig.SignalName.ToString().Length > 0) // ignore unnamed signals
                {
                    var key = prevHeader + thisSig.SignalName;
                    if (thisSig.DataType.IsOfType(dataTypeFilter))
                    {
                        if (!dict.ContainsKey(key))
                        {
                            dict.Add(key, thisSig);
                        }
                    }
                }
            }
            else
            {
                foreach (var child in fromNode.ChildCollection)
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


                    if (nInstructionGroup != null || nInstruction != null || nDeviceConfiguration != null
                        || nDriver != null || nDiscreteInput != null || nAnalogInput != null 
                        || nStringInput != null || nSignal != null)
                    {
                        SignalsDepthFirst(child, header, dict, localRoot, dataTypeFilter);
                    }
                    else if (nPageCollection != null)
                    {
                        SignalsDepthFirst(child, prevHeader + nPageCollection.PageCollectionName, dict, localRoot, dataTypeFilter);
                    }
                    else if (nPage != null)
                    {
                        string newHeader = prevHeader + nPage.PageName;
                        if (nPage == localRoot)
                        {
                            newHeader = string.Empty;
                        }
                        SignalsDepthFirst(child, newHeader, dict, localRoot, dataTypeFilter);
                    }
                    else if (nDevice != null)
                    {
                        SignalsDepthFirst(child, prevHeader + nDevice.DeviceName, dict, localRoot, dataTypeFilter);
                    }

                }
            }
        }
        #endregion

        #region FindSignalAndName
        private static Tuple<string, NodeSignal> FindSignalAndName(NodeBase fromNode, string header, NodePage localRoot, FieldGuid signalId, 
            Dictionary<NodePage, NodePage> edits)
        {
            const string SEPARATOR = "/";

            string prevHeader;
            if (header.Length == 0)
            {
                prevHeader = string.Empty;
            }
            else
            {
                prevHeader = header + SEPARATOR;
            }

            var thisSig = fromNode as NodeSignal;
            if (thisSig != null && thisSig.SignalId == signalId)
            {
                return new Tuple<string, NodeSignal>(prevHeader + thisSig.SignalName, thisSig);
            }
            else
            {
                foreach (var child in fromNode.ChildCollection)
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

                    Tuple<string, NodeSignal> found = null;

                    if (nInstructionGroup != null || nInstruction != null || nDeviceConfiguration != null
                        || nDriver != null || nDiscreteInput != null || nAnalogInput != null 
                        || nStringInput != null || nSignal != null)
                    {
                        found = FindSignalAndName(child, header, localRoot, signalId, edits);
                    }
                    else if (nPageCollection != null)
                    {
                        found = FindSignalAndName(child, prevHeader + nPageCollection.PageCollectionName, localRoot, signalId, edits);
                    }
                    else if (nPage != null)
                    {
                        string newHeader = prevHeader + nPage.PageName;
                        if (nPage == localRoot) // first, search local page, if there is one
                        {
                            found = FindSignalAndName(child, string.Empty, localRoot, signalId, edits);
                        }
                        if (found == null && edits.ContainsKey(nPage)) // search edited page, if there is one
                        {
                            found = FindSignalAndName(edits[nPage], newHeader, localRoot, signalId, edits);
                        }
                        if (found == null) // search from the root otherwise
                        {
                            found = FindSignalAndName(child, newHeader, localRoot, signalId, edits);
                        }
                    }
                    else if (nDevice != null)
                    {
                        found = FindSignalAndName(child, prevHeader + nDevice.DeviceName, localRoot, signalId, edits);
                    }
                    if (found != null)
                    {
                        return found;
                    }
                }
                return null;
            }
        }
        #endregion

    }
}
