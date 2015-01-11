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
using SoapBox.Core;
using System.ComponentModel.Composition;
using System.ComponentModel;
using SoapBox.Protocol.Automation;
using SoapBox.Utilities;
using System.Collections.ObjectModel;
using SoapBox.Protocol.Base;
using System.Windows;

namespace SoapBox.Snap.Application
{
    [Export(SoapBox.Core.ExtensionPoints.Workbench.Documents, typeof(IDocument))]
    [Export(CompositionPoints.Workbench.Documents.RuntimeApplicationProperties, typeof(RuntimeApplicationProperties))]
    [Document(Name = Extensions.Workbench.Documents.RuntimeApplicationProperties)]
    public class RuntimeApplicationProperties : AbstractDocument, IPartImportsSatisfiedNotification
    {
        private RuntimeApplicationProperties()
        {
            // Constructs the factory.  Called by MEF.
            // Also called by self when creating a new runtime application
            Name = Extensions.Workbench.Documents.RuntimeApplicationProperties;
        }

        public RuntimeApplicationProperties(RuntimeApplicationItem runtimeApplication)
        {
            Name = Extensions.Workbench.Documents.RuntimeApplicationProperties;
            //Memento = runtimeApplication.RuntimeApplication.RuntimeId.ToString();
            //Title = runtimeApplication.RuntimeApplication.Code.ToString();
            RuntimeApplication = runtimeApplication;
            //m_editRuntimeApplication = runtimeApplication.RuntimeApplication;
        }

        [Import(SoapBox.Core.Services.Host.ExtensionService, typeof(IExtensionService))]
        private IExtensionService extensionService { get; set; }

        [ImportMany(ExtensionPoints.Runtime.Types, typeof(IRuntimeType), AllowRecomposition = true)]
        public IEnumerable<IRuntimeType> RuntimeTypes { get; private set; }

        public void OnImportsSatisfied()
        {
            TypeList = extensionService.Sort(RuntimeTypes);
        }

        private NodeRuntimeApplication m_editRuntimeApplication = null;

        #region "RuntimeApplication"

        /// <summary>
        /// If null, then we're creating a new RuntimeApplication.
        /// If not null, we're editing an existing one, and have
        /// to apply the changes at the end.
        /// </summary>
        public RuntimeApplicationItem RuntimeApplication
        {
            get
            {
                return m_RuntimeApplication;
            }
            set
            {
                if (m_RuntimeApplication != value)
                {
                    if (m_RuntimeApplication != null)
                    {
                        m_RuntimeApplication.PropertyChanged -= m_RuntimeApplication_PropertyChanged;
                    }
                    m_RuntimeApplication = value;
                    if (m_RuntimeApplication != null)
                    {
                        m_editRuntimeApplication = m_RuntimeApplication.RuntimeApplication;
                        Memento = m_editRuntimeApplication.RuntimeId.ToString();
                        Title = m_editRuntimeApplication.Code.ToString();
                        m_RuntimeApplication.PropertyChanged += m_RuntimeApplication_PropertyChanged;
                    }
                    NotifyPropertyChanged(m_RuntimeApplicationArgs);
                }
            }
        }
        private RuntimeApplicationItem m_RuntimeApplication = null;
        static readonly PropertyChangedEventArgs m_RuntimeApplicationArgs =
            NotifyPropertyChangedHelper.CreateArgs<RuntimeApplicationProperties>(o => o.RuntimeApplication);

