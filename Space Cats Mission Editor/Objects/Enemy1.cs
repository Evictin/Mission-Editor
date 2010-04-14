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
        //private static Random zs_randomGen;
        private static List<Enemy1> zs_pool;
        private static Texture2D zs_image;
        private static EnemyManager zs_enemyManager;
        private static Texture2D zs_explosionSprite;

        //Constructor - this is private to force ppl to call the static function
        private Enemy1(Texture2D loadedSprite, IArtificialIntelligence ai)
            : base(loadedSprite)
        {
            this.IsKillerObject = true;
            this.IsPickUp = false;
            this.fireTime = 0;
            this.fireCoolOff = MathHelper.Lerp(1000, 5000, (float)RandomGen.NextDouble());
            this.PointValue = 100;
            this.Health = 100;
            this.Damage = 100000; // ensure that this will kill the player despite shields, etc
            this.CanTakeDamage = true;
            DrawRotation = -MathHelper.PiOver2;
            DrawDepth = .4f;
            if (ai != null)
            {
                AI = ai;
                this.Position = this.AI.getStartingPosition();
            }
        }

        //Accessors

        //Mutators

        // This function initializes the internal pool and loads in the image to use to create all the 
        // 'Enemy1' type ships. It also creates a few enemies in the pool for fast retrieval.
        public static void Initialize(ContentManager content)
        {
            zs_pool = new List<Enemy1>();
            zs_enemyManager = EnemyManager.getInstance();
            zs_image = content.Load<Texture2D>("Content\\Images\\EnemyShips\\EnemyShip1");
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
                enemy.AI = ai;
                zs_pool.RemoveAt(zs_pool.Count - 1);
            }
            else 
                enemy = new Enemy1(zs_image, ai); // pool was empty, so create a new enemy

            // reset the enemy before use
            enemy.IsAlive = true;
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
            if (this.AI.okToRemove())
            {
                this.IsAlive = false;
                return;
            }

            fireTime += gameTime.ElapsedGameTime.Milliseconds;
            if (fireTime>=fireCoolOff)
            {
                EnemyManager.AddEnemy(EnemySimpleBullet.getNewBullet(this.Position));
                fireCoolOff = MathHelper.Lerp(1000, 5000, (float)RandomGen.NextDouble());
                fireTime = 0;
            }

            this.Velocity = this.AI.calculateNewVelocity(this.Position, gameTime);
            if (((AI_Script)AI).getScriptStatus() == AI_PathNodeStatus.STAGED)
            {
                DrawRotation = GameObject.AngleFromVector(PlayerShip.getInstance().Position - Position);
            }
            else
            {
                DrawRotation = Direction;
            }

            this.upDatePosition();
        }

        override public void reset()
        {
            this.Velocity = Vector2.Zero;
            this.Speed = 1f;
            this.fireTime = 0;
            this.fireCoolOff = MathHelper.Lerp(1000, 5000, (float)RandomGen.NextDouble());
            this.AI.reset();
            this.Health = 100;
            this.Position = AI.getStartingPosition();
        }
    }
}
