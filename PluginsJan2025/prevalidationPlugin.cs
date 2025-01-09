using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginsJan2025
{
    public class prevalidationPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            try
            {
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    IOrganizationServiceFactory orgFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    IOrganizationService service = orgFactory.CreateOrganizationService(context.UserId);

                    if (context.MessageName.ToLower() == "create" || context.Stage == 20)
                    {

                    }
                    else
                    {
                        tracingService.Trace($"Invalid Plugin Context, cant proceeding further. message - {context.MessageName.ToLower()}, stage - {context.Stage}");
                    }

                }

            }
            catch (Exception ex)
            {
                tracingService.Trace($"Exception Caught - {ex.Message}, trace -- {ex.StackTrace}");
            }
        }
}
