using Colso.Xrm.DataTransporter.Models;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colso.Xrm.DataTransporter.AppCode
{
    public static class AutoMappings
    {

        public static Item<EntityReference, EntityReference> GetRootBusinessUnitMapping(IOrganizationService sourceService, IOrganizationService targetService)
        {

            // Add BU mappings
            var sourceBU = sourceService.GetRootBusinessUnit();
            var targetBU = targetService.GetRootBusinessUnit();

            if (sourceBU != null && targetBU != null)
                return new Item<EntityReference, EntityReference>(sourceBU, targetBU);

            return null;
        }

        public static Item<EntityReference, EntityReference> GetDefaultTransactionCurrencyMapping(IOrganizationService sourceService, IOrganizationService targetService)
        {
            var sourceTC = sourceService.GetDefaultTransactionCurrency();
            var targetTC = targetService.GetDefaultTransactionCurrency();

            if (sourceTC != null && targetTC != null)
                return new Item<EntityReference, EntityReference>(sourceTC, targetTC);

            return null;
        }

        public static Item<EntityReference, EntityReference>[] GetSystemUsersMapping(IOrganizationService sourceService, IOrganizationService targetService)
        {
            var autoMappings = new List<Item<EntityReference, EntityReference>>();
            var sourceUsers = sourceService.GetSystemUsers();
            var targetUsers = targetService.GetSystemUsers();

            foreach (var su in sourceUsers)
            {
                var domainname = su.GetAttributeValue<string>("domainname");
                // Make sure we have a domain name
                if (!string.IsNullOrEmpty(domainname))
                {
                    var tu = targetUsers.Where(u => u.GetAttributeValue<string>("domainname") == domainname).FirstOrDefault()?.ToEntityReference();
                    // Do we have a target user?
                    if (tu != null)
                        autoMappings.Add(new Item<EntityReference, EntityReference>(su.ToEntityReference(), tu));
                }
            }

            return autoMappings.ToArray();
        }
    }
}
