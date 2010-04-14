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

namespace Space_Cats_V1._2
{
    class GameObject
    {
        //Declare Instance Variables ----------------------------------------------------------------------------
        private int z_ID;
        private Texture2D z_sprite;
        private int z_spriteCols, z_spriteRows;
        private int z_spriteWidth, z_spriteHeight;
        protected int z_animationDelay;
        protected int z_animationTimer;
        protected int z_currentSprite;
        protected int z_firstSpriteToDraw, z_lastSpriteToDraw;
        private Vector2 z_position;
        private Vector2 z_velocity;
        private float z_speed;
        private bool z_isAlive;
        //For Hit Detection Purposes
        private bool z_isKillerObject;
        private bool z_isPickUp;
        private int z_pointValue;
        private bool z_canTakeDamage;
        private int z_width, z_height;
        private bool z_drawOnTop;
        private bool z_hasPool;
        private Color z_spriteColor;
        private float z_spriteOrientation;
        private float z_drawRotation;
        private float z_drawDepth;


        private static Random z_random;
        public static Random RandomGen
        { get { return z_random; } }

        #region Public properties
        public int ID
        {
            get { return z_ID; }
            set { z_ID = value; }
        }
        virtual public float Speed
        {
            get { return z_speed; }
            set { z_speed = value; }
        }
        public Vector2 Position
        {
            get { return z_position; }
            set { z_position = value; }
        }
        public int Left
        {
            get { return (int)(z_position.X - z_width / 2); }
            set { z_position.X = value + z_width / 2; }
        }
        public int Right
        {
            get { return (int)(z_position.X + z_width / 2); }
            set { z_position.X = value - z_width / 2; }
        }
        public int Top
        {
            get { return (int)(z_position.Y - z_height / 2); }
            set { z_position.Y = value + z_height / 2; }
        }
        public int Bottom
        {
            get { return (int)(z_position.Y + z_height / 2); }
            set { z_position.Y = value - z_height / 2; }
        }
        public int Height
        {
            get { return z_height; }
            set { z_height = value; }
        }
        public int Width
        {
            get { return z_width; }
            set { z_width = value; }
        }
        public Rectangle DrawRect
        {
            get { return new Rectangle((int)z_position.X, (int)z_position.Y, z_width, z_height); }
        }
        virtual public Vector2 DrawPosition
        { get { return new Vector2(z_position.X - z_width / 2, z_position.Y - z_height / 2); } }
        public Vector2 SpriteCenter
        { get { return new Vector2((float)SpriteWidth / 2, (float)SpriteHeight / 2); } }
        public Vector2 Velocity
        {
            get { return z_velocity; }
            set { z_velocity = value; }
        }
        public float Direction
        {
            get { return (float)Math.Atan2(-z_velocity.Y, -z_velocity.X); }
            set { z_velocity = new Vector2((float)Math.Cos(value), -(float)Math.Sin(value)); }
        }
        public float DrawRotation
        {
            get { return z_drawRotation; }
            set { z_drawRotation = value; }
        }
        public Vector2 VelocityWithSpeed
        { get { return z_velocity * z_speed; } }
        public bool HasPool
        {
            get { return z_hasPool; }
            set { z_hasPool = value; }
        }
        public bool IsAlive
        { 
            get { return z_isAlive; }
            set { z_isAlive = value; }
        }
        public bool CanTakeDamage
        {
            get { return z_canTakeDamage; }
            set { z_canTakeDamage = value; }
        }
        public bool IsKillerObject
        {
            get { return z_isKillerObject; }
            set
            {
                z_isKillerObject = value;
                if (value)
                    z_isPickUp = false;
            }
        }
        public bool IsPickUp
        { 
            get { return z_isPickUp; }
            set
            {
                z_isPickUp = value;
                if (value)
                    z_isKillerObject = false;
            }
        }
        public bool DrawOnTop
        {
            get { return z_drawOnTop; }
            set { z_drawOnTop = value; }
        }
        public Texture2D Sprite
        {
            get { return z_sprite; }
            set 
            {
                if (value != null)
                {
                    z_sprite = value;
                    SpriteRows = SpriteCols = 1;
                    z_currentSprite = z_firstSpriteToDraw = z_lastSpriteToDraw = 0;
                }
            }
        }
        public int PointValue
        {
            get { return z_pointValue; }
            set { z_pointValue = value; }
        }
        virtual public Circle HitCircle
        { get { return new Circle(z_position, (z_width < z_height ? z_width : z_height) / 2); } }
        virtual public Rectangle HitRec
        { get { return new Rectangle((int)z_position.X - z_width / 2, (int)z_position.Y - z_height / 2, z_width, z_height); } }
        public int SpriteCols
        {
            get { return z_spriteCols;  }
            set
            {
                z_spriteCols = value;
                z_width = z_sprite.Width / value;
                z_spriteWidth = z_width;
                z_firstSpriteToDraw = 0;
                z_lastSpriteToDraw = z_spriteCols * z_spriteRows - 1;
            }
        }
        public int SpriteRows
        {
            get { return z_spriteRows; }
            set
            {
                z_spriteRows = value;
                z_height = z_sprite.Height / value;
                z_spriteHeight = z_height;
                z_firstSpriteToDraw = 0;
                z_lastSpriteToDraw = z_spriteCols * z_spriteRows - 1;
            }
        }
        public int FirstSpriteToDraw
        {
            get { return z_firstSpriteToDraw; }
            set 
            { 
                z_firstSpriteToDraw = value;
                if (z_currentSprite < z_firstSpriteToDraw)
                    z_currentSprite = z_firstSpriteToDraw;
            }
        }
        public int LastSpriteToDraw
        {
            get { return z_lastSpriteToDraw; }
            set 
            { 
                z_lastSpriteToDraw = value;
                if (z_currentSprite > z_lastSpriteToDraw)
                    z_currentSprite = z_lastSpriteToDraw;
            }
        }
        public int AnimationDelay
        {
            get { return z_animationDelay; }
            set { z_animationDelay = value; }
        }
        public int AnimationTimer
        {
            get { return z_animationTimer; }
            set
            {
                z_animationTimer = value;
                if (z_animationTimer >= z_animationDelay)
                {
                    z_animationTimer = 0;
                    CurrentSprite++;
                }
                if (z_animationDelay < 0)
                {
                    z_animationTimer = z_animationDelay; 
                    CurrentSprite--;
                }
            }
        }
        public int CurrentSprite
        {
            get { return z_currentSprite; }
            set
            {
                z_currentSprite = value;
                if (value > z_lastSpriteToDraw)
                    z_currentSprite = z_firstSpriteToDraw;
                else if (value < z_firstSpriteToDraw)
                    z_currentSprite = z_lastSpriteToDraw;
            }
        }
        public int NumSprites
        { get { return z_spriteCols * z_spriteRows; } }
        public Color SpriteColor
        {
            get { return z_spriteColor; }
            set { z_spriteColor = value; }
        }
        public Rectangle SpriteRect
        { get { return new Rectangle(z_spriteWidth * (z_currentSprite % z_spriteCols), z_spriteHeight * (z_currentSprite / z_spriteCols), z_spriteWidth, z_spriteHeight); } }
        public int SpriteWidth
        { get { return z_spriteWidth; } }
        public int SpriteHeight
        { get { return z_spriteHeight; } }
        public float SpriteOrientation
        {
            get { return z_spriteOrientation; }
            set { z_spriteOrientation = value; }
        }
        public float DrawDepth
        {
            get { return z_drawDepth; }
            set { z_drawDepth = value; }
        }
        #endregion

