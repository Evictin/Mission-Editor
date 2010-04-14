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
    class Asteroid : IEnemyShip, IExplodable
    {
        public enum AsteroidDensity
        {
            None,
            Lite,
            Moderate,
            Heavy,
            VeryHeavy
        }
        public static float MIN_SPEED = 0.2f;
        public static float MAX_SPEED = 1.3f;
        public static int MAX_SIZE = 3;
        private static int MAXX_SIZE = MAX_SIZE + 1;
        public static int MIN_SIZE = 1;

        //Instance Variables
        private float z_rotationSpeed;
        private int z_size;
        private static List<Asteroid> zs_pool;
        private static List<Texture2D> zs_images;
        private static int z_activeAsteroids;
        private static int z_density;
        private Vector2 z_spriteCenter;
        private static Texture2D zs_explosionSprite;

        // public properties
        public int Size
        {
            get { return z_size; }
            set
            {
                if ((value>=Asteroid.MIN_SIZE)&&(value<=Asteroid.MAX_SIZE))
                    z_size = value;
                else
                    z_size = RandomGen.Next(MIN_SIZE,MAXX_SIZE) ;
                Health = 100 * (z_size);
                PointValue = 10 * (MAXX_SIZE - z_size);
                Width = SpriteWidth / (MAXX_SIZE - z_size);
                Height = SpriteHeight / (MAXX_SIZE - z_size);
                z_spriteCenter = new Vector2(Width/2, Height/2);
            }
        }
        override public Circle HitCircle
        { 
            get 
            { 
                return new Circle(Position, (Width > Height ? Width : Height) / 2); 
            } 
        }
        public override float Speed
        {
            get { return base.Speed; }
            set
            {
                if ((value >= MIN_SPEED) && (value <= MAX_SPEED))
                    base.Speed = value;
                else
                    base.Speed = MathHelper.Lerp(MIN_SPEED, MAX_SPEED,(float) RandomGen.NextDouble());
                z_rotationSpeed = base.Speed / 50 * (RandomGen.Next(2)>0 ? -1 : 1);
            }
        }
        public static AsteroidDensity Density
        {
            get
            {
                switch (z_density)
                {
                    case 0:
                        return AsteroidDensity.None;
                    case 3:
                        return AsteroidDensity.Lite;
                    case 5:
                        return AsteroidDensity.Moderate;
                    case 7:
                        return AsteroidDensity.Heavy;
                    case 15:
                        return AsteroidDensity.VeryHeavy;
                    default:
                        return AsteroidDensity.None;
                }
            }
            set
            {
                switch (value)
                {
                    case AsteroidDensity.None:
                        z_density=0;
                        break;
                    case AsteroidDensity.Lite:
                        z_density=3;
                        break;
                    case AsteroidDensity.Moderate:
                        z_density=5;
                        break;
                    case AsteroidDensity.Heavy:
                        z_density=7;
                        break;
                    case AsteroidDensity.VeryHeavy:
                        z_density = 15;
                        break;
                }
                // if there aren't enough asteroids, then add them
                while (z_activeAsteroids < z_density)
                {
                    EnemyManager.AddEnemy(getNewAsteroid(Vector2.Zero, 0, 0, null));
                }

            }
        }
        public Texture2D ExplosionImage
        {
            get { return zs_explosionSprite; }
            set { zs_explosionSprite = value; }
        }

        //Constructor
        private Asteroid(Texture2D image, int size)
            : base((image == null ? zs_images[RandomGen.Next(zs_images.Count)] : image))
        {
            this.IsKillerObject = true;
            this.IsPickUp = false;
            this.Damage = 100000; // ensure that this will kill the player despite shields, etc
            this.CanTakeDamage = true;
            this.DrawRotation = 0.0f;
            this.z_rotationSpeed = 0f;
            DrawDepth = .8f;
            Size = size;        // health, point value, and draw size are set based on size
        }

        private Asteroid(int size)
            : this(zs_images[RandomGen.Next(zs_images.Count)], size)
        {
        }

        public static void Initialize(ContentManager content, AsteroidDensity density)
        {
            zs_pool = new List<Asteroid>();
            zs_images = new List<Texture2D>();
            for (int i = 1; i <= 9; i++)
            {
                zs_images.Add(content.Load<Texture2D>("Content\\Images\\Asteroids\\Asteroid" + i));
            }
            zs_explosionSprite = content.Load<Texture2D>("Content\\Images\\Asteroids\\Explosion");
            for (int i = 0; i < 5; i++)
            {
                zs_pool.Add(new Asteroid(0));
            }
            z_activeAsteroids = 0;
            Density = density;
        }

        // Retrieves a new asteroid from the pool. If the pool is empty, it creates one from scratch.
        public static Asteroid getNewAsteroid(Vector2 position, int size, float speed, Texture2D image)
        {
            Rectangle viewport = StageManager.GetViewport();
            Asteroid asteroid; ;
            // if there are any enemies in the pool, use one of them
            if (zs_pool.Count > 0)
            {
                asteroid = zs_pool[zs_pool.Count - 1];
                asteroid.Sprite = (image==null ? zs_images[RandomGen.Next(zs_images.Count)] : image);
                asteroid.Size = size;
                zs_pool.RemoveAt(zs_pool.Count - 1);
            }
            else
                asteroid = new Asteroid(image,size); // pool was empty, so create a new enemy

            asteroid.Speed = speed;
            asteroid.Direction = MathHelper.Lerp(1.35f, 1.65f, (float)RandomGen.NextDouble()) * MathHelper.Pi;

            if (position == Vector2.Zero)
                asteroid.Position = new Vector2(RandomGen.Next(viewport.Width), -RandomGen.Next(viewport.Height/5));
            else
                asteroid.Position = position;

            asteroid.IsAlive = true;
            z_activeAsteroids++;
            return asteroid;
        }

        // return this enemy to the pool
        public override void returnToPool()
        {
            Asteroid asteroid;

            if (Health <= 0)
            {
                if (z_size > MIN_SIZE)
                {
                    asteroid = getNewAsteroid(new Vector2(Left, Position.Y), z_size - 1, 0, null);
                    asteroid.Direction = MathHelper.Lerp(1.15f, 1.35f, (float)RandomGen.NextDouble()) * MathHelper.Pi;
                    EnemyManager.AddEnemy(asteroid);
                    asteroid = getNewAsteroid(new Vector2(Right, Position.Y), z_size - 1, 0, null);
                    asteroid.Direction = MathHelper.Lerp(1.65f, 1.85f, (float)RandomGen.NextDouble()) * MathHelper.Pi;
                    EnemyManager.AddEnemy(asteroid);
                }
            }

            zs_pool.Add(this);
            z_activeAsteroids--;

            if (z_activeAsteroids < z_density)
            {
                EnemyManager.AddEnemy(getNewAsteroid(Vector2.Zero,0,0, null));
            }
        }

        public void SpawnExplosion()
        {
            Explosion exp = Explosion.getNewExplosion(this.Position, zs_explosionSprite, 1, 6);
            exp.Width = (int)(Width * 1.5);
            exp.Height = (int)(Height * 1.5);
            exp.FadeAway = true;
            StageManager.AddObject(exp);
        }

        public override void AIUpdate(GameTime gameTime)
        {
            Rectangle viewport = StageManager.GetViewport();
            DrawRotation += z_rotationSpeed;
            //z_rotation += z_rotationSpeed;
            this.upDatePositionWithSpeed();
            if ((this.Top > viewport.Bottom)||(this.Left>viewport.Right)||(this.Right<viewport.Left))
            {
                reset();
                if (z_activeAsteroids > z_density)
                    IsAlive = false;
            }
       }
        //Accessor Methods
        //Mutator Methods

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);
        }

        public override void reset()
        {
            Rectangle viewport = StageManager.GetViewport();
            Sprite = zs_images[RandomGen.Next(zs_images.Count)];
            Size = 0;
            Speed = 0;
            Direction = MathHelper.Lerp(1.35f, 1.65f, (float)RandomGen.NextDouble()) * MathHelper.Pi;
            Position = new Vector2(RandomGen.Next(viewport.Width), -RandomGen.Next(viewport.Height / 5));
        }

    }
}
