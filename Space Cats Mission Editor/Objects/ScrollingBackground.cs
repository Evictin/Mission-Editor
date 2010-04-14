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
    class ScrollingBackground : GameObject
    {
        //Constructor
        public ScrollingBackground(Texture2D background)
            : base(background)
        {
            this.Velocity = Vector2.UnitY;
            Speed = 0.4f;
            Top = Left = 0;
            DrawDepth = 1f;
        }

        //Methods
        // reset position to the tope of the screen if at the bottom
        public override void upDatePosition()
        {
            base.upDatePositionWithSpeed();
            Rectangle viewport = StageManager.GetViewport();
            if (Top > viewport.Bottom)
                Top = viewport.Top;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Rectangle view = StageManager.GetViewport();
            spriteBatch.Draw(Sprite, new Rectangle(0, Top, view.Width, view.Height), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, DrawDepth);
            spriteBatch.Draw(Sprite, new Rectangle(0, Top - view.Height, view.Width, view.Height), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, DrawDepth);
        } 


    }
}
