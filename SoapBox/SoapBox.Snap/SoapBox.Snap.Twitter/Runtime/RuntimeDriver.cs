#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// Copyright (C) 2009-2014 SoapBox Automation, All Rights Reserved.
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
using System.Diagnostics;
using System.ComponentModel.Composition;
using SoapBox.Core;
using System.Timers;
using SoapBox.Protocol.Base;
using SoapBox.Protocol.Automation;
using System.Collections.ObjectModel;
using SoapBox.Snap.Runtime;
using TweetSharp.Twitter.Fluent;
using TweetSharp.Twitter.Extensions;
using TweetSharp.Extensions;
using TweetSharp.Twitter.Model;

namespace SoapBox.Snap.Twitter
{
    [Export(SoapBox.Snap.Runtime.ExtensionPoints.Runtime.Snap.Drivers, typeof(IRuntimeDriver))]
    public class RuntimeDriver : AbstractExtension, IRuntimeDriver
    {

        public RuntimeDriver()
        {
            ID = SoapBox.Snap.Twitter.Extensions.Runtime.Snap.Drivers.Twitter;
        }

        #region IRuntimeDriver Members

        public void Start()
        {
            Running = true;
        }

        public void Stop()
        {
            Running = false;
        }

        public bool Running
        {
            get 
            {
                bool retVal;
                lock (m_RunningLock)
                {
                    retVal = m_Running;
                }
                return retVal; 
            }
            private set
            {
                lock (m_RunningLock)
                {
                    m_Running = value;
                }
            }
        }
        private bool m_Running = false;
        private readonly object m_RunningLock = new object();

        // BE CAREFUL!  RUNS ON A SEPARATE THREAD!
        public NodeDriver ReadConfiguration()
        {
            var driver = NodeDriver.BuildWith(
                new FieldGuid(SnapDriver.TWITTER_DRIVER_ID),  // TypeId
                new FieldString(string.Empty),      // Address
                new FieldBase64(string.Empty),     // Configuration
                new FieldString(Resources.Strings.DriverName));// DriverName
            driver = driver.SetRunning(new FieldBool(Running)); // status only
            if (Running) // Running property is threadsafe
            {
                var devicesMutable = new Collection<NodeDevice>();
                driver = driver.NodeDeviceChildren.Append(
                    new ReadOnlyCollection<NodeDevice>(devicesMutable));
            }
            return driver;
        }

        public event EventHandler ConfigurationChanged;

        private void RaiseConfigurationChanged()
        {
            var evt = ConfigurationChanged;
            if (evt != null)
            {
                evt(this, new EventArgs());
            }
        }

        public Guid TypeId
        {
            get
            {
                return m_TypeId;
            }
        }
        private readonly Guid m_TypeId = new Guid(SnapDriver.TWITTER_DRIVER_ID);

        public void ScanInputs(NodeDriver driver)
        {
            foreach (var device in driver.NodeDeviceChildren.Items)
            {
                kickOffBackgroundRequestIfNecessary(device);
                switch (device.TypeId.ToString())
                {
                    case UserStatusMonitor.TYPE_ID:
                        NodeStringInput latestStatus = device.NodeStringInputChildren.Items[0];
                        var statuses = deviceState.GetUserStatusMonitor(device);
                        var oldStatus = latestStatus.Value;
                        if (statuses.Count > 0)
                        {
                            latestStatus.Value = statuses[0].Text;
                        }
                        else
                        {
                            latestStatus.Value = string.Empty;
                        }
                        NodeDiscreteInput statusChanged = device.NodeDiscreteInputChildren.Items[0];
                        statusChanged.Value = oldStatus != latestStatus.Value;
                        break;
                }
            }
        }

        private void kickOffBackgroundRequestIfNecessary(NodeDevice device)
        {
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }

