using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.IO;
using Space_Cats_V1._2;

namespace Space_Cats_Mission_Editor
{
    public partial class frmMain : Form
    {
        const int MISSIONFILEID = 12;
        int z_missionID;
        string z_filename, z_pathname;
        string FileName
        {
            get { return z_filename; }
        }
        string PathName
        {
            get { return z_pathname; }
            set
            {
                string[] pathParts;
                if (value == null)
                {
                    z_pathname = z_filename = null;
                    menuFileSave.Enabled = false;
                    Text = "";
                }
                else
                {
                    z_pathname = value;
                    pathParts = value.Split('\\');
                    z_filename = pathParts[pathParts.Count() - 1];
                    menuFileSave.Enabled = true;
                    Text = FileName + " - ";
                }
                Text += "Space Cats! Mission Editor";
            }
        }
        bool z_modified;
        bool modified
        {
            get { return z_modified; }
            set
            {
                if (value == false)
                {
                    z_modified = false;
                    PathName = PathName;
                }
                else
                {
                    if (!modified)
                    {
                        z_modified = true;
                        Text += " (Modified)";
                    }
                }
            }
        }
        bool z_unCompressed;
        List<GameObject> objectList;
        System.Drawing.Point offset;
        System.Drawing.Rectangle screenRect;
        Microsoft.Xna.Framework.Rectangle viewport;
        private Bitmap offscreen;
        private Vector2 originalLocation;
        private bool didMouseDown;
        private bool isDragging;
        private System.Drawing.Point coords;
        private System.Drawing.Point MouseCoords
        {
            get { return coords; }
            set
            {
                coords = value;
                this.mouseCoordsLabel.Text = coords.ToString();
            }
        }
        private object menuOpener;

        private AI_Script SelectedAI
        {
            get 
            {
                if (tvAIScripts.SelectedNode == null)
                    return null;
                else if (tvAIScripts.SelectedNode.Tag is AI_Script)
                    return (AI_Script)tvAIScripts.SelectedNode.Tag;
                else if (tvAIScripts.SelectedNode.Tag is AI_ScriptNode)
                {
                    return (AI_Script)tvAIScripts.SelectedNode.Parent.Tag;
                }
                else
                    return null;
            }
        }
        private AI_ScriptNode SelectedAIScriptNode
        {
            get
            {
                if (tvAIScripts.SelectedNode == null)
                    return null;
                else if (tvAIScripts.SelectedNode.Tag is AI_ScriptNode)
                    return (AI_ScriptNode)tvAIScripts.SelectedNode.Tag;
                else 
                    return null;
            }
        }

        public frmMain()
        {
            InitializeComponent();
            offscreen = new Bitmap(1200, 1000);
        }

        private void NewFile()
        {
            MissionScriptNode missionNode;
            TreeNode node;
            objectList = new List<GameObject>();

            screenRect = new System.Drawing.Rectangle(0, 0, 800, 600);
            viewport = new Microsoft.Xna.Framework.Rectangle(0, 0, 800, 600);
            PathName =  null;
            modified = false;
            offset = new System.Drawing.Point(200,200);
            z_missionID = 1;
            z_unCompressed = false;
            this.propertyGrid1.SelectedObject = null;
            tvAIScripts.Nodes.Clear();
            tvMissionScript.Nodes.Clear();
            isDragging = false;
            didMouseDown = false;
            missionNode = new MS_Start();
            node = new TreeNode(missionNode.ToString());
            node.Tag = missionNode;
            tvMissionScript.Nodes.Add(node);
            missionNode = new MS_End(180000);
            node = new TreeNode(missionNode.ToString());
            node.Tag = missionNode;
            tvMissionScript.Nodes.Add(node);
        }

