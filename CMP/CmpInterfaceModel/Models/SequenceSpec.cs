using System;
using System.Collections.Generic;

namespace CmpInterfaceModel.Models
{
    public class PoshParamSpec
    {
        public string Name;
        public string Value;
        public bool SelectReachableHost = false;
    }

    public class SmaConfigSpec
    {
        public string SmaServerUrl { set; get; }
        public string RunbookId { set; get; }
        public string RunbookName { set; get; }
        public List<PoshParamSpec> ParamList { set; get; }
    }

    public class SequenceRunOuputSpec
    {
        public DateTime When { set; get; }
        public string Output { set; get; }
    }

    public class SequenceRunResultSpec
    {
        public string JobId { set; get; }
        public string StatusCode { set; get; }
        public string StatusMessage { set; get; }
        public DateTime LastUpdate { set; get; }
        public DateTime WhenSubmitted { set; get; }
        public List<SequenceRunOuputSpec> Output { set; get; }
    }

    public class SequenceSpec
    {
        public enum SequenceEngineEnum
        {
            Powershell,
            SMA
        };

        public enum SequenceLocaleEnum
        {
            Local,
            Remote
        };

        public enum BreakOnEnum
        {
            None,
            Warning,
            Exception
        };

        public enum WaitmodeEnum
        {
            Synchronous,
            Asynchronous
        };

        public enum StatusEnum
        {
            NotSubmitted,
            Submitted,
            Complete,
            Exception
        };

        public int ID { set; get; }
        public string Name { set; get; }
        public string Engine { set; get; }
        public string Locale { set; get; }
        public string BreakOn { set; get; }
        public string Waitmode { set; get; }
        public int TimeoutMinutes { set; get; }
        public List<string> ScriptList { set; get; }
        public string ExecuteInState { set; get; }
        public string ResultCode { set; get; }
        public string Config { set; get; }
        public SmaConfigSpec SmaConfig { set; get; }
        public string TargetName { set; get; }
        public string TargetTypeCode { set; get; }
        public string TagData { set; get; }
        public List<string> ErrorList { set; get; }
        public SequenceRunResultSpec RunResult { set; get; }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public string Serialize()
        {
            return Utilities.Serialize(typeof(SequenceSpec), this);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public static SequenceSpec Deserialize(string input)
        {
            try
            {
                return Utilities.DeSerialize(typeof(SequenceSpec), input, true) as SequenceSpec;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in SequenceSpec.Deserialize() : Unable to deserialize given SequenceSpec structure, may be malformed : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

    }
}
