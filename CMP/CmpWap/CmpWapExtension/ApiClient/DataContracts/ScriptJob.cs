// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts
{
    /// <summary>
    /// This is a data contract class between extensions and resource provider
    /// ScriptJob contains data contract of data which shows up in "ScriptJobs" tab inside Cmp Tenant Extension
    /// </summary>
    [DataContract(Namespace = Constants.DataContractNamespaces.Default)]

    public class ScriptJobPoshParamSpec
    {
        [DataMember(Order = 1)]
        public string Name { set; get; }

        [DataMember(Order = 2)]
        public string Value { set; get; }

        [DataMember(Order = 3)]
        public bool SelectReachableHost { set; get; }
    }

    public class ScriptJobSmaConfigSpec
    {
        [DataMember(Order = 1)]
        public string SmaServerUrl { set; get; }

        [DataMember(Order = 2)]
        public string RunbookId { set; get; }

        [DataMember(Order = 3)]
        public string RunbookName { set; get; }

        [DataMember(Order = 4)]
        public List<ScriptJobPoshParamSpec> ParamList { set; get; }
    }

    public class ScriptJobRunOuputSpec
    {
        [DataMember(Order = 1)]
        public DateTime When { set; get; }

        [DataMember(Order = 2)]
        public string Output { set; get; }
    }

    public class ScriptJobRunResultSpec
    {
        [DataMember(Order = 1)]
        public string JobId { set; get; }

        [DataMember(Order = 2)]
        public string StatusCode { set; get; }

        [DataMember(Order = 3)]
        public string StatusMessage { set; get; }

        [DataMember(Order = 4)]
        public DateTime LastUpdate { set; get; }

        [DataMember(Order = 5)]
        public DateTime WhenSubmitted { set; get; }

        [DataMember(Order = 6)]
        public List<ScriptJobRunOuputSpec> Output { set; get; }
    }

    public class ScriptJob
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

        [DataMember(Order = 1)]
        public int ID { set; get; }

        [DataMember(Order = 2)]
        public string Name { set; get; }

        [DataMember(Order = 3)]
        public string Engine { set; get; }

        [DataMember(Order = 4)]
        public string Locale { set; get; }

        [DataMember(Order = 5)]
        public string BreakOn { set; get; }

        [DataMember(Order = 6)]
        public string Waitmode { set; get; }

        [DataMember(Order = 7)]
        public int TimeoutMinutes { set; get; }

        [DataMember(Order = 8)]
        public List<string> ScriptList { set; get; }

        [DataMember(Order = 9)]
        public string ExecuteInState { set; get; }

        [DataMember(Order = 10)]
        public string ResultCode { set; get; }

        [DataMember(Order = 11)]
        public string Config { set; get; }

        [DataMember(Order = 12)]
        public ScriptJobSmaConfigSpec SmaConfig { set; get; }

        [DataMember(Order = 13)]
        public string TagData { set; get; }

        [DataMember(Order = 14)]
        public List<string> ErrorList { set; get; }

        [DataMember(Order = 15)]
        public ScriptJobRunResultSpec RunResult { set; get; }

        [DataMember(Order = 16)]
        public string TargetName { set; get; }

        [DataMember(Order = 17)]
        public string TargetTypeCode { set; get; }
    }

}
