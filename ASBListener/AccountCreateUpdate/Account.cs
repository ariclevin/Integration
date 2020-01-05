
namespace CDSPlugins
{
    using System;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;

    using CDSPlugins.Logic;
    /// <summary>
    /// Base class for all Plugins.
    /// </summary>    
    public class Account : Plugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Email"/> class.
        /// </summary>
        public Account()
            : base(typeof(Account))
        {
            this.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, "Create", "account", new Action<LocalPluginContext>(ExecutePostAccountCreate)));
            this.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, "Update", "account", new Action<LocalPluginContext>(ExecutePostAccountUpdate)));
        }

        /// <summary>
        /// Executes the plug-in.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <remarks>
        /// For improved performance, Microsoft Dynamics CRM caches plug-in instances. 
        /// The plug-in's Execute method should be written to be stateless as the constructor 
        /// is not called for every invocation of the plug-in. Also, multiple system threads 
        /// could execute the plug-in at the same time. All per invocation state information 
        /// is stored in the context. This means that you should not use global variables in plug-ins.
        /// </remarks>

        #region Execute Methods

        protected void ExecutePostAccountCreate(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new ArgumentNullException("localContext");
            }

            Guid accountId = localContext.PluginExecutionContext.PrimaryEntityId;
            using (AccountLogic logic = new AccountLogic(localContext.PluginExecutionContext, localContext.OrganizationService, localContext.TracingService, localContext.CloudService))
            {
                logic.CreateAccount(accountId);
            }
                
        }

        protected void ExecutePostAccountUpdate(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new ArgumentNullException("localContext");
            }

            Guid accountId = localContext.PluginExecutionContext.PrimaryEntityId;
            using (AccountLogic logic = new AccountLogic(localContext.PluginExecutionContext, localContext.OrganizationService, localContext.TracingService, localContext.CloudService))
            {
                logic.UpdateAccount(accountId);
            }
        }


        #endregion

    }
}