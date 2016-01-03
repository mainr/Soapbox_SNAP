#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// Copyright (C) 2009-2016 SoapBox Automation, All Rights Reserved.
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
using SoapBox.Core;
using System.ComponentModel.Composition;
using System.Collections.ObjectModel;
using SoapBox.Utilities;

namespace SoapBox.Snap.Application.Workbench.Documents.PageEditor.ContextMenu
{
    [Export(ExtensionPoints.Workbench.Documents.PageEditor.ContextMenu, typeof(IMenuItem))]
    class InsertBefore : AbstractMenuItem, IPartImportsSatisfiedNotification
    {
        public InsertBefore()
        {
            ID = Extensions.Workbench.Documents.PageEditor_.ContextMenu.InsertBefore;
            Header = Resources.Strings.PageEditor_GroupCommand_InsertBefore;
            BeforeOrAfter = RelativeDirection.After;
            InsertRelativeToID = Extensions.Workbench.Documents.PageEditor_.ContextMenu.Append;
        }

        [ImportMany(ExtensionPoints.Workbench.Documents.PageEditor.InstructionGroupItems,
            typeof(IInstructionGroupItem), AllowRecomposition = true)]
        private IEnumerable<Lazy<IInstructionGroupItem, IInstructionGroupItemMeta>> instructionGroupItems { get; set; }

        public void OnImportsSatisfied()
        {
            var newItems = new Collection<IMenuItem>();
            foreach (var groupItem in instructionGroupItems)
            {
                newItems.Add(new InsertBeforeWrapper(groupItem));
            }
            Items = newItems;
        }
    }

    class InsertBeforeWrapper : AbstractMenuItem
    {
        public InsertBeforeWrapper(Lazy<IInstructionGroupItem, IInstructionGroupItemMeta> groupItem)
        {
            if (groupItem == null)
            {
                throw new ArgumentNullException("groupItem");
            }
            ID = groupItem.Metadata.Language;
            Header = ResourceHelper.GetResourceLookup<string>(groupItem.Metadata.LanguageHeaderType, groupItem.Metadata.LanguageHeaderKey);
            m_groupItem = groupItem;
        }

        Lazy<IInstructionGroupItem, IInstructionGroupItemMeta> m_groupItem = null;

        protected override void Run()
        {
            base.Run();
            var pageEditorItem = Context as SoapBox.Snap.Application.PageEditor.PageEditorItem;
            if (pageEditorItem != null)
            {
                pageEditorItem.InsertBeforeInstructionGroup(m_groupItem.Value);
            }
        }
    }
}
