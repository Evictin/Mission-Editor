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
using System.IO;

namespace Space_Cats_V1._2
{
    class EnemyManager
    {
        /*
         * Logic Note: A lot of these variables should be converted into lists. Because 
         * Each wave will probably need to have it's own counters if we decide to allow
         * multiple waves to spawn at the same time.
         * */
        //Instance Variables
        private static List<IEnemyShip> z_enemyShips;
        private ContentManager z_content;
        private SpriteBatch z_spriteBatch;
        private Rectangle z_viewPort;
        //A counter for the Update Method
        private float z_counter;
        //another counter for spreading out the enemies as they spawn
        private float z_interval;
        //A counter for keeping track how many enemies are spawned
        private int z_EnemiesSpawn;
        //Booleans for activating a type of wave of enemies
        //E1W1 stands for Enemey 1, Wave 1
        private bool z_ActivateE1W1;
        private PlayerShip playerShip = PlayerShip.getInstance(null, Vector2.Zero);
        private static EnemyManager z_instance = null;
        private List<IArtificialIntelligence> z_AIList;

        public static EnemyManager getInstance()
        {
            return z_instance;
        }

        public static EnemyManager getInstance(ContentManager content, SpriteBatch spriteBatch, Rectangle viewPort)
        {
            if (z_instance == null)
                z_instance = new EnemyManager(content, spriteBatch, viewPort);
            return z_instance;
        }

        //Constructor
        private EnemyManager(ContentManager content, SpriteBatch spriteBatch, Rectangle viewPort)
        {
            if (z_instance == null)
                z_instance = this;

            z_enemyShips = new List<IEnemyShip>();
            this.z_content = content;
            this.z_spriteBatch = spriteBatch;
            this.z_viewPort = viewPort;
            this.z_counter = 0;
            this.z_interval = 0;
            this.z_EnemiesSpawn = 0;

            // Initialize the enemy1 pool
            Enemy1.Initialize(this.z_content);
            EnemySimpleBullet.Initialize(this.z_content, this.z_viewPort);
            Asteroid.Initialize(this.z_content, Asteroid.AsteroidDensity.None);
            Explosion.Initialize();
        }

       
        // Accessors
        public static List<IEnemyShip> getEnemiesList()
        {
            return z_enemyShips;
        }
 
        //Mutators
        public static void AddEnemy(IEnemyShip enemy)
        {
            z_enemyShips.Add(enemy);
        }

        //Update all Enemies in the list method
        public void mainUpdate(GameTime gameTime)
        {
            this.z_counter += (float)gameTime.ElapsedGameTime.Milliseconds;

            for (int i = 0; i< z_enemyShips.Count;i++)
            {
                z_enemyShips[i].AIUpdate(gameTime);
                if (!z_enemyShips[i].IsAlive)
                {   
                    z_enemyShips[i].returnToPool();
                    z_enemyShips.RemoveAt(i);
                }
            }

            for (int i = 0; i < z_enemyShips.Count; i++)
            {
                //Check for collision with player ship
                if (!this.playerShip.IsInvincible &&
                    z_enemyShips[i].HitCircle.Intersects(playerShip.HitCircle))
                {
                    // a collision will reduce the enemies health to zero
                    z_enemyShips[i].Health=0;
                    if (z_enemyShips[i] is IExplodable)
                        ((IExplodable)z_enemyShips[i]).SpawnExplosion();
                    // but will reduce the players life by the amount damage the enemy deals (it could be a missile, etc)
                    this.playerShip.Health -= z_enemyShips[i].Damage;
                }
            }
        }

        //Draw Method
        public void draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (IEnemyShip enemy in z_enemyShips)
                enemy.Draw(spriteBatch, gameTime);
        }

        static public int totalEnemyCount()
        {
            return z_enemyShips.Count;
        }

        //Main reset
        public void resetAllEnemies()
        {
            Asteroid.Density = Asteroid.AsteroidDensity.None;
            while (z_enemyShips.Count>0)
            {
                z_enemyShips[0].returnToPool();
                z_enemyShips.RemoveAt(0);
            }
            this.z_ActivateE1W1 = false;
        }
    }
}
