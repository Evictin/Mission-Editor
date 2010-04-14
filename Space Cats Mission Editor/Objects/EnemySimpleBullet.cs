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
    class EnemySimpleBullet : IEnemyShip
    {
        // Instance Variables

        // static Variables
        private static List<EnemySimpleBullet> zs_pool;
        private static Texture2D zs_image;
        private static SoundEffect zs_sound;
        private static Rectangle zs_viewport;
        private static PlayerShip zs_playerShip;
        private static Texture2D zs_explosionSprite;
        public Rectangle Viewport
        { set { zs_viewport = value; } }
        public Texture2D ExplosionImage
        {
            get { return zs_explosionSprite; }
            set { zs_explosionSprite = value; }
        }

        //Constructor - this is private to force ppl to call the static function
        private EnemySimpleBullet()
            : base(zs_image)
        {
            this.Velocity = Vector2.UnitY;
            this.Speed = 5;
            this.IsKillerObject = true;
            this.IsPickUp = false;
            this.CanTakeDamage = false;
            this.Damage = 100;
            this.HasPool = true;
            DrawDepth = .9f;
        }

        //Accessors

        //Mutators

        // This function initializes the internal pool and loads in the image to use to create all the 
        // 'Enemy1' type ships. It also creates a few bullets in the pool for fast retrieval.
        public static void Initialize(ContentManager content, Rectangle viewport)
        {
            zs_pool = new List<EnemySimpleBullet>();
            zs_sound = content.Load<SoundEffect>("Content\\Audio\\SoundFX\\LaserPellet");
            zs_image = content.Load<Texture2D>("Content\\Images\\Missiles\\EnemyBulletRed");
            zs_playerShip = PlayerShip.getInstance();
            zs_viewport = viewport;

            for (int i = 0; i < 50; i++)
            {
                zs_pool.Add(new EnemySimpleBullet());
            }
        }

        // Retrieves a bullet from the pool. If the pool is empty, it creates one from scratch.
        public static EnemySimpleBullet getNewBullet(Vector2 startLocation)
        {
            EnemySimpleBullet bullet;
            // if the pool is empty, refill it
            if (zs_pool.Count == 0)
            {
                for (int i = 0; i < 50; i++)
                {
                    zs_pool.Add(new EnemySimpleBullet());
                }
            }
            bullet = zs_pool[zs_pool.Count - 1];
            zs_pool.RemoveAt(zs_pool.Count - 1);
            // set the bullet parameters
            bullet.Position = startLocation;
            
            // play the bullet launch sound and exit
            zs_sound.Play(.2f,0f,0);
            return bullet;
        }

        // return this enemy to the pool
        public override void returnToPool()
        {
            this.reset();
            zs_pool.Add(this);
        }

        public override void AIUpdate(GameTime gameTime)
        {
            if (this.Top > zs_viewport.Height)
            {
                this.IsAlive = false;
                return;
            }
            this.upDatePositionWithSpeed();
        }

        override public void reset()
        {
            this.IsAlive = true;
        }
    }
}
