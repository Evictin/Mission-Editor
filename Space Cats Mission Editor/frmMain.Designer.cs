namespace Space_Cats_Mission_Editor
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileNew = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditCut = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.dlgFolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.dlgFileOpen = new System.Windows.Forms.OpenFileDialog();
            this.dlgFileSave = new System.Windows.Forms.SaveFileDialog();
            this.splitter1 = new System.Windows.Forms.SplitContainer();
            this.menuNodes = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuAddScript = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDeleteScript = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuAddNodeBefore = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAddNodeAfter = new System.Windows.Forms.ToolStripMenuItem();
            this.menuChangeNodeType = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDeleteNode = new System.Windows.Forms.ToolStripMenuItem();
            this.hScroller = new System.Windows.Forms.HScrollBar();
            this.vScroller = new System.Windows.Forms.VScrollBar();
            this.splitter2 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tvAIScripts = new System.Windows.Forms.TreeView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.imgListObjects = new System.Windows.Forms.ImageList(this.components);
            this.statusStrip.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.splitter1.Panel1.SuspendLayout();
            this.splitter1.Panel2.SuspendLayout();
            this.splitter1.SuspendLayout();
            this.menuNodes.SuspendLayout();
            this.splitter2.Panel1.SuspendLayout();
            this.splitter2.Panel2.SuspendLayout();
            this.splitter2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel,
            this.toolStripStatusLabel1});
            this.statusStrip.Location = new System.Drawing.Point(0, 698);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1233, 22);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(39, 17);
            this.statusLabel.Text = "Ready";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1233, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFileNew,
            this.menuFileOpen,
            this.menuFileSave,
            this.menuFileSaveAs,
            this.toolStripMenuItem1,
            this.menuFileExit});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // menuFileNew
            // 
            this.menuFileNew.Name = "menuFileNew";
            this.menuFileNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.menuFileNew.Size = new System.Drawing.Size(155, 22);
            this.menuFileNew.Text = "&New";
            this.menuFileNew.Click += new System.EventHandler(this.menuFileNew_Click);
            // 
            // menuFileOpen
            // 
            this.menuFileOpen.AutoToolTip = true;
            this.menuFileOpen.Name = "menuFileOpen";
            this.menuFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.menuFileOpen.Size = new System.Drawing.Size(155, 22);
            this.menuFileOpen.Text = "&Open...";
            this.menuFileOpen.Click += new System.EventHandler(this.menuFileOpen_Click);
            // 
            // menuFileSave
            // 
            this.menuFileSave.Name = "menuFileSave";
            this.menuFileSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.menuFileSave.Size = new System.Drawing.Size(155, 22);
            this.menuFileSave.Text = "&Save";
            this.menuFileSave.Click += new System.EventHandler(this.menuFileSave_Click);
            // 
            // menuFileSaveAs
            // 
            this.menuFileSaveAs.Name = "menuFileSaveAs";
            this.menuFileSaveAs.Size = new System.Drawing.Size(155, 22);
            this.menuFileSaveAs.Text = "Save &As...";
            this.menuFileSaveAs.Click += new System.EventHandler(this.menuFileSaveAs_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(152, 6);
            // 
            // menuFileExit
            // 
            this.menuFileExit.Name = "menuFileExit";
            this.menuFileExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.menuFileExit.Size = new System.Drawing.Size(155, 22);
            this.menuFileExit.Text = "E&xit";
            this.menuFileExit.Click += new System.EventHandler(this.menuFileExit_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuEditCopy,
            this.menuEditCut,
            this.menuEditDelete,
            this.menuEditPaste});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // menuEditCopy
            // 
            this.menuEditCopy.Name = "menuEditCopy";
            this.menuEditCopy.Size = new System.Drawing.Size(107, 22);
            this.menuEditCopy.Text = "&Copy";
            // 
            // menuEditCut
            // 
            this.menuEditCut.Name = "menuEditCut";
            this.menuEditCut.Size = new System.Drawing.Size(107, 22);
            this.menuEditCut.Text = "C&ut";
            // 
            // menuEditDelete
            // 
            this.menuEditDelete.Name = "menuEditDelete";
            this.menuEditDelete.Size = new System.Drawing.Size(107, 22);
            this.menuEditDelete.Text = "&Delete";
            // 
            // menuEditPaste
            // 
            this.menuEditPaste.Name = "menuEditPaste";
            this.menuEditPaste.Size = new System.Drawing.Size(107, 22);
            this.menuEditPaste.Text = "&Paste";
            // 
            // dlgFileOpen
            // 
            this.dlgFileOpen.DefaultExt = "ai";
            this.dlgFileOpen.Filter = "Mission files (*.ai, *.aic, *.msn)|*.ai;*.aic;*.msn|All files (*.*)|*.*";
            this.dlgFileOpen.Title = "Open File";
            // 
            // dlgFileSave
            // 
            this.dlgFileSave.DefaultExt = "msn";
            this.dlgFileSave.Filter = "Mission files (*.aic, *.msn)|*.aic;*.msn|All files (*.*)|*.*";
            this.dlgFileSave.Title = "Save File As";
            // 
            // splitter1
            // 
            this.splitter1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitter1.Location = new System.Drawing.Point(0, 24);
            this.splitter1.Name = "splitter1";
            // 
            // splitter1.Panel1
            // 
            this.splitter1.Panel1.BackColor = System.Drawing.Color.Beige;
            this.splitter1.Panel1.ContextMenuStrip = this.menuNodes;
            this.splitter1.Panel1.Controls.Add(this.hScroller);
            this.splitter1.Panel1.Controls.Add(this.vScroller);
            this.splitter1.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.splitter1_Panel1_Paint);
            this.splitter1.Panel1.ClientSizeChanged += new System.EventHandler(this.vScroller_ValueChanged);
            // 
            // splitter1.Panel2
            // 
            this.splitter1.Panel2.Controls.Add(this.splitter2);
            this.splitter1.Size = new System.Drawing.Size(1233, 674);
            this.splitter1.SplitterDistance = 1034;
            this.splitter1.TabIndex = 2;
            // 
            // menuNodes
            // 
            this.menuNodes.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAddScript,
            this.menuDeleteScript,
            this.toolStripSeparator1,
            this.menuAddNodeBefore,
            this.menuAddNodeAfter,
            this.menuChangeNodeType,
            this.menuDeleteNode});
            this.menuNodes.Name = "menuNodes";
            this.menuNodes.Size = new System.Drawing.Size(186, 164);
            this.menuNodes.Opening += new System.ComponentModel.CancelEventHandler(this.menuNodes_Opening);
            // 
            // menuAddScript
            // 
            this.menuAddScript.Name = "menuAddScript";
            this.menuAddScript.Size = new System.Drawing.Size(185, 22);
            this.menuAddScript.Text = "Add AI Script";
            this.menuAddScript.Click += new System.EventHandler(this.menuAddScript_Click);
            // 
            // menuDeleteScript
            // 
            this.menuDeleteScript.Enabled = false;
            this.menuDeleteScript.Name = "menuDeleteScript";
            this.menuDeleteScript.Size = new System.Drawing.Size(185, 22);
            this.menuDeleteScript.Text = "Delete AI Script";
            this.menuDeleteScript.Click += new System.EventHandler(this.menuDeleteScript_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(182, 6);
            // 
            // menuAddNodeBefore
            // 
            this.menuAddNodeBefore.Enabled = false;
            this.menuAddNodeBefore.Name = "menuAddNodeBefore";
            this.menuAddNodeBefore.Size = new System.Drawing.Size(185, 22);
            this.menuAddNodeBefore.Text = "Add Node Before...";
            this.menuAddNodeBefore.Click += new System.EventHandler(this.menuAddNodeBefore_Click);
            // 
            // menuAddNodeAfter
            // 
            this.menuAddNodeAfter.Enabled = false;
            this.menuAddNodeAfter.Name = "menuAddNodeAfter";
            this.menuAddNodeAfter.Size = new System.Drawing.Size(185, 22);
            this.menuAddNodeAfter.Text = "Add Node After...";
            this.menuAddNodeAfter.Click += new System.EventHandler(this.menuAddNodeAfter_Click);
            // 
            // menuChangeNodeType
            // 
            this.menuChangeNodeType.Enabled = false;
            this.menuChangeNodeType.Name = "menuChangeNodeType";
            this.menuChangeNodeType.Size = new System.Drawing.Size(185, 22);
            this.menuChangeNodeType.Text = "Change Node Type...";
            // 
            // menuDeleteNode
            // 
            this.menuDeleteNode.Enabled = false;
            this.menuDeleteNode.Name = "menuDeleteNode";
            this.menuDeleteNode.Size = new System.Drawing.Size(185, 22);
            this.menuDeleteNode.Text = "Delete Node";
            this.menuDeleteNode.Click += new System.EventHandler(this.menuDeleteNode_Click);
            // 
            // hScroller
            // 
            this.hScroller.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.hScroller.Location = new System.Drawing.Point(0, 653);
            this.hScroller.Minimum = -100;
            this.hScroller.Name = "hScroller";
            this.hScroller.Size = new System.Drawing.Size(1013, 17);
            this.hScroller.TabIndex = 2;
            this.hScroller.ValueChanged += new System.EventHandler(this.vScroller_ValueChanged);
            // 
            // vScroller
            // 
            this.vScroller.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScroller.Location = new System.Drawing.Point(1013, 0);
            this.vScroller.Minimum = -100;
            this.vScroller.Name = "vScroller";
            this.vScroller.Size = new System.Drawing.Size(17, 670);
            this.vScroller.TabIndex = 1;
            this.vScroller.ValueChanged += new System.EventHandler(this.vScroller_ValueChanged);
            // 
            // splitter2
            // 
            this.splitter2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitter2.Location = new System.Drawing.Point(0, 0);
            this.splitter2.Name = "splitter2";
            this.splitter2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitter2.Panel1
            // 
            this.splitter2.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitter2.Panel2
            // 
            this.splitter2.Panel2.Controls.Add(this.propertyGrid1);
            this.splitter2.Size = new System.Drawing.Size(195, 674);
            this.splitter2.SplitterDistance = 309;
            this.splitter2.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(191, 305);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tvAIScripts);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(183, 279);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "AI Scripts";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tvAIScripts
            // 
            this.tvAIScripts.ContextMenuStrip = this.menuNodes;
            this.tvAIScripts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvAIScripts.HideSelection = false;
            this.tvAIScripts.Location = new System.Drawing.Point(3, 3);
            this.tvAIScripts.Name = "tvAIScripts";
            this.tvAIScripts.Size = new System.Drawing.Size(177, 273);
            this.tvAIScripts.TabIndex = 0;
            this.tvAIScripts.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvAIScripts_AfterSelect);
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(183, 279);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Objects";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(183, 279);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Mission Script";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.LineColor = System.Drawing.SystemColors.ControlLight;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(191, 357);
            this.propertyGrid1.TabIndex = 3;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
            // 
            // imgListObjects
            // 
            this.imgListObjects.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgListObjects.ImageStream")));
            this.imgListObjects.TransparentColor = System.Drawing.Color.White;
            this.imgListObjects.Images.SetKeyName(0, "EnemyShip1.png");
            this.imgListObjects.Images.SetKeyName(1, "Asteroid7.png");
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1233, 720);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmMain";
            this.Text = "Space Cats Mission Editor";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.frmMain_Paint);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitter1.Panel1.ResumeLayout(false);
            this.splitter1.Panel2.ResumeLayout(false);
            this.splitter1.ResumeLayout(false);
            this.menuNodes.ResumeLayout(false);
            this.splitter2.Panel1.ResumeLayout(false);
            this.splitter2.Panel2.ResumeLayout(false);
            this.splitter2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuFileNew;
        private System.Windows.Forms.ToolStripMenuItem menuFileOpen;
        private System.Windows.Forms.ToolStripMenuItem menuFileSave;
        private System.Windows.Forms.ToolStripMenuItem menuFileSaveAs;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem menuFileExit;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuEditCopy;
        private System.Windows.Forms.ToolStripMenuItem menuEditCut;
        private System.Windows.Forms.ToolStripMenuItem menuEditPaste;
        private System.Windows.Forms.ToolStripMenuItem menuEditDelete;
        private System.Windows.Forms.FolderBrowserDialog dlgFolderBrowser;
        private System.Windows.Forms.OpenFileDialog dlgFileOpen;
        private System.Windows.Forms.SaveFileDialog dlgFileSave;
        private System.Windows.Forms.SplitContainer splitter1;
        private System.Windows.Forms.ImageList imgListObjects;
        private System.Windows.Forms.SplitContainer splitter2;
        private System.Windows.Forms.HScrollBar hScroller;
        private System.Windows.Forms.VScrollBar vScroller;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.TreeView tvAIScripts;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ContextMenuStrip menuNodes;
        private System.Windows.Forms.ToolStripMenuItem menuAddNodeBefore;
        private System.Windows.Forms.ToolStripMenuItem menuAddNodeAfter;
        private System.Windows.Forms.ToolStripMenuItem menuChangeNodeType;
        private System.Windows.Forms.ToolStripMenuItem menuDeleteNode;
        private System.Windows.Forms.ToolStripMenuItem menuAddScript;
        private System.Windows.Forms.ToolStripMenuItem menuDeleteScript;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}

