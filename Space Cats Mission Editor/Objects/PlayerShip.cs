#region using
using System;
using System.Collections.Generic;
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
#endregion


namespace Space_Cats_V1._2
{
    class PlayerShip : GameObject
    {

        enum AccelerationState
        {
            positive,
            negative,
            zero
        }


        #region InstanceVariables
        //Instance Variables ----------------------------------------------------------------------------
        private int z_health;
        private int z_lives;
        private float z_maxSpeed;
        private float z_acceleration;
        private bool z_IsSlowingDownX;
        private bool z_IsSlowingDownY;
        private float z_accelTimerX;
        private float z_accelTimerY;
        private AccelerationState z_currentXstate;
        private AccelerationState z_currentYstate;
        private bool z_IsInvincible;
        private float z_InvincibleTimer;
        private Vector2 z_startingPosition;
        private static PlayerShip instanceOf = null;
        private static Rectangle z_hitRecInvincible;
        private static Circle z_hitCircleInvincible;
        public int score;
        #endregion

        #region Public Properties
        public override Circle HitCircle
        {
            get
            {
                if (this.z_IsInvincible)
                    return z_hitCircleInvincible;
                return base.HitCircle;
            }
        }
        override public Rectangle HitRec
        {
            get
            {
                if (this.z_IsInvincible)
                    return z_hitRecInvincible;
                else
                    return new Rectangle((int)(this.Left + this.Width * .15),
                        (int)(this.Top + this.Height * .1), (int)(this.Width * .7), (int)(this.Height * .9));
            }
        }
        public int Health
        { 
            get { return z_health;}
            set
            {
                if (value < z_health && !z_IsInvincible)
                {
                    z_health = value;
                    if (z_health <= 0)
                    {
                        this.IsAlive = false;
                        Lives--;
                    }
                }
                else
                    z_health = value;
            }
        }
        public int Lives
        {
            get { return z_lives; }
            set
            {
                z_lives = value;
                if (value == 0)
                {
                }
            }
        }
        public bool IsInvincible
        {
            get { return z_IsInvincible; }
            set
            {
                // if we are setting inv. mode and not there already...
                if (value && !z_IsInvincible)
                    z_InvincibleTimer = 0;
                z_IsInvincible = value;
            }
        }
        #endregion

        private float InvincibleTimer
        {
            get { return z_InvincibleTimer; }
            set
            {
                z_InvincibleTimer = value;
                if (value >= 2000)
                    z_IsInvincible = false;
            }
        }

        // Used by other classes....
        public static PlayerShip getInstance()
        {
            return instanceOf;
        }

        public static PlayerShip getInstance(Texture2D loadedSprite, Vector2 startingPosition)
        {
            if (instanceOf == null)
                instanceOf = new PlayerShip(loadedSprite, startingPosition);
            return instanceOf;
        }

        #region Constructor
        //Constructor -----------------------------------------------------------------------------------     
        private PlayerShip(Texture2D loadedSprite, Vector2 startingPosition)
            : base(loadedSprite)
        {
            score = 0;
            this.z_health = 100;
            //IMPORTANT!! Acceleration must divide into maxSpeed evenly.
            this.z_maxSpeed = 7.0f; // <--- Is this divisable
            this.z_acceleration = 1.0f;// <-- By this?
            //END-IMPORTANT
            this.z_IsSlowingDownX = false;
            this.z_IsSlowingDownY = false;
            this.z_accelTimerX = 0;
            this.z_accelTimerY = 0;
            this.z_currentXstate = AccelerationState.zero;
            this.z_currentYstate = AccelerationState.zero;
            this.Lives = 3;
            this.IsInvincible = true;
            this.z_startingPosition = startingPosition;
            this.Position = z_startingPosition;
            SpriteRows = 1;
            SpriteCols = 1;
            this.z_animationDelay = 50;
            z_hitRecInvincible = new Rectangle(100000, 100000, 0, 0);
            z_hitCircleInvincible = new Circle(new Vector2(100000, 100000), 0);
            DrawRotation = MathHelper.PiOver2;
            SpriteOrientation = MathHelper.PiOver2;
            DrawDepth = .4f;
        }
        #endregion

