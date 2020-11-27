namespace Colso.DataTransporter.Forms
{
    partial class PlaylistDialog
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
            this.dgvPlaylist = new System.Windows.Forms.DataGridView();
            this.clMoveDown = new System.Windows.Forms.DataGridViewImageColumn();
            this.clMoveUp = new System.Windows.Forms.DataGridViewImageColumn();
            this.clSequence = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clConfiguration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clCreate = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.clUpdate = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.clDelete = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnAddEntitySetting = new System.Windows.Forms.Button();
            this.btnTransfer = new System.Windows.Forms.Button();
            this.PlaylistBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.btnCancel = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPlaylist)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PlaylistBindingSource)).BeginInit();
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
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1161, 92);
            this.panel1.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(9, 45);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(538, 23);
            this.label2.TabIndex = 1;
            this.label2.Text = "The configurations in this list will be executed in the sequenced order";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 6);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 38);
            this.label1.TabIndex = 0;
            this.label1.Text = "Playlist";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(1018, 620);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(112, 38);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.BtnCloseClick);
            // 
            // dgvPlaylist
            // 
            this.dgvPlaylist.AllowDrop = true;
            this.dgvPlaylist.AllowUserToAddRows = false;
            this.dgvPlaylist.AllowUserToResizeRows = false;
            this.dgvPlaylist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPlaylist.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPlaylist.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clMoveDown,
            this.clMoveUp,
            this.clSequence,
            this.clConfiguration,
            this.clCreate,
            this.clUpdate,
            this.clDelete});
            this.dgvPlaylist.Location = new System.Drawing.Point(18, 102);
            this.dgvPlaylist.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgvPlaylist.MultiSelect = false;
            this.dgvPlaylist.Name = "dgvPlaylist";
            this.dgvPlaylist.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPlaylist.Size = new System.Drawing.Size(1113, 509);
            this.dgvPlaylist.TabIndex = 3;
            this.dgvPlaylist.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMappings_CellClick);
            this.dgvPlaylist.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPlaylist_CellEndEdit);
            // 
            // clMoveDown
            // 
            this.clMoveDown.FillWeight = 25F;
            this.clMoveDown.HeaderText = "";
            this.clMoveDown.Image = global::Colso.Xrm.DataTransporter.Properties.Resources.down;
            this.clMoveDown.Name = "clMoveDown";
            this.clMoveDown.ToolTipText = "Move Down";
            this.clMoveDown.Width = 25;
            // 
            // clMoveUp
            // 
            this.clMoveUp.FillWeight = 25F;
            this.clMoveUp.HeaderText = "";
            this.clMoveUp.Image = global::Colso.Xrm.DataTransporter.Properties.Resources.up;
            this.clMoveUp.Name = "clMoveUp";
            this.clMoveUp.ToolTipText = "Move Up";
            this.clMoveUp.Width = 25;
            // 
            // clSequence
            // 
            this.clSequence.FillWeight = 65F;
            this.clSequence.HeaderText = "Sequence";
            this.clSequence.Name = "clSequence";
            this.clSequence.ReadOnly = true;
            this.clSequence.Width = 65;
            // 
            // clConfiguration
            // 
            this.clConfiguration.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clConfiguration.HeaderText = "Configuration";
            this.clConfiguration.Name = "clConfiguration";
            this.clConfiguration.ReadOnly = true;
            this.clConfiguration.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clConfiguration.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clCreate
            // 
            this.clCreate.FillWeight = 50F;
            this.clCreate.HeaderText = "Create";
            this.clCreate.Name = "clCreate";
            this.clCreate.Width = 50;
            // 
            // clUpdate
            // 
            this.clUpdate.FillWeight = 50F;
            this.clUpdate.HeaderText = "Update";
            this.clUpdate.Name = "clUpdate";
            this.clUpdate.Width = 50;
            // 
            // clDelete
            // 
            this.clDelete.FillWeight = 50F;
            this.clDelete.HeaderText = "Delete";
            this.clDelete.Name = "clDelete";
            this.clDelete.Width = 50;
            // 
            // btnOpen
            // 
            this.btnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpen.Location = new System.Drawing.Point(897, 620);
            this.btnOpen.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(112, 38);
            this.btnOpen.TabIndex = 6;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(776, 620);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(112, 38);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnAddEntitySetting
            // 
            this.btnAddEntitySetting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddEntitySetting.Image = global::Colso.Xrm.DataTransporter.Properties.Resources.load;
            this.btnAddEntitySetting.Location = new System.Drawing.Point(18, 620);
            this.btnAddEntitySetting.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnAddEntitySetting.Name = "btnAddEntitySetting";
            this.btnAddEntitySetting.Size = new System.Drawing.Size(172, 38);
            this.btnAddEntitySetting.TabIndex = 5;
            this.btnAddEntitySetting.Text = "Add Configuration";
            this.btnAddEntitySetting.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAddEntitySetting.UseVisualStyleBackColor = true;
            this.btnAddEntitySetting.Click += new System.EventHandler(this.btnAddEntitySetting_Click);
            // 
            // btnTransfer
            // 
            this.btnTransfer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTransfer.Image = global::Colso.Xrm.DataTransporter.Properties.Resources.export;
            this.btnTransfer.Location = new System.Drawing.Point(200, 620);
            this.btnTransfer.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnTransfer.Name = "btnTransfer";
            this.btnTransfer.Size = new System.Drawing.Size(165, 38);
            this.btnTransfer.TabIndex = 4;
            this.btnTransfer.Text = "Transfer Data";
            this.btnTransfer.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnTransfer.UseVisualStyleBackColor = true;
            this.btnTransfer.Click += new System.EventHandler(this.btnTransfer_Click);
            // 
            // PlaylistBindingSource
            // 
            this.PlaylistBindingSource.DataSource = typeof(Colso.DataTransporter.Forms.PlaylistDialog);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.Location = new System.Drawing.Point(373, 620);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(165, 38);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // PlaylistDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1149, 674);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.btnAddEntitySetting);
            this.Controls.Add(this.btnTransfer);
            this.Controls.Add(this.dgvPlaylist);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PlaylistDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Playlist";
            this.Load += new System.EventHandler(this.PlaylistLoad);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPlaylist)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PlaylistBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridView dgvPlaylist;
        private System.Windows.Forms.BindingSource PlaylistBindingSource;
        private System.Windows.Forms.Button btnTransfer;
        private System.Windows.Forms.Button btnAddEntitySetting;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.DataGridViewImageColumn clMoveDown;
        private System.Windows.Forms.DataGridViewImageColumn clMoveUp;
        private System.Windows.Forms.DataGridViewTextBoxColumn clSequence;
        private System.Windows.Forms.DataGridViewTextBoxColumn clConfiguration;
        private System.Windows.Forms.DataGridViewCheckBoxColumn clCreate;
        private System.Windows.Forms.DataGridViewCheckBoxColumn clUpdate;
        private System.Windows.Forms.DataGridViewCheckBoxColumn clDelete;
        private System.Windows.Forms.Button btnCancel;
    }
}