﻿using Colso.Xrm.DataTransporter.Models;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using static Colso.Xrm.DataTransporter.AppCode.Enumerations;
using UpdateRequest = Microsoft.Xrm.Sdk.Messages.UpdateRequest;

namespace Colso.DataTransporter.AppCode
{
    public class EntityRecord
    {
        private static List<EntityMetadata> sourceEntitiesMetadata;
        private static Guid targetCurrentUserId;
        private static List<EntityMetadata> targetEntitiesMetadata;
        private readonly List<AttributeMetadata> attributes;
        private readonly EntityMetadata entity;
        private readonly IOrganizationService sourceService;
        private readonly IOrganizationService targetService;
        private readonly TransferMode transfermode;
        private EntityCollection sourceRecords;
        private EntityCollection targetRecords;
        private BackgroundWorker worker;

        public EntityRecord(EntityMetadata entity, List<AttributeMetadata> attributes, TransferMode mode, BackgroundWorker worker, IOrganizationService sourceService, IOrganizationService targetService)
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

            this.worker = worker;
            this.entity = entity;
            this.attributes = attributes;
            this.transfermode = mode;
            this.sourceService = sourceService;
            this.targetService = targetService;
            this.Name = entity.DisplayName.UserLocalizedLabel == null ? string.Empty : entity.DisplayName.UserLocalizedLabel.Label;
            this.Messages = new List<string>();
            this.PreviewList = new List<ListViewItem>();
        }

        public event EventHandler OnProgress;

        public event EventHandler OnStatusMessage;

        public string Filter { get; set; }
        public List<Item<EntityReference, EntityReference>> Mappings { get; set; }

        public List<string> Messages { get; }
        public string Name { get; }
        public List<ListViewItem> PreviewList { get; }

        public void Transfer(bool useBulk = false, int bulkCount = 200)
        {
            RetrieveData();
            DoTransfer(useBulk, bulkCount);
        }

        private void AddMissingAttributes(Entity entity)
        {
            // Make sure only selected attributes are send
            var missingattributes = this.attributes.Where(ae => !entity.Attributes.Any(a => a.Key.Equals(ae.LogicalName))).Select(ae => ae.LogicalName).ToArray();

            // Remove unwanted attributes
            foreach (var att in missingattributes)
                entity.Attributes.Add(att, null);
        }

        private void ApplyEntityCollectionMappings(Entity e)
        {
            var references = e.Attributes.Select(a => a.Value).OfType<EntityCollection>().ToArray();
            foreach (var ec in references)
                foreach (var entity in ec.Entities)
                {
                    ApplyMappings(entity);
                }
        }

        private void ApplyMappings(Entity e)
        {
            //.OfType<A>()
            var references = e.Attributes.Select(a => a.Value).OfType<EntityReference>().ToArray();

            foreach (var map in Mappings)
            {
                var matches = references.Where(r => r.LogicalName == map.Key.LogicalName && r.Id.Equals(map.Key.Id)).ToArray();

                foreach (var match in matches)
                    match.Id = map.Value.Id;
            }
        }

        private XmlDocument BuildFetchXml(string entityLogicalName, string[] columns, string filter)
        {
            // Check the filter
            filter = filter?.Trim();
            if (!string.IsNullOrEmpty(filter))
            {
                var filterdoc = new XmlDocument();
                filterdoc.LoadXml(filter);
                var rootfilter = filterdoc.DocumentElement;
                if (rootfilter.Name == "fetch")
                {
                    return filterdoc;
                }
            }

            return BuildFetchXmlWithFilter(entityLogicalName, columns, filter);
        }

