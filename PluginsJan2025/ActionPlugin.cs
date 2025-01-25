using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PluginsJan2025
{
    public class ActionPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService trace = serviceProvider.GetService(typeof(ITracingService)) as ITracingService;
            try
            {
                IExecutionContext context = serviceProvider.GetService(typeof(IExecutionContext)) as IExecutionContext;
                
                IOrganizationServiceFactory factory = serviceProvider.GetService(typeof(IOrganizationServiceFactory)) as IOrganizationServiceFactory;
                IOrganizationService service = factory.CreateOrganizationService(context.UserId);

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is EntityReference)
                {
                    trace.Trace($"message name is {context.MessageName}");
                    if (context.MessageName == "gv_action_calculateTotalLoanAmt")
                    {
                        context.OutputParameters["NewUserBalance"] = int.Parse(context.InputParameters["accountMinBalance"].ToString()) - 500;
                        trace.Trace($" setting balance - {context.OutputParameters["NewUserBalance"]}, recieved - {context.InputParameters["accountMinBalance"].ToString()}");
                    }
                }
            }
            catch (Exception ex)
            {
                trace.Trace($"Exception caught - {ex.Message}, Trace - {ex.StackTrace}");
            }
        }
    }

    public class ActionPlugin_Ribbon : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService trace = serviceProvider.GetService(typeof(ITracingService)) as ITracingService;
            try
            {
                IExecutionContext context = serviceProvider.GetService(typeof(IExecutionContext)) as IExecutionContext;

                IOrganizationServiceFactory factory = serviceProvider.GetService(typeof(IOrganizationServiceFactory)) as IOrganizationServiceFactory;
                IOrganizationService service = factory.CreateOrganizationService(context.UserId);

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is EntityReference)
                {
                    trace.Trace($"message name is {context.MessageName}");
                    if (context.MessageName == "gv_action_testSample")
                    {
                        context.OutputParameters["outputmessage"] = "tumhara naame kya hai " + context.InputParameters["inputmessage"].ToString();
                        trace.Trace($" params - {context.OutputParameters["outputmessage"]}, recieved - {context.InputParameters["inputmessage"]}");
                    }
                }
            }
            catch (Exception ex)
            {
                trace.Trace($"Exception caught - {ex.Message}, Trace - {ex.StackTrace}");
            }
        }
    }
}
