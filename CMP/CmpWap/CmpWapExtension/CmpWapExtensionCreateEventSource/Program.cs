using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmpWapExtensionCreateEventSource
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                EventLog EL = new EventLog("Application");
                EL.Source = CmpCommon.Constants.CmpWapConnector_EventlogSourceName;
                EL.WriteEntry("Successfully created '" + CmpCommon.Constants.CmpWapConnector_EventlogSourceName + "' event source.", EventLogEntryType.Information, 0, 0);
                Console.WriteLine("Successfully created '" + CmpCommon.Constants.CmpWapConnector_EventlogSourceName + "' event source.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }
    }
}
