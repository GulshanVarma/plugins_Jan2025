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

                    if (context.MessageName.ToLower() == "create" || context.Stage == 10)
                    {
                        Entity TargetEntity = context.InputParameters["Target"] as Entity;
                        if (string.IsNullOrEmpty(TargetEntity.GetAttributeValue<EntityReference>("gv_pan")?.Id.ToString()))
                        {
                            throw new Exception("PAN is not selected");
                        }

                        // check valid score from Loan > Account > User

                        /*QueryExpression queryExpression = new QueryExpression() { 
                            EntityName = "gv_Bank_Account",
                            ColumnSet = new ColumnSet("gv_accountnumber", "gv_AccountHolder")
                        };*/


                        Entity AccountDetails = service.Retrieve("gv_bank_account", TargetEntity.GetAttributeValue<EntityReference>("gv_pan").Id, 
                            new ColumnSet("gv_accountnumber", "gv_accountholder"));

                        Entity userDetail = service.Retrieve("gv_bank_user", AccountDetails.GetAttributeValue<EntityReference>("gv_accountholder").Id,
                            new ColumnSet("gv_userscore"));

                        Entity LoanConfigDetails = service.Retrieve("gv_bank_loan_config", TargetEntity.GetAttributeValue<EntityReference>("gv_loantype").Id,
                            new ColumnSet("gv_minimumscore"));

                        if(LoanConfigDetails.GetAttributeValue<Int32>("gv_minimumscore") > userDetail.GetAttributeValue<Int32>("gv_userscore"))
                        {
                            throw new Exception("user score is lower than the required limit - "+ LoanConfigDetails.GetAttributeValue<Int32>("gv_minimumscore"));
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
