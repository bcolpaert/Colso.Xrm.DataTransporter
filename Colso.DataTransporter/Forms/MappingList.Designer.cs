namespace Colso.DataTransporter.Forms
{
    partial class MappingList
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.dgvMappings = new System.Windows.Forms.DataGridView();
            this.mappingListBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.clEntity = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.clSourceID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clTargetID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMappings)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mappingListBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(688, 60);
            this.panel1.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(389, 26);
            this.label2.TabIndex = 1;
            this.label2.Text = "The references in this list will be replaced in the source dataset before the \r\nt" +
    "ransfer to the target environment";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Mapping list";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(593, 307);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.BtnCloseClick);
            // 
            // dgvMappings
            // 
            this.dgvMappings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvMappings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMappings.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clEntity,
            this.clSourceID,
            this.clTargetID});
            this.dgvMappings.Location = new System.Drawing.Point(12, 66);
            this.dgvMappings.Name = "dgvMappings";
            this.dgvMappings.Size = new System.Drawing.Size(656, 235);
            this.dgvMappings.TabIndex = 3;
            this.dgvMappings.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dgvMappings_CellValidating);
            this.dgvMappings.DefaultValuesNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.dgvMappings_DefaultValuesNeeded);
            // 
            // mappingListBindingSource
            // 
            this.mappingListBindingSource.DataSource = typeof(Colso.DataTransporter.Forms.MappingList);
            // 
            // clEntity
            // 
            this.clEntity.HeaderText = "Entity";
            this.clEntity.Name = "clEntity";
            this.clEntity.Width = 200;
            // 
            // clSourceID
            // 
            this.clSourceID.HeaderText = "Source ID";
            this.clSourceID.Name = "clSourceID";
            this.clSourceID.Width = 205;
            // 
            // clTargetID
            // 
            this.clTargetID.HeaderText = "Target ID";
            this.clTargetID.Name = "clTargetID";
            this.clTargetID.Width = 205;
            // 
            // MappingList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(680, 342);
            this.Controls.Add(this.dgvMappings);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MappingList";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Mapping list";
            this.Load += new System.EventHandler(this.MappingListLoad);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMappings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mappingListBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridView dgvMappings;
        private System.Windows.Forms.DataGridViewComboBoxColumn clEntity;
        private System.Windows.Forms.DataGridViewTextBoxColumn clSourceID;
        private System.Windows.Forms.DataGridViewTextBoxColumn clTargetID;
        private System.Windows.Forms.BindingSource mappingListBindingSource;
    }
}