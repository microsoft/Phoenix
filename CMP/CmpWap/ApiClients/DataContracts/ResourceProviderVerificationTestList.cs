//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// List of Resource Provider verification Uris
    /// </summary>
    [CollectionDataContract(Namespace = "http://schemas.microsoft.com/windowsazure", Name = "Tests", ItemName = "Test")]
    public class ResourceProviderVerificationTestList : List<ResourceProviderVerificationTest>
    {
    }
}
