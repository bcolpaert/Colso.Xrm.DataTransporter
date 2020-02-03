using Colso.DataTransporter.AppCode;
using Colso.Xrm.DataTransporter.AppCode;
using Colso.Xrm.DataTransporter.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Windows.Forms;
using System.Xml.Serialization;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Args;
using static Colso.Xrm.DataTransporter.AppCode.Enumerations;

namespace Colso.DataTransporter.Forms
{
    public partial class PlaylistDialog : Form
    {
        protected Playlist list;
        public Playlist Playlist { get { return list;  } }

        public event EventHandler OnStatusMessage;

        private IOrganizationService sourceService;
        private IOrganizationService targetService;


        private int colIdxMoveDown;
        private int colIdxMoveUp;
        private int colIdxSequence;
        private int colIdxCreate;
        private int colIdxUpdate;
        private int colIdxDelete;

        private const string trueValue = "True";

        public PlaylistDialog(IOrganizationService sourceService, IOrganizationService targetService, Playlist list)
        {
            this.sourceService = sourceService;
            this.targetService = targetService;
            if (list.Items == null) list.Items = new List<PlaylistItem>();
            this.list = list;
            InitializeComponent();
            // Set col indexes
            colIdxMoveDown = dgvPlaylist.Columns["clMoveDown"].Index;
            colIdxMoveUp = dgvPlaylist.Columns["clMoveUp"].Index;
            colIdxCreate = dgvPlaylist.Columns["clCreate"].Index;
            colIdxUpdate = dgvPlaylist.Columns["clUpdate"].Index;
            colIdxDelete = dgvPlaylist.Columns["clDelete"].Index;
            colIdxSequence = dgvPlaylist.Columns["clSequence"].Index;
        }

        private void BtnCloseClick(object sender, EventArgs e)
        {
            Close();
        }

        private void PlaylistLoad(object sender, EventArgs e)
        {
            ReloadList();
        }
        
