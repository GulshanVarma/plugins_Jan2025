using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace PluginsJan2025
{
    public class plg_OnCreate_Loans_PreOps : IPlugin
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
                        tracingService.Trace("plg_OnCreate_Loans_PreOps -> Create Message, Pre - Operation Plugin");

                        Entity TargetEntity = context.InputParameters["Target"] as Entity;

                        // lookup is entity reference, not guid. Get Lookup details
                        Guid LoanTypeGuid = TargetEntity.GetAttributeValue<EntityReference>("gv_loantype").Id;
                        Entity LoanTypeEntity = service.Retrieve("gv_bank_loan_config", LoanTypeGuid, new ColumnSet("gv_bank_loan_configid", "gv_interestrate", "gv_loantenure"));
                        tracingService.Trace("plg_OnCreate_Loans_PreOps -> Fetched Lookup details successfully");

                        // fill empty values from Target Entity -> (null values on form are not included in entity context)
                        if (TargetEntity.Contains("gv_loantenure"))
                            TargetEntity["gv_loantenure"] = LoanTypeEntity.GetAttributeValue<Int32>("gv_loantenure");
                        TargetEntity.Attributes.Add("gv_loantenure", LoanTypeEntity.GetAttributeValue<Int32>("gv_loantenure"));

                        if (TargetEntity.Contains("gv_interest"))
                            TargetEntity["gv_interest"] = LoanTypeEntity.GetAttributeValue<decimal>("gv_interestrate");
                        TargetEntity.Attributes.Add("gv_interest", LoanTypeEntity.GetAttributeValue<decimal>("gv_interestrate"));
                        tracingService.Trace("plg_OnCreate_Loans_PreOps -> Autofilled the records");
                    }
                    else
                    {
                        tracingService.Trace($"plg_OnCreate_Loans_PreOps -> Invalid Plugin Context, cant proceeding further. message - {context.MessageName.ToLower()}, stage - {context.Stage}");
                    }
                }
            }
            catch (Exception ex)
            {
                tracingService.Trace($"plg_OnCreate_Loans_PreOps -> Exception Caught - {ex.Message}, trace -- {ex.StackTrace}");
            }
        }
    }

    public class plg_OnUpdate_Loans_PostOps : IPlugin
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

                    Entity Loandetails = null,AccountDetails = null,userDetail = null,LoanConfigDetails = null;
                    Entity TargetEntity = context.InputParameters["Target"] as Entity;
                    
                    // we get only ID and updated field
                    if (context.MessageName.ToLower() == "update" && context.Stage == 40)
                    {
                        // load additional fields of Target Entity
                        ColumnSet getDataForLoan = new ColumnSet("gv_loantype", "gv_pan");
                        Loandetails = service.Retrieve("gv_bank_loan", TargetEntity.Id, getDataForLoan);

                        // load pan details 
                        AccountDetails = service.Retrieve("gv_bank_account", Loandetails.GetAttributeValue<EntityReference>("gv_pan").Id,
                            new ColumnSet("gv_accountnumber", "gv_accountholder"));

                        // load user details - userScore
                        userDetail = service.Retrieve("gv_bank_user", AccountDetails.GetAttributeValue<EntityReference>("gv_accountholder").Id,
                            new ColumnSet("gv_userscore", "gv_userfullname"));

                        // get LoanConfig data - required score
                        LoanConfigDetails = service.Retrieve("gv_bank_loan_config", Loandetails.GetAttributeValue<EntityReference>("gv_loantype").Id,
                            new ColumnSet("gv_minimumscore"));

                        if (LoanConfigDetails.GetAttributeValue<Int32>("gv_minimumscore") > userDetail.GetAttributeValue<Int32>("gv_userscore"))
                        {
                            tracingService.Trace($"plg_OnUpdate_Loans_PostOps -> User {userDetail.GetAttributeValue<string>("gv_userfullname")} -> score {userDetail.GetAttributeValue<Int32>("gv_userscore")} is lower than the required limit - {LoanConfigDetails.GetAttributeValue<Int32>("gv_minimumscore")}");
                            throw new InvalidPluginExecutionException("user score is lower than the required limit - " + LoanConfigDetails.GetAttributeValue<Int32>("gv_minimumscore"));
                        }
                        tracingService.Trace($"plg_OnUpdate_Loans_PostOps -> Update Success. User {userDetail.GetAttributeValue<string>("gv_userfullname")} -> score {userDetail.GetAttributeValue<Int32>("gv_userscore")} is greater than the required limit - {LoanConfigDetails.GetAttributeValue<Int32>("gv_minimumscore")}");
                    }
                }
                else
                {
                    tracingService.Trace($"plg_OnUpdate_Loans_PostOps -> Invalid Plugin Context, cant proceeding further. message - {context.MessageName.ToLower()}, stage - {context.Stage}");
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
