using Colso.Xrm.DataTransporter.Models;
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
using System.Xml;
using static Colso.Xrm.DataTransporter.AppCode.Enumerations;

namespace Colso.DataTransporter.AppCode
{
    public class RelationRecord
    {
        private static List<ManyToManyRelationshipMetadata> sourceEntitiesMetadata;
        private static Guid targetCurrentUserId;
        private static List<ManyToManyRelationshipMetadata> targetEntitiesMetadata;
        private readonly ManyToManyRelationshipMetadata relation;
        private readonly IOrganizationService sourceService;
        private readonly IOrganizationService targetService;
        private readonly TransferMode transfermode;
        private EntityCollection sourceRecords;
        private EntityCollection targetRecords;
        private BackgroundWorker worker;

        public RelationRecord(ManyToManyRelationshipMetadata relation, TransferMode mode, BackgroundWorker worker, IOrganizationService sourceService, IOrganizationService targetService)
        {
            if (sourceEntitiesMetadata == null)
            {
                sourceEntitiesMetadata = new List<ManyToManyRelationshipMetadata>();
            }

            if (targetEntitiesMetadata == null)
            {
                targetEntitiesMetadata = new List<ManyToManyRelationshipMetadata>();
            }

            if (targetCurrentUserId == Guid.Empty)
            {
                targetCurrentUserId = ((WhoAmIResponse)targetService.Execute(new WhoAmIRequest())).UserId;
            }

            this.worker = worker;
            this.relation = relation;
            this.transfermode = mode;
            this.sourceService = sourceService;
            this.targetService = targetService;
            this.Name = relation.SchemaName;
            this.Messages = new List<string>();
        }

        public event EventHandler OnProgress;

        public event EventHandler OnStatusMessage;

        public string Filter { get; set; }
        public List<Item<EntityReference, EntityReference>> Mappings { get; set; }

        public List<string> Messages { get; }
        public string Name { get; }

        public void Transfer(bool useBulk = false, int bulkCount = 100)
        {
            RetrieveData();
            DoTransfer(useBulk, bulkCount);
        }

        private void ApplyMappings(Entity e)
        {
            foreach (var map in Mappings)
            {
                if (relation.Entity1LogicalName == map.Key.LogicalName && map.Key.Id.Equals(e.GetAttributeValue<Guid>(relation.Entity1IntersectAttribute)))
                    e.Attributes[relation.Entity1IntersectAttribute] = map.Value.Id;
                else if (relation.Entity2LogicalName == map.Key.LogicalName && map.Key.Id.Equals(e.GetAttributeValue<Guid>(relation.Entity2IntersectAttribute)))
                    e.Attributes[relation.Entity2IntersectAttribute] = map.Value.Id;
            }
        }

        private void Associate(string relationshipName, string entity1LogicalName, Guid entity1Id, string entity2LogicalName, Guid entity2Id)
        {
            var request = new AssociateEntitiesRequest();
            request.Moniker1 = new EntityReference { Id = entity1Id, LogicalName = entity1LogicalName };
            request.Moniker2 = new EntityReference { Id = entity2Id, LogicalName = entity2LogicalName };
            request.RelationshipName = relationshipName;

            targetService.Execute(request);
        }

        private XmlDocument BuildFetchXml(string entityLogicalName, string[] columns, string filter)
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

        private void Disassociate(string relationshipName, string entity1LogicalName, Guid entity1Id, string entity2LogicalName, Guid entity2Id)
        {
            var request = new DisassociateEntitiesRequest();
            request.Moniker1 = new EntityReference { Id = entity1Id, LogicalName = entity1LogicalName };
            request.Moniker2 = new EntityReference { Id = entity2Id, LogicalName = entity2LogicalName };
            request.RelationshipName = relationshipName;

            targetService.Execute(request);
        }

        private void DoTransfer(bool useBulk = false, int bulkCount = 100)
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
                //var missing = targetRecords.Entities.Select(e => e.Id).Except(sourceRecords.Entities.Select(e => e.Id)).ToArray();
                var missing = targetRecords.Entities
                    .Where(te => !sourceRecords.Entities.Any(se => te.GetAttributeValue<Guid>(relation.Entity1IntersectAttribute).Equals(se.GetAttributeValue<Guid>(relation.Entity1IntersectAttribute)) && te.GetAttributeValue<Guid>(relation.Entity2IntersectAttribute).Equals(se.GetAttributeValue<Guid>(relation.Entity2IntersectAttribute))))
                    .ToArray();

