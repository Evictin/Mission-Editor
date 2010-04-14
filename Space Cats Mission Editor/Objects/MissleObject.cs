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
    class MissileObject : GameObject
    {
        //Instance Variables ---------------------------------------------------------
        private int z_damage;
        public int Damage
        {
            get { return z_damage; }
            set { z_damage = value; }
        }

        //Constructor ----------------------------------------------------------------
        public MissileObject(Texture2D MissleSprite)
            : base(MissleSprite)
        {
        }

        //Accessor Methods -----------------------------------------------------------       
 
        //Mutator Methods ------------------------------------------------------------       

        //Other Methods --------------------------------------------------------------
        public void upDateMissle()
        {
            this.upDatePositionWithSpeed();
        }
        virtual public void returnToPool()
        {
        }
   
    }
}
