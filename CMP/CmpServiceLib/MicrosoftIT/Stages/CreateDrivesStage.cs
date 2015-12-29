using CmpInterfaceModel;

namespace CmpServiceLib.MicrosoftIT.Stages
{
    public class CreateDrivesStage : CmpServiceLib.Stages.CreateDrivesStage
    {
        protected override Constants.StatusEnum NextStatusCode
        {
            get { return Constants.StatusEnum.InstallingIpak; }
        }

        protected override string NextStatusMessage
        {
            get { return "Installing Ipack"; }
        }
    }
}