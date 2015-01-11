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
using SoapBox.Protocol.Automation;
using System.ComponentModel.Composition;

namespace SoapBox.Snap.Application
{
    public class InstructionGroupDummy : AbstractInstructionGroupItem 
    {
        public InstructionGroupDummy(IEditorItem parent, NodeInstructionGroup instructionGroup)
            : base(parent)
        {
            if (instructionGroup == null)
            {
                throw new ArgumentNullException();
            }
            m_instructionGroup = instructionGroup;
        }

        private readonly NodeInstructionGroup m_instructionGroup = null;

        #region " Create "
        public override IInstructionGroupItem Create(IEditorItem parent, NodeInstructionGroup instructionGroup)
        {
            return new InstructionGroupDummy(parent, instructionGroup);
        }
        #endregion

        public override NodeInstructionGroup CreateEmptyNode()
        {
            return NodeInstructionGroup.BuildWith(new Protocol.Base.FieldIdentifier(string.Empty));
        }

        public string Language
        {
            get
            {
                return m_instructionGroup.Language.ToString();
            }
        }
    }
}
