//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    public static class StateUtilities
    {
        public static QuotaSyncState ConvertToQuotaSyncState(ActivationSyncState activationSyncState)
        {
            switch (activationSyncState)
            {
                case ActivationSyncState.InSync:
                    return QuotaSyncState.InSync;
                case ActivationSyncState.Syncing:
                    return QuotaSyncState.Syncing;
                case ActivationSyncState.OutOfSync:
                    return QuotaSyncState.OutOfSync;
                default:
                    Debug.Fail("Unexpected activationSyncState: " + activationSyncState.ToString());
                    return QuotaSyncState.OutOfSync;
            }
        }

        public static QuotaConfigurationState GetParentConfigState(IEnumerable<IQuotaConfigurable> children)
        {
            if (!children.Any())
                return QuotaConfigurationState.NotConfigured;

            return children.Any(c => c.ConfigState == QuotaConfigurationState.NotConfigured) ? QuotaConfigurationState.NotConfigured : QuotaConfigurationState.Configured;
        }

        public static QuotaSyncState GetParentQuotaSyncState(IEnumerable<IQuotaSyncable> children)
        {
            if (children.Any(c => c.QuotaSyncState == QuotaSyncState.Syncing))
            {
                return QuotaSyncState.Syncing;
            }
            else if (children.Any(c => c.QuotaSyncState == QuotaSyncState.OutOfSync))
            {
                return QuotaSyncState.OutOfSync;
            }
            else if (children.All(c => c.QuotaSyncState == QuotaSyncState.InSync))
            {
                return QuotaSyncState.InSync;
            }
            else
            {
                Debug.Fail("Unexpected service quota sync states");
                return QuotaSyncState.OutOfSync;
            }
        }

        public static ActivationSyncState GetParentActivationSyncState(IEnumerable<SubscriptionService> children)
        {
            if (children.Any(c => c.ActivationSyncState == ActivationSyncState.Syncing))
            {
                return ActivationSyncState.Syncing;
            }
            else if (children.Any(c => c.ActivationSyncState == ActivationSyncState.OutOfSync))
            {
                return ActivationSyncState.OutOfSync;
            }
            else if (children.All(c => c.ActivationSyncState == ActivationSyncState.InSync))
            {
                return ActivationSyncState.InSync;
            }
            else
            {
                Debug.Fail("Unexpected service activation sync states");
                return ActivationSyncState.OutOfSync;
            }
        }

        // Cannot use IConfigurable/ISyncable interfaces since the setters are internal
        public static void SetParentConfigState(IEnumerable<ServiceQuota> children, QuotaConfigurationState configState)
        {
            if (children != null)
            {
                foreach (var child in children)
                {
                    child.ConfigState = configState;
                }
            }
        }

        public static void SetParentSyncState(IEnumerable<ServiceQuota> children, QuotaSyncState syncState)
        {
            if (children != null)
            {
                foreach (var child in children)
                {
                    child.QuotaSyncState = syncState;
                }
            }
        }

        public static void SetParentQuotaSyncState(IEnumerable<SubscriptionService> children, QuotaSyncState syncState)
        {
            if (children != null)
            {
                foreach (var child in children)
                {
                    child.QuotaSyncState = syncState;
                }
            }
        }

        public static void SetParentActivationSyncState(IEnumerable<SubscriptionService> children, ActivationSyncState syncState)
        {
            if (children != null)
            {
                foreach (var child in children)
                {
                    child.ActivationSyncState = syncState;
                }
            }
        }
    }
}
