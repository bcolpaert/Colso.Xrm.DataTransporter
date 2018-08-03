using Colso.DataTransporter.AppCode;
using Colso.DataTransporter.Forms;
using Colso.Xrm.DataTransporter.Models;
using McTools.Xrm.Connection;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Windows.Forms;
using System.Xml;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Args;
using XrmToolBox.Extensibility.Interfaces;

namespace Colso.DataTransporter
{
    public partial class DataTransporter : MultipleConnectionsPluginControlBase, IXrmToolBoxPluginControl, IGitHubPlugin, IHelpPlugin, IStatusBarMessenger, IPayPalPlugin
    {
        #region Variables

        /// <summary>
        /// Information panel
        /// </summary>
        private Panel informationPanel;

        /// <summary>
        /// Dynamics CRM 2011 target organization service
        /// </summary>
        private IOrganizationService targetService;

        private bool workingstate = false;
        private Guid organisationid;
        private Settings settings;

        // keep list of listview items 
        List<ListViewItem> Entities = new List<ListViewItem>();
        List<ListViewItem> Associations = new List<ListViewItem>();
        private enum ServiceType
        {
            Source,
            Target
        }

        #endregion Variables

        public DataTransporter()
        {
            SettingFileHandler.GetConfigData(out settings);
            InitializeComponent();
        }

        #region XrmToolbox

        //public event EventHandler OnCloseTool;
        //public event EventHandler OnRequestConnection;
        public event EventHandler<StatusBarMessageEventArgs> SendMessageToStatusBar;

        public Image PluginLogo
        {
            get { return null; }
        }

        //public IOrganizationService Service
        //{
        //    get { throw new NotImplementedException(); }
        //}

        public string HelpUrl
        {
            get
            {
                return "https://github.com/MscrmTools/Colso.Xrm.DataTransporter/wiki";
            }
        }

        public string RepositoryName
        {
            get
            {
                return "Colso.Xrm.DataTransporter";
            }
        }

        public string UserName
        {
            get
            {
                return "MscrmTools";
            }
        }

        public string DonationDescription
        {
            get
            {
                return "Donation for Data Transporter Tool - XrmToolBox";
            }
        }

        public string EmailAccount
        {
            get
            {
                return "bramcolpaert@outlook.com";
            }
        }

        public string GetCompany()
        {
            return GetType().GetCompany();
        }

        public string GetMyType()
        {
            return GetType().FullName;
        }

        public string GetVersion()
        {
            return GetType().Assembly.GetName().Version.ToString();
        }

        #endregion XrmToolbox

        #region Form events

        private void btnSelectTarget_Click(object sender, EventArgs e)
        {
            AddAdditionalOrganization();
        }

        protected override void ConnectionDetailsUpdated(NotifyCollectionChangedEventArgs e)
        {
            // For now, only support one target org
            if (e.Action.Equals(NotifyCollectionChangedAction.Add))
            {
                var detail = (ConnectionDetail)e.NewItems[0];
                SetConnectionLabel(detail.ConnectionName, ServiceType.Target);
                targetService = detail.ServiceClient;
            }
        }

        protected void DataTransporter_ConnectionUpdated(object sender, ConnectionUpdatedEventArgs e)
        {
            var service = e.Service;
            var detail = e.ConnectionDetail;

            organisationid = detail.ConnectionId.Value;
            SetConnectionLabel(detail.ConnectionName, ServiceType.Source);
            // init buttons -> value based on connection
            InitMappings();
            InitFilter();
            // Save settings file
            SettingFileHandler.SaveConfigData(settings);
            // Load entities when source connection changes
            PopulateEntities();
        }

        private void tsbCloseThisTab_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        private void tsbRefreshEntities_Click(object sender, EventArgs e)
        {
            PopulateEntities();
        }

        private void tsbRefreshAssociations_Click(object sender, EventArgs e)
        {
            PopulateAssociations();
        }

        private void tsbTransferData_Click(object sender, EventArgs e)
        {
            ExecuteAction(false);
        }

        private void btnPreviewTransfer_Click(object sender, EventArgs e)
        {
            ExecuteAction(true);
        }

