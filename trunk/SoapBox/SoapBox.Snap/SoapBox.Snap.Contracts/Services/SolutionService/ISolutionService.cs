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
using System.ComponentModel;
using SoapBox.Core;

namespace SoapBox.Snap
{
    public interface ISolutionService
    {
        void NewExecute();
        ICondition NewEnabled { get; }
        void OpenExecute(string openFileName, bool readOnly); // open solution in openFileName as if user had selected it
        void OpenExecute();
        ICondition OpenEnabled { get; }
        void CloseExecute();
        ICondition CloseEnabled { get; }
        void SaveExecute();
        ICondition SaveEnabled { get; }
        void SaveAsExecute();
        ICondition SaveAsEnabled { get; }

        // Other objects (like pages) can tell the solution
        // that they need to be saved.  This enables the 
        // SaveEnabled condition.  They have to remove themselves
        // when they no longer require saving.
        // They listen to the Saving event to do their saving
        void RequiresSave(object sender);
        void NoLongerRequiresSave(object sender);
        event EventHandler Saving;
    }
}
