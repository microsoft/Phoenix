//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Represents a language-specific plan advertisement.
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public class PlanAdvertisement : IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets the language code (e.g. en-us ).
        /// </summary>
        [DataMember(Order = 0)]
        public string LanguageCode { get; set; }

        /// <summary>
        /// Gets or sets the language-specific display name of the plan.
        /// </summary>
        [DataMember(Order = 1)]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the language-specific plan description.
        /// </summary>
        [DataMember(Order = 2)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the structure that contains extra data.
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}