        private void enableNodeMenuItems()
        {
            if (SelectedAI != null)
            {
                menuDeleteScript.Enabled = (SelectedAIScriptNode == null);
                if (SelectedAIScriptNode != null)
                {
                    menuDeleteNode.Enabled = (tvAIScripts.SelectedNode.Index > 0 && tvAIScripts.SelectedNode.Index < tvAIScripts.SelectedNode.Parent.Nodes.Count - 1);
                    menuAddNodeBefore.Enabled = (tvAIScripts.SelectedNode.Index > 0);
                    menuAddNodeAfter.Enabled = (tvAIScripts.SelectedNode.Index < tvAIScripts.SelectedNode.Parent.Nodes.Count - 1);
                }
                else
                {
                    menuDeleteNode.Enabled = false;
                    menuAddNodeBefore.Enabled = false;
                    menuAddNodeAfter.Enabled = false;
                }
            }
            else
            {
                menuDeleteScript.Enabled = false;
                menuDeleteNode.Enabled = false;
                menuChangeNodeType.Enabled = false;
                menuAddNodeAfter.Enabled = false;
                menuAddNodeBefore.Enabled = false;
            }
        }

        private void enableScriptEventMenuItems()
        {
            if (tvMissionScript.SelectedNode != null)
            {
                menuDeleteScriptEvent.Enabled = (tvMissionScript.SelectedNode.Index > 0 && tvMissionScript.SelectedNode.Index < tvMissionScript.Nodes.Count - 1);
                menuInsertScriptEvent.Enabled = (tvMissionScript.SelectedNode.Index > 0);
                menuAddScriptEvent.Enabled = (tvMissionScript.SelectedNode.Index < tvMissionScript.Nodes.Count - 1);
            }
            else
            {
                menuDeleteScriptEvent.Enabled = false;
                menuAddScriptEvent.Enabled = false;
                menuInsertScriptEvent.Enabled = false;
            }
        }

        private void menuFileSave_Click(object sender, EventArgs e)
        {
            if (PathName == null || z_unCompressed)
                menuFileSaveAs_Click(sender, e);
            else
                writeFile();
        }

        private void menuFileSaveAs_Click(object sender, EventArgs e)
        {
            if (dlgFileSave.ShowDialog()==DialogResult.OK)
            {
                    PathName = dlgFileSave.FileName;
                    writeFile();
            }
        }

