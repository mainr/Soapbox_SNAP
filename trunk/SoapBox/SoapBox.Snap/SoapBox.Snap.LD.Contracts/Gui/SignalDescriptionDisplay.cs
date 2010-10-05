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
using SoapBox.Protocol.Automation;
using System.ComponentModel.Composition;
using System.ComponentModel;
using SoapBox.Utilities;

namespace SoapBox.Snap.LD
{
    [Export(SoapBox.Core.ExtensionPoints.Host.Void, typeof(object))]
    public class SignalDescriptionDisplay : SignalDescriptionEditor
    {
         // for MEF to call
        public SignalDescriptionDisplay()
            : base(string.Empty, 1, 1, TextAlignment.Center,false)
        {
        }

        public SignalDescriptionDisplay(IInstructionItem instructionNode, NodeSignalIn signalIn, double maxWidth, double maxHeight, TextAlignment textAlignment)
            : base(string.Empty, maxWidth, maxHeight, textAlignment, false)
        {
            if (instructionNode == null)
            {
                throw new ArgumentNullException("instructionNode");
            }

            m_instructionNode = instructionNode;
            SignalIn = signalIn;
            runtimeService.SignalChanged += new SignalChangedHandler(runtimeService_SignalChanged);
        }

        void runtimeService_SignalChanged(NodeSignal signal)
        {
            if (SignalIn != null && signal != null && SignalIn.SignalId == signal.SignalId)
            {
                setText();
            }
        }

        private IInstructionItem m_instructionNode = null;

        #region " runtimeServiceSingleton "
       [Import(Services.Solution.RuntimeService, typeof(IRuntimeService))]
       private IRuntimeService runtimeService
       {
           get
           {
               return m_runtimeService;
           }
           set
           {
               m_runtimeService = value;
           }
       }
       private static IRuntimeService m_runtimeService = null;

       #endregion

        private void setText()
        {
            var newText = string.Empty;
            if (SignalIn.SignalId != null)
            {
                var t = runtimeService.FindSignal(m_instructionNode, SignalIn.SignalId);
                if (t != null)
                {
                    newText = t.Item2.Comment.ToString();
                }
            }
            Text = newText;
        }

        #region " SignalIn "
        public NodeSignalIn SignalIn
        {
            get
            {
                return m_SignalIn;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_SignalInName);
                }
                if (m_SignalIn != value)
                {
                    m_SignalIn = value;
                    setText();
                    NotifyPropertyChanged(m_SignalInArgs);
                }
            }
        }
        private NodeSignalIn m_SignalIn = null;
        private static readonly PropertyChangedEventArgs m_SignalInArgs =
            NotifyPropertyChangedHelper.CreateArgs<SignalDescriptionDisplay>(o => o.SignalIn);
        private static string m_SignalInName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalDescriptionDisplay>(o => o.SignalIn);
        #endregion

    }
}
