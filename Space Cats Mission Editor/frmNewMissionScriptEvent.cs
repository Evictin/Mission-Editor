using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Space_Cats_V1._2;

namespace Space_Cats_Mission_Editor
{
    public partial class frmNewMissionScriptEvent : Form
    {
        private MissionScriptNode node;
        public MissionScriptNode Node
        {
            get { return node; }
            set
            {
                node = value;
            }
        }
        public frmNewMissionScriptEvent()
        {
            InitializeComponent();
            node = new MS_SpawnEnemy1(0, 0);
            nodeProperties.SelectedObject = node;
        }

        private void cmbEventType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEventType.Text.CompareTo("SetAsteroidDensity") == 0)
                node = new MS_SetAsteroidDensity(node.TimeStamp, Asteroid.AsteroidDensity.Lite);
            else if (cmbEventType.Text.CompareTo("SpawnEnemy1") == 0)
            {
                node = new MS_SpawnEnemy1(node.TimeStamp, 0);
            }
            else if (cmbEventType.Text.CompareTo("SpawnEnemy1Wave") == 0)
            {
                node = new MS_SpawnEnemy1Wave(node.TimeStamp, 1, 50, 0, 0);
            }
            nodeProperties.SelectedObject = node;
            nodeProperties.Refresh();
        }
    }
}