        private void tabSourceObjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Associations.Count == 0)
            {
                // Initial load
                PopulateAssociations();
            }
        }

        private void lvEntities_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateAttributes();
        }

        private void lvAssociations_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lvEntities_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            SetListViewSorting(lvEntities, e.Column);
        }

        private void lvAttributes_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            SetListViewSorting(lvAttributes, e.Column);
        }

        private void lvAssociations_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            SetListViewSorting(lvAssociations, e.Column);
        }

        private void txtEntityFilter_TextChanged(object sender, EventArgs e)
        {
            SetListViewFilter(lvEntities, Entities, txtEntityFilter.Text);
        }

        private void txtAssFilter_TextChanged(object sender, EventArgs e)
        {
            SetListViewFilter(lvAssociations, Associations, txtAssFilter.Text);
        }

        private void chkAllAttributes_CheckedChanged(object sender, EventArgs e)
        {
            lvAttributes.Items.OfType<ListViewItem>().ToList().ForEach(item => item.Checked = chkAllAttributes.Checked);
        }

        private void btnMappings_Click(object sender, EventArgs e)
        {
            var entities = Entities.ToArray();
            var mappingDialog = new MappingList(entities, settings[organisationid].Mappings);
            mappingDialog.ShowDialog(ParentForm);
            settings[organisationid].Mappings = mappingDialog.GetMappingList();
            InitMappings();
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            if (lvEntities.SelectedItems.Count > 0)
            {
                var entityitem = lvEntities.SelectedItems[0];

                if (entityitem != null && entityitem.Tag != null)
                {
                    var filter = string.Empty;
                    var entity = (EntityMetadata)entityitem.Tag;
                    var filterDialog = new FilterEditor(filter = settings[organisationid][entity.LogicalName].Filter);
                    filterDialog.ShowDialog(ParentForm);
                    settings[organisationid][entity.LogicalName].Filter = filterDialog.Filter;
                    InitFilter();
                }
            }
        }

        private void donateInUSDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenDonationPage("USD");
        }

        private void donateInEURToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenDonationPage("EUR");
        }

        private void donateInGBPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenDonationPage("GBP");
        }

        protected void DataTransporter_OnCloseTool(object sender, EventArgs e)
        {
            // First save settings file
            SettingFileHandler.SaveConfigData(settings);
        }

        #endregion Form events

        #region Methods

        private void InitMappings()
        {
            btnEntityMappings.ForeColor = settings[organisationid].Mappings.Count == 0 ? Color.Black : Color.Blue;
            btnAssMappings.ForeColor = settings[organisationid].Mappings.Count == 0 ? Color.Black : Color.Blue;
        }

        private void InitFilter()
        {
            string filter = null;

            if (lvEntities.SelectedItems.Count > 0)
            {
                var entityitem = lvEntities.SelectedItems[0];

                if (entityitem != null && entityitem.Tag != null)
                {
                    var entity = (EntityMetadata)entityitem.Tag;
                    filter = settings[organisationid][entity.LogicalName].Filter;
                }
            }

            btnFilter.ForeColor = string.IsNullOrEmpty(filter) ? Color.Black : Color.Blue;
        }

        private void SetConnectionLabel(string name, ServiceType serviceType)
        {
            switch (serviceType)
            {
                case ServiceType.Source:
                    lbSourceValue.Text = name;
                    lbSourceValue.ForeColor = Color.Green;
                    break;

                case ServiceType.Target:
                    lbTargetValue.Text = name;
                    lbTargetValue.ForeColor = Color.Green;
                    break;
            }
        }

        private void ManageWorkingState(bool working)
        {
            workingstate = working;
            tabSourceObjects.Enabled = !working;
            Cursor = working ? Cursors.WaitCursor : Cursors.Default;
        }

        private bool CheckConnection()
        {
            if (this.Service == null)
            {
                //if (OnRequestConnection != null)
                //{
                //    var args = new RequestConnectionEventArgs
                //    {
                //        ActionName = "Load",
                //        Control = this
                //    };
                //    OnRequestConnection(this, args);
                //}
                var args = new RequestConnectionEventArgs { ActionName = "Load", Control = this };
                RaiseRequestConnectionEvent(args);

                return false;
            }
            else
            {
                return true;
            }
        }

        private void PopulateEntities()
        {
            if (!CheckConnection())
                return;

            if (!workingstate)
            {
                // Reinit other controls
                lvAttributes.Items.Clear();
                ManageWorkingState(true);

                informationPanel = InformationPanel.GetInformationPanel(this, "Loading entities...", 340, 150);

                // Launch treatment
                var bwFill = new BackgroundWorker();
                bwFill.DoWork += (sender, e) =>
                {
                    // Retrieve 
                    List<EntityMetadata> sourceList = MetadataHelper.RetrieveEntities(this.Service);

                    // Prepare list of items
                    Entities.Clear();

                    foreach (EntityMetadata entity in sourceList)
                    {
                        var name = entity.DisplayName.UserLocalizedLabel == null ? string.Empty : entity.DisplayName.UserLocalizedLabel.Label;
                        var item = new ListViewItem(name) {
                            Tag = entity
                        };
                        item.SubItems.Add(entity.LogicalName);

                        if (!entity.IsCustomizable.Value)
                        {
                            item.ForeColor = Color.Gray;
                            item.SubItems.Add("This entity has not been defined as customizable");
                        }

                        Entities.Add(item);
                    }

                    e.Result = Entities;
                };
                bwFill.RunWorkerCompleted += (sender, e) =>
                {
                    informationPanel.Dispose();

                    if (e.Error != null)
                    {
                        MessageBox.Show(this, "An error occured: " + e.Error.Message, "Error", MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    }
                    else
                    {
                        var items = (List<ListViewItem>)e.Result;
                        if (items.Count == 0)
                            MessageBox.Show(this, "The system does not contain any entities", "Warning", MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning);
                        else
                            SetListViewFilter(lvEntities, items, txtEntityFilter.Text);
                    }

                    ManageWorkingState(false);
                };
                bwFill.RunWorkerAsync();
            }
        }

        private void PopulateAttributes()
        {
            if (!workingstate)
            {
                // Reinit other controls
                lvAttributes.Items.Clear();
                chkAllAttributes.Checked = true;
                InitFilter();

                if (lvEntities.SelectedItems.Count > 0)
                {
                    var entityitem = lvEntities.SelectedItems[0];

                    if (entityitem != null && entityitem.Tag != null)
                    {
                        ManageWorkingState(true);

                        var entity = (EntityMetadata)entityitem.Tag;

                        // Launch treatment
                        var bwFill = new BackgroundWorker();
                        bwFill.DoWork += (sender, e) =>
                        {
                            // Retrieve 
                            var entitymeta = MetadataHelper.RetrieveEntity(entity.LogicalName, this.Service);
                            entityitem.Tag = entitymeta;

                            // Get attribute checked settings
                            var unmarkedattributes = settings[organisationid][entity.LogicalName].UnmarkedAttributes;

                            // Prepare list of items
                            var sourceAttributesList = new List<ListViewItem>();

                            // Only use create/editable attributes && properties which are valid for read
                            var attributes = entitymeta.Attributes
                                .Where(a => (a.IsValidForCreate != null && a.IsValidForCreate.Value) || (a.IsValidForUpdate != null && a.IsValidForUpdate.Value))
                                .Where(a => a.IsValidForRead != null && a.IsValidForRead.Value)
                                .ToList();

                            if(attributes.FirstOrDefault(x=> x.LogicalName == "statecode") == null)
                            {
                                attributes.Add(entitymeta.Attributes.FirstOrDefault(x => x.LogicalName == "statecode"));
                            }

                            foreach (AttributeMetadata attribute in attributes)
                            {
                                if (attribute == null)
                                    continue;

                                var name = attribute.DisplayName.UserLocalizedLabel == null ? string.Empty : attribute.DisplayName.UserLocalizedLabel.Label;
                                var typename = attribute.AttributeTypeName == null ? string.Empty : attribute.AttributeTypeName.Value;

                                if (typename != "EntityName")
                                {
                                    var item = new ListViewItem(name){ Tag = attribute };
                                    item.SubItems.Add(attribute.LogicalName);
                                    item.SubItems.Add(typename.EndsWith("Type") ? typename.Substring(0, typename.LastIndexOf("Type")) : typename);

                                    if (attribute.IsValidForCreate == null || !attribute.IsValidForCreate.Value)
                                    {
                                        item.ForeColor = Color.Gray;
                                        item.ToolTipText = "This attribute is not valid for create";
                                        item.SubItems.Add(item.ToolTipText);
                                    }
                                    else if (attribute.IsValidForUpdate == null || !attribute.IsValidForUpdate.Value)
                                    {
                                        item.ForeColor = Color.Gray;
                                        item.ToolTipText = "This attribute is not valid for update";
                                        item.SubItems.Add(item.ToolTipText);
                                    }

                                    item.Checked = !unmarkedattributes.Contains(attribute.LogicalName);
                                    sourceAttributesList.Add(item);
                                }
                            }

                            e.Result = sourceAttributesList;
                        };
                        bwFill.RunWorkerCompleted += (sender, e) =>
                        {
                            if (e.Error != null)
                            {
                                MessageBox.Show(this, "An error occured: " + e.Error.Message, "Error", MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                            }
                            else
                            {
                                var items = (List<ListViewItem>)e.Result;
                                if (items.Count == 0)
                                {
                                    MessageBox.Show(this, "The entity does not contain any attributes", "Warning", MessageBoxButtons.OK,
                                                    MessageBoxIcon.Warning);
                                }
                                else
                                {
                                    lvAttributes.Items.AddRange(items.ToArray());
                                }
                            }

                            ManageWorkingState(false);
                        };
                        bwFill.RunWorkerAsync();
                    }
                }
            }
        }

        private void PopulateAssociations()
        {
            if (!CheckConnection())
                return;

            if (!workingstate)
            {
                // Reinit other controls
                ManageWorkingState(true);

                informationPanel = InformationPanel.GetInformationPanel(this, "Loading associations...", 340, 150);

                // Launch treatment
                var bwFill = new BackgroundWorker();
                bwFill.DoWork += (sender, e) =>
                {
                    // Retrieve 
                    List<ManyToManyRelationshipMetadata> sourceList = MetadataHelper.RetrieveAssociations(this.Service);

                    // Prepare list of items
                    Associations.Clear();

                    foreach (ManyToManyRelationshipMetadata ass in sourceList)
                    {
                        var name = ass.SchemaName;
                        var item = new ListViewItem(name) { Tag = ass };
                        item.SubItems.Add(ass.IntersectEntityName);
                        item.SubItems.Add(ass.Entity1LogicalName);
                        item.SubItems.Add(ass.Entity1IntersectAttribute);
                        item.SubItems.Add(ass.Entity2LogicalName);
                        item.SubItems.Add(ass.Entity2IntersectAttribute);

                        if (!ass.IsCustomizable.Value)
                        {
                            item.ForeColor = Color.Gray;
                            item.SubItems.Add("This relation has not been defined as customizable");
                        }

                        Associations.Add(item);
                    }

                    e.Result = Associations;
                };
                bwFill.RunWorkerCompleted += (sender, e) =>
                {
                    informationPanel.Dispose();

                    if (e.Error != null)
                    {
                        MessageBox.Show(this, "An error occured: " + e.Error.Message, "Error", MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    }
                    else
                    {
                        var items = (List<ListViewItem>)e.Result;
                        if (items.Count == 0)
                            MessageBox.Show(this, "The system does not contain any N:N relationships", "Warning", MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning);
                        else
                            SetListViewFilter(lvAssociations, items, txtAssFilter.Text);
                    }

                    ManageWorkingState(false);
                };
                bwFill.RunWorkerAsync();
            }
        }

        private void ExecuteAction(bool preview)
        {
            if (this.Service == null || targetService == null)
            {
                MessageBox.Show("You must select both a source and a target organization", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!(cbCreate.Checked || cbUpdate.Checked || cbDelete.Checked))
            {
                MessageBox.Show("You must select at least one setting for transporting the data", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check what transfer we should execute
            if (tabEntities.Equals(tabSourceObjects.SelectedTab))
            {
                TransferEntities(preview);
            } else if (tabAssociations.Equals(tabSourceObjects.SelectedTab))
            {
                TransferAssociations(preview);
            } else
            {
                MessageBox.Show("Unexpected error: no object type selected (Entity/Relationship)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TransferEntities(bool preview)
        {
            if (lvEntities.SelectedItems.Count == 0)
            {
                MessageBox.Show("You must select at least one entity to be transfered", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Good time to save the attributes
            SaveUnmarkedAttributes();

            if (!preview && cbDelete.Checked)
            {
                foreach (ListViewItem entityitem in lvEntities.SelectedItems)
                {
                    if (entityitem != null && entityitem.Tag != null)
                    {
                        var entity = (EntityMetadata)entityitem.Tag;

                        if (!string.IsNullOrEmpty(settings[organisationid][entity.LogicalName].Filter))
                        {
                            var msg = string.Format("You have a filter applied on \"{0}\" and checked the \"Delete\" flag. All records on the target environment which don't match the filtered soure set will be deleted! Are you sure you want to continue?", entity.LogicalName);
                            var result = MessageBox.Show(msg, "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (result.Equals(DialogResult.No))
                                return;
                        }
                    }
                }
            }

            ManageWorkingState(true);

            informationPanel = InformationPanel.GetInformationPanel(this, "Transfering records...", 340, 150);
            SendMessageToStatusBar?.Invoke(this, new StatusBarMessageEventArgs(0, "Start transfering records..."));

            var transfermode = EntityRecord.TransferMode.None;
            if (preview) transfermode |= EntityRecord.TransferMode.Preview;
            if (cbCreate.Checked) transfermode |= EntityRecord.TransferMode.Create;
            if (cbUpdate.Checked) transfermode |= EntityRecord.TransferMode.Update;
            if (cbDelete.Checked) transfermode |= EntityRecord.TransferMode.Delete;

            var bwTransferData = new BackgroundWorker { WorkerReportsProgress = true };
            bwTransferData.DoWork += (sender, e) =>
            {
                var worker = (BackgroundWorker)sender;
                var entities = (List<EntityMetadata>)e.Argument;
                var errors = new List<Item<string, string>>();
                var manualMappings = settings[organisationid].Mappings;
                var autoMappings = new List<Item<EntityReference, EntityReference>>();

                if (cbBusinessUnit.Checked)
                {
                    SendMessageToStatusBar?.Invoke(this, new StatusBarMessageEventArgs(1, "Retrieving root Business Units..."));
                    // Add BU mappings
                    var sourceBU = GetRootBusinessUnit(this.Service);
                    var targetBU = GetRootBusinessUnit(targetService);

                    if (sourceBU != null && targetBU != null)
                        autoMappings.Add(new Item<EntityReference, EntityReference>(sourceBU, targetBU));
                }

                if (cbTransactionCurrency.Checked)
                {
                    SendMessageToStatusBar?.Invoke(this, new StatusBarMessageEventArgs(1, "Retrieving default transaction currencies..."));
                    // Add BU mappings
                    var sourceTC = GetDefaultTransactionCurrency(this.Service);
                    var targetTC = GetDefaultTransactionCurrency(targetService);

                    if (sourceTC != null && targetTC != null)
                        autoMappings.Add(new Item<EntityReference, EntityReference>(sourceTC, targetTC));
                }

                if (cbSystemUserEntityReferences.Checked)
                {
                    SendMessageToStatusBar?.Invoke(this, new StatusBarMessageEventArgs(1, "Retrieving systemuser mappings..."));
                    // Add BU mappings
                    var sourceUsers = GetSystemUsers(this.Service);
                    var targetUsers = GetSystemUsers(targetService);

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
                }

                for (int i = 0; i < entities.Count; i++)
                {
                    var entitymeta = entities[i];
                    var attributes = lvAttributes.CheckedItems.Cast<ListViewItem>().Select(v => (AttributeMetadata)v.Tag).ToList();
                    var entity = new AppCode.EntityRecord(entitymeta, attributes, transfermode, this.Service, targetService);

                    worker.ReportProgress((i / entities.Count), string.Format("{1} entity '{0}'...", entity.Name, (preview ? "Previewing" : "Transfering")));

                    try
                    {
                        entity.Filter = settings[organisationid][entitymeta.LogicalName].Filter;
                        entity.Mappings = autoMappings;
                        entity.Mappings.AddRange(manualMappings);
                        entity.OnStatusMessage += Transfer_OnStatusMessage;
                        entity.Transfer();
                        errors.AddRange(entity.Messages.Select(m => new Item<string, string>(entity.Name, m)));

                        // Show preview window
                        if (preview)
                        {
                            var prvwDialog = new Preview(entity.PreviewList);
                            prvwDialog.ShowDialog(ParentForm);
                        }
                    }
                    catch (FaultException<OrganizationServiceFault> error)
                    {
                        errors.Add(new Item<string, string>(entity.Name, error.Message));
                    }
                }

                e.Result = errors;
            };
            bwTransferData.RunWorkerCompleted += (sender, e) =>
            {
                Controls.Remove(informationPanel);
                informationPanel.Dispose();
                //SendMessageToStatusBar(this, new StatusBarMessageEventArgs(string.Empty)); // keep showing transfer results afterwards
                ManageWorkingState(false);

                var errors = (List<Item<string, string>>)e.Result;

                if (errors.Count > 0)
                {
                    var errorDialog = new ErrorList((List<Item<string, string>>)e.Result);
                    errorDialog.ShowDialog(ParentForm);
                }
            };
            bwTransferData.ProgressChanged += (sender, e) =>
            {
                InformationPanel.ChangeInformationPanelMessage(informationPanel, e.UserState.ToString());
                SendMessageToStatusBar?.Invoke(this, new StatusBarMessageEventArgs(e.ProgressPercentage * 100, e.UserState.ToString()));
            };
            bwTransferData.RunWorkerAsync(lvEntities.SelectedItems.Cast<ListViewItem>().Select(v => (EntityMetadata)v.Tag).ToList());
        }

        private void TransferAssociations(bool preview)
        {
            if (lvAssociations.SelectedItems.Count == 0)
            {
                MessageBox.Show("You must select at least one relationship to be transfered", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ManageWorkingState(true);

            informationPanel = InformationPanel.GetInformationPanel(this, "Transfering records...", 340, 150);
            SendMessageToStatusBar?.Invoke(this, new StatusBarMessageEventArgs(0, "Start associating records..."));

            var transfermode = RelationRecord.TransferMode.None;
            if (preview) transfermode |= RelationRecord.TransferMode.Preview;
            if (cbCreate.Checked) transfermode |= RelationRecord.TransferMode.Create;
            if (cbDelete.Checked) transfermode |= RelationRecord.TransferMode.Delete;

            var bwTransferData = new BackgroundWorker { WorkerReportsProgress = true };
            bwTransferData.DoWork += (sender, e) =>
            {
                var worker = (BackgroundWorker)sender;
                var associations = (List<ManyToManyRelationshipMetadata>)e.Argument;
                var errors = new List<Item<string, string>>();

                for (int i = 0; i < associations.Count; i++)
                {
                    var entitymeta = associations[i];
                    var ass = new AppCode.RelationRecord(entitymeta, transfermode, this.Service, targetService);
                    
                    worker.ReportProgress((i / associations.Count), string.Format("{1} relation '{0}'...", ass.Name, (preview ? "Previewing" : "Transfering")));

                    try
                    {
                        ass.Mappings = settings[organisationid].Mappings;
                        ass.OnStatusMessage += Transfer_OnStatusMessage;
                        ass.Transfer();
                        errors.AddRange(ass.Messages.Select(m => new Item<string, string>(ass.Name, m)));
                    }
                    catch (FaultException<OrganizationServiceFault> error)
                    {
                        errors.Add(new Item<string, string>(ass.Name, error.Message));
                    }
                }

                e.Result = errors;
            };
            bwTransferData.RunWorkerCompleted += (sender, e) =>
            {
                Controls.Remove(informationPanel);
                informationPanel.Dispose();
                //SendMessageToStatusBar(this, new StatusBarMessageEventArgs(string.Empty)); // keep showing transfer results afterwards
                ManageWorkingState(false);

                var errors = (List<Item<string, string>>)e.Result;

                if (errors.Count > 0)
                {
                    var errorDialog = new ErrorList((List<Item<string, string>>)e.Result);
                    errorDialog.ShowDialog(ParentForm);
                }
            };
            bwTransferData.ProgressChanged += (sender, e) =>
            {
                InformationPanel.ChangeInformationPanelMessage(informationPanel, e.UserState.ToString());
                SendMessageToStatusBar?.Invoke(this, new StatusBarMessageEventArgs(e.ProgressPercentage * 100, e.UserState.ToString()));
            };
            bwTransferData.RunWorkerAsync(lvAssociations.SelectedItems.Cast<ListViewItem>().Select(v => (ManyToManyRelationshipMetadata)v.Tag).ToList());
        }

        private void Transfer_OnStatusMessage(object sender, EventArgs e)
        {
            SendMessageToStatusBar?.Invoke(this, new StatusBarMessageEventArgs(((StatusMessageEventArgs)e).Message));
        }

        private void SetListViewSorting(ListView listview, int column)
        {
            var setting = settings[organisationid].Sortcolumns.Where(s => s.Key == listview.Name).FirstOrDefault();
            if (setting == null)
            {
                setting = new Item<string, int>(listview.Name, -1);
                settings[organisationid].Sortcolumns.Add(setting);
            }

            if (setting.Value != column)
            {
                setting.Value = column;
                listview.Sorting = SortOrder.Ascending;
            }
            else
            {
                if (listview.Sorting == SortOrder.Ascending)
                    listview.Sorting = SortOrder.Descending;
                else
                    listview.Sorting = SortOrder.Ascending;
            }

            listview.ListViewItemSorter = new ListViewItemComparer(column, listview.Sorting);
        }

        private void SetListViewFilter(ListView listview, List<ListViewItem> items, string filter)
        {
            workingstate = true;

            listview.Items.Clear(); // clear list items before adding 
            // filter the items match with search key and add result to list view 
            listview.Items.AddRange(items.Where(i => string.IsNullOrEmpty(filter) || ContainsText(i, filter)).ToArray());

            workingstate = false;
        }

        private bool ContainsText(ListViewItem item, string text)
        {
            if (string.IsNullOrEmpty(text))
                return true;

            // Check everything lowercase
            text = text.ToLower();

            // Check item text
            if (item.Text.ToLower().Contains(text))
                return true;

            // Check subitems text
            foreach (ListViewItem.ListViewSubItem sitem in item.SubItems)
                if (sitem.Text.ToLower().Contains(text))
                    return true;

            // No matches found
            return false;
        }

        private void SaveUnmarkedAttributes()
        {
            if (lvEntities.SelectedItems.Count > 0)
            {
                var entityitem = lvEntities.SelectedItems[0];

                if (entityitem != null && entityitem.Tag != null)
                {
                    var entity = (EntityMetadata)entityitem.Tag;
                    var attributes = lvAttributes.Items.Cast<ListViewItem>().Where(i => !i.Checked).Select(v => (AttributeMetadata)v.Tag).Select(a => a.LogicalName).ToList();
                    settings[organisationid][entity.LogicalName].UnmarkedAttributes = attributes;
                }
            }
        }

        private EntityReference GetRootBusinessUnit(IOrganizationService service)
        {
            var qry = new Microsoft.Xrm.Sdk.Query.QueryExpression("businessunit");
            qry.Criteria.AddCondition("parentbusinessunitid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Null);
            var results = service.RetrieveMultiple(qry);

            if (results != null && results.Entities.Count > 0)
                return results.Entities[0].ToEntityReference();

            return null;
        }

        private EntityReference GetDefaultTransactionCurrency(IOrganizationService service)
        {
            var qry = new Microsoft.Xrm.Sdk.Query.QueryExpression("transactioncurrency");
            var results = service.RetrieveMultiple(qry);

            if (results != null && results.Entities.Count > 0)
                return results.Entities[0].ToEntityReference();

            return null;
        }

        private Entity[] GetSystemUsers(IOrganizationService service)
        {
            var qry = new Microsoft.Xrm.Sdk.Query.QueryExpression("systemuser");
            qry.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("domainname");
            var results = service.RetrieveMultiple(qry);

            return results.Entities.ToArray<Entity>();
        }

        private void OpenDonationPage(string currency)
        {
            var url = string.Format("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business={0}&lc=GB&item_name={1}&currency_code={2}&no_note=0&bn=PP-DonationsBF:btn_donateCC_LG.gif:NonHostedGuest", EmailAccount, DonationDescription, currency);
            System.Diagnostics.Process.Start(url);
        }

        #endregion Methods

    }
}