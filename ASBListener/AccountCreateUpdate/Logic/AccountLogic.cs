using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Crm.Sdk.Messages;

namespace CDSPlugins.Logic
{
    public class AccountLogic : IDisposable
    {
        // Global Variables
        private IPluginExecutionContext context;
        private IOrganizationService service;
        private ITracingService tracingService;
        private IServiceEndpointNotificationService cloudService;

        #region Constructors

        public AccountLogic(IOrganizationService orgService)
        {
            service = orgService;
        }

        public AccountLogic(IOrganizationService orgService, ITracingService traceService)
        {
            service = orgService;
            tracingService = traceService;
        }

        public AccountLogic(IPluginExecutionContext pluginContext, IOrganizationService orgService, ITracingService traceService)
        {
            context = pluginContext;
            service = orgService;
            tracingService = traceService;
        }

        public AccountLogic(IPluginExecutionContext pluginContext, IOrganizationService orgService, ITracingService traceService, IServiceEndpointNotificationService notificationService)
        {
            context = pluginContext;
            service = orgService;
            tracingService = traceService;
            cloudService = notificationService;
        }


        #endregion

        #region Entry Points

        internal void CreateAccount(Guid accountId)
        {
            tracingService.Trace("Entry for CreateAccount");
            CreateUpdateAccount(accountId);
        }

        internal void UpdateAccount(Guid accountId)
        {
            tracingService.Trace("Entry for UpdateAccount");
            CreateUpdateAccount(accountId);
        }

        private void CreateUpdateAccount(Guid accountId)
        {
            tracingService.Trace("Entry for CreateUpdateAccount");
            Entity account = service.Retrieve("account", accountId, new ColumnSet(true));
            Guid endpointId = RetrieveServiceEndPointId();
            tracingService.Trace($"Endpoint Id: {endpointId.ToString()}");

            string response = "";
            if (endpointId != Guid.Empty)
            {
                this.context.SharedVariables.Add("Account", account);
                tracingService.Trace("Added Account Entity to SharedVariables");

                try
                {
                    tracingService.Trace("Trying to connect to Cloud Service");
                    response = cloudService.Execute(new EntityReference("serviceendpoint", endpointId), context);
                }
                catch (Exception ex)
                {
                    tracingService.Trace($"CreateUpdateAccount Error: {ex.Message}");
                    response = "There was no response from the Azure Service Bus Listener. Please try again";
                }
            }

            CreateAnnotation(accountId, response);
        }

        private void CreateAnnotation(Guid accountId, string message)
        {
            Entity annotation = new Entity("annotation");
            annotation["subject"] = "Response from Azure Service Bus";
            annotation["objecttypecode"] = "account";
            annotation["notetext"] = message;
            annotation["objectid"] = new EntityReference("account", accountId);

            try
            {
                service.Create(annotation);
            }
            catch (Exception ex)
            {
                tracingService.Trace($"Create Annotaion Error: {ex.Message}");
            }
        }

        private Guid RetrieveServiceEndPointId()
        {
            Guid definitionId = RetrieveEnvironmentVariableDefinitionId("cds_asbendpointid");
            Guid endpointId = Guid.Empty;

            if (definitionId != Guid.Empty)
            {
                string rc = RetrieveEnvironmentalVariableValue(definitionId);
                Guid.TryParse(rc, out endpointId);

            }
            return endpointId;
        }

        private string RetrieveAdxSetting(string key)
        {
            string value = "";
            QueryExpression query = new QueryExpression("adx_setting");
            query.ColumnSet = new ColumnSet("adx_value");
            query.Criteria.AddCondition("adx_name", ConditionOperator.Equal, key);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            var results = service.RetrieveMultiple(query);
            if (results.Entities.Count > 0 && results.Entities[0].Contains("adx_value"))
            {
                value = results.Entities[0].GetAttributeValue<string>("adx_value");
            }

            return value;
        }

        private Guid RetrieveEnvironmentVariableDefinitionId(string key)
        {
            Guid definitionId = Guid.Empty;
            QueryExpression query = new QueryExpression("environmentvariabledefinition");
            query.ColumnSet = new ColumnSet("environmentvariabledefinitionid", "schemaname", "displayname", "description", "defaultvalue");
            query.Criteria.AddCondition("schemaname", ConditionOperator.Equal, key);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            var results = service.RetrieveMultiple(query);
            if (results.Entities.Count > 0)
            {
                definitionId = results.Entities[0].Id;
            }

            return definitionId;
        }

        private string RetrieveEnvironmentalVariableValue(Guid definitionId)
        {
            string rc = string.Empty;
            QueryExpression query = new QueryExpression("environmentvariablevalue");
            query.ColumnSet = new ColumnSet("value");
            query.Criteria.AddCondition("environmentvariabledefinitionid", ConditionOperator.Equal, definitionId);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            var results = service.RetrieveMultiple(query);
            if (results.Entities.Count > 0)
            {
                rc = results.Entities[0].GetAttributeValue<string>("value");
            }

            return rc;

        }


        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~AccountLogic()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion



    }
}
