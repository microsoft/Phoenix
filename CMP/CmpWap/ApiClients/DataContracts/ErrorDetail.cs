// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// The details of an error
    /// </summary>
    [DataContract(Name = "Detail", Namespace = "http://schemas.microsoft.com/windowsazure")]
    public class ErrorDetail
    {
        /// <summary>
        /// Gets the error code. 
        /// </summary>
        [DataMember(Order = 0)]
        public string Code { get; set; }

        /// <summary>
        /// Gets the error source. 
        /// </summary>
        [DataMember(Order = 1)]
        public string Source { get; set; }

        /// <summary>
        /// Gets the error message. 
        /// </summary>
        [DataMember(Order = 2)]
        public string Message { get; set; }

        /// <summary>
        /// Required override to properly display the error detail
        /// </summary>
        /// <returns>string representing the error detail</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Code: {0}, Source: {1}, Message: {2}", this.Code, this.Source, this.Message);
        }
    }
}
