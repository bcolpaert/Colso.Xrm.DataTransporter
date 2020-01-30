using Colso.Xrm.DataTransporter.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using static Colso.Xrm.DataTransporter.AppCode.Enumerations;

namespace Colso.DataTransporter.Forms
{
    public partial class PlaylistDialog : Form
    {
        protected Playlist list;
        public Playlist Playlist { get { return list;  } }

        private int colIdxMoveDown;
        private int colIdxMoveUp;
        private int colIdxSequence;
        private int colIdxCreate;
        private int colIdxUpdate;
        private int colIdxDelete;

        private const string trueValue = "True";

        public PlaylistDialog(Playlist list)
        {
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
                                Sequence = dgvPlaylist.Rows.Count,
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
                TransferMode actions = TransferMode.None;
                if (doCreate) actions = (actions | TransferMode.Create);
                if (doUpdate) actions = (actions | TransferMode.Update);
                if (doDelete) actions = (actions | TransferMode.Delete);
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
            var isCreate = ((item.Actions & TransferMode.Create) == TransferMode.Create);
            var isUpdate = ((item.Actions & TransferMode.Update) == TransferMode.Update);
            var isDelete = ((item.Actions & TransferMode.Delete) == TransferMode.Delete);
            var vals = new object[7] { null, null, item.Sequence, item.Setting.LogicalName, isCreate.ToString(), isUpdate.ToString(), isDelete.ToString() };
            dgvPlaylist.Rows.Add(vals);
        }
    }
}