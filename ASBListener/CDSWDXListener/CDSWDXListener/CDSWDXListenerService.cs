using System;
using System.ServiceModel;
using System.ServiceProcess;

namespace CDSWDXListener
{
    partial class CDSWDXListenerService : ServiceBase
    {
        public ServiceHost serviceHost = null;        
        public CDSWDXListenerService()
        {
            InitializeComponent();
            eventLogCDSWDXListener = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("CDSWDXListenerSource"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "CDSWDXListenerSource", "WDX");
            }
            eventLogCDSWDXListener.Source = "CDSWDXListenerSource";
            eventLogCDSWDXListener.Log = "WDX";
        }

        protected override void OnStart(string[] args)
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
            }

            // Start the service
            serviceHost = new ServiceHost(typeof(WDXEngine));
            serviceHost.Open();            
            eventLogCDSWDXListener.WriteEntry("CDSWDXListener listening .. ");            
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
            if (serviceHost != null)
            {
                serviceHost.Close();
                serviceHost = null;
            }
            eventLogCDSWDXListener.WriteEntry("CDSWDXListener stopped.");
        }

        private void eventLogCDSWDXListener_EntryWritten(object sender, System.Diagnostics.EntryWrittenEventArgs e)
        {

        }
    }
}