        //Constructor -------------------------------------------------------------------------------------------
        public GameObject(Texture2D loadedTexture)
        {
            //Initialize all instance variables
            this.Sprite = loadedTexture;
            this.z_animationDelay = 100;
            this.z_animationTimer = 0;
            this.z_position = Vector2.Zero;
            this.z_velocity = Vector2.Zero;
            this.z_speed = 1.0f;
            this.z_isAlive = true;
            this.z_isKillerObject = false;
            this.z_isPickUp = false;
            this.z_currentSprite = 0;
            this.z_drawOnTop = false;
            this.z_hasPool = false;
            this.z_spriteColor = Color.White;
            SpriteOrientation = -MathHelper.PiOver2;
            DrawRotation = 0;
            DrawDepth = 0; // front
            if (z_random == null)
                z_random = new Random();
        }

        //Access Methods ----------------------------------------------------------------------------------------

        //Mutator Methods ---------------------------------------------------------------------------------------
        public void resetHeightWidth()
        {
            z_height = z_sprite.Height / z_spriteRows;
            z_width = z_sprite.Width / z_spriteCols;
        }
        //Other Methods -----------------------------------------------------------------------------------------
        virtual public void upDatePosition()
        {
            this.z_position += this.z_velocity;
        }

        public static Vector2 VectorFromAngle(float angle)
        {
            return new Vector2((float)Math.Cos(angle), -(float)Math.Sin(angle));
        }

        public static float AngleFromVector(Vector2 vec)
        {
            return (float) Math.Atan2(-vec.Y, -vec.X);
        }

        //Use this method for updating position if a speed is set
        virtual public void upDatePositionWithSpeed()
        {
            this.z_position += z_velocity * z_speed;
        }

        virtual public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            AnimationTimer+=gameTime.ElapsedGameTime.Milliseconds;
            spriteBatch.Draw(Sprite, DrawRect, SpriteRect, SpriteColor, DrawRotation - SpriteOrientation, SpriteCenter, SpriteEffects.None, 0);
        }

        virtual public void Draw(SpriteBatch spriteBatch)
        {
            // This assumes only one sprite for the object, hence no animation
            if (IsAlive)
                spriteBatch.Draw(this.z_sprite, this.DrawRect, SpriteColor);
        }

        virtual public void returnToPool()
        {
        }
    }
}
