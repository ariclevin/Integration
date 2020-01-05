using System;
using System.ComponentModel;
using System.Configuration;
using System.Reflection;
using System.ServiceProcess;

namespace CDSWDXListener
{
    // Provide the ProjectInstaller class which allows 
    // the service to be installed by the Installutil.exe tool
    [RunInstaller(true)]
    public class ProjectInstaller : System.Configuration.Install.Installer
    {
        private ServiceProcessInstaller process;
        private ServiceInstaller service;

        public ProjectInstaller()
        {
            var config = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.NetworkService;
            service = new ServiceInstaller();
            if (config.AppSettings.Settings["ServiceName"] != null)
            {
                service.ServiceName = config.AppSettings.Settings["ServiceName"].Value;
            }
            else
                service.ServiceName = "CDSWDXListenerService";
            service.StartType = ServiceStartMode.Automatic;
            Installers.Add(process);
            Installers.Add(service);
        }
    }
}
