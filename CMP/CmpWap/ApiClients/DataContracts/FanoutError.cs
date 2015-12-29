// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// A fanout exception
    /// </summary>
    [DataContract(Name = "FanoutError", Namespace = "http://schemas.microsoft.com/windowsazure")]
    public class FanoutError
    {
        /// <summary>
        /// Gets or sets the http status code. 
        /// </summary>
        [DataMember(Order = 0)]
        public int HttpStatusCode { get; set; }

        /// <summary>
        /// Gets or sets the service name. 
        /// </summary>
        [DataMember(Order = 1)]
        public string ServiceName { get; set; }

        /// <summary>
        /// Gets or sets the service type.
        /// </summary>
        [DataMember(Order = 2)]
        public string ServiceType { get; set; }

        /// <summary>
        /// Gets or sets the message. 
        /// </summary>
        [DataMember(Order = 3)]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the code. 
        /// </summary>
        [DataMember(Order = 4)]
        public string Code { get; set; }
    }
}
