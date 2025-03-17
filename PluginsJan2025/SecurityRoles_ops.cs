using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PluginsJan2025
{
    public class SecurityRoles_ops : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.InitiatingUserId);

            try
            {
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity InputEntity = (Entity)context.InputParameters["Target"];
                    tracingService.Trace($"update plugin triggered for table - {InputEntity.LogicalName} and if column gv_name? - {InputEntity.Attributes.Contains("gv_name")}");
                    if (InputEntity.LogicalName == "gv_project" && InputEntity.Attributes.Contains("gv_name"))
                    {
                        if (context.MessageName.ToLower() == "update")
                        {
                            tracingService.Trace($"update plugin triggered for table - {InputEntity.LogicalName} and column - {InputEntity["gv_name"]}");
                           
                            Guid userId = new Guid("513b8243-7bde-ef11-8eea-7c1e523d27d3");
                            Guid roleIdRoles = new Guid("b6d85192-f9ea-ef11-be20-6045bdc5e2d2"); // tt_user role

                            QueryExpression query = new QueryExpression("role");
                            query.ColumnSet = new ColumnSet("name");

                            LinkEntity userRoleLink = new LinkEntity("role", "systemuserrole", "roleid", "roleid", JoinOperator.Inner);
                            //userRoleLink.LinkCriteria.AddCondition("roleid", ConditionOperator.Equal, roleIdRoles);

                            //query.LinkEntities.Add(userRoleLink);

                            EntityCollection results = service.RetrieveMultiple(query);

                            foreach (Entity role in results.Entities)
                            {
                                string roleName = role.GetAttributeValue<string>("name");
                                tracingService.Trace("Role: " + roleName);
                            }
                        }
                    }
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                throw new InvalidPluginExecutionException("An error occurred in the plugin.", ex);
            }
            catch (Exception ex)
            {
                tracingService.Trace("CheckAndUpdateSecurityRole: {0}", ex.ToString());
                throw;
            }
        }
    }


}
