using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Text;
using System.IO;

namespace Space_Cats_V1._2
{
    class AI_Script : IArtificialIntelligence
    {
        private List<AI_ScriptNode> z_script;
        private Rectangle z_viewport;
        private int z_step;
        private bool z_removeEnemy;
        private int z_ID;

        #region Public Properties
        [Browsable(false)]
        public int Step
        { get { return z_step; } }
        [Browsable(false)]
        public int Count 
        { get { return z_script.Count; } }
        [Description("The ID number for this script.")]
        public int ID
        {
            get { return z_ID; }
            set { z_ID = value; }
        }
        #endregion

        #region Constructors
        private AI_Script()
        {
            // Make it private so a blank one can't be called
            // we do NOT want a blank AI here
        }

        public AI_Script(Rectangle viewport, List<AI_ScriptNode> script)
            : this(viewport)
        {
            foreach (AI_ScriptNode node in script)
            {
                z_script.Add(node.clone());
            }
        }
        
        public AI_Script(Rectangle viewport)
        {
            z_script = new List<AI_ScriptNode>();
            this.z_removeEnemy = false;
            z_viewport = viewport;
            z_step = 0;
        }

        public AI_Script(Rectangle viewport, BinaryReader br)
            : this(viewport)
        {
            AI_ScriptNode node;
            ID = br.ReadInt32();
            do
            {
                node = AI_ScriptNode.readNodeFromFile(br);
                if (node == null)
                    node = new AI_EndPoint();
                z_script.Add(node);
            } while (!(node is AI_EndPoint)); 
        }

        public AI_Script(Rectangle viewport, StreamReader sr)
         : this(viewport)
        {
            AI_ScriptNode node;
            string[] input = sr.ReadLine().Split(' ');
            z_ID = int.Parse(input[1]);
            do
            {
                node = AI_ScriptNode.readNodeFromFile(sr);
                if (node == null)
                    node = new AI_EndPoint();
                z_script.Add(node);
            } while (!(node is AI_EndPoint));
        }
        #endregion

        #region Accessors
        public AI_PathNodeStatus getScriptStatus()
        {
            return z_script[z_step].getStatus();
        }

        //Get the starting position of the enemy
        public Vector2 getStartingPosition()
        {
            return z_script[0].getLocation();
        }

        public AI_ScriptNode getNode(int index)
        {
            return z_script[index];
        }

        #endregion

        public void writeAIToFile(BinaryWriter bw)
        {
            bw.Write("AI_SCRIPT");
            bw.Write(ID);
            foreach (AI_ScriptNode node in z_script)
            {
                node.writeNodeToFile(bw);
            }
        }

        public void Add(AI_ScriptNode node)
        {
            z_script.Add(node);
        }

        public void RemoveAt(int index)
        {
            if (index >= 0 && index < z_script.Count)
                z_script.RemoveAt(index);
        }

        public void Insert(int index, AI_ScriptNode node)
        {
            z_script.Insert(index, node);
        }

        public IArtificialIntelligence clone()
        {
            return new AI_Script(z_viewport, z_script);
        }

        public void translate(Vector2 offset)
        {
            int i;
            for (i = 0; i < z_script.Count; i++)
                z_script[i].translate(offset);
        }

        public void translate(Vector2 offset, int startIndex)
        {
            int i;
            for (i = startIndex; i < z_script.Count; i++)
                z_script[i].translate(offset);
        }

        public void translate(Vector2 offset, int startIndex, int stopIndex)
        {
            int i;
            int end = (stopIndex < z_script.Count ? stopIndex : z_script.Count);
            for (i = startIndex; i < end; i++)
                z_script[i].translate(offset);
        }

        public void reset()
        {
            int i;
            // reset all nodes in the script
            for (i = 0; i < z_script.Count; i++)
            {
                z_script[i].reset();
            }
            z_step = 0;
        }

        public void goToNextStep()
        {
            z_step++;
        }

        //Return the new velocity for the enemy
        public Vector2 calculateNewVelocity(Vector2 currentPosition, GameTime gameTime)
        {
            // what are we doing here?
            if (z_script[z_step] is AI_MoveTo || z_script[z_step] is AI_ArcTo)
            {
                if (z_script[z_step].GetDistanceTo(currentPosition) <= z_script[z_step].getSpeed())
                    z_step++;
                return z_script[z_step].CreateVectorFrom(currentPosition);
            }
            else if (z_script[z_step] is AI_JumpTo || z_script[z_step] is AI_StartPoint)
            {
                return z_script[z_step++].CreateVectorFrom(currentPosition);
            }
            else if (z_script[z_step] is AI_Stage)
            {
                return ((AI_Stage)z_script[z_step]).CreateVectorFrom(currentPosition);
            }
            else if (z_script[z_step] is AI_Kamikaze)
            {
                if (!z_viewport.Contains((int)currentPosition.X, (int)currentPosition.Y))
                    z_step++;
                return z_script[z_step].CreateVectorFrom(currentPosition);
            }
            else if (z_script[z_step] is AI_Reset)
            {
                if (((AI_Reset)z_script[z_step]).canReset())
                    reset();
                else
                    z_step++;
                return z_script[z_step].CreateVectorFrom(currentPosition);
            }
            else if (z_script[z_step] is AI_Wait)
            {
                if (((AI_Wait)z_script[z_step]).isTimerDone(gameTime))
                    z_step++;
                return z_script[z_step].CreateVectorFrom(currentPosition);
            }
            else if (z_script[z_step] is AI_EndPoint)
            {
                this.z_removeEnemy = true;
            }
            return Vector2.Zero;
        }

        //Return a new speed for the enemy
        public float calculateNewSpeed(Vector2 currentPosition, GameTime gameTime)
        {
            return 1.0f;
        }

        //Decide when the enemy should fire a missle
        public bool firesMissle(Vector2 currentPosition, GameTime gameTime)
        {
            return false;
        }

        //The enemy's AI is finished and the enemy is ready to be removed from the game
        public bool okToRemove()
        {
            return this.z_removeEnemy;
        }
	
	}
}
