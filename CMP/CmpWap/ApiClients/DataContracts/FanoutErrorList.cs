//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Represents a collection of Fanout errors
    /// </summary>
    [CollectionDataContract(Name = "FanoutErrors", ItemName = "FanoutError", Namespace = "http://schemas.microsoft.com/windowsazure")]
    public class FanoutErrorList : List<FanoutError>
    {
    }
}
