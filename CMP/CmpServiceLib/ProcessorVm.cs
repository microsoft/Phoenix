using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using AzureAdminClientLib;
using CmpInterfaceModel;
using CmpInterfaceModel.Models;
using CmpServiceLib.Models;
using System.Threading;
using SMAApi;
using SMAApi.Entities;
//using SMAApi.Interface;
using Role = CmpInterfaceModel.Models.Role;
using VmDeploymentRequest = CmpServiceLib.Models.VmDeploymentRequest;
//using CmpServiceLib.MaintenanceModeClient;
using CmpServiceLib.Stages;

namespace CmpServiceLib
{
    //**********************************************************************
    ///
    /// <summary>
    /// 
    /// </summary>
    ///         
    //*********************************************************************

    public class ProcessorVm : IDisposable
    {
        enum PostProvInintDisksResultEnum { Success, FailToConnect, NotFound }

        private const string AZUREHSBODYSHELL =
            @"<?xml version='1.0' encoding='utf-8'?><CreateHostedService xmlns='http://schemas.microsoft.com/windowsazure'>{0}</CreateHostedService>";
        private const string AZURESERVCERTBODYSHELL =
            @"<?xml version='1.0' encoding='utf-8'?><CertificateFile xmlns='http://schemas.microsoft.com/windowsazure'>{0}</CertificateFile>";
        private const string REBOOT_EXIT_TRAP_MATCH = "thread exit or an application request";
        private const string _DefaultVhdContainerName = "VHDS";
        //*** IT Managed group policy admin name must be 'ITSVC0' ***
        private const string _DefaultocalAdminUserName = "ITSVC0";
        private const bool _DomainJoinSecondTry = false;
        private const int HostedServiceCreationDwellTime = 2000;
        private const int _defaultTargetServiceProviderAccountID = 0;
        private const int _DeploymentCountLimit = 48;
        private const int _ContactingVmMinutesTTL = 180;
        //*** Increased from 4 retry to 6 retries
        private const int _CurrentStateTryCountLimit = 6;

        private static IList<Stage> Stages { get; set; }
        private static string _CmpDbConnectionString = null;
        private static string _AftsDbConnectionString = null;
        protected static EventLog _EventLog = null;

        private const int _diskCreationRetryLimit = 12;
        private const int _diskCreationRetryDwellTime = 10;
        private const int _DeleteDwelltimeMinutes = 2;
        private const int _PlacementDwelltime = 30000;
        private const int _PlacementBusyDwelltime = 30000;
        private string _autoLocalAdminUserName = null;
        private string _blobsPerContainerLimit_t = null;
        private int _blobsPerContainerLimit = 45;
        private int _SmaRetryTimeoutMinutes = 15;
        private string _vmsPerVnetLimit_t = null;
        private int _vmsPerVnetLimit = 180;

        private static string _PlacementLockObject = "x";
        private static StorageOps.ContainerSpaceInfo _csi = null;
        private static IEnumerable<StorageOps.ContainerSpaceInfo> _containerSpaceList;

        private bool _isMsitDeployment = false;
        private bool _performValidation = false;
        private bool _disableSmartCardAuth = false;
        private bool _movePagefile = false;
        private bool _makeIpStatic = false;
        private bool _provisionDisks = false;
        private bool _addUsersToGroups = false;
        private bool _processSequences = false;

        bool _disposed = false;

