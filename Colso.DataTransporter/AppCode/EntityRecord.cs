using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Colso.DataTransporter.AppCode
{
    public class EntityRecord
    {
        public enum TransferMode
        {
            None = 0,
            Create = 1,
            Update = 2,
            Delete = 4
        }

        private static List<EntityMetadata> sourceEntitiesMetadata;
        private static List<EntityMetadata> targetEntitiesMetadata;
        private static Guid targetCurrentUserId;
        private readonly IOrganizationService sourceService;
        private readonly IOrganizationService targetService;
        private readonly EntityMetadata entity;
        private readonly List<AttributeMetadata> attributes;
        private readonly TransferMode transfermode;

        public event EventHandler OnStatusMessage;
        public event EventHandler OnProgress;


        public EntityRecord(EntityMetadata entity, List<AttributeMetadata> attributes, TransferMode mode, IOrganizationService sourceService, IOrganizationService targetService)
        {
            if (sourceEntitiesMetadata == null)
            {
                sourceEntitiesMetadata = new List<EntityMetadata>();
            }

            if (targetEntitiesMetadata == null)
            {
                targetEntitiesMetadata = new List<EntityMetadata>();
            }

            if (targetCurrentUserId == Guid.Empty)
            {
                targetCurrentUserId = ((WhoAmIResponse)targetService.Execute(new WhoAmIRequest())).UserId;
            }

            this.entity = entity;
            this.attributes = attributes;
            this.transfermode = mode;
            this.sourceService = sourceService;
            this.targetService = targetService;
        }

        public void Transfer()
        {
            DoTransfer();
        }

        private void DoTransfer()
        {
            var columns = this.attributes.Select(a => a.LogicalName).ToArray();

            // Check if the record already exists on target organization
            var sourceqry = new QueryExpression(entity.LogicalName) { ColumnSet = new ColumnSet(columns) };
            var targetqry = new QueryExpression(entity.LogicalName) { ColumnSet = new ColumnSet(false) };

            SetProgress(0, "Retrieving records...");
            var sourceRetrieveTask = Task.Factory.StartNew<EntityCollection>(() => { return RetrieveAll(sourceService, "source", sourceqry); });
            var targetRetrieveTask = Task.Factory.StartNew<EntityCollection>(() => { return RetrieveAll(targetService, "target", targetqry); });
            Task.WaitAll(sourceRetrieveTask, targetRetrieveTask);

            var sourceRecords = sourceRetrieveTask.Result;
            var targetRecords = targetRetrieveTask.Result;
            var recordCount = sourceRecords.Entities.Count;
            var missingCount = 0;
            var createCount = 0;
            var updateCount = 0;
            var deleteCount = 0;
            var skipCount = 0;
            var totalTaskCount = sourceRecords.Entities.Count;

            // Delete the missing source records in the target environment
            if ((transfermode & TransferMode.Delete) == TransferMode.Delete)
            {
                var missing = targetRecords.Entities.Select(e => e.Id).Except(sourceRecords.Entities.Select(e => e.Id)).ToArray();
                missingCount = missing.Length;
                totalTaskCount += missingCount;
                for (int i = 0; i < missingCount; i++)
                {
                    var record = missing[i];
                    SetProgress(i / totalTaskCount, "");
                    SetStatusMessage("{0}/{1}: delete record", i + 1, missingCount);
                    targetService.Delete(entity.LogicalName, record);
                    deleteCount++;
                }
            }

            for (int i = 0; i < recordCount; i++)
            {
                var record = sourceRecords.Entities[i];
                var recordexist = targetRecords.Entities.Any(e => e.Id.Equals(record.Id));

                var name = entity.DisplayName.UserLocalizedLabel == null ? string.Empty : entity.DisplayName.UserLocalizedLabel.Label;
                SetProgress((i + missingCount) / totalTaskCount, "Transfering entity '{0}'...", name);

                if (recordexist && ((transfermode & TransferMode.Update) == TransferMode.Update))
                {
                    // Update existing record
                    SetStatusMessage("{0}/{1}: update record", i + 1, recordCount);
                    targetService.Update(record);
                    updateCount++;
                }
                else if (!recordexist && ((transfermode & TransferMode.Create) == TransferMode.Create))
                {
                    // Create missing record
                    SetStatusMessage("{0}/{1}: create record", i + 1, recordCount);
                    targetService.Create(record);
                    createCount++;
                }
                else
                {
                    SetStatusMessage("{0}/{1}: skip record", i + 1, recordCount);
                    skipCount++;
                }
            }

            SetStatusMessage("{0} created; {1} updated; {2} deleted; {3} skipped", createCount, updateCount, deleteCount, skipCount);
        }

        private EntityCollection RetrieveAll(IOrganizationService service, string servicename, QueryExpression query, int pageSize = 250)
        {
            var collection = new EntityCollection();
            query.PageInfo.PageNumber = 1;
            query.PageInfo.Count = pageSize;
            query.PageInfo.PagingCookie = null;

            EntityCollection tempCollection;
            do
            {
                tempCollection = service.RetrieveMultiple((QueryBase)query);
                PagingInfo pageInfo = query.PageInfo;
                int num = pageInfo.PageNumber + 1;
                pageInfo.PageNumber = num;
                query.PageInfo.PagingCookie = tempCollection.PagingCookie;
                collection.Entities.AddRange(tempCollection.Entities);
            }
            while (tempCollection.MoreRecords);

            collection.EntityName = query.EntityName;
            collection.MoreRecords = false;
            collection.TotalRecordCount = collection.Entities.Count;

            return collection;
        }

        private void SetStatusMessage(string format, params object[] args)
        {
            // Make sure someone is listening to event
            if (OnStatusMessage == null) return;

            OnStatusMessage(this, new StatusMessageEventArgs(string.Format(format, args)));
        }
        private void SetProgress(int progress, string format, params object[] args)
        {
            // Make sure someone is listening to event
            if (OnProgress == null) return;

            OnProgress(this, new ProgressEventArgs(progress, string.Format(format, args)));
        }
    }
}