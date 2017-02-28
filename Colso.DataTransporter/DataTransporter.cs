using Colso.DataTransporter.AppCode;
using Colso.DataTransporter.Forms;
using McTools.Xrm.Connection;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
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
    public partial class DataTransporter : UserControl, IXrmToolBoxPluginControl, IGitHubPlugin, IHelpPlugin, IStatusBarMessenger
    {
        #region Variables

        /// <summary>
        /// Information panel
        /// </summary>
        private Panel informationPanel;

        /// <summary>
        /// Dynamics CRM 2011 organization service
        /// </summary>
        private IOrganizationService service;

        /// <summary>
        /// Dynamics CRM 2011 target organization service
        /// </summary>
        private IOrganizationService targetService;

        private bool workingstate = false;
        private Guid organisationid;
        private Settings settings;

        #endregion Variables

        public DataTransporter()
        {
            SettingFileHandler.GetConfigData(out settings);
            InitializeComponent();
        }

        #region XrmToolbox

        public event EventHandler OnCloseTool;
        public event EventHandler OnRequestConnection;
        public event EventHandler<StatusBarMessageEventArgs> SendMessageToStatusBar;

        public Image PluginLogo
        {
            get { return null; }
        }

        public IOrganizationService Service
        {
            get { throw new NotImplementedException(); }
        }

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

        public void ClosingPlugin(PluginCloseInfo info)
        {
            // First save settings file
            SettingFileHandler.SaveConfigData(settings);

            if (info.FormReason != CloseReason.None ||
                info.ToolBoxReason == ToolBoxCloseReason.CloseAll ||
                info.ToolBoxReason == ToolBoxCloseReason.CloseAllExceptActive)
            {
                return;
            }

            info.Cancel = MessageBox.Show(@"Are you sure you want to close this tab?", @"Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes;
        }

        public void UpdateConnection(IOrganizationService newService, ConnectionDetail connectionDetail, string actionName = "", object parameter = null)
        {
            if (actionName == "TargetOrganization")
            {
                SetConnectionLabel(connectionDetail.ConnectionName, "Target");
                targetService = newService;
            }
            else
            {
                organisationid = connectionDetail.ConnectionId.Value;
                SetConnectionLabel(connectionDetail.ConnectionName, "Source");
                service = newService;
                // init buttons -> value based on connection
                InitMappings();
                InitFilter();
                // Save settings file
                SettingFileHandler.SaveConfigData(settings);
                // Load entities when source connection changes
                PopulateEntities();
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
            if (OnRequestConnection != null)
            {
                var args = new RequestConnectionEventArgs { ActionName = "TargetOrganization", Control = this };
                OnRequestConnection(this, args);
            }
        }

        private void tsbCloseThisTab_Click(object sender, EventArgs e)
        {
            if (OnCloseTool != null)
                OnCloseTool(this, null);
        }

        private void tsbRefreshEntities_Click(object sender, EventArgs e)
        {
            PopulateEntities();
        }

        private void tsbTransferData_Click(object sender, EventArgs e)
        {
            Transfer();
        }

        private void lvEntities_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateAttributes();
        }

        private void lvEntities_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            SetListViewSorting(lvEntities, e.Column);
        }

        private void lvAttributes_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            SetListViewSorting(lvAttributes, e.Column);
        }

        private void chkAllAttributes_CheckedChanged(object sender, EventArgs e)
        {
            lvAttributes.Items.OfType<ListViewItem>().ToList().ForEach(item => item.Checked = chkAllAttributes.Checked);
        }

        private void btnMappings_Click(object sender, EventArgs e)
        {
            var entities = lvEntities.Items.Cast<ListViewItem>().ToArray();
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

        #endregion Form events

        #region Methods

        private void InitMappings()
        {
            btnMappings.ForeColor = settings[organisationid].Mappings.Count == 0 ? Color.Black : Color.Blue;
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

        private void SetConnectionLabel(string name, string serviceType)
        {
            switch (serviceType)
            {
                case "Source":
                    lbSourceValue.Text = name;
                    lbSourceValue.ForeColor = Color.Green;
                    break;

                case "Target":
                    lbTargetValue.Text = name;
                    lbTargetValue.ForeColor = Color.Green;
                    break;
            }
        }

        private void ManageWorkingState(bool working)
        {
            workingstate = working;
            gbEntities.Enabled = !working;
            gbAttributes.Enabled = !working;
            Cursor = working ? Cursors.WaitCursor : Cursors.Default;
        }

        private bool CheckConnection()
        {
            if (service == null)
            {
                if (OnRequestConnection != null)
                {
                    var args = new RequestConnectionEventArgs
                    {
                        ActionName = "Load",
                        Control = this
                    };
                    OnRequestConnection(this, args);
                }
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
                lvEntities.Items.Clear();
                lvAttributes.Items.Clear();
                ManageWorkingState(true);

                informationPanel = InformationPanel.GetInformationPanel(this, "Loading entities...", 340, 150);

                // Launch treatment
                var bwFill = new BackgroundWorker();
                bwFill.DoWork += (sender, e) =>
                {
                    // Retrieve 
                    List<EntityMetadata> sourceList = MetadataHelper.RetrieveEntities(service);

                    // Prepare list of items
                    var sourceEntitiesList = new List<ListViewItem>();

                    foreach (EntityMetadata entity in sourceList)
                    {
                        var name = entity.DisplayName.UserLocalizedLabel == null ? string.Empty : entity.DisplayName.UserLocalizedLabel.Label;
                        var item = new ListViewItem(name);
                        item.Tag = entity;
                        item.SubItems.Add(entity.LogicalName);

                        if (!entity.IsCustomizable.Value)
                        {
                            item.ForeColor = Color.Gray;
                            item.ToolTipText = "This entity has not been defined as customizable";
                        }

                        sourceEntitiesList.Add(item);
                    }

                    e.Result = sourceEntitiesList;
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
                        {
                            MessageBox.Show(this, "The system does not contain any entities", "Warning", MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning);
                        }
                        else
                        {
                            lvEntities.Items.AddRange(items.ToArray());
                        }
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
                            var entitymeta = MetadataHelper.RetrieveEntity(entity.LogicalName, service);

                            // Get attribute checked settings
                            var unmarkedattributes = settings[organisationid][entity.LogicalName].UnmarkedAttributes;

                            // Prepare list of items
                            var sourceAttributesList = new List<ListViewItem>();

                            // Only use create/editable attributes && properties which are valid for read
                            var attributes = entitymeta.Attributes
                                .Where(a => (a.IsValidForCreate != null && a.IsValidForCreate.Value) || (a.IsValidForUpdate != null && a.IsValidForUpdate.Value))
                                .Where(a => a.IsValidForRead != null && a.IsValidForRead.Value)
                                .ToArray();

                            foreach (AttributeMetadata attribute in attributes)
                            {
                                // Skip "statecode", "statuscode" (should be updated via SetStateRequest)
                                if (!attribute.LogicalName.Equals("statecode")
                                && !attribute.LogicalName.Equals("statuscode"))
                                {
                                    var name = attribute.DisplayName.UserLocalizedLabel == null ? string.Empty : attribute.DisplayName.UserLocalizedLabel.Label;
                                    var typename = attribute.AttributeTypeName == null ? string.Empty : attribute.AttributeTypeName.Value;
                                    var item = new ListViewItem(name);
                                    item.Tag = attribute;
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

        private void Transfer()
        {
            // Good time to save the attributes
            SaveUnmarkedAttributes();

            if (service == null || targetService == null)
            {
                MessageBox.Show("You must select both a source and a target organization", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (lvEntities.SelectedItems.Count == 0)
            {
                MessageBox.Show("You must select at least one entity to be transfered", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!(cbCreate.Checked || cbUpdate.Checked || cbDelete.Checked))
            {
                MessageBox.Show("You must select at least one setting for transporting the data", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cbDelete.Checked)
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
            SendMessageToStatusBar(this, new StatusBarMessageEventArgs("Start transfering records..."));

            var transfermode = EntityRecord.TransferMode.None;
            if (cbCreate.Checked) transfermode |= EntityRecord.TransferMode.Create;
            if (cbUpdate.Checked) transfermode |= EntityRecord.TransferMode.Update;
            if (cbDelete.Checked) transfermode |= EntityRecord.TransferMode.Delete;

            var bwTransferData = new BackgroundWorker { WorkerReportsProgress = true };
            bwTransferData.DoWork += (sender, e) =>
            {
                var worker = (BackgroundWorker)sender;
                var entities = (List<EntityMetadata>)e.Argument;
                var errors = new List<Item<string, string>>();

                for (int i = 0; i < entities.Count; i++)
                {
                    var entitymeta = entities[i];
                    var attributes = lvAttributes.CheckedItems.Cast<ListViewItem>().Select(v => (AttributeMetadata)v.Tag).ToList();
                    var entity = new AppCode.EntityRecord(entitymeta, attributes, transfermode, service, targetService);

                    worker.ReportProgress((i / entities.Count), string.Format("Transfering entity '{0}'...", entity.Name));

                    try
                    {
                        entity.Filter = settings[organisationid][entitymeta.LogicalName].Filter;
                        entity.Mappings = settings[organisationid].Mappings;
                        entity.OnStatusMessage += Entity_OnStatusMessage;
                        entity.Transfer();
                        errors.AddRange(entity.Messages.Select(m => new Item<string, string>(entity.Name, m)));
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
                SendMessageToStatusBar(this, new StatusBarMessageEventArgs(e.UserState.ToString()));
            };
            bwTransferData.RunWorkerAsync(lvEntities.SelectedItems.Cast<ListViewItem>().Select(v => (EntityMetadata)v.Tag).ToList());
        }

        private void Entity_OnStatusMessage(object sender, EventArgs e)
        {
            SendMessageToStatusBar(this, new StatusBarMessageEventArgs(((StatusMessageEventArgs)e).Message));
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

        #endregion Methods

    }
}