        private void btnAddEntitySetting_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "xml files (*.xml)|*.xml";
                dlg.FilterIndex = 2;
                dlg.RestoreDirectory = true;
                dlg.Multiselect = true;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    foreach (var filename in dlg.FileNames)
                    {
                        using (var myFileStream = new System.IO.FileStream(filename, System.IO.FileMode.Open))
                        {
                            var mySerializer = new XmlSerializer(typeof(EntitySetting));
                            var es = (EntitySetting)mySerializer.Deserialize(myFileStream);
                            var item = new PlaylistItem()
                            {
                                Sequence = dgvPlaylist.Rows.Count + 1,
                                Setting = es
                            };
                            list.Items.Add(item);
                            AddToList(item);
                        }
                    }
                }
            }
        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            if (sourceService == null || targetService == null)
                MessageBox.Show("You must select both a source and a target organization", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                ExecuteTransfer();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (var dlg = new SaveFileDialog())
            {
                dlg.Filter = "xml files (*.xml)|*.xml";
                dlg.FilterIndex = 2;
                dlg.RestoreDirectory = true;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    System.IO.Stream myStream;
                    if ((myStream = dlg.OpenFile()) != null)
                    {
                        try
                        {
                            var SerializerObj = new XmlSerializer(typeof(Playlist));
                            SerializerObj.Serialize(myStream, list);
                        }
                        finally
                        {
                            myStream.Close();
                        }
                    }
                }
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "xml files (*.xml)|*.xml";
                dlg.FilterIndex = 2;
                dlg.RestoreDirectory = true;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    using (var myFileStream = new System.IO.FileStream(dlg.FileName, System.IO.FileMode.Open))
                    {
                        var mySerializer = new XmlSerializer(typeof(Playlist));
                        try
                        {
                            list = (Playlist)mySerializer.Deserialize(myFileStream);
                        }
                        catch (Exception ex)
                        {
                            // Error deserializing
                            MessageBox.Show(ex.Message, "Error opening Playlist", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        // Reset settings
                        ReloadList();
                    }
                }
            }
        }
        
        private void dgvMappings_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var index = e.RowIndex;
            bool moveDown = e.ColumnIndex == colIdxMoveDown;
            bool moveUp = e.ColumnIndex == colIdxMoveUp;

            // Ignore clicks that are not on button cells. 
            if ((moveDown || moveUp))
            {
                var movedirection = (moveDown) ? 1 : -1;
                // Ignore moves that are out of bounds. 
                if (index < 0 || (index + movedirection) < 0 || index >= this.list.Items.Count || (index + movedirection) >= this.list.Items.Count)
                    return;

                MoveItem(index, (index + movedirection));
            }
        }

        private void dgvPlaylist_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var index = e.RowIndex;
            bool create = e.ColumnIndex == colIdxCreate;
            bool update = e.ColumnIndex == colIdxUpdate;
            bool delete = e.ColumnIndex == colIdxDelete;

            if (create || update || delete)
            {
                var doCreate = trueValue.Equals(dgvPlaylist[colIdxCreate, index].Value?.ToString());
                var doUpdate = trueValue.Equals(dgvPlaylist[colIdxUpdate, index].Value?.ToString());
                var doDelete = trueValue.Equals(dgvPlaylist[colIdxDelete, index].Value?.ToString());
                var actions = Enumerations.TransferMode.None;
                if (doCreate) actions = (actions | Enumerations.TransferMode.Create);
                if (doUpdate) actions = (actions | Enumerations.TransferMode.Update);
                if (doDelete) actions = (actions | Enumerations.TransferMode.Delete);
                this.list.Items[index].Actions = actions;
            }
        }

        private void MoveItem(int index, int newIndex)
        {

            // Change index in data source
            this.list.Items[index].Sequence = newIndex + 1;
            this.list.Items[newIndex].Sequence = index + 1;
            this.list.Items = this.list.Items.OrderBy(i => i.Sequence).ToList();

            // Change index in view
            dgvPlaylist[colIdxSequence, index].Value = newIndex + 1;
            dgvPlaylist[colIdxSequence, newIndex].Value = index + 1;
            var row = dgvPlaylist.Rows[index];
            dgvPlaylist.Rows.RemoveAt(index);
            dgvPlaylist.Rows.Insert(newIndex, row);
        }

        private void ReloadList()
        {
            dgvPlaylist.Rows.Clear();
            // Add mappings
            foreach (var item in list.Items)
                AddToList(item);
        }

        private void AddToList(PlaylistItem item)
        {
            var isCreate = ((item.Actions & Enumerations.TransferMode.Create) == Enumerations.TransferMode.Create);
            var isUpdate = ((item.Actions & Enumerations.TransferMode.Update) == Enumerations.TransferMode.Update);
            var isDelete = ((item.Actions & Enumerations.TransferMode.Delete) == Enumerations.TransferMode.Delete);
            var vals = new object[7] { null, null, item.Sequence, item.Setting.LogicalName, isCreate.ToString(), isUpdate.ToString(), isDelete.ToString() };
            dgvPlaylist.Rows.Add(vals);
        }

        private void ExecuteTransfer()
        {
            var informationPanel = InformationPanel.GetInformationPanel(this, "Starting playlist transfer...", 340, 150);
            var bwTransferData = new BackgroundWorker { WorkerReportsProgress = true };
            bwTransferData.DoWork += (sender, e) =>
            {
                var worker = (BackgroundWorker)sender;
                var errors = new List<Item<string, string>>();
                var autoMappings = new List<Item<EntityReference, EntityReference>>();

                #region Auto-mappings

                OnStatusMessage?.Invoke(this, new StatusBarMessageEventArgs(1, "Retrieving root Business Units..."));
                // Add BU mappings
                var bumapping = AutoMappings.GetRootBusinessUnitMapping(sourceService, targetService);
                if (bumapping != null) autoMappings.Add(bumapping);

                OnStatusMessage?.Invoke(this, new StatusBarMessageEventArgs(1, "Retrieving default transaction currencies..."));
                var tcmapping = AutoMappings.GetDefaultTransactionCurrencyMapping(sourceService, targetService);
                if (tcmapping != null) autoMappings.Add(tcmapping);

                OnStatusMessage?.Invoke(this, new StatusBarMessageEventArgs(1, "Retrieving systemuser mappings..."));
                var sumapping = AutoMappings.GetSystemUsersMapping(sourceService, targetService);
                if (sumapping != null) autoMappings.AddRange(sumapping);

                #endregion

                for (int i = 0; i < list.Items.Count; i++)
                {
                    var item = list.Items[i];
                    var entitymeta = sourceService.RetrieveEntity(item.Setting.LogicalName);
                    var attributes = entitymeta.Attributes
                                .Where(a => (a.IsValidForCreate != null && a.IsValidForCreate.Value))// || (a.IsValidForUpdate != null && a.IsValidForUpdate.Value))
                                .Where(a => a.IsValidForRead != null && a.IsValidForRead.Value)
                                .Where(a => a.DisplayName != null && a.DisplayName.UserLocalizedLabel != null && !string.IsNullOrEmpty(a.DisplayName.UserLocalizedLabel.Label))
                                .Where(a => !item.Setting.UnmarkedAttributes.Any(ua => ua.Equals(a.LogicalName)))
                                .ToList();
                    var entity = new AppCode.EntityRecord(entitymeta, attributes, item.Actions, sourceService, targetService);

                    worker.ReportProgress((i / list.Items.Count), string.Format("Transfering entity '{0}'...", entity.Name));

                    try
                    {
                        entity.Filter = item.Setting.Filter;
                        entity.Mappings = autoMappings;
                        entity.Mappings.AddRange(item.Setting.Mappings);
                        entity.OnStatusMessage += OnStatusMessage;
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
                OnStatusMessage?.Invoke(this, new StatusBarMessageEventArgs(e.ProgressPercentage * 100, e.UserState.ToString()));
            };
            bwTransferData.RunWorkerAsync();
        }
    }
}