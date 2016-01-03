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
using System.Windows.Controls;
using MarkdownSharp;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.IO;

namespace SoapBox.Snap.MarkdownUtility
{
    /// <summary>
    /// Defines an attached property that lets you bind a WebBrowser source
    /// to a string property *and* automatically convert the string from
    /// Markdown to html in the process.  Also makes sure that links in the
    /// document that are clicked on get routed to the default browser.
    /// </summary>
    public class MarkdownBinding
    {

        private static readonly Markdown m_markdown = new Markdown();
        private static bool m_initialized = false;
        private static readonly Collection<WebBrowser> m_webBrowsers = new Collection<WebBrowser>();
        private static readonly string m_style = @"
<style type='text/css'>
body {
    font-family:Verdana,Arial,Sans-serif;
    font-size:0.75em;
}
h1 {
    font-size:1.3em;
}
h2 {
    font-size:1.2em;
}
h3 {
    font-size:1.1em;
}
h4 {
    font-size:1.0em;
}
h5 {
    font-size:0.9em;
}
h6 {
    font-size:0.875em;
}
</style>
";

        // Using a DependencyProperty as the backing store for MarkdownSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarkdownSourceProperty =
            DependencyProperty.RegisterAttached("MarkdownSource", typeof(string), typeof(MarkdownBinding), new UIPropertyMetadata(null,MarkdownSourcePropertyChanged));

        public static string GetMarkdownSource(DependencyObject o)
        {
            return (string)o.GetValue(MarkdownSourceProperty);
        }

        public static void SetMarkdownSource(DependencyObject o, string value)
        {
            o.SetValue(MarkdownSourceProperty, value);
        }

        public static void MarkdownSourcePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (!m_initialized)
            {
                m_markdown.AutoHyperlink = true;
                m_markdown.AutoNewLines = true;
                m_markdown.EmptyElementSuffix = "/>";
                m_markdown.EncodeProblemUrlCharacters = true;
                m_initialized = true;
            }
            WebBrowser browser = o as WebBrowser;
            if (browser != null)
            {
                if (!m_webBrowsers.Contains(browser))
                {
                    m_webBrowsers.Add(browser);
                    browser.Navigating += new System.Windows.Navigation.NavigatingCancelEventHandler(browser_Navigating);
                }
                string markdownText = e.NewValue as string;
                browser.NavigateToString("<html><head>" + m_style + "</head><body>" + m_markdown.Transform(markdownText) + "</body></html>");
            }
        }

        /// <summary>
        /// Just opens a page showing Markdown Syntax
        /// </summary>
        public static void NavigateToMarkdownSyntax()
        {
            NavigateToUrl(@"http://daringfireball.net/projects/markdown/syntax");
        }

        private static void NavigateToUrl(string url)
        {
            if (url != null)
            {
                try
                {
                    // Starts by trying to start the default browser
                    System.Diagnostics.Process.Start(url);
                }
                catch (System.Exception)
                {
                    bool success = false;
                    // This time we'll go to the registry and try to find the default browser
                    RegistryKey httpKey = Registry.ClassesRoot.OpenSubKey(@"http\shell\open\command");
                    if (httpKey != null && httpKey.GetValue(string.Empty) != null)
                    {
                        string cmd = httpKey.GetValue(string.Empty) as string;
                        if (cmd != null)
                        {
                            try
                            {
                                if (cmd.Length > 0)
                                {
                                    string[] splitStr;
                                    string fileName;
                                    string args;
                                    if (cmd.Substring(0, 1) == "\"")
                                    {
                                        splitStr = cmd.Split(new string[] { "\" " }, StringSplitOptions.None);
                                        fileName = splitStr[0] + "\"";
                                        args = cmd.Substring(splitStr[0].Length + 2);
                                    }
                                    else
                                    {
                                        splitStr = cmd.Split(new string[] { " " }, StringSplitOptions.None);
                                        fileName = splitStr[0];
                                        args = cmd.Substring(splitStr[0].Length + 1);
                                    }
                                    System.Diagnostics.Process.Start(fileName, args.Replace("%1", url));
                                    success = true;
                                }
                            }
                            catch (Exception)
                            {
                                success = false;
                            }
                        }
                        httpKey.Close();
                    }
                    if (!success)
                    {
                        try
                        {
                            // This is our backup-backup method, which of course only works if IE is where we think it is
                            System.Diagnostics.Process.Start(@"C:\Program Files\Internet Explorer\IExplore.exe", url);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The markdown display controls in our app shouldn't be navigating to 
        /// new pages inside themselves.  They should always open a new browser
        /// window to go to a link.  This event handler cancels the navigation
        /// and attempts to open it in the default browser.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void browser_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (e.Uri != null)
            {
                NavigateToUrl(e.Uri.AbsoluteUri);
                e.Cancel = true;
            }
        }

    }
}
