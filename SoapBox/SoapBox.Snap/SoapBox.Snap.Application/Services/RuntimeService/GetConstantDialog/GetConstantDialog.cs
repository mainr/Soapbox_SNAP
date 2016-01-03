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
    /// ViewModel for the GetConstant Dialog
    /// </summary>
    [Export(CompositionPoints.Workbench.Dialogs.GetConstantDialog, typeof(GetConstantDialog))]
    public class GetConstantDialog : AbstractViewModel
    {
        public GetConstantDialog()
        {
        }

        [Import(SoapBox.Snap.Services.Solution.RuntimeService, typeof(IRuntimeService))]
        private IRuntimeService runtimeService { get; set; }

        [Import(SoapBox.Core.CompositionPoints.Host.MainWindow)]
        private Lazy<Window> mainWindowExport { get; set; }

        private FieldDataType.DataTypeEnum m_dataType = FieldDataType.DataTypeEnum.ANY;

        private void setDefaultValues()
        {
            Literal = OriginalLiteral;
            LiteralText = OriginalLiteral.Value.ToString();
        }

        /// <summary>
        /// Displays the Dialog as modal
        /// </summary>
        /// <returns>FieldConstant</returns>
        public FieldConstant ShowDialog(FieldDataType.DataTypeEnum dataType, FieldConstant defaultConstant)
        {
            m_dataType = dataType;
            OriginalLiteral = defaultConstant;
            setDefaultValues();
            Window dlg = new GetConstantDialogView();
            dlg.Owner = mainWindowExport.Value;
            dlg.DataContext = this;
            dlg.ShowDialog();
            return Literal;
        }

        #region " OriginalLiteral "
        public FieldConstant OriginalLiteral
        {
            get
            {
                return m_OriginalLiteral;
            }
            set
            {
                if (m_OriginalLiteral != value)
                {
                    m_OriginalLiteral = value;
                    NotifyPropertyChanged(m_OriginalLiteralArgs);
                }
            }
        }
        private FieldConstant m_OriginalLiteral = null;
        private static readonly PropertyChangedEventArgs m_OriginalLiteralArgs =
            NotifyPropertyChangedHelper.CreateArgs<GetConstantDialog>(o => o.OriginalLiteral);
        private static string m_OriginalLiteralName =
            NotifyPropertyChangedHelper.GetPropertyName<GetConstantDialog>(o => o.OriginalLiteral);
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
                }
            }
        }
        private FieldConstant m_Literal = null;
        private static readonly PropertyChangedEventArgs m_LiteralArgs =
            NotifyPropertyChangedHelper.CreateArgs<GetConstantDialog>(o => o.Literal);
        private static string m_LiteralName =
            NotifyPropertyChangedHelper.GetPropertyName<GetConstantDialog>(o => o.Literal);
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
            NotifyPropertyChangedHelper.CreateArgs<GetConstantDialog>(o => o.LiteralText);
        private static string m_LiteralTextName =
            NotifyPropertyChangedHelper.GetPropertyName<GetConstantDialog>(o => o.LiteralText);
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
            public CommitChangesButton(GetConstantDialog dlg)
            {
                m_GetConstantDialog = dlg;
            }

            private GetConstantDialog m_GetConstantDialog = null;

            protected override void Run()
            {
                // do a validation 
                if (FieldConstant.CheckSyntax(m_GetConstantDialog.m_dataType, m_GetConstantDialog.LiteralText))
                {
                    m_GetConstantDialog.Literal = new FieldConstant(m_GetConstantDialog.m_dataType, m_GetConstantDialog.LiteralText);
                }
                else
                {
                    m_GetConstantDialog.setDefaultValues();
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
            public CancelChangesButton(GetConstantDialog dlg)
            {
                m_GetConstantDialog = dlg;
            }

            private GetConstantDialog m_GetConstantDialog = null;

            protected override void Run()
            {
                m_GetConstantDialog.setDefaultValues();
            }
        }
        #endregion

    }
}