        public void reset()
        {
            score = 0;
            this.z_health = 100;
            //IMPORTANT!! Acceleration must divide into maxSpeed evenly.
            this.z_maxSpeed = 7.0f; // <--- Is this divisable
            this.z_acceleration = 1.0f;// <-- By this?
            //END-IMPORTANT
            this.z_IsSlowingDownX = false;
            this.z_IsSlowingDownY = false;
            this.z_accelTimerX = 0;
            this.z_accelTimerY = 0;
            this.z_currentXstate = AccelerationState.zero;
            this.z_currentYstate = AccelerationState.zero;
            this.Lives = 3;
            this.IsInvincible = true;
            this.Position = z_startingPosition;
            this.resetXvelocity();
            this.resetYvelocity();
        }

        #region Accessors
        //Accessor Methods ------------------------------------------------------------------------------
        public bool getIsSlowingDownX()
        {
            return this.z_IsSlowingDownX;
        }
        public bool getIsSlowingDownY()
        {
            return this.z_IsSlowingDownY;
        }
        public bool isAcceleratingX()
        {
            switch (this.z_currentXstate)
            {
                case AccelerationState.zero:
                    {
                        return false;
                    }
                default: return true;
            }
        }
        public bool isAcceleratingY()
        {
            switch (this.z_currentYstate)
            {
                case AccelerationState.zero:
                    {
                        return false;
                    }
                default: return true;
            }
        }
        #endregion



        #region Mutators
        //Mutator Methods -------------------------------------------------------------------------------
        public void setIsSlowingDownX(bool isIt)
        {
            this.z_IsSlowingDownX = isIt;
        }
        public void setIsSlowingDownY(bool isIt)
        {
            this.z_IsSlowingDownY = isIt;
        }
        #endregion

        #region AccelerationMethods
        //Acceleration Methods ---------------------------------------------------------------------------------
        public void accelerateLeft()
        {
            if (this.Velocity.X > -1 * this.z_maxSpeed)
            {
                if (this.z_IsSlowingDownX)
                    this.resetXvelocity();
                this.Velocity = new Vector2(this.Velocity.X - this.z_acceleration, this.Velocity.Y);
                this.z_currentXstate = AccelerationState.negative;
            }
        }
        public void accelerateRight()
        {
            if (this.Velocity.X < this.z_maxSpeed)
            {
                if (this.z_IsSlowingDownX)
                    this.resetXvelocity();
                this.Velocity = new Vector2(this.Velocity.X + this.z_acceleration, this.Velocity.Y);
                this.z_currentXstate = AccelerationState.positive;
            }
        }
        public void accelerateUp()
        {
            if (this.Velocity.Y > -1 * this.z_maxSpeed)
            {
                if (this.z_IsSlowingDownY)
                    this.resetYvelocity();
                this.Velocity = new Vector2(this.Velocity.X, this.Velocity.Y - this.z_acceleration);
                this.z_currentYstate = AccelerationState.negative;
            }
        }
        public void accelerateDown()
        {
            if (this.Velocity.Y < this.z_maxSpeed)
            {
                if (this.z_IsSlowingDownY)
                    this.resetYvelocity();
                this.Velocity = new Vector2(this.Velocity.X, this.Velocity.Y + this.z_acceleration);
                this.z_currentYstate = AccelerationState.positive;
            }
        }
        #endregion