        #region --- Setup Methods --------------------------------------------

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="eLog"></param>
        /// <param name="eLog"></param>
        ///  
        //*********************************************************************
        public ProcessorVm(EventLog eLog)
        {
            InitVals(eLog, null, null);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eLog"></param>
        /// <param name="cmpDbConnectionString"></param>
        /// <param name="aftsDbConnectionString"></param>\
        /// 
        //*********************************************************************

        public ProcessorVm(EventLog eLog, string cmpDbConnectionString, 
            string aftsDbConnectionString)
        {
            InitVals(eLog, cmpDbConnectionString, aftsDbConnectionString);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        bool GetSettingBoolValue(string settingName)
        {
            var value_t =
                Microsoft.Azure.CloudConfigurationManager.GetSetting(settingName) as string;

            if (null == value_t)
                return false;

            return value_t.ToUpper().Contains("T");
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eLog"></param>
        /// <param name="cmpDbConnectionString"></param>
        /// <param name="aftsDbConnectionString"></param>
        /// 
        //*********************************************************************

        private void InitVals(EventLog eLog, string cmpDbConnectionString, 
            string aftsDbConnectionString)
        {
            _EventLog = eLog;
            Stages = new List<Stage>();

            if (null == _csi)
                _csi = new StorageOps.ContainerSpaceInfo();

            _autoLocalAdminUserName =
                Microsoft.Azure.CloudConfigurationManager.GetSetting("AUTOLOCALADMINUSERNAME");

            //***

            _blobsPerContainerLimit_t =
                Microsoft.Azure.CloudConfigurationManager.GetSetting("BLOBSPERCONTAINERLIMIT");

            if (null != _blobsPerContainerLimit_t)
                if (!int.TryParse(_blobsPerContainerLimit_t, out _blobsPerContainerLimit))
                    throw new Exception("Non integer value specified for BLOBSPERCONTAINERLIMIT in app.config");

            //***

            _vmsPerVnetLimit_t =
                Microsoft.Azure.CloudConfigurationManager.GetSetting("VMSPERVNETLIMIT");

            if (null != _vmsPerVnetLimit_t)
                if (!int.TryParse(_vmsPerVnetLimit_t, out _vmsPerVnetLimit))
                    throw new Exception("Non integer value specified for VMSPERVNETLIMIT in app.config");

            //***

            //_isMsitDeployment = GetSettingBoolValue("MsitDeployment");
            _performValidation = GetSettingBoolValue("PerformValidation");
            _disableSmartCardAuth = GetSettingBoolValue("DisableSmartCardAuth");
            _movePagefile = GetSettingBoolValue("MovePagefile");
            _makeIpStatic = GetSettingBoolValue("MakeIpStatic");
            _provisionDisks = GetSettingBoolValue("ProvisionDisks");
            _addUsersToGroups = GetSettingBoolValue("AddUsersToGroups");
            _processSequences = GetSettingBoolValue("ProcessSequences");

            _CmpDbConnectionString = cmpDbConnectionString;
            _AftsDbConnectionString = aftsDbConnectionString;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        /// 
        //*********************************************************************

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
            }

            // Free any unmanaged objects here. 
            //
            _disposed = true;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmpDbConnectionString"></param>
        /// <param name="aftsDbConnectionString"></param>
        /// 
        //*********************************************************************

        protected virtual void SetStages(string cmpDbConnectionString, string aftsDbConnectionString)
        {
            var stages = new List<Stage>
            {
                new VmSubmissionsStage
                {
                    CmpDbConnectionString = cmpDbConnectionString,
                    EventLog = _EventLog,
                    ReadConfigValues = ReadConfigValues,
                },
                new QcVmRequestSubmissionsStage
                {
                    CmpDbConnectionString = cmpDbConnectionString,
                    EventLog = _EventLog,
                },
                new ConvertedSubmissionsStage
                {
                    AftsDbConnectionString = aftsDbConnectionString,
                    BlobsPerContainerLimit = _blobsPerContainerLimit,
                    CmpDbConnectionString = cmpDbConnectionString,
                    DefaultVhdContainerName = _DefaultVhdContainerName,
                    EnforceAppAgAffinity = _EnforceAppAgAffinity,
                    EventLog = _EventLog,
                    GetHostService = GetHostService,
                    GetSafeHostServiceName = GetSafeHostServiceName,
                    GetTargetServiceProviderAccountResourceGroup = GetTargetServiceProviderAccountResourceGroup,
                    VmsPerVnetLimit = _vmsPerVnetLimit,
                },
                new ReadyForTransferSubmissionsStage
                {
                    AftsDbConnectionString = aftsDbConnectionString,
                    CmpDbConnectionString = cmpDbConnectionString,
                    EventLog = _EventLog,
                },
                new TransferringSubmissionsStage
                {
                    AftsDbConnectionString = aftsDbConnectionString,
                    CmpDbConnectionString = cmpDbConnectionString,
                    EventLog = _EventLog,
                },
                new TransferredSubmissionsStage
                {
                    BuildMigratedVmConfigString = BuildMigratedVmConfigString,
                    CmpDbConnectionString = cmpDbConnectionString,
                },
                new QcVmRequestsStage
                {
                    AftsDbConnectionString = aftsDbConnectionString,
                    CmpDbConnectionString = cmpDbConnectionString,
                    DefaultCoreSafetyStockValue = DefaultCoreSafetyStockValue,
                    EventLog = _EventLog,
                    GetTargetServiceProviderAccountResourceGroup = GetTargetServiceProviderAccountResourceGroup,
                    PerformValidation = _performValidation,
                    ServToInt = ServToInt,
                },
                new QcVmRequestPassedStage
                {
                    BuildAzureHsRequestBody = BuildAzureHsRequestBody,
                    CheckServiceNameAvailability = CheckServiceNameAvailability,
                    CmpDbConnectionString = cmpDbConnectionString,
                    EventLog = _EventLog,
                    GetTargetServiceProviderAccountResourceGroup = GetTargetServiceProviderAccountResourceGroup,
                    HostedServiceCreationDwellTime = HostedServiceCreationDwellTime,
                    PlacementBusyDwelltime = _PlacementBusyDwelltime,
                    PlacementDwelltime = _PlacementDwelltime,
                    SetPlacement = SetPlacement,
                },
                new UploadServiceCertificateStage
                {
                    CmpDbConnectionString = cmpDbConnectionString,
                    EventLog = _EventLog,
                    HostedServiceCreationDwellTime = HostedServiceCreationDwellTime,
                },
                new CreateVmStage
                {
                    CmpDbConnectionString = cmpDbConnectionString,
                    CreateVmMinutesTtl = CreateVmMinutesTtl,
                    DeleteDwelltimeMinutes = _DeleteDwelltimeMinutes,
                    DeleteVm = DeleteVm,
                    EventLog = _EventLog,
                    HasTimedOut = HasTimedOut,
                    NonNull = NonNull,
                    ResubmitRequest = ResubmitRequest,
                },
                new CheckVmCreationStage
                {
                    CmpDbConnectionString = cmpDbConnectionString,
                    ContactingVmMinutesTTL = _ContactingVmMinutesTTL,
                    DeleteDwelltimeMinutes = _DeleteDwelltimeMinutes,
                    DeleteVm = DeleteVm,
                    EventLog = _EventLog,
                    GetOsFamily = GetOsFamily,
                    HasTimedOut = HasTimedOut,
                    MarkBadAsset = MarkBadAsset,
                    ResubmitRequest = ResubmitRequest,
                    CompleteVmRequest = CompleteVmRequest
                },
                new ContactVmStage
                {
                    AftsDbConnectionString = aftsDbConnectionString,
                    CmpDbConnectionString = cmpDbConnectionString,
                    ContactingVmMinutesTTL = _ContactingVmMinutesTTL,
                    DeleteDwelltimeMinutes = _DeleteDwelltimeMinutes,
                    DeleteVm = DeleteVm,
                    DisableSmartCardAuth = _disableSmartCardAuth,
                    EventLog = _EventLog,
                    HasTimedOut = HasTimedOut,
                    ResubmitRequest = ResubmitRequest,
                    TestPsConnection = TestPsConnection,
                },
                new MovePagefileStage
                {
                    CmpDbConnectionString = cmpDbConnectionString,
                    DisableSmartCardAuth = _disableSmartCardAuth,
                    DwellTime = DwellTime,
                    EventLog = _EventLog,
                    GetPowershellConnection = GetPowershellConnection,
                    IsMsitDeployment = _isMsitDeployment,
                    MakeIpStatic = _makeIpStatic,
                    MovePagefile = _movePagefile,
                    REBOOT_EXIT_TRAP_MATCH = REBOOT_EXIT_TRAP_MATCH,
                },
                new WaitForRebootStage
                {
                    CmpDbConnectionString = cmpDbConnectionString,
                    ContactingVmMinutesTTL = _ContactingVmMinutesTTL,
                    DeleteDwelltimeMinutes = _DeleteDwelltimeMinutes,
                    DeleteVm = DeleteVm,
                    EventLog = _EventLog,
                    GetPowershellConnection = GetPowershellConnection,
                    HasTimedOut = HasTimedOut,
                    ResubmitRequest = ResubmitRequest,
                },
                new CreateDrivesStage
                {
                    AddUsersToGroups = _addUsersToGroups,
                    ADDUSERTOGROUPTEMPLATE = _ADDUSERTOGROUPTEMPLATE,
                    CmpDbConnectionString = cmpDbConnectionString,
                    DefaultRebootDwellTimeMinutes = DefaultRebootDwellTimeMinutes,
                    EventLog = _EventLog,
                    GetPowershellConnection = GetPowershellConnection,
                    MovePagefile = _movePagefile,
                    ProvisionDisks = _provisionDisks,
                },
                new StartSequencesStage
                {
                    CmpDbConnectionString = cmpDbConnectionString,
                    EventLog = _EventLog,
                    ExecuteSequences = ExecuteSequences,
                    ProcessSequences = _processSequences,
                },
                new RunningSequencesStage
                {
                    CheckSequencesStatus = CheckSequencesStatus,
                    CmpDbConnectionString = cmpDbConnectionString,
                    EventLog = _EventLog,
                    ProcessSequences = _processSequences,
                },
                new PostProcessingStage
                {
                    //ClearMaintenanceMode = ClearMaintenanceMode,
                    CmpDbConnectionString = cmpDbConnectionString,
                    CompleteVmRequest = CompleteVmRequest,
                    DwellTime = DwellTime,
                    EventLog = _EventLog,
                    GetOsFamily = GetOsFamily,
                    GetPowershellConnection = GetPowershellConnection,
                    IsMsitDeployment = _isMsitDeployment,
                    PerformValidation = _performValidation,
                    PostQcValidation = PostQcValidation,
                    REBOOT_EXIT_TRAP_MATCH = REBOOT_EXIT_TRAP_MATCH,
                },
            };
            Stages = stages;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stage"></param>
        /// 
        //*********************************************************************

        protected static void Add(Stage stage)
        {
            var parentType = stage.GetType().BaseType;
            var parent = Stages.FirstOrDefault(s => s.GetType() == parentType);

            if (parentType != typeof (Stage) && parent != null)
            {
                // Copy properties from parent and then remove it
                foreach (var property in parentType.GetProperties().Where(p => p.GetSetMethod() != null))
                {
                    property.SetValue(stage, property.GetValue(parent));
                }
                Stages.Remove(parent);
            }
            Stages.Add(stage);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stages"></param>
        /// 
        //*********************************************************************

        protected static void Add(IEnumerable<Stage> stages)
        {
            foreach (var stage in stages)
            {
                Add(stage);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmpDbConnectionString"></param>
        /// <param name="aftsDbConnectionString"></param>
        /// 
        //*********************************************************************

        public void ProcessVmRequests(string cmpDbConnectionString, 
            string aftsDbConnectionString)
        {
            try
            {
                SetStages(cmpDbConnectionString, aftsDbConnectionString);

                foreach (var stage in Stages)
                {
                    stage.Execute();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ProcessVmRequests() " + 
                    CmpCommon.Utilities.UnwindExceptionMessages(ex));
            }
        }

        #endregion

        #region --- Async Methods --------------------------------------------

        int _SleepTime = 30 * 1000;
        private int _SynchSleepTime = 12 * 60 * 60 * 1000;
        //private int _SynchSleepTime = 30 * 1000;

        List<Thread> _workerThreadList = null;
        private Thread _synchThread = null;

        delegate object ProcessorDel();

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="proc"></param>
        /// 
        //*********************************************************************

        void AddProcessor(ProcessorDel proc)
        {
            var workerThread = new Thread(Worker) { IsBackground = true };
            workerThread.Start(new ProcessorDel(proc));
            _workerThreadList.Add(workerThread);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        void StartSynchThread()
        {
            //*** If we are ARM only, then we don't need to sync.
            if (!Utilities.HasRdfe)
            {
                Stage.ContainersSynchronized = true;
                return;
            }

            //*** We need to sync, which can take a long time. Start the thread.
            _synchThread = new Thread(SynchWorker) { IsBackground=true };
            _synchThread.Start();
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="cmpDbConnectionString"></param>
        /// <param name="aftsDbConnectionString"></param>
        ///  
        //*********************************************************************

        public void ProcessVmRequestsAsynch(string cmpDbConnectionString, 
            string aftsDbConnectionString)
        {
            try
            {
                _CmpDbConnectionString = cmpDbConnectionString;
                _AftsDbConnectionString = aftsDbConnectionString;

                StartSynchThread();

                SetStages(cmpDbConnectionString, aftsDbConnectionString);
                _workerThreadList = new List<Thread>(Stages.Count);

                foreach (var stage in Stages)
                {
                    AddProcessor(stage.Execute);
            }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ProcessVmRequestsAsynch() " +
                    CmpCommon.Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        public void StopProcessingAsynch()
        {
            if (null == _workerThreadList)
                return;

            foreach (var worker in _workerThreadList)
            {
                try
                {
                    if (null != worker)
                        worker.Abort();
                }
                catch (Exception)
                { }
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// 
        //*********************************************************************

        //*** NOTE * Refresh

        public void Worker(object method)
        {
            var proc = method as ProcessorDel;
            object ret;
            var methodName = "x";

            try
            {
                if (null == proc)
                    throw new Exception("proc == null");

                var target = proc.Target as System.Delegate;

                if (null == target)
                    throw new Exception("target == null"); 

                methodName = target.Method.ToString();

                //LogThis(null, EventLogEntryType.Information,
                //    "CmpServiceLib.ProcessorVm.Worker(" + methodName + ") successfully registered", 3, 3);
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error,
                    "Exception in CmpServiceLib.ProcessorVm.Worker() setup", 100, 100);
                return;
            }

            while (true)
            {
                try
                {
                    ret = proc();
                    ret = 1;
                }
                catch (Exception ex)
                {
                    LogThis(ex, EventLogEntryType.Error,
                        "Exception in CmpServiceLib.ProcessorVm.Worker(" + methodName + ")", 100, 100);
                }

                Thread.Sleep(_SleepTime);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        //*** NOTE * Refresh

        public void SynchWorker()
        {
            object ret;

            while (true)
            {
                try
                {
                    Stage.ContainersSynchronized = false;

                    IEnumerable<StorageOps.ContainerSpaceInfo> csl;
                    lock (_PlacementLockObject)
                    {
                        csl = SynchContainerSpaces(true);
                    }

                    if (null == csl)
                        continue;

                    if (!csl.Any())
                        continue;

                    Stage.ContainersSynchronized = true;
                }
                catch (Exception ex)
                {
                    LogThis(ex, EventLogEntryType.Error,
                        "Exception in CmpServiceLib.ProcessorVm.SynchWorker() " +
                        CmpCommon.Utilities.UnwindExceptionMessages(ex), 100, 100);
                }

                Thread.Sleep(_SynchSleepTime);
            }
        }

        #endregion

        #region --- Helper methods --------------------------------------------

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cdb"></param>
        /// 
        //*********************************************************************

        private void ReadConfigValues(CmpDb cdb)
        {
            try
            {
                Stage.SubmittedQueueBlocked = false;
                Stage.AllQueuesBlocked = false;

                var temp = cdb.FetchConfigValue("BlockAllQueues", true);

                if (null != temp)
                    if (temp.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Stage.SubmittedQueueBlocked = true;
                        Stage.AllQueuesBlocked = true;
                    }

                if (!Stage.SubmittedQueueBlocked)
                {
                    temp = cdb.FetchConfigValue("BlockSubmittedQueue", true);

                    if (null != temp)
                        if (temp.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                            Stage.SubmittedQueueBlocked = true;
                }
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error,
                    "Exception in CmpServiceLib.ReadConfigValues()", 1, 1);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmReq"></param>
        /// 
        //*********************************************************************

        private void DeleteVmDisks(VmDeploymentRequest vmReq)
        {
            try
            {
                var vmCfg = CmpInterfaceModel.Models.VmConfig.Deserialize(vmReq.Config);

                if (null == vmReq.ServiceProviderAccountID)
                    return;

                if (null == vmCfg.AzureVmConfig)
                    return;

                if (null == vmCfg.AzureVmConfig.RoleList)
                    return;

                if (0 == vmCfg.AzureVmConfig.RoleList.Count)
                    return;

                var role = vmCfg.AzureVmConfig.RoleList[0] as PersistentVMRole;

                if (null == role)
                    return;

                var cdb = new CmpDb(_CmpDbConnectionString);
                var servProdAccount = cdb.FetchServiceProviderAccount((int)vmReq.ServiceProviderAccountID);
                var connection = ServProvAccount.GetAzureServiceAccountConnection(
                    servProdAccount.AccountID, servProdAccount.CertificateThumbprint,
                    servProdAccount.AzureADTenantId, servProdAccount.AzureADClientId, servProdAccount.AzureADClientKey);
                var dops = new DiskOps(connection);

                var diskNames = new List<string> { role.OSVirtualHardDisk.DiskName };
                diskNames.AddRange(role.DataVirtualHardDisks.Select(dvhd => dvhd.DiskName));

                foreach (var diskName in diskNames)
                {
                    try
                    {
                        dops.DeleteDiskFromSubscription(diskName);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in DeleteVmDisks() : " +
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        public void ExecuteSmaScript(SequenceSpec sequence)
        {
            try
            {
                var rnbookops = new RunBookOperations(sequence.SmaConfig.SmaServerUrl);
                //var runbookId = new Guid(sequence.SmaConfig.RunbookId);
                var rnbookName = sequence.SmaConfig.RunbookName;
                JobStatus runbookJobStatus = null;

                //  The service operation is not seen as IQueryable but just as IEnumerable so we need to cast to Ienumerable
                //var runbooks = rnbookops.GetAllRunbooks() as IEnumerable<SMAApi.Entities.SMARunbook>;

                //  var runBook=runbooks.Where(r=>r.l)
                //var rnbook = runbooks.Where(r => r.Id == runbookId).FirstOrDefault();

                var parameters = sequence.SmaConfig.ParamList.Select(
                    param => new SMAApi.SMAWebService.NameValuePair 
                    {Name = param.Name, Value = param.Value}).ToList();

                var runbookJob = rnbookops.StartRunBook(rnbookName, parameters);
                var now = DateTime.UtcNow;

                sequence.RunResult = new SequenceRunResultSpec
                {
                    JobId = runbookJob.Id.ToString(),
                    StatusCode = SequenceSpec.StatusEnum.Submitted.ToString(),
                    WhenSubmitted = now,
                    LastUpdate = now,
                    StatusMessage = "Submitted to SMA server : " + runbookJob.OutputMessage
                };

                sequence.ResultCode = SequenceSpec.StatusEnum.Submitted.ToString();

                /*if (sequence.Waitmode.Equals(CmpInterfaceModel.Models.SequenceSpec.WaitmodeSpec.Synchronous.ToString(),
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    var startTime = DateTime.Now;

                    while (true)
                    {
                        runbookJobStatus = rnbookops.GetJobStatus(runbookJob.Id);

                        if (null != runbookJobStatus.Output)
                            break;

                        if(0 > startTime.AddHours(1).CompareTo(DateTime.Now))
                            throw new Exception("SMA Invocation Timeout");
                }
                }*/
            }

            catch (Exception ex)
            {
                var now = DateTime.UtcNow;

                sequence.ResultCode = SequenceSpec.StatusEnum.Exception.ToString();

                sequence.RunResult = new SequenceRunResultSpec
                {
                    JobId = "",
                    StatusCode = SequenceSpec.StatusEnum.Exception.ToString(),
                    WhenSubmitted = now,
                    LastUpdate = now,
                    StatusMessage = "Exception : " + Utilities.UnwindExceptionMessages(ex)
                };

                //throw new Exception("Exception in ExecuteSmaScript() : " +
                //    CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex));
            }
        }

        public class SequenceExecutionResults
        {
            public bool HadError = false;
            public bool HadWarning = false;
            public bool HadException = false;
            public bool HadChange = false;
            public List<string> Errorlist = null;
            public List<string> Warninglist = null;
            public List<string> Exceptionlist = null;
            public List<string> Outputlist = null;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmDepReq"></param>
        /// 
        //*********************************************************************

        private SequenceExecutionResults ExecuteSequences(VmDeploymentRequest vmDepReq)
        {
            try
            {
                var hadError = false;
                var ret = new SequenceExecutionResults();

                var vmc = CmpInterfaceModel.Models.VmConfig.Deserialize(vmDepReq.Config);

                if (null == vmc.ScriptList)
                    return ret;

                foreach (var scriptSpec in vmc.ScriptList)
                {
                    /*if (!scriptSpec.ExecuteInState.Equals(vmDepReq.StatusCode,
                        StringComparison.InvariantCultureIgnoreCase))
                        continue;*/

                    if (scriptSpec.Engine.Equals(SequenceSpec.SequenceEngineEnum.SMA.ToString(),
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        ExecuteSmaScript(scriptSpec);
                        vmDepReq.Config = vmc.Serialize();
                    }

                    if (scriptSpec.Engine.Equals(SequenceSpec.SequenceEngineEnum.Powershell.ToString(),
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        using (var psRem = GetPowershellConnection(vmDepReq))
                        {
                            var commandList = new List<string>(1);
                            commandList.AddRange(scriptSpec.ScriptList);

                            var rr = psRem.Execute(null, commandList);

                            var remoteErrorDescriptionList = new List<string>();

                            if (rr.HasErrors)
                            {
                                remoteErrorDescriptionList.AddRange(rr.ErrorDecsriptionList.Select(ed => 
                                    string.Format("Sequence:'{0}' state: '{1}', Error: {2}", 
                                    scriptSpec.Name, vmDepReq.StatusCode, ed)));

                                hadError = true;
                            }
                        }

                    }

                    if (hadError)
                    {
                        if (scriptSpec.BreakOn.Equals(SequenceSpec.BreakOnEnum.Warning.ToString(),
                            StringComparison.InvariantCultureIgnoreCase))
                            throw new Exception(string.Format(
                                "Breaking error in sequence: '{0}' in state: '{1}'",
                                scriptSpec.Name, vmDepReq.StatusCode));
                    }
                }

                return ret;
            }

            catch (Exception ex)
            {
                throw new Exception("Exception in ExecuteSequences() : " +
                    CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmDepReq"></param>
        /// 
        //*********************************************************************

        private bool _smaDown = false;
        private DateTime _smaDownStartTime;

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmDepReq"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private SequenceExecutionResults CheckSequencesStatus(VmDeploymentRequest vmDepReq)
        {
            var ret = new SequenceExecutionResults();

            try
            {
                var hadError = false;
                var vmc = CmpInterfaceModel.Models.VmConfig.Deserialize(vmDepReq.Config);

                if (null == vmc.ScriptList)
                    return ret;

                foreach (var scriptSpec in vmc.ScriptList)
                {
                    string jobErrorMessage = null;  // IPAK Job Error Message

                    if (scriptSpec.Engine.Equals(SequenceSpec.SequenceEngineEnum.SMA.ToString(),
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        //ExecuteSmaScript(scriptSpec);

                        var now = DateTime.UtcNow;

                        var rnbookops = new RunBookOperations(scriptSpec.SmaConfig.SmaServerUrl);

                        var runbookJobStatus = rnbookops.GetJobStatus(new Guid(scriptSpec.RunResult.JobId));

                        /*if (null != runbookJobStatus.Output)
                            break;

                        if (0 > scriptSpec.RunResult.WhenSubmitted.AddHours(12).CompareTo(now))
                            throw new Exception("SMA Runtime Timeout");*/

                        //*** If status didn't change, don't update anything ***
                        //***(addsa) If Job has not completed, continue the process***
                        string[] _jobStatuses = {"Running", "New", "Activating", "Queued"};
                        if ((runbookJobStatus.Status.Equals(
                          scriptSpec.ResultCode, StringComparison.InvariantCultureIgnoreCase)) || 
                          (Array.Find(_jobStatuses, element => element == runbookJobStatus.Status) != null))
                            continue;

                        //*** Status changed, update things ***

                        ret.HadChange = true;

                        //addsa: Parse runbook job output ro get JSON output. (Method definition in SMA Lib)
                        var runbookJobOutput = rnbookops.GetJSONJobOutput(new Guid(scriptSpec.RunResult.JobId));

                        if (null == scriptSpec.RunResult.Output)
                            scriptSpec.RunResult.Output = new List<SequenceRunOuputSpec>(1);

                        if (null != runbookJobStatus.Exception)
                        {
                            hadError = true;
                            scriptSpec.ResultCode = "Exception";
                            scriptSpec.RunResult.StatusCode = "Exception";
                            jobErrorMessage = runbookJobStatus.Exception;
                            scriptSpec.RunResult.Output.Add(new SequenceRunOuputSpec()
                            {Output = jobErrorMessage, When = now});
                        }
                        else if (runbookJobOutput == null)
                        {
                            hadError = true;
                            scriptSpec.ResultCode = "Exception";
                            scriptSpec.RunResult.StatusCode = "Exception";
                            jobErrorMessage = "Unable to get Job information from the SMA environment";
                            scriptSpec.RunResult.Output.Add(new SequenceRunOuputSpec() { Output = jobErrorMessage, When = now });
                        }
                        else if (runbookJobOutput.Result is bool && runbookJobOutput.Result == false ||
                                 runbookJobOutput.Result is string && runbookJobOutput.Result == "Fail")
                        {
                            hadError = true;
                            scriptSpec.ResultCode = "Exception";
                            scriptSpec.RunResult.StatusCode = "Exception";
                            jobErrorMessage = runbookJobOutput.Message;
                            scriptSpec.RunResult.Output.Add(new SequenceRunOuputSpec() { Output = jobErrorMessage, When = now });
                        }
                        else
                        {
                            scriptSpec.ResultCode = runbookJobStatus.Status;
                            scriptSpec.RunResult.StatusCode = runbookJobStatus.Status;
                            scriptSpec.RunResult.Output.Add(new SequenceRunOuputSpec() { Output = runbookJobStatus.Output, When = now });
                        }

                        vmDepReq.Config = vmc.Serialize();
                    }

                    if (hadError)
                    {
                        if (scriptSpec.BreakOn.Equals(SequenceSpec.BreakOnEnum.Warning.ToString(),
                            StringComparison.InvariantCultureIgnoreCase) || scriptSpec.BreakOn.Equals(SequenceSpec.BreakOnEnum.Exception.ToString(), // Included to handle exception sequences
                            StringComparison.InvariantCultureIgnoreCase))
                            throw new Exception(string.Format(
                                "Breaking error in sequence: '{0}' in state: '{1}' Message: '{2}'",
                                scriptSpec.Name, vmDepReq.StatusCode, jobErrorMessage));
                    }
                }

                _smaDown = false;
                return ret;
            }
            catch (Exception ex)
            {
                if (CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex).Contains("actively refused it"))
                {
                    if (_smaDown)
                    {
                        if (_smaDownStartTime.AddMinutes(_SmaRetryTimeoutMinutes) < DateTime.UtcNow)
                        {
                            ret.HadChange = true;
                            ret.HadError = true;
                            ret.HadException = true;
                            ret.Exceptionlist = new List<string>
                            {
                                "Exception in CheckSequencesStatus() : SMA Server not responding"
                            };
                            ret.Errorlist = ret.Exceptionlist;
                        }
                        else
                            ret.HadChange = false;
                    }
                    else
                    {
                        _smaDown = true;
                        _smaDownStartTime = DateTime.UtcNow;
                        LogThis(ex, EventLogEntryType.Error, "Exception in CheckSequencesStatus() : SMA Server not responding", 0, 0);
                        ret.HadChange = false;
                    }

                    return ret;
                }
                throw new Exception("Exception in CheckSequencesStatus() : " +
                    CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        private IEnumerable<StorageOps.ContainerSpaceInfo> SynchContainerSpaces(bool writelogTime)
        {
            try
            {
                var startTime = DateTime.Now;

                var serviceAccountList =
                    ServProvAccount.GetAzureServiceAccountList(_CmpDbConnectionString);

                if (0 == serviceAccountList.Count)
                    throw new Exception("No Azure accounts found in provider table.");

                var soList = serviceAccountList.Select(
                    sa => new StorageOps(new Connection(
                        sa.AccountID, sa.CertificateThumbprint, 
                        sa.AzureADTenantId, sa.AzureADClientId, sa.AzureADClientKey ))).ToList();

                var enforceAppAgAffinity = true;

                var eaaga =
                    Microsoft.Azure.CloudConfigurationManager.GetSetting("EnforceAppAgAffinity");
                if (null != eaaga)
                    if (eaaga.Equals("false", StringComparison.InvariantCultureIgnoreCase))
                        enforceAppAgAffinity = false;

                string synchExceptionMessageList = null;
                var requireAffinityGroup = true;

                _containerSpaceList = AzureAdminClientLib.StorageOps.SynchContainerSpace(
                    soList, _DefaultVhdContainerName, StorageOps.SynchType.Reset,
                    enforceAppAgAffinity, out synchExceptionMessageList);

                if (!_containerSpaceList.Any())
                    throw new Exception("No usable containers found in any given Azure account.");

                if (null != synchExceptionMessageList)
                    LogThis(null, EventLogEntryType.Error,
                        string.Format("SynchContainerSpaces() : synchExceptionMessageList : {0}",
                        synchExceptionMessageList), 1, 1);

                if (writelogTime)
                    LogThis(null, EventLogEntryType.Information,
                        string.Format("SynchContainerSpaces() : Accounts: {0}, Containers: {1}, Processing Time: {2}",
                        serviceAccountList.Count, _containerSpaceList.Count(), DateTime.Now.Subtract(startTime)), 1, 1);

            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error,
                    "Exception in ProcessorVm:SynchContainerSpaces()", 1, 1);
            }

            return _containerSpaceList;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="type"></param>
        /// <param name="prefix"></param>
        /// <param name="id"></param>
        /// <param name="category"></param>
        /// 
        //*********************************************************************

        private void LogThis(Exception ex, EventLogEntryType type, string prefix, 
            int id, short category)
        {
            if (null == _EventLog)
                return;

            if (null == ex)
                _EventLog.WriteEntry(prefix, type, id, category);
            else
                _EventLog.WriteEntry(prefix + " : " +
                    CmpCommon.Utilities.UnwindExceptionMessages(ex), 
                    type, id, category);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        int NonNull(int? arg)
        {
            return Utilities.GetDbInt(arg);
        }

        static int _defaultStateTtlMinutes = 0;

        static int DefaultStateTtlMinutes
        {
            get
            {
                if (0 == _defaultStateTtlMinutes)
                {
                    if (null == Microsoft.Azure.CloudConfigurationManager.GetSetting("DefaultStateTtlMinutes"))
                        _defaultStateTtlMinutes = 120;
                    else
                        try
                        {
                            _defaultStateTtlMinutes = Convert.ToInt32(Microsoft.Azure.CloudConfigurationManager.GetSetting("DefaultStateTtlMinutes"));
                        }
                        catch (Exception)
                        {
                            _defaultStateTtlMinutes = 120;
                        }
                }

                return _defaultStateTtlMinutes;
            }
        }

        static int _defaultRebootDwellTimeMinutes = 0;

        private static int DefaultRebootDwellTimeMinutes
        {
            get
            {
                if (0 == _defaultRebootDwellTimeMinutes)
                {
                    if (null == Microsoft.Azure.CloudConfigurationManager.GetSetting("DefaultVmRebootDwellTimeMinutes"))
                        _defaultRebootDwellTimeMinutes = 3;
                    else
                        try
                        {
                            _defaultRebootDwellTimeMinutes =
                                Convert.ToInt32(Microsoft.Azure.CloudConfigurationManager.GetSetting("DefaultVmRebootDwellTimeMinutes"));
                        }
                        catch (Exception)
                        {
                            _defaultRebootDwellTimeMinutes = 3;
                        }
                }

                return _defaultRebootDwellTimeMinutes;
            }
        }

        static int _defaultCoreSafetyStockValue = 0;

        static int DefaultCoreSafetyStockValue
        {
            get
            {
                if (Microsoft.Azure.CloudConfigurationManager.GetSetting("DefaultCoreSafetyStockValue") != null)
                {
                    try
                    {
                        _defaultCoreSafetyStockValue = Convert.ToInt32(Microsoft.Azure.CloudConfigurationManager.GetSetting("DefaultCoreSafetyStockValue"));
                    }
                    catch (Exception)
                    {
                    }
                }
                return _defaultCoreSafetyStockValue;
            }
        }


        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmReq"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        string VerifyMigratedVhds(Models.VmDeploymentRequest vmReq)
        {
            try
            {
                if (null == vmReq)
                    throw new Exception("vmReq == null");

                if (null == vmReq.ServiceProviderAccountID)
                    throw new Exception("vmReq.ServiceProviderAccountID == null");

                var adb = new AftsDb(_AftsDbConnectionString);
                var transferReqList = adb.FetchRequestByTagId(vmReq.ID);

                //*** assemble the vmReq.config ***

                var vmc = CmpInterfaceModel.Models.VmConfig.Deserialize(vmReq.Config);
                var lun = 1;

                var dataDiskList = new List<DataVirtualHardDisk>();
                OsVirtualHardDisk osDisk = null;

                var cdb = new CmpDb(_CmpDbConnectionString);
                var servProdAccount = cdb.FetchServiceProviderAccount((int)vmReq.ServiceProviderAccountID);
                var connection = ServProvAccount.GetAzureServiceAccountConnection(
                    servProdAccount.AccountID, servProdAccount.CertificateThumbprint, 
                    servProdAccount.AzureADTenantId, servProdAccount.AzureADClientId, servProdAccount.AzureADClientKey);
                var dops = new DiskOps(connection);
                //
                foreach (var transferReq in transferReqList)
                {
                    if (!transferReq.Destination[transferReq.Destination.Length - 1].Equals('/'))
                        transferReq.Destination += "/";

                    var fileName = CmpInterfaceModel.Utilities.ExtractFileName(transferReq.Source);
                    //var diskName = string.Format("{0}-{1}-{2}-{3}",
                    //    vmReq.ParentAppID, vmReq.TargetServicename, fileName, rand.Next(1000));
                    var diskName = string.Format("{0}-{1}-{2}",
                        vmReq.ParentAppID, vmReq.TargetServicename, fileName);

                    var diskInfo = new DiskInfo()
                    {
                        Label = diskName,
                        MediaLink = transferReq.Destination + fileName,
                        Name = diskName
                    };

                    for (var tryCount = 1; ; tryCount++)
                    {
                        var resp = dops.AddDiskToSubscription(diskInfo);

                        if (resp.HadError)
                        {
                            if (tryCount > _diskCreationRetryLimit - 1)
                                throw new Exception("Unable to allocate Azure disk from media link '"
                                    + diskInfo.MediaLink + "' - Exceeded retry limit : " + resp.Body);

                            if (resp.Body.Contains("(400)") | resp.Body.Contains("(404)")) //bad request, Not Found
                            {
                                Thread.Sleep((int)(1000 * Math.Pow(4.17, tryCount)));
                                continue;
                            }

                            throw new Exception("Unable to allocate Azure disk from media link '"
                                                + diskInfo.MediaLink + "' : " + resp.Body);
                        }

                        break;
                    }
                }

                return vmc.Serialize();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in BuildCmpConfig() : " +
                    CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex));
            }
        }

        private enum BuildMigratedVmConfigStringResultEnum
        {
            Success,
            MissingDisk
        };

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="vmReq"></param>
        /// <param name="vmConfigString"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************

        Constants.BuildMigratedVmConfigStringResultEnum BuildMigratedVmConfigString(
            Models.VmDeploymentRequest vmReq,
            out string vmConfigString)
        {
            try
            {
                if (null == vmReq)
                    throw new Exception("vmReq == null");

                if (null == vmReq.ServiceProviderAccountID)
                    throw new Exception("vmReq.ServiceProviderAccountID == null");

                var adb = new AftsDb(_AftsDbConnectionString);
                var transferReqList = adb.FetchRequestByTagId(vmReq.ID);

                //*** assemble the vmReq.config ***

                var vmc = CmpInterfaceModel.Models.VmConfig.Deserialize(vmReq.Config);
                var lun = 1;

                var dataDiskList = new List<DataVirtualHardDisk>();
                OsVirtualHardDisk osDisk = null;

                var cdb = new CmpDb(_CmpDbConnectionString);
                var servProdAccount = cdb.FetchServiceProviderAccount((int)vmReq.ServiceProviderAccountID);
                var connection = ServProvAccount.GetAzureServiceAccountConnection(
                    servProdAccount.AccountID, servProdAccount.CertificateThumbprint, 
                    servProdAccount.AzureADTenantId, servProdAccount.AzureADClientId, servProdAccount.AzureADClientKey);
                var dops = new DiskOps(connection);
                //var rand = new Random();

                foreach (var transferReq in transferReqList)
                {
                    if (!transferReq.Destination[transferReq.Destination.Length - 1].Equals('/'))
                        transferReq.Destination += "/";

                    var fileName = CmpInterfaceModel.Utilities.ExtractFileName(transferReq.Source);
                    //var diskName = string.Format("{0}-{1}-{2}-{3}",
                    //    vmReq.ParentAppID, vmReq.TargetServicename, fileName, rand.Next(1000));
                    var diskName = string.Format("{0}-{1}-{2}",
                        vmReq.ParentAppID, vmReq.TargetServicename, fileName);

                    var diskInfo = new DiskInfo()
                    {
                        Label = diskName,
                        MediaLink = transferReq.Destination + fileName,
                        Name = diskName
                    };

                    if (!StorageOps.DoesBlobExist(diskInfo.MediaLink, transferReq.StorageAccountKey))
                    {
                        vmConfigString = "Source blob: '" + diskInfo.MediaLink + "' not found";
                        return Constants.BuildMigratedVmConfigStringResultEnum.MissingDisk;
                    }

                    if (transferReq.Config.ToLower().Contains("<isos>true</isos>"))
                    {
                        diskInfo.Os = DiskInfo.OsEnum.Windows;

                        osDisk = new OsVirtualHardDisk()
                        {
                            //DiskLabel = Utilities.StringToB64(diskName),
                            DiskName = diskName,
                            //HostCaching = "ReadOnly"
                            //Populating media link property for dashborad to show storage account in migration scenario
                            //MediaLink = transferReq.Destination +fileName, //*** Commented out by markwes 20140702
                            //SourceImageName = ""
                        };
                    }
                    else
                    {
                        diskInfo.Os = DiskInfo.OsEnum.None;

                        dataDiskList.Add(new DataVirtualHardDisk()
                        {
                            //DiskLabel = Utilities.StringToB64(diskName),
                            DiskName = diskName,
                            //HostCaching = "None",
                            LogicalDiskSizeInGB = "0",
                            Lun = lun++.ToString()
                            //MediaLink = "",
                            //SourceMediaLink = transferReq.Destination + fileName
                        });
                    }

                    for (var tryCount = 1; ; tryCount++)
                    {
                        var resp = dops.AddDiskToSubscription(diskInfo);

                        if (resp.HadError)
                        {
                            if (tryCount > _diskCreationRetryLimit - 1)
                                throw new Exception("Unable to allocate Azure disk from media link '"
                                    + diskInfo.MediaLink + "' - Exceeded retry limit : " + resp.Body);

                            if (resp.Body.Contains("(400)") | resp.Body.Contains("(404)")) //bad request, Not Found
                            {
                                Thread.Sleep((int)(1000 * Math.Pow(4.17, tryCount)));
                                continue;
                            }

                            throw new Exception("Unable to allocate Azure disk from media link '"
                                                + diskInfo.MediaLink + "' : " + resp.Body);
                        }

                        break;
                    }
                }

                vmReq.CurrentStateStartTime = DateTime.UtcNow;
                var computerName = vmReq.TargetVmName;
                var roleName = computerName;
                const string deploymentSlot = "Production";

                //*** Disks ***

                var ovd = new CmpInterfaceModel.Models.OsVirtualHardDisk
                {
                    //MediaLink = mediaLink,
                    //SourceImageName = GetSourceImageName(myCapReq)
                };

                var vmDep = new CmpInterfaceModel.Models.AzureVmDeployment
                {
                    //Name = cdr.TargetServiceName,
                    Name = roleName,
                    Label = CmpInterfaceModel.Utilities.StringToB64(roleName),
                    DeploymentSlot = deploymentSlot,
                    VirtualNetworkName = CmpInterfaceModel.Constants.AUTOVNET
                };

                //*** WinRM ***

                /*var List = new CmpInterfaceModel.Models.Listener { Protocol = "Https" };

                var listList = new List<CmpInterfaceModel.Models.Listener> { List };
                var wrm = new CmpInterfaceModel.Models.WinRM { Listeners = listList };*/

                //*** ConfigSet ***

                var cs = new CmpInterfaceModel.Models.WindowsProvisioningConfigurationSet
                {
                    //AdminUsername = vmAdminUserInfo.UserName,
                    //AdminPassword = vmAdminUserInfo.Password,
                    ComputerName = computerName,
                    ConfigurationSetType = "WindowsProvisioningConfiguration",
                    EnableAutomaticUpdates = "true"
                    //WinRM = wrm,
                    //TimeZone = GetTimezoneID(myCapReq)
                    //DomainJoin = DJ,
                    //StoredCertificateSettings = CertSetList,
                };

                //*** Domain Join ***

                /*var domainInfo = GetDomainInfo(myCapReq);

                string djUserDomain = null;
                string djUserName = null;

                if (domainInfo.JoinCredsUserName.Contains('\\'))
                {
                    var djCredSplit = domainInfo.JoinCredsUserName.Split('\\');
                    djUserDomain = djCredSplit[0];
                    djUserName = djCredSplit[1];
                }
                else
                {
                    djUserDomain = domainInfo.DomainFullName;
                    djUserName = domainInfo.JoinCredsUserName;
                }

                var djCreds = new CmpInterfaceModel.Models.Credentials
                {
                    Domain = djUserDomain,
                    Password = domainInfo.JoinCredsPasword,
                    Username = djUserName
                };

                var dj = new CmpInterfaceModel.Models.DomainJoin
                {
                    Credentials = djCreds,
                    JoinDomain = domainInfo.DomainFullName,
                    MachineObjectOU = domainInfo.ServerOU
                };

                cs.DomainJoin = dj;*/

                var csList = new List<CmpInterfaceModel.Models.ConfigurationSet>();
                //csList.Add(cs);

                //*** Ports ***


                var ie1 = new CmpInterfaceModel.Models.InputEndpoint
                {
                    //Port = "5986",
                    LocalPort = "5986",
                    Name = "WinRmHTTPs",
                    Protocol = "tcp"
                };

                var ie2 = new CmpInterfaceModel.Models.InputEndpoint
                {
                    //LoadBalancedEndpointSetName = "LBESN",
                    //LoadBalancerProbe = LBP,
                    //Port = "1234",
                    LocalPort = "3389",
                    Name = "RDP",
                    Protocol = "tcp"
                };

                var ie3 = new CmpInterfaceModel.Models.InputEndpoint
                {
                    //LoadBalancedEndpointSetName = "LBESN",
                    //LoadBalancerProbe = LBP,
                    //Port = "1234",
                    LocalPort = "5985",
                    Name = "WinRmHTTP",
                    Protocol = "tcp"
                };

                var ieList = new List<CmpInterfaceModel.Models.InputEndpoint> { ie1, ie2, ie3 };

                var cs2 = new CmpInterfaceModel.Models.NetworkConfigurationSet
                {
                    ConfigurationSetType = "NetworkConfiguration",
                    //InputEndpoints = ieList,
                    SubnetNames =
                        new CmpInterfaceModel.Models.SubnetNames
                        {
                            SubnetName = CmpInterfaceModel.Constants.AUTOSUBNETNAME
                        }
                };

                if (null == vmc.InfoFromVM.MachineDomain)
                    cs2.InputEndpoints = ieList;
                else if (0 < vmc.InfoFromVM.MachineDomain.Length)
                    cs2.InputEndpoints = ieList;

                /*if (null != vmc.Placement)
                    if (null != vmc.Placement.Config)
                    {
                        var pc = PlacementConfig.Deserialize(vmc.Placement.Config);
                        if(pc.UseStaticIpAddr)
                            cs2.StaticVirtualNetworkIPAddress = CmpInterfaceModel.Constants.AUTOSTATICIPADDRESS;
                    }*/

                csList.Add(cs2);

                //*** Add VM Agent ***

                var resourceExtensionReferences = new List<ResourceExtensionReference>
                {
                    new ResourceExtensionReference()
                    {
                        Name = "BGInfo",
                        Publisher = "Microsoft.Compute",
                        ReferenceName = "BGInfo",
                        ResourceExtensionParameterValues = new List<ResourceExtensionParameterValue>(),
                        State = "Enable",
                        Version = "1.*"
                    }
                };

                //*** Role ***

                var vmRole = new CmpInterfaceModel.Models.PersistentVMRole
                {
                    //AvailabilitySetName = "AsName",
                    ConfigurationSets = csList,
                    DataVirtualHardDisks = dataDiskList,
                    OSVirtualHardDisk = osDisk,
                    RoleName = roleName,
                    RoleSize = vmReq.VmSize,
                    RoleType = "PersistentVMRole",
                    ProvisionGuestAgent = "true",
                    Label = CmpInterfaceModel.Utilities.StringToB64(roleName),
                    ResourceExtensionReferences = resourceExtensionReferences
                };

                vmDep.RoleList = new List<CmpInterfaceModel.Models.Role> { vmRole };

                //***

                /*var placement = new CmpInterfaceModel.Models.PlacementSpec
                {
                    Method = CmpInterfaceModel.Models.PlacementMethodEnum.AutoDefault.ToString(),
                    TargetServiceProviderAccountID = 0,
                    TargetServiceProviderAccountGroup = GetTargetServiceProviderAccountGroup(myCapReq._RequestRecord),
                    ServerOu = domainInfo.ServerOU,
                    WorkstationOu = domainInfo.WorkstationOU
                };

                if (null == placement.TargetServiceProviderAccountGroup)
                    placement.TargetServiceProviderAccountID = GetDefaultTargetServiceProviderAccountID();*/

                //***

                vmc.AzureVmConfig = vmDep;

                /*var vmc = new CmpInterfaceModel.Models.VmConfig
                {
                    AzureVmConfig = vmDep,
                    HostedServiceConfig = hostServ,
                    ServiceCertList = serviceCertFileList,
                    DiskSpecList = GetDiskSpecList(myCapReq._RequestRecord.StorageConfigXML),
                    Placement = placement,
                    UserSpecList = GetLocalUserList(myCapReq, domainInfo.DefaultVmAdminMember),
                    PageFileConfig = GetPageFileSpec(myCapReq)
                };*/

                //return vmc.Serialize();

                vmConfigString = vmc.Serialize();
                return Constants.BuildMigratedVmConfigStringResultEnum.Success;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in BuildCmpConfig() : " +
                    CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmReq"></param>
        /// <param name="bm"></param>
        /// <param name="warningList"></param>
        /// <param name="cdb"></param>
        /// 
        //*********************************************************************

        void CompleteVmRequest(Models.VmDeploymentRequest vmReq, 
            List<string> warningList, CmpDb cdb)
        {
            vmReq.CurrentStateStartTime = DateTime.UtcNow;
            vmReq.StatusCode = Constants.StatusEnum.Complete.ToString();
            vmReq.StatusMessage = "VM Processing Complete";

            //*** Set warnings as exceptions, not sure that I like this ***
            

            if (vmReq.Warnings != null)
                if (0 < vmReq.Warnings.Length)
                {
                    if (vmReq.Warnings.ToLower((CultureInfo.InvariantCulture)).Contains("itsm") || vmReq.Warnings.Contains("6000") || (vmReq.Warnings.ToLower((CultureInfo.InvariantCulture)).Contains("ci creation")))
                    {
                        vmReq.StatusCode = CmpInterfaceModel.Constants.StatusEnum.Complete.ToString();
                        vmReq.StatusMessage = "VM Processing Complete.CI Creation Pending";
                    }
                    else
                    {
                        vmReq.StatusCode = CmpInterfaceModel.Constants.StatusEnum.Exception.ToString();
                        vmReq.StatusMessage = "Completed with warnings: See warning list";
                    }
                }
            if (warningList != null)
                if (0 < warningList.Count)
                {
                    foreach (var warning in warningList)
                    {
                        if ((warning.ToLower((CultureInfo.InvariantCulture)).Contains("itsm") || warning.ToLower((CultureInfo.InvariantCulture)).Contains("6000") || (warning.ToLower((CultureInfo.InvariantCulture)).Contains("ci creation")))&&(warningList.Count <=1))
                        {
                            vmReq.StatusCode = CmpInterfaceModel.Constants.StatusEnum.Complete.ToString();
                            vmReq.StatusMessage = "VM Processing Complete.CI Creation Pending";
                            break;
                        }
                        else
                        {
                            vmReq.StatusCode = CmpInterfaceModel.Constants.StatusEnum.Exception.ToString();
                            vmReq.StatusMessage = "Completed with warnings: See warning list";
                        }
                    }
                }

            try
            {
                cdb.SetVmDepRequestStatus(vmReq, warningList);
            }
            catch (Exception ex)
            {
                LogThis(ex, System.Diagnostics.EventLogEntryType.Error,
                    "Exception while setting req status in CompleteVmRequest()", 100, 100);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmReq"></param>
        /// <param name="ttl"></param>
        /// <param name="prefix"></param>
        /// 
        //*********************************************************************

        void AssertIfTimedOut(Models.VmDeploymentRequest vmReq, int ttl, string prefix)
        {
            if (0 == ttl)
                ttl = DefaultStateTtlMinutes;

            if (null != vmReq.CurrentStateStartTime)
                if (ttl < DateTime.UtcNow.Subtract(((DateTime)vmReq.CurrentStateStartTime)).TotalMinutes)
                    throw new Exception(prefix + " processing time (" +
                        ttl + " minutes) exceeded. Processing abandoned.");
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmReq"></param>
        /// <param name="ttl"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        bool HasTimedOut(Models.VmDeploymentRequest vmReq, int ttl)
        {
            if (0 == ttl)
                ttl = DefaultStateTtlMinutes;

            if (null != vmReq.CurrentStateStartTime)
                if (ttl < DateTime.UtcNow.Subtract(((DateTime)vmReq.CurrentStateStartTime)).TotalMinutes)
                    return true;

            return false;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmdeploymentrequest"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        CmpInterfaceModel.Models.VmDeploymentRequest ServToInt(
            CmpServiceLib.Models.VmDeploymentRequest vmdeploymentrequest)
        {
            if (null == vmdeploymentrequest)
                return null;

            var ret = new CmpInterfaceModel.Models.VmDeploymentRequest
            {
                ID = vmdeploymentrequest.ID,
                ExceptionMessage = vmdeploymentrequest.ExceptionMessage,
                LastStatusUpdate = CmpServiceLib.Utilities.GetDbDateTime(vmdeploymentrequest.LastStatusUpdate),
                ParentAppName = vmdeploymentrequest.ParentAppName,
                RequestDescription = vmdeploymentrequest.RequestDescription,
                RequestName = vmdeploymentrequest.RequestName,
                SourceServerName = vmdeploymentrequest.SourceServerName,
                SourceVhdFilesCSV = vmdeploymentrequest.SourceVhdFilesCSV,
                Status = vmdeploymentrequest.StatusCode,
                TagData = vmdeploymentrequest.TagData,
                TargetLocation = vmdeploymentrequest.TargetLocation,
                TargetLocationType = vmdeploymentrequest.TargetLocationType,
                TargetVmName = vmdeploymentrequest.TargetVmName,
                TargetServiceName = vmdeploymentrequest.TargetServicename,
                VmSize = vmdeploymentrequest.VmSize,
                WhenRequested = CmpServiceLib.Utilities.GetDbDateTime(vmdeploymentrequest.WhenRequested),
                WhoRequested = vmdeploymentrequest.WhoRequested,
                Active = CmpServiceLib.Utilities.GetDbBool(vmdeploymentrequest.Active),
                AftsID = CmpServiceLib.Utilities.GetDbInt(vmdeploymentrequest.AftsID),
                Config = vmdeploymentrequest.Config,
                ParentAppID = vmdeploymentrequest.ParentAppID,
                RequestType = vmdeploymentrequest.RequestType,
                StatusMessage = vmdeploymentrequest.StatusMessage,
                TargetAccount = vmdeploymentrequest.TargetAccount,
                TargetAccountCreds = vmdeploymentrequest.TargetAccountCreds,
                TargetAccountType = vmdeploymentrequest.TargetAccountType,
                TargetServiceProviderType = vmdeploymentrequest.TargetServiceProviderType,
                TagID = CmpServiceLib.Utilities.GetDbInt(vmdeploymentrequest.TagID),
                ValidationResults = vmdeploymentrequest.ValidationResults,
                ExceptionTypeCode = vmdeploymentrequest.ExceptionTypeCode,
                SourceServerRegion = vmdeploymentrequest.SourceServerRegion,
                TargetServiceProviderAccountID = Convert.ToInt32(NonNull(vmdeploymentrequest.ServiceProviderAccountID))
            };
            return ret;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="osVhdStoreConnection"></param>
        /// <param name="blobUrl"></param>
        /// <param name="timeoutHours"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private string GetSasUrl(Connection osVhdStoreConnection, string blobUrl, int timeoutHours)
        {
            var so = new StorageOps(osVhdStoreConnection);
            return so.GetBlobSasUri(blobUrl, "storagekey", timeoutHours);
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="vmReq"></param>
        /// <param name="affinityGroup"></param>
        /// <param name="connection"></param>
        /// <param name="servProdAccount"></param>
        ///  
        //*********************************************************************

        //*** NOTE * Compute

        private HttpResponse SetPlacement(Models.VmDeploymentRequest vmReq, string affinityGroup,
            AzureAdminClientLib.Connection connection, Models.ServiceProviderAccount servProdAccount)
        {
            try
            {
                lock (_PlacementLockObject)
                {
                    AzureAdminClientLib.StorageOps so = null;
                    var diskCount = 0;
                    var reserveSpace = false;
                    AzureVirtualNetwork leastUsedVnet = null;

                    if (null == _autoLocalAdminUserName)
                        _autoLocalAdminUserName = _DefaultocalAdminUserName;

                    vmReq.Config = vmReq.Config.Replace(Constants.AUTOLOCALADMINUSERNAME, _autoLocalAdminUserName);
                    vmReq.Config = vmReq.Config.Replace(Constants.AUTOLOCALADMINPASSWORD,
                        "P123!!" + Guid.NewGuid().ToString("N").Substring(0, 9));
                    vmReq.ServiceProviderAccountID = servProdAccount.ID;

                    //*** Deserialize ***

                    var vmCfg = CmpInterfaceModel.Models.VmConfig.Deserialize(vmReq.Config);

                    if (null == vmCfg.Placement)
                        vmCfg.Placement = new PlacementSpec();

                    //*** Get SAS URL for given OS source URL ***

                    /*string blobUrl = "u";
                    int timeoutHours = 24;

                    var cdb = new CmpDb(_CmpDbConnectionString);
                    var osVhdStoreAccount = cdb.FetchOsVhdStoreAccount(vmReq.TargetLocation);
                    var osVhdStoreConnection = new Connection(osVhdStoreAccount.AccountID, osVhdStoreAccount.CertificateThumbprint);

                    var sasUrl = GetSasUrl(osVhdStoreConnection, blobUrl, timeoutHours);*/

                    //*** Fetch info about storage account ***

                    so = new AzureAdminClientLib.StorageOps(connection);
                    AzureStorageAccountContainer container = null;

                    if (null == vmCfg.Placement.StorageContainerUrl)
                    {
                        var enforceAppAgAffinity = true;

                        //if (null != affinityGroup)
                        //{
                        var eaaga =
                            Microsoft.Azure.CloudConfigurationManager.GetSetting("EnforceAppAgAffinity");
                        if (null != eaaga)
                            if (eaaga.Equals("false", StringComparison.InvariantCultureIgnoreCase))
                                enforceAppAgAffinity = false;
                        //}
                        //*** Get the least used container ***

                        diskCount = 1;
                        if (null != vmCfg.DiskSpecList)
                            diskCount += vmCfg.DiskSpecList.Count;

                        try
                        {
                            if (_EnforceAppAgAffinity)
                                container = so.GetLeastUsedContainer(_DefaultVhdContainerName,
                                    affinityGroup, null, _blobsPerContainerLimit, _vmsPerVnetLimit, diskCount, true);
                            else
                                container = so.GetLeastUsedContainer(_DefaultVhdContainerName,
                                    null, vmCfg.HostedServiceConfig.Location, _blobsPerContainerLimit, _vmsPerVnetLimit, diskCount, false);
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("503"))
                                return new HttpResponse
                                {
                                    HadError = true,
                                    Retry = true,
                                    Body = CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex)
                                };

                            if (ex.Message.Contains("keys failed"))
                                return new HttpResponse
                                {
                                    HadError = true,
                                    Retry = true,
                                    Body = CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex)
                                };

                            throw;
                        }

                        if (null == container)
                            throw new Exception("No suitable containers found in subscription. Suitable containers must be named '" +
                                _DefaultVhdContainerName + "' and must be in an InUse VNet");

                        //if (container.resolved)

                        reserveSpace = true;
                        //so.ReserveContainerSpace(container.Url, diskCount, vmReq.ID, vmReq);

                        leastUsedVnet = AzureVirtualNetwork.FindLeastUsed(container.StorageAccount.VirtualNetworksAvailable);

                        //*** Populate placement ***

                        vmCfg.Placement.StorageContainerUrl = container.Url;
                        vmCfg.Placement.AffinityGroup = container.StorageAccount.AffinityGroup;
                        vmCfg.Placement.Location = container.StorageAccount.Location;
                        vmCfg.Placement.VNet = leastUsedVnet.Name;
                        vmCfg.Placement.Subnet = servProdAccount.AzSubnet;
                        vmCfg.Placement.DiskCount = diskCount;

                        //* Duplicate *
                        //vmCfg.Placement.StorageContainerUrl = servProdAccount.AzStorageContainerUrl;

                        if (null != container.StorageAccount.AffinityGroup)
                            vmCfg.HostedServiceConfig.AffinityGroup = Constants.AUTOAFFINITYGROUP;
                        else
                            vmCfg.HostedServiceConfig.Location = Constants.AUTOLOCATION;

                        vmReq.Config = vmCfg.Serialize();
                    }
                    else
                    {
                        //*** get the given container ***
                        container = so.GetContainer(vmCfg.Placement.StorageContainerUrl);
                    }

                    if (null == container)
                        throw new Exception(string.Format(
                            "Container {0} not found in given subscription",
                            vmCfg.Placement.StorageContainerUrl));

                    //*** Do we need to change Service? ***

                    if (null != affinityGroup)
                    {
                        var spaList = new List<ServiceProviderAccount> { servProdAccount };

                        if (!affinityGroup.Equals(container.StorageAccount.AffinityGroup,
                            StringComparison.InvariantCultureIgnoreCase))
                        {
                            string useThisServiceName;
                            string useThisAffinityGroupName;

                            var avail = CheckServiceNameAvailability(spaList,
                                vmReq.TargetServicename, container.StorageAccount.AffinityGroup, vmCfg.HostedServiceConfig.Location, vmCfg.Placement.DiskCount,
                                out servProdAccount, out useThisServiceName, out useThisAffinityGroupName);

                            switch (avail)
                            {
                                case HostedServiceOps.ServiceAvailabilityEnum.AlredayOwnIt:
                                    break;
                                case HostedServiceOps.ServiceAvailabilityEnum.Available:

                                    vmReq.Config = vmReq.Config.Replace(Constants.AUTOAFFINITYGROUP, container.StorageAccount.AffinityGroup);
                                    vmReq.Config = vmReq.Config.Replace(Constants.AUTOLOCATION, container.StorageAccount.Location);
                                    vmReq.Config = vmReq.Config.Replace(vmReq.TargetServicename, useThisServiceName);
                                    vmReq.TargetServicename = useThisServiceName;

                                    var hso = new AzureAdminClientLib.HostedServiceOps(connection);
                                    var hsBody = BuildAzureHsRequestBody(vmReq.Config);
                                    var resp = hso.CreateHostedService(hsBody);

                                    if (resp.HadError)
                                    {
                                        throw new Exception(string.Format(
                                            "Unable to create new service '{0}' needed to match given affinity group '{1}'",
                                            useThisServiceName, container.StorageAccount.AffinityGroup));
                                    }

                                    break;
                            }

                            vmReq.Config = vmReq.Config.Replace(Constants.AUTOAFFINITYGROUP, container.StorageAccount.AffinityGroup);
                            vmReq.Config = vmReq.Config.Replace(Constants.AUTOLOCATION, container.StorageAccount.Location);
                            vmReq.Config = vmReq.Config.Replace(vmReq.TargetServicename, useThisServiceName);
                            vmReq.TargetServicename = useThisServiceName;
                        }
                    }

                    if (null == leastUsedVnet)
                        leastUsedVnet = AzureVirtualNetwork.FindLeastUsed(container.StorageAccount.VirtualNetworksAvailable);

                    servProdAccount.AzStorageContainerUrl = container.Url;
                    servProdAccount.AzAffinityGroup = container.StorageAccount.AffinityGroup;
                    //servProdAccount.Location = container.StorageAccount.Location;
                    servProdAccount.AzVNet = leastUsedVnet.Name;
                    servProdAccount.AzSubnet = servProdAccount.AzSubnet;

                    //*** Replace placeholders with live values ***

                    vmReq.Config = vmReq.Config.Replace(Constants.AUTOSTORAGEACCOUNTNAME,
                        container.StorageAccount.Name);
                    vmReq.Config = vmReq.Config.Replace(Constants.AUTOBLOBSTORELOCATION,
                        servProdAccount.AzStorageContainerUrl);
                    vmReq.Config = vmReq.Config.Replace(Constants.AUTOAFFINITYGROUP, container.StorageAccount.AffinityGroup);
                    vmReq.Config = vmReq.Config.Replace(Constants.AUTOLOCATION, container.StorageAccount.Location);
                    vmReq.Config = vmReq.Config.Replace(Constants.AUTOVNET, servProdAccount.AzVNet);
                    vmReq.Config = vmReq.Config.Replace(Constants.AUTOSUBNETNAME, servProdAccount.AzSubnet);

                    vmReq.TargetAccount = servProdAccount.AccountID;
                    vmReq.TargetAccountType = servProdAccount.AccountType;

                    var roleSize = Utilities.GetXmlInnerText(vmReq.Config, "RoleSize");

                    if (null == roleSize)
                        throw new Exception("NULL role size name");

                    CmpDb cdb = new CmpDb(_CmpDbConnectionString);
                    var matchedRoleSizeFromDb = cdb.GetAzureRoleSizeFromName(roleSize);
                    
                    if (null == matchedRoleSizeFromDb)
                        throw new Exception("Invalid role size name : '" + roleSize + "'");

                    /* Translating AzureRoleSize object from CmpDb to RoleSize object, which belongs
                     * to AzureClientAdminLib, to keep them independent. In the future, we can combine both
                     * classes and remove redundancies.
                     */
                    var requestedSize = new RoleSize(
                        matchedRoleSizeFromDb.Name,
                        matchedRoleSizeFromDb.CoreCount,
                        matchedRoleSizeFromDb.DiskCount,
                        matchedRoleSizeFromDb.RamMb,
                        matchedRoleSizeFromDb.DiskSizeRoleOs,
                        matchedRoleSizeFromDb.DiskSizeRoleApps,
                        matchedRoleSizeFromDb.DiskSizeVmOs,
                        matchedRoleSizeFromDb.DiskSizeVmTemp,
                        matchedRoleSizeFromDb.CanBeService,
                        matchedRoleSizeFromDb.CanBeVm);

                    /*AzureSubscription subscriptionInfo = new AzureSubscription();

                    // Call Load to populate the subscription object with values from the cloud.
                    subscriptionInfo.Load(connection.SubcriptionID, connection.CertThumbprint);

                    // Check the pertinent subscription level values against the VM requested values.
                    if (subscriptionInfo.CurrentCoreCount + requestedSize.CoreCount > subscriptionInfo.MaxCoreCount)
                    {
                        throw new Exception("Requested role size exceeds number of unused cores remaining on subscription");
                    }*/

                    //*** TODO: Remove this when the cross disk/image issue is resolved ***
                    /*return;

                    AzureVirtualNetwork leastUsedVirtualNetwork = null;
                    AzureStorageAccountContainer aStorageAccountContainerToUse = null;
                     * 
                    // If there are no virtual networks on the subscription then throw 
                    if (subscriptionInfo.VirtualNetworks == null)
                    {
                        throw new Exception("No VNets found on subscription");
                    }
                    else
                    {
                        // get the least used VN that's in service.
                        leastUsedVirtualNetwork = subscriptionInfo.VirtualNetworks.Min();

                        if (!leastUsedVirtualNetwork.Subnets.Contains(servProdAccount.AzSubnet))
                            throw new Exception("Requested subnet: '" + servProdAccount.AzSubnet + "' not found on VNet: '" + leastUsedVirtualNetwork.Name + "'");

                        aStorageAccountContainerToUse = subscriptionInfo.GetStorageAccountContainer(leastUsedVirtualNetwork.AffinityGroup);

                        VmReq.Config = VmReq.Config.Replace(Constants.AUTOBLOBSTORELOCATION, aStorageAccountContainerToUse.Url);
                        VmReq.Config = VmReq.Config.Replace(Constants.AUTOAFFINITYGROUP, leastUsedVirtualNetwork.AffinityGroup);
                        VmReq.Config = VmReq.Config.Replace(Constants.AUTOVNET, leastUsedVirtualNetwork.Name);
                    }*/

                    if (reserveSpace)
                        so.ReserveContainerSpace(container.Url, diskCount, vmReq.ID, vmReq);
                }

                return new HttpResponse { HadError = false, Retry = false };
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in SetPlacement() : " +
                    CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmReq"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************
        private PowershellLib.Remoting GetPowershellConnection(Models.VmDeploymentRequest vmReq)
        {
            try
            {
                var vmc = CmpInterfaceModel.Models.VmConfig.Deserialize(vmReq.Config);

                var tempHostName = vmReq.TargetVmName + ".cloudapp.net";
                var tempDomain = Utilities.GetXmlInnerText(vmReq.Config, "ComputerName");
                var tempAccountName = Utilities.GetXmlInnerText(vmReq.Config, "AdminUsername");
                var tempPassword = Utilities.GetXmlInnerText(vmReq.Config, "AdminPassword");
                string remotePsUrl = null;

                var connection =
                    ServProvAccount.GetAzureServiceAccountConnection(
                    Convert.ToInt32(vmReq.ServiceProviderAccountID), _CmpDbConnectionString);

                if (null == tempAccountName)
                {
                    var defaultRemotingAccount =
                        Microsoft.Azure.CloudConfigurationManager.GetSetting("DefaultRemotingAccount");

                    if (null != defaultRemotingAccount)
                    {
                        var xk = new KryptoLib.X509Krypto(null);
                        defaultRemotingAccount = xk.DecrpytKText(defaultRemotingAccount);

                        var accountParts = defaultRemotingAccount.Split(',');

                        if (2 < accountParts.Count())
                        {
                            tempDomain = accountParts[0];
                            tempAccountName = accountParts[1];
                            tempPassword = accountParts[2];
                        }
                    }

                    string ipAddress;

                    remotePsUrl = PowershellLib.VirtualMachineRemotePowerShell.GetPowerShellUrl(
                        connection.SubcriptionID, connection.Certificate, connection.AdToken, 
                        vmReq.TargetServicename, vmReq.TargetVmName,
                        PowershellLib.VirtualMachineRemotePowerShell.RpcPortVisibility.Private, out ipAddress);

                    try
                    {
                        return new PowershellLib.Remoting(remotePsUrl, tempDomain + "\\" + tempAccountName, tempPassword);
                    }
                    catch(Exception ex)
                    {
                        throw new Exception(string.Format(
                            "Using 'DefaultRemotingAccount' ({0}\\{1}) : {2}", tempDomain, tempAccountName, ex.Message));
                    }
                }

                //*** Try to connect to private network RPC port ***

                string ipAddress2;

                remotePsUrl = PowershellLib.VirtualMachineRemotePowerShell.GetPowerShellUrl(
                    connection.SubcriptionID, connection.Certificate, connection.AdToken, 
                    vmReq.TargetServicename, tempHostName,
                    PowershellLib.VirtualMachineRemotePowerShell.RpcPortVisibility.PrivateHttps, out ipAddress2);

                if (null != remotePsUrl)
                {
                    try
                    {
                        return new PowershellLib.Remoting(remotePsUrl, 
                            tempDomain + "\\" + tempAccountName, tempPassword);
                    }
                    catch (PowershellLib.FailToConnectException)
                    {
                    }
                }

                //*** Try to connect to public network HTTPS RPC port ***

                remotePsUrl = PowershellLib.VirtualMachineRemotePowerShell.GetPowerShellUrl(
                    connection.SubcriptionID, connection.Certificate, connection.AdToken,
                    vmReq.TargetServicename, tempHostName,
                    PowershellLib.VirtualMachineRemotePowerShell.RpcPortVisibility.PublicHttps, out ipAddress2);

                if (null != remotePsUrl)
                {
                    try
                    {
                        return new PowershellLib.Remoting(remotePsUrl, tempDomain + "\\" +
                            tempAccountName, tempPassword);
                    }
                    catch (PowershellLib.FailToConnectException)
                    {
                    }
                }

                //*** Try to connect to public network HTTP RPC port ***

                remotePsUrl = PowershellLib.VirtualMachineRemotePowerShell.GetPowerShellUrl(
                    connection.SubcriptionID, connection.Certificate, connection.AdToken,
                    vmReq.TargetServicename, tempHostName,
                    PowershellLib.VirtualMachineRemotePowerShell.RpcPortVisibility.PublicHttp, out ipAddress2);

                if (null == remotePsUrl)
                    throw new Exception("Not Found");

                return new PowershellLib.Remoting(remotePsUrl, tempDomain + "\\" +
                    tempAccountName, tempPassword);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ProcessorVm.GetPowershellConnection() : " +
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="vmReq"></param>
        /// <param name="remotePsUrl"></param>
        /// <param name="rpcPortVisibility"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************

        private Constants.PostProvInintDisksResultEnum TestPsConnection(
            Models.VmDeploymentRequest vmReq, out string remotePsUrl,
            out PowershellLib.VirtualMachineRemotePowerShell.RpcPortVisibility rpcPortVisibility)
        {
            CmpServiceLib.PostProvDisk ppd = null;

            try
            {
                var connectSuccess = false;
                rpcPortVisibility = PowershellLib.VirtualMachineRemotePowerShell.RpcPortVisibility.None;

                var vmc = CmpInterfaceModel.Models.VmConfig.Deserialize(vmReq.Config);

                var tempHostName = vmReq.TargetVmName + ".cloudapp.net";
                var tempDomain = Utilities.GetXmlInnerText(vmReq.Config, "ComputerName");
                var tempAccountName = Utilities.GetXmlInnerText(vmReq.Config, "AdminUsername");
                var tempPassword = Utilities.GetXmlInnerText(vmReq.Config, "AdminPassword");

                var connection =
                    ServProvAccount.GetAzureServiceAccountConnection(
                    Convert.ToInt32(vmReq.ServiceProviderAccountID), _CmpDbConnectionString);

                if (null == tempAccountName)
                {
                    var defaultRemotingAccount =
                        Microsoft.Azure.CloudConfigurationManager.GetSetting("DefaultRemotingAccount");

                    if (null != defaultRemotingAccount)
                    {
                        var xk = new KryptoLib.X509Krypto(null);
                        defaultRemotingAccount = xk.DecrpytKText(defaultRemotingAccount);

                        var accountParts = defaultRemotingAccount.Split(',');

                        if (2 < accountParts.Count())
                        {
                            tempDomain = accountParts[0];
                            tempAccountName = accountParts[1];
                            tempPassword = accountParts[2];
                        }
                    }

                    string ipAddress;

                    remotePsUrl = PowershellLib.VirtualMachineRemotePowerShell.GetPowerShellUrl(
                        connection.SubcriptionID, connection.Certificate, connection.AdToken, 
                        vmReq.TargetServicename, vmReq.TargetVmName,
                        PowershellLib.VirtualMachineRemotePowerShell.RpcPortVisibility.Private, out ipAddress);

                    try
                    {
                        ppd = new PostProvDisk(remotePsUrl, tempAccountName, tempDomain, tempPassword);
                        remotePsUrl = ipAddress;
                    }
                    catch (PowershellLib.FailToConnectException)
                    {
                        return Constants.PostProvInintDisksResultEnum.FailToConnect;
                    }

                    rpcPortVisibility = PowershellLib.VirtualMachineRemotePowerShell.RpcPortVisibility.Private;
                    return Constants.PostProvInintDisksResultEnum.Success;
                }

                //*** Try to connect to public network RPC port ***

                string ipAddress2;

                remotePsUrl = PowershellLib.VirtualMachineRemotePowerShell.GetPowerShellUrl(
                    connection.SubcriptionID, connection.Certificate, connection.AdToken,
                    vmReq.TargetServicename, tempHostName,
                    PowershellLib.VirtualMachineRemotePowerShell.RpcPortVisibility.PublicHttps, out ipAddress2);

                if (null != remotePsUrl)
                {
                    try
                    {
                        ppd = new PostProvDisk(remotePsUrl, tempAccountName, tempDomain, tempPassword);
                        rpcPortVisibility = PowershellLib.VirtualMachineRemotePowerShell.RpcPortVisibility.PublicHttps;
                        connectSuccess = true;
                    }
                    catch (PowershellLib.FailToConnectException)
                    {
                    }
                }

                if (!connectSuccess)
                {
                    //*** Try to connect to public network RPC port ***

                    remotePsUrl = PowershellLib.VirtualMachineRemotePowerShell.GetPowerShellUrl(
                        connection.SubcriptionID, connection.Certificate, connection.AdToken,
                        vmReq.TargetServicename, tempHostName,
                        PowershellLib.VirtualMachineRemotePowerShell.RpcPortVisibility.PublicHttp, out ipAddress2);

                    if (null != remotePsUrl)
                    {
                        try
                        {
                            ppd = new PostProvDisk(remotePsUrl, tempAccountName, tempDomain, tempPassword);
                            rpcPortVisibility =
                                PowershellLib.VirtualMachineRemotePowerShell.RpcPortVisibility.PublicHttp;
                            connectSuccess = true;
                        }
                        catch (PowershellLib.FailToConnectException)
                        {
                        }
                    }
                }

                //*** Try to connect to private network RPC port ***

                if (!connectSuccess)
                {
                    remotePsUrl = PowershellLib.VirtualMachineRemotePowerShell.GetPowerShellUrl(
                        connection.SubcriptionID, connection.Certificate, connection.AdToken, 
                        vmReq.TargetServicename, tempHostName,
                        PowershellLib.VirtualMachineRemotePowerShell.RpcPortVisibility.PrivateHttps, out ipAddress2);

                    if (null == remotePsUrl)
                    {
                        return Constants.PostProvInintDisksResultEnum.NotFound;
                    }

                    try
                    {
                        ppd = new PostProvDisk(remotePsUrl, tempAccountName, tempDomain, tempPassword);
                    }
                    catch (PowershellLib.FailToConnectException)
                    {
                        return Constants.PostProvInintDisksResultEnum.FailToConnect;
                    }
                }

                //*** Successful connection, you may proceed ***
                //rpcPortVisibility = PowershellLib.VirtualMachineRemotePowerShell.RpcPortVisibility.PrivateHttps;
                return Constants.PostProvInintDisksResultEnum.Success;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ProcessorVm.TestPsConnection() : " +
                    Utilities.UnwindExceptionMessages(ex));
            }
            finally
            {
                if (null != ppd)
                    ppd.Dispose();
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private string BuildAzureHsRequestBody(string requestBody)
        {
            var innerText = Utilities.GetXmlInnerText(requestBody, "HostedServiceConfig");

            if (null == innerText)
                return null;

            if (0 == requestBody.Length)
                return null;

            return string.Format(AZUREHSBODYSHELL, innerText);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private string BuildAzureServiceCertRequestBody(string requestBody)
        {
            var innerText = Utilities.GetXmlInnerText(requestBody, "CertificateFile");

            if (null == requestBody)
                return null;

            if (0 == requestBody.Length)
                return null;

            return string.Format(AZURESERVCERTBODYSHELL, innerText);
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="vmReq"></param>
        /// <param name="impersonate"></param>
        ///  
        //*********************************************************************

        void ChangeOu(Models.VmDeploymentRequest vmReq, bool impersonate)
        {
            try
            {
                var domainJoin = Utilities.GetXmlInnerText(vmReq.Config, "DomainJoin");

                //*** change the OU only if a domain join is specified in config ***
                if (null == domainJoin)
                    return;

                var vmc =
                    CmpInterfaceModel.Models.VmConfig.Deserialize(vmReq.Config);

                //*** change the OU only if the server OU and workstation OU are specified in config ***
                if (null == vmc.Placement)
                    return;

                if (null == vmc.Placement.WorkstationOu)
                    return;

                if (null == vmc.Placement.ServerOu)
                    return;

                //*** run locally

                //string xxx = string.Format(@"$server = [adsi](""LDAP://CN={0},{1}"")", vmReq.TargetVmName, vmc.Placement.WorkstationOu);
                //string yyy = string.Format(@"$destOU = [adsi](""LDAP://{0}"")", vmc.Placement.ServerOu);

                var psCmdList = new List<string>(3)
                {
                    string.Format(@"$server = [adsi](""LDAP://CN={0},{1}"")", vmReq.TargetVmName,
                        vmc.Placement.WorkstationOu),
                    string.Format(@"$destOU = [adsi](""LDAP://{0}"")", vmc.Placement.ServerOu),
                        @"$server.PSBase.MoveTo($DestOU)"
                };

                if (impersonate)
                {
                    var userDomain = Utilities.GetXmlInnerText(domainJoin, "Domain");
                    var userName = Utilities.GetXmlInnerText(domainJoin, "Username");
                    var userPassword = Utilities.GetXmlInnerText(domainJoin, "Password");

                    if (null == userDomain)
                        throw new Exception("Missing 'Domain' element from 'DomainJoin' element in VM Config");
                    if (null == userName)
                        throw new Exception("Missing 'Username' element from 'DomainJoin' element in VM Config");
                    if (null == userPassword)
                        throw new Exception("Missing 'Password' element from 'DomainJoin' element in VM Config");

                    using (new Impersonator(userDomain, userName, userPassword))
                    {
                        //PowershellLib.LocalShell.Execute(psCmdList);
                    }
                }
                else
                {
                    //PowershellLib.LocalShell.Execute(psCmdList);
                }

                //*** run remotely 

                using (var psRem = GetPowershellConnection(vmReq))
                {
                    var commandList = new List<string>(1) { "gpupdate /force" };
                    var rr = psRem.Execute(null, commandList);

                    var remoteErrorDescriptionList = new List<string>();

                    if (rr.HasErrors)
                        foreach (var ED in rr.ErrorDecsriptionList)
                            remoteErrorDescriptionList.Add(string.Format("gpupdate : {0}", ED));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ChangeOu() : " +
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="vmReq"></param>
        /// <param name="psRem"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************

        private CmpServiceLib.Models.VmDeploymentRequest PostQcValidation(
            CmpServiceLib.Models.VmDeploymentRequest vmReq, PowershellLib.Remoting psRem)
        {
            try
            {
                //gather the updated config data prior to post validation
                vmReq.Config = RunPostProcessingDataGathering(vmReq, psRem);
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, "Exception in PostQcValidation() ", 2, 2);
                return vmReq;
            }

            try
            {
                //process the results
                vmReq.CurrentStateStartTime = DateTime.UtcNow;

                //*** Do validation here if you wish
                // adjust vmReq.StatusMessage and vmReq.StatusCode to match validation results

                vmReq.StatusMessage = "Post Validation Passed";
                vmReq.StatusCode = Constants.StatusEnum.PostProvisioningQC.ToString();

                var cdb = new CmpDb(_CmpDbConnectionString);
                cdb.SetVmDepRequestStatus(vmReq, null);
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, "Exception in PostQcValidation() ", 2, 2);
            }

            return vmReq;
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="vmReq"></param>
        /// <param name="psRem"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************

        string RunPostProcessingDataGathering(Models.VmDeploymentRequest vmReq,
            PowershellLib.Remoting psRem)
        {
            try
            {
                //psRem = GetPowershellConnection(vmReq);
                var vmc = CmpInterfaceModel.Models.VmConfig.Deserialize(vmReq.Config);

                //PoSh Commands

                const string POSH_PAGEFILE_LOCATION = "$returnstr = $null;Get-WmiObject win32_pagefile | ForEach-Object {if($returnstr -eq $null){$returnstr = $_.Name}else{$returnstr += \",$($_.Name)\"}};$returnstr";
                const string POSH_TEMPFILELOCATION = "$returnstr = $null; Get-WmiObject Win32_LogicalDisk | Where-Object{$_.VolumeName -eq \"Temporary Storage\"} | ForEach-Object {$returnstr = $_.DeviceID};$returnstr";
                const string POSH_LOCALADMINS = "$ComputerName=(Get-WmiObject Win32_ComputerSystem).Name;$GroupMembers = ([ADSI](\"WinNT://$ComputerName/Administrators,group\")).psbase.invoke(\"Members\") | %{ $_.GetType().InvokeMember(\"ADSPath\",'GetProperty',$Null,$_,$Null) | %{ $_ -replace \"WinNT://\", \"\" -replace '/','\\' } };[string]$adminString = \"\";$GroupMembers | foreach-object { $adminString = $adminString + $_ + ';' }; $adminString";
                const string POSH_DRIVELETTERS = "$DriveLetters=\"\";$Drives = (Get-WmiObject Win32_LogicalDisk -Filter DriveType=3) | ForEach-Object {if($DriveLetters -eq \"\"){$DriveLetters = $_.DeviceID + \";\" + [math]::round($_.FreeSpace/1024/1024/1024,0).ToString() + \";\" + [math]::round($_.Size/1024/1024/1024,0).ToString()}else{$DriveLetters += \",\" + $_.DeviceID + \";\" + [math]::round($_.FreeSpace/1024/1024/1024,0).ToString() + \";\" + [math]::round($_.Size/1024/1024/1024,0).ToString()}};$DriveLetters";
                const string POSH_DOMAIN = "(Get-WmiObject Win32_ComputerSystem).Domain";
                const string POSH_WINDOWSACTIVATION = "$returnstr=$null;if ((Get-WmiObject -Class SoftwareLicensingProduct -Filter \"Name like 'Windows%' and LicenseStatus = 1\") -ne $null){$returnstr = \"true\"}else{$returnstr = \"false\"};$returnstr";
                const string POSH_AZUREVMAGENTINSTALLED = "$returnstr=$null;if(Test-Path C:\\WindowsAzure\\Packages\\GuestAgent){$returnstr = \"true\"}else{$returnstr = \"false\"};$returnstr";
                const string POSH_COMPUTEROU = "$returnstr=$null;$ADSystemInfo = New-Object -ComObject ADSystemInfo; $type = $ADSystemInfo.GetType();$returnstr=$type.InvokeMember('ComputerName','GetProperty',$null,$ADSystemInfo,$null);$returnstr";

                //get the page file location
                vmc.PostInfoFromVM.PageFiles = GetPowerShellDataGathering(vmReq, POSH_PAGEFILE_LOCATION, psRem);

                //get the tempfile location
                vmc.PostInfoFromVM.TempFileDrive = GetPowerShellDataGathering(vmReq, POSH_TEMPFILELOCATION, psRem);

                //get the local admins
                vmc.PostInfoFromVM.LocalAdmins = GetPowerShellDataGathering(vmReq, POSH_LOCALADMINS, psRem);

                //get the drive letters
                vmc.PostInfoFromVM.DriveLetters = GetPowerShellDataGathering(vmReq, POSH_DRIVELETTERS, psRem);

                //get the domain
                vmc.PostInfoFromVM.MachineDomain = GetPowerShellDataGathering(vmReq, POSH_DOMAIN, psRem);

                //check the windows activation
                vmc.PostInfoFromVM.IsWindowsActivated = bool.Parse(GetPowerShellDataGathering(vmReq, POSH_WINDOWSACTIVATION, psRem));

                //check the azure vm agent install
                vmc.PostInfoFromVM.IsAzureVmAgentInstalled = bool.Parse(GetPowerShellDataGathering(vmReq, POSH_AZUREVMAGENTINSTALLED, psRem));

                //get the OU for the machine
                vmc.PostInfoFromVM.MachineOU = GetPowerShellDataGathering(vmReq, POSH_COMPUTEROU, psRem);

                //return the updated config                
                return vmc.Serialize();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in RunPostProcessingDataGathering() : " +
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="vmReq"></param>
        ///  <param name="poShCommand"></param>
        /// <param name="psRem"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************

        string GetPowerShellDataGathering(Models.VmDeploymentRequest vmReq,
            string poShCommand, PowershellLib.Remoting psRem)
        {
            try
            {
                var returnString = string.Empty;

                //get the page file
                var commandList = new List<string>(1) { poShCommand };
                var rr = psRem.Execute(null, commandList);

                var remoteErrorDescriptionList = new List<string>();

                if (rr.HasErrors)
                    remoteErrorDescriptionList.AddRange(rr.ErrorDecsriptionList.Select(ED => 
                        string.Format("GetPowerShellDataGathering : {0}", ED)));

                var vmc = CmpInterfaceModel.Models.VmConfig.Deserialize(vmReq.Config);

                if (null != rr.StringOutput)
                    foreach (var outLine in rr.StringOutput)
                        returnString = outLine;

                return returnString;

            }
            catch (Exception ex)
            {
                throw new Exception("Exception in GetPowerShellDataGathering() : " +
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmReq"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        int GetTargetServiceProviderAccountID(Models.VmDeploymentRequest vmReq)
        {
            var vmc =
                CmpInterfaceModel.Models.VmConfig.Deserialize(vmReq.Config);

            if (null == vmc)
                return _defaultTargetServiceProviderAccountID;

            if (null == vmc.Placement)
                return _defaultTargetServiceProviderAccountID;

            if (0 == NonNull(vmc.Placement.TargetServiceProviderAccountID))
                return _defaultTargetServiceProviderAccountID;

            return vmc.Placement.TargetServiceProviderAccountID;
        }

        //*********************************************************************
        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmReq"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        string GetTargetServiceProviderAccountResourceGroup(Models.VmDeploymentRequest vmReq)
        {
            var vmc =
                CmpInterfaceModel.Models.VmConfig.Deserialize(vmReq.Config);

            if (null == vmc)
                return null;

            if (null == vmc.Placement)
                return null;

            return (vmc.Placement.TargetServiceProviderAccountGroup);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        bool _EnforceAppAgAffinity
        {
            get
            {
                var eaaga =
                    Microsoft.Azure.CloudConfigurationManager.GetSetting("EnforceAppAgAffinity");
                if (null != eaaga)
                    if (eaaga.Equals("false", StringComparison.InvariantCultureIgnoreCase))
                        return false;

                return true;
            }
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="hsName"></param>
        ///  <returns></returns>
        ///  
        ///  
        ///   <summary>
        ///   
        ///   </summary>
        ///   <param name="servProdAcctList"></param>
        ///   <param name="hsName"></param>
        /// <param name="location"></param>
        /// <param name="diskCount"></param>
        ///  <param name="useThisServProdAcct"></param>
        ///  <param name="useThisServiceName"></param>
        ///  <param name="useThisAffinityGroupName"></param>
        ///  <param name="affinityGroupName"></param>
        ///  <returns></returns>
        ///   
        //*********************************************************************

        //*** NOTE * Compute

        AzureAdminClientLib.HostedServiceOps.ServiceAvailabilityEnum 
            CheckServiceNameAvailability(
            IEnumerable<ServiceProviderAccount> servProdAcctList, string hsName,
            string affinityGroupName, string location, int diskCount,
            out Models.ServiceProviderAccount useThisServProdAcct,
            out string useThisServiceName, out string useThisAffinityGroupName)
        {
            useThisServProdAcct = null;
            useThisServiceName = hsName;
            useThisAffinityGroupName = null;
            var foundContainerSpaceAvailable = false;
            CmpDb cdb = null;
            
            try
            {
                foreach (var servProdAcct in servProdAcctList)
                {
                    var connection =
                        ServProvAccount.GetAzureServiceAccountConnection(
                            servProdAcct.AccountID, servProdAcct.CertificateThumbprint, 
                            servProdAcct.AzureADTenantId, servProdAcct.AzureADClientId, servProdAcct.AzureADClientKey);


                    //*** check if we have storage space in this subscription ***

                    AzureStorageAccountContainer container = null;

                    var so = new AzureAdminClientLib.StorageOps(connection);

                    try
                    {
                        if (_EnforceAppAgAffinity)
                            container = so.GetLeastUsedContainer(_DefaultVhdContainerName,
                                affinityGroupName, null, _blobsPerContainerLimit, _vmsPerVnetLimit, diskCount, true);
                        else
                            container = so.GetLeastUsedContainer(_DefaultVhdContainerName,
                                null, location, _blobsPerContainerLimit, _vmsPerVnetLimit, diskCount, false);
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("Out of storage space"))
                            continue;

                        throw;
                    }

                    if (null == container)
                        continue;

                    foundContainerSpaceAvailable = true;

                    //************

                    var hso = new AzureAdminClientLib.HostedServiceOps(connection);

                    for (var index = 0; index < 100; index++)
                        switch (hso.CheckAvailability(hsName, _DeploymentCountLimit, out useThisAffinityGroupName))
                        {
                            case HostedServiceOps.ServiceAvailabilityEnum.DepolymentLimitFull:
                                //*** suggest a new name

                                hsName = GetNextHostServiceName(hsName);

                                //useThisServProdAcct = servProdAcct;
                                //useThisServiceName = hsName;

                                continue;

                            case HostedServiceOps.ServiceAvailabilityEnum.AlredayOwnIt:

                                if (null != affinityGroupName)
                                    if (!affinityGroupName.Equals(useThisAffinityGroupName,
                                        StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        hsName = GetNextHostServiceName(hsName);
                                        useThisServiceName = hsName;
                                        continue;
                                    }

                                if(null == cdb) 
                                    cdb = new CmpDb(_CmpDbConnectionString);

                                if (cdb.IsAssetBad(hsName, Constants.AssetTypeCodeEnum.Hostservice))
                                {
                                    hsName = GetNextHostServiceName(hsName);
                                    useThisServiceName = hsName;
                                    continue;
                                }

                                useThisServProdAcct = servProdAcct;
                                useThisServiceName = hsName;

                                return AzureAdminClientLib.HostedServiceOps.ServiceAvailabilityEnum.AlredayOwnIt;

                            case HostedServiceOps.ServiceAvailabilityEnum.Unavailable:

                                hsName = GetNextHostServiceName(hsName);
                                continue;

                            case HostedServiceOps.ServiceAvailabilityEnum.Available:

                                useThisServProdAcct = servProdAcct;
                                useThisServiceName = hsName;

                                return AzureAdminClientLib.HostedServiceOps.ServiceAvailabilityEnum.Available;
                        }
                }

                if (!foundContainerSpaceAvailable)
                {
                    throw new Exception("Out of storage or vnet space. Allocation would exceed limit of " +
                        _blobsPerContainerLimit + " blobs per container or " +
                        _vmsPerVnetLimit + " vms per vnet");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in CheckServiceNameAvailability() : " +
                    Utilities.UnwindExceptionMessages(ex));
            }

            return AzureAdminClientLib.HostedServiceOps.ServiceAvailabilityEnum.Unavailable;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="servName"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        string GetSafeHostServiceName(string servName)
        {
            servName = new string(servName.Where(c => char.IsLetterOrDigit(c) || c == '-').ToArray());

            if (!Char.IsLetter(servName.FirstOrDefault()))
                servName = "S" + servName;

            //*** always end with a two digit number, to support multiple host services per app (in the future)

            if (12 < servName.Length)
                servName = servName.Substring(0, 12);

            servName = servName + "-01";

            return servName;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="servName"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        string GetNextHostServiceName(string servName)
        {
            //try
            //{
            //    var lastTwoDigits = servName.Substring(servName.Length - 2);

            //    var serviceNameIndex = Convert.ToInt64(lastTwoDigits);

            //    serviceNameIndex += 1;

            //    servName = servName.Substring(0, servName.Length - 3);

            //    if (10 > serviceNameIndex)
            //        servName = string.Format("{0}-0{1}", servName, serviceNameIndex);
            //    else
            //        servName = string.Format("{0}-{1}", servName, serviceNameIndex);

            //    return servName;
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("Exception in GetNextHostServiceName() : " + 
            //        CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex));
            //}
            return servName;
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="vmReq"></param>
        ///  <param name="servProvAcctList"></param>
        /// <param name="servProdAccount"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************

        HttpResponse GetHostService(Models.VmDeploymentRequest vmReq,
            List<Models.ServiceProviderAccount> servProvAcctList,
            out Models.ServiceProviderAccount servProdAccount)
        {
            servProdAccount = null;
            var hsBody = BuildAzureHsRequestBody(vmReq.Config);

            if (null == hsBody)
            {
                vmReq.StatusMessage = "No host service specified";
                vmReq.StatusCode = Constants.StatusEnum.Exception.ToString();
                return new HttpResponse { HadError = true, Retry = false };
            }

            AzureAdminClientLib.HttpResponse resp = null;
            AzureAdminClientLib.Connection connection = null;

            //*** Count disks ***

            var diskCount = 1;

            var vmCfg = CmpInterfaceModel.Models.VmConfig.Deserialize(vmReq.Config);

            if (null != vmCfg.DiskSpecList)
                diskCount += vmCfg.DiskSpecList.Count;

            //*** Do we need to create the hosted service? ***

            string useThisServiceName;
            string useThisAffinityGroupName;

            var serviceName = Utilities.GetXmlInnerText(hsBody, "ServiceName");
            var avail = CheckServiceNameAvailability(servProvAcctList,
                serviceName, null, vmCfg.HostedServiceConfig.Location, diskCount, out servProdAccount, out useThisServiceName, out useThisAffinityGroupName);
            vmReq.Config = vmReq.Config.Replace("<ServiceName>" + serviceName + "</ServiceName>",
                "<ServiceName>" + useThisServiceName + "</ServiceName>");

            switch (avail)
            {
                case AzureAdminClientLib.HostedServiceOps.ServiceAvailabilityEnum.Unavailable:
                    resp = new AzureAdminClientLib.HttpResponse { HadError = true, Body = "Service Name Not Available", HTTP = "" };
                    break;

                case AzureAdminClientLib.HostedServiceOps.ServiceAvailabilityEnum.AlredayOwnIt:

                    connection = ServProvAccount.GetAzureServiceAccountConnection(
                        servProdAccount.AccountID, servProdAccount.CertificateThumbprint,
                        servProdAccount.AzureADTenantId, servProdAccount.AzureADClientId, servProdAccount.AzureADClientKey);

                    resp = SetPlacement(vmReq, useThisAffinityGroupName, connection, servProdAccount);

                    if (resp.HadError)
                    {
                        if (resp.Retry)
                            return new HttpResponse { HadError = true, Retry = true };

                        return new HttpResponse { HadError = true, Retry = false, Body = resp.Body };
                    }

                    resp = new AzureAdminClientLib.HttpResponse { HadError = false, Body = 
                        "Already Own Service Name", StatusCheckUrl = "" };
                    break;

                case AzureAdminClientLib.HostedServiceOps.ServiceAvailabilityEnum.Available:

                    // Go through each of the service provider accounts (a.k.a. subscriptions) and
                    // populate the Azure properties. Currently this is just the core counts.
                    // This needs to be done before calling the Min() function, otherwise all the values
                    // will be zero and the comparison won't be of much value.
                    foreach (var oneProviderAccount in servProvAcctList)
                    {
                        oneProviderAccount.LoadAzureProperties();
                    }

                    //*** Get the provider account with the maximum percentage of available cores ***
                    servProdAccount = servProvAcctList.Max();

                    //*** Get the storage account with the greatest available capacity ***
                    //var storageAccounts = servProdAccount..FetchStorageAccountList();

                    connection = ServProvAccount.GetAzureServiceAccountConnection(
                        servProdAccount.AccountID, servProdAccount.CertificateThumbprint,
                        servProdAccount.AzureADTenantId, servProdAccount.AzureADClientId, servProdAccount.AzureADClientKey);

                    var hso = new AzureAdminClientLib.HostedServiceOps(connection);

                    resp = SetPlacement(vmReq, null, connection, servProdAccount);

                    if (resp.HadError)
                    {
                        if (resp.Retry)
                            return new HttpResponse { HadError = true, Retry = true };

                        return new HttpResponse { HadError = true, Retry = false, Body = resp.Body };
                    }

                    hsBody = BuildAzureHsRequestBody(vmReq.Config);
                    resp = hso.CreateHostedService(hsBody);
                    break;
            }

            if (resp.HadError)
            {
                if (resp.HTTP.Contains("409"))
                {
                    vmReq.StatusCode = Constants.StatusEnum.ReadyForUploadingServiceCert.ToString();
                    vmReq.ExceptionMessage = "";
                    vmReq.StatusMessage = "Service already exists";
                }
                else
                {
                    //haltSequence = true;
                    vmReq.ExceptionMessage = resp.Body;
                    vmReq.CurrentStateStartTime = DateTime.UtcNow;

                    if (avail == HostedServiceOps.ServiceAvailabilityEnum.Unavailable)
                    {
                        vmReq.StatusMessage = "Service name unavailable";
                        vmReq.StatusCode = Constants.StatusEnum.Rejected.ToString();
                    }
                    else
                    {
                        vmReq.StatusMessage = resp.Body;
                        vmReq.StatusCode = Constants.StatusEnum.Exception.ToString();
                    }

                    return new HttpResponse { HadError = true, Retry = false };
                }
            }
            else
            {
                vmReq.StatusCode = Constants.StatusEnum.ReadyForUploadingServiceCert.ToString();
                vmReq.ExceptionMessage = "";
                vmReq.StatusMessage = resp.Body;
                vmReq.ServiceProviderStatusCheckTag = resp.StatusCheckUrl;
                System.Threading.Thread.Sleep(HostedServiceCreationDwellTime);
            }

            return new HttpResponse { HadError = false, Retry = false };
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="vmReq"></param>
        /// <param name="dwellTimeMinutes"></param>
        /// <param name="deleteFromStorage"></param>
        /// <param name="throwIfNotFound"></param>
        ///  
        //*********************************************************************

        public void DeleteVm(Models.VmDeploymentRequest vmReq,
            int dwellTimeMinutes, bool deleteFromStorage, bool throwIfNotFound)
        {
            try
            {
                if (null == vmReq)
                    throw new Exception("vmReq == NULL");

                if (vmReq.Config.Contains("[AUTOVNET]"))
                    return;

                var connection = ServProvAccount.GetAzureServiceAccountConnection(
                    (int)vmReq.ServiceProviderAccountID, _CmpDbConnectionString);

                //*** don't delete VHDs of migrated VMs
                if (vmReq.RequestType.Equals(Constants.RequestTypeEnum.MigrateVm.ToString()))
                    deleteFromStorage = false;

                if (null == connection)
                    throw new Exception("Unable to locate account for given ServiceProviderAccountID");

                var vmo = new AzureAdminClientLib.VmOps(connection);
                var domain = Utilities.GetXmlInnerText(vmReq.Config, "Domain");
                var resp = vmo.Delete(vmReq.TargetVmName, vmReq.TargetServicename, deleteFromStorage, throwIfNotFound);

                if (resp.HadError)
                    throw new Exception(resp.Body);
                else
                {
                    var cmp = new CmpService(_EventLog, _CmpDbConnectionString, _AftsDbConnectionString);

                    try
                    {
                        cmp.DeleteComputerfromAD(vmReq.TargetVmName, domain);
                    }
                    catch (Exception ex)
                    {
                        //*** Ignore for now ***
                        if (!ex.Message.Contains("denied"))
                            throw;
                    }
                }

                //*** Unreserve blob storage ***

                var vmCfg = CmpInterfaceModel.Models.VmConfig.Deserialize(vmReq.Config);

                if (null != vmCfg.Placement)
                {
                    var so = new AzureAdminClientLib.StorageOps(connection);

                    so.FreeContainerSpace(vmCfg.Placement.StorageContainerUrl,
                        vmCfg.Placement.DiskCount, vmReq.ID, vmReq);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in DeleteVm() : " +
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="vmReq"></param>
        ///  <param name="message"></param>
        ///  <param name="cdb"></param>
        /// <param name="ignoreRetryCount"></param>
        /// <param name="reSubmitFullMigration"></param>
        ///  
        //*********************************************************************

        public void ResubmitRequest(Models.VmDeploymentRequest vmReq,
            string message, CmpDb cdb,  bool ignoreRetryCount,
            bool reSubmitFullMigration)
        {
            if (null == vmReq.CurrentStateTryCount)
                vmReq.CurrentStateTryCount = 1;

            if (!ignoreRetryCount)
                if (_CurrentStateTryCountLimit - 1 < vmReq.CurrentStateTryCount)
                    throw new Exception("Exceeded retry limit (" +
                        _CurrentStateTryCountLimit.ToString() + ")");

            if (vmReq.RequestType.Equals(Constants.RequestTypeEnum.NewVM.ToString()))
                vmReq.StatusCode = Constants.StatusEnum.Submitted.ToString();
            else
            {
                if (reSubmitFullMigration)
                {
                    //*** delete existing AFTS request records
                    var adb = new AftsDb(_AftsDbConnectionString);
                    adb.DeleteRequestByTagId(vmReq.ID);

                    //*** fetch original config from migration request
                    var migReq = cdb.FetchMigrationRequest(vmReq.ID, CmpDb.FetchMigrationRequestKeyTypeEnum.DepReqId);
                    vmReq.Config = migReq.Config;

                    vmReq.StatusCode = Constants.StatusEnum.Submitted.ToString();
                    migReq.StatusCode = Constants.StatusEnum.Submitted.ToString();

                    cdb.UpdateVmMigrationRequest(migReq, null);
                }
                else
                    vmReq.StatusCode = Constants.StatusEnum.Transferred.ToString();

                //*** delete disks

                //DeleteVmDisks(vmReq);
            }

            if (null != vmReq.ConfigOriginal)
                vmReq.Config = vmReq.ConfigOriginal;

            vmReq.CurrentStateTryCount++;
            vmReq.CurrentStateStartTime = DateTime.UtcNow;
            vmReq.ExceptionMessage = string.Format("{0} : resubmission count ({1})",
                message, vmReq.CurrentStateTryCount);
            vmReq.StatusMessage = vmReq.ExceptionMessage;
            vmReq.Warnings = null;

            cdb.ResubmitVmDepRequest(vmReq);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmReq"></param>
        /// 
        //*********************************************************************

        /*private void ClearMaintenanceMode(VmDeploymentRequest vmReq)
        {
            if (vmReq.RequestType.Equals(Constants.RequestTypeEnum.MigrateVm.ToString()))
            {
                try
                {
                    var maintModeclient = new MmmServiceClient();
                    var request = string.Format("420^/E^by1{0}^", vmReq.SourceServerName);
                    var result = maintModeclient.ProcessCmdLine(request);
                    LogThis(new Exception(), EventLogEntryType.Information, 
                        "Clear MaintenanceMode : " + result, 119, 100);
                }
                catch (Exception ex)
                {
                    LogThis(ex, System.Diagnostics.EventLogEntryType.Error,
                        "Exception in ClearMaintenanceMode() : " +
                        CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex), 130, 100);
                }
            }
        }*/

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmReq"></param>
        /// <param name="startUtc"></param>
        /// <param name="stopUtc"></param>
        /// <param name="reason"></param>
        /// <param name="comment"></param>
        /// 
        //*********************************************************************

        //public void SetMaintenanceMode(VmDeploymentRequest vmReq, DateTime startUtc, 
        //    DateTime stopUtc, MaintenanceModeReason reason, string comment)
        //{
        //    if (vmReq.RequestType.Equals(Constants.RequestTypeEnum.MigrateVm.ToString()))
        //    {
        //        try
        //        {
        //            var maintModeclient = new MmmServiceClient();

        //            // 0^/A^co1itspmd02ps01.redmond.corp.microsoft.com;aitpolap23.redmond.corp.microsoft.com^2013-03-22 10:00;2013-03-22 11:00^PlannedHardwareMaintenance^Charging Capacitators^

        //            var request = string.Format("0^/A^{0}^{1};{2}^{3}^{4}^",
        //                vmReq.SourceServerName, startUtc, stopUtc, reason, comment);

        //            var result = maintModeclient.ProcessCmdLine(request);

        //            LogThis(new Exception(), EventLogEntryType.Information, 
        //                "Set MaintenanceMode : " + result, 119, 100);
        //        }
        //        catch (Exception ex)
        //        {
        //            LogThis(ex, System.Diagnostics.EventLogEntryType.Error,
        //                "Exception in SetMaintenanceMode() : " +
        //                CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex), 130, 100);
        //        }
        //    }
        //}

        private Constants.OsFamily GetOsFamily(VmDeploymentRequest vmReq)
        {
            try
            {
                var vmc = VmConfig.Deserialize(vmReq.Config);

                if (null != vmc.AzureArmConfig)
                    return null != vmc.AzureArmConfig.properties.template.variables.windowsOSVersion ? 
                        Constants.OsFamily.Windows : Constants.OsFamily.Undefined;

                var role = vmc.AzureVmConfig.RoleList[0] as PersistentVMRole;
                Constants.OsFamily family;
                var found = Enum.TryParse(role.OSVirtualHardDisk.OS, true, out family);

                return found ? family : Constants.OsFamily.Undefined;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in GetOsFamily() " + CmpCommon.Utilities.UnwindExceptionMessages(ex));
            }
        }

        #endregion

        #region --- State Processors ------------------------------------------


//Debug Adrian
        private const int CreateVmMinutesTtl = 5;

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="cdb"></param>
        ///  <param name="assetName"></param>
        /// <param name="assetType"></param>
        /// <param name="problemDescription"></param>
        ///  <param name="tagData"></param>
        ///  <param name="whoReported"></param>
        ///  
        //*********************************************************************

        private void MarkBadAsset(CmpDb cdb, string assetName, 
            Constants.AssetTypeCodeEnum assetType,
            string problemDescription, string tagData, string whoReported)
        {
            if (null == cdb)
                cdb = new CmpDb(_CmpDbConnectionString);

            cdb.InsertBadAsset(assetName, assetType, 
                problemDescription, tagData, whoReported);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Here we check the status of Azure VM provisioning tasks. If the tasks
        /// completes OK then we change the request status to 'CreatedVM', if
        /// an error is reported we change the request status to 'Exception'.
        /// </summary>
        /// 
        //*********************************************************************

        //int _Timeout_CheckCheckVmCreation = 5;
        private const string _ADDUSERTOGROUPTEMPLATE = @"([ADSI]""WinNT://$env:computername/{0},group"").Add(""WinNT://{1}/{2}"")";
        const int DwellTime = 5000;

        #endregion
    }
}
