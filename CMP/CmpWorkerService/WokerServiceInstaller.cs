using System.ComponentModel;
using System.ServiceProcess;

namespace CmpWorkerService
{
    [RunInstaller(true)]
    public partial class WokerServiceInstaller : System.Configuration.Install.Installer
    {
        private ServiceInstaller serviceInstaller;
        private ServiceProcessInstaller processInstaller;

        public WokerServiceInstaller()
        {
            InitializeComponent();

            // Instantiate installers for process and services.
            processInstaller = new ServiceProcessInstaller();
            serviceInstaller = new ServiceInstaller();

            // The services run under the system account.
            processInstaller.Account = ServiceAccount.LocalSystem;

            // The services are started manually.
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            // ServiceName must equal those on ServiceBase derived classes.            
            serviceInstaller.ServiceName = "CmpWorker";
            serviceInstaller.DisplayName = "CmpWorker";

            // Add installers to collection. Order is not important.
            Installers.Add(serviceInstaller);
            Installers.Add(processInstaller);

        }
    }
}