        private XmlDocument BuildFetchXmlWithFilter(string entityLogicalName, string[] columns, string filter)
        {
            var doc = new XmlDocument();

            // the xml declaration is recommended, but not mandatory
            var xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.InsertBefore(xmlDeclaration, doc.DocumentElement);

            // string.Empty makes cleaner code
            var elfetch = doc.CreateElement(string.Empty, "fetch", string.Empty);
            doc.AppendChild(elfetch);

            // Add fetch attributes
            var attversion = doc.CreateAttribute("version");
            attversion.Value = "1.0";
            elfetch.Attributes.Append(attversion);
            var attoutputformat = doc.CreateAttribute("output-format");
            attoutputformat.Value = "xml-platform";
            elfetch.Attributes.Append(attoutputformat);
            var attmapping = doc.CreateAttribute("mapping");
            attmapping.Value = "logical";
            elfetch.Attributes.Append(attmapping);
            var attdistinct = doc.CreateAttribute("distinct");
            attdistinct.Value = "false";
            elfetch.Attributes.Append(attdistinct);

            // Create entity element
            var elEntity = doc.CreateElement(string.Empty, "entity", string.Empty);
            elfetch.AppendChild(elEntity);

            // Add entity attributes
            var attname = doc.CreateAttribute("name");
            attname.Value = entityLogicalName;
            elEntity.Attributes.Append(attname);

            // Add all attributes
            foreach (var column in columns)
            {
                var elAttribute = doc.CreateElement(string.Empty, "attribute", string.Empty);
                elEntity.AppendChild(elAttribute);

                // Add entity attributes
                var attaname = doc.CreateAttribute("name");
                attaname.Value = column;
                elAttribute.Attributes.Append(attaname);
            }

            // Add the filter
            filter = filter?.Trim();
            if (!string.IsNullOrEmpty(filter))
            {
                var filterdoc = new XmlDocument();
                filterdoc.LoadXml(filter);
                var rootfilter = filterdoc.DocumentElement;
                var elFilter = doc.ImportNode(rootfilter, true);
                elEntity.AppendChild(elFilter);
            }

            return doc;
        }

        private string CreateXml(XmlDocument doc, string cookie, int page, int count)
        {
            XmlAttributeCollection attributes = doc.DocumentElement.Attributes;
            if (cookie != null)
            {
                XmlAttribute attribute = doc.CreateAttribute("paging-cookie");
                attribute.Value = cookie;
                attributes.Append(attribute);
            }
            XmlAttribute attribute1 = doc.CreateAttribute("page");
            attribute1.Value = Convert.ToString(page);
            attributes.Append(attribute1);
            XmlAttribute attribute2 = doc.CreateAttribute("count");
            attribute2.Value = Convert.ToString(count);
            attributes.Append(attribute2);
            StringBuilder sb = new StringBuilder(1024);
            using (StringWriter stringWriter = new StringWriter(sb))
            {
                XmlTextWriter xmlTextWriter = new XmlTextWriter((TextWriter)stringWriter);
                doc.WriteTo((XmlWriter)xmlTextWriter);
                xmlTextWriter.Close();
            }
            return sb.ToString();
        }

