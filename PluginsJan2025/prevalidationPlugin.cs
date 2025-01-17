using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace PluginsJan2025
{
    public class prevalidationPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            try
            {
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    IOrganizationServiceFactory orgFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    IOrganizationService service = orgFactory.CreateOrganizationService(context.UserId);

                    Entity Loandetails = null;
                    Entity AccountDetails = null;
                    Entity userDetail = null;
                    Entity LoanConfigDetails = null;

                    if ((context.MessageName.ToLower() == "create" && context.Stage == 10) || (context.MessageName.ToLower() == "update" && context.Stage == 10))
                    {
                        tracingService.Trace($" Creation/Update Plugin, prevalidation started");
                        Entity TargetEntity = context.InputParameters["Target"] as Entity;
                        if (context.MessageName.ToLower() == "create" && string.IsNullOrEmpty(TargetEntity.GetAttributeValue<EntityReference>("gv_pan")?.Id.ToString()))
                        {
                            tracingService.Trace($"Pan is not selected");
                            return;
                        }

                        if ((context.MessageName.ToLower() == "update" && context.Stage == 10))
                        {
                            // fetch remainig attributes, we get only ID and updated field
                            ColumnSet getDataForLoan = new ColumnSet("gv_loantype","gv_pan");
                            Loandetails = service.Retrieve("gv_bank_loan", TargetEntity.Id, getDataForLoan);
                            AccountDetails = service.Retrieve("gv_bank_account", Loandetails.GetAttributeValue<EntityReference>("gv_pan").Id,
                                new ColumnSet("gv_accountnumber", "gv_accountholder"));

                            userDetail = service.Retrieve("gv_bank_user", AccountDetails.GetAttributeValue<EntityReference>("gv_accountholder").Id,
                                new ColumnSet("gv_userscore", "gv_userfullname"));

                            LoanConfigDetails = service.Retrieve("gv_bank_loan_config", Loandetails.GetAttributeValue<EntityReference>("gv_loantype").Id,
                                new ColumnSet("gv_minimumscore"));
                        }
                        else
                        {
                            AccountDetails = service.Retrieve("gv_bank_account", TargetEntity.GetAttributeValue<EntityReference>("gv_pan").Id,
                                new ColumnSet("gv_accountnumber", "gv_accountholder"));

                            userDetail = service.Retrieve("gv_bank_user", AccountDetails.GetAttributeValue<EntityReference>("gv_accountholder").Id,
                                new ColumnSet("gv_userscore", "gv_userfullname"));

                            LoanConfigDetails = service.Retrieve("gv_bank_loan_config", TargetEntity.GetAttributeValue<EntityReference>("gv_loantype").Id,
                                new ColumnSet("gv_minimumscore"));
                        }

                        if (LoanConfigDetails.GetAttributeValue<Int32>("gv_minimumscore") > userDetail.GetAttributeValue<Int32>("gv_userscore"))
                        {
                            tracingService.Trace($"user {userDetail.GetAttributeValue<string>("gv_userfullname")} -> score {userDetail.GetAttributeValue<Int32>("gv_userscore")} is lower than the required limit - {LoanConfigDetails.GetAttributeValue<Int32>("gv_minimumscore")}");
                            throw new InvalidPluginExecutionException("user score is lower than the required limit - " + LoanConfigDetails.GetAttributeValue<Int32>("gv_minimumscore"));
                        }

                    }
                    else
                    {
                        tracingService.Trace($"Invalid Plugin Context, cant proceeding further. message - {context.MessageName.ToLower()}, stage - {context.Stage}");
                    }

                }
            }
            catch (InvalidPluginExecutionException) { throw; }
            catch (Exception ex)
            {
                tracingService.Trace($"Exception Caught -> Msg - {ex.Message}, trace - {ex.StackTrace}");
            }
        }
    }
}
