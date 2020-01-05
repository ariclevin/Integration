using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CDSWDXListener
{
    /// <summary>
    /// Bidirectional Plugin class to listen and process the incoming requests from Azure service bus.
    /// </summary>
    public class WDXEngine : ITwoWayServiceEndpointPlugin
    {
        /// This Execute method will execute automatically whenever a message has been passed
        /// to the service bus.
        /// In realworld scenarios this would become a backend worker role and will call the
        /// third party services to get the details.
        /// We can use the executionContext Object to get the details of the CRM object    
        public string Execute(RemoteExecutionContext executionContext)
        {                    
            string result = string.Empty;
            Console.WriteLine("----------");
            string message = "";

            if (executionContext.SharedVariables.Contains("Account"))
            {
                Console.WriteLine("Get Account Information...");
                Entity account = executionContext.SharedVariables["Account"] as Entity;

                if (executionContext.MessageName.Equals("create", StringComparison.OrdinalIgnoreCase))
                {
                    result = CreateAccount(account);
                    message = result; // "Account Record Created";
                }
                else if (executionContext.MessageName.Equals("create", StringComparison.OrdinalIgnoreCase))
                {
                    result = UpdateAccount(account);
                    message = result; // "Account Record Update";
                }
                else
                {
                    message = "No Action performed on Account Record";
                    result = message;
                }
            }


            Console.WriteLine($"result - {message}");
            EventLog.WriteEntry("CDSWDXListenerSource", $"result - {message}", EventLogEntryType.Information);
            Console.WriteLine("----------");


            return message;

        }

        private string CreateAccount(Entity account)
        {
            Guid accountId = account.Id;
            string accountName = account.GetAttributeValue<string>("name");
            string accountNumber = account.GetAttributeValue<string>("accountnumber");
            string description = account.GetAttributeValue<string>("description");
            string webSiteUrl = account.GetAttributeValue<string>("websiteurl");
            string emailAddress = account.GetAttributeValue<string>("emailaddress1");
            string telephone1 = account.GetAttributeValue<string>("telephone1");
            Guid createdById = account.GetAttributeValue<EntityReference>("createdby").Id;
            string createdByName = account.GetAttributeValue<EntityReference>("createdby").Name;
            Guid ownerId = account.GetAttributeValue<EntityReference>("ownerid").Id;
            string ownerName = account.GetAttributeValue<EntityReference>("ownerid").Name;
            int stateCode = account.GetAttributeValue<OptionSetValue>("statecode").Value;
            int statusCode = account.GetAttributeValue<OptionSetValue>("statuscode").Value;

            return SqlDataManager.CreateAccount(accountId, accountName, accountNumber, description, webSiteUrl, emailAddress, telephone1, createdById, ownerId, stateCode, statusCode);
        }

        private string UpdateAccount(Entity account)
        {
            Guid accountId = account.Id;
            string accountName = account.GetAttributeValue<string>("name");
            string accountNumber = account.GetAttributeValue<string>("accountnumber");
            string description = account.GetAttributeValue<string>("description");
            string webSiteUrl = account.GetAttributeValue<string>("websiteurl");
            string emailAddress = account.GetAttributeValue<string>("emailaddress1");
            string telephone1 = account.GetAttributeValue<string>("telephone1");
            Guid modifiedById = account.GetAttributeValue<EntityReference>("modifiedby").Id;
            string modifiedByName = account.GetAttributeValue<EntityReference>("modifiedby").Name;
            Guid ownerId = account.GetAttributeValue<EntityReference>("ownerid").Id;
            string ownerName = account.GetAttributeValue<EntityReference>("ownerid").Name;
            int stateCode = account.GetAttributeValue<OptionSetValue>("statecode").Value;
            int statusCode = account.GetAttributeValue<OptionSetValue>("statuscode").Value;

            return SqlDataManager.UpdateAccount(accountId, accountName, accountNumber, description, webSiteUrl, emailAddress, telephone1, modifiedById, ownerId, stateCode, statusCode);
        }



        /// <summary>
        /// Containts methods to display the RemoteExecutionContext provided when a 
        /// message is posted to the Azure Service Bus.
        /// </summary>
        private static class Utility
        {
            /// <summary>
            /// Writes out the RemoteExecutionContext to the Console.
            /// </summary>
            /// <param name="context"></param>
            public static void Print(RemoteExecutionContext context)
            {
                Console.WriteLine("----------");
                if (context == null)
                {
                    Console.WriteLine("Context is null.");
                    return;
                }

                Console.WriteLine("UserId: {0}", context.UserId);
                Console.WriteLine("OrganizationId: {0}", context.OrganizationId);
                Console.WriteLine("OrganizationName: {0}", context.OrganizationName);
                Console.WriteLine("MessageName: {0}", context.MessageName);
                Console.WriteLine("Stage: {0}", context.Stage);
                Console.WriteLine("Mode: {0}", context.Mode);
                Console.WriteLine("PrimaryEntityName: {0}", context.PrimaryEntityName);
                Console.WriteLine("SecondaryEntityName: {0}", context.SecondaryEntityName);

                Console.WriteLine("BusinessUnitId: {0}", context.BusinessUnitId);
                Console.WriteLine("CorrelationId: {0}", context.CorrelationId);
                Console.WriteLine("Depth: {0}", context.Depth);
                Console.WriteLine("InitiatingUserId: {0}", context.InitiatingUserId);
                Console.WriteLine("IsExecutingOffline: {0}", context.IsExecutingOffline);
                Console.WriteLine("IsInTransaction: {0}", context.IsInTransaction);
                Console.WriteLine("IsolationMode: {0}", context.IsolationMode);
                Console.WriteLine("Mode: {0}", context.Mode);
                Console.WriteLine("OperationCreatedOn: {0}", context.OperationCreatedOn.ToString());
                Console.WriteLine("OperationId: {0}", context.OperationId);
                Console.WriteLine("PrimaryEntityId: {0}", context.PrimaryEntityId);
                Console.WriteLine("OwningExtension LogicalName: {0}", context.OwningExtension.LogicalName);
                Console.WriteLine("OwningExtension Name: {0}", context.OwningExtension.Name);
                Console.WriteLine("OwningExtension Id: {0}", context.OwningExtension.Id);
                Console.WriteLine("SharedVariables: {0}", (context.SharedVariables == null
                    ? "NULL" : SerializeParameterCollection(context.SharedVariables)));
                Console.WriteLine("InputParameters: {0}", (context.InputParameters == null
                    ? "NULL" : SerializeParameterCollection(context.InputParameters)));
                Console.WriteLine("OutputParameters: {0}", (context.OutputParameters == null
                    ? "NULL" : SerializeParameterCollection(context.OutputParameters)));
                Console.WriteLine("PreEntityImages: {0}", (context.PreEntityImages == null
                    ? "NULL" : SerializeEntityImageCollection(context.PreEntityImages)));
                Console.WriteLine("PostEntityImages: {0}", (context.PostEntityImages == null
                    ? "NULL" : SerializeEntityImageCollection(context.PostEntityImages)));
                Console.WriteLine("----------");
            }

            /// <summary>
            /// Writes out the attributes of an entity.
            /// </summary>
            /// <param name="e">The entity to serialize.</param>
            /// <returns>A human readable representation of the entity.</returns>
            private static string SerializeEntity(Entity e)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0} LogicalName: {1}{0} EntityId: {2}{0} Attributes: [",
                    Environment.NewLine,
                    e.LogicalName,
                    e.Id);
                foreach (KeyValuePair<string, object> parameter in e.Attributes)
                {
                    sb.AppendFormat("{0}: {1}; ", parameter.Key, parameter.Value);
                }
                sb.Append("]");
                return sb.ToString();
            }

            /// <summary>
            /// Flattens a collection into a delimited string.
            /// </summary>
            /// <param name="parameterCollection">The values must be of type Entity 
            /// to print the values.</param>
            /// <returns>A string representing the collection passed in.</returns>
            private static string SerializeParameterCollection(ParameterCollection parameterCollection)
            {
                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<string, object> parameter in parameterCollection)
                {
                    Entity e = parameter.Value as Entity;
                    if (e != null)
                    {
                        sb.AppendFormat("{0}: {1}", parameter.Key, SerializeEntity(e));
                    }
                    else
                    {
                        sb.AppendFormat("{0}: {1}; ", parameter.Key, parameter.Value);
                    }
                }
                return sb.ToString();
            }

            /// <summary>
            /// Flattens a collection into a delimited string.
            /// </summary>
            /// <param name="entityImageCollection">The collection to flatten.</param>
            /// <returns>A string representation of the collection.</returns>
            private static string SerializeEntityImageCollection(EntityImageCollection entityImageCollection)
            {
                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<string, Entity> entityImage in entityImageCollection)
                {
                    sb.AppendFormat("{0}{1}: {2}", Environment.NewLine, entityImage.Key,
                        SerializeEntity(entityImage.Value));
                }
                return sb.ToString();
            }
        }
    }

}