        private void DoTransfer(bool useBulk = false, int bulkCount = 200)
        {
            if (sourceRecords == null || targetRecords == null)
                return;

            var recordCount = sourceRecords.Entities.Count;
            var missingCount = 0;
            var createCount = 0;
            var updateCount = 0;
            var deleteCount = 0;
            var skipCount = 0;
            var errorCount = 0;
            var totalTaskCount = sourceRecords.Entities.Count;
            var bulk = new ExecuteMultipleRequest
            {
                Requests = new OrganizationRequestCollection(),
                Settings = new ExecuteMultipleSettings
                {
                    ContinueOnError = true
                }
            };

            int processed = 0;

            // Delete the missing source records in the target environment
            if ((transfermode & TransferMode.Delete) == TransferMode.Delete)
            {
                var missing = targetRecords.Entities.Select(e => e.Id).Except(sourceRecords.Entities.Select(e => e.Id)).ToArray();
                missingCount = missing.Length;
                totalTaskCount += missingCount;
                for (int i = 0; i < missingCount; i++)
                {
                    if (worker.CancellationPending) return;

                    processed++;
                    var recordid = missing[i];
                    SetProgress(i / totalTaskCount, "");
                    SetStatusMessage("{0}/{1}: delete record", i + 1, missingCount);
                    if ((transfermode & TransferMode.Preview) != TransferMode.Preview)
                    {
                        if (useBulk)
                        {
                            bulk.Requests.Add(new DeleteRequest { Target = new EntityReference(entity.LogicalName, recordid) });
                        }
                        else
                        {
                            targetService.Delete(entity.LogicalName, recordid);
                        }
                    }
                    else
                    {
                        var record = targetRecords.Entities.Where(e => e.Id.Equals(recordid)).FirstOrDefault();
                        var recordname = record.GetAttributeValue<string>(this.entity.PrimaryNameAttribute);
                        var lvrecord = ToListViewItem(recordname, record, "DELETE");
                        PreviewList.Add(lvrecord);
                    }

                    if (useBulk)
                    {
                        if (bulk.Requests.Count % bulkCount == 0 || processed == missingCount)
                        {
                            var bulkResponse = (ExecuteMultipleResponse)targetService.Execute(bulk);
                            // MscrmTools (26/11/2020): Should we need to handle potential exceptions?
                            deleteCount += bulk.Requests.Count;
                            bulk.Requests = new OrganizationRequestCollection();
                        }
                    }
                    else
                    {
                        deleteCount++;
                    }
                }
            }

            // Transfer records
            processed = 0;
            for (int i = 0; i < recordCount; i++)
            {
                if (worker.CancellationPending) return;

                processed++;
                try
                {
                    var record = sourceRecords.Entities[i];
                    var targetEntity = targetRecords.Entities.FirstOrDefault(e => e.Id.Equals(record.Id));
                    var recordexist = targetEntity != null;
                    var name = entity.DisplayName.UserLocalizedLabel == null ? string.Empty : entity.DisplayName.UserLocalizedLabel.Label;
                    SetProgress((i + missingCount) / totalTaskCount, "Transfering entity '{0}'...", name);

                    // BC 22/11/2016: some attributes are auto added in the result query
                    var recordname = record.GetAttributeValue<string>(this.entity.PrimaryNameAttribute);
                    AddMissingAttributes(record); // BC 19/12/2017: null values are not retrieved
                    var sourceStateAndStatus = RemoveStateAndStatus(record);
                    RemoveUnwantedAttributes(record);

                    if (recordexist && ((transfermode & TransferMode.Update) == TransferMode.Update))
                    {
                        // Update existing record
                        SetStatusMessage(
                            useBulk ? "{0}/{1}: Adding record '{2}' for update" : "{0}/{1}: update record '{2}'", i + 1,
                            recordCount, recordname);

                        ApplyEntityCollectionMappings(record);
                        ApplyMappings(record);
                        if ((transfermode & TransferMode.Preview) != TransferMode.Preview)
                        {
                            if (useBulk)
                            {
                                var setStateRequest = GetSetState(record, sourceStateAndStatus, targetEntity);
                                var requests = new OrganizationRequestCollection { new UpdateRequest { Target = record } };
                                if (setStateRequest != null) requests.Add(setStateRequest);

                                bulk.Requests.Add(new ExecuteTransactionRequest { Requests = requests });
                            }
                            else
                            {
                                targetService.Update(record);
                                SetState(record, sourceStateAndStatus, targetEntity);
                            }
                        }
                        else
                        {
                            PreviewList.Add(ToListViewItem(recordname, record, "UPDATE"));
                        }

                        if (!useBulk)
                        {
                            updateCount++;
                        }
                    }
                    else if (!recordexist && ((transfermode & TransferMode.Create) == TransferMode.Create))
                    {
                        // Create missing record
                        SetStatusMessage(
                            useBulk ? "{0}/{1}: Adding record '{2}' for create" : "{0}/{1}: create record '{2}'", i + 1,
                            recordCount, recordname);

                        ApplyEntityCollectionMappings(record);
                        ApplyMappings(record);
                        if ((transfermode & TransferMode.Preview) != TransferMode.Preview)
                        {
                            if (useBulk)
                            {
                                var setStateRequest = GetSetState(record, sourceStateAndStatus);
                                var requests = new OrganizationRequestCollection { new CreateRequest { Target = record } };
                                if (setStateRequest != null) requests.Add(setStateRequest);

                                bulk.Requests.Add(new ExecuteTransactionRequest { Requests = requests });
                            }
                            else
                            {
                                targetService.Create(record);
                                SetState(record, sourceStateAndStatus);
                            }
                        }
                        else
                        {
                            PreviewList.Add(ToListViewItem(recordname, record, "CREATE"));
                        }

                        if (!useBulk)
                        {
                            createCount++;
                        }
                    }
                    else
                    {
                        SetStatusMessage("{0}/{1}: skip record", i + 1, recordCount);
                        skipCount++;
                    }

                    if (useBulk)
                    {
                        if (bulk.Requests.Count > 0 && bulk.Requests.Count % bulkCount == 0 || processed == recordCount)
                        {
                            SetStatusMessage($"Processing records {processed - bulk.Requests.Count} to {processed} of {recordCount}");

                            var bulkResponse = (ExecuteMultipleResponse)targetService.Execute(bulk);
                            // MscrmTools (26/11/2020): Should we need to handle potential exceptions?

                            var errorIndexes = new List<int>();
                            foreach (var response in bulkResponse.Responses)
                            {
                                if (response.Fault != null)
                                {
                                    errorIndexes.Add(response.RequestIndex);
                                    this.Messages.Add(response.Fault.Message);
                                    errorCount++;
                                }
                            }

                            for (var j = 0; j < bulk.Requests.Count; j++)
                            {
                                if (errorIndexes.Contains(j)) continue;
                                if (bulk.Requests[j] is ExecuteTransactionRequest etr)
                                {
                                    if (etr.Requests.First() is CreateRequest)
                                    {
                                        createCount++;
                                    }
                                    else
                                    {
                                        updateCount++;
                                    }
                                }
                            }

                            bulk.Requests = new OrganizationRequestCollection();
                        }
                    }
                }
                catch (System.ServiceModel.FaultException<OrganizationServiceFault> error)
                {
                    this.Messages.Add(error.Message);
                    errorCount++;
                }
            }

            if ((transfermode & TransferMode.Preview) != TransferMode.Preview) SetStatusMessage("{0} created; {1} updated; {2} deleted; {3} skipped; {4} errors", createCount, updateCount, deleteCount, skipCount, errorCount);
            else SetStatusMessage("PREVIEW: {0} created; {1} updated; {2} deleted; {3} skipped; {4} errors", createCount, updateCount, deleteCount, skipCount, errorCount);
        }

