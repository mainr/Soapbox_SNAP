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
using System.Collections.ObjectModel;
using SoapBox.Protocol.Automation;
using SoapBox.Core;
using System.ComponentModel;
using System.ComponentModel.Composition;
using SoapBox.Protocol.Base;
using SoapBox.Utilities;

namespace SoapBox.Snap.Application
{
    [Export(ExtensionPoints.Workbench.Pads.SolutionPad.SolutionItems, typeof(ISolutionItem))]
    [Export(CompositionPoints.Workbench.Pads.SolutionPad_.RootSolutionItem, typeof(RootSolutionItem))]
    public class RootSolutionItem : AbstractSolutionItem, IPartImportsSatisfiedNotification
    {
        public RootSolutionItem()
            : base(null, Resources.Strings.Solution, Resources.Images.Solution_RootIcon)
        {
            IsExpanded = true;
        }

        [Import(CompositionPoints.Workbench.Pads.SolutionPad, typeof(SolutionPad))]
        private SolutionPad solutionPad { get; set; }

        [ImportMany(ExtensionPoints.Workbench.Pads.SolutionPad.RootSolutionItem.ContextMenu, 
            typeof(IMenuItem), AllowRecomposition = true)]
        private IEnumerable<IMenuItem> contextMenu { get; set; }

        [Import(CompositionPoints.Workbench.Documents.RuntimeApplicationProperties, typeof(RuntimeApplicationProperties))]
        private Lazy<RuntimeApplicationProperties> runtimeApplicationProperties { get; set; }

        public void OnImportsSatisfied()
        {
            ContextMenu = extensionService.Sort(contextMenu);
            solutionPad.SolutionLoaded += new EventHandler(solutionPad_SolutionLoaded);
            this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(this_PropertyChanged);
            setHeader();
        }

        void solutionPad_SolutionLoaded(object sender, EventArgs e)
        {
            setItems();
        }
        
        void this_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_NodeName)
            {
                setHeader();
                NotifyPropertyChanged(m_SolutionArgs);
            }
        }
        private readonly string m_NodeName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractSolutionItem>(o => o.Node);

        public NodeSolution Solution
        {
            get
            {
                return this.Node as NodeSolution;
            }
            set
            {
                this.Node = value;
            }
        }
        private readonly PropertyChangedEventArgs m_SolutionArgs =
            NotifyPropertyChangedHelper.CreateArgs<RootSolutionItem>(o => o.Solution);

        #region "Header"

        /// <summary>
        /// Set the header text based on the loaded solution
        /// </summary>
        private void setHeader()
        {
            if (Solution == null)
            {
                Header = Resources.Strings.Solution + " [" + Resources.Strings.Solution_Root_NotLoadedHeader + "]";
                HeaderEdit = string.Empty;
                HeaderIsEditable = false;
                ContextMenuEnabled = false;
            }
            else
            {
                Header = Resources.Strings.Solution + " [" + Solution.SolutionName.ToString() + "]";
                HeaderEdit = Solution.SolutionName.ToString();
                HeaderIsEditable = true;
                ContextMenuEnabled = true;
            }
        }

        public override void HeaderEditAccept()
        {
            base.HeaderEditAccept();
            bool accepted = FieldSolutionName.CheckSyntax(HeaderEdit);
            if (accepted)
            {
                Solution = Solution.SetSolutionName(new FieldSolutionName(HeaderEdit));
            }
            else
            {
                HeaderEdit = Solution.SolutionName.ToString();
            }
        }

        public override void HeaderEditCancel()
        {
            base.HeaderEditCancel();
            HeaderEdit = Solution.SolutionName.ToString();
        }

        #endregion

        private void setItems()
        {
            var newCollection = new ObservableCollection<INodeWrapper>();
            if (Solution != null)
            {
                foreach (NodeRuntimeApplication app in Solution.NodeRuntimeApplicationChildren.Items)
                {
                    var rta = FindItemByNodeId(app.ID) as RuntimeApplicationItem;
                    if (rta == null)
                    {
                        rta = new RuntimeApplicationItem(this, app);
                        HookupHandlers(rta);
                    }
                    newCollection.Add(rta);
                }
            }
            Items = newCollection;
        }

        public void AddRuntimeApplication(RuntimeApplicationItem rta)
        {
            Solution = Solution.NodeRuntimeApplicationChildren.Append(
                rta.RuntimeApplication);

            HookupHandlers(rta);
            m_Items.Add(rta);
            rta.IsSelected = true;
        }

        void HookupHandlers(RuntimeApplicationItem rta)
        {
            rta.Parent = this;
            rta.Edited += new EditedHandler(RuntimeApplication_Edited);
            rta.Deleted += new DeletedHandler(RuntimeApplication_Deleted);
        }

        void RuntimeApplication_Edited(INodeWrapper sender, NodeBase oldNode, NodeBase newNode)
        {
            var oldRTA = oldNode as NodeRuntimeApplication;
            var newRTA = newNode as NodeRuntimeApplication;
            if (oldRTA != null && newRTA != null)
            {
                Solution = Solution.NodeRuntimeApplicationChildren.Replace(oldRTA, newRTA);
            }
        }
        void RuntimeApplication_Deleted(INodeWrapper sender, NodeBase deletedNode)
        {
            var deletedRTA = deletedNode as NodeRuntimeApplication;
            var solutionItem = sender as RuntimeApplicationItem;
            if (deletedRTA != null && solutionItem != null)
            {
                if (solutionItem.Parent == this)
                {
                    solutionItem.Parent = null;
                }
                Solution = Solution.NodeRuntimeApplicationChildren.Remove(deletedRTA);
                solutionItem.Edited -= new EditedHandler(RuntimeApplication_Edited);
                solutionItem.Deleted -= new DeletedHandler(RuntimeApplication_Deleted);
                m_Items.Remove(solutionItem);
            }
        }
    }
}
