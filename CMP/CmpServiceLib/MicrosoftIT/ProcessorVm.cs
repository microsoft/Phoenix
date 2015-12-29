using System.Collections.Generic;
using System.Diagnostics;
using CmpServiceLib.MicrosoftIT.Stages;

namespace CmpServiceLib.MicrosoftIT
{
    public class ProcessorVm : CmpServiceLib.ProcessorVm
    {
        public ProcessorVm(EventLog eLog) : base(eLog)
        {
        }

        public ProcessorVm(EventLog eLog, string cmpDbConnectionString, string aftsDbConnectionString)
            : base(eLog, cmpDbConnectionString, aftsDbConnectionString)
        {
        }

        protected override void SetStages(string cmpDbConnectionString, string aftsDbConnectionString)
        {
            base.SetStages(cmpDbConnectionString, aftsDbConnectionString);
            Add(new List<CmpServiceLib.Stages.Stage>
            {
                new CreateDrivesStage(),
                new InstallIpakStage
                {
                    CmpDbConnectionString = cmpDbConnectionString,
                    EventLog = _EventLog,
                }
            });
        }
    }
}