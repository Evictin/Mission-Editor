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
    public enum AI_PathNodeStatus
    {
        NORMAL, WAITING, STAGED, KAMIKAZE
    }
    
    public enum AI_CircleDirections
    {
        Clockwise, Counter_Clockwise
    }

    public class AI_ScriptNode
    {
        #region Node ID Constants
        public const int ID_NULL = 0;
        public const int ID_START_POINT = 1;
        public const int ID_END_POINT = 2;
        public const int ID_RESET = 3;
        public const int ID_WAIT = 4;
        public const int ID_JUMP_TO = 5;
        public const int ID_MOVE_TO = 6;
        public const int ID_ARC_TO = 7;
        public const int ID_KAMIKAZE = 8;
        public const int ID_STAGE_HORIZONTAL = 9;
        public const int ID_STAGE_VERTICAL = 10;
        public const int ID_STAGE_POINT_TO_POINT = 11;
        public const int ID_STAGE_CIRCLE = 12;
        #endregion

        protected int z_command;
        protected Vector2 z_location;
        protected float z_speed;
        public const float CIRCLE_DIRECTION_CW = 1.0f;
        public const float CIRCLE_DIRECTION_CCW = -1.0f;

        public AI_ScriptNode()
        {
            z_command = AI_ScriptNode.ID_NULL;
        }   

        public AI_ScriptNode(AI_ScriptNode node)
            : this(node.z_location, node.z_speed)
        {
            z_command = AI_ScriptNode.ID_NULL;
        }

        public AI_ScriptNode(Vector2 location, float speed)
        {
            z_location = location;
            z_speed = speed;
            z_command = AI_ScriptNode.ID_NULL;
        }

        public int getCommand()
        {
            return z_command;
        }

        public Vector2 getLocation()
        {
            return z_location;
        }

        virtual public AI_PathNodeStatus getStatus()
        {
            return AI_PathNodeStatus.NORMAL;
        }

        virtual public void reset()
        {
        }

        virtual public Vector2 CreateVectorFrom(Vector2 start)
        {
            Vector2 retVector = z_location - start;
            retVector.Normalize();
            return retVector * z_speed;
        }

        public float getSpeed()
        {
            return z_speed;
        }

        public float GetDistanceTo(Vector2 destination)
        {
            return Vector2.Distance(z_location, destination);
        }

        virtual public AI_ScriptNode clone()
        {
            return new AI_ScriptNode(this);
        }

        virtual public void translate(Vector2 offset)
        {
            z_location = Vector2.Add(z_location, offset);
        }

        virtual public void rescale(float scaleX, float scaleY)
        {
            z_location.X *= scaleX;
            z_location.Y *= scaleY;
            z_speed *= (scaleX<scaleY ? scaleX : scaleY);
        }

        virtual public void writeNodeToFile(BinaryWriter bw)
        {
        }

        // read from a compressed file
        public static AI_ScriptNode readNodeFromFile(BinaryReader br)
        {
            int command = br.ReadInt32();
            switch (command)
            {
                case ID_START_POINT:
                    return new AI_StartPoint(new Vector2(br.ReadSingle(), br.ReadSingle()));
                case ID_END_POINT:
                    return new AI_EndPoint();
                case ID_RESET:
                    return new AI_Reset(br.ReadInt32());
                case ID_WAIT:
                    return new AI_Wait(br.ReadDouble());
                case ID_JUMP_TO:
                    return new AI_JumpTo(new Vector2(br.ReadSingle(), br.ReadSingle()));
                case ID_MOVE_TO:
                    return new AI_MoveTo(new Vector2(br.ReadSingle(), br.ReadSingle()), br.ReadSingle());
                case ID_ARC_TO:
                    return new AI_ArcTo(new Vector2(br.ReadSingle(), br.ReadSingle()), br.ReadSingle(), br.ReadSingle());
                case ID_KAMIKAZE:
                    return new AI_Kamikaze(br.ReadSingle());
                case ID_STAGE_HORIZONTAL:
                    return new AI_StageHorizontal(br.ReadSingle(), br.ReadSingle());
                case ID_STAGE_VERTICAL:
                    return new AI_StageVertical(br.ReadSingle(), br.ReadSingle());
                case ID_STAGE_POINT_TO_POINT:
                    return new AI_StagePointToPoint(new Vector2(br.ReadSingle(), br.ReadSingle()), br.ReadSingle());
                case ID_STAGE_CIRCLE:
                    return new AI_StageCircle(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            }
            return null;
        }

        // read from an uncompressed file
        public static AI_ScriptNode readNodeFromFile(StreamReader sr)
        {
            string input = sr.ReadLine();
            string[] lines = input.Split(' ');
            if (lines[0].CompareTo("START_POINT") == 0)
            {
                return new AI_StartPoint(new Vector2(int.Parse(lines[1]), int.Parse(lines[2])));
            }
            else if (lines[0].CompareTo("END_POINT") == 0)
            {
                return new AI_EndPoint();
            }
            else if (lines[0].CompareTo("RESET") == 0)
            {
                return new AI_Reset(int.Parse(lines[1]));
            }
            else if (lines[0].CompareTo("JUMP_TO") == 0)
            {
                return new AI_JumpTo(new Vector2(int.Parse(lines[1]), int.Parse(lines[2])));
            }
            else if (lines[0].CompareTo("MOVE_TO") == 0)
            {
                return new AI_MoveTo(new Vector2(int.Parse(lines[1]), int.Parse(lines[2])), float.Parse(lines[3]));
            }
            else if (lines[0].CompareTo("ARC_TO") == 0)
            {
                return new AI_ArcTo(new Vector2(int.Parse(lines[1]), int.Parse(lines[2])), int.Parse(lines[3]), float.Parse(lines[4]));
            }
            else if (lines[0].CompareTo("KAMIKAZE") == 0)
            {
                return new AI_Kamikaze(float.Parse(lines[1]));
            }
            else if (lines[0].CompareTo("WAIT") == 0)
            {
                return new AI_Wait(double.Parse(lines[1]));
            }
            else if (lines[0].CompareTo("STAGE_HORIZONTAL") == 0)
            {
                return new AI_StageHorizontal(float.Parse(lines[1]), float.Parse(lines[2]));
            }
            else if (lines[0].CompareTo("STAGE_VERTICAL") == 0)
            {
                return new AI_StageVertical(float.Parse(lines[1]), float.Parse(lines[2]));
            }
            else if (lines[0].CompareTo("STAGE_POINT_TO_POINT") == 0)
            {
                return new AI_StagePointToPoint(new Vector2(int.Parse(lines[1]), int.Parse(lines[2])), float.Parse(lines[4]));
            }
            else if (lines[0].CompareTo("STAGE_CIRCLE") == 0)
            {
                return new AI_StageCircle(float.Parse(lines[1]), float.Parse(lines[2]), 
                    (lines[3].CompareTo("CCW")==0?CIRCLE_DIRECTION_CCW:CIRCLE_DIRECTION_CW));
            }
            return null;
        }

        public override string ToString()
        {
            return "ScriptNode";
        }
    }

    class AI_StartPoint : AI_ScriptNode
    {
        #region Public Properties
        [Description("The location to start the script.")]
        public Vector2 Location
        {
            get { return z_location; }
            set { z_location = value; }
        }
        #endregion

        public AI_StartPoint(Vector2 location)
            : base(location, 0)
        {
            z_command = AI_ScriptNode.ID_START_POINT;
        }

        public AI_StartPoint(AI_StartPoint node)
            : this(node.z_location)
        {
        }

        override public AI_ScriptNode clone()
        {
            return new AI_StartPoint(this);
        }

        override public Vector2 CreateVectorFrom(Vector2 start)
        {
            return z_location - start;
        }

        public override void writeNodeToFile(BinaryWriter bw)
        {
            bw.Write(z_command);
            bw.Write(z_location.X);
            bw.Write(z_location.Y);
        }

        public override string ToString()
        {
            return string.Format("Start Point ({0},{1})", z_location.X, z_location.Y);
        }
    }

    class AI_EndPoint : AI_ScriptNode
    {
        public AI_EndPoint()
            : base(Vector2.Zero, 0) 
        {
            z_command = AI_ArcTo.ID_END_POINT;
        }

        override public AI_ScriptNode clone()
        {
            return  new AI_EndPoint();
        }

        override public Vector2 CreateVectorFrom(Vector2 start)
        {
            return Vector2.Zero;
        }

        public override void writeNodeToFile(BinaryWriter bw)
        {
            bw.Write(z_command);
        }

        public override string ToString()
        {
            return "EndPoint";
        }
    }

    class AI_Reset : AI_ScriptNode
    {
        private int z_count;
        private int z_maxResets;

        #region Public Properties
        [Description("The number of times to reset the AI.\nSet to -1 to reset indefinitely.")]
        public int MaxResets
        {
            get { return z_maxResets; }
            set { z_maxResets = z_count = value; }
        }
        #endregion

        public AI_Reset(int count)
            :base(Vector2.Zero, 0)
        {
            z_count = z_maxResets = count;
            z_command = AI_ScriptNode.ID_RESET;
        }

        public AI_Reset(AI_Reset node)
            : this(node.z_count)
        {
        }

        public AI_Reset()
            : this(-1)
        {
            z_count = z_maxResets;
        }

        override public AI_ScriptNode clone()
        {
            return new AI_Reset(this);
        }

        override public Vector2 CreateVectorFrom(Vector2 start)
        {
            return Vector2.Zero;
        }

        public bool canReset()
        {
            return (--z_count > 0);
        }

        public override void writeNodeToFile(BinaryWriter bw)
        {
            bw.Write(z_command);
            bw.Write(z_maxResets);
        }

        public override string ToString()
        {
            if (z_maxResets > 0)
                return "Reset (indef.)";
            else 
                return string.Format("Reset {0} times", z_maxResets);
        }
    }

    class AI_JumpTo : AI_ScriptNode
    {
        #region Public Properties
        [Description("The location to jump to.")]
        public Vector2 Location
        {
            get { return z_location; }
            set { z_location = value; }
        }
        #endregion

        public AI_JumpTo(Vector2 location)
            : base(location, 0)
        {
            z_command = AI_ScriptNode.ID_JUMP_TO;
        }

        public AI_JumpTo(AI_JumpTo node)
            : this(node.z_location)
        {
        }

        override public AI_ScriptNode clone()
        {
            return new AI_JumpTo(this);
        }

        override public Vector2 CreateVectorFrom(Vector2 start)
        {
            return z_location - start;
        }

        public override void writeNodeToFile(BinaryWriter bw)
        {
            bw.Write(z_command);
            bw.Write(z_location.X);
            bw.Write(z_location.Y);
        }

        public override string ToString()
        {
            return string.Format("JumpTo ({0},{1})", z_location.X, z_location.Y);
        }
    }

    class AI_MoveTo : AI_ScriptNode
    {
        #region Public Properties
        [Description("The location to move to.")]
        public Vector2 Location
        {
            get { return z_location; }
            set { z_location = value; }
        }
        [Description("The speed at which to move.")]
        public float Speed
        {
            get { return z_speed; }
            set { z_speed = value; }
        }
        #endregion

        public AI_MoveTo(Vector2 location, float speed)
            : base(location, speed)
        {
            z_command = AI_ScriptNode.ID_MOVE_TO;
        }

        public AI_MoveTo(AI_MoveTo node)
            : this(node.z_location, node.z_speed)
        {
        }

        override public AI_ScriptNode clone()
        {
            return new AI_MoveTo(this);
        }

        public override void writeNodeToFile(BinaryWriter bw)
        {
            bw.Write(z_command);
            bw.Write(z_location.X);
            bw.Write(z_location.Y);
            bw.Write(z_speed);
        }

        public override string ToString()
        {
            return string.Format("MoveTo ({0},{1}) Speed:{2}", z_location.X, z_location.Y, z_speed);
        }
    }

    class AI_ArcTo : AI_ScriptNode
    {
        private float z_arcHeight;
        private Vector2 z_startPoint;
        private float z_DX;
        private float z_DY;

        #region Public Properties
        [Description("The location to move to.")]
        public Vector2 Location
        {
            get { return z_location; }
            set { z_location = value; }
        }
        [Description("The speed at which to move.")]
        public float Speed
        {
            get { return z_speed; }
            set { z_speed = value; }
        }
        [Description("The height of the arc above the straight line." +
                    "\n Use a negative value to arc below the straight path.")]
        public float ArcHeight
        {
            get { return z_arcHeight; }
            set { z_arcHeight = value; }
        }
        #endregion

        public AI_ArcTo(Vector2 location, float arcHeight, float speed)
            : base(location, speed)
        {
            z_arcHeight = arcHeight;
            z_startPoint = Vector2.Zero;
            z_command = AI_ScriptNode.ID_ARC_TO;
        }

        public AI_ArcTo(AI_ArcTo node)
            : this(node.z_location, node.z_arcHeight, node.z_speed)
        {
        }

        override public AI_ScriptNode clone()
        {
            return new AI_ArcTo(this);
        }

        public override void reset()
        {
            z_startPoint = Vector2.Zero;
        }

        public override void rescale(float scaleX, float scaleY)
        {
            base.rescale(scaleX, scaleY);
            z_arcHeight *= scaleY;
            z_DX *= scaleX;
            z_DY *= scaleY;
        }

        override public Vector2 CreateVectorFrom(Vector2 currentPosition)
        {
            // declare variables
            float percentage;
            Vector2 temp;

            // if not started, then start
            if (z_startPoint==Vector2.Zero)
            {
                z_startPoint = currentPosition;
                z_DX = z_location.X - z_startPoint.X;
                z_DY = z_location.Y - z_startPoint.Y;
            }
            percentage = ((currentPosition.X - z_startPoint.X) / z_DX) + (.01f * z_speed);
            temp = z_startPoint + new Vector2(z_DX * percentage, z_DY * percentage - (float)Math.Sin(percentage * Math.PI) * z_arcHeight) - currentPosition;
            temp.Normalize();
            return temp * z_speed;
        }

        public override void writeNodeToFile(BinaryWriter bw)
        {
            bw.Write(z_command);
            bw.Write(z_location.X);
            bw.Write(z_location.Y);
            bw.Write(z_arcHeight);
            bw.Write(z_speed);
        }

        public override string ToString()
        {
            return string.Format("ArcTo ({0},{1}) Height:{2} Speed:{3}", z_location.X, z_location.Y, z_arcHeight, z_speed);
        }

    }

    class AI_Kamikaze : AI_ScriptNode
    {
        #region Public Properties
        [Description("The speed at which to dive bomb the player.")]
        public float Speed
        {
            get { return z_speed; }
            set { z_speed = value; }
        }
        #endregion

        public AI_Kamikaze(float speed)
            : base(Vector2.Zero, speed)
        {
            z_command = AI_ScriptNode.ID_KAMIKAZE;
        }

        public AI_Kamikaze(AI_Kamikaze node)
            : this(node.z_speed)
        {
        }

        override public Vector2 CreateVectorFrom(Vector2 start)
        {
            PlayerShip player = PlayerShip.getInstance();

            if (z_location==Vector2.Zero)
            {
                z_location = player.getPosition() - start;
                z_location.Normalize();
                z_location = z_location * z_speed;
            }
            return z_location;
        }

        override public void reset()
        {
            z_location = Vector2.Zero;
        }

        override public AI_PathNodeStatus getStatus()
        {
            return AI_PathNodeStatus.KAMIKAZE;
        }

        override public AI_ScriptNode clone()
        {
            return new AI_Kamikaze(this);
        }

        public override void writeNodeToFile(BinaryWriter bw)
        {
            bw.Write(z_command);
            bw.Write(z_speed);
        }

        public override string ToString()
        {
            return string.Format("Kamimaze Speed:{0}", z_speed);
        }
    }

    class AI_Wait : AI_ScriptNode
    {
        private double z_Time;
        private double z_runTilTime;

        #region Public Properties
        [Description("The amount of time in milliseconds to wait around.")]
        public double Time
        {
            get { return z_Time; }
            set { z_Time = value; }
        }
        #endregion

        public AI_Wait(double time)
            : base(Vector2.Zero, 0)
        {
            z_Time = time;
            z_runTilTime = 0;
            z_command = AI_ScriptNode.ID_WAIT;
        }

        public AI_Wait(AI_Wait node)
            : this(node.z_Time)
        {
        }

        override public AI_ScriptNode clone()
        {
            return new AI_Wait(this);
        }

        override public void reset()
        {
            z_runTilTime = 0;
        }

        public bool isTimerDone(GameTime time)
        {
            // if the timer hasn't been started, then start the timer and return false;
            if (z_runTilTime == 0)
            {
                z_runTilTime = time.TotalGameTime.TotalMilliseconds + z_Time;
                return false;
            }
            // otherwise, check the time
            if (time.TotalGameTime.TotalMilliseconds >= z_runTilTime)
            {
                z_runTilTime = 0;
                return true;
            }
            return false;
        }

        override public AI_PathNodeStatus getStatus()
        {
            return AI_PathNodeStatus.WAITING;
        }

        override public Vector2 CreateVectorFrom(Vector2 start)
        {
            return Vector2.Zero;
        }

        public override void writeNodeToFile(BinaryWriter bw)
        {
            bw.Write(z_command);
            bw.Write(z_Time);
        }

        public override string ToString()
        {
            return string.Format("Wait ({0}ms)", z_Time);
        }
    }

    abstract class AI_Stage : AI_ScriptNode
    {
        protected float z_swingDistance;
        protected float z_angle;
        protected float z_dAngle;

        public AI_Stage(Vector2 location, float speed, float swingDistance)
            : base(location, speed)
        {
            z_angle = 0f;
            z_dAngle = 0f;
            z_swingDistance = swingDistance;
        }

        override public void reset()
        {
            base.reset();
            z_angle = 0f;
            z_dAngle = 0f;
            z_location = Vector2.Zero;
        }

        override public AI_PathNodeStatus getStatus()
        {
            return AI_PathNodeStatus.STAGED;
        }

        public float getSwingDistance()
        {
            return z_swingDistance;
        }

        public override void rescale(float scaleX, float scaleY)
        {
            base.rescale(scaleX, scaleY);
        }

        public override Vector2 CreateVectorFrom(Vector2 start)
        {
            return base.CreateVectorFrom(start);
        }

    }

    class AI_StageHorizontal : AI_Stage
    {
        #region Public Properties
        [Description("The distance to swing back and forth.")]
        public float SwingDistance
        {
            get { return z_swingDistance; }
            set { z_swingDistance = value; }
        }
        [Description("The speed at which to swing back and forth.")]
        public float Speed
        {
            get { return z_speed; }
            set { z_speed = value; }
        }
        #endregion

        public AI_StageHorizontal(float swingDistance, float speed)
            : base(Vector2.Zero, speed, swingDistance)
        {
            z_command = AI_ScriptNode.ID_STAGE_HORIZONTAL;
        }

        public AI_StageHorizontal(AI_StageHorizontal node)
            : this(node.z_swingDistance, node.z_speed)
        {
        }

        override public AI_ScriptNode clone()
        {
            return new AI_StageHorizontal(this);
        }

        public override void rescale(float scaleX, float scaleY)
        {
            base.rescale(scaleX, scaleY);
            z_swingDistance *= scaleX;
        }

        public override Vector2 CreateVectorFrom(Vector2 start)
        {
            if (z_location == Vector2.Zero)
            {
                z_location = start;
                z_dAngle = (float)z_speed / (float)z_swingDistance;
            }
            z_angle += z_dAngle;
            return new Vector2(z_location.X + (float)Math.Sin(z_angle) * z_swingDistance - start.X, z_location.Y - start.Y);
        }
        
        public override void writeNodeToFile(BinaryWriter bw)
        {
            bw.Write(z_command);
            bw.Write(z_swingDistance);
            bw.Write(z_speed);
        }

        public override string ToString()
        {
            return string.Format("StageHorizontal Swing:{0} Speed:{1}", z_swingDistance, z_speed);
        }
    }

    class AI_StagePointToPoint : AI_Stage
    {
        private Vector2 z_midPoint;
        private float z_dx, z_dy;
        private double z_startTime, z_swingTime;

        #region Public Properties
        [Description("The location to swing between.")]
        public Vector2 Location
        {
            get { return z_location; }
            set { z_location = value; }
        }
        [Description("The speed at which to swing back and forth.")]
        public float Speed
        {
            get { return z_speed; }
            set { z_speed = value; }
        }
        #endregion

        public AI_StagePointToPoint(Vector2 swingToPoint, float speed)
            : base(swingToPoint, speed, 0)
        {
            z_midPoint = Vector2.Zero;
            z_command = AI_ScriptNode.ID_STAGE_POINT_TO_POINT;
        }

        public AI_StagePointToPoint(AI_StagePointToPoint node)
            : this(node.z_location, node.z_speed)
        {
        }

        override public AI_ScriptNode clone()
        {
            return new AI_StagePointToPoint(this);
        }

        public override void rescale(float scaleX, float scaleY)
        {
            base.rescale(scaleX, scaleY);
            z_dx *= scaleX;
            z_dy *= scaleY;
            z_midPoint.X *= scaleX;
            z_midPoint.Y *= scaleY;
        }

        public Vector2 CreateVectorAtTime(Vector2 currentPosition, GameTime gameTime)
        {
            Vector2 retVector;
            // if not started, then start
             if (z_startTime == 0)
            {
                // if we don't have a midpoint yet, create one
                if (z_midPoint == Vector2.Zero)
                {
                    z_midPoint = Vector2.Lerp(currentPosition, z_location, 0.5f);
                    z_dx = z_midPoint.X - currentPosition.X;
                    z_dy = z_midPoint.Y - currentPosition.Y;
                }
                // travel to the midpoint of the zone first
                if (Vector2.Distance(currentPosition, z_midPoint) > z_speed)
                {
                    retVector = z_midPoint - currentPosition;
                    retVector.Normalize();
                    return retVector * z_speed;
                }
                // we are at the midpoint, so go ahead and start it up
                z_startTime = gameTime.TotalGameTime.TotalMilliseconds;
                return Vector2.Zero;
            }
            
            double percentage = ((gameTime.TotalGameTime.TotalMilliseconds - z_startTime) % z_swingTime) / z_swingTime;
            return new Vector2(z_midPoint.X + (float) Math.Sin(2 * Math.PI * percentage) * z_dx,
                z_midPoint.Y + (float)Math.Sin(2 * Math.PI * percentage) * z_dy) - currentPosition;
        }

        public override void reset()
        {
            base.reset();
            z_midPoint = Vector2.Zero;
        }

        public override void writeNodeToFile(BinaryWriter bw)
        {
            bw.Write(z_command);
            bw.Write(z_location.X);
            bw.Write(z_location.Y); 
            bw.Write(z_speed);
        }

        public override string ToString()
        {
            return string.Format("StagePointToPoint ({0},{1}) Speed:{2}", z_location.X, z_location.Y, z_speed);
        }
    }

    class AI_StageVertical : AI_Stage
    {
        #region Public Properties
        [Description("The distance to swing up and down.")]
        public float SwingDistance
        {
            get { return z_swingDistance; }
            set { z_swingDistance = value; }
        }
        [Description("The speed at which to swing up and down.")]
        public float Speed
        {
            get { return z_speed; }
            set { z_speed = value; }
        }
        #endregion

        public AI_StageVertical(float swingDistance, float speed)
            : base(Vector2.Zero, speed, swingDistance)
        {
            z_command = AI_ScriptNode.ID_STAGE_VERTICAL;
        }

        public AI_StageVertical(AI_StageVertical node)
            : this(node.z_swingDistance, node.z_speed)
        {
        }

        override public AI_ScriptNode clone()
        {
            return new AI_StageVertical(this);
        }

        public override void rescale(float scaleX, float scaleY)
        {
            base.rescale(scaleX, scaleY);
            z_swingDistance *= scaleY;
        }

        public override Vector2 CreateVectorFrom(Vector2 start)
        {
            if (z_location == Vector2.Zero)
            {
                z_location = start;
                z_dAngle = (float)z_speed / (float)z_swingDistance;
            }
            z_angle += z_dAngle;
            return new Vector2(z_location.X - start.X, z_location.Y + (float)Math.Sin(z_angle) * z_swingDistance-start.Y);
        }
        
        public override void writeNodeToFile(BinaryWriter bw)
        {
            bw.Write(z_command);
            bw.Write(z_swingDistance);
            bw.Write(z_speed);
        }

        public override string ToString()
        {
            return string.Format("StageVertical Swing:{0} Speed:{1}", z_swingDistance, z_speed);
        }
    }

    class AI_StageCircle : AI_Stage
    {
        private float z_direction;
        #region Public Properties
        [Description("The radius of the circle to swing around.")]
        public float Radius
        {
            get { return z_swingDistance; }
            set { z_swingDistance = value; }
        }
        [Description("The speed at which to swing around.")]
        public float Speed
        {
            get { return z_speed; }
            set { z_speed = value; }
        }
        [Description("The direction to circle.\nCW to swing clockwise.\n" +
                    "CCW to swing counter-clockwise.")]
        public AI_CircleDirections Direction
        {
            get { return (z_direction == AI_ScriptNode.CIRCLE_DIRECTION_CW ? AI_CircleDirections.Clockwise : AI_CircleDirections.Counter_Clockwise); }
            set
            {
                if (value == AI_CircleDirections.Clockwise)
                    z_direction = AI_ScriptNode.CIRCLE_DIRECTION_CW;
                else
                    z_direction = AI_ScriptNode.CIRCLE_DIRECTION_CCW;
            }
        }
        #endregion


        public AI_StageCircle(float radius, float speed, float direction) 
            : base(Vector2.Zero, speed, radius)
        {
            z_direction = direction;
            z_command = AI_ScriptNode.ID_STAGE_CIRCLE;
            z_angle = 0.0f;
        }

        public AI_StageCircle(AI_StageCircle node)
            : this(node.z_swingDistance, node.z_speed, node.z_direction)
        {
        }

        override public AI_ScriptNode clone()
        {
            return new AI_StageCircle(this);
        }

        override public void reset()
        {
            base.reset();
            z_angle = 0.0f;
        }

        public override void rescale(float scaleX, float scaleY)
        {
            base.rescale(scaleX, scaleY);
            z_swingDistance *= (scaleX<scaleY ? scaleX : scaleY);
        }


        public override Vector2 CreateVectorFrom(Vector2 start)
        {
            Vector2 temp, retVector;
            if (z_location == Vector2.Zero)
                z_location = start;
            z_angle += (float)z_speed / (float)z_swingDistance;
            temp = new Vector2(z_location.X + (float)Math.Cos(z_angle) * z_swingDistance * z_direction,
                z_location.Y + (float)Math.Sin(z_angle) * z_swingDistance);
            retVector = temp - start;
            retVector.Normalize();
            return retVector * z_speed;
        }

        public override void writeNodeToFile(BinaryWriter bw)
        {
            bw.Write(z_command);
            bw.Write(z_swingDistance);
            bw.Write(z_speed);
            bw.Write(z_direction);
        }

        public override string ToString()
        {
            return string.Format("StageCircle Speed:{0} Radius:{1} Dir:{2}", z_speed, z_swingDistance,
                (z_direction==AI_ScriptNode.CIRCLE_DIRECTION_CW ? "CW" : "CCW"));
        }

    }
}
