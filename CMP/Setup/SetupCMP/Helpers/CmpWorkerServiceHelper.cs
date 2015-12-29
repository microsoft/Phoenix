using CMP.Setup.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;

namespace CMP.Setup.Helpers
{
    class CmpWorkerServiceHelper
    {
        /// <summary>
        /// Create the engine service and start it
        /// </summary>
        public static void ConfigureCMPWorkerService(ServiceConfigurationHandler serviceConfigurationHandler)
        {
            AppAssert.AssertNotNull(serviceConfigurationHandler, "serviceConfigurationHandler");

            string installPath = SetupConstants.GetServerInstallPath();

            //attempt to remove services first.
            //this will ignore errors of service does not exist and service marked for deletion
            //If service is marked for deletion, then exception will be thrown at create time as
            //we will be unable to create the service.
            //reason: 
            //1. Keeps the code path of Install and Repair same
            //2. Handles corner case of service already existing in Install mode
            //3. Repairs the configuration of the service if broken

            IntPtr hSCManager = NativeMethods.NullIntPtr;
            IntPtr password = NativeMethods.NullIntPtr;
            try
            {
                hSCManager = ServiceConfigurationHandler.GetSCMHandle();

                //TODO: Handle rollback if exception is thrown
                ServiceConfigurationHandler.StopAndRemoveService(hSCManager, SetupConstants.EngineServiceName);

                //construct paths to service binaries
                string servicePathEngine = PathHelper.QuoteString(installPath + @"\MSIT\CmpWorkerService\" + SetupConstants.EngineServiceBinary);
                SetupLogger.LogInfo("BackEnd.Configure: Engine Service path is : {0}", servicePathEngine);

                //Get account
                string userAccountName = null;
                bool runasLocalAccount = SetupInputs.Instance.FindItem(SetupInputTags.CmpServiceLocalAccountTag);
                if (!runasLocalAccount)
                {
                    userAccountName = UserAccountHelper.GetVmmServiceDomainAccount();
                    password = Marshal.SecureStringToGlobalAllocUnicode(SetupInputs.Instance.FindItem(SetupInputTags.CmpServiceUserPasswordTag));
                }

                //create engine service
                ServiceConfigurationHandler.CreateService(
                    hSCManager,
                    SetupConstants.EngineServiceName,
                    Resources.EngineServiceDisplayName,
                    Resources.EngineServiceDescription,
                    servicePathEngine,
                    null,   // dependent services
                    userAccountName,
                    password:password,
                    autoStart:true,
                    interactive:false);

                //set failure actions for VMMService
                NativeMethods.SERVICE_FAILURE_ACTIONS sfa = new NativeMethods.SERVICE_FAILURE_ACTIONS();
                NativeMethods.SC_ACTION[] sca = new NativeMethods.SC_ACTION[SetupConstants.ServiceActionsCount + 1];
                for (int i = 0; i < SetupConstants.ServiceActionsCount; i++)
                {
                    sca[i].Delay = SetupConstants.ServiceRestartDelay;
                    sca[i].Type = NativeMethods.SC_ACTION_TYPE.SC_ACTION_RESTART;
                }
                sca[SetupConstants.ServiceActionsCount].Delay = 0;
                sca[SetupConstants.ServiceActionsCount].Type = NativeMethods.SC_ACTION_TYPE.SC_ACTION_NONE;

                IntPtr unmanagedStructArray = NativeMethods.NullIntPtr;
                try
                {
                    unmanagedStructArray = GetUnmanagedStructArray(sca);

                    sfa.sc_Action = unmanagedStructArray;
                    sfa.cActions = SetupConstants.ServiceActionsCount + 1;
                    sfa.dwResetPeriod = SetupConstants.ServiceResetPeriod;
                    sfa.lpCommand = null;
                    sfa.lpRebootMsg = null;

                    //set service failure actions for engine service
                    ServiceConfigurationHandler.SetFailureActions(SetupConstants.EngineServiceName, ref sfa);

                    //ConfigurationProgressEvent(this, new EventArgs());
                }
                finally
                {

                    if (NativeMethods.NullIntPtr != unmanagedStructArray)
                        Marshal.FreeHGlobal(unmanagedStructArray);
                }
            }
            finally
            {
                if (!NativeMethods.NullIntPtr.Equals(hSCManager))
                {
                    NativeMethods.CloseServiceHandle(hSCManager);
                }
                if (!NativeMethods.NullIntPtr.Equals(password))
                {
                    Marshal.ZeroFreeGlobalAllocUnicode(password);
                }
            }
        }

        private static IntPtr GetUnmanagedStructArray(NativeMethods.SC_ACTION[] sca)
        {
            IntPtr retPtr;
            retPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeMethods.SC_ACTION)) * sca.Length);
            int offset = 0;
            foreach (NativeMethods.SC_ACTION ac in sca)
            {
                Marshal.WriteInt32(retPtr, offset, (int)ac.Type);
                offset += Marshal.SizeOf(typeof(int));
                Marshal.WriteInt32(retPtr, offset, ac.Delay);
                offset += Marshal.SizeOf(typeof(int));
            }
            return retPtr;
        }
    }

    public enum ServiceDeleteStatus
    {
        ServiceDeleteSuccess       = 0,
        ServiceDeleteFailed        = 1,
        ServiceDeleteRebootPending = 2,
        ServiceDeleteStatusUnknown = 3
    }

    /// <summary>
    /// Collection of static utility methods to be used with service configuration
    /// </summary>
    public class ServiceConfigurationHandler
    {
        /// <summary>
        /// Time out for starting the service
        /// </summary>
        private static readonly TimeSpan ServiceStopTimeout = new TimeSpan(0, 2, 0);

        /// <summary>
        /// Progress tick used to indicate starting of a service
        /// </summary>
        public const int ServiceStartProgressTick = 100;

        /// <summary>
        /// Private constructor to prevent instantiation of the class. This class provides only static methods.
        /// </summary>
        public ServiceConfigurationHandler()
        {
            ConfigurationMessageEvent += new ConfigurationMessageEventHandler(Configuration_ConfigurationMessageEvent);
            ConfigurationProgressEvent += new ConfigurationProgressEventHandler(Configuration_ConfigurationProgressEvent);
        }

        #region Events declared
        #region Configuration UI events

        /// <summary>
        /// Configuration message event arguments
        /// </summary>
        public class ConfigurationMessageEventArgs : EventArgs
        {
            /// <summary>
            /// message indicating the current action being performed by Windows Configuration
            /// </summary>
            private String message;

            /// <summary>
            /// Constructor : initialize the message field
            /// </summary>
            /// <param name="message"></param>
            public ConfigurationMessageEventArgs(String message)
            {
                this.message = message;
            }

            /// <summary>
            /// Read/ Write property for message
            /// </summary>
            public String Message
            {
                get
                {
                    return this.message;
                }
                set
                {
                    this.message = value;
                }
            }
        }

        /// <summary>
        /// delegate for Configuration message event handler
        /// </summary>
        public delegate void ConfigurationMessageEventHandler(Object sender, ConfigurationMessageEventArgs e);

        /// <summary>
        /// delegate for Configuration progress event handler
        /// </summary>
        public delegate void ConfigurationProgressEventHandler(Object sender, EventArgs e);

        #endregion


        public event ConfigurationMessageEventHandler ConfigurationMessageEvent
        {
            add { }
            remove { }
        }

        public event ConfigurationProgressEventHandler ConfigurationProgressEvent
        {
            add { }
            remove { }
        }

        #endregion

        #region Private constants

        /// <summary>
        /// Wait for 2 secs for deleteservice call to take effect. See bug://8665
        /// </summary>
        private const int WaitForDeleteServiceMillis = 2000;

        #endregion

        /// <summary>
        /// This method opens the Service control manager handle with rights to create service and the standard rights.
        /// </summary>
        /// <returns>Handle to SCM</returns>
        public static IntPtr GetSCMHandle()
        {
            IntPtr hSCMgr = NativeMethods.OpenSCManager
                (null, 
                null,
                NativeMethods.SC_MANAGER_LOCK |
                NativeMethods.SC_MANAGER_CREATE_SERVICE |
                NativeMethods.STANDARD_RIGHTS_REQUIRED
                );

            if (NativeMethods.NullIntPtr.Equals(hSCMgr))
            {
                int lastWin32Error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                throw new Exception("Cannot connect to the Service Control Manager (SCM).");
            }

            SetupLogger.LogInfo("BackEnd.Configure: Doing get last error after obtaining SCM handle: {0}", System.Runtime.InteropServices.Marshal.GetLastWin32Error());
            return hSCMgr;
        }

        public static void StopAndDisableService(string serviceName)
        {
            IntPtr hSCManager = NativeMethods.NullIntPtr;

            try
            {
                hSCManager = ServiceConfigurationHandler.GetSCMHandle();
                ServiceConfigurationHandler.StopAndDisableService(hSCManager, serviceName);
            }
            finally
            {
                if (!NativeMethods.NullIntPtr.Equals(hSCManager))
                {
                    NativeMethods.CloseServiceHandle(hSCManager);
                }
            }
        }

        public static void StopAndDisableService(IntPtr hSCManager, string svcName)
        {
            ServiceConfigurationHandler.StopService(svcName);
            int returnCode = SetServiceStatus(hSCManager, svcName, NativeMethods.SERVICE_DISABLED);

            if (returnCode != 0)
            {
                throw new Exception(String.Format("Cannot stop the {0} service", svcName));
            }
        }

        public static void EnableService(IntPtr hSCManager, string svcName)
        {   
            int returnCode = SetServiceStatus(hSCManager, svcName, NativeMethods.SERVICE_AUTO_START);
            if (returnCode != 0)
            {                
                throw new Exception(String.Format("Cannot enable the service {0}", svcName));
            }
        }

        /// <summary>
        /// Sets status of the specified service to the specified value
        /// Ideally this method should throw an exception with the Win32 error value. 
        /// As of now it returns the error code to the caller
        /// </summary>
        /// <param name="hSCManager"></param>
        /// <param name="svcName"></param>
        /// <param name="serviceStatus"></param>
        /// <returns></returns>
        private static int SetServiceStatus(IntPtr hSCManager, string svcName, int serviceStatus)
        {
            AppAssert.Assert(NativeMethods.NullIntPtr != hSCManager, "Invalid SC Manager handle!");
            AppAssert.Assert(null != svcName, "Null service name passed!");

            int returnCode = 0;
            IntPtr svcHandle = NativeMethods.NullIntPtr;
            try
            {
                svcHandle = NativeMethods.OpenService(hSCManager, svcName, NativeMethods.SERVICE_CHANGE_CONFIG);
                if (NativeMethods.NullIntPtr.Equals(svcHandle))
                {
                    int lastWin32Error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                    throw new Exception(String.Format("Cannot connect to the service.\nService name: {0}", svcName));
                }

                NativeMethods.ChangeServiceConfig(
                    svcHandle,
                    NativeMethods.SERVICE_NO_CHANGE,
                    serviceStatus,
                    NativeMethods.SERVICE_NO_CHANGE,
                    null,
                    null,
                    NativeMethods.NullIntPtr,
                    null,
                    null,
                    null,
                    null
                    );

                returnCode = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                return returnCode;
            }
            finally
            {
                if (!NativeMethods.NullIntPtr.Equals(svcHandle))
                {
                    NativeMethods.CloseServiceHandle(svcHandle);
                }
            }
        }

        private static int MapServiceStartModeToNative(ServiceStartMode startMode)
        {
            switch(startMode)
            {
                case ServiceStartMode.Automatic : 
                {
                    return NativeMethods.SERVICE_AUTO_START;
                }
                case ServiceStartMode.Manual : 
                {
                    return NativeMethods.SERVICE_DEMAND_START;
                }
                case ServiceStartMode.Disabled : 
                {
                    return NativeMethods.SERVICE_DISABLED;
                }
                default:
                {
                    AppAssert.Assert (false, "Code should not reach here");
                    return NativeMethods.SERVICE_DEMAND_START;
                }
            }
        }

        public static bool RemoveService(string serviceName)
        {
            bool isRemoved = true;
            IntPtr hSCManager = NativeMethods.NullIntPtr;

            try
            {
                hSCManager = ServiceConfigurationHandler.GetSCMHandle();
                isRemoved = ServiceConfigurationHandler.RemoveService(hSCManager, serviceName);
            }
            catch
            {
                isRemoved = false;
            }
            finally
            {
                if (!NativeMethods.NullIntPtr.Equals(hSCManager))
                {
                    NativeMethods.CloseServiceHandle(hSCManager);
                }
            }

            return isRemoved;
        }

        private static bool RemoveService(IntPtr hSCManager, string serviceName)
        {
            ServiceDeleteStatus status = ServiceConfigurationHandler.StopAndRemoveService(hSCManager, serviceName);
            bool isRemoved = true;

            switch (status)
            {
                case ServiceDeleteStatus.ServiceDeleteFailed:
                    {
                        isRemoved = false;
                        break;
                    }
                case ServiceDeleteStatus.ServiceDeleteSuccess:
                    {
                        isRemoved = true;
                        break;
                    }
                default:
                    {
                        AppAssert.Assert(false, "Unexpected ServiceDeleteStatus returned");
                        isRemoved = false;
                        break;
                    }
            }

            return isRemoved;
        }


        /// <summary>
        /// This method stops a given service and deletes it. The SCM handle passed must have standard rights.
        /// It handles the case of service not being in started mode.
        /// </summary>
        /// <param name="hSCManager">Handle to SCM with standard rights</param>
        /// <param name="svcName">Name of the service to be deleted</param>
        public static ServiceDeleteStatus StopAndRemoveService(IntPtr hSCManager, string svcName)
        {
            AppAssert.Assert(NativeMethods.NullIntPtr != hSCManager, "Invalid SC Manager handle!");
            AppAssert.Assert(null != svcName, "Null service name passed!");

            //return success if the service doesn't exist
            if (ServiceConfigurationHandler.CheckServiceExists(hSCManager, svcName) == false)
            {
                return ServiceDeleteStatus.ServiceDeleteSuccess;
            }

            // stop and delete the service
            try
            {
                //stop the service
                ServiceConfigurationHandler.StopService(svcName);

                ServiceConfigurationHandler.DeleteService(hSCManager, svcName);//throws ServiceConfigurationException if delete failed
            }
            catch
            {
                return ServiceDeleteStatus.ServiceDeleteFailed;
            }

            //state: Delete succeeded
            return ServiceDeleteStatus.ServiceDeleteSuccess;
        }

        /// <summary>
        /// Attempts to stop a service
        /// Waits for timeout period for the service to stop
        /// Logs result and returns.
        /// </summary>
        /// <param name="serviceName"></param>
        private static void StopService(string serviceName)
        {
            AppAssert.Assert(null != serviceName, "Null service name passed!");

            SetupLogger.LogInfo("ServiceConfigurationHandler.StopService: Stop the service {0}", serviceName);

            try
            {
                //InspectHelper.StopService(serviceName);
                using (ServiceController serviceController = new ServiceController(serviceName))
                {
                    // Check if the service is stopped or paused
                    if (serviceController.Status == ServiceControllerStatus.Stopped)
                    {
                        // Service is stopped, log and return
                        return;
                    }
                    serviceController.Stop();
                    serviceController.WaitForStatus(ServiceControllerStatus.Stopped, ServiceStopTimeout);
                }
            }
            catch (Exception e)
            {
                SetupLogger.LogException(e);
                throw new Exception(String.Format("Cannot stop the {0} service", serviceName));
            }
        }

        /// <summary>
        /// Attempts to delete the service
        /// Does nothing if the service doesn't exist
        /// Throws exception only if the delete actually failed
        /// </summary>
        /// <param name="hSCManager"></param>
        /// <param name="svcName"></param>
        private static void DeleteService(IntPtr hSCManager, string svcName)
        {
            AppAssert.Assert(NativeMethods.NullIntPtr != hSCManager, "Invalid SC Manager handle!");
            AppAssert.Assert(null != svcName, "Null service name passed!");

            IntPtr hSvc = NativeMethods.NullIntPtr;
            try
            {
                hSvc = NativeMethods.OpenService(hSCManager, svcName, NativeMethods.DELETE);
                if (NativeMethods.NullIntPtr.Equals(hSvc))
                {
                    int lastWin32Error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                    if (
                        ((NativeMethods.ERROR_SERVICE_DOES_NOT_EXIST != lastWin32Error)
                        && (NativeMethods.ERROR_SERVICE_MARKED_FOR_DELETE != lastWin32Error))
                        )
                    {
                        throw new Exception(String.Format("Cannot connect to the service.\nService name: {0}", svcName));
                    }
                    else
                    {
                        //no exception if the service doesn't exist.
                        return;
                    }
                }

                //got a handle to the service... let's try to delete it.
                bool deleteSvcReturn = NativeMethods.DeleteService(hSvc);
                if (!deleteSvcReturn)
                {
                    int lastWin32Error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                    SetupLogger.LogInfo("Couldn't delete service: Doing get last error: {0}.", lastWin32Error);
                    throw new Exception(String.Format("Setup could not delete service {0}", svcName));
                }
            }
            finally
            {
                if (!NativeMethods.NullIntPtr.Equals(hSvc))
                {
                    NativeMethods.CloseServiceHandle(hSvc);
                }
                SetupLogger.LogInfo("ServiceConfigurationHandler.DeleteService: Doing get last error after closing service handle: {0}", Marshal.GetLastWin32Error());
            }
        }

        /// <summary>
        /// Checks if a service exists.
        /// </summary>
        /// <param name="hSCManager">Handle to SCM</param>
        /// <param name="serviceName">Name of the service</param>
        /// <returns></returns>
        public static bool CheckServiceExists(IntPtr hSCManager, string serviceName)
        {
            AppAssert.Assert(NativeMethods.NullIntPtr != hSCManager, "Invalid SC Manager handle!");
            AppAssert.Assert(null != serviceName, "Null service name passed!");

            IntPtr hSvc = NativeMethods.NullIntPtr;
            try
            {
                hSvc = NativeMethods.OpenService(hSCManager, serviceName, NativeMethods.SERVICE_QUERY_CONFIG);
                if (NativeMethods.NullIntPtr.Equals(hSvc))
                {
                    int lastWin32Error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                    if (NativeMethods.ERROR_SERVICE_DOES_NOT_EXIST == lastWin32Error)
                    {
                        return false;
                    }
                    else
                    {
                        throw new Exception(String.Format("Cannot connect to the service {0}", serviceName));
                    }
                }
                else
                {
                    return true;
                }
            }
            finally
            {
                if (!NativeMethods.NullIntPtr.Equals(hSvc))
                {
                    NativeMethods.CloseServiceHandle(hSvc);
                }
            }
        }

        /// <summary>
        /// This method calls into native code for creating a service.
        /// The service is created with the following standard parameters: 
        /// WIN32_OWN_PROCESS, ERROR_NORMAL, No dependencies
        /// The handle of the service obtained is closed and not returned
        /// </summary>
        /// <param name="hSCManager">Handle to SCM with create service rights</param>
        /// <param name="svcName">Name of service to create</param>
        /// <param name="displayName">Localized display name of the service</param>
        /// <param name="svcDescription">Description of the service</param>
        /// <param name="binaryPath">Path to the binary</param>
        /// <param name="dependenciesArray">Array of dependencies</param>
        /// <param name="serviceStartName">The user account under which the service will run</param>
        /// <param name="password">Password for the user account</param>
        /// <param name="autoStart">Should the service be AutoStart on booting the machine</param>
        /// <param name="interactive">Allow service to interact with desktop or not</param>
        public static void CreateService(IntPtr hSCManager,
            string svcName,
            string displayName,
            string svcDescription,
            string binaryPath,
            string[] dependenciesArray,
            string serviceStartName,
            IntPtr password,
            bool autoStart,
            bool interactive)
        {
            IntPtr hService = NativeMethods.NullIntPtr;
            try
            {
                AppAssert.Assert(null != svcName, "Null service name passed!");
                AppAssert.Assert(null != binaryPath, "Null binary path passed!");

                string dependenciesString = CreateNativeStringArray(dependenciesArray);

                int svcType = NativeMethods.SERVICE_WIN32_OWN_PROCESS;
                if (interactive)
                    svcType = NativeMethods.SERVICE_WIN32_OWN_PROCESS | NativeMethods.SERVICE_INTERACTIVE_PROCESS;

                int svcStartType = NativeMethods.SERVICE_DEMAND_START;
                if (autoStart)
                    svcStartType = NativeMethods.SERVICE_AUTO_START;

                if (!NativeMethods.NullIntPtr.Equals(password)) //if we are using a password we need to gr  ant logon as service permissions to this user.
                {
                    try
                    {
                        NativeMethods.SetRight(serviceStartName, "SeServiceLogonRight", true);
                        NativeMethods.SetRight(serviceStartName, "SeAssignPrimaryTokenPrivilege", true);
                    }
                    catch (Exception exp)
                    {
                        SetupLogger.LogError("Failed to grant user ( " + serviceStartName + " ) logon as service permissions. Error: " + exp.Message);
                    }
                }

                hService = NativeMethods.CreateService(
                    hSCManager,
                    svcName,
                    displayName,
                    NativeMethods.SERVICE_CHANGE_CONFIG,
                    svcType,
                    svcStartType,
                    NativeMethods.SERVICE_ERROR_NORMAL,
                    binaryPath,
                    null,  //load order group
                    NativeMethods.NullIntPtr,  //[out] tagId
                    dependenciesString,
                    serviceStartName,  //Username
                    password
                    );

                if (NativeMethods.NullIntPtr.Equals(hService))
                {

                    SetupLogger.LogInfo("BackEnd.Configure: Got back NULL service handle!");
                    int lastWin32Error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                    throw new Exception(String.Format("Cannot create service {0}", svcName));
                }

                NativeMethods.SERVICE_DESCRIPTION svcDesc = new NativeMethods.SERVICE_DESCRIPTION();
                svcDesc.lpDescription = svcDescription;
                bool success = NativeMethods.ChangeServiceConfig2(hService, NativeMethods.SERVICE_CONFIG_DESCRIPTION, ref svcDesc);

                if (!success)
                {
                    SetupLogger.LogInfo("BackEnd.Configure: Couldn't modify service description!");
                    int lastWin32Error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                    throw new Exception(String.Format("Cannot create service {0}", svcName));
                }

                // Set service SID type. This is required for any service that has a firewall rule targeted for that specific service.
                NativeMethods.SERVICE_SID_INFO svcSidType = new NativeMethods.SERVICE_SID_INFO();
                svcSidType.serviceSidType = NativeMethods.SERVICE_SID_TYPE.SERVICE_SID_TYPE_UNRESTRICTED;
                success = NativeMethods.ChangeServiceConfig2(hService, NativeMethods.SERVICE_CONFIG_SERVICE_SID_INFO, ref svcSidType);

                if (!success)
                {
                    SetupLogger.LogInfo("BackEnd.Configure: Couldn't modify service SID type!");
                    int lastWin32Error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                    throw new Exception(String.Format("Cannot create service {0}", svcName));
                }
            }
            finally
            {
                if (NativeMethods.NullIntPtr != hService)
                {
                    NativeMethods.CloseServiceHandle(hService);
                }
            }
        }

        /// <summary>
        /// This method calls into native code for creating a service.
        /// The service is created with the following standard parameters: 
        /// WIN32_OWN_PROCESS, ERROR_NORMAL, DEMAND_START, No dependencies
        /// The handle of the service obtained is closed and not returned
        /// </summary>
        /// <param name="hSCManager">Handle to SCM with create service rights</param>
        /// <param name="svcName">Name of service to create</param>
        /// <param name="displayName">Localized display name of the service</param>
        /// <param name="svcDescription">Description of the service</param>
        /// <param name="binaryPath">Path to the binary</param>
        /// <param name="dependenciesArray">Array of dependencies</param>
        /// <param name="serviceStartName">The user account under which the service will run</param>
        /// <param name="password">Password for the user account</param>
        /// <param name="interactive">Allow service to interact with desktop or not</param>
        public static void CreateService(IntPtr hSCManager,
            string svcName, 
            string displayName, 
            string svcDescription, 
            string binaryPath,
            string[] dependenciesArray,
            string serviceStartName,
            IntPtr password,
            bool interactive)
        {
            CreateService(hSCManager, 
                svcName, 
                displayName, 
                svcDescription, 
                binaryPath, 
                dependenciesArray, 
                serviceStartName, 
                password, 
                false, //auto start is false for this overload of CreateService
                interactive);
        }

        public static IntPtr RepeatLockServiceDatabase(IntPtr scmHandle)
        {
            const int NumOfTries = 10;
            const int SleepTimeInMilliSeconds = 2000;
            IntPtr lockHandle = NativeMethods.NullIntPtr;

            for (int num = 0; num < NumOfTries; num++)
            {
                lockHandle = NativeMethods.LockServiceDatabase(scmHandle);

                if (!NativeMethods.NullIntPtr.Equals(lockHandle))
                {
                    return lockHandle;
                }

                SetupLogger.LogInfo(
                    "RepeatLockServiceDatabase: sleep {0} seconds before trying to lock again...",
                    SleepTimeInMilliSeconds / 1000);

                System.Threading.Thread.Sleep(SleepTimeInMilliSeconds);
            }

            return NativeMethods.NullIntPtr;
        }

        /// <summary>
        /// Sets the failure actions for the specified service
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="sfa"></param>
        internal static void SetFailureActions(string serviceName, ref NativeMethods.SERVICE_FAILURE_ACTIONS sfa)
        {
            AppAssert.Assert(null != serviceName, "Null service name passed!");

            IntPtr lockHandle = NativeMethods.NullIntPtr;
            IntPtr scmHandle = NativeMethods.NullIntPtr;
            IntPtr svcHandle = NativeMethods.NullIntPtr; 
            try
            {
                scmHandle = ServiceConfigurationHandler.GetSCMHandle();

                //lock the database
                lockHandle = RepeatLockServiceDatabase(scmHandle);
                if (NativeMethods.NullIntPtr.Equals(lockHandle))
                {
                    SetupLogger.LogError("BackEnd.Configure: Got back NULL service handle!");
                    int lastWin32Error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                    throw new Exception(String.Format("Setup was unable to configure service {0}", serviceName));
                }

                svcHandle = NativeMethods.OpenService(
                    scmHandle,
                    serviceName,
                    NativeMethods.SERVICE_CHANGE_CONFIG | NativeMethods.SERVICE_START);//since we set restart as a failure option, the handle should have SERVICE_START rights
                if (NativeMethods.NullIntPtr.Equals(svcHandle))
                {
                    //throw exception here
                    SetupLogger.LogInfo("BackEnd.Configure: Got back NULL service handle!");
                    int lastWin32Error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                    throw new Exception(String.Format("Setup was unable to configure service {0}", serviceName));
                }

                SetupLogger.LogInfo("BackEnd.Configure: Successfully opened {0} service", serviceName);

                bool success = NativeMethods.ChangeServiceConfig2(svcHandle, NativeMethods.SERVICE_CONFIG_FAILURE_ACTIONS, ref sfa);
                if (!success)
                {
                    SetupLogger.LogInfo("BackEnd.Configure: Couldn't modify service description!");
                    int lastWin32Error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                    throw new Exception(String.Format("Setup was unable to configure service {0}", serviceName));
                }
            }
            finally
            {
                if (!NativeMethods.NullIntPtr.Equals(lockHandle))
                {
                    NativeMethods.UnlockServiceDatabase(lockHandle);
                }
                if (!NativeMethods.NullIntPtr.Equals(svcHandle))
                {
                    NativeMethods.CloseServiceHandle(svcHandle);
                }
                if (!NativeMethods.NullIntPtr.Equals(scmHandle))
                {
                    NativeMethods.CloseServiceHandle(scmHandle);
                }
            }
        }

        private static string CreateNativeStringArray(string[] strArray)
        {
            char nullChar = (char)0;//Unicode NULL character. 16 bit zero
            if (null == strArray)
                return null;
            System.Text.StringBuilder retString = new System.Text.StringBuilder();
            foreach (string str in strArray)
            {
                retString.Append(str);
                retString.Append(nullChar);
            }
            retString.Append(nullChar);

            SetupLogger.LogInfo("ServiceConfigurationHandler.CreateNativeStringArray: String created is {0}", retString.ToString());
            return retString.ToString();
        }

        /// <summary>
        /// Starts a particular service
        /// The method times out if service does not reach "Running" status within the timeout period.
        /// </summary>
        /// <param name="serviceName">name of service to start</param>
        /// <exception cref="ServiceConfigurationException">when  service could not be started</exception>
        public void StartService(String serviceName)
        {
            StartService(serviceName, null);
        }

        /// <summary>
        /// Starts a particular service
        /// The method times out if service does not reach "Running" status within the timeout period.
        /// </summary>
        /// <param name="serviceName">name of service to start</param>
        /// <param name="machineName">name of computer on which the service resides</param>
        /// <exception cref="ServiceConfigurationException">when  service could not be started</exception>
        public void StartService(String serviceName, String machineName)
        {
            AppAssert.Assert(null != serviceName, "Null service name passed!");

            SetupLogger.LogInfo("StartService: Start the service {0}", serviceName);
            //ConfigurationMessageEvent(this, new ConfigurationMessageEventArgs(String.Format(Resources.StartingService, serviceName)));
            try
            {
                AppAssert.AssertNotNull(serviceName, "serviceName");

                ServiceController serviceController = null;

                try
                {
                    SetupLogger.LogInfo("Starting service" + serviceName);

                    if (machineName == null)
                    {
                        serviceController = new ServiceController(serviceName); // local host
                    }
                    else
                    {
                        serviceController = new ServiceController(serviceName, machineName);
                    }

                    SetupLogger.LogInfo("StartService: Service {0} status = {1}", serviceName, serviceController.Status);

                    //
                    // Query configurations (credentials and start mode) of service
                    // 
                    ServiceConfig serviceConfig = GetServiceConfig(serviceName, machineName);

                    //
                    // Check if the service is disabled or manual and stopped
                    //
                    if (serviceConfig.StartMode == ServiceStartMode.Disabled ||
                        (serviceConfig.StartMode == ServiceStartMode.Manual &&
                        serviceController.Status == ServiceControllerStatus.Stopped))
                    {

                        SetupLogger.LogInfo(
                            "StartService : service is disabled or manual and not running");

                        throw new Exception(String.Format("Cannot start the service. Service name: {0}", serviceName));
                    }

                    // Check if the service is stopped or paused
                    if (serviceController.Status == ServiceControllerStatus.Running)
                    {
                        // Service is running, log and return

                        SetupLogger.LogInfo("StartService: Service running, check if needs to be restarted");

                        return;
                    }
                    if (serviceController.Status == ServiceControllerStatus.Stopped)
                    {
                        // Service stopped, Start the service

                        SetupLogger.LogInfo("StartService: Service stopped, Start the service");
                        serviceController.Start();
                    }
                    else if (serviceController.Status == ServiceControllerStatus.Paused)
                    {
                        // Service paused, Resume the service

                        SetupLogger.LogInfo("StartService: Service paused, Resume the service");
                        serviceController.Continue();
                    }

                    SetupLogger.LogInfo("StartService: Wait for service to start (timeout = {0})", SetupConstants.ServiceStartTimeout);
                    serviceController.WaitForStatus(ServiceControllerStatus.Running, SetupConstants.ServiceStartTimeout);
                }
                catch (System.ComponentModel.Win32Exception win32Exception)
                {
                    // The native service API failed
                    SetupLogger.LogError("StartService: Couldn't start service! Throwing exception: {0}", win32Exception.ToString());
                    throw new Exception(String.Format("Cannot start the service. Service name: {0}", serviceName));
                }
                catch (InvalidOperationException invalidOperationException)
                {
                    // The service can not be started
                    SetupLogger.LogError("StartService: Couldn't start service! Throwing exception: {0}", invalidOperationException.ToString());
                    throw;
                }
                catch (System.ServiceProcess.TimeoutException timeoutException)
                {
                    // There was a timeout
                    SetupLogger.LogError("StartService: Couldn't start service! Throwing exception: {0}", timeoutException.ToString());
                    throw new Exception(String.Format("Cannot start the service. Service name: {0}", serviceName));
                }
                finally
                {
                    if (serviceController != null)
                    {
                        serviceController.Close();
                    }
                }

            }
            catch (Exception serviceNotFoundException)
            {
                SetupLogger.LogInfo("ServiceConfigurationHandler.StartService: Start the service, exception {0}", serviceNotFoundException);
                throw new Exception(String.Format("Cannot start the service. Service name: {0}", serviceName));
            }
        }

        /// <summary>
        /// Container used as output type
        /// Contains Service credentials and start mode
        /// </summary>
        [CLSCompliant(false), ComVisible(false)]
        public struct ServiceConfig
        {
            /// <summary>
            /// service account name (credentails) e.g. LocalService
            /// </summary>
            String serviceCredentials;

            /// <summary>
            /// Start Mode of service e.g. Automatic
            /// </summary>
            ServiceStartMode startMode;

            /// <summary>
            /// Read/Write property for service credentials
            /// </summary>
            public String ServiceCredentials
            {
                get
                {
                    return this.serviceCredentials;
                }
                set
                {
                    this.serviceCredentials = value;
                }
            }

            /// <summary>
            /// Read/Write property for start mode
            /// </summary>
            public ServiceStartMode StartMode
            {
                get
                {
                    return this.startMode;
                }
                set
                {
                    this.startMode = value;
                }
            }
        };

        public static ServiceConfig GetServiceConfig(String serviceName, String machineName)
        {
            if (serviceName == null)
            {
                SetupLogger.LogInfo("InspectConfigs.GetServiceConfig : Service name parametr is null");

                throw new Exception("Service name is null");
            }

            ServiceConfig serviceConfig = new ServiceConfig();

            NativeMethods.QUERY_SERVICE_CONFIGW serviceConfigQueryResult = new NativeMethods.QUERY_SERVICE_CONFIGW();

            IntPtr schSCManager = new IntPtr(0);
            IntPtr schService = new IntPtr(0);

            int result = 0;

            UInt32 dwBytesNeeded = 0;

            try
            {
                //
                // Open handle to Service control manager
                //

                schSCManager = NativeMethods.OpenSCManager(
                    machineName, // null for local machine
                    null, // ServicesActive database 
                    NativeMethods.SC_MANAGER_CONNECT);

                if (NativeMethods.NullIntPtr.Equals(schSCManager))
                {
                    //
                    // Could not open service database
                    //

                    result = Marshal.GetLastWin32Error();
                    SetupLogger.LogError("GetServiceConfig : Unknown error while connecting to OpenSCManager");

                    throw new Exception("Cannot connect to service manager");
                }

                //
                // Open handle to the service
                //

                schService = NativeMethods.OpenService(
                    schSCManager,           // SCManager database 
                    serviceName,			// name of service 
                    NativeMethods.SERVICE_QUERY_CONFIG);  // need QUERY access 

                if (NativeMethods.NullIntPtr.Equals(schService))
                {
                    //
                    // Could not open service
                    //

                    result = Marshal.GetLastWin32Error();

                    switch (result)
                    {
                        case NativeMethods.ERROR_SERVICE_DOES_NOT_EXIST:
                            {
                                SetupLogger.LogInfo("GetServiceConfig : Service not found, ERROR_SERVICE_DOES_NOT_EXIST");

                                throw new Exception(String.Format("OpenService returned error ERROR_SERVICE_DOES_NOT_EXIST (Service not found) for service {0}", serviceName));
                            }
                        default:
                            {
                                SetupLogger.LogError("GetServiceConfig : Unknown error while connecting to Service");

                                throw new Exception("GetServiceConfig : Unknown error while connecting to Service");
                            }
                    }
                }

                //
                // Get the configuration information (Query SCM) 
                //

                UInt32 size = (UInt32)Marshal.SizeOf(serviceConfigQueryResult);
                bool queryResult = NativeMethods.QueryServiceConfig(
                    schService,
                    ref serviceConfigQueryResult,
                    size,
                    ref dwBytesNeeded);

                if (queryResult == false)
                {
                    //
                    // Query to SCM for service configuration failed
                    //

                    result = Marshal.GetLastWin32Error();

                    SetupLogger.LogError("InspectConfigs.GetServiceConfig : Unknown error while querying service configuration information");

                    throw new Exception(String.Format("Error getting the service configuration for service {0}", serviceName));
                }

                //
                // Set service credentials parameter in output structure ServiceConfig
                //

                serviceConfig.ServiceCredentials = Marshal.PtrToStringAuto(serviceConfigQueryResult.lpServiceStartName);

                //
                // Set start mode parameter in output structure ServiceConfig
                //

                if (serviceConfigQueryResult.dwStartType == NativeMethods.SERVICE_AUTO_START)
                {
                    //
                    // Service AUTO start
                    //

                    serviceConfig.StartMode = ServiceStartMode.Automatic;
                }
                else if (serviceConfigQueryResult.dwStartType == NativeMethods.SERVICE_DISABLED)
                {
                    //
                    // Service disabled (can not be started)
                    //

                    serviceConfig.StartMode = ServiceStartMode.Disabled;
                }
                else
                {
                    //
                    // The .NET enumeration does not define SERVICE_BOOT_START and SERVICE_SYSTEM_START
                    // Hence we set it to manual. Service needs to be started manually
                    //

                    serviceConfig.StartMode = ServiceStartMode.Manual;
                }
            }
            finally
            {
                // cleanup
                if (!NativeMethods.NullIntPtr.Equals(schService))
                    NativeMethods.CloseServiceHandle(schService);

                if (!NativeMethods.NullIntPtr.Equals(schSCManager))
                    NativeMethods.CloseServiceHandle(schSCManager);
            }


            return serviceConfig;
        }

        /// <summary>
        /// Restarts a particular service
        /// The method times out if service does not reach "Running" status within the timeout period.
        /// </summary>
        /// <param name="serviceName">name of service to start</param>
        /// <exception cref="ServiceConfigurationException">when  service could not be started</exception>
        public void RestartService(String serviceName)
        {
            AppAssert.Assert(null != serviceName, "Null service name passed!");

            SetupLogger.LogInfo("ServiceConfigurationHandler.RestartService: Start the service {0}", serviceName);

            try
            {
                this.StartService(serviceName);
            }
            catch (Exception serviceNotFoundException)
            {
                SetupLogger.LogInfo("ServiceConfigurationHandler.StartService: Start the service, exception {0}", serviceNotFoundException);
                throw new Exception(String.Format("Cannot start the service. Service name: {0}", serviceName));
            }
        }

        private void Configuration_ConfigurationMessageEvent(Object sender, ConfigurationMessageEventArgs e)
        {

            SetupLogger.LogInfo("ServiceConfigurationHandler : Configure message event : {0}", e.Message);
        }

        private void Configuration_ConfigurationProgressEvent(Object sender, EventArgs e)
        {

            SetupLogger.LogInfo("ServiceConfigurationHandler : Configure progress event");
        }
    }
}