        private void menuFileExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuFileOpen_Click(object sender, EventArgs e)
        {
            StreamReader sr = null;
            BinaryReader br = null;
            FormClosingEventArgs eArgs = new FormClosingEventArgs(CloseReason.None, false);
            int ID;

            this.frmMain_FormClosing(this, eArgs);
            if (!eArgs.Cancel)
            {
                switch (dlgFileOpen.ShowDialog())
                {
                    case DialogResult.OK:
                        // reset everything
                        NewFile();
                        // get the filename
                        PathName = dlgFileOpen.FileName;
                        try
                        {
                            br = new BinaryReader(File.OpenRead(PathName));
                            ID = br.ReadInt32();
                            if (ID != frmMain.MISSIONFILEID)
                            {
                                br.Close();
                                sr = File.OpenText(PathName);
                                readFile(sr);
                            }
                            else
                            {
                                readFile(br);
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Could not open \"" + PathName + "\".", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            if (br != null)
                                br.Close();
                            if (sr != null)
                                sr.Close();
                        }
                        break;
                }
            }
        }

        private void readFile(StreamReader sr)
        {
            string input;
            string[] words;
           
            while (!sr.EndOfStream)
            {
                words = sr.ReadLine().Split(' ');
                if (words[0].CompareTo("VIEWPORT") == 0)
                {
                    screenRect.Width = int.Parse(words[1]);
                    screenRect.Height = int.Parse(words[2]);
                    viewport = new Microsoft.Xna.Framework.Rectangle(0, 0, screenRect.Width, screenRect.Height);
                }
                else if (words[0].CompareTo("AI") == 0)
                {
                    if (words[1].CompareTo("SCRIPT") == 0)
                    {
                        AddAIScriptToTree(new AI_Script(viewport, sr));
                    }
                }
                else if (words[0].CompareTo("EOF") == 0)
                    break;
            }
            z_unCompressed = true;
        }

        private void readFile(BinaryReader br)
        {
            string input;
            viewport = new Microsoft.Xna.Framework.Rectangle();

            screenRect.Width = br.ReadInt32();
            screenRect.Height = br.ReadInt32();
            viewport = new Microsoft.Xna.Framework.Rectangle(0, 0, screenRect.Width, screenRect.Height);
            do
            {
                input = br.ReadString();
                if (input.CompareTo("AI_SCRIPT") == 0)
                {
                    AddAIScriptToTree(new AI_Script(viewport, br));
                }
                if (input.CompareTo("MISSION_SCRIPT") == 0)
                {
                    AddMissionScriptToTree(br);
                }
            } while (input.CompareTo("EOF") != 0);
        }

        private void writeFile()
        {
            BinaryWriter bw=null;
            int i;
            try
            {
                bw = new BinaryWriter(File.Create(PathName));
                bw.Write(MISSIONFILEID);
                bw.Write(screenRect.Width);
                bw.Write(screenRect.Height);
                foreach (TreeNode node in tvAIScripts.Nodes)
                {
                    ((AI_Script)node.Tag).writeAIToFile(bw);
                }
                bw.Write("MISSION_SCRIPT");
                foreach (TreeNode node in tvMissionScript.Nodes)
                {
                    ((MissionScriptNode)node.Tag).WriteToFile(bw);
                }
                bw.Write("EOF");
                z_unCompressed = false;
                modified = false;
            }
            catch
            {
                MessageBox.Show("Could not write to \"" + PathName + "\".", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (bw != null)
                    bw.Close();
            }
        }

        private void AddMissionScriptToTree(BinaryReader br)
        {
            MissionScriptNode node;
            TreeNode xnode;
            tvMissionScript.Nodes.Clear();
            do
            {
                node = MissionScriptNode.readNodeFromFile(br);
                xnode = new TreeNode(node.ToString());
                xnode.Tag = node;
                tvMissionScript.Nodes.Add(xnode);
            } while (node.Command != MissionScriptNode.CommandID.End);
        }

        private void AddAIScriptToTree(AI_Script ai)
        {
            int i;
            TreeNode subNode;
            TreeNode scriptNode = new TreeNode("Script " + ai.ID);
            scriptNode.Tag = ai;
            for (i = 0; i < ai.Count; i++)
            {
                subNode = new TreeNode(ai.getNode(i).ToString());
                subNode.Tag = ai.getNode(i);
                scriptNode.Nodes.Add(subNode);
            }
            tvAIScripts.Nodes.Add(scriptNode);
        }

        private void dlgFileOpen_FileOk(object sender, CancelEventArgs e)
        {
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            NewFile();
        }

        private void redrawAIScripts()
        {
            Graphics gr = Graphics.FromImage(offscreen);
            gr.Clear(Color.Beige);
            System.Drawing.Rectangle r = screenRect;
            r.Offset(offset);
            gr.FillRectangle(Brushes.Black, r);
            foreach (TreeNode script in tvAIScripts.Nodes)
            {
                if (script.Tag is AI_Script)
                {
                    drawAIScript(gr, (AI_Script)script.Tag);
                }
            }
            if (SelectedAI != null)
            {
                drawAIScript(gr, (AI_Script)SelectedAI);
            }
            gr.Dispose();
        }

        private void splitter1_Panel1_Paint(object sender, PaintEventArgs e)
        {
            screen.Invalidate();
        }

        private void drawAIScript(Graphics gr, AI_Script script)
        {
            int i;
            Pen pen = Pens.DarkSlateBlue;
            Brush brush = Brushes.DarkSlateBlue;
            Vector2 current = Vector2.Zero;
            Vector2 temp = Vector2.Zero;
            AI_ScriptNode node;

            for (i = 0; i < script.Count; i++)
            {
                if (script == SelectedAI)
                {
                    pen = new Pen(Color.GreenYellow, 2);
                    brush = Brushes.GreenYellow;
                }
                if (script.getNode(i) == SelectedAIScriptNode)
                    temp = current;
                drawScriptSegment(gr, brush, pen, ref current, script.getNode(i));
            }
            // if a node is selected and we are drawing the selected script
            if (SelectedAIScriptNode != null)
            {
                if (script == SelectedAI)
                    drawScriptSegment(gr, Brushes.Red, new Pen(Color.Red, 2), ref temp, SelectedAIScriptNode);
            }
        }

        private void drawScriptSegment(Graphics gr, Brush brush, Pen pen, ref Vector2 current, AI_ScriptNode node)
        {
            Vector2 temp, center;
            bool drawDot = true;
            switch (node.getCommand())
            {
                case AI_ScriptNode.ID_START_POINT:
                    current = node.getLocation();
                    break;
                case AI_ScriptNode.ID_MOVE_TO:
                case AI_ScriptNode.ID_JUMP_TO:
                case AI_ScriptNode.ID_STAGE_POINT_TO_POINT:
                    gr.DrawLine(pen, offset.X + current.X, offset.Y + current.Y, offset.X + node.getLocation().X, offset.Y + node.getLocation().Y);
                    current = node.getLocation();
                    break;
                case AI_ScriptNode.ID_ARC_TO:
                    while (node.GetDistanceTo(current) > node.getSpeed())
                    {
                        temp = current + node.CreateVectorFrom(current);
                        gr.DrawLine(pen, offset.X + current.X, offset.Y + current.Y, offset.X + temp.X, offset.Y + temp.Y);
                        current = temp;
                    }
                    node.reset();
                    break;
                case AI_ScriptNode.ID_STAGE_HORIZONTAL:
                    gr.DrawLine(pen, offset.X + current.X - ((AI_Stage)node).getSwingDistance(), offset.Y + current.Y,
                        offset.X + current.X + ((AI_Stage)node).getSwingDistance(), offset.Y + current.Y);
                    break;
                case AI_ScriptNode.ID_STAGE_VERTICAL:
                    gr.DrawLine(pen, offset.X+ current.X, offset.Y + current.Y - ((AI_Stage)node).getSwingDistance(),
                        offset.X + current.X, offset.Y + current.Y + ((AI_Stage)node).getSwingDistance());
                    break;
                case AI_ScriptNode.ID_STAGE_CIRCLE:
                    center = current;
                    for (float theta = 0f; theta < 3 * Math.PI; theta += node.getSpeed() / ((AI_Stage)node).getSwingDistance())
                    {
                        temp = current + node.CreateVectorFrom(current);
                        gr.DrawLine(pen, offset.X + current.X, offset.Y + current.Y, offset.X + temp.X, offset.Y + temp.Y);
                        current = temp;
                    }
                    // reset current to the center after drawing this one
                    current = center;
                    node.reset();
                    break;
                // nothing to do or draw for these...
                case AI_ScriptNode.ID_RESET:
                case AI_ScriptNode.ID_WAIT:
                case AI_ScriptNode.ID_END_POINT:
                case AI_ScriptNode.ID_NULL:
                default:
                    drawDot = false;
                    break;
            }
            if (drawDot)
                gr.FillEllipse(brush, offset.X + current.X - 5, offset.Y + current.Y - 5, 10, 10);
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result;
            if (modified)
            {
                result = MessageBox.Show("File \"" + (FileName==null?"Unnamed":FileName) + "\" has changed. Save Changes?","Save Changes?",
                    MessageBoxButtons.YesNoCancel,MessageBoxIcon.Warning);
                switch (result)
                {
                    case DialogResult.Yes:
                        this.menuFileSave_Click(this, null);
                        if (modified)
                            e.Cancel = true;
                        break;
                    case DialogResult.No:
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }

            }
        }

        private void menuFileNew_Click(object sender, EventArgs e)
        {
            FormClosingEventArgs eArgs = new FormClosingEventArgs(CloseReason.None, false);
            this.frmMain_FormClosing(this, eArgs);
            if (!eArgs.Cancel)
            {
                NewFile();
            }
        }

        private void tvAIScripts_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            IArtificialIntelligence ai;
            propertyGrid1.SelectedObject = tvAIScripts.SelectedNode.Tag;
            screen.Invalidate();
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (propertyGrid1.SelectedObject is AI_Script)
            {
                tvAIScripts.SelectedNode.Text = "Script " + ((AI_Script)propertyGrid1.SelectedObject).ID;
            }
            else if (propertyGrid1.SelectedObject is AI_ScriptNode)
            {
                tvAIScripts.SelectedNode.Text = ((AI_ScriptNode)propertyGrid1.SelectedObject).ToString();
                screen.Invalidate();
            }
            else if (propertyGrid1.SelectedObject is MissionScriptNode)
            {
                tvMissionScript.SelectedNode.Text = ((MissionScriptNode)propertyGrid1.SelectedObject).ToString();
            }
            modified = true;
        }

        private void frmMain_Paint(object sender, PaintEventArgs e)
        {

        }

        private void menuNodes_Opening(object sender, CancelEventArgs e)
        {
            enableNodeMenuItems();
        }

        private void menuDeleteScript_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete the selected script?\n(This cannot be undone.)",
                "Delete AI Script", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                tvAIScripts.Nodes.Remove(tvAIScripts.SelectedNode);
                propertyGrid1.SelectedObject = (tvAIScripts.SelectedNode == null ? null : tvAIScripts.SelectedNode.Tag);
                modified = true;
                screen.Invalidate();
            }
        }

