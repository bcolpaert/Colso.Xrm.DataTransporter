using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Colso.DataTransporter.Forms
{
    public partial class MappingList : Form
    {
        protected ComboboxItem[] entities;
        private List<Tuple<EntityReference, EntityReference>> mappings;

        public MappingList(ListViewItem[] entities, List<Tuple<EntityReference, EntityReference>> mappings)
        {
            this.entities = entities.Select(lvi => new ComboboxItem() { Text = lvi.Text, Value = lvi.SubItems[1].Text }).ToArray();
            this.mappings = mappings;
            InitializeComponent();
        }

        public List<Tuple<EntityReference, EntityReference>> GetMappingList()
        {
            var list = new List<Tuple<EntityReference, EntityReference>>();

            foreach (DataGridViewRow m in dgvMappings.Rows)
            {
                if (!m.IsNewRow)
                {
                    var entity = (string)m.Cells[0].Value;
                    var sourceid = Guid.Parse((string)m.Cells[1].Value);
                    var targetid = Guid.Parse((string)m.Cells[2].Value);
                    list.Add(new Tuple<EntityReference, EntityReference>(new EntityReference(entity, sourceid), new EntityReference(entity, targetid)));
                }
            }

            return list;
        }

        private void BtnCloseClick(object sender, EventArgs e)
        {
            Close();
        }

        private void MappingListLoad(object sender, EventArgs e)
        {
            // Add entities
            var entityColumn = (DataGridViewComboBoxColumn)this.dgvMappings.Columns["clEntity"];
            entityColumn.DataSource = entities;
            entityColumn.ValueMember = "Value";
            entityColumn.DisplayMember = "Text";
            entityColumn.DataPropertyName = "clEntity";

            // Add mappings
            foreach (var m in mappings)
            {
                var vals = new object[3] { m.Item1.LogicalName, m.Item1.Id.ToString(), m.Item2.Id.ToString() };
                dgvMappings.Rows.Add(vals);
            }
        }

        private void dgvMappings_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            var blank = new object[3] { null, Guid.Empty.ToString(), Guid.Empty.ToString() };
            e.Row.SetValues(blank);
        }

        private void dgvMappings_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            Guid dummy;
            string column = dgvMappings.Columns[e.ColumnIndex].Name;

            // Abort validation if cell is not in the CompanyName column.
            if (column.Equals("clEntity"))
            {
                if (e.FormattedValue == null)
                {
                    dgvMappings.Rows[e.RowIndex].ErrorText = "Entity must not be empty";
                    e.Cancel = true;
                }
            }
            else if (!Guid.TryParse(e.FormattedValue.ToString(), out dummy))
            {
                // Check on valid GUID
                dgvMappings.Rows[e.RowIndex].ErrorText = string.Format("{0} is not a valid GUID", dgvMappings.Columns[e.ColumnIndex].HeaderText);
                e.Cancel = true;
            }
        }

        public class ComboboxItem
        {
            public string Text { get; set; }
            public string Value { get; set; }
        }
    }
}