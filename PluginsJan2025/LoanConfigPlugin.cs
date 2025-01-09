using Microsoft.SqlServer.Server;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Services;
using System.Text;
using System.Threading.Tasks;

namespace PluginsJan2025
{
    public class LoanConfigPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            try
            {
                IPluginExecutionContext context = (IPluginExecutionContext) serviceProvider.GetService(typeof(IPluginExecutionContext));

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    IOrganizationServiceFactory orgFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    IOrganizationService service = orgFactory.CreateOrganizationService(context.UserId);

                    if(context.MessageName.ToLower() == "create" || context.Stage == 20)
                    {
                        tracingService.Trace("Create Message, Pre - Operation Plugin");

                        Entity TargetEntity = context.InputParameters["Target"] as Entity;

                        Guid LoanTypeGuid = TargetEntity.GetAttributeValue<EntityReference>("gv_loantype").Id;   // lookup is entity reference, not guid

                        Entity LoanTypeEntity = service.Retrieve("gv_bank_loan_config", LoanTypeGuid, new ColumnSet("gv_bank_loan_configid", "gv_interestrate", "gv_loantenure"));


                        if (TargetEntity.Contains("gv_loantenure"))
                        {
                            TargetEntity["gv_loantenure"] = LoanTypeEntity.GetAttributeValue<Int32>("gv_loantenure");
                        }
                        else
                        {
                            TargetEntity.Attributes.Add("gv_loantenure", LoanTypeEntity.GetAttributeValue<Int32>("gv_loantenure"));
                        }

                        if (TargetEntity.Contains("gv_interest"))
                        {
                            TargetEntity["gv_interest"] = LoanTypeEntity.GetAttributeValue<decimal>("gv_interestrate");
                        }
                        else
                        {
                            TargetEntity.Attributes.Add("gv_interest", LoanTypeEntity.GetAttributeValue<decimal>("gv_interestrate"));
                        }

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
}
