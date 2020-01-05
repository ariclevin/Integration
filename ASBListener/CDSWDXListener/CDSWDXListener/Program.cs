using System;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceProcess;

namespace CDSWDXListener
{
    /// <summary>
    /// Main class of the application.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main entry point of the application
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {   
                if (Environment.UserInteractive)
                {
                    string parameter = string.Concat(args);
                    Console.WriteLine(parameter);
                    if (args.Count() > 0)
                    {
                        switch (args[0])
                        {
                            case "--install":
                                //Install the windows service.
                                Console.WriteLine(Assembly.GetExecutingAssembly().Location);
                                ManagedInstallerClass.InstallHelper(new[] { Assembly.GetExecutingAssembly().Location });                                                                                                
                                break;
                            case "--uninstall":
                                //Uninstall the windows service.
                                ManagedInstallerClass.InstallHelper(new[] { "/u", Assembly.GetExecutingAssembly().Location });                                
                                break;
                            case "--console":
                                //Runs the service application as console application.
                                var sh = new ServiceHost(typeof(WDXEngine));
                                sh.Open();
                                Console.WriteLine("CDSWDXListener listening .. ");
                                Console.WriteLine("Press ENTER to close");
                                Console.ReadLine();
                                sh.Close();
                                break;
                        }
                    }
                }
                else
                {
                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new ServiceBase[]
                    {
                        new CDSWDXListenerService()
                    };
                    ServiceBase.Run(ServicesToRun);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