            if (deviceShouldBeRunning(device) && !deviceState.IsRunning(device))
            {
                // start it
                deviceState.SetIsRunning(device, true);
                string[] addressParts = device.Address.ToString().Split(new string[] { AbstractTwitterDevice.ADDRESS_SEPARATOR }, StringSplitOptions.None);
                string token = new FieldBase64(addressParts[0]).Decode();
                string tokenSecret = new FieldBase64(addressParts[1]).Decode();

                var twitter = FluentTwitter.CreateRequest()
                    .AuthenticateWith(TwitterConsumer.ConsumerKey, TwitterConsumer.ConsumerSecret, token, tokenSecret)
                    .Statuses().OnUserTimeline(); 
                twitter.CallbackTo((sender, result, userstate) =>
                {
                    deviceState.SetUserStatusMonitor(device, result.AsStatuses());
                    // Implement some rate limiting
                    long waitSeconds100Percent = 20;
                    if (result.RateLimitStatus.RemainingHits > 0)
                    {
                        long secondsBeforeReset = (long)result.RateLimitStatus.ResetTime.Subtract(DateTime.Now).TotalSeconds;
                        waitSeconds100Percent = secondsBeforeReset / result.RateLimitStatus.RemainingHits;
                    }
                    long waitSecondsMinimum = 20;
                    if (result.RateLimitStatus.HourlyLimit > 0)
                    {
                        waitSecondsMinimum = 3600 / result.RateLimitStatus.HourlyLimit;
                    }
                    long waitSeconds = Math.Max((long)((1/50.Percent()) * waitSeconds100Percent), waitSecondsMinimum); // limits to a certain percentage, with a floor
                    System.Threading.Thread.Sleep((int)(waitSeconds * 1000));
                    deviceState.SetIsRunning(device, false);
                });
                twitter.BeginRequest();
            }
        }

        private bool deviceShouldBeRunning(NodeDevice device)
        {
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }

            return device.Address.ToString() != string.Empty && Running;
        }

        private readonly ThreadSafeDeviceState deviceState = new ThreadSafeDeviceState();

        private class ThreadSafeDeviceState
        {
            #region IsRunning
            private readonly Dictionary<NodeDevice, bool> m_IsRunning = new Dictionary<NodeDevice, bool>();

            public bool IsRunning(NodeDevice device)
            {
                if (device == null)
                {
                    throw new ArgumentNullException("device");
                }

                bool retVal;
                lock (this)
                {
                    if (!m_IsRunning.ContainsKey(device))
                    {
                        SetIsRunning(device, false);
                    }
                    retVal = m_IsRunning[device];
                }
                return retVal;
            }

            public void SetIsRunning(NodeDevice device, bool value)
            {
                if (device == null)
                {
                    throw new ArgumentNullException("device");
                }

                lock (this)
                {
                    if (!m_IsRunning.ContainsKey(device))
                    {
                        m_IsRunning.Add(device, value);
                    }
                    else
                    {
                        m_IsRunning[device] = value;
                    }
                }
            }

            public ReadOnlyCollection<NodeDevice> RunningDevices
            {
                get
                {
                    ReadOnlyCollection<NodeDevice> retVal;
                    lock (this)
                    {
                        IEnumerable<NodeDevice> runningDevices = from k in m_IsRunning.Keys where m_IsRunning[k] select k;
                        retVal = new ReadOnlyCollection<NodeDevice>(runningDevices.ToList());
                    }
                    return retVal;
                }
            }
            #endregion

            #region UserStatusMonitor
            private readonly Dictionary<NodeDevice, IEnumerable<TwitterStatus>> m_UserStatusMonitorResults = 
                new Dictionary<NodeDevice, IEnumerable<TwitterStatus>>();

            public void SetUserStatusMonitor(NodeDevice device, IEnumerable<TwitterStatus> statuses)
            {
                if (device == null)
                {
                    throw new ArgumentNullException("device");
                }

                lock (this)
                {
                    if (!m_UserStatusMonitorResults.ContainsKey(device))
                    {
                        m_UserStatusMonitorResults.Add(device, statuses);
                    }
                    else
                    {
                        m_UserStatusMonitorResults[device] = statuses;
                    }
                }
            }

            public ReadOnlyCollection<TwitterStatus> GetUserStatusMonitor(NodeDevice device)
            {
                if (device == null)
                {
                    throw new ArgumentNullException("device");
                }

                ReadOnlyCollection<TwitterStatus> retVal;
                lock (this)
                {
                    if (m_UserStatusMonitorResults.ContainsKey(device) && m_UserStatusMonitorResults[device] != null)
                    {
                        retVal = new ReadOnlyCollection<TwitterStatus>(m_UserStatusMonitorResults[device].ToList());
                    }
                    else
                    {
                        retVal = new ReadOnlyCollection<TwitterStatus>(new List<TwitterStatus>());
                    }
                }
                return retVal;
            }
            #endregion
        }

        public void ScanOutputs(NodeDriver driver, NodeRuntimeApplication runtimeApplication)
        {
            // No outputs yet
            //foreach (var device in driver.NodeDeviceChildren.Items)
            //{

            //}
        }

        #endregion

    }
}
