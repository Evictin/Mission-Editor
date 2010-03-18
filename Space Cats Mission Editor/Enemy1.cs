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
    class Enemy1 : IEnemyShip
    {
        //Instance Variables
        public float fireTime;
        public float fireCoolOff;
        private static Random zs_randomGen;
        private static List<Enemy1> zs_pool;
        private static Texture2D zs_image;

        //Constructor - this is private to force ppl to call the static function
        private Enemy1(Texture2D loadedSprite, IArtificialIntelligence ai)
            : base(loadedSprite)
        {
            this.setIsKillerObject(true);
            this.setIsPickUp(false);
            this.fireTime = 0;
            this.fireCoolOff = 1000;
            if (ai != null)
            {
                setAI(ai);
                this.setPosition(this.getAI().getStartingPosition());
            }
        }

        //Accessors

        //Mutators

        // This function initializes the internal pool and loads in the image to use to create all the 
        // 'Enemy1' type ships. It also creates a few enemies in the pool for fast retrieval.
        public static void Initialize(ContentManager content)
        {
            zs_pool = new List<Enemy1>();
            zs_image = content.Load<Texture2D>("Content\\Images\\EnemyShips\\EnemyShip1");
            zs_randomGen = new Random();
            for (int i = 0; i < 5; i++)
            {
                zs_pool.Add(new Enemy1(zs_image, null));
            }
        }

        // Retrieves a new enemy from the pool. If the pool is empty, it creates one from scratch.
        public static Enemy1 getNewEnemy(IArtificialIntelligence ai)
        {
            Enemy1 enemy;
            // This must be called with an AI for the critter
            if (ai == null)
                return null;
            // if there are any enemies in the pool, use one of them
            if (zs_pool.Count > 0)
            {
                enemy = zs_pool[zs_pool.Count - 1];
                enemy.setAI(ai);
                zs_pool.RemoveAt(zs_pool.Count - 1);
            }
            else 
                enemy = new Enemy1(zs_image, ai); // pool was empty, so create a new enemy

            // reset the enemy before use
            enemy.setIsAlive(true);
            return enemy;
        }

        // return this enemy to the pool
        public override void returnToPool()
        {
            this.reset();
            zs_pool.Add(this);
        }
        
        public override void AIUpdate(GameTime gameTime)
        {
            float time = (float)gameTime.TotalGameTime.TotalMilliseconds;
            if (fireTime == 0)
            {
                this.fireCoolOff = MathHelper.Lerp(1000, 5000, (float)zs_randomGen.NextDouble());
                fireTime = time + this.fireCoolOff;
            }
            if (time > fireTime)
            {
                fireTime = time + this.fireCoolOff;
//                MissleManager.getCurrent().fireEnemyMissle(this.getPosition(), this.getSprite());
            }
            this.fireCoolOff = MathHelper.Lerp(1000, 5000, (float)zs_randomGen.NextDouble());
            if (this.getAI().okToRemove())
            {
                this.setIsAlive(false);
                return;
            }

            this.setVelocity(this.getAI().calculateNewVelocity(this.getPosition(), gameTime));

            this.upDatePosition();
            this.setHitRec(new Rectangle((int)this.getPosition().X, (int)this.getPosition().Y,
                          (int)this.getSprite().Width, (int)this.getSprite().Height));
        }

        override public void reset()
        {
            this.setVelocity(Vector2.Zero);
            this.setSpeed(1.0f);
            this.setIsAlive(false);
            this.setHitRec(new Rectangle(0, 0, 0, 0));
            this.setIsKillerObject(false);
            this.setIsPickUp(false);
            this.fireTime = 0;
            this.fireCoolOff = 1000;
            this.getAI().reset();
            this.setPosition(getAI().getStartingPosition());
        }
    }
}