        public void m_RuntimeApplication_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_RuntimeApplicationName)
            {
                Memento = m_RuntimeApplication.RuntimeApplication.RuntimeId.ToString();
                Title = m_RuntimeApplication.RuntimeApplication.Code.ToString();
            }
        }
        static readonly string m_RuntimeApplicationName =
            NotifyPropertyChangedHelper.GetPropertyName<RuntimeApplicationItem>(o => o.RuntimeApplication);
        #endregion

        #region "rootSolutionItemSingleton"
        [Import(CompositionPoints.Workbench.Pads.SolutionPad_.RootSolutionItem, typeof(RootSolutionItem))]
        private RootSolutionItem rootSolutionItem
        {
            get
            {
                return rootSolutionItemSingleton;
            }
            set
            {
                rootSolutionItemSingleton = value;
            }
        }
        private static RootSolutionItem rootSolutionItemSingleton = null;
        #endregion

        #region "layoutManagerSingleton"
        [Import(SoapBox.Core.Services.Layout.LayoutManager, typeof(ILayoutManager))]
        private Lazy<ILayoutManager> layoutManager
        {
            get
            {
                return layoutManagerSingleton;
            }
            set
            {
                layoutManagerSingleton = value;
            }
        }
        private static Lazy<ILayoutManager> layoutManagerSingleton = null;
        #endregion

        #region "Close"
        public void Close()
        {
            layoutManagerSingleton.Value.CloseDocument(this);
        }
        #endregion

        #region "Save"
        public void Save()
        {
            if (RuntimeApplication == null)
            {
                // it's a new one
                RuntimeApplication = new RuntimeApplicationItem(null, m_editRuntimeApplication);
                rootSolutionItem.AddRuntimeApplication(RuntimeApplication);
            }
            else
            {
                // it's an existing one
                RuntimeApplication.RuntimeApplication = m_editRuntimeApplication;
            }
        }
        #endregion

        #region "CreateDocument"

        static Dictionary<string,RuntimeApplicationProperties> m_docs = 
            new Dictionary<string,RuntimeApplicationProperties>();

        /// <summary>
        /// Assumes the memento is the RuntimeId (FieldGuid)
        /// </summary>
        /// <param name="memento"></param>
        /// <returns></returns>
        public override IDocument CreateDocument(string memento)
        {
            if (memento == null)
            {
                // They want to create a new runtime application (not saved yet)
                var rtp = new RuntimeApplicationProperties();
                rtp.Title = Resources.Strings.RuntimeApplication_NewTitle;
                rtp.m_editRuntimeApplication = NodeRuntimeApplication.BuildWith(
                    new FieldIdentifier(Resources.Strings.RuntimeApplication_Code_Default),    // Code 
                    new FieldGuid(firstRuntimeId()),// TypeId
                    new FieldGuid(),            // RuntimeId
                    new FieldString(),          // Address
                    new FieldBase64(),          // Configuration
                    new FieldBool(false));      // ExecuteOnStartup
                return rtp;
            }

            if (!m_docs.ContainsKey(memento))
            {
                RuntimeApplicationItem item = null;
                foreach (var solutionItem in rootSolutionItem.Items)
                {
                    RuntimeApplicationItem itemTest = solutionItem as RuntimeApplicationItem;
                    if (itemTest != null)
                    {
                        if (itemTest.RuntimeApplication.RuntimeId.ToString() == memento)
                        {
                            item = itemTest;
                            break;
                        }
                    }
                }

                if (item != null)
                {
                    RuntimeApplicationProperties rtp = new RuntimeApplicationProperties(item);
                    m_docs.Add(rtp.Memento, rtp);
                }
                else
                {
                    return null;
                }
            }
            return m_docs[memento];
        }
        #endregion

        #region "Valid"

        private void ValidProperty(string propertyName)
        {
            if (m_InvalidProperties.Contains(propertyName))
            {
                m_InvalidProperties.Remove(propertyName);
                Valid.SetCondition(m_InvalidProperties.Count == 0);
            }
        }
        private void InvalidProperty(string propertyName)
        {
            if (!m_InvalidProperties.Contains(propertyName))
            {
                m_InvalidProperties.Add(propertyName);
                Valid.SetCondition(m_InvalidProperties.Count == 0);
            }
        }
        private readonly Collection<string> m_InvalidProperties = new Collection<string>();

        public readonly ConcreteCondition Valid = new ConcreteCondition(true);

        #endregion

        #region "Code"

        public string Code
        {
            get
            {
                return m_editRuntimeApplication.Code.ToString();
            }
            set
            {
                InvalidProperty(m_CodeName);
                if (m_editRuntimeApplication.Code.ToString() != value)
                {
                    m_editRuntimeApplication
                        = m_editRuntimeApplication.SetCode(new FieldIdentifier(value));
                    NotifyPropertyChanged(m_CodeArgs);
                }
                ValidProperty(m_CodeName);
            }
        }
        static readonly PropertyChangedEventArgs m_CodeArgs =
            NotifyPropertyChangedHelper.CreateArgs<RuntimeApplicationProperties>(o => o.Code);
        static readonly string m_CodeName =
            NotifyPropertyChangedHelper.GetPropertyName<RuntimeApplicationProperties>(o => o.Code);

        #endregion

        #region "Type"

        public IEnumerable<IRuntimeType> TypeList
        {
            get
            {
                return m_TypeList;
            }
            private set
            {
                if (m_TypeList != value)
                {
                    m_TypeList = value;
                    NotifyPropertyChanged(m_TypeListArgs);
                }
            }
        }
        private static IEnumerable<IRuntimeType> m_TypeList = null; // only one
        static readonly PropertyChangedEventArgs m_TypeListArgs =
            NotifyPropertyChangedHelper.CreateArgs<RuntimeApplicationProperties>(o => o.TypeList);
        static readonly string m_TypeListName =
            NotifyPropertyChangedHelper.GetPropertyName<RuntimeApplicationProperties>(o => o.TypeList);

        private bool validType(string typeId)
        {
            bool retVal = false;
            foreach (IRuntimeType rt in TypeList)
            {
                if (rt.TypeId.ToString() == typeId)
                {
                    retVal = true;
                }
            }
            return retVal;
        }

        public string TypeId
        {
            get
            {
                return m_editRuntimeApplication.TypeId.ToString();
            }
            set
            {
                if (validType(value))
                {
                    ValidProperty(m_TypeIdName);
                    if (m_editRuntimeApplication.TypeId.ToString() != value)
                    {
                        m_editRuntimeApplication
                            = m_editRuntimeApplication.SetTypeId(new FieldGuid(value));
                        NotifyPropertyChanged(m_TypeIdArgs);
                    }
                }
                else
                {
                    InvalidProperty(m_TypeIdName);
                    throw new ArgumentOutOfRangeException(m_TypeIdName);
                }
            }
        }
        static readonly PropertyChangedEventArgs m_TypeIdArgs =
            NotifyPropertyChangedHelper.CreateArgs<RuntimeApplicationProperties>(o => o.TypeId);
        static readonly string m_TypeIdName =
            NotifyPropertyChangedHelper.GetPropertyName<RuntimeApplicationProperties>(o => o.TypeId);

        private Guid firstRuntimeId()
        {
            Guid defaultTypeId;
            IRuntimeType firstRuntime = null;
            foreach (var rt in TypeList)
            {
                firstRuntime = rt;
                break;
            }
            if (firstRuntime != null)
            {
                defaultTypeId = firstRuntime.TypeId;
            }
            else
            {
                defaultTypeId = Guid.NewGuid();
                InvalidProperty(m_TypeIdName);
            }
            return defaultTypeId;
        }
        #endregion

        #region "ExecuteOnStartup"

        public bool ExecuteOnStartup
        {
            get
            {
                return m_editRuntimeApplication.ExecuteOnStartup.BoolValue;
            }
            set
            {
                if (m_editRuntimeApplication.ExecuteOnStartup.BoolValue != value)
                {
                    m_editRuntimeApplication 
                        = m_editRuntimeApplication.SetExecuteOnStartup(new FieldBool(value));
                    NotifyPropertyChanged(m_ExecuteOnStartupArgs);
                }
            }
        }
        static readonly PropertyChangedEventArgs m_ExecuteOnStartupArgs =
            NotifyPropertyChangedHelper.CreateArgs<RuntimeApplicationProperties>(o => o.ExecuteOnStartup);

        #endregion

        #region "Address"

        public string Address
        {
            get
            {
                return m_editRuntimeApplication.Address.ToString();
            }
            set
            {
                InvalidProperty(m_AddressName);
                if (m_editRuntimeApplication.Address.ToString() != value)
                {
                    m_editRuntimeApplication
                        = m_editRuntimeApplication.SetAddress(new FieldString(value));
                    NotifyPropertyChanged(m_AddressArgs);
                }
                ValidProperty(m_AddressName);
            }
        }
        static readonly PropertyChangedEventArgs m_AddressArgs =
            NotifyPropertyChangedHelper.CreateArgs<RuntimeApplicationProperties>(o => o.Address);
        static readonly string m_AddressName =
            NotifyPropertyChangedHelper.GetPropertyName<RuntimeApplicationProperties>(o => o.Address);

        #endregion

        #region "Configuration"

        public string Configuration
        {
            get
            {
                return m_editRuntimeApplication.Configuration.Decode();
            }
            set
            {
                InvalidProperty(m_ConfigurationName);
                if (m_editRuntimeApplication.Configuration.Decode() != value)
                {
                    m_editRuntimeApplication
                        = m_editRuntimeApplication.SetConfiguration(FieldBase64.Encode(value));
                    NotifyPropertyChanged(m_ConfigurationArgs);
                }
                ValidProperty(m_ConfigurationName);
            }
        }
        static readonly PropertyChangedEventArgs m_ConfigurationArgs =
            NotifyPropertyChangedHelper.CreateArgs<RuntimeApplicationProperties>(o => o.Configuration);
        static readonly string m_ConfigurationName =
            NotifyPropertyChangedHelper.GetPropertyName<RuntimeApplicationProperties>(o => o.Configuration);

        #endregion

        #region "Buttons"

        /// <summary>
        /// Lazily instantiated collection of buttons
        /// for across the bottom of the properties page
        /// </summary>
        public IEnumerable<IButton> Buttons
        {
            get
            {
                if (m_Buttons == null)
                {
                    m_Buttons = new Collection<IButton>();
                    m_Buttons.Add(new OKButton(this));
                    m_Buttons.Add(new ApplyButton(this));
                    m_Buttons.Add(new CancelButton(this));
                }
                return m_Buttons;
            }
        }
        private Collection<IButton> m_Buttons = null;

        #region " OK Button "
        private class OKButton : AbstractButton
        {
            public OKButton(RuntimeApplicationProperties doc)
            {
                m_doc = doc;
                Text = Resources.Strings.RuntimeApplication_OkButton_Text;
                IsDefault = true;
                Padding = new Thickness(15,3,15,3);
                Margin = new Thickness(5);
                EnableCondition = doc.Valid;
            }

            private RuntimeApplicationProperties m_doc = null;

            protected override void Run()
            {
                m_doc.Save();
                m_doc.Close();
            }
        }
        #endregion

        #region " Apply Button "
        private class ApplyButton : AbstractButton
        {
            public ApplyButton(RuntimeApplicationProperties doc)
            {
                m_doc = doc;
                Text = Resources.Strings.RuntimeApplication_ApplyButton_Text;
                Padding = new Thickness(15, 3, 15, 3);
                Margin = new Thickness(5);
                EnableCondition = doc.Valid;
            }

            private RuntimeApplicationProperties m_doc = null;

            protected override void Run()
            {
                m_doc.Save();
            }
        }
        #endregion

        #region " Cancel Button "
        private class CancelButton : AbstractButton
        {
            public CancelButton(RuntimeApplicationProperties doc)
            {
                m_doc = doc;
                Text = Resources.Strings.RuntimeApplication_CancelButton_Text;
                IsCancel = true;
                Padding = new Thickness(15,3,15,3);
                Margin = new Thickness(5);
            }

            private RuntimeApplicationProperties m_doc = null;

            protected override void Run()
            {
                m_doc.Close();
            }
        }
        #endregion

        #endregion "Buttons"


    }
}