                missingCount = missing.Length;
                totalTaskCount += missingCount;
                for (int i = 0; i < missingCount; i++)
                {
                    processed++;
                    var record = missing[i];
                    var entity1id = record.GetAttributeValue<Guid>(relation.Entity1IntersectAttribute);
                    var entity2id = record.GetAttributeValue<Guid>(relation.Entity2IntersectAttribute);
                    SetProgress(i / totalTaskCount, "");
                    SetStatusMessage(useBulk ? "Adding relationship {0}/{1} for deletion" : "{0}/{1}: delete record", i + 1, missingCount);
                    if ((transfermode & TransferMode.Preview) != TransferMode.Preview)
                    {
                        if (useBulk)
                        {
                            bulk.Requests.Add(new DisassociateRequest
                            {
                                Target = new EntityReference(relation.Entity1LogicalName, entity1id),
                                Relationship = new Relationship(relation.SchemaName),
                                RelatedEntities = new EntityReferenceCollection
                                {
                                    new EntityReference(relation.Entity2LogicalName, entity2id)
                                }
                            });

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
                            Disassociate(relation.SchemaName, relation.Entity1LogicalName, entity1id,
                                relation.Entity2LogicalName, entity2id);
                            deleteCount++;
                        }
                    }
                }
            }

            processed = 0;
            // Transfer records
            for (int i = 0; i < recordCount; i++)
            {
                processed++;
                try
                {
                    var record = sourceRecords.Entities[i];
                    var entity1id = record.GetAttributeValue<Guid>(relation.Entity1IntersectAttribute);
                    var entity2id = record.GetAttributeValue<Guid>(relation.Entity2IntersectAttribute);
                    var recordexist = targetRecords.Entities.Any(e => e.GetAttributeValue<Guid>(relation.Entity1IntersectAttribute).Equals(entity1id) && e.GetAttributeValue<Guid>(relation.Entity2IntersectAttribute).Equals(entity2id));

                    var name = relation.SchemaName;
                    SetProgress((i + missingCount) / totalTaskCount, "Transfering relation '{0}'...", name);

                    if (!recordexist && ((transfermode & TransferMode.Create) == TransferMode.Create))
                    {
                        // Create missing record
                        SetStatusMessage(useBulk ? "Adding relationship {0}/{1} for creation" : "{0}/{1}: create record", i + 1, recordCount);
                        ApplyMappings(record);
                        if ((transfermode & TransferMode.Preview) != TransferMode.Preview)
                        {
                            if (useBulk)
                            {
                                bulk.Requests.Add(new AssociateRequest
                                {
                                    Target = new EntityReference(relation.Entity1LogicalName, entity1id),
                                    Relationship = new Relationship(relation.SchemaName),
                                    RelatedEntities = new EntityReferenceCollection
                                    {
                                        new EntityReference(relation.Entity2LogicalName, entity2id)
                                    }
                                });
                            }
                            else
                            {
                                Associate(relation.SchemaName, relation.Entity1LogicalName, entity1id,
                                    relation.Entity2LogicalName, entity2id);
                                createCount++;
                            }
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
                                    Messages.Add(response.Fault.Message);
                                    errorCount++;
                                }
                            }

                            for (var j = 0; j < bulk.Requests.Count; j++)
                            {
                                if (errorIndexes.Contains(j)) continue;
                                createCount++;
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

            SetStatusMessage("{0} created; {1} updated; {2} deleted; {3} skipped; {4} errors", createCount, updateCount, deleteCount, skipCount, errorCount);
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
            // Check if the record already exists on target organization
            var sourceqry = new QueryExpression(relation.IntersectEntityName) { ColumnSet = new ColumnSet(true) };
            var targetqry = new QueryExpression(relation.IntersectEntityName) { ColumnSet = new ColumnSet(true) };

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

        private void SetStatusMessage(string format, params object[] args)
        {
            // Make sure someone is listening to event
            if (OnStatusMessage == null) return;

            OnStatusMessage(this, new StatusMessageEventArgs(string.Format(format, args)));
        }
    }
}