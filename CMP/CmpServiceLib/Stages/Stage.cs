using System;
using System.Diagnostics;

namespace CmpServiceLib.Stages
{
    public abstract class Stage
    {
        public static bool AllQueuesBlocked { get; set; }

        public string CmpDbConnectionString { get; set; }

        public static bool ContainersSynchronized { get; set; }

        public EventLog EventLog { get; set; }

        public static bool SubmittedQueueBlocked { get; set; }

        public abstract object Execute();

        protected void LogThis(Exception ex, EventLogEntryType type, string prefix,
            int id, short category)
        {
            if (null == EventLog)
                return;

            if (null == ex)
                EventLog.WriteEntry(prefix, type, id, category);
            else
                EventLog.WriteEntry(prefix + " : " +
                    CmpCommon.Utilities.UnwindExceptionMessages(ex),
                    type, id, category);
        }
    }
}