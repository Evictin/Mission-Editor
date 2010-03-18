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
using Space_Cats_V1._2;

namespace Space_Cats_Mission_Editor
{
    public partial class frmNewScriptNode : Form
    {
        private AI_ScriptNode node;

        public frmNewScriptNode()
        {
            InitializeComponent();
            node = new AI_MoveTo(Vector2.Zero, 0f);
            nodeProperties.SelectedObject = node;
        }

        private void cmbNodeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbNodeType.Text.CompareTo("MoveTo") == 0)
                node = new AI_MoveTo(node.getLocation(), node.getSpeed());
            else if (cmbNodeType.Text.CompareTo("JumpTo") == 0)
                node = new AI_JumpTo(node.getLocation());
            else if (cmbNodeType.Text.CompareTo("ArcTo") == 0)
                node = new AI_ArcTo(node.getLocation(), 0, node.getSpeed());
            else if (cmbNodeType.Text.CompareTo("Wait") == 0)
                node = new AI_Wait(0.0);
            else if (cmbNodeType.Text.CompareTo("Reset") == 0)
                node = new AI_Reset(-1);
            else if (cmbNodeType.Text.CompareTo("StageHorizontal") == 0)
                node = new AI_StageHorizontal(100, node.getSpeed());
            else if (cmbNodeType.Text.CompareTo("StageVertical") == 0)
                node = new AI_StageVertical(100, node.getSpeed());
            else if (cmbNodeType.Text.CompareTo("StageCircle") == 0)
                node = new AI_StageCircle(100, node.getSpeed(), AI_ScriptNode.CIRCLE_DIRECTION_CW);
            else if (cmbNodeType.Text.CompareTo("StagePointToPoint") == 0)
                node = new AI_StagePointToPoint(node.getLocation(), node.getSpeed());
            else if (cmbNodeType.Text.CompareTo("Kamikaze") == 0)
                node = new AI_Kamikaze(node.getSpeed());

            nodeProperties.SelectedObject = node;
        }

        public AI_ScriptNode getNode()
        {
            return node;
        }

    }
}