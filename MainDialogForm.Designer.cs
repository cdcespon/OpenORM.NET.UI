namespace OpenORM.UI
{
    partial class MainDialogForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainDialogForm));
            this.TreeImageList = new System.Windows.Forms.ImageList(this.components);
            this.StatusStrip = new System.Windows.Forms.StatusStrip();
            this.ToolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.MainButtonsPanel = new System.Windows.Forms.Panel();
            this.btnOpenFolder = new System.Windows.Forms.Button();
            this.MainImageList = new System.Windows.Forms.ImageList(this.components);
            this.btnReload = new System.Windows.Forms.Button();
            this.btnAccept = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.MainPluginImageList = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.MainsplitContainer = new System.Windows.Forms.SplitContainer();
            this.splitContainerSelectors = new System.Windows.Forms.SplitContainer();
            this.dbObjectsTreeview = new System.Windows.Forms.TreeView();
            this.pluginsListView = new System.Windows.Forms.ListView();
            this.Data = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.OutputListview = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Source = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.OutputToolStrip = new System.Windows.Forms.ToolStrip();
            this.ErrorsToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.WarningsToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.InfoToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.DeleteOutputToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.StatusStrip.SuspendLayout();
            this.MainButtonsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MainsplitContainer)).BeginInit();
            this.MainsplitContainer.Panel1.SuspendLayout();
            this.MainsplitContainer.Panel2.SuspendLayout();
            this.MainsplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerSelectors)).BeginInit();
            this.splitContainerSelectors.Panel1.SuspendLayout();
            this.splitContainerSelectors.Panel2.SuspendLayout();
            this.splitContainerSelectors.SuspendLayout();
            this.OutputToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // TreeImageList
            // 
            this.TreeImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("TreeImageList.ImageStream")));
            this.TreeImageList.TransparentColor = System.Drawing.Color.White;
            this.TreeImageList.Images.SetKeyName(0, "");
            this.TreeImageList.Images.SetKeyName(1, "");
            this.TreeImageList.Images.SetKeyName(2, "");
            this.TreeImageList.Images.SetKeyName(3, "");
            this.TreeImageList.Images.SetKeyName(4, "");
            this.TreeImageList.Images.SetKeyName(5, "");
            this.TreeImageList.Images.SetKeyName(6, "");
            this.TreeImageList.Images.SetKeyName(7, "");
            this.TreeImageList.Images.SetKeyName(8, "");
            this.TreeImageList.Images.SetKeyName(9, "");
            this.TreeImageList.Images.SetKeyName(10, "");
            this.TreeImageList.Images.SetKeyName(11, "");
            this.TreeImageList.Images.SetKeyName(12, "");
            this.TreeImageList.Images.SetKeyName(13, "");
            this.TreeImageList.Images.SetKeyName(14, "");
            this.TreeImageList.Images.SetKeyName(15, "");
            this.TreeImageList.Images.SetKeyName(16, "");
            this.TreeImageList.Images.SetKeyName(17, "");
            this.TreeImageList.Images.SetKeyName(18, "");
            this.TreeImageList.Images.SetKeyName(19, "");
            this.TreeImageList.Images.SetKeyName(20, "");
            this.TreeImageList.Images.SetKeyName(21, "");
            this.TreeImageList.Images.SetKeyName(22, "");
            this.TreeImageList.Images.SetKeyName(23, "");
            this.TreeImageList.Images.SetKeyName(24, "");
            this.TreeImageList.Images.SetKeyName(25, "");
            // 
            // StatusStrip
            // 
            this.StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripProgressBar});
            this.StatusStrip.Location = new System.Drawing.Point(0, 541);
            this.StatusStrip.Name = "StatusStrip";
            this.StatusStrip.Size = new System.Drawing.Size(1029, 22);
            this.StatusStrip.TabIndex = 13;
            this.StatusStrip.Text = "StatusStrip1";
            // 
            // ToolStripProgressBar
            // 
            this.ToolStripProgressBar.Enabled = false;
            this.ToolStripProgressBar.Name = "ToolStripProgressBar";
            this.ToolStripProgressBar.Size = new System.Drawing.Size(100, 16);
            this.ToolStripProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.ToolStripProgressBar.Visible = false;
            // 
            // MainButtonsPanel
            // 
            this.MainButtonsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MainButtonsPanel.Controls.Add(this.btnOpenFolder);
            this.MainButtonsPanel.Controls.Add(this.btnReload);
            this.MainButtonsPanel.Controls.Add(this.btnAccept);
            this.MainButtonsPanel.Controls.Add(this.btnCancel);
            this.MainButtonsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.MainButtonsPanel.Location = new System.Drawing.Point(0, 0);
            this.MainButtonsPanel.Name = "MainButtonsPanel";
            this.MainButtonsPanel.Size = new System.Drawing.Size(1029, 28);
            this.MainButtonsPanel.TabIndex = 14;
            // 
            // btnOpenFolder
            // 
            this.btnOpenFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOpenFolder.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOpenFolder.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOpenFolder.ImageIndex = 4;
            this.btnOpenFolder.ImageList = this.MainImageList;
            this.btnOpenFolder.Location = new System.Drawing.Point(80, 3);
            this.btnOpenFolder.Name = "btnOpenFolder";
            this.btnOpenFolder.Size = new System.Drawing.Size(89, 22);
            this.btnOpenFolder.TabIndex = 3;
            this.btnOpenFolder.Text = "&Open Folder";
            this.btnOpenFolder.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnOpenFolder.UseVisualStyleBackColor = true;
            this.btnOpenFolder.Click += new System.EventHandler(this.btnOpenFolder_Click);
            // 
            // MainImageList
            // 
            this.MainImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("MainImageList.ImageStream")));
            this.MainImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.MainImageList.Images.SetKeyName(0, "INFO.ICO");
            this.MainImageList.Images.SetKeyName(1, "error.ico");
            this.MainImageList.Images.SetKeyName(2, "warning.ico");
            this.MainImageList.Images.SetKeyName(3, "Refresh_24x24.png");
            this.MainImageList.Images.SetKeyName(4, "Open_24x24.png");
            this.MainImageList.Images.SetKeyName(5, "Settings_24x24.png");
            this.MainImageList.Images.SetKeyName(6, "FolderClose_24x24.png");
            // 
            // btnReload
            // 
            this.btnReload.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReload.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnReload.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReload.ImageIndex = 3;
            this.btnReload.ImageList = this.MainImageList;
            this.btnReload.Location = new System.Drawing.Point(173, 3);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(68, 22);
            this.btnReload.TabIndex = 2;
            this.btnReload.Text = "&Reload";
            this.btnReload.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // btnAccept
            // 
            this.btnAccept.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAccept.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAccept.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAccept.ImageIndex = 5;
            this.btnAccept.ImageList = this.MainImageList;
            this.btnAccept.Location = new System.Drawing.Point(3, 3);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(75, 22);
            this.btnAccept.TabIndex = 1;
            this.btnAccept.Text = "&Generate";
            this.btnAccept.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.ImageIndex = 6;
            this.btnCancel.ImageList = this.MainImageList;
            this.btnCancel.Location = new System.Drawing.Point(952, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 22);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "&Exit";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // MainPluginImageList
            // 
            this.MainPluginImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("MainPluginImageList.ImageStream")));
            this.MainPluginImageList.TransparentColor = System.Drawing.Color.White;
            this.MainPluginImageList.Images.SetKeyName(0, "");
            this.MainPluginImageList.Images.SetKeyName(1, "");
            this.MainPluginImageList.Images.SetKeyName(2, "");
            this.MainPluginImageList.Images.SetKeyName(3, "");
            this.MainPluginImageList.Images.SetKeyName(4, "");
            this.MainPluginImageList.Images.SetKeyName(5, "");
            this.MainPluginImageList.Images.SetKeyName(6, "");
            this.MainPluginImageList.Images.SetKeyName(7, "");
            this.MainPluginImageList.Images.SetKeyName(8, "");
            this.MainPluginImageList.Images.SetKeyName(9, "");
            this.MainPluginImageList.Images.SetKeyName(10, "");
            this.MainPluginImageList.Images.SetKeyName(11, "");
            this.MainPluginImageList.Images.SetKeyName(12, "");
            this.MainPluginImageList.Images.SetKeyName(13, "");
            this.MainPluginImageList.Images.SetKeyName(14, "");
            this.MainPluginImageList.Images.SetKeyName(15, "");
            this.MainPluginImageList.Images.SetKeyName(16, "");
            this.MainPluginImageList.Images.SetKeyName(17, "");
            this.MainPluginImageList.Images.SetKeyName(18, "");
            this.MainPluginImageList.Images.SetKeyName(19, "");
            this.MainPluginImageList.Images.SetKeyName(20, "");
            this.MainPluginImageList.Images.SetKeyName(21, "");
            this.MainPluginImageList.Images.SetKeyName(22, "");
            this.MainPluginImageList.Images.SetKeyName(23, "");
            this.MainPluginImageList.Images.SetKeyName(24, "");
            this.MainPluginImageList.Images.SetKeyName(25, "");
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 28);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.MainsplitContainer);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.OutputListview);
            this.splitContainer1.Panel2.Controls.Add(this.OutputToolStrip);
            this.splitContainer1.Size = new System.Drawing.Size(1029, 513);
            this.splitContainer1.SplitterDistance = 333;
            this.splitContainer1.TabIndex = 15;
            // 
            // MainsplitContainer
            // 
            this.MainsplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainsplitContainer.Location = new System.Drawing.Point(0, 5);
            this.MainsplitContainer.Name = "MainsplitContainer";
            // 
            // MainsplitContainer.Panel1
            // 
            this.MainsplitContainer.Panel1.Controls.Add(this.splitContainerSelectors);
            // 
            // MainsplitContainer.Panel2
            // 
            this.MainsplitContainer.Panel2.Controls.Add(this.PropertyGrid);
            this.MainsplitContainer.Size = new System.Drawing.Size(1029, 322);
            this.MainsplitContainer.SplitterDistance = 687;
            this.MainsplitContainer.TabIndex = 1;
            // 
            // splitContainerSelectors
            // 
            this.splitContainerSelectors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerSelectors.Location = new System.Drawing.Point(0, 0);
            this.splitContainerSelectors.Name = "splitContainerSelectors";
            // 
            // splitContainerSelectors.Panel1
            // 
            this.splitContainerSelectors.Panel1.Controls.Add(this.dbObjectsTreeview);
            // 
            // splitContainerSelectors.Panel2
            // 
            this.splitContainerSelectors.Panel2.Controls.Add(this.pluginsListView);
            this.splitContainerSelectors.Size = new System.Drawing.Size(687, 322);
            this.splitContainerSelectors.SplitterDistance = 320;
            this.splitContainerSelectors.TabIndex = 14;
            // 
            // dbObjectsTreeview
            // 
            this.dbObjectsTreeview.BackColor = System.Drawing.Color.White;
            this.dbObjectsTreeview.CheckBoxes = true;
            this.dbObjectsTreeview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dbObjectsTreeview.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dbObjectsTreeview.FullRowSelect = true;
            this.dbObjectsTreeview.HotTracking = true;
            this.dbObjectsTreeview.ImageIndex = 0;
            this.dbObjectsTreeview.ImageList = this.TreeImageList;
            this.dbObjectsTreeview.Location = new System.Drawing.Point(0, 0);
            this.dbObjectsTreeview.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.dbObjectsTreeview.Name = "dbObjectsTreeview";
            this.dbObjectsTreeview.SelectedImageIndex = 0;
            this.dbObjectsTreeview.Size = new System.Drawing.Size(320, 322);
            this.dbObjectsTreeview.TabIndex = 14;
            this.dbObjectsTreeview.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.dbObjectsTreeview_AfterCheck);
            this.dbObjectsTreeview.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.dbObjectsTreeview_AfterExpand);
            // 
            // pluginsListView
            // 
            this.pluginsListView.CheckBoxes = true;
            this.pluginsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Data});
            this.pluginsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pluginsListView.HideSelection = false;
            this.pluginsListView.Location = new System.Drawing.Point(0, 0);
            this.pluginsListView.Name = "pluginsListView";
            this.pluginsListView.Size = new System.Drawing.Size(363, 322);
            this.pluginsListView.SmallImageList = this.MainPluginImageList;
            this.pluginsListView.TabIndex = 9;
            this.pluginsListView.UseCompatibleStateImageBehavior = false;
            this.pluginsListView.View = System.Windows.Forms.View.Details;
            this.pluginsListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.pluginsListView_ItemChecked);
            this.pluginsListView.DoubleClick += new System.EventHandler(this.pluginsListView_DoubleClick);
            // 
            // Data
            // 
            this.Data.Text = "Available Plugins";
            this.Data.Width = 500;
            // 
            // PropertyGrid
            // 
            this.PropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.PropertyGrid.Name = "PropertyGrid";
            this.PropertyGrid.Size = new System.Drawing.Size(338, 322);
            this.PropertyGrid.TabIndex = 1;
            this.PropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.PropertyGrid_PropertyValueChanged);
            // 
            // OutputListview
            // 
            this.OutputListview.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.Source});
            this.OutputListview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OutputListview.HideSelection = false;
            this.OutputListview.Location = new System.Drawing.Point(0, 25);
            this.OutputListview.Name = "OutputListview";
            this.OutputListview.Size = new System.Drawing.Size(1029, 151);
            this.OutputListview.SmallImageList = this.MainImageList;
            this.OutputListview.TabIndex = 13;
            this.OutputListview.UseCompatibleStateImageBehavior = false;
            this.OutputListview.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Description";
            this.columnHeader1.Width = 525;
            // 
            // Source
            // 
            this.Source.Text = "Source";
            this.Source.Width = 250;
            // 
            // OutputToolStrip
            // 
            this.OutputToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ErrorsToolStripButton,
            this.WarningsToolStripButton,
            this.InfoToolStripButton,
            this.DeleteOutputToolStripButton});
            this.OutputToolStrip.Location = new System.Drawing.Point(0, 0);
            this.OutputToolStrip.Name = "OutputToolStrip";
            this.OutputToolStrip.Size = new System.Drawing.Size(1029, 25);
            this.OutputToolStrip.TabIndex = 12;
            this.OutputToolStrip.Text = "ToolStrip1";
            // 
            // ErrorsToolStripButton
            // 
            this.ErrorsToolStripButton.Checked = true;
            this.ErrorsToolStripButton.CheckOnClick = true;
            this.ErrorsToolStripButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ErrorsToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("ErrorsToolStripButton.Image")));
            this.ErrorsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ErrorsToolStripButton.Name = "ErrorsToolStripButton";
            this.ErrorsToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.ErrorsToolStripButton.ToolTipText = "Show errors";
            this.ErrorsToolStripButton.Click += new System.EventHandler(this.OutputEventsToolStripButton_Click);
            // 
            // WarningsToolStripButton
            // 
            this.WarningsToolStripButton.Checked = true;
            this.WarningsToolStripButton.CheckOnClick = true;
            this.WarningsToolStripButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.WarningsToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("WarningsToolStripButton.Image")));
            this.WarningsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.WarningsToolStripButton.Name = "WarningsToolStripButton";
            this.WarningsToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.WarningsToolStripButton.ToolTipText = "Add ";
            this.WarningsToolStripButton.Click += new System.EventHandler(this.OutputEventsToolStripButton_Click);
            // 
            // InfoToolStripButton
            // 
            this.InfoToolStripButton.Checked = true;
            this.InfoToolStripButton.CheckOnClick = true;
            this.InfoToolStripButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.InfoToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("InfoToolStripButton.Image")));
            this.InfoToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.InfoToolStripButton.Name = "InfoToolStripButton";
            this.InfoToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.InfoToolStripButton.ToolTipText = "Delete ";
            this.InfoToolStripButton.Click += new System.EventHandler(this.OutputEventsToolStripButton_Click);
            // 
            // DeleteOutputToolStripButton
            // 
            this.DeleteOutputToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("DeleteOutputToolStripButton.Image")));
            this.DeleteOutputToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.DeleteOutputToolStripButton.Name = "DeleteOutputToolStripButton";
            this.DeleteOutputToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.DeleteOutputToolStripButton.Click += new System.EventHandler(this.DeleteOutputToolStripButton_Click);
            // 
            // MainDialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1029, 563);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.MainButtonsPanel);
            this.Controls.Add(this.StatusStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainDialogForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
            this.Text = "OpenORM";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainDialogForm_FormClosed);
            this.StatusStrip.ResumeLayout(false);
            this.StatusStrip.PerformLayout();
            this.MainButtonsPanel.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.MainsplitContainer.Panel1.ResumeLayout(false);
            this.MainsplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MainsplitContainer)).EndInit();
            this.MainsplitContainer.ResumeLayout(false);
            this.splitContainerSelectors.Panel1.ResumeLayout(false);
            this.splitContainerSelectors.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerSelectors)).EndInit();
            this.splitContainerSelectors.ResumeLayout(false);
            this.OutputToolStrip.ResumeLayout(false);
            this.OutputToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ImageList TreeImageList;
        internal System.Windows.Forms.StatusStrip StatusStrip;
        internal System.Windows.Forms.ToolStripProgressBar ToolStripProgressBar;
        private System.Windows.Forms.Panel MainButtonsPanel;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer MainsplitContainer;
        private System.Windows.Forms.SplitContainer splitContainerSelectors;
        public System.Windows.Forms.TreeView dbObjectsTreeview;
        internal System.Windows.Forms.PropertyGrid PropertyGrid;
        internal System.Windows.Forms.ListView pluginsListView;
        internal System.Windows.Forms.ColumnHeader Data;
        internal System.Windows.Forms.ListView OutputListview;
        internal System.Windows.Forms.ColumnHeader columnHeader1;
        internal System.Windows.Forms.ColumnHeader Source;
        internal System.Windows.Forms.ToolStrip OutputToolStrip;
        internal System.Windows.Forms.ToolStripButton ErrorsToolStripButton;
        internal System.Windows.Forms.ToolStripButton WarningsToolStripButton;
        internal System.Windows.Forms.ToolStripButton InfoToolStripButton;
        internal System.Windows.Forms.ToolStripButton DeleteOutputToolStripButton;
        internal System.Windows.Forms.ImageList MainImageList;
        public System.Windows.Forms.ImageList MainPluginImageList;
        private System.Windows.Forms.Button btnReload;
        private System.Windows.Forms.Button btnOpenFolder;
    }
}