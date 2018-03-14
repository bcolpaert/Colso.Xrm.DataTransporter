using Colso.DataTransporter.AppCode;
using Colso.Xrm.DataTransporter.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Colso.DataTransporter.Forms
{
    public partial class Preview : Form
    {
        private List<ListViewItem> items;

        public Preview(List<ListViewItem> items)
        {
            this.items = items;
            InitializeComponent();
        }

        private void BtnCloseClick(object sender, EventArgs e)
        {
            Close();
        }

        private void ListLoad(object sender, EventArgs e)
        {
            lvItems.Columns.Clear();
            lvItems.Items.Clear();

            // Add columns
            lvItems.Columns.Add("Action", -2, HorizontalAlignment.Left);
            lvItems.Columns.Add("Name", -2, HorizontalAlignment.Left);
            lvItems.Columns.Add("Id", -2, HorizontalAlignment.Left);

            // Add items
            foreach (var item in items)
                lvItems.Items.Add(item);
        }
    }
}