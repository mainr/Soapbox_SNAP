#region "SoapBox.Core License"
/// <header module="SoapBox.Core"> 
/// Copyright (C) 2009 SoapBox Automation Inc., All Rights Reserved.
/// Contact: SoapBox Automation Licencing (license@soapboxautomation.com)
/// 
/// This file is part of SoapBox Core.
/// 
/// Commercial Usage
/// Licensees holding valid SoapBox Automation Commercial licenses may use  
/// this file in accordance with the SoapBox Automation Commercial License
/// Agreement provided with the Software or, alternatively, in accordance 
/// with the terms contained in a written agreement between you and
/// SoapBox Automation Inc.
/// 
/// GNU Lesser General Public License Usage
/// SoapBox Core is free software: you can redistribute it and/or modify 
/// it under the terms of the GNU Lesser General Public License
/// as published by the Free Software Foundation, either version 3 of the
/// License, or (at your option) any later version.
/// 
/// SoapBox Core is distributed in the hope that it will be useful, 
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU Lesser General Public License for more details.
/// 
/// You should have received a copy of the GNU Lesser General Public License 
/// along with SoapBox Core. If not, see <http://www.gnu.org/licenses/>.
/// </header>
#endregion
        
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Diagnostics;
using System.Threading;


namespace SoapBox.Core.Host
{

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, IPartImportsSatisfiedNotification
    {
        private CompositionContainer _container;

        /// <summary>
        /// Main WPF startup window for the application
        /// </summary>
        [Import(CompositionPoints.Host.MainWindow, typeof(Window))]
        public new Window MainWindow
        {
            get { return base.MainWindow; }
            set { base.MainWindow = value; }
        }

        /// <summary>
        /// This imports a resource dictionary for a Theme so it's
        /// added to the application resources.
        /// This gets imported before the Styles.
        /// </summary>
        [Import(ExtensionPoints.Host.Theme, typeof(ResourceDictionary), AllowRecomposition = true, AllowDefault=true)]
        private ResourceDictionary Theme { get; set; }

        /// <summary>
        /// This imports resource dictionaries for Styles so they're
        /// all added to the application resources.
        /// These get imported before the Views.
        /// </summary>
        [ImportMany(ExtensionPoints.Host.Styles, typeof(ResourceDictionary), AllowRecomposition = true)]
        private IEnumerable<ResourceDictionary> Styles { get; set; }

        /// <summary>
        /// This imports resource dictionaries for Views so they're
        /// all added to the application resources.
        /// In general these should be full of DataTemplates for 
        /// displaying ViewModel classes.
        /// </summary>
        [ImportMany(ExtensionPoints.Host.Views, typeof(ResourceDictionary), AllowRecomposition=true)]
        private IEnumerable<ResourceDictionary> Views { get; set; }

        /// <summary>
        /// Hosts a logging service
        /// </summary>
        [Import(Services.Logging.LoggingService,typeof(ILoggingService))]
        public ILoggingService logger { get; set; }

        /// <summary>
        /// This imports any commands that are supposed to run when
        /// the application starts.
        /// </summary>
        [ImportMany(ExtensionPoints.Host.StartupCommands, typeof(IExecutableCommand), AllowRecomposition = true)]
        private IEnumerable<IExecutableCommand> StartupCommands { get; set; }

        /// <summary>
        /// This imports any commands that are supposed to run when
        /// the application is shutdown.
        /// </summary>
        [ImportMany(ExtensionPoints.Host.ShutdownCommands, typeof(IExecutableCommand), AllowRecomposition = true)]
        private IEnumerable<IExecutableCommand> ShutdownCommands { get; set; }

        /// <summary>
        /// This imports things that just want to be part of the composition.
        /// </summary>
        [ImportMany(ExtensionPoints.Host.Void, typeof(Object), AllowRecomposition = true)]
        private IEnumerable<Object> VoidObjects { get; set; }

        /// <summary>
        /// We need this to sort ordered extentions, like the startup commands
        /// </summary>
        [Import(Services.Host.ExtensionService)]
        private IExtensionService ExtensionService { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            // DON'T USE LOGGER HERE.  It's not composed yet.
            base.OnStartup(e);

            ArgumentsService.SetArgs(e.Args);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            if (Compose())
            {
                stopWatch.Stop();

                // Now we can use logger
                logger.InfoWithFormat("Composition complete...({0} milliseconds)", stopWatch.ElapsedMilliseconds);

                logger.Info("Showing Main Window...");
                MainWindow.Show();
            }
            else
            {
                Shutdown();
            }
        }
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            // Run all the shutdown commands
            IList<IExecutableCommand> commands = ExtensionService.Sort(ShutdownCommands);
            foreach (IExecutableCommand cmd in commands)
            {
                logger.Info("Running shutdown command " + cmd.ID + "...");
                try
                {
                    cmd.Run();
                }
                catch (Exception ex)
                {
                    logger.Error("Exception while running command " + cmd.ID, ex);
                }
                logger.Info("Shutdown command " + cmd.ID + " completed.");
            }

            Thread.Sleep(250); // Give threads and other parts of the app a bit of time to
                               // end gracefully.

            if (_container != null)
            {
                _container.Dispose();
            }
        }

        /// <summary>
        /// Don't use logger in here!  It's not composed yet.
        /// </summary>
        /// <returns>True if successful</returns>
        private bool Compose()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog("."));
            catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));

            _container = new CompositionContainer(catalog);

            try
            {
                _container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                MessageBox.Show(compositionException.ToString());
                return false;
            }
            return true;
        }

        private bool m_startupCommandsRun = false;

        public void OnImportsSatisfied()
        {            
            // Add the imported resource dictionaries
            // to the application resources
            this.Resources.MergedDictionaries.Clear(); // in case of recompose
            if (Theme != null) // Theme is optional
            {
                logger.Info("Importing Theme...");
                this.Resources.MergedDictionaries.Add(Theme);
            }
            logger.Info("Importing Styles...");
            foreach (ResourceDictionary r in Styles)
            {
                this.Resources.MergedDictionaries.Add(r);
            }
            logger.Info("Importing Views...");
            foreach (ResourceDictionary r in Views)
            {
                this.Resources.MergedDictionaries.Add(r);
            }

            if (!m_startupCommandsRun) // Don't run on recomposition
            {
                m_startupCommandsRun = true;
                // Run all the startup commands
                IList<IExecutableCommand> commands = ExtensionService.Sort(StartupCommands);
                foreach (IExecutableCommand cmd in commands)
                {
                    logger.Info("Running startup command " + cmd.ID + "...");
                    try
                    {
                        cmd.Run();
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Exception while running command " + cmd.ID, ex);
                    } 
                    logger.Info("Startup command " + cmd.ID + " completed.");
                }
            }
        }


    }
}
