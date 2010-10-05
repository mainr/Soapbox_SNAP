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
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using NLog;

namespace SoapBox.Core
{
    [Export(Services.Logging.LoggingService, typeof(ILoggingService))]
    class NLogLoggingService : ILoggingService 
    {
        Logger log;

        public NLogLoggingService()
		{
            log = LogManager.GetCurrentClassLogger();
		}
		
		public void Debug(object message)
		{
			log.Debug(message);
		}

        public void DebugWithFormat(string format, params object[] args)
		{
            if (args.Length == 0)
            {
                log.Debug(format);
            }
            else
            {
                Debug(string.Format(format, args));
            }
		}
		
		public void Info(object message)
		{
			log.Info(message);
		}

        public void InfoWithFormat(string format, params object[] args)
		{
            if (args.Length == 0)
            {
                log.Info(format);
            }
            else
            {
                Info(string.Format(format, args));
            }
		}
		
		public void Warn(object message)
		{
			log.Warn(message);
		}
		
		public void Warn(object message, Exception exception)
		{
            log.WarnException(message.ToString(), exception);
		}

        public void WarnWithFormat(string format, params object[] args)
		{
            if (args.Length == 0)
            {
                log.Warn(format);
            }
            else
            {
                log.Warn(string.Format(format, args));
            }
		}
		
		public void Error(object message)
		{
			log.Error(message);
		}
		
		public void Error(object message, Exception exception)
		{
			log.ErrorException(message.ToString(), exception);
		}

        public void ErrorWithFormat(string format, params object[] args)
		{
            if (args.Length == 0)
            {
                log.Error(format);
            }
            else
            {
                log.Error(string.Format(format, args));
            }
		}
		
		public void Fatal(object message)
		{
			log.Fatal(message);
		}
		
		public void Fatal(object message, Exception exception)
		{
			log.FatalException(message.ToString(), exception);
		}

        public void FatalWithFormat(string format, params object[] args)
		{
            if (args.Length == 0)
            {
                log.Fatal(format);
            }
            else
            {
                log.Fatal(string.Format(format, args));
            }
		}
		
		public bool IsDebugEnabled {
			get {
				return log.IsDebugEnabled;
			}
		}
		
		public bool IsInfoEnabled {
			get {
				return log.IsInfoEnabled;
			}
		}
		
		public bool IsWarnEnabled {
			get {
				return log.IsWarnEnabled;
			}
		}
		
		public bool IsErrorEnabled {
			get {
				return log.IsErrorEnabled;
			}
		}
		
		public bool IsFatalEnabled {
			get {
				return log.IsFatalEnabled;
			}
		}
    }
}
