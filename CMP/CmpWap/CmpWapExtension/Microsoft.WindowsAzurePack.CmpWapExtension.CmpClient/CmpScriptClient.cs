using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmpInterfaceModel.Models;
using CmpServiceLib;
using SMAApi;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient
{
    //*********************************************************************
    ///
    /// <summary>
    /// Gets and executes SMA workflow scripts
    /// </summary>
    /// 
    //*********************************************************************
    public class CmpScriptClient
    {
        //*********************************************************************
        /// 
        /// <summary> Executes a runbook with the specified parameters at a 
        /// specified time
        /// </summary>
        /// <param name="sequence">The sequence spec specifying which runbook 
        /// to run, its parameters, and the time to run the runbook</param>
        /// <returns>The sequence containing details of the runbook's 
        /// execution.</returns>
        ///
        //*********************************************************************
        public static SequenceSpec ExecuteSmaScript(SequenceSpec sequence)
        {
            var pvm = new CmpServiceLib.ProcessorVm(null);
            pvm.ExecuteSmaScript(sequence);
            return sequence;
        }

        //*********************************************************************
        /// 
        /// <summary>
        /// Creates a new sequence spec and returns null
        /// </summary>
        /// <param name="wapSubscriptionId">A WAP subscription ID for the 
        /// script</param>
        /// <returns>null</returns>
        /// 
        //*********************************************************************
        /* todo: remove or complete this */
        public static SequenceSpec GetSmaScripts(string wapSubscriptionId)
        {
            //var rnbookops = new RunBookOperations(sequence.SmaConfig.SmaServerUrl);
            //var runbookJobStatus = rnbookops.GetJobStatus(new Guid(sequence.RunResult.JobId));

            var ss = new SequenceSpec();
            return null;
        }

        //*********************************************************************
        /// 
        /// <summary>
        /// Creates a new sequence spec and returns null
        /// </summary>
        /// <param name="wapSubscriptionId">A WAP subscription ID for the 
        /// script </param>
        /// <param name="providerJobId">ID for the job provider</param>
        /// <returns>null</returns>
        /// 
        //*********************************************************************
        /* todo: remove or complete this */
        public static SequenceSpec GetSmaScript(string wapSubscriptionId, string providerJobId)
        {
            //var rnbookops = new RunBookOperations(sequence.SmaConfig.SmaServerUrl);
            //var runbookJobStatus = rnbookops.GetJobStatus(new Guid(sequence.RunResult.JobId));

            var ss = new SequenceSpec();
            return null;
        }
    }
}