        private SetStateRequest GetSetState(Entity target, RecordStateAndStatus sourceStateAndStatus, Entity currentStateAndStatusEntity = null)
        {
            // Don't update state when it's not available
            if (sourceStateAndStatus.State == null)
                return null;

            if (currentStateAndStatusEntity != null)
            {
                var currentStateAndStatus = RemoveStateAndStatus(currentStateAndStatusEntity);

                // If the entity has no state: return
                if (currentStateAndStatus.State == null || (sourceStateAndStatus.State.Value == currentStateAndStatus.State.Value && sourceStateAndStatus.Status.Value == currentStateAndStatus.Status.Value))
                    return null;
            }

            return new SetStateRequest()
            {
                EntityMoniker = target.ToEntityReference(),
                State = sourceStateAndStatus.State,
                Status = sourceStateAndStatus.Status
            };
        }

        private RecordStateAndStatus RemoveStateAndStatus(Entity record)
        {
            var recordStateAndStatus = new RecordStateAndStatus
            {
                State = record.GetAttributeValue<OptionSetValue>("statecode"),
                Status = record.GetAttributeValue<OptionSetValue>("statuscode")
            };

            record.Attributes.Remove("statecode");
            record.Attributes.Remove("statuscode");

            return recordStateAndStatus;
        }

        private void RemoveUnwantedAttributes(Entity entity)
        {
            // Make sure only selected attributes are send
            var unwantedattributes = entity.Attributes.Where(ae => !this.attributes.Any(a => a.LogicalName.Equals(ae.Key))).Select(ae => ae.Key).ToArray();

            // Remove unwanted attributes
            foreach (var att in unwantedattributes)
                entity.Attributes.Remove(att);
        }