        private void menuDeleteNode_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete the selected script node?\n(This cannot be undone.)",
                "Delete AI Script Node", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                SelectedAI.RemoveAt(tvAIScripts.SelectedNode.Index);
                tvAIScripts.Nodes.Remove(tvAIScripts.SelectedNode);
                propertyGrid1.SelectedObject = (tvAIScripts.SelectedNode==null?null:tvAIScripts.SelectedNode.Tag);
                modified = true;
                screen.Invalidate();
            }
        }

        private void menuAddScript_Click(object sender, EventArgs e)
        {
            AI_Script script = new AI_Script(viewport);
            if (tvAIScripts.Nodes.Count > 0)
                script.ID = ((AI_Script)tvAIScripts.Nodes[tvAIScripts.Nodes.Count - 1].Tag).ID + 1;
            else
                script.ID = 1;
            if (menuOpener == screen)
                script.Add(new AI_StartPoint(new Vector2((float)coords.X, (float)coords.Y)));
            else
                script.Add(new AI_StartPoint(Vector2.Zero));
            script.Add(new AI_EndPoint());
            AddAIScriptToTree(script);
            tvAIScripts.SelectedNode = tvAIScripts.Nodes[tvAIScripts.Nodes.Count - 1].Nodes[0];
            modified = true;
            screen.Invalidate();
        }

        private void menuAddNodeAfter_Click(object sender, EventArgs e)
        {
            TreeNode node;
            frmNewScriptNode nodeForm = new frmNewScriptNode();
            if (menuOpener == screen)
            {
                nodeForm.Node.Location = new Vector2((float)MouseCoords.X, (float)MouseCoords.Y);
            }
            else
                nodeForm.Node.Location = SelectedAIScriptNode.Location;
            ((AI_MoveTo)nodeForm.Node).Speed = SelectedAIScriptNode.getSpeed();
            nodeForm.cmbNodeType.SelectedItem = "MoveTo";
            if (nodeForm.ShowDialog(this) == DialogResult.OK)
            {
                SelectedAI.Insert(tvAIScripts.SelectedNode.Index + 1, nodeForm.Node);
                node = new TreeNode(nodeForm.Node.ToString());
                node.Tag = nodeForm.Node;
                tvAIScripts.SelectedNode.Parent.Nodes.Insert(tvAIScripts.SelectedNode.Index + 1, node);
                tvAIScripts.SelectedNode = node;
                propertyGrid1.SelectedObject = tvAIScripts.SelectedNode.Tag;
                modified = true;
                screen.Invalidate();
            }
        }

        private void menuAddNodeBefore_Click(object sender, EventArgs e)
        {
            TreeNode node;
            frmNewScriptNode nodeForm = new frmNewScriptNode();
            if (menuOpener == screen)
            {
                nodeForm.Node.Location = new Vector2((float)MouseCoords.X, (float)MouseCoords.Y);
            }
            else
                nodeForm.Node.Location = SelectedAIScriptNode.Location;
            ((AI_MoveTo)nodeForm.Node).Speed = SelectedAIScriptNode.getSpeed();
            nodeForm.cmbNodeType.SelectedItem = "MoveTo";
            if (nodeForm.ShowDialog(this) == DialogResult.OK)
            {
                SelectedAI.Insert(tvAIScripts.SelectedNode.Index, nodeForm.Node);
                node = new TreeNode(nodeForm.Node.ToString());
                node.Tag = nodeForm.Node;
                tvAIScripts.SelectedNode.Parent.Nodes.Insert(tvAIScripts.SelectedNode.Index, node);
                tvAIScripts.SelectedNode = node;
                propertyGrid1.SelectedObject = tvAIScripts.SelectedNode.Tag;
                modified = true;
                screen.Invalidate();
            }
        }

        private void screen_Paint(object sender, PaintEventArgs e)
        {
            if (offscreen == null)
            {
                return; // Nothing to do   
            }
            else
            {
                if (this.tabControl1.SelectedTab.Text.CompareTo("AI Scripts") == 0)
                    redrawAIScripts();
                else if (this.tabControl1.SelectedTab.Text.CompareTo("Mission Script") == 0)
                {
                    // insert mission drawing code here
                }
                e.Graphics.DrawImage(offscreen, new System.Drawing.Point(0, 0));
            }   

        }

        private int Distance(int x1, int y1, int x2, int y2)
        {
            return (int)Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        }

        private AI_ScriptNode getAIScriptNodeAtPoint()
        {
            AI_ScriptNode scriptNode;
            // check the currently selected script first, if any
            if (SelectedAI != null)
            {
                foreach (AI_ScriptNode node in SelectedAI.Nodes)
                {
                    if (node is IAI_HasNoLocation)
                        continue;
                    if (Distance(coords.X, coords.Y, (int)node.Location.X, (int)node.Location.Y) <= 5)
                        return node;
                }
            }
            foreach (TreeNode script in tvAIScripts.Nodes)
            {
                foreach (TreeNode xnode in script.Nodes)
                {
                    scriptNode = (AI_ScriptNode)xnode.Tag;
                    if (scriptNode is AI_EndPoint || scriptNode is AI_Kamikaze || scriptNode is AI_Reset || scriptNode is AI_Wait || scriptNode is AI_StageCircle || scriptNode is AI_StageHorizontal || scriptNode is AI_StageVertical)
                        continue;
                    if (Distance(coords.X, coords.Y, (int)scriptNode.Location.X, (int)scriptNode.Location.Y) <= 5)
                        return scriptNode;
                }
            }
            return null;
        }

        private void screen_MouseMove(object sender, MouseEventArgs e)
        {
            MouseCoords = new System.Drawing.Point(e.Location.X - offset.X, e.Location.Y - offset.Y);
            if (tabControl1.SelectedTab.Text.CompareTo("AI Scripts") == 0)
            {
                if (e.Button == MouseButtons.None)
                {
                    if (SelectedAIScriptNode != null)
                    {
                        if (getAIScriptNodeAtPoint() != null)
                            screen.Cursor = Cursors.Hand;
                        else
                            screen.Cursor = Cursors.Default;
                    }
                }
                else if (didMouseDown && e.Button == MouseButtons.Left)
                {
                    if (SelectedAIScriptNode != null)
                    {
                        if (!isDragging)
                        {
                            originalLocation = SelectedAIScriptNode.Location;
                            isDragging = true;
                        }
                        SelectedAIScriptNode.Location = new Vector2((float)coords.X, (float)coords.Y);
                        screen.Invalidate();
                    }
                }
            }
        }

        private void screen_MouseDown(object sender, MouseEventArgs e)
        {
            MouseCoords = new System.Drawing.Point(e.Location.X - offset.X, e.Location.Y - offset.Y);
            if (tabControl1.SelectedTab.Text.CompareTo("AI Scripts") == 0)
            {
                if (e.Button == MouseButtons.Left && SelectedAIScriptNode != null)
                {
                    if (Distance(coords.X, coords.Y, (int)SelectedAIScriptNode.Location.X, (int)SelectedAIScriptNode.Location.Y) <= 5)
                    {
                        didMouseDown = true;
                        screen.Cursor = Cursors.SizeAll;
                    }
                }
            }
        }

        private void screen_MouseUp(object sender, MouseEventArgs e)
        {
            MouseCoords = new System.Drawing.Point(e.Location.X - offset.X, e.Location.Y - offset.Y);
            if (tabControl1.SelectedTab.Text.CompareTo("AI Scripts") == 0)
            {
                if (isDragging && SelectedAIScriptNode != null)
                {
                    isDragging = false;
                    didMouseDown = false;
                    modified = true;
                    tvAIScripts.SelectedNode.Text = SelectedAIScriptNode.ToString();
                    propertyGrid1.SelectedObject = SelectedAIScriptNode;
                }
            }
        }

        private void screen_MouseClick(object sender, MouseEventArgs e)
        {
            AI_ScriptNode node;
            if (tabControl1.SelectedTab.Text.CompareTo("AI Scripts") == 0)
            {
                node = getAIScriptNodeAtPoint();
                if (node != null)
                {
                    foreach (TreeNode script in tvAIScripts.Nodes)
                    {
                        foreach (TreeNode xnode in script.Nodes)
                        {
                            if (xnode.Tag == node)
                            {
                                tvAIScripts.SelectedNode = xnode;
                                return;
                            }
                        }
                    }
                }
            }
        }

        private void screen_MouseLeave(object sender, EventArgs e)
        {
            if (isDragging)
            {
                SelectedAIScriptNode.Location = originalLocation;
                isDragging = false;
                didMouseDown = false;
                screen.Invalidate();
            }
        }

        private void menuNodes_Opened(object sender, EventArgs e)
        {
            menuOpener = ((ContextMenuStrip)sender).SourceControl;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab.Text.CompareTo("AI Scripts") == 0)
            {
                screen.ContextMenuStrip = menuNodes;
                propertyGrid1.SelectedObject = (tvAIScripts.SelectedNode == null ? null : tvAIScripts.SelectedNode.Tag);
            }
            else if (tabControl1.SelectedTab.Text.CompareTo("Mission Script") == 0)
            {
                screen.ContextMenuStrip = menuScript;
                propertyGrid1.SelectedObject = (tvMissionScript.SelectedNode == null ? null : tvMissionScript.SelectedNode.Tag);
            }
        }

        private void tvMissionScript_AfterSelect(object sender, TreeViewEventArgs e)
        {
            propertyGrid1.SelectedObject = (tvMissionScript.SelectedNode == null ? null : tvMissionScript.SelectedNode.Tag);
        }

        private void menuDeleteScriptEvent_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete the selected script event?\n(This cannot be undone.)",
                "Delete Mission Script Event", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                tvMissionScript.Nodes.Remove(tvMissionScript.SelectedNode);
                tvMissionScript.Refresh();
                propertyGrid1.SelectedObject = (tvMissionScript.SelectedNode == null ? null : tvMissionScript.SelectedNode.Tag);
                modified = true;
                screen.Invalidate();
            }
        }

        private void menuInsertScriptEvent_Click(object sender, EventArgs e)
        {
            TreeNode xnode;
            frmNewMissionScriptEvent eventForm = new frmNewMissionScriptEvent();
            if (tvMissionScript.SelectedNode != null)
                eventForm.Node.TimeStamp = ((MissionScriptNode)tvMissionScript.SelectedNode.Tag).TimeStamp - 1;
            else
                eventForm.Node.TimeStamp = 1;
            if (eventForm.ShowDialog() == DialogResult.OK)
            {
                xnode = new TreeNode(eventForm.Node.ToString());
                xnode.Tag = eventForm.Node;
                tvMissionScript.Nodes.Insert(tvMissionScript.SelectedNode.Index, xnode);
                tvMissionScript.SelectedNode = xnode;
                propertyGrid1.SelectedObject = tvMissionScript.SelectedNode;
            }

        }

        private void menuAddScriptEvent_Click(object sender, EventArgs e)
        {

            TreeNode xnode;
            frmNewMissionScriptEvent eventForm = new frmNewMissionScriptEvent();
            if (tvMissionScript.SelectedNode != null)
                eventForm.Node.TimeStamp = ((MissionScriptNode)tvMissionScript.SelectedNode.Tag).TimeStamp + 1;
            else
                eventForm.Node.TimeStamp = 1;
            if (eventForm.ShowDialog() == DialogResult.OK)
            {
                xnode = new TreeNode(eventForm.Node.ToString());
                xnode.Tag = eventForm.Node;
                tvMissionScript.Nodes.Insert(tvMissionScript.SelectedNode.Index+1, xnode);
                tvMissionScript.SelectedNode = xnode;
                propertyGrid1.SelectedObject = tvMissionScript.SelectedNode;
            }
        }

        private void menuScript_Opening(object sender, CancelEventArgs e)
        {
            enableScriptEventMenuItems();
        }

    }
}