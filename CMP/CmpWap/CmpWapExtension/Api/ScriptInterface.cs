//*****************************************************************************
// File: ScriptInterface.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: This class contains methods that can be used to call external
//          PowerShell Scripts
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using CmpInterfaceModel.Models;
using Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;
using SMAApi;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api
{
    class ScriptInterface
    {
        //*********************************************************************
        ///
        /// <summary>
        /// This method is used to convert to SequenceSpec type
        /// </summary>
        /// <param name="scriptJob"></param>
        /// <returns>SequenceSpec</returns>
        /// 
        //*********************************************************************

        private static SequenceSpec Convert(ScriptJob scriptJob)
        {
            if (null == scriptJob)
                return null;

            try
            {
                return new SequenceSpec
                {
                    BreakOn = scriptJob.BreakOn,
                    Config = scriptJob.Config,
                    Engine = scriptJob.Engine,
                    ErrorList = scriptJob.ErrorList,
                    ExecuteInState = scriptJob.ExecuteInState,
                    ID = scriptJob.ID,
                    Locale = scriptJob.Locale,
                    Name = scriptJob.Name,
                    ResultCode = scriptJob.ResultCode,
                    RunResult = Convert(scriptJob.RunResult),
                    ScriptList = scriptJob.ScriptList,
                    SmaConfig = Convert(scriptJob.SmaConfig),
                    TagData = scriptJob.TagData,
                    TimeoutMinutes = scriptJob.TimeoutMinutes,
                    Waitmode = scriptJob.Waitmode,
                    TargetName = scriptJob.TargetName,
                    TargetTypeCode = scriptJob.TargetTypeCode
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ScriptInterface.Convert() : " +
                    CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to convert to SequenceRunResultSpec type
        /// </summary>
        /// <param name="scriptJobRr"></param>
        /// <returns>SequenceRunResultSpec</returns>
        /// 
        //*********************************************************************

        private static SequenceRunResultSpec Convert(ScriptJobRunResultSpec scriptJobRr)
        {
            if (null == scriptJobRr)
                return null;

            try
            {
                return new SequenceRunResultSpec
                {
                    JobId = scriptJobRr.JobId,
                    LastUpdate = scriptJobRr.LastUpdate,
                    Output = Convert(scriptJobRr.Output),
                    StatusCode = scriptJobRr.StatusCode,
                    StatusMessage = scriptJobRr.StatusMessage,
                    WhenSubmitted = scriptJobRr.WhenSubmitted
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ScriptInterface.Convert() : " +
                    CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to provide an output of a run
        /// </summary>
        /// <param name="scriptJobRol"></param>
        /// <returns>list of SequenceRunOuputSpec</returns>
        /// 
        //*********************************************************************

        private static List<SequenceRunOuputSpec> Convert(List<ScriptJobRunOuputSpec> scriptJobRol)
        {
            if (null == scriptJobRol)
                return null;

            try
            {
                var srol = new List<SequenceRunOuputSpec>();

                foreach (var scriptJobRo in scriptJobRol)
                {
                    srol.Add(new SequenceRunOuputSpec() 
                    { 
                        Output = scriptJobRo.Output, 
                        When = scriptJobRo .When
                    });
                }
                return srol;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ScriptInterface.Convert() : " +
                    CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to return SmaConfig Spec type
        /// </summary>
        /// <param name="scriptJobSc"></param>
        /// <returns>SmaConfigSpec</returns>
        /// 
        //*********************************************************************

        private static SmaConfigSpec Convert(ScriptJobSmaConfigSpec scriptJobSc)
        {
            if (null == scriptJobSc)
                return null;

            try
            {
                return new SmaConfigSpec()
                {
                    SmaServerUrl = scriptJobSc.SmaServerUrl,
                    ParamList = Convert(scriptJobSc.ParamList),
                    RunbookId = scriptJobSc.RunbookId,
                    RunbookName = scriptJobSc.RunbookName
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ScriptInterface.Convert() : " +
                    CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to return PoshParam Spec type
        /// </summary>
        /// <param name="scriptJobPpl"></param>
        /// <returns>list of PoshParamSpec</returns>
        /// 
        //*********************************************************************

        private static List<PoshParamSpec> Convert(List<ScriptJobPoshParamSpec> scriptJobPpl)
        {
            if (null == scriptJobPpl)
                return null;

            var ppl = new List<PoshParamSpec>();

            try
            {
                foreach (var scriptJobPp in scriptJobPpl)
                {
                    ppl.Add(new PoshParamSpec()
                    {
                        Name = scriptJobPp.Name,
                        Value = scriptJobPp.Name,
                        SelectReachableHost = scriptJobPp.SelectReachableHost

                    });
                }
                return ppl;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ScriptInterface.Convert() : " +
                    CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to return ScriptJob type
        /// </summary>
        /// <param name="sequenceReq"></param>
        /// <returns>ScriptJob</returns>
        /// 
        //*********************************************************************

        private static ScriptJob Convert(SequenceRequest sequenceReq)
        {
            try
            {
                var ss = Convert(SequenceSpec.Deserialize(sequenceReq.Config));
                return ss;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ScriptInterface.Convert() : " +
                    CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to return list of ScriptJob type
        /// </summary>
        /// <param name="sequenceReqList"></param>
        /// <returns>list of ScriptJob</returns>
        /// 
        //*********************************************************************

        private static List<ScriptJob> Convert(List<SequenceRequest> sequenceReqList)
        {
            try
            {
                return sequenceReqList.Select(
                    sequenceReq => Convert(SequenceSpec.Deserialize(sequenceReq.Config))).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ScriptInterface.Convert() : " +
                    CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to return list of ScriptJob type
        /// </summary>
        /// <param name="sequenceList"></param>
        /// <returns>list of ScriptJob</returns>
        /// 
        //*********************************************************************

        private static List<ScriptJob> Convert(IEnumerable<SequenceSpec> sequenceList)
        {
            try
            {
                return sequenceList.Select(
                    sequence => Convert(sequence)).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ScriptInterface.Convert() : " +
                    CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to convert to ScriptJob type
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns>list of ScriptJob</returns>
        /// 
        //*********************************************************************

        private static ScriptJob Convert(SequenceSpec sequence)
        {
            if (null == sequence)
                return null;

            try
            {
                return new ScriptJob
                {
                    BreakOn = sequence.BreakOn,
                    Config = sequence.Config,
                    Engine = sequence.Engine,
                    ErrorList = sequence.ErrorList,
                    ExecuteInState = sequence.ExecuteInState,
                    ID = sequence.ID,
                    Locale = sequence.Locale,
                    Name = sequence.Name,
                    ResultCode = sequence.ResultCode,
                    RunResult = Convert(sequence.RunResult),
                    ScriptList = sequence.ScriptList,
                    SmaConfig = Convert(sequence.SmaConfig),
                    TagData = sequence.TagData,
                    TimeoutMinutes = sequence.TimeoutMinutes,
                    Waitmode = sequence.Waitmode, 
                    TargetName = sequence.TargetName,
                    TargetTypeCode = sequence.TargetTypeCode
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ScriptInterface.Convert() : " +
                    CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to convert to ScriptJobRunResultSpec type
        /// </summary>
        /// <param name="scriptJobRr"></param>
        /// <returns>list of ScriptJobRunResultSpec</returns>
        /// 
        //*********************************************************************

        private static ScriptJobRunResultSpec Convert(SequenceRunResultSpec scriptJobRr)
        {
            if (null == scriptJobRr)
                return null;

            try
            {
                return new ScriptJobRunResultSpec
                {
                    JobId = scriptJobRr.JobId,
                    LastUpdate = scriptJobRr.LastUpdate,
                    Output = Convert(scriptJobRr.Output),
                    StatusCode = scriptJobRr.StatusCode,
                    StatusMessage = scriptJobRr.StatusMessage,
                    WhenSubmitted = scriptJobRr.WhenSubmitted
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ScriptInterface.Convert() : " +
                    CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to return a list of ScriptJobRunOuputSpec
        /// </summary>
        /// <param name="scriptJobRol"></param>
        /// <returns>list of ScriptJobRunOuputSpec</returns>
        /// 
        //*********************************************************************

        private static List<ScriptJobRunOuputSpec> Convert(List<SequenceRunOuputSpec> scriptJobRol)
        {
            if (null == scriptJobRol)
                return null;

            var srol = new List<ScriptJobRunOuputSpec>();

            try
            {
                foreach (var scriptJobRo in scriptJobRol)
                {
                    srol.Add(new ScriptJobRunOuputSpec()
                    {
                        Output = scriptJobRo.Output,
                        When = scriptJobRo.When
                    });
                }
                return srol;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ScriptInterface.Convert() : " +
                    CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is sued to return an object of type ScriptJobSmaConfigSpec
        /// </summary>
        /// <param name="scriptJobSc"></param>
        /// <returns>ScriptJobSmaConfigSpec</returns>
        /// 
        //*********************************************************************

        private static ScriptJobSmaConfigSpec Convert(SmaConfigSpec scriptJobSc)
        {
            if (null == scriptJobSc)
                return null;

            try
            {
                return new ScriptJobSmaConfigSpec()
                {
                    SmaServerUrl = scriptJobSc.SmaServerUrl,
                    ParamList = Convert(scriptJobSc.ParamList),
                    RunbookId = scriptJobSc.RunbookId,
                    RunbookName = scriptJobSc.RunbookName
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ScriptInterface.Convert() : " +
                    CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is sued to return a list of ScriptJobPoshParamSpec type
        /// </summary>
        /// <param name="scriptJobPpl"></param>
        /// <returns>list of ScriptJobPoshParamSpec</returns>
        /// 
        //*********************************************************************

        private static List<ScriptJobPoshParamSpec> Convert(List<PoshParamSpec> scriptJobPpl)
        {
            if (null == scriptJobPpl)
                return null;

            var ppl = new List<ScriptJobPoshParamSpec>();

            try
            {
                foreach (var scriptJobPp in scriptJobPpl)
                {
                    ppl.Add(new ScriptJobPoshParamSpec()
                    {
                        Name = scriptJobPp.Name,
                        Value = scriptJobPp.Name,
                        SelectReachableHost = scriptJobPp.SelectReachableHost

                    });
                }
                return ppl;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ScriptInterface.Convert() : " +
                    CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method returns a ScriptJob type
        /// </summary>
        /// <param name="scriptJob"></param>
        /// <param name="wapSubscriptionId"></param>
        /// <returns>ScriptJob</returns>
        /// 
        //*********************************************************************

        public static ScriptJob SubmitScriptJob(ScriptJob scriptJob,
            string wapSubscriptionId)
        {
            if (scriptJob == null) 
                throw new ArgumentNullException("scriptJob");

            if (wapSubscriptionId == null) 
                throw new ArgumentNullException("wapSubscriptionId");

            try
            {
                var seqSpec = Convert(scriptJob);
                seqSpec = CmpClient.CmpScriptClient.ExecuteSmaScript(seqSpec);

                var sequenceRequest = new SequenceRequest
                {
                    Active = true,
                    CmpRequestID = 0,
                    Config = seqSpec.Serialize(),
                    ExceptionMessage = null,
                    Id = 0,
                    LastStatusUpdate = seqSpec.RunResult.LastUpdate,
                    ServiceProviderJobId = seqSpec.RunResult.JobId,
                    ServiceProviderName = seqSpec.SmaConfig.SmaServerUrl,
                    ServiceProviderTypeCode = seqSpec.Engine,
                    StatusCode = seqSpec.RunResult.StatusCode,
                    StatusMessage = seqSpec.RunResult.StatusMessage,
                    TagData = seqSpec.TagData,
                    TagID = 0,
                    TagOpName = seqSpec.SmaConfig.RunbookName,
                    TargetName = scriptJob.TargetName,
                    TargetTypeCode = scriptJob.TargetTypeCode,
                    WapSubscriptionID = wapSubscriptionId,
                    Warnings = null,
                    WhenRequested = seqSpec.RunResult.WhenSubmitted,
                    WhoRequested = null
                };

                var cdb = new CmpWapDb();
                sequenceRequest = cdb.InsertSequenceRequest(sequenceRequest);
                seqSpec.ID = sequenceRequest.Id;

                return Convert(seqSpec);
            }
            catch (Exception ex)
            {
                throw new Exception("ScriptInterface.SubmitScriptJob() : " +
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method returns a list of ScriptJob type
        /// </summary>
        /// <param name="wapSubscriptionId"></param>
        /// <returns>list of ScriptJob</returns>
        /// 
        //*********************************************************************

        public static List<ScriptJob> GetScriptJobList(string wapSubscriptionId)
        {
            try
            {
                var cdb = new CmpWapDb();
                return Convert(cdb.FetchSequenceRequests(wapSubscriptionId));
            }
            catch (Exception ex)
            {
                throw new Exception("ScriptInterface.GetScriptJobList() : " +
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     Thie method returns a ScriptJob type
        /// </summary>
        /// <param name="wapSubscriptionId"></param>
        /// <param name="jobId"></param>
        /// <returns>ScriptJob</returns>
        /// 
        //*********************************************************************

        public static ScriptJob GetScriptJob(string wapSubscriptionId, string jobId)
        {
            try
            {
                var cdb = new CmpWapDb();
                var sr = cdb.FetchSequenceRequest(wapSubscriptionId, jobId);

                if (null == sr)
                {
                    if (null == wapSubscriptionId)
                        throw new Exception(string.Format("No record found for job '{0}' in any subscription", jobId));
                    else
                        throw new Exception(string.Format("No record found for job '{0}' in subscription '{1}' ", jobId,
                            wapSubscriptionId));
                }

                var ss = Convert(Convert(sr));
                
                CheckSequenceStatus(ss);
                return Convert(ss);
            }
            catch (Exception ex)
            {
                throw new Exception("ScriptInterface.GetScriptJob() : " +
                    Utilities.UnwindExceptionMessages(ex));
            }

        }

        /// <summary>
        ///     This method is used to check the status of a Script Job
        /// </summary>
        /// <param name="scriptSpec"></param>
        /// <returns>Sequence specification</returns>
        public static SequenceSpec CheckSequenceStatus(SequenceSpec scriptSpec)
        {
            try
            {
                var hadError = false;
                string jobErrorMessage = null;  // IPAK Job Error Message

                if (scriptSpec.Engine.Equals(SequenceSpec.SequenceEngineEnum.SMA.ToString(),
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    var now = DateTime.UtcNow;
                    var rnbookops = new RunBookOperations(scriptSpec.SmaConfig.SmaServerUrl);
                    var runbookJobStatus = rnbookops.GetJobStatus(new Guid(scriptSpec.RunResult.JobId));
                    var runbookJobOutput = rnbookops.GetJSONJobOutput(new Guid(scriptSpec.RunResult.JobId));

                    if (null == scriptSpec.RunResult.Output)
                        scriptSpec.RunResult.Output = new List<SequenceRunOuputSpec>(1);

                    if (null != runbookJobStatus.Exception)
                    {
                        hadError = true;
                        scriptSpec.ResultCode = "Exception";
                        scriptSpec.RunResult.StatusCode = "Exception";
                        jobErrorMessage = runbookJobStatus.Exception;
                        scriptSpec.RunResult.Output.Add(new SequenceRunOuputSpec() { Output = jobErrorMessage, When = now });
                    }
                    else if (runbookJobOutput == null)
                    {
                        hadError = true;
                        scriptSpec.ResultCode = "Exception";
                        scriptSpec.RunResult.StatusCode = "Exception";
                        jobErrorMessage = "Unable to get Job information from the SMA environment";
                        scriptSpec.RunResult.Output.Add(new SequenceRunOuputSpec() { Output = jobErrorMessage, When = now });
                    }
                    else if (runbookJobOutput.Result == "Fail")
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
                }

                return scriptSpec;
            }

            catch (Exception ex)
            {
                throw new Exception("Exception in CheckSequencesStatus() : " +
                    CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex));
            }
        }

    }
}
