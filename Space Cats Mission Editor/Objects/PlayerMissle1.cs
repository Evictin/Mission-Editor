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
    class PlayerMissile1 : MissileObject
    {
        //Instance Variables ---------------------------------------------------------
        private static List<PlayerMissile1> z_pool;
        private static Texture2D zs_image;
        private static SoundEffect zs_fireSound;
        
        //Constructor ----------------------------------------------------------------
        public PlayerMissile1()
            : base(zs_image)
        {
            this.Velocity = -Vector2.UnitY;
            this.Speed = 5;
            this.IsAlive = true;
            this.Damage = 100;
            DrawDepth = .9f;
        }
        
        public static void Initialize(ContentManager content)
        {
            z_pool = new List<PlayerMissile1>();
            zs_image = content.Load<Texture2D>("Content\\Images\\Missiles\\PlayerBulletBlue");
            zs_fireSound = content.Load<SoundEffect>("Content\\Audio\\SoundFX\\LaserPellet");

            for (int i = 0; i < 20; i++)
                z_pool.Add(new PlayerMissile1());
        }

        //Accessor Methods -----------------------------------------------------------
        public static PlayerMissile1 GetNextMissile(Vector2 coords)
        {
            PlayerMissile1 missile;
            if (z_pool.Count == 0)
            {
                for (int i = 0; i < 5; i++)
                    z_pool.Add(new PlayerMissile1());
            }
            missile = z_pool[z_pool.Count-1];
            z_pool.RemoveAt(z_pool.Count-1);
            missile.Position = coords;
            zs_fireSound.Play(.2f, 0, 0);
            return missile;
        }
        
        //Mutator Methods ------------------------------------------------------------
        public void reset()
        {
            this.IsAlive = true;
        }
        
        //Other Methods --------------------------------------------------------------
        public override void returnToPool()
        {
            z_pool.Add(this);
            this.reset();
        }

    }
}
