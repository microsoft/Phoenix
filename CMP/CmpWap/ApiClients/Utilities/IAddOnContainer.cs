//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Interface add-on containers (i.e. Subscription and Plan)
    /// </summary>
    public interface IAddOnContainer
    {
        /// <summary>
        /// Gets the add-on references.
        /// </summary>
        IList<PlanAddOnReference> AddOnReferences { get; }

        /// <summary>
        /// Gets the add-ons.
        /// </summary>
        IList<PlanAddOn> AddOns { get; }
    }
}
