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
using SoapBox.Protocol.Base;
using System.Threading;
using System.ComponentModel.Composition;
using SoapBox.Core;
using System.IO;
using SoapBox.Protocol.Automation;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SoapBox.Snap.Runtime
{
    [Export(SoapBox.Snap.Runtime.CompositionPoints.Runtime.Engine, typeof(IEngine))]
    public class Engine : IEngine, IPartImportsSatisfiedNotification
    {
        public const string PEER_CODE = "SoapBox_Snap_Engine";
        public string COMMON_APPLICATION_DATA_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\SoapBox Automation\\SoapBox Snap";
        public const string RUNTIME_APPLICATION_FILENAME = "SavedRuntimeApplication.sna";
        public const string SIGNAL_STATE_FILENAME = "SavedSignalState.sns";
        public const int ENGINE_SLEEP_TIME = 1; // ms

        private readonly FieldBase64 m_EmptyResponse = new FieldBase64();
        private readonly FieldBase64 m_FalseResponse = FieldBase64.Encode(false.ToString());
        private readonly FieldBase64 m_TrueResponse = FieldBase64.Encode(true.ToString());

        // All communication comes through here.
        private readonly NodePeer peer = CommunicationManager.RegisterLocalPeer(
                new FieldIdentifier(PEER_CODE));

        [Import(SoapBox.Core.Services.Logging.LoggingService, typeof(ILoggingService))]
        private ILoggingService logger { get; set; }

        [ImportMany(SoapBox.Snap.Runtime.ExtensionPoints.Runtime.Snap.Drivers, typeof(IRuntimeDriver))]
        private IEnumerable<IRuntimeDriver> drivers { get; set; }

        [ImportMany(SoapBox.Snap.Runtime.ExtensionPoints.Runtime.Snap.GroupExecutors, typeof(IGroupExecutor))]
        private IEnumerable<Lazy<IGroupExecutor, IGroupExecutorMeta>> groupExecutorsImported { get; set; }

        private Dictionary<string, IGroupExecutor> m_groupExecutors = new Dictionary<string, IGroupExecutor>();

        public void OnImportsSatisfied()
        {
            foreach (IRuntimeDriver drv in drivers)
            {
                logger.Info("Loaded driver: " + drv.ID);
                drv.Start();
            }
        }

        #region "lastMessage"

        private NodeBase lastMessage
        {
            get
            {
                NodeBase retVal;
                lock (m_lastMessage_Lock)
                {
                    retVal = m_lastMessage;
                }
                return retVal;
            }
            set
            {
                lock (m_lastMessage_Lock)
                {
                    if (m_lastMessage != value)
                    {
                        m_lastMessage = value;
                    }
                }
            }
        }
        private NodeBase m_lastMessage = null;
        private object m_lastMessage_Lock = new object();

        #endregion

        #region "runtimeApplication"

        private NodeRuntimeApplication runtimeApplication
        {
            get
            {
                NodeRuntimeApplication retVal;
                lock (m_runtimeApplication_Lock)
                {
                    retVal = m_runtimeApplication;
                }
                return retVal;
            }
            set
            {
                lock (m_runtimeApplication_Lock)
                {
                    // We have to transfer any mutable state from the old one to the new one.
                    // It's very important that we do this "atomically", so we are doing
                    // this inside of the lock block.
                    var old = m_runtimeApplication;
                    m_runtimeApplication = value;
                    if (old != null && m_runtimeApplication != null)
                    {
                        // these are immutable (except for the value itself), so we can do it in parallel
                        Parallel.ForEach(m_runtimeApplication.Signals, sig =>
                        {
                            var oldSig = old.FindSignal(sig.SignalId);
                            if (oldSig != null && oldSig != sig)
                            {
                                sig.Value = oldSig.Value;
                            }
                        });
                    }
                }
            }
        }
        private NodeRuntimeApplication m_runtimeApplication = null;
        private object m_runtimeApplication_Lock = new object();

        #endregion

        #region "run"

        /// <summary>
        /// Threadsafe run flag.  This gets set to true when starting,
        /// and cleared when we want to stop the engine.
        /// </summary>
        private bool run
        {
            get
            {
                bool retVal;
                lock (m_run_Lock)
                {
                    retVal = m_run;
                }
                return retVal;
            }
            set
            {
                lock (m_run_Lock)
                {
                    m_run = value;
                }
            }
        }
        private bool m_run = false;
        private object m_run_Lock = new object();

        #endregion

        public Engine()
        {
            peer.OnReceiveDelta += new NodePeer.ReceiveDeltaHandler(
                delegate(NodePeer fromPeer, FieldGuid basedOnMessageID)
                {
                    // This is responsible for returning the message
                    // the delta is based on so that the communication
                    // manager can reconstruct the entire message.
                    // WATCH OUT - RUNS ON A SEPARATE THREAD
                    if (basedOnMessageID == runtimeApplication.ID)
                    {
                        return runtimeApplication;
                    }
                    else
                    {
                        return lastMessage;
                    }
                });

            peer.OnReceive += new NodePeer.ReceiveHandler(
                delegate(NodePeer fromPeer, NodeBase message)
                {
                    // New message has arrived.
                    // WATCH OUT - RUNS ON A SEPARATE THREAD
                    lastMessage = message;
                    var runtimeApp = message as NodeRuntimeApplication;
                    var telegram = message as NodeTelegram;
                    if (runtimeApp != null)
                    {
                        // peer is sending a new version of the 
                        // runtime application - store it and we'll
                        // "see" it on the next logic scan.
                        runtimeApplication = runtimeApp;
                    }
                    else if (telegram != null)
                    {
                        try
                        {
                            var toPeer = fromPeer;
                            var telegramType = (TelegramType)Enum.Parse(typeof(TelegramType), telegram.MessageType.ToString());
                            switch (telegramType)
                            {
                                case TelegramType.READCONFIGURATION:
                                    // peer is requesting an I/O configuration
                                    // so do a scan and send one back
                                    peer.SendToPeer(toPeer, getDeviceConfiguration());
                                    break;
                                case TelegramType.START:
                                    Start();
                                    peer.SendToPeer(toPeer, telegram.SetPayload(m_TrueResponse));
                                    break;
                                case TelegramType.STOP:
                                    Stop();
                                    peer.SendToPeer(toPeer, telegram.SetPayload(m_TrueResponse));
                                    break;
                                case TelegramType.RUNNING:
                                    var response = m_FalseResponse;
                                    if (run)
                                    {
                                        response = m_TrueResponse;
                                    }
                                    peer.SendToPeer(toPeer, telegram.SetPayload(response));
                                    break;
                                case TelegramType.RUNTIMEID:
                                    FieldBase64 runtimeIdResponse = m_EmptyResponse;
                                    if (runtimeApplication != null)
                                    {
                                        runtimeIdResponse = FieldBase64.Encode(runtimeApplication.RuntimeId.ToString());
                                    }
                                    peer.SendToPeer(toPeer, telegram.SetPayload(runtimeIdResponse));
                                    break;
                                case TelegramType.RUNTIMEVERSIONID:
                                    FieldBase64 runtimeVersionIdResponse = m_EmptyResponse;
                                    if (runtimeApplication != null)
                                    {
                                        runtimeVersionIdResponse = FieldBase64.Encode(runtimeApplication.ID.ToString());
                                    }
                                    peer.SendToPeer(toPeer, telegram.SetPayload(runtimeVersionIdResponse));
                                    break;
                                case TelegramType.UPLOAD:
                                    peer.SendToPeer(toPeer,runtimeApplication);
                                    break;
                                case TelegramType.READSIGNALS:
                                    peer.SendToPeer(toPeer, readSignals(telegram));
                                    break;
                            }
                        }
                        catch (ArgumentException)
                        {
                            // means message type wasn't parsed to an enum - shouldn't happen, but whatever
                        }
                    }
                });

            Load();
        }

        private readonly int m_guidLength = (new FieldGuid()).ToString().Length;

        private NodeTelegram readSignals(NodeTelegram request)
        {
            // incoming request should just be a bunch of Guids that represent signalIds
            var inPayload = request.Payload.Decode();
            var outPayload = new StringBuilder();
            if (runtimeApplication != null)
            {
                for (int index = 0; index < inPayload.Length; index += m_guidLength)
                {
                    string oneGuidString = inPayload.Substring(index, m_guidLength);
                    if (FieldGuid.CheckSyntax(oneGuidString))
                    {
                        var signalId = new FieldGuid(oneGuidString);
                        var signal = runtimeApplication.FindSignal(signalId);
                        if (signal != null)
                        {
                            outPayload.Append(EncodedSignalValue.EncodeSignalValue(signal));
                        }
                    }
                }
            }
            return request.SetPayload(FieldBase64.Encode(outPayload.ToString()));
        }

        /// <summary>
        /// Stores the state of all signals into a string that can be saved to a file.
        /// </summary>
        /// <param name="rta">Runtime application to store the state of</param>
        /// <returns>blob</returns>
        private static string signalStateToBlob(NodeRuntimeApplication rta)
        {
            if (rta == null)
            {
                throw new ArgumentNullException("rta");
            }
            var encodedSignals = from signal in rta.Signals select EncodedSignalValue.EncodeSignalValue(signal);
            return string.Join(string.Empty, encodedSignals);
        }

        /// <summary>
        /// Restores the state of all signals from a blob string created by signalStateToBlob
        /// </summary>
        /// <param name="rta">Runtime application to restore to</param>
        /// <param name="blob">blob</param>
        private static void restoreSignalStateFromBlob(NodeRuntimeApplication rta, string blob)
        {
            if (rta == null)
            {
                throw new ArgumentNullException("rta");
            }
            if (blob == null)
            {
                return;
            }
            var signalValues = EncodedSignalValue.ParseEncodedSignals(blob, rta.Signals);
            foreach (var signalTuple in signalValues)
            {
                NodeSignal signal = signalTuple.Item1;
                object value = signalTuple.Item2;
                try
                {
                    signal.Value = value;
                }
                catch
                {
                    // FIXME - tie into error reporting feature, to be developed
                }
            }
        }

        private NodeDeviceConfiguration getDeviceConfiguration()
        {
            var driversMutable = new Collection<NodeDriver>();
            foreach (IRuntimeDriver drv in drivers)
            {
                driversMutable.Add(drv.ReadConfiguration());
            }
            return NodeDeviceConfiguration.BuildWith(
                new ReadOnlyCollection<NodeDriver>(driversMutable));
        }

        #region "Start/Stop Runtime Engine"

        public void Load()
        {
            runtimeApplication = loadRuntimeApplication();
            if (runtimeApplication != null &&
                runtimeApplication.ExecuteOnStartup.BoolValue)
            {
                Start();
            }
        }

        /// <summary>
        /// Starts the runtime as a separate thread.
        /// </summary>
        public void Start()
        {
            if (run)
            {
                // Don't allow the engine to be started
                // more than once.
                return;
            }
            else
            {
                //logger.Info("Starting runtime engine...");
                run = true;
                ThreadPool.QueueUserWorkItem(
                    o => ThreadStart()
                    );
            }
        }
        /// <summary>
        /// Tells the runtime to stop.
        /// </summary>
        public void Stop()
        {
            logger.Info("Stopping runtime engine...");
            run = false;
            logger.Info("Saving runtime engine's copy of the Runtime Application...");
            saveRuntimeApplication(runtimeApplication);
        }

        #endregion

        private void ScanIO(NodeRuntimeApplication rta, Action<IRuntimeDriver, NodeDriver> action)
        {
            foreach (var nDriver in rta.DeviceConfiguration.NodeDriverChildren)
            {
                if (nDriver.Running.BoolValue)
                {
                    var nDriverGuid = new Guid(nDriver.TypeId.ToString());
                    foreach (IRuntimeDriver driver in drivers) // NOT threadsafe: won't support recomposition
                    {
                        if (driver.TypeId == nDriverGuid)
                        {
                            action(driver, nDriver);
                        }
                    }
                }
            }
        }

        public double LastScanTime { get; private set; }

        private void ThreadStart()
        {
            var lastScan = DateTime.Now;

            logger.Info("Runtime engine started.");
            while (run)
            {
                NodeRuntimeApplication runtimeSnapshot = runtimeApplication; // threadsafe getter

                var thisScan = DateTime.Now;
                LastScanTime = thisScan.Subtract(lastScan).TotalMilliseconds;
                lastScan = thisScan;

                if (runtimeSnapshot != null)
                {
                    ScanIO(runtimeSnapshot, (driver, nDriver) => driver.ScanInputs(nDriver));
                    SolveLogic(runtimeSnapshot);
                    ScanIO(runtimeSnapshot, (driver, nDriver) => driver.ScanOutputs(nDriver, runtimeSnapshot));
                }
                Thread.Sleep(ENGINE_SLEEP_TIME);
            }

            logger.Info("Runtime engine stopped.");
        }

        #region SolveLogic
        private void SolveLogic(NodeRuntimeApplication rta)
        {
            ScanPageCollection(rta, rta.Logic);
        }

        private void ScanPageCollection(NodeRuntimeApplication rta, NodePageCollection pageCollection)
        {
            foreach (var childPageCollection in pageCollection.NodePageCollectionChildren)
            {
                ScanPageCollection(rta, childPageCollection);
            }
            foreach (var childPage in pageCollection.NodePageChildren)
            {
                ScanPage(rta, childPage);
            }
        }

        private void ScanPage(NodeRuntimeApplication rta, NodePage page)
        {
            foreach (var instructionGroup in page.NodeInstructionGroupChildren)
            {
                ScanInstructionGroup(rta, instructionGroup);
            }
        }

        private void ScanInstructionGroup(NodeRuntimeApplication rta, NodeInstructionGroup instructionGroup)
        {
            string language = instructionGroup.Language.ToString();
            IGroupExecutor executor = null;
            if (m_groupExecutors.ContainsKey(language))
            {
                executor = m_groupExecutors[language];
            }
            else
            {
                // this only instantiates new language modules if we're actually using that language
                foreach (var executorSearch in groupExecutorsImported)
                {
                    if (executorSearch.Metadata.Language == language)
                    {
                        executor = executorSearch.Value;
                        m_groupExecutors.Add(language, executor);
                        break;
                    }
                }
            }

            if (executor != null)
            {
                executor.ScanInstructionGroup(rta, instructionGroup);
            }
            else
            {
                // FIXME - should report the error - we can't execute it
            }
        }
        #endregion

        private void saveRuntimeApplication(NodeRuntimeApplication runtimeApplicationToSave)
        {
            try
            {
                if (!Directory.Exists(COMMON_APPLICATION_DATA_FOLDER))
                {
                    Directory.CreateDirectory(COMMON_APPLICATION_DATA_FOLDER);
                }

                string fileName = Path.Combine(COMMON_APPLICATION_DATA_FOLDER, RUNTIME_APPLICATION_FILENAME);
                string signalFileName = Path.Combine(COMMON_APPLICATION_DATA_FOLDER, SIGNAL_STATE_FILENAME);
                if (runtimeApplicationToSave != null)
                {
                    byte[] byteArray = Encoding.Unicode.GetBytes(runtimeApplicationToSave.ToXml());
                    File.WriteAllBytes(fileName, byteArray);
                    byte[] signalByteArray = Encoding.Unicode.GetBytes(signalStateToBlob(runtimeApplicationToSave));
                    File.WriteAllBytes(signalFileName, signalByteArray);
                }
                else
                {
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }
                    if (File.Exists(signalFileName))
                    {
                        File.Delete(signalFileName);
                    }
                }
            }
            catch
            {
                //FIXME - tie into error reporting feature, to be written
            }
        }

        private NodeRuntimeApplication loadRuntimeApplication()
        {
            NodeRuntimeApplication readApplication = null;
            try
            {
                string fileName = Path.Combine(COMMON_APPLICATION_DATA_FOLDER, RUNTIME_APPLICATION_FILENAME);
                string xml = readFile(fileName);
                if (xml != null)
                {
                    readApplication = NodeBase.NodeFromXML(xml, null) as NodeRuntimeApplication;
                    if (readApplication != null)
                    {
                        string signalFileName = Path.Combine(COMMON_APPLICATION_DATA_FOLDER, SIGNAL_STATE_FILENAME);
                        string blob = readFile(signalFileName);
                        restoreSignalStateFromBlob(readApplication, blob);
                    }
                }
            }
            catch
            {
                //FIXME - tie into future error reporting mechanism
            }
            return readApplication;
        }

        /// <summary>
        /// Reads the contents of a file
        /// </summary>
        /// <param name="fileName">Fully qualified name of file to read</param>
        /// <returns>file contents, or null if there was an error</returns>
        private string readFile(string fileName)
        {
            string errMsg = "Error reading file " + fileName;
            string retVal = null;
            if (File.Exists(fileName))
            {
                try
                {
                    using (Stream src = File.OpenRead(fileName))
                    {
                        using (StreamReader reader = new StreamReader(src, Encoding.Unicode))
                        {
                            retVal = reader.ReadToEnd();
                        }
                    }
                }
                catch (PathTooLongException ex)
                {
                    logger.Error(errMsg, ex);
                }
                catch (DirectoryNotFoundException ex)
                {
                    logger.Error(errMsg, ex);
                }
                catch (UnauthorizedAccessException ex)
                {
                    logger.Error(errMsg, ex);
                }
                catch (FileNotFoundException ex)
                {
                    logger.Error(errMsg, ex);
                }
            }
            return retVal;
        }
    }
}
