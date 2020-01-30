using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colso.Xrm.DataTransporter.AppCode
{
    internal static partial class XrmConnection
    {
        public static EntityReference GetRootBusinessUnit(this IOrganizationService service)
        {
            var qry = new Microsoft.Xrm.Sdk.Query.QueryExpression("businessunit");
            qry.Criteria.AddCondition("parentbusinessunitid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Null);
            var results = service.RetrieveMultiple(qry);

            if (results != null && results.Entities.Count > 0)
                return results.Entities[0].ToEntityReference();

            return null;
        }

        public static EntityReference GetDefaultTransactionCurrency(this IOrganizationService service)
        {
            var qry = new Microsoft.Xrm.Sdk.Query.QueryExpression("transactioncurrency");
            var results = service.RetrieveMultiple(qry);

            if (results != null && results.Entities.Count > 0)
                return results.Entities[0].ToEntityReference();

            return null;
        }

        public static Entity[] GetSystemUsers(this IOrganizationService service)
        {
            var qry = new Microsoft.Xrm.Sdk.Query.QueryExpression("systemuser");
            qry.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("domainname");
            var results = service.RetrieveMultiple(qry);

            return results.Entities.ToArray<Entity>();
        }
    }
}
