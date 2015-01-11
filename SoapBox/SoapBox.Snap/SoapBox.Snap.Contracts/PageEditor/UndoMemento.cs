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

namespace SoapBox.Snap
{
    public enum ActionType
    {
        NO_ACTION,
        ADD,
        EDIT,
        DELETE
    }

    public class UndoMemento : IUndoMemento 
    {
        public UndoMemento(IUndoRedo sender, ActionType actionType, string description)
        {
            if (sender == null || description == null)
            {
                throw new ArgumentNullException();
            }
            m_Sender = sender;
            m_ActionType = actionType;
            m_Description = description;
        }

        public UndoMemento(IUndoRedo sender, ActionType actionType, string description,
            IEnumerable<Action> undoActions,
            IEnumerable<Action> redoActions)
        {
            if (sender == null || description == null || undoActions == null || redoActions == null)
            {
                throw new ArgumentNullException();
            }
            m_Sender = sender;
            m_ActionType = actionType;
            m_Description = description;
            m_UndoActions = new List<Action>(undoActions);
            m_RedoActions = new List<Action>(redoActions);
        }

        public UndoMemento(IUndoRedo sender, ActionType actionType, string description,
            IEnumerable<Action> undoActions,
            IEnumerable<Action> redoActions, bool bindWithNext)
        {
            if (sender == null || description == null || undoActions == null || redoActions == null)
            {
                throw new ArgumentNullException();
            }
            m_Sender = sender;
            m_ActionType = actionType;
            m_Description = description;
            m_UndoActions = new List<Action>(undoActions);
            m_RedoActions = new List<Action>(redoActions);
            m_BindWithNext = bindWithNext;
        }

        #region " Sender "
        public IUndoRedo Sender
        {
            get
            {
                return m_Sender;
            }
        }
        private readonly IUndoRedo m_Sender = null;
        #endregion

        #region " ActionType "
        public ActionType ActionType
        {
            get
            {
                return m_ActionType;
            }
        }
        private readonly ActionType m_ActionType = ActionType.NO_ACTION;
        #endregion

        #region " UndoActions "
        public IList<Action> UndoActions
        {
            get
            {
                return m_UndoActions;
            }
        }
        private readonly List<Action> m_UndoActions = new List<Action>();
        #endregion

        #region " RedoActions "
        public IList<Action> RedoActions
        {
            get
            {
                return m_RedoActions;
            }
        }
        private readonly List<Action> m_RedoActions = new List<Action>();
        #endregion

        #region " Description "
        public string Description
        {
            get
            {
                return m_Description;
            }
        }
        private readonly string m_Description = string.Empty;
        #endregion

        #region " BindWithNext "
        public bool BindWithNext
        {
            get
            {
                return m_BindWithNext;
            }
        }
        private readonly bool m_BindWithNext = false;
        #endregion

        #region " BindWithPrevious "
        public bool BindWithPrevious
        {
            get
            {
                return m_BindWithPrevious;
            }
            set
            {
                m_BindWithPrevious = value;
            }
        }
        private bool m_BindWithPrevious = false;
        #endregion

    }
}
