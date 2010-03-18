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
        List<IArtificialIntelligence> aiList;
        List<GameObject> objectList;
        System.Drawing.Point offset;
        System.Drawing.Rectangle screenRect;
        Microsoft.Xna.Framework.Rectangle viewport;
        int selectedScript, selectedNode;

        public frmMain()
        {
            InitializeComponent();
        }

        private void NewFile()
        {
            objectList = new List<GameObject>();
            aiList = new List<IArtificialIntelligence>();

            screenRect = new System.Drawing.Rectangle(0, 0, 800, 600);
            viewport = new Microsoft.Xna.Framework.Rectangle(0, 0, 800, 600);
            PathName =  null;
            modified = false;
            hScroller.Value = vScroller.Value = 0;
            hScroller.Minimum = vScroller.Minimum = -200;
            hScroller.Maximum = vScroller.Maximum = 200;
            offset = new System.Drawing.Point(this.splitter1.Panel1.ClientSize.Width / 2 - 400 - hScroller.Value,
                this.splitter1.Panel1.ClientSize.Height / 2 - 300 - vScroller.Value);
            z_missionID = 1;
            z_unCompressed = false;
            this.propertyGrid1.SelectedObject = null;
            tvAIScripts.Nodes.Clear();
            selectedScript = selectedNode = -1;
        }

        private void enableNodeMenuItems()
        {
            if (selectedScript != -1)
            {
                menuDeleteScript.Enabled = (selectedNode == -1);
                if (selectedNode != -1)
                {
                    menuDeleteNode.Enabled = (selectedNode > 0 && selectedNode < tvAIScripts.SelectedNode.Parent.Nodes.Count - 1);
                    menuAddNodeBefore.Enabled = (selectedNode > 0);
                    menuAddNodeAfter.Enabled = (selectedNode < tvAIScripts.SelectedNode.Parent.Nodes.Count - 1);
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
                        aiList.Add(new AI_Script(viewport, sr));
                        AddAIScriptToTree((AI_Script) aiList[aiList.Count - 1]);
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
                    aiList.Add(new AI_Script(viewport, br));
                    AddAIScriptToTree((AI_Script)aiList[aiList.Count - 1]);
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
                for (i = 0; i < aiList.Count; i++)
                {
                    aiList[i].writeAIToFile(bw);
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

        private void AddAIScriptToTree(AI_Script ai)
        {
            int i;
            TreeNode treeNode = new TreeNode("Script " + ai.ID);
            for (i = 0; i < ai.Count; i++)
            {
                treeNode.Nodes.Add(ai.getNode(i).ToString());
            }
            tvAIScripts.Nodes.Add(treeNode);
        }

        private void dlgFileOpen_FileOk(object sender, CancelEventArgs e)
        {
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            NewFile();
        }

        private void splitter1_Panel1_Paint(object sender, PaintEventArgs e)
        {
            int i;
            Graphics gr = splitter1.Panel1.CreateGraphics();
            gr.Clear(Color.Beige);
            System.Drawing.Rectangle r = screenRect;
            r.Offset(offset);
            gr.FillRectangle(Brushes.Black, r);
            for (i = 0; i < aiList.Count; i++)
            {
                if (aiList[i] is AI_Script)
                {
                    if (selectedScript!=i)
                        drawAIScript(gr, (AI_Script)aiList[i]);
                }
            }
            if (selectedScript != -1)
            {
                if (aiList[selectedScript] is AI_Script)
                    drawAIScript(gr, (AI_Script)aiList[selectedScript]);
            }
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
                if (selectedScript != -1)
                {
                    if (script == aiList[selectedScript])
                    {
                        pen = Pens.GreenYellow;
                        brush = Brushes.GreenYellow;
                    }
                }
                if (i == selectedNode)
                    temp = current;
                node = script.getNode(i);
                drawScriptSegment(gr, brush, pen, ref current, node);
            }
            if (selectedNode != -1)
            {
                if (script == aiList[selectedScript])
                    drawScriptSegment(gr, Brushes.Red, Pens.Red, ref temp, script.getNode(selectedNode));
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

        private void vScroller_ValueChanged(object sender, EventArgs e)
        {
            offset.X = this.splitter1.Panel1.ClientSize.Width / 2 - 400 - hScroller.Value;
            offset.Y = this.splitter1.Panel1.ClientSize.Height / 2 - 300 - vScroller.Value;
            this.splitter1.Panel1.Invalidate();
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
            Graphics gr = splitter1.Panel1.CreateGraphics();
            int lastScriptDrawn = selectedScript;

            // if the Parent is null then a root node is selected
            if (e.Node.Parent == null)
            {
                selectedScript = e.Node.Index;
                selectedNode = -1;
                propertyGrid1.SelectedObject = aiList[selectedScript];
            }
            else
            {
                selectedScript = e.Node.Parent.Index;
                selectedNode = e.Node.Index;
                ai = aiList[selectedScript];
                if (ai is AI_Script)
                {
                    drawAIScript(gr, (AI_Script)ai);
                    propertyGrid1.SelectedObject = ((AI_Script)ai).getNode(selectedNode);
                }
            }
            if (lastScriptDrawn != selectedScript)
            {
                if (lastScriptDrawn != -1)
                {
                    if (aiList[lastScriptDrawn] is AI_Script)
                        drawAIScript(gr, (AI_Script)aiList[lastScriptDrawn]);
                }
                if (aiList[selectedScript] is AI_Script)
                    drawAIScript(gr, (AI_Script)aiList[selectedScript]);
            }
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            TreeNode node = tvAIScripts.SelectedNode;
            IArtificialIntelligence ai;
            // if the Parent is null then a root node is selected
            if (node.Parent == null)
            {
                if (aiList[node.Index] is AI_Script)
                {
                    node.Text = "Script " + ((AI_Script)aiList[node.Index]).ID;
                }
            }
            else
            {
                ai = aiList[node.Parent.Index];
                if (ai is AI_Script)
                {
                    node.Text = ((AI_Script)ai).getNode(node.Index).ToString();
                }
            }
            this.splitter1_Panel1_Paint(this, null);
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
                aiList.RemoveAt(selectedScript); 
                tvAIScripts.Nodes.RemoveAt(selectedScript);
                selectedScript = -1;
                selectedNode = -1;
                tvAIScripts.SelectedNode = null;
                modified = true;
                splitter1_Panel1_Paint(this, null);
            }
        }

        private void menuDeleteNode_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete the selected script node?\n(This cannot be undone.)",
                "Delete AI Script Node", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                if (aiList[selectedScript] is AI_Script)
                {
                    ((AI_Script)aiList[selectedScript]).RemoveAt(selectedNode);
                    tvAIScripts.SelectedNode.Remove();
                    selectedScript = -1;
                    selectedNode = -1;
                    tvAIScripts.SelectedNode = null;
                    modified = true;
                    splitter1_Panel1_Paint(this, null);
                }
            }
        }

        private void menuAddScript_Click(object sender, EventArgs e)
        {
            AI_Script script = new AI_Script(viewport);
            script.Add(new AI_StartPoint(Vector2.Zero));
            script.Add(new AI_EndPoint());
            aiList.Add(script);
            AddAIScriptToTree(script);
            modified = true;
        }

        private void menuAddNodeAfter_Click(object sender, EventArgs e)
        {
            frmNewScriptNode nodeForm = new frmNewScriptNode();
            if (nodeForm.ShowDialog(this) == DialogResult.OK)
            {
                ((AI_Script)aiList[selectedScript]).Insert(selectedNode + 1, nodeForm.getNode());
                tvAIScripts.Nodes[selectedScript].Nodes.Insert(selectedNode + 1, nodeForm.getNode().ToString());
                tvAIScripts.SelectedNode = tvAIScripts.Nodes[selectedScript].Nodes[selectedNode+1];
                modified = true;
                splitter1_Panel1_Paint(this, null);
            }
        }

        private void menuAddNodeBefore_Click(object sender, EventArgs e)
        {
            frmNewScriptNode nodeForm = new frmNewScriptNode();
            if (nodeForm.ShowDialog(this) == DialogResult.OK)
            {
                ((AI_Script)aiList[selectedScript]).Insert(selectedNode, nodeForm.getNode());
                tvAIScripts.Nodes[selectedScript].Nodes.Insert(selectedNode, nodeForm.getNode().ToString());
                tvAIScripts.SelectedNode = tvAIScripts.Nodes[selectedScript].Nodes[selectedNode];
                modified = true;
                splitter1_Panel1_Paint(this, null);
            }
        }

    }
}