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
    class FloatingText:GameObject
    {
        private String z_text;
        private Color z_color;
        private static SpriteFont z_font;
        private int z_lifeSpan;
        private int z_originalLifeSpan;
        private static List<FloatingText> z_pool;
        private Vector2 z_textSize;
        public int TextWidth
        { get { return (int)z_textSize.X; } }
        public int TextHeight
        { get { return (int)z_textSize.Y; } }
        public override Vector2 DrawPosition
        {
            get
            {
                return new Vector2(Position.X - z_textSize.X / 2, Position.Y - z_textSize.Y / 2);
            }
        }
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
        public String Text
        {
            get { return z_text; }
            set
            {
                z_text = value;
                z_textSize = z_font.MeasureString(z_text);
            }
        }

        public FloatingText(Vector2 position, String text, Color color, int lifeSpan)
            : base(null)
        {
            Position = position;
            Text = text; ;
            z_color = color;
            z_originalLifeSpan = z_lifeSpan = lifeSpan;
            Velocity = new Vector2(0, -.5f);
            IsAlive = true;
            DrawOnTop = true;
            DrawDepth = .1f;
        }

        public static FloatingText getNewText(Vector2 position, String text, Color color, int lifeSpan)
        {
            FloatingText floater;
            if (z_pool.Count > 0)
            {
                floater = z_pool[z_pool.Count - 1];
                z_pool.RemoveAt(z_pool.Count-1);
                floater.Position = position;
                floater.z_text = text;
                floater.z_color = color;
                floater.z_lifeSpan = lifeSpan;
            }
            else
                floater = new FloatingText(position, text, color, lifeSpan);
            return floater;
        }

        public static void Initialize(ContentManager content, String font)
        {
            z_font = content.Load<SpriteFont>(font);
            z_pool = new List<FloatingText>();
        }

        public override void returnToPool()
        {
            z_pool.Add(this);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            LifeSpan -= gameTime.ElapsedGameTime.Milliseconds;
            z_color.A = (byte) (LifeSpan*256/z_originalLifeSpan);
            spriteBatch.DrawString(z_font, z_text, DrawPosition, z_color, 0, Vector2.Zero, 1, SpriteEffects.None, DrawDepth);
        }
    }
}