        private EntityCollection RetrieveAll(IOrganizationService service, QueryExpression query, int pageSize = 250)
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

        private EntityCollection RetrieveAll(IOrganizationService service, XmlDocument fetchXml, int pageSize = 250)
        {
            EntityCollection collection = new EntityCollection();
            int page = 1;
            string cookie = (string)null;
            EntityCollection tempCollection;
            do
            {
                tempCollection = service.RetrieveMultiple((QueryBase)new FetchExpression(this.CreateXml(fetchXml, cookie, page, pageSize)));
                ++page;
                cookie = tempCollection.PagingCookie;
                collection.Entities.AddRange((IEnumerable<Entity>)tempCollection.Entities);
            }
            while (tempCollection.MoreRecords);

            collection.MoreRecords = false;
            collection.TotalRecordCount = collection.Entities.Count;

            return collection;
        }

        private void RetrieveData()
        {
            var columns = this.attributes.Select(a => a.LogicalName).ToList();
            if (!columns.Contains(this.entity.PrimaryNameAttribute)) columns.Add(this.entity.PrimaryNameAttribute);

            // Check if the record already exists on target organization
            var sourceqry = BuildFetchXml(entity.LogicalName, columns.ToArray(), Filter);

            var targetcolumnset = new ColumnSet(this.entity.PrimaryNameAttribute);
            // Make sure we have a state on the entity
            if (this.entity.Attributes.Any(a => a != null && !string.IsNullOrEmpty(a.LogicalName) && a.LogicalName.Equals("statecode")))
                targetcolumnset.AddColumns("statecode", "statuscode");
            var targetqry = new QueryExpression(entity.LogicalName) { ColumnSet = targetcolumnset };

            SetProgress(0, "Retrieving records...");
            var sourceRetrieveTask = Task.Factory.StartNew<EntityCollection>(() => { return RetrieveAll(sourceService, sourceqry); });
            var targetRetrieveTask = Task.Factory.StartNew<EntityCollection>(() => { return RetrieveAll(targetService, targetqry); });
            Task.WaitAll(sourceRetrieveTask, targetRetrieveTask);

            sourceRecords = sourceRetrieveTask.Result;
            targetRecords = targetRetrieveTask.Result;
        }

        private void SetProgress(int progress, string format, params object[] args)
        {
            // Make sure someone is listening to event
            if (OnProgress == null) return;

            OnProgress(this, new ProgressEventArgs(progress, string.Format(format, args)));
        }

        private void SetState(Entity target, RecordStateAndStatus sourceStateAndStatus, Entity currentStateAndStatusEntity = null)
        {
            // Don't update state when it's not available
            if (sourceStateAndStatus.State == null)
                return;

            if (currentStateAndStatusEntity != null)
            {
                var currentStateAndStatus = RemoveStateAndStatus(currentStateAndStatusEntity);

                // If the entity has no state: return
                if (currentStateAndStatus.State == null || (sourceStateAndStatus.State.Value == currentStateAndStatus.State.Value && sourceStateAndStatus.Status.Value == currentStateAndStatus.Status.Value))
                    return;
            }

            SetStateRequest setState = new SetStateRequest()
            {
                EntityMoniker = target.ToEntityReference(),
                State = sourceStateAndStatus.State,
                Status = sourceStateAndStatus.Status
            };

            targetService.Execute(setState);
        }

        private void SetStatusMessage(string format, params object[] args)
        {
            // Make sure someone is listening to event
            if (OnStatusMessage == null) return;

            OnStatusMessage(this, new StatusMessageEventArgs(string.Format(format, args)));
        }

        private ListViewItem ToListViewItem(string name, Entity entity, string action)
        {
            var lv = new ListViewItem(action);
            lv.SubItems.Add(entity.Id.ToString());
            lv.SubItems.Add(name);

            // Add all attributes
            foreach (var item in entity.Attributes)
            {
                //lv.SubItems.Add(item.Value);
            }

            return lv;
        }
    }
}