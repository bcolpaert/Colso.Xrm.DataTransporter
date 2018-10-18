using Colso.DataTransporter.AppCode;
using Colso.Xrm.DataTransporter.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Colso.DataTransporter.Forms
{
    public partial class ErrorList : Form
    {
        private List<Item<string, string>> errors;

        public ErrorList(List<Item<string, string>> errors)
        {
            this.errors = errors;
            InitializeComponent();
        }

        private void BtnCloseClick(object sender, EventArgs e)
        {
            Close();
        }

        private void ErrorListLoad(object sender, EventArgs e)
        {
            foreach (var error in errors)
            {
                var item = new ListViewItem(error.Key);
                item.SubItems.Add(error.Value);

                lvErrors.Items.Add(item);
            }
        }

        private void lvErrorsListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (sender != lvErrors) return;

            if (e.Control && e.KeyCode == Keys.C)
                CopySelectedValuesToClipboard();
        }

        private void CopySelectedValuesToClipboard()
        {
            var builder = new StringBuilder();
            foreach (ListViewItem item in lvErrors.SelectedItems)
                builder.AppendLine(item.SubItems[1].Text);

            Clipboard.SetText(builder.ToString());
        }

    }
}