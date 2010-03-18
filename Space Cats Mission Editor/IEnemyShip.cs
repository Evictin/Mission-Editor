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
        private static Random z_random;

        public IEnemyShip(Texture2D loadedSprite)
            : base(loadedSprite)
        {
        }
        //This method should be calaulated using some sort of AI
        abstract public void AIUpdate(GameTime gameTime);

        //abstract public bool readyToFire();
        abstract public void reset();

        public static Random getRandom()
        {
            if (z_random == null)
                z_random = new Random();
            return z_random;
        }

        public IArtificialIntelligence getAI()
        {
            return z_AI;
        }

        abstract public void returnToPool();        

        public void setAI(IArtificialIntelligence ai)
        {
            if (ai == null)
                z_AI = null;
            else
                z_AI = ai.clone();
        }

        public void transferAI(IArtificialIntelligence ai)
        {
            z_AI = ai;
        }
        
    }
}
