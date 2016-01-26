//*****************************************************************************
//
// File:
// Author: Mark West (mark.west@microsoft.com)
//
//*****************************************************************************

using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace PowershellLib
{
    //*************************************************************************
    ///
    /// <summary>
    /// 
    /// </summary>
    /// 
    //*************************************************************************

    public class RemotingResult
    {
        public bool ConnectionFailed = false;
        public bool HasErrors = false;
        public Collection<PSObject> Output = null;
        public Collection<ErrorRecord> ErrorList = null;
        public List<string> ErrorDecsriptionList = null;
        public List<string> StringOutput = null;
    }

    //*************************************************************************
    ///
    /// <summary>
    /// Thrown by members of the Utilities class
    /// </summary>
    /// 
    //*************************************************************************
    public class FailToConnectException : Exception
    {
        /// <summary>
        /// Thrown by members of the Utilities class
        /// </summary>
        public FailToConnectException()
        {
        }

        public FailToConnectException(string message)
            : base(message)
        {            
        }
    }

    //*************************************************************************
    ///
    /// <summary>
    /// 
    /// </summary>
    /// 
    //*************************************************************************

    public class LocalShell
    {
        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandList"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public static RemotingResult Execute(List<string> commandList)
        {
            var RR = new RemotingResult();
            RR.Output = null;

            try
            {
                using (var powershell = PowerShell.Create())
                {
                    foreach (var Command in commandList)
                    {
                        if (null == Command)
                            continue;

                        if (0 == Command.Length)
                            continue;

                        powershell.AddScript(Command);
                    }

                    RR.Output = powershell.Invoke();

                    foreach (var ER in powershell.Streams.Error)
                    {
                        if (null == RR.ErrorList)
                        {
                            RR.HasErrors = true;
                            RR.ErrorList = new Collection<ErrorRecord>();
                            RR.ErrorDecsriptionList = new List<string>();
                        }

                        RR.ErrorList.Add(ER);

                        if (null == ER.Exception)
                            RR.ErrorDecsriptionList.Add("Unspecified Error");
                        else
                            RR.ErrorDecsriptionList.Add(ER.Exception.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in PowershellLib.Local.Execute() : " + ex.Message);
            }

            return RR;
        }
    }

    public class Remoting : IDisposable
    {
        Runspace _Runspace = null;

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="userName"></param>
        /// <param name="userPassword"></param>
        /// 
        //*********************************************************************

        public Remoting(string url, string userName, string userPassword)
        {
            GetOpenRunspace(url, userName, userPassword);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="directory"></param>
        /// <param name="userName"></param>
        /// <param name="userPassword"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        Runspace GetOpenRunspace_old(string url, string userName, string userPassword)
        {
            var RR = new RemotingResult();

            try
            {
                if (null != _Runspace)
                    if (_Runspace.RunspaceStateInfo.State == RunspaceState.Opened)
                        return _Runspace;

                var connectTo = new Uri(String.Format("{0}/wsman", url));

                //*** http://social.msdn.microsoft.com/Forums/vstudio/en-US/a0e5b23c-b605-431d-a32f-942d7c5fd843/wsmanconnectioninfo

                var ClearPassword = new char[userPassword.Length];
                var SecString = new System.Security.SecureString();

                foreach (var c in userPassword)
                    SecString.AppendChar(c);

                var PsCred = new PSCredential(userName, SecString);

                var ConnectionInfo = new WSManConnectionInfo(connectTo,
                    "http://schemas.microsoft.com/powershell/Microsoft.PowerShell", PsCred);
                ConnectionInfo.SkipCACheck = true;
                ConnectionInfo.SkipCNCheck = true;
                ConnectionInfo.SkipRevocationCheck = true;

                // http://blogs.msdn.com/b/powershell/archive/2006/04/25/583250.aspx
                //*** Can I use this? Why should I? ***
                /*using (RunspaceInvoke invoker = new RunspaceInvoke())
                {
                    invoker.Invoke("Set-ExecutionPolicy Unrestricted -Scope Process");
                }*/

                _Runspace = RunspaceFactory.CreateRunspace(ConnectionInfo);
                _Runspace.Open();

                return _Runspace;
            }
            catch (System.Management.Automation.Remoting.PSRemotingTransportException ex)
            {
                var Message = ex.Message;
                throw new FailToConnectException();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in Remoting.GetOpenRunspace() : " + ex.Message);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="userName"></param>
        /// <param name="userPassword"></param>
        /// <returns></returns>
        /// 
        /// http://technet.microsoft.com/en-us/library/dd347642.aspx
        /// http://stackoverflow.com/questions/6587426/powershell-remoting-with-ip-address-as-target
        /// http://social.msdn.microsoft.com/Forums/vstudio/en-US/a0e5b23c-b605-431d-a32f-942d7c5fd843/wsmanconnectioninfo
        ///
        //*********************************************************************

        Runspace GetOpenRunspace(string url, string userName, string userPassword)
        {
            var rr = new RemotingResult();
            var impersonate = false;

            try
            {
                if (null != _Runspace)
                    if (_Runspace.RunspaceStateInfo.State == RunspaceState.Opened)
                        return _Runspace;

                var connectTo = new Uri(String.Format("{0}/wsman", url));
                WSManConnectionInfo connectionInfo = null;

                //if (url.ToLower().Contains("https:"))
                if (url.ToLower().Contains("https:") | url.ToLower().Contains("http:"))
                {
                    if (url.ToLower().Contains("http:"))
                        AddHostToLocalTrusedHosts(url);

                    var secString = new System.Security.SecureString();

                    foreach (var c in userPassword)
                        secString.AppendChar(c);

                    var psCred = new PSCredential(userName, secString);

                    connectionInfo = new WSManConnectionInfo(connectTo,
                        "http://schemas.microsoft.com/powershell/Microsoft.PowerShell", psCred);
                }
                else
                {
                    connectionInfo = new WSManConnectionInfo(connectTo);

                    if (null != userName)
                        impersonate = true;
                }

                connectionInfo.SkipCACheck = true;
                connectionInfo.SkipCNCheck = true;
                connectionInfo.SkipRevocationCheck = true;

                // http://blogs.msdn.com/b/powershell/archive/2006/04/25/583250.aspx
                //*** Can I use this? Why should I? ***
                /*using (RunspaceInvoke invoker = new RunspaceInvoke())
                {
                    invoker.Invoke("Set-ExecutionPolicy Unrestricted -Scope Process");
                }*/

                /*if (impersonate)
                {
                    using (new CmpServiceLib.Impersonator(userName, "ImpersonateDomain", userPassword))
                    {
                        _Runspace = RunspaceFactory.CreateRunspace(connectionInfo);
                        _Runspace.Open();
                    }
                }
                else
                {*/
                    _Runspace = RunspaceFactory.CreateRunspace(connectionInfo);
                    _Runspace.Open();
                //}

                return _Runspace;
            }
            catch (System.Management.Automation.Remoting.PSRemotingTransportException ex)
            {
                var message = ex.Message;
                throw new FailToConnectException(UnwindExceptionMessages(ex));
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in Remoting.GetOpenRunspace() : " + ex.Message);
            }
        }

        //*************************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*************************************************************************

        Runspace GetOpenRunspace()
        {
            try
            {
                if (_Runspace?.RunspaceStateInfo.State == RunspaceState.Opened)
                    return _Runspace;

                var connectionInfo = new WSManConnectionInfo();

                // http://blogs.msdn.com/b/powershell/archive/2006/04/25/583250.aspx
                //*** Can I use this? Why should I? ***
                /*using (RunspaceInvoke invoker = new RunspaceInvoke())
                {
                    invoker.Invoke("Set-ExecutionPolicy Unrestricted -Scope Process");
                }*/

                /*if (impersonate)
                {
                    using (new CmpServiceLib.Impersonator(userName, "ImpersonateDomain", userPassword))
                    {
                        _Runspace = RunspaceFactory.CreateRunspace(connectionInfo);
                        _Runspace.Open();
                    }
                }
                else
                {*/
                _Runspace = RunspaceFactory.CreateRunspace(connectionInfo);
                _Runspace.Open();
                //}

                return _Runspace;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in Remoting.GetOpenRunspace() : " + ex.Message);
            }
        }

        //*************************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        /// 
        //*************************************************************************

        public static string UnwindExceptionMessages(Exception ex)
        {
            var message = ex.Message;

            if (null != ex.InnerException)
            {
                ex = ex.InnerException;
                message += " - " + ex.Message;

                if (null != ex.InnerException)
                {
                    ex = ex.InnerException;
                    message += " - " + ex.Message;
                }
            }

            return message;
        }


        //*********************************************************************
        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="directory"></param>
        /// <param name="commandList"></param>
        /// <returns></returns>
        /// 
        // http://msdn.microsoft.com/en-us/library/windows/desktop/system.management.automation.runspaces.wsmanconnectioninfo_members(v=vs.85).aspx
        //
        //*********************************************************************

        public RemotingResult Execute( string directory, List<string> commandList)
        {
            var rr = new RemotingResult {Output = null};

            try
            {
                ServicePointManager.ServerCertificateValidationCallback += 
                    OnServerCertificateValidationCallback;

                using (var powershell = PowerShell.Create())
                {
                    powershell.Runspace = _Runspace;

                    if (null != directory)
                        if (0 < directory.Length)
                            powershell.AddScript("cd " + directory);

                    foreach (var command in commandList)
                    {
                        if (null == command)
                            continue;

                        if (0 == command.Length)
                            continue;

                        powershell.AddScript(command);
                    }

                    rr.Output = powershell.Invoke();

                    if (null != rr) if (null != rr.Output)
                        {
                            rr.StringOutput = new List<string>();

                            foreach (var outObj in rr.Output)
                            {
                                if (null != outObj)
                                    rr.StringOutput.Add(outObj.ToString());
                            }
                        }

                    foreach (var er in powershell.Streams.Error)
                    {
                        if (null == rr.ErrorList)
                        {
                            rr.HasErrors = true;
                            rr.ErrorList = new Collection<ErrorRecord>();
                            rr.ErrorDecsriptionList = new List<string>();
                        }

                        rr.ErrorList.Add(er);

                        if (null == er.Exception)
                            rr.ErrorDecsriptionList.Add("Unspecified Error");
                        else
                            rr.ErrorDecsriptionList.Add(er.Exception.Message);
                    }

                    //_Runspace.Close();
                }
            }
            /*catch (System.Management.Automation.Remoting.PSRemotingTransportException)
            {
                RR.ConnectionFailed = true;
            }*/
            catch (Exception ex)
            {
                throw new Exception("Exception in Remoting.Execute() : " + ex.Message);
            }
            finally
            {
                ServicePointManager.ServerCertificateValidationCallback -= 
                    OnServerCertificateValidationCallback;
            }

            return rr;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// The certificate is self signed, so skip the validation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="policyErrors"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static bool OnServerCertificateValidationCallback(object sender,
            X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true;
        }

        private static readonly object SyncTrustedHosts = new object();

        //*************************************************************************
        ///
        /// <summary>
        /// Locally updates the trusted hosts to have the ipAddress provided.
        /// </summary>
        /// <param name="ipAddress">IP Address to add to local trusted hosts.</param>
        /// 
        //*************************************************************************

        public static void AddHostToLocalTrusedHosts(string ipAddress)
        {
            lock (SyncTrustedHosts)
            {
                try
                {
                    var parts = ipAddress.Split('/');

                    if (2 < parts.Length)
                        ipAddress = parts[2];

                    parts = ipAddress.Split(':');

                    if (1 < parts.Length)
                        ipAddress = parts[0];

                    var currentTrustedHosts = GetListOfIpAddressesFromLocalTrustedHosts().ToList();

                    if (!currentTrustedHosts.Contains(ipAddress))
                    {
                        currentTrustedHosts.Add(ipAddress);
                        var newValue = currentTrustedHosts.Any()
                            ? string.Join(",", currentTrustedHosts)
                            : ipAddress;

                        using (var runspace = RunspaceFactory.CreateRunspace())
                        {
                            runspace.Open();

                            var command = new Command("Set-Item");
                            command.Parameters.Add("Path", @"WSMan:\localhost\Client\TrustedHosts");
                            command.Parameters.Add("Value", newValue);
                            command.Parameters.Add("Force", true);

                            var notUsedOutput = RunLocalCommand(runspace, command);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.ToLower().Contains("access is denied"))
                        throw new Exception("Exception in Remoting.AddHostToLocalTrusedHosts() : (calling process must be run with local admin privileges) : " 
                            + UnwindExceptionMessages(ex));
                    else
                        throw new Exception("Exception in Remoting.AddHostToLocalTrusedHosts() : " 
                            + UnwindExceptionMessages(ex));
                }
            }
        }

        //*************************************************************************
        ///
        /// <summary>
        /// Locally updates the trusted hosts to remove the ipAddress provided (if applicable).
        /// </summary>
        /// <param name="ipAddress">IP Address to remove from local trusted hosts.</param>
        /// 
        //*************************************************************************

        public static void RemoveIpAddressFromLocalTrusedHosts(string ipAddress)
        {
            lock (SyncTrustedHosts)
            {
                var currentTrustedHosts = 
                    GetListOfIpAddressesFromLocalTrustedHosts().ToList();

                if (currentTrustedHosts.Contains(ipAddress))
                {
                    currentTrustedHosts.Remove(ipAddress);

                    // can't pass null must be an empty string...
                    var newValue = currentTrustedHosts.Any() ? 
                        string.Join(",", currentTrustedHosts) : string.Empty;

                    using (var runspace = RunspaceFactory.CreateRunspace())
                    {
                        runspace.Open();

                        var command = new Command("Set-Item");
                        command.Parameters.Add("Path", @"WSMan:\localhost\Client\TrustedHosts");
                        command.Parameters.Add("Value", newValue);
                        command.Parameters.Add("Force", true);

                        var notUsedOutput = RunLocalCommand(runspace, command);
                    }
                }
            }
        }

        //*************************************************************************
        ///
        /// <summary>
        /// Locally updates the trusted hosts to have the ipAddress provided.
        /// </summary>
        /// <returns>List of the trusted hosts.</returns>
        /// 
        //*************************************************************************

        public static ICollection<string> GetListOfIpAddressesFromLocalTrustedHosts()
        {
            lock (SyncTrustedHosts)
            {
                try
                {
                    using (var runspace = RunspaceFactory.CreateRunspace())
                    {
                        runspace.Open();

                        var command = new Command("Get-Item");
                        command.Parameters.Add("Path", @"WSMan:\localhost\Client\TrustedHosts");

                        var response = RunLocalCommand(runspace, command);

                        var valueProperty = response.Single().Properties.Single(_ => _.Name == "Value");

                        var value = valueProperty.Value.ToString();

                        var ret = string.IsNullOrEmpty(value) ? new string[0] : value.Split(',');

                        return ret;
                    }
                }
                catch (Exception remoteException)
                {
                    // if we don't have any trusted hosts then just ignore...
                    if (
                        remoteException.Message.Contains(
                            "Cannot find path 'WSMan:\\localhost\\Client\\TrustedHosts' because it does not exist."))
                    {
                        return new List<string>();
                    }

                    throw;
                }
            }
        }

        //*************************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="runspace"></param>
        /// <param name="arbitraryCommand"></param>
        /// <returns></returns>
        /// 
        //*************************************************************************

        private static List<PSObject> RunLocalCommand(Runspace runspace, 
            Command arbitraryCommand)
        {
            using (var powershell = PowerShell.Create())
            {
                powershell.Runspace = runspace;

                powershell.Commands.AddCommand(arbitraryCommand);

                var output = powershell.Invoke();

                ThrowOnError(powershell, arbitraryCommand.CommandText, "localhost");

                var ret = output.ToList();
                return ret;
            }
        }

        //*************************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="powershell"></param>
        /// <param name="attemptedScriptBlock"></param>
        /// <param name="ipAddress"></param>
        /// 
        //*************************************************************************

        private static void ThrowOnError(PowerShell powershell, 
            string attemptedScriptBlock, string ipAddress)
        {
            if (powershell.Streams.Error.Count > 0)
            {
                var errorString = string.Join(
                    Environment.NewLine,
                    powershell.Streams.Error.Select( _ =>
                        (_.ErrorDetails == null ? null 
                            : _.ErrorDetails.ToString())
                            ?? (_.Exception == null ? 
                            "Naos.WinRM: No error message available" 
                            : _.Exception.ToString())));

                throw new Exception(
                    "Failed to run script (" + attemptedScriptBlock + ") on " 
                    + ipAddress + " got errors: " + errorString);
            }
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
            if (null != _Runspace)
            {
                _Runspace.Close();
                _Runspace.Dispose();
                _Runspace = null;
            }
        }
    }
}