        #region UpdateMethod
        //The main Update Method for the Player Ship --------------------------------------------------------
        public void playerShipUpdate(GameTime gameTime, Rectangle viewPort)
        {
            //MissleManager.getCurrent().missleStartPosition = this.getPosition();
            //Check to see if the player is alive
            if (!this.IsAlive)
            {
                //If any lives left, then revive
                if (this.z_lives > 0)
                {
                    this.IsInvincible = true;
                    this.IsAlive = true;
                    this.Position = this.z_startingPosition;
                    this.Health = 100;
                }
                else
                {
                    //Game Over
                    this.Health = 100;
                    return;
                }

            }

            //Update the invincible timer
            if (this.IsInvincible)
            {
                this.InvincibleTimer += gameTime.ElapsedGameTime.Milliseconds;
            }


            //Ensure that the ship can not leave the viewPort ever
            if ((this.Left <= 1 && this.z_currentXstate == AccelerationState.negative) ||
                (this.Right >= (float)viewPort.Width - 1 &&
                 this.z_currentXstate == AccelerationState.positive))
                this.resetXvelocity();
            if ((this.Top <= 1 && this.z_currentYstate == AccelerationState.negative) ||
               (this.Bottom >= (float)viewPort.Height - 1 &&
                this.z_currentYstate == AccelerationState.positive))
                this.resetYvelocity();

            //Bring ship back to screen if ever necessary
            if (this.Left < 0)
                this.Left = 0;
            if (this.Right > viewPort.Width)
                this.Right = viewPort.Width;
            if (this.Top < 0)
                this.Top = 0;
            if (this.Bottom > viewPort.Height)
                this.Bottom = viewPort.Height;

            //Perform the actual update on the ship Object
            this.upDatePosition();


            //Check to see if the ship is slowing down in the X direction
            if (this.z_IsSlowingDownX)
            {
                if (this.z_accelTimerX < 100)
                    this.z_accelTimerX += (float)gameTime.ElapsedGameTime.Milliseconds;
                else
                {
                    this.z_accelTimerX = 0;
                    //Time to bring the x component of velocity closer to zero
                    //Determine if velocity is greater or less than zero by getting the current State of acceleration
                    switch (this.z_currentXstate)
                    {
                        case AccelerationState.negative:
                            //We want to make X velocity more positive, until zero
                            if (this.Velocity.X < 0)
                                this.Velocity = new Vector2(this.Velocity.X + this.z_acceleration,
                                                             this.Velocity.Y);
                            else
                                this.resetXvelocity();
                            break;
                        case AccelerationState.positive:
                            //We want to make X velocity more negative, until zero
                            if (this.Velocity.X > 0)
                                this.Velocity = new Vector2(this.Velocity.X - this.z_acceleration,
                                                             this.Velocity.Y);
                            else
                                this.resetXvelocity();
                            break;
                        default:
                            this.resetXvelocity();
                            break;
                    }
                }
            }

            //Also check to see if the ship is slowing down in the Y direction
            if (this.z_IsSlowingDownY)
            {
                if (this.z_accelTimerY < 100)
                    this.z_accelTimerY += (float)gameTime.ElapsedGameTime.Milliseconds;
                else
                {
                    this.z_accelTimerY = 0;
                    //Time to bring the Y component of velocity closer to zero
                    //Determine if velocity is greater or less than zero by getting the current State of acceleration
                    switch (this.z_currentYstate)
                    {
                        case AccelerationState.negative:
                            //We want to make Y velocity more positive, until zero
                            if (this.Velocity.Y < 0)
                                this.Velocity = new Vector2(this.Velocity.X,
                                                 this.Velocity.Y + this.z_acceleration);
                            else
                                this.resetYvelocity();
                            break;
                        case AccelerationState.positive:
                            //We want to make Y velocity more negative, until zero
                            if (this.Velocity.Y > 0)
                                this.Velocity = new Vector2(this.Velocity.X,
                                                             this.Velocity.Y - this.z_acceleration);
                            else
                                this.resetYvelocity();
                            break;
                        default:
                            this.resetYvelocity();
                            break;
                    }
                }

            }


        }
        #endregion



        #region HelperMethod
        //Helper Methods for updating the player's ship
        private void resetXvelocity()
        {
            this.z_currentXstate = AccelerationState.zero;
            this.Velocity = new Vector2(0, this.Velocity.Y);
            this.z_IsSlowingDownX = false;
        }
        private void resetYvelocity()
        {
            this.z_currentYstate = AccelerationState.zero;
            this.Velocity = new Vector2(this.Velocity.X, 0);
            this.z_IsSlowingDownY = false;
        }
        #endregion


        #region DrawMethod
        //Draw Method for PlayerShip
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (IsInvincible)
                SpriteColor = new Color(.8f, .8f, .8f, (float)z_InvincibleTimer % 500 / 500);
            else
                SpriteColor = Color.White;
            base.Draw(spriteBatch, gameTime);
        }
        #endregion

    }
}
