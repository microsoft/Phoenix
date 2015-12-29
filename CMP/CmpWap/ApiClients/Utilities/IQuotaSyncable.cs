//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Interface for entities whose quota can be synced.
    /// </summary>
    public interface IQuotaSyncable
    {
        /// <summary>
        /// Gets the sync state of the quota.
        /// </summary>
        QuotaSyncState QuotaSyncState { get; }
    }
}
