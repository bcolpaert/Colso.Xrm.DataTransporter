using Colso.DataTransporter.AppCode;
using Colso.DataTransporter.Forms;
using Colso.Xrm.DataTransporter.AppCode;
using Colso.Xrm.DataTransporter.Models;
using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Windows.Forms;
using System.Xml.Serialization;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Args;
using XrmToolBox.Extensibility.Interfaces;
using EntitySetting = Colso.Xrm.DataTransporter.Models.EntitySetting;

namespace Colso.DataTransporter
{
    public partial class DataTransporter : MultipleConnectionsPluginControlBase, IXrmToolBoxPluginControl, IGitHubPlugin, IHelpPlugin, IStatusBarMessenger, IPayPalPlugin
    {
        #region Variables

        private List<ListViewItem> Associations = new List<ListViewItem>();

        private Playlist currentplaylist;

        // keep list of listview items
        private List<ListViewItem> Entities = new List<ListViewItem>();

        private Guid organisationid;

        private Settings settings;

        /// <summary>
        /// Dynamics CRM 2011 target organization service
        /// </summary>
        private IOrganizationService targetService;

        private bool workingstate;

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

        public string DonationDescription => "Donation for Data Transporter Tool - XrmToolBox";

        public string EmailAccount => "bramcolpaert@outlook.com";

        public string HelpUrl => "https://github.com/MscrmTools/Colso.Xrm.DataTransporter/wiki";

        public string RepositoryName => "Colso.Xrm.DataTransporter";

        public string UserName => "MscrmTools";

        #endregion XrmToolbox

        #region Form events

        public override void ClosingPlugin(PluginCloseInfo info)
        {
            // First save settings file
            SettingFileHandler.SaveConfigData(settings);

            base.ClosingPlugin(info);
        }

        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail,
                    string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (actionName != "AdditionalOrganization")
            {
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

        private void btnFilter_Click(object sender, EventArgs e)
        {
            if (lvEntities.SelectedItems.Count > 0)
            {
                var entityitem = lvEntities.SelectedItems[0];

                if (entityitem != null && entityitem.Tag != null)
                {
                    var filter = string.Empty;
                    var entity = (EntityMetadata)entityitem.Tag;
                    var filterDialog = new FilterEditor(filter = settings[entity.LogicalName].Filter);
                    filterDialog.ShowDialog(ParentForm);
                    settings[entity.LogicalName].Filter = filterDialog.Filter;
                    InitFilter();
                }
            }
        }

        private void btnLoadSettings_Click(object sender, EventArgs e)
        {
            if (lvEntities.SelectedItems.Count > 0)
            {
                var entityitem = lvEntities.SelectedItems[0];

                if (entityitem != null && entityitem.Tag != null)
                {
                    var entity = (EntityMetadata)entityitem.Tag;
                    using (var dlg = new OpenFileDialog())
                    {
                        dlg.Filter = "xml files (*.xml)|*.xml";
                        dlg.FilterIndex = 2;
                        dlg.RestoreDirectory = true;

                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            using (var myFileStream = new System.IO.FileStream(dlg.FileName, System.IO.FileMode.Open))
                            {
                                var mySerializer = new XmlSerializer(typeof(EntitySetting));

                                try
                                {
                                    var es = (EntitySetting)mySerializer.Deserialize(myFileStream);
                                    // Reset settings
                                    settings[entity.LogicalName].Filter = es.Filter;
                                    settings[entity.LogicalName].UnmarkedAttributes = es.UnmarkedAttributes;
                                    settings[organisationid].Mappings = es.Mappings;
                                }
                                catch (Exception ex)
                                {
                                    // Error deserializing
                                    MessageBox.Show(ex.Message, "Error opening Entity Setting", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }

                                // Init new settings
                                InitMappings();
                                InitFilter();
                                PopulateAttributes();
                            }
                        }
                    }
                }
            }
        }

        private void btnMappings_Click(object sender, EventArgs e)
        {
            var entities = Entities.ToArray();
            var mappingDialog = new MappingList(entities, settings[organisationid].Mappings);
            mappingDialog.ShowDialog(ParentForm);
            settings[organisationid].Mappings = mappingDialog.GetMappingList();
            InitMappings();
        }

        private void btnPreviewTransfer_Click(object sender, EventArgs e)
        {
            ExecuteAction(true);
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            if (lvEntities.SelectedItems.Count > 0)
            {
                var entityitem = lvEntities.SelectedItems[0];

                if (entityitem != null && entityitem.Tag != null)
                {
                    var entity = (EntityMetadata)entityitem.Tag;
                    using (var dlg = new SaveFileDialog())
                    {
                        dlg.FileName = string.Concat(entity.LogicalName, ".xml");
                        dlg.Filter = "xml files (*.xml)|*.xml";
                        dlg.FilterIndex = 2;
                        dlg.RestoreDirectory = true;

                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            // Good time to save the attributes
                            SaveUnmarkedAttributes();

                            // Save filter + selected attributes + mappings (org independant)
                            var es = new EntitySetting()
                            {
                                LogicalName = entity.LogicalName,
                                Filter = settings[entity.LogicalName].Filter,
                                UnmarkedAttributes = settings[entity.LogicalName].UnmarkedAttributes,
                                Mappings = settings[organisationid].Mappings
                            };

                            System.IO.Stream myStream;
                            if ((myStream = dlg.OpenFile()) != null)
                            {
                                try
                                {
                                    var SerializerObj = new XmlSerializer(typeof(EntitySetting));
                                    SerializerObj.Serialize(myStream, es);
                                }
                                finally
                                {
                                    myStream.Close();
                                }
                            }
                        }
                    }
                }
            }
        }

