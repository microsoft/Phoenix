using System.ServiceProcess;

namespace CmpWorkerService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new CmpWorker() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
