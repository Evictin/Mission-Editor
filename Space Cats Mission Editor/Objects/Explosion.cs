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
    class Explosion : GameObject
    {
        private int z_lifeSpan;
        private int z_originalLifeSpan;
        private static List<Explosion> z_pool;
        private bool z_fadeAway;
        public int LifeSpan
        {
            get { return z_lifeSpan; }
            set
            {
                z_lifeSpan = value;
                if (value <= 0)
                {
                    IsAlive = false;
                    z_lifeSpan = 0;
                }
            }
        }
        public bool FadeAway
        {
            get { return z_fadeAway; }
            set { z_fadeAway = value; }
        }

        public Explosion(Vector2 position, Texture2D image, int spriteCols, int spriteRows)
            : base(image)
        {
            Position = position;
               Speed = 0f;
            Velocity = Vector2.Zero;
            IsAlive = true;
            DrawOnTop = true;
            SpriteRows = spriteRows;
            SpriteCols = spriteCols;
            AnimationDelay = 50;
            FadeAway = true;
            DrawDepth = .2f;
            z_originalLifeSpan = LifeSpan = AnimationDelay * (NumSprites-1);
        }

        public static Explosion getNewExplosion(Vector2 position, Texture2D image, int spriteCols, int spriteRows)
        {
            Explosion exp;
            if (z_pool.Count > 0)
            {
                exp = z_pool[z_pool.Count - 1];
                z_pool.RemoveAt(z_pool.Count - 1);
                exp.Position = position;
                exp.Sprite = image;
                exp.SpriteRows = spriteRows;
                exp.SpriteCols = spriteCols;
                exp.AnimationDelay = 50;
                exp.z_originalLifeSpan = exp.LifeSpan = exp.AnimationDelay * (exp.NumSprites - 1);
            }
            else
                exp = new Explosion(position, image, spriteCols, spriteRows);
            
            return exp;
        }

        public static void Initialize()
        {
            z_pool = new List<Explosion>();
        }

        public override void returnToPool()
        {
            z_pool.Add(this);
        }

        public void reset()
        {
         
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            LifeSpan -= gameTime.ElapsedGameTime.Milliseconds;
            Color c = SpriteColor;
            if (FadeAway)
            {
                c.A = (byte)(LifeSpan * 128 / z_originalLifeSpan + 128);
                SpriteColor = c;
            }
            base.Draw(spriteBatch, gameTime);
        }
    }
}