        private void btnSelectTarget_Click(object sender, EventArgs e)
        {
            AddAdditionalOrganization();
        }

        private void chkAllAttributes_CheckedChanged(object sender, EventArgs e)
        {
            lvAttributes.Items.OfType<ListViewItem>().ToList().ForEach(item => item.Checked = chkAllAttributes.Checked);
        }

        private void donateInEURToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenDonationPage("EUR");
        }

        private void donateInGBPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenDonationPage("GBP");
        }

        private void donateInUSDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenDonationPage("USD");
        }

        private void lvAssociations_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            SetListViewSorting(lvAssociations, e.Column);
        }

        private void lvAssociations_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void lvAttributes_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            SetListViewSorting(lvAttributes, e.Column);
        }

        private void lvEntities_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            SetListViewSorting(lvEntities, e.Column);
        }

        private void lvEntities_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateAttributes();
        }

        private void tabSourceObjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Associations.Count == 0)
            {
                // Initial load
                ExecuteMethod(PopulateAssociations);
            }
        }

        private void tsbPlaylist_Click(object sender, EventArgs e)
        {
            if (currentplaylist == null) currentplaylist = new Playlist();
            var mappingDialog = new PlaylistDialog(this.Service, this.targetService, currentplaylist, chkUseBulk.Checked, nudBulkCount.Value);
            mappingDialog.ShowDialog(ParentForm);
            currentplaylist = mappingDialog.Playlist;
        }

        private void tsbRefreshAssociations_Click(object sender, EventArgs e)
        {
            ExecuteMethod(PopulateAssociations);
        }

        private void tsbRefreshEntities_Click(object sender, EventArgs e)
        {
            ExecuteMethod(PopulateEntities);
        }

        private void tsbTransferData_Click(object sender, EventArgs e)
        {
            ExecuteAction(false);
        }

        private void txtAssFilter_TextChanged(object sender, EventArgs e)
        {
            SetListViewFilter(lvAssociations, Associations, txtAssFilter.Text);
        }

        private void txtEntityFilter_TextChanged(object sender, EventArgs e)
        {
            SetListViewFilter(lvEntities, Entities, txtEntityFilter.Text);
        }

        #endregion Form events

        #region Methods

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
            }
            else if (tabAssociations.Equals(tabSourceObjects.SelectedTab))
            {
                TransferAssociations(preview);
            }
            else
            {
                MessageBox.Show("Unexpected error: no object type selected (Entity/Relationship)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                    filter = settings[entity.LogicalName].Filter;
                }
            }

            btnFilter.ForeColor = string.IsNullOrEmpty(filter) ? Color.Black : Color.Blue;
        }

        private void InitMappings()
        {
            btnEntityMappings.ForeColor = settings[organisationid].Mappings.Count == 0 ? Color.Black : Color.Blue;
            btnAssMappings.ForeColor = settings[organisationid].Mappings.Count == 0 ? Color.Black : Color.Blue;
        }

        private void ManageWorkingState(bool working)
        {
            workingstate = working;
            tabSourceObjects.Enabled = !working;
            Cursor = working ? Cursors.WaitCursor : Cursors.Default;
            tsbCancel.Visible = working;
        }

        private void OpenDonationPage(string currency)
        {
            var url = string.Format("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business={0}&lc=GB&item_name={1}&currency_code={2}&no_note=0&bn=PP-DonationsBF:btn_donateCC_LG.gif:NonHostedGuest", EmailAccount, DonationDescription, currency);
            System.Diagnostics.Process.Start(url);
        }

        private void PopulateAssociations()
        {
            if (!workingstate)
            {
                // Reinit other controls
                ManageWorkingState(true);

                WorkAsync(new WorkAsyncInfo
                {
                    Message = "Loading associations...",
                    Work = (worker, e) =>
                    {
                        // Retrieve
                        List<ManyToManyRelationshipMetadata> sourceList = this.Service.RetrieveAssociations();

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
                    },
                    PostWorkCallBack = e =>
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
                                MessageBox.Show(this, "The system does not contain any N:N relationships", "Warning", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                            else
                                SetListViewFilter(lvAssociations, items, txtAssFilter.Text);
                        }

                        ManageWorkingState(false);
                    }
                });
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

                        WorkAsync(new WorkAsyncInfo
                        {
                            Work = (worker, e) =>
                            {
                                // Retrieve
                                var entitymeta = this.Service.RetrieveEntity(entity.LogicalName);
                                entityitem.Tag = entitymeta;

                                // Get attribute checked settings
                                var unmarkedattributes = settings[entity.LogicalName].UnmarkedAttributes;

                                // Prepare list of items
                                var sourceAttributesList = new List<ListViewItem>();

                                // Only use create/editable attributes && properties which are valid for read and have a display name
                                var attributes = entitymeta.Attributes
                                    .Where(a => (a.IsValidForCreate != null && a.IsValidForCreate.Value))// || (a.IsValidForUpdate != null && a.IsValidForUpdate.Value))
                                    .Where(a => a.IsValidForRead != null && a.IsValidForRead.Value)
                                    .Where(a => a.DisplayName != null && a.DisplayName.UserLocalizedLabel != null && !string.IsNullOrEmpty(a.DisplayName.UserLocalizedLabel.Label))
                                    .ToList();

                                // This is not necessary
                                //if(attributes.FirstOrDefault(x=> x.LogicalName == "statecode") == null)
                                //{
                                //    attributes.Add(entitymeta.Attributes.FirstOrDefault(x => x.LogicalName == "statecode"));
                                //}

                                foreach (AttributeMetadata attribute in attributes)
                                {
                                    if (attribute == null)
                                        continue;

                                    var name = attribute.DisplayName.UserLocalizedLabel == null ? string.Empty : attribute.DisplayName.UserLocalizedLabel.Label;
                                    var typename = attribute.AttributeTypeName == null ? string.Empty : attribute.AttributeTypeName.Value;

                                    if (typename != "EntityName")
                                    {
                                        var item = new ListViewItem(name) { Tag = attribute };
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
                            },
                            PostWorkCallBack = e =>
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
                            }
                        });
                    }
                }
            }
        }

        private void PopulateEntities()
        {
            if (!workingstate)
            {
                // Reinit other controls
                lvAttributes.Items.Clear();
                ManageWorkingState(true);

                WorkAsync(new WorkAsyncInfo
                {
                    Message = "Loading entities...",
                    Work = (worker, e) =>
                    {
                        // Retrieve
                        List<EntityMetadata> sourceList = this.Service.RetrieveEntities();

                        // Prepare list of items
                        Entities.Clear();

                        foreach (EntityMetadata entity in sourceList)
                        {
                            var name = entity.DisplayName.UserLocalizedLabel == null ? string.Empty : entity.DisplayName.UserLocalizedLabel.Label;
                            var item = new ListViewItem(name)
                            {
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
                    },
                    PostWorkCallBack = e =>
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
                                MessageBox.Show(this, "The system does not contain any entities", "Warning", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                            else
                                SetListViewFilter(lvEntities, items, txtEntityFilter.Text);
                        }

                        ManageWorkingState(false);
                    }
                });
            }
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
                    settings[entity.LogicalName].UnmarkedAttributes = attributes;
                }
            }
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

        private void SetListViewFilter(ListView listview, List<ListViewItem> items, string filter)
        {
            workingstate = true;

            listview.Items.Clear(); // clear list items before adding
            // filter the items match with search key and add result to list view
            listview.Items.AddRange(items.Where(i => string.IsNullOrEmpty(filter) || ContainsText(i, filter)).ToArray());

            workingstate = false;
        }

        private void SetListViewSorting(ListView listview, int column)
        {
            var setting = settings.Sortcolumns.FirstOrDefault(s => s.Key == listview.Name);
            if (setting == null)
            {
                setting = new Item<string, int>(listview.Name, -1);
                settings.Sortcolumns.Add(setting);
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

        private void Transfer_OnStatusMessage(object sender, EventArgs e)
        {
            SendMessageToStatusBar?.Invoke(this, new StatusBarMessageEventArgs(((StatusMessageEventArgs)e).Message));
        }

        private void TransferAssociations(bool preview)
        {
            if (lvAssociations.SelectedItems.Count == 0)
            {
                MessageBox.Show("You must select at least one relationship to be transfered", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ManageWorkingState(true);

            SendMessageToStatusBar?.Invoke(this, new StatusBarMessageEventArgs(0, "Start associating records..."));

            var transfermode = Enumerations.TransferMode.None;
            if (preview) transfermode |= Enumerations.TransferMode.Preview;
            if (cbCreate.Checked) transfermode |= Enumerations.TransferMode.Create;
            if (cbDelete.Checked) transfermode |= Enumerations.TransferMode.Delete;

            var useBulk = chkUseBulk.Checked;
            var bulkCount = Convert.ToInt32(nudBulkCount.Value);

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Transfering records",
                AsyncArgument = lvAssociations.SelectedItems.Cast<ListViewItem>().Select(v => (ManyToManyRelationshipMetadata)v.Tag).ToList(),
                IsCancelable = true,
                Work = (worker, evt) =>
                {
                    var associations = (List<ManyToManyRelationshipMetadata>)evt.Argument;
                    var errors = new List<Item<string, string>>();

                    for (int i = 0; i < associations.Count; i++)
                    {
                        var entitymeta = associations[i];
                        var ass = new RelationRecord(entitymeta, transfermode, worker, Service, targetService);

                        worker.ReportProgress((i / associations.Count), string.Format("{1} relation '{0}'...", ass.Name, (preview ? "Previewing" : "Transfering")));

                        try
                        {
                            ass.Mappings = settings[organisationid].Mappings;
                            ass.OnStatusMessage += Transfer_OnStatusMessage;
                            ass.Transfer(useBulk, bulkCount);
                            errors.AddRange(ass.Messages.Select(m => new Item<string, string>(ass.Name, m)));
                        }
                        catch (FaultException<OrganizationServiceFault> error)
                        {
                            errors.Add(new Item<string, string>(ass.Name, error.Message));
                        }
                    }

                    evt.Result = errors;
                },
                PostWorkCallBack = evt =>
                {
                    //SendMessageToStatusBar(this, new StatusBarMessageEventArgs(string.Empty)); // keep showing transfer results afterwards
                    ManageWorkingState(false);

                    var errors = (List<Item<string, string>>)evt.Result;

                    if (errors.Count > 0)
                    {
                        var errorDialog = new ErrorList((List<Item<string, string>>)evt.Result);
                        errorDialog.ShowDialog(ParentForm);
                    }
                },
                ProgressChanged = evt =>
                {
                    SetWorkingMessage(evt.UserState.ToString());
                    SendMessageToStatusBar?.Invoke(this, new StatusBarMessageEventArgs(evt.ProgressPercentage * 100, evt.UserState.ToString()));
                }
            });
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

                        if (!string.IsNullOrEmpty(settings[entity.LogicalName].Filter))
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

            SendMessageToStatusBar?.Invoke(this, new StatusBarMessageEventArgs(0, "Start transfering records..."));

            var transfermode = Enumerations.TransferMode.None;
            if (preview) transfermode |= Enumerations.TransferMode.Preview;
            if (cbCreate.Checked) transfermode |= Enumerations.TransferMode.Create;
            if (cbUpdate.Checked) transfermode |= Enumerations.TransferMode.Update;
            if (cbDelete.Checked) transfermode |= Enumerations.TransferMode.Delete;

            bool useBulk = chkUseBulk.Checked;
            int bulkCount = Convert.ToInt32(nudBulkCount.Value);
            var attributes = lvAttributes.CheckedItems.Cast<ListViewItem>().Select(v => (AttributeMetadata)v.Tag).ToList();

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Transfering records...",
                AsyncArgument = lvEntities.SelectedItems.Cast<ListViewItem>().Select(v => (EntityMetadata)v.Tag).ToList(),
                IsCancelable = true,
                Work = (worker, evt) =>
                {
                    var entities = (List<EntityMetadata>)evt.Argument;
                    var errors = new List<Item<string, string>>();
                    var manualMappings = settings[organisationid].Mappings;
                    var autoMappings = new List<Item<EntityReference, EntityReference>>();

                    if (cbBusinessUnit.Checked)
                    {
                        SendMessageToStatusBar?.Invoke(this, new StatusBarMessageEventArgs(1, "Retrieving root Business Units..."));
                        var bumapping = AutoMappings.GetRootBusinessUnitMapping(this.Service, targetService);
                        if (bumapping != null) autoMappings.Add(bumapping);
                    }

                    if (cbTransactionCurrency.Checked)
                    {
                        SendMessageToStatusBar?.Invoke(this, new StatusBarMessageEventArgs(1, "Retrieving default transaction currencies..."));
                        var tcmapping = AutoMappings.GetDefaultTransactionCurrencyMapping(this.Service, targetService);
                        if (tcmapping != null) autoMappings.Add(tcmapping);
                    }

                    if (cbSystemUserEntityReferences.Checked)
                    {
                        SendMessageToStatusBar?.Invoke(this, new StatusBarMessageEventArgs(1, "Retrieving systemuser mappings..."));
                        var sumapping = AutoMappings.GetSystemUsersMapping(this.Service, targetService);
                        if (sumapping != null) autoMappings.AddRange(sumapping);
                    }

                    for (int i = 0; i < entities.Count; i++)
                    {
                        var entitymeta = entities[i];
                        var entity = new AppCode.EntityRecord(entitymeta, attributes, transfermode, worker, this.Service, targetService);

                        worker.ReportProgress((i / entities.Count), string.Format("{1} entity '{0}'...", entity.Name, (preview ? "Previewing" : "Transfering")));

                        try
                        {
                            entity.Filter = settings[entitymeta.LogicalName].Filter;
                            entity.Mappings = autoMappings;
                            entity.Mappings.AddRange(manualMappings);
                            entity.OnStatusMessage += Transfer_OnStatusMessage;
                            entity.Transfer(useBulk, bulkCount);
                            errors.AddRange(entity.Messages.Select(m => new Item<string, string>(entity.Name, m)));

                            // Show preview window
                            if (preview)
                            {
                                Invoke(new Action(() =>
                                {
                                    var prvwDialog = new Preview(entity.PreviewList);
                                    prvwDialog.ShowDialog(ParentForm);
                                }));
                            }
                        }
                        catch (FaultException<OrganizationServiceFault> error)
                        {
                            errors.Add(new Item<string, string>(entity.Name, error.Message));
                        }
                    }

                    evt.Result = errors;
                },
                PostWorkCallBack = evt =>
                {
                    tsbCancel.Text = @"Cancel";
                    //SendMessageToStatusBar(this, new StatusBarMessageEventArgs(string.Empty)); // keep showing transfer results afterwards
                    ManageWorkingState(false);

                    var errors = (List<Item<string, string>>)evt.Result;

                    if (errors.Count > 0)
                    {
                        var errorDialog = new ErrorList((List<Item<string, string>>)evt.Result);
                        errorDialog.ShowDialog(ParentForm);
                    }
                },
                ProgressChanged = evt =>
                {
                    SetWorkingMessage(evt.UserState.ToString());
                    SendMessageToStatusBar?.Invoke(this, new StatusBarMessageEventArgs(evt.ProgressPercentage * 100, evt.UserState.ToString()));
                }
            });
        }

        #endregion Methods

        private void chkUseBulk_CheckedChanged(object sender, EventArgs e)
        {
            nudBulkCount.Enabled = chkUseBulk.Checked;
        }

        private void tsbCancel_Click(object sender, EventArgs e)
        {
            CancelWorker();
            tsbCancel.Text = @"Cancelling...";
        }
    }
}