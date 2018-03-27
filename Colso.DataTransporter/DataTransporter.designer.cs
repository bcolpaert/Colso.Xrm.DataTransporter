namespace Colso.DataTransporter
{
    partial class DataTransporter
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataTransporter));
            this.txtEntityFilter = new System.Windows.Forms.TextBox();
            this.lblEntityFilter = new System.Windows.Forms.Label();
            this.lvEntities = new System.Windows.Forms.ListView();
            this.clEntDisplayName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clEntLogicalName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clComment = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.viewImageList = new System.Windows.Forms.ImageList(this.components);
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbTransactionCurrency = new System.Windows.Forms.CheckBox();
            this.cbBusinessUnit = new System.Windows.Forms.CheckBox();
            this.gbSettings = new System.Windows.Forms.GroupBox();
            this.cbUpdate = new System.Windows.Forms.CheckBox();
            this.cbDelete = new System.Windows.Forms.CheckBox();
            this.cbCreate = new System.Windows.Forms.CheckBox();
            this.gbEnvironments = new System.Windows.Forms.GroupBox();
            this.lbSourceValue = new System.Windows.Forms.Label();
            this.lblSource = new System.Windows.Forms.Label();
            this.btnSelectTarget = new System.Windows.Forms.Button();
            this.lbTargetValue = new System.Windows.Forms.Label();
            this.tsMain = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.pnlBody = new System.Windows.Forms.TableLayoutPanel();
            this.gbAttributes = new System.Windows.Forms.GroupBox();
            this.btnEntityMappings = new System.Windows.Forms.Button();
            this.btnFilter = new System.Windows.Forms.Button();
            this.chkAllAttributes = new System.Windows.Forms.CheckBox();
            this.lvAttributes = new System.Windows.Forms.ListView();
            this.clAttDisplayName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clAttLogicalName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clAttType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clAttComment = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gbEntities = new System.Windows.Forms.GroupBox();
            this.tabSourceObjects = new System.Windows.Forms.TabControl();
            this.tabEntities = new System.Windows.Forms.TabPage();
            this.tabAssociations = new System.Windows.Forms.TabPage();
            this.btnAssMappings = new System.Windows.Forms.Button();
            this.txtAssFilter = new System.Windows.Forms.TextBox();
            this.lvAssociations = new System.Windows.Forms.ListView();
            this.clSchemaName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clIntersectEntityName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clEntity1LogicalName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clEntity1IntersectAttribute = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clEntity2LogicalName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clEntity2IntersectAttribute = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblAssFilter = new System.Windows.Forms.Label();
            this.tsbCloseThisTab = new System.Windows.Forms.ToolStripButton();
            this.tsbRefreshEntities = new System.Windows.Forms.ToolStripButton();
            this.tsbRefreshAssociations = new System.Windows.Forms.ToolStripButton();
            this.btnPreviewTransfer = new System.Windows.Forms.ToolStripButton();
            this.tsbTransferDashboards = new System.Windows.Forms.ToolStripButton();
            this.tsbDonate = new System.Windows.Forms.ToolStripDropDownButton();
            this.donateInUSDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.donateInEURToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.donateInGBPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlHeader.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.gbSettings.SuspendLayout();
            this.gbEnvironments.SuspendLayout();
            this.tsMain.SuspendLayout();
            this.pnlBody.SuspendLayout();
            this.gbAttributes.SuspendLayout();
            this.gbEntities.SuspendLayout();
            this.tabSourceObjects.SuspendLayout();
            this.tabEntities.SuspendLayout();
            this.tabAssociations.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtEntityFilter
            // 
            this.txtEntityFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEntityFilter.Location = new System.Drawing.Point(48, 17);
            this.txtEntityFilter.Name = "txtEntityFilter";
            this.txtEntityFilter.Size = new System.Drawing.Size(261, 20);
            this.txtEntityFilter.TabIndex = 66;
            this.txtEntityFilter.TextChanged += new System.EventHandler(this.txtEntityFilter_TextChanged);
            // 
            // lblEntityFilter
            // 
            this.lblEntityFilter.AutoSize = true;
            this.lblEntityFilter.Location = new System.Drawing.Point(6, 22);
            this.lblEntityFilter.Name = "lblEntityFilter";
            this.lblEntityFilter.Size = new System.Drawing.Size(32, 13);
            this.lblEntityFilter.TabIndex = 65;
            this.lblEntityFilter.Text = "Filter:";
            // 
            // lvEntities
            // 
            this.lvEntities.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvEntities.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clEntDisplayName,
            this.clEntLogicalName,
            this.clComment});
            this.lvEntities.FullRowSelect = true;
            this.lvEntities.HideSelection = false;
            this.lvEntities.Location = new System.Drawing.Point(7, 44);
            this.lvEntities.MultiSelect = false;
            this.lvEntities.Name = "lvEntities";
            this.lvEntities.Size = new System.Drawing.Size(302, 387);
            this.lvEntities.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvEntities.TabIndex = 64;
            this.lvEntities.UseCompatibleStateImageBehavior = false;
            this.lvEntities.View = System.Windows.Forms.View.Details;
            this.lvEntities.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvEntities_ColumnClick);
            this.lvEntities.SelectedIndexChanged += new System.EventHandler(this.lvEntities_SelectedIndexChanged);
            // 
            // clEntDisplayName
            // 
            this.clEntDisplayName.Text = "Display Name";
            this.clEntDisplayName.Width = 150;
            // 
            // clEntLogicalName
            // 
            this.clEntLogicalName.Text = "Logical Name";
            this.clEntLogicalName.Width = 150;
            // 
            // clComment
            // 
            this.clComment.Text = "Comment";
            this.clComment.Width = 120;
            // 
            // viewImageList
            // 
            this.viewImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("viewImageList.ImageStream")));
            this.viewImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.viewImageList.Images.SetKeyName(0, "dashboard.gif");
            this.viewImageList.Images.SetKeyName(1, "dashboard_user.png");
            // 
            // pnlHeader
            // 
            this.pnlHeader.Controls.Add(this.groupBox1);
            this.pnlHeader.Controls.Add(this.gbSettings);
            this.pnlHeader.Controls.Add(this.gbEnvironments);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 25);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(800, 100);
            this.pnlHeader.TabIndex = 103;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.cbTransactionCurrency);
            this.groupBox1.Controls.Add(this.cbBusinessUnit);
            this.groupBox1.Location = new System.Drawing.Point(599, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(194, 93);
            this.groupBox1.TabIndex = 103;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Auto-Mappings";
            // 
            // cbTransactionCurrency
            // 
            this.cbTransactionCurrency.AutoSize = true;
            this.cbTransactionCurrency.Checked = true;
            this.cbTransactionCurrency.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbTransactionCurrency.Location = new System.Drawing.Point(6, 44);
            this.cbTransactionCurrency.Name = "cbTransactionCurrency";
            this.cbTransactionCurrency.Size = new System.Drawing.Size(164, 17);
            this.cbTransactionCurrency.TabIndex = 2;
            this.cbTransactionCurrency.Text = "Default Transaction Currency";
            this.cbTransactionCurrency.UseVisualStyleBackColor = true;
            // 
            // cbBusinessUnit
            // 
            this.cbBusinessUnit.AutoSize = true;
            this.cbBusinessUnit.Checked = true;
            this.cbBusinessUnit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbBusinessUnit.Location = new System.Drawing.Point(6, 21);
            this.cbBusinessUnit.Name = "cbBusinessUnit";
            this.cbBusinessUnit.Size = new System.Drawing.Size(116, 17);
            this.cbBusinessUnit.TabIndex = 0;
            this.cbBusinessUnit.Text = "Root Business Unit";
            this.cbBusinessUnit.UseVisualStyleBackColor = true;
            // 
            // gbSettings
            // 
            this.gbSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSettings.Controls.Add(this.cbUpdate);
            this.gbSettings.Controls.Add(this.cbDelete);
            this.gbSettings.Controls.Add(this.cbCreate);
            this.gbSettings.Location = new System.Drawing.Point(516, 3);
            this.gbSettings.Name = "gbSettings";
            this.gbSettings.Size = new System.Drawing.Size(77, 94);
            this.gbSettings.TabIndex = 102;
            this.gbSettings.TabStop = false;
            this.gbSettings.Text = "Settings";
            // 
            // cbUpdate
            // 
            this.cbUpdate.AutoSize = true;
            this.cbUpdate.Location = new System.Drawing.Point(6, 44);
            this.cbUpdate.Name = "cbUpdate";
            this.cbUpdate.Size = new System.Drawing.Size(61, 17);
            this.cbUpdate.TabIndex = 2;
            this.cbUpdate.Text = "Update";
            this.cbUpdate.UseVisualStyleBackColor = true;
            // 
            // cbDelete
            // 
            this.cbDelete.AutoSize = true;
            this.cbDelete.Location = new System.Drawing.Point(6, 67);
            this.cbDelete.Name = "cbDelete";
            this.cbDelete.Size = new System.Drawing.Size(57, 17);
            this.cbDelete.TabIndex = 1;
            this.cbDelete.Text = "Delete";
            this.cbDelete.UseVisualStyleBackColor = true;
            // 
            // cbCreate
            // 
            this.cbCreate.AutoSize = true;
            this.cbCreate.Checked = true;
            this.cbCreate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCreate.Location = new System.Drawing.Point(6, 21);
            this.cbCreate.Name = "cbCreate";
            this.cbCreate.Size = new System.Drawing.Size(57, 17);
            this.cbCreate.TabIndex = 0;
            this.cbCreate.Text = "Create";
            this.cbCreate.UseVisualStyleBackColor = true;
            // 
            // gbEnvironments
            // 
            this.gbEnvironments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbEnvironments.Controls.Add(this.lbSourceValue);
            this.gbEnvironments.Controls.Add(this.lblSource);
            this.gbEnvironments.Controls.Add(this.btnSelectTarget);
            this.gbEnvironments.Controls.Add(this.lbTargetValue);
            this.gbEnvironments.Location = new System.Drawing.Point(3, 4);
            this.gbEnvironments.Name = "gbEnvironments";
            this.gbEnvironments.Size = new System.Drawing.Size(507, 93);
            this.gbEnvironments.TabIndex = 101;
            this.gbEnvironments.TabStop = false;
            this.gbEnvironments.Text = "Environments";
            // 
            // lbSourceValue
            // 
            this.lbSourceValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbSourceValue.AutoSize = true;
            this.lbSourceValue.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lbSourceValue.ForeColor = System.Drawing.Color.Red;
            this.lbSourceValue.Location = new System.Drawing.Point(114, 24);
            this.lbSourceValue.Name = "lbSourceValue";
            this.lbSourceValue.Size = new System.Drawing.Size(64, 13);
            this.lbSourceValue.TabIndex = 97;
            this.lbSourceValue.Text = "Unselected";
            // 
            // lblSource
            // 
            this.lblSource.AutoSize = true;
            this.lblSource.Location = new System.Drawing.Point(6, 24);
            this.lblSource.Name = "lblSource";
            this.lblSource.Size = new System.Drawing.Size(41, 13);
            this.lblSource.TabIndex = 100;
            this.lblSource.Text = "Source";
            // 
            // btnSelectTarget
            // 
            this.btnSelectTarget.Location = new System.Drawing.Point(6, 40);
            this.btnSelectTarget.Name = "btnSelectTarget";
            this.btnSelectTarget.Size = new System.Drawing.Size(85, 23);
            this.btnSelectTarget.TabIndex = 99;
            this.btnSelectTarget.Text = "Select target";
            this.btnSelectTarget.UseVisualStyleBackColor = true;
            this.btnSelectTarget.Click += new System.EventHandler(this.btnSelectTarget_Click);
            // 
            // lbTargetValue
            // 
            this.lbTargetValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbTargetValue.AutoSize = true;
            this.lbTargetValue.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lbTargetValue.ForeColor = System.Drawing.Color.Red;
            this.lbTargetValue.Location = new System.Drawing.Point(114, 45);
            this.lbTargetValue.Name = "lbTargetValue";
            this.lbTargetValue.Size = new System.Drawing.Size(64, 13);
            this.lbTargetValue.TabIndex = 98;
            this.lbTargetValue.Text = "Unselected";
            // 
            // tsMain
            // 
            this.tsMain.AutoSize = false;
            this.tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbCloseThisTab,
            this.toolStripSeparator2,
            this.tsbRefreshEntities,
            this.tsbRefreshAssociations,
            this.toolStripSeparator3,
            this.btnPreviewTransfer,
            this.tsbTransferDashboards,
            this.toolStripSeparator1,
            this.tsbDonate});
            this.tsMain.Location = new System.Drawing.Point(0, 0);
            this.tsMain.Name = "tsMain";
            this.tsMain.Size = new System.Drawing.Size(800, 25);
            this.tsMain.TabIndex = 90;
            this.tsMain.Text = "toolStrip1";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // pnlBody
            // 
            this.pnlBody.ColumnCount = 2;
            this.pnlBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40.875F));
            this.pnlBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 59.125F));
            this.pnlBody.Controls.Add(this.gbAttributes, 1, 0);
            this.pnlBody.Controls.Add(this.gbEntities, 0, 0);
            this.pnlBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBody.Location = new System.Drawing.Point(3, 3);
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.RowCount = 1;
            this.pnlBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pnlBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pnlBody.Size = new System.Drawing.Size(786, 443);
            this.pnlBody.TabIndex = 104;
            // 
            // gbAttributes
            // 
            this.gbAttributes.Controls.Add(this.btnEntityMappings);
            this.gbAttributes.Controls.Add(this.btnFilter);
            this.gbAttributes.Controls.Add(this.chkAllAttributes);
            this.gbAttributes.Controls.Add(this.lvAttributes);
            this.gbAttributes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbAttributes.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.gbAttributes.Location = new System.Drawing.Point(324, 3);
            this.gbAttributes.Name = "gbAttributes";
            this.gbAttributes.Size = new System.Drawing.Size(459, 437);
            this.gbAttributes.TabIndex = 92;
            this.gbAttributes.TabStop = false;
            this.gbAttributes.Text = "Available attributes";
            // 
            // btnEntityMappings
            // 
            this.btnEntityMappings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEntityMappings.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnEntityMappings.Location = new System.Drawing.Point(322, 17);
            this.btnEntityMappings.Name = "btnEntityMappings";
            this.btnEntityMappings.Size = new System.Drawing.Size(72, 23);
            this.btnEntityMappings.TabIndex = 102;
            this.btnEntityMappings.Text = "Mappings";
            this.btnEntityMappings.UseVisualStyleBackColor = true;
            this.btnEntityMappings.Click += new System.EventHandler(this.btnMappings_Click);
            // 
            // btnFilter
            // 
            this.btnFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFilter.Location = new System.Drawing.Point(400, 17);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(53, 23);
            this.btnFilter.TabIndex = 101;
            this.btnFilter.Text = "Filters";
            this.btnFilter.UseVisualStyleBackColor = true;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // chkAllAttributes
            // 
            this.chkAllAttributes.AutoSize = true;
            this.chkAllAttributes.Checked = true;
            this.chkAllAttributes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAllAttributes.Location = new System.Drawing.Point(6, 21);
            this.chkAllAttributes.Name = "chkAllAttributes";
            this.chkAllAttributes.Size = new System.Drawing.Size(120, 17);
            this.chkAllAttributes.TabIndex = 3;
            this.chkAllAttributes.Text = "Select/Unselect All";
            this.chkAllAttributes.UseVisualStyleBackColor = true;
            this.chkAllAttributes.CheckedChanged += new System.EventHandler(this.chkAllAttributes_CheckedChanged);
            // 
            // lvAttributes
            // 
            this.lvAttributes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvAttributes.CheckBoxes = true;
            this.lvAttributes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clAttDisplayName,
            this.clAttLogicalName,
            this.clAttType,
            this.clAttComment});
            this.lvAttributes.FullRowSelect = true;
            this.lvAttributes.HideSelection = false;
            this.lvAttributes.Location = new System.Drawing.Point(6, 44);
            this.lvAttributes.Name = "lvAttributes";
            this.lvAttributes.Size = new System.Drawing.Size(447, 387);
            this.lvAttributes.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvAttributes.TabIndex = 64;
            this.lvAttributes.UseCompatibleStateImageBehavior = false;
            this.lvAttributes.View = System.Windows.Forms.View.Details;
            this.lvAttributes.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvAttributes_ColumnClick);
            // 
            // clAttDisplayName
            // 
            this.clAttDisplayName.Text = "Display Name";
            this.clAttDisplayName.Width = 150;
            // 
            // clAttLogicalName
            // 
            this.clAttLogicalName.Text = "Logical Name";
            this.clAttLogicalName.Width = 150;
            // 
            // clAttType
            // 
            this.clAttType.Text = "Type";
            this.clAttType.Width = 100;
            // 
            // clAttComment
            // 
            this.clAttComment.Text = "Comment";
            this.clAttComment.Width = 200;
            // 
            // gbEntities
            // 
            this.gbEntities.Controls.Add(this.txtEntityFilter);
            this.gbEntities.Controls.Add(this.lblEntityFilter);
            this.gbEntities.Controls.Add(this.lvEntities);
            this.gbEntities.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbEntities.Location = new System.Drawing.Point(3, 3);
            this.gbEntities.Name = "gbEntities";
            this.gbEntities.Size = new System.Drawing.Size(315, 437);
            this.gbEntities.TabIndex = 93;
            this.gbEntities.TabStop = false;
            this.gbEntities.Text = "Available Entities";
            // 
            // tabSourceObjects
            // 
            this.tabSourceObjects.Controls.Add(this.tabEntities);
            this.tabSourceObjects.Controls.Add(this.tabAssociations);
            this.tabSourceObjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabSourceObjects.Location = new System.Drawing.Point(0, 125);
            this.tabSourceObjects.Name = "tabSourceObjects";
            this.tabSourceObjects.SelectedIndex = 0;
            this.tabSourceObjects.Size = new System.Drawing.Size(800, 475);
            this.tabSourceObjects.TabIndex = 67;
            this.tabSourceObjects.SelectedIndexChanged += new System.EventHandler(this.tabSourceObjects_SelectedIndexChanged);
            // 
            // tabEntities
            // 
            this.tabEntities.Controls.Add(this.pnlBody);
            this.tabEntities.Location = new System.Drawing.Point(4, 22);
            this.tabEntities.Name = "tabEntities";
            this.tabEntities.Padding = new System.Windows.Forms.Padding(3);
            this.tabEntities.Size = new System.Drawing.Size(792, 449);
            this.tabEntities.TabIndex = 0;
            this.tabEntities.Text = "Entities";
            this.tabEntities.UseVisualStyleBackColor = true;
            // 
            // tabAssociations
            // 
            this.tabAssociations.Controls.Add(this.btnAssMappings);
            this.tabAssociations.Controls.Add(this.txtAssFilter);
            this.tabAssociations.Controls.Add(this.lvAssociations);
            this.tabAssociations.Controls.Add(this.lblAssFilter);
            this.tabAssociations.Location = new System.Drawing.Point(4, 22);
            this.tabAssociations.Name = "tabAssociations";
            this.tabAssociations.Padding = new System.Windows.Forms.Padding(3);
            this.tabAssociations.Size = new System.Drawing.Size(792, 449);
            this.tabAssociations.TabIndex = 1;
            this.tabAssociations.Text = "Associations";
            this.tabAssociations.UseVisualStyleBackColor = true;
            // 
            // btnAssMappings
            // 
            this.btnAssMappings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAssMappings.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnAssMappings.Location = new System.Drawing.Point(714, 4);
            this.btnAssMappings.Name = "btnAssMappings";
            this.btnAssMappings.Size = new System.Drawing.Size(72, 23);
            this.btnAssMappings.TabIndex = 103;
            this.btnAssMappings.Text = "Mappings";
            this.btnAssMappings.UseVisualStyleBackColor = true;
            this.btnAssMappings.Click += new System.EventHandler(this.btnMappings_Click);
            // 
            // txtAssFilter
            // 
            this.txtAssFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAssFilter.Location = new System.Drawing.Point(47, 6);
            this.txtAssFilter.Name = "txtAssFilter";
            this.txtAssFilter.Size = new System.Drawing.Size(661, 20);
            this.txtAssFilter.TabIndex = 69;
            this.txtAssFilter.TextChanged += new System.EventHandler(this.txtAssFilter_TextChanged);
            // 
            // lvAssociations
            // 
            this.lvAssociations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvAssociations.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clSchemaName,
            this.clIntersectEntityName,
            this.clEntity1LogicalName,
            this.clEntity1IntersectAttribute,
            this.clEntity2LogicalName,
            this.clEntity2IntersectAttribute});
            this.lvAssociations.FullRowSelect = true;
            this.lvAssociations.HideSelection = false;
            this.lvAssociations.Location = new System.Drawing.Point(6, 34);
            this.lvAssociations.MultiSelect = false;
            this.lvAssociations.Name = "lvAssociations";
            this.lvAssociations.Size = new System.Drawing.Size(780, 409);
            this.lvAssociations.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvAssociations.TabIndex = 67;
            this.lvAssociations.UseCompatibleStateImageBehavior = false;
            this.lvAssociations.View = System.Windows.Forms.View.Details;
            this.lvAssociations.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvAssociations_ColumnClick);
            this.lvAssociations.SelectedIndexChanged += new System.EventHandler(this.lvAssociations_SelectedIndexChanged);
            // 
            // clSchemaName
            // 
            this.clSchemaName.Text = "Relationship Schema Name";
            this.clSchemaName.Width = 200;
            // 
            // clIntersectEntityName
            // 
            this.clIntersectEntityName.Text = "Intersect Entity Name";
            this.clIntersectEntityName.Width = 120;
            // 
            // clEntity1LogicalName
            // 
            this.clEntity1LogicalName.Text = "Entity 1";
            this.clEntity1LogicalName.Width = 120;
            // 
            // clEntity1IntersectAttribute
            // 
            this.clEntity1IntersectAttribute.Text = "Attribute 1";
            this.clEntity1IntersectAttribute.Width = 120;
            // 
            // clEntity2LogicalName
            // 
            this.clEntity2LogicalName.Text = "Entity 2";
            this.clEntity2LogicalName.Width = 120;
            // 
            // clEntity2IntersectAttribute
            // 
            this.clEntity2IntersectAttribute.Text = "Attribute 2";
            this.clEntity2IntersectAttribute.Width = 120;
            // 
            // lblAssFilter
            // 
            this.lblAssFilter.AutoSize = true;
            this.lblAssFilter.Location = new System.Drawing.Point(5, 11);
            this.lblAssFilter.Name = "lblAssFilter";
            this.lblAssFilter.Size = new System.Drawing.Size(32, 13);
            this.lblAssFilter.TabIndex = 68;
            this.lblAssFilter.Text = "Filter:";
            // 
            // tsbCloseThisTab
            // 
            this.tsbCloseThisTab.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.tsbCloseThisTab.Image = ((System.Drawing.Image)(resources.GetObject("tsbCloseThisTab.Image")));
            this.tsbCloseThisTab.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCloseThisTab.Name = "tsbCloseThisTab";
            this.tsbCloseThisTab.Size = new System.Drawing.Size(55, 22);
            this.tsbCloseThisTab.Text = "Close";
            this.tsbCloseThisTab.Click += new System.EventHandler(this.tsbCloseThisTab_Click);
            // 
            // tsbRefreshEntities
            // 
            this.tsbRefreshEntities.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.tsbRefreshEntities.Image = global::Colso.Xrm.DataTransporter.Properties.Resources.entities;
            this.tsbRefreshEntities.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRefreshEntities.Name = "tsbRefreshEntities";
            this.tsbRefreshEntities.Size = new System.Drawing.Size(107, 22);
            this.tsbRefreshEntities.Text = "Refresh Entities";
            this.tsbRefreshEntities.Click += new System.EventHandler(this.tsbRefreshEntities_Click);
            // 
            // tsbRefreshAssociations
            // 
            this.tsbRefreshAssociations.Image = global::Colso.Xrm.DataTransporter.Properties.Resources.relation;
            this.tsbRefreshAssociations.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRefreshAssociations.Name = "tsbRefreshAssociations";
            this.tsbRefreshAssociations.Size = new System.Drawing.Size(135, 22);
            this.tsbRefreshAssociations.Text = "Refresh Associations";
            this.tsbRefreshAssociations.Click += new System.EventHandler(this.tsbRefreshAssociations_Click);
            // 
            // btnPreviewTransfer
            // 
            this.btnPreviewTransfer.Image = global::Colso.Xrm.DataTransporter.Properties.Resources.preview;
            this.btnPreviewTransfer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPreviewTransfer.Name = "btnPreviewTransfer";
            this.btnPreviewTransfer.Size = new System.Drawing.Size(68, 22);
            this.btnPreviewTransfer.Text = "Preview";
            this.btnPreviewTransfer.Click += new System.EventHandler(this.btnPreviewTransfer_Click);
            // 
            // tsbTransferDashboards
            // 
            this.tsbTransferDashboards.Image = global::Colso.Xrm.DataTransporter.Properties.Resources.export;
            this.tsbTransferDashboards.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbTransferDashboards.Name = "tsbTransferDashboards";
            this.tsbTransferDashboards.Size = new System.Drawing.Size(96, 22);
            this.tsbTransferDashboards.Text = "Transfer Data";
            this.tsbTransferDashboards.Click += new System.EventHandler(this.tsbTransferData_Click);
            // 
            // tsbDonate
            // 
            this.tsbDonate.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.donateInUSDToolStripMenuItem,
            this.donateInEURToolStripMenuItem,
            this.donateInGBPToolStripMenuItem});
            this.tsbDonate.Image = global::Colso.Xrm.DataTransporter.Properties.Resources.paypal;
            this.tsbDonate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDonate.Name = "tsbDonate";
            this.tsbDonate.Size = new System.Drawing.Size(74, 22);
            this.tsbDonate.Text = "Donate";
            // 
            // donateInUSDToolStripMenuItem
            // 
            this.donateInUSDToolStripMenuItem.Name = "donateInUSDToolStripMenuItem";
            this.donateInUSDToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.donateInUSDToolStripMenuItem.Text = "Donate in USD";
            this.donateInUSDToolStripMenuItem.Click += new System.EventHandler(this.donateInUSDToolStripMenuItem_Click);
            // 
            // donateInEURToolStripMenuItem
            // 
            this.donateInEURToolStripMenuItem.Name = "donateInEURToolStripMenuItem";
            this.donateInEURToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.donateInEURToolStripMenuItem.Text = "Donate in EUR";
            this.donateInEURToolStripMenuItem.Click += new System.EventHandler(this.donateInEURToolStripMenuItem_Click);
            // 
            // donateInGBPToolStripMenuItem
            // 
            this.donateInGBPToolStripMenuItem.Name = "donateInGBPToolStripMenuItem";
            this.donateInGBPToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.donateInGBPToolStripMenuItem.Text = "Donate in GBP";
            this.donateInGBPToolStripMenuItem.Click += new System.EventHandler(this.donateInGBPToolStripMenuItem_Click);
            // 
            // DataTransporter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.tabSourceObjects);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.tsMain);
            this.Name = "DataTransporter";
            this.Size = new System.Drawing.Size(800, 600);
            this.pnlHeader.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gbSettings.ResumeLayout(false);
            this.gbSettings.PerformLayout();
            this.gbEnvironments.ResumeLayout(false);
            this.gbEnvironments.PerformLayout();
            this.tsMain.ResumeLayout(false);
            this.tsMain.PerformLayout();
            this.pnlBody.ResumeLayout(false);
            this.gbAttributes.ResumeLayout(false);
            this.gbAttributes.PerformLayout();
            this.gbEntities.ResumeLayout(false);
            this.gbEntities.PerformLayout();
            this.tabSourceObjects.ResumeLayout(false);
            this.tabEntities.ResumeLayout(false);
            this.tabAssociations.ResumeLayout(false);
            this.tabAssociations.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ToolStrip tsMain;
        private System.Windows.Forms.ToolStripButton tsbCloseThisTab;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton tsbRefreshEntities;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbTransferDashboards;
        private System.Windows.Forms.ImageList viewImageList;
        private System.Windows.Forms.Label lbSourceValue;
        private System.Windows.Forms.Label lbTargetValue;
        private System.Windows.Forms.GroupBox gbEnvironments;
        private System.Windows.Forms.Button btnSelectTarget;
        private System.Windows.Forms.Label lblSource;
        private System.Windows.Forms.ListView lvEntities;
        private System.Windows.Forms.ColumnHeader clEntDisplayName;
        private System.Windows.Forms.ColumnHeader clEntLogicalName;
        private System.Windows.Forms.GroupBox gbSettings;
        private System.Windows.Forms.CheckBox cbUpdate;
        private System.Windows.Forms.CheckBox cbDelete;
        private System.Windows.Forms.CheckBox cbCreate;
        private System.Windows.Forms.TableLayoutPanel pnlBody;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.GroupBox gbAttributes;
        private System.Windows.Forms.ListView lvAttributes;
        private System.Windows.Forms.ColumnHeader clAttDisplayName;
        private System.Windows.Forms.ColumnHeader clAttLogicalName;
        private System.Windows.Forms.CheckBox chkAllAttributes;
        private System.Windows.Forms.ColumnHeader clAttType;
        private System.Windows.Forms.Button btnEntityMappings;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.ColumnHeader clAttComment;
        private System.Windows.Forms.TextBox txtEntityFilter;
        private System.Windows.Forms.Label lblEntityFilter;
        private System.Windows.Forms.ColumnHeader clComment;
        private System.Windows.Forms.TabControl tabSourceObjects;
        private System.Windows.Forms.TabPage tabEntities;
        private System.Windows.Forms.TabPage tabAssociations;
        private System.Windows.Forms.ToolStripButton tsbRefreshAssociations;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.TextBox txtAssFilter;
        private System.Windows.Forms.ListView lvAssociations;
        private System.Windows.Forms.ColumnHeader clSchemaName;
        private System.Windows.Forms.ColumnHeader clEntity1LogicalName;
        private System.Windows.Forms.Label lblAssFilter;
        private System.Windows.Forms.GroupBox gbEntities;
        private System.Windows.Forms.Button btnAssMappings;
        private System.Windows.Forms.ColumnHeader clIntersectEntityName;
        private System.Windows.Forms.ColumnHeader clEntity1IntersectAttribute;
        private System.Windows.Forms.ColumnHeader clEntity2LogicalName;
        private System.Windows.Forms.ColumnHeader clEntity2IntersectAttribute;
        private System.Windows.Forms.ToolStripButton btnPreviewTransfer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cbTransactionCurrency;
        private System.Windows.Forms.CheckBox cbBusinessUnit;
        private System.Windows.Forms.ToolStripDropDownButton tsbDonate;
        private System.Windows.Forms.ToolStripMenuItem donateInUSDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem donateInEURToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem donateInGBPToolStripMenuItem;
    }
}
