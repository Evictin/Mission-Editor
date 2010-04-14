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

namespace Space_Cats_V1._2
{
    abstract class IEnemyShip : GameObject
    {
        private IArtificialIntelligence z_AI;
        private int z_health;
        private int z_damage;
        public int Health
        {
            get { return z_health; }
            set
            {
                z_health = value;
                IsAlive = (value > 0);
            }
        }
        public int Damage
        {
            get { return z_damage; }
            set { z_damage = value; }
        }
        public IArtificialIntelligence AI
        {
            get { return z_AI; }
            set
            {
                if (value == null)
                    z_AI = null;
                else
                    z_AI = value.clone();
            }
        }

        public IEnemyShip(Texture2D loadedSprite)
            : base(loadedSprite)
        {
        }

        //This method should be calaulated using some sort of AI
        abstract public void AIUpdate(GameTime gameTime);
        abstract public void reset();
        public void transferAI(IArtificialIntelligence ai)
        {
            z_AI = ai;
        }
        
    }
}
