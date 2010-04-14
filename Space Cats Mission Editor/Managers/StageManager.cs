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
    class StageManager
    {
        #region Variables
        //Declare Instance Variables ----------------------------------------------------------------------------
        private SpriteBatch z_spriteBatch;
        private static List<GameObject> z_objects;
        private static Rectangle z_viewportRec;
        private float z_gameTimer;
        private float z_accelTimerX = 0;
        private float z_accelTimerY = 0;
        private float z_interval1 = 15;
        private GameObject z_achivementFail;
        private bool z_achivementFailUnlocked = false;
        private ContentManager z_contentManager;
        private GamePadState z_previousGamePadState = GamePad.GetState(PlayerIndex.One);
        private KeyboardState z_previousKeyboardState = Keyboard.GetState();
        private Vector2 z_startingPosition;
        private bool z_isGameOver;
        

        private MissionManager z_missionManager;
        //The Asteroid Manager
        // private AsteroidManager z_asteroidManager;
        //The Missle Manager
        private MissleManager z_missleManager;
        //The Enemy Manager
        private EnemyManager z_enemyManager;
        //Variables for GameObjects
        private PlayerShip z_playerShip;
        //Variables For Music
        private Song z_MarksSong;
        private bool z_songStart = false;
        private SoundEffect z_achievementSound;
        //Variables For Text Fonts
        private SpriteFont z_timerFont;
        private SpriteFont z_livesFont;
        #endregion

        public static Rectangle Viewport
        {
            get { return z_viewportRec; }
            set { z_viewportRec = value; }
        }

        public StageManager(SpriteBatch spriteBatch, Rectangle viewPort, ContentManager content)
        {
            this.z_isGameOver = false;
            this.z_spriteBatch = spriteBatch;
            z_viewportRec = viewPort;
            this.z_contentManager = content;

            z_objects = new List<GameObject>();

            z_objects.Add(new ScrollingBackground(this.z_contentManager.Load<Texture2D>("Content\\Textures\\Starscape4")));

            //Set the starting position for player's ship
            this.z_startingPosition = new Vector2(z_viewportRec.Center.X,
                                                    z_viewportRec.Height - 80);

            //Create the player ship
            this.z_playerShip = PlayerShip.getInstance(this.z_contentManager.Load<Texture2D>("Content\\Images\\ship1"), 
                                               this.z_startingPosition);

            // create the mission manager
            this.z_missionManager = new MissionManager(z_contentManager);

            //Set the player alive
            this.z_playerShip.IsAlive = true;

            //Load the Music
            this.z_MarksSong = this.z_contentManager.Load<Song>("Content\\Audio\\Music\\ATreeFalls");
            MediaPlayer.IsRepeating = true;

            //Load Fonts
            this.z_timerFont = this.z_contentManager.Load<SpriteFont>("Content\\Fonts\\TimerFont");
            this.z_livesFont = this.z_contentManager.Load<SpriteFont>("Content\\Fonts\\LivesFont");

            FloatingText.Initialize(z_contentManager, "Content\\Fonts\\TimerFont");

            //Load Achivement Stuff
            this.z_achivementFail = new GameObject(this.z_contentManager.Load<Texture2D>("Content\\Images\\AchievementFailed"));
            this.z_achivementFail.IsAlive = false;
            this.z_achivementFail.Position = new Vector2((z_viewportRec.Width / 2) - (this.z_achivementFail.Sprite.Width / 2),
                                                            z_viewportRec.Height - 100);
            this.z_achievementSound = this.z_contentManager.Load<SoundEffect>("Content\\Audio\\SoundFX\\AchievementSound");

            //Load the Settings for the MissleManager
            this.z_missleManager = new MissleManager(this.z_contentManager, this.z_spriteBatch);

            //Load the Settings for the EnemyManager
            this.z_enemyManager = EnemyManager.getInstance(this.z_contentManager, this.z_spriteBatch, z_viewportRec);
        }

        public static Rectangle GetViewport()
        {
            return z_viewportRec;
        }

        public static void AddObject(GameObject obj)
        {
            z_objects.Add(obj);
        }

        public void Update(KeyboardState currentKeyboardState, GamePadState currentPadState,
                           KeyboardState previousKeyboardState, GamePadState previousPadState, GameTime gameTime)
        {
            //Update GameTimer
            this.z_gameTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // update the mission manager
            z_missionManager.update(gameTime);

            for (int i = 0; i < z_objects.Count; i++)
            {
                z_objects[i].upDatePosition();
                if (!z_objects[i].IsAlive)
                {
                    z_objects.RemoveAt(i);
                }
            }
            
            //Update the backgroundManager
            //this.z_backgroundManager.Update();

            //Check Achivements
            //if (this.z_gameTimer > this.z_interval1 && !this.z_achivementFailUnlocked)
            //{
            //    this.z_achivementFail.IsAlive = true;
            //    this.z_achievementSound.Play();
            //    this.z_achivementFailUnlocked = true;
            //}
            //if (this.z_gameTimer > 21)
            //    this.z_achivementFail.IsAlive = false;


            //Play Background Music
            if (!this.z_songStart)
            {
                MediaPlayer.Play(this.z_MarksSong);
                MediaPlayer.Volume = 0.7f;
                this.z_songStart = true;
            }

           

            //Update Asteroid Manager
            // this.z_asteroidManager.updateAsteroids(this.z_playerShip);

            //########### Input for Controls and Options ########################################


            //For Xbox controller 1
            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                // Allows the game to exit
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                {
                    //Handle the pause state
                }

                //Local Variables for 360 controller
                GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

                //Input for moving the player's ship on the xbox360
                //Acceleration needs to be worked on for xbox controller
                this.z_playerShip.Velocity = new Vector2(gamePadState.ThumbSticks.Left.X * 0.07f,
                                                        gamePadState.ThumbSticks.Left.Y * 0.07f);
                this.z_playerShip.upDatePosition();

                //Update Missle Manager
                this.z_missleManager.MissleManagerUpdateFriendlyGamepad(gamePadState, this.z_previousGamePadState,
                                                                        this.z_playerShip, this.z_spriteBatch);


                //At the end of Xbox Controller Updates
                this.z_previousGamePadState = gamePadState;
            }


            //For Keyboard
#if !XBOX
            //Local Variables for Keyboard
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                //Handle the pause state
            }

            #region ShipControls
            //Input for accelerating the ship --------------------------------------------------------
            //Move Left
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            {
                if (z_accelTimerX < 100)
                    z_accelTimerX += (float)gameTime.ElapsedGameTime.Milliseconds;
                else
                    if (this.z_playerShip.Left > 1)
                    {
                        this.z_playerShip.accelerateLeft();
                        z_accelTimerX = 0;
                    }
            }

            //Move Right
            if ((keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D)))
            {
                if (z_accelTimerX < 100)
                    z_accelTimerX += (float)gameTime.ElapsedGameTime.Milliseconds;
                else
                    if (this.z_playerShip.Right < z_viewportRec.Width - 1)
                    {
                        this.z_playerShip.accelerateRight();
                        z_accelTimerX = 0;
                    }
            }

            //Move Up
            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
            {
                if (z_accelTimerY < 100)
                    z_accelTimerY += (float)gameTime.ElapsedGameTime.Milliseconds;
                else
                    if (this.z_playerShip.Top > 1)
                    {
                        this.z_playerShip.accelerateUp();
                        z_accelTimerY = 0;
                    }
            }
            //Move Down
            else if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
            {
                if (z_accelTimerY < 100)
                    z_accelTimerY += (float)gameTime.ElapsedGameTime.Milliseconds;
                else
                    if (this.z_playerShip.Bottom < z_viewportRec.Height - 1)
                    {
                        this.z_playerShip.accelerateDown();
                        z_accelTimerY = 0;
                    }
            }


            //Don't Move in the X direction when opposite keys are pressed
            if (((keyboardState.IsKeyDown(Keys.Right) && (keyboardState.IsKeyDown(Keys.Left)) ||
                ((keyboardState.IsKeyDown(Keys.Right) && (keyboardState.IsKeyDown(Keys.A)))) ||
                ((keyboardState.IsKeyDown(Keys.Left) && (keyboardState.IsKeyDown(Keys.D)))) ||
                ((keyboardState.IsKeyDown(Keys.A) && (keyboardState.IsKeyDown(Keys.D)))))))
            {
                this.z_playerShip.Velocity = new Vector2(0, this.z_playerShip.Velocity.Y);
                this.z_playerShip.setIsSlowingDownX(false);
            }

            //Don't Move in the Y direction when opposite keys are pressed
            if (((keyboardState.IsKeyDown(Keys.Up) && (keyboardState.IsKeyDown(Keys.Down)) ||
                ((keyboardState.IsKeyDown(Keys.Up) && (keyboardState.IsKeyDown(Keys.S)))) ||
                ((keyboardState.IsKeyDown(Keys.Down) && (keyboardState.IsKeyDown(Keys.W)))) ||
                ((keyboardState.IsKeyDown(Keys.W) && (keyboardState.IsKeyDown(Keys.S)))))))
            {
                this.z_playerShip.Velocity = new Vector2(this.z_playerShip.Velocity.X, 0);
                this.z_playerShip.setIsSlowingDownY(false);
            }


            //Check if a key was let go for deAccelerating to a stop -------------------------------------
            if ((this.z_previousKeyboardState.IsKeyDown(Keys.Left) && keyboardState.IsKeyUp(Keys.Left)) ||
                (this.z_previousKeyboardState.IsKeyDown(Keys.A) && keyboardState.IsKeyUp(Keys.A)) &&
                (!(keyboardState.IsKeyDown(Keys.Left) && keyboardState.IsKeyUp(Keys.A))) &&
                (!(keyboardState.IsKeyDown(Keys.A) && keyboardState.IsKeyUp(Keys.Left))))
                this.z_playerShip.setIsSlowingDownX(true);

            else if ((this.z_previousKeyboardState.IsKeyDown(Keys.Right) && keyboardState.IsKeyUp(Keys.Right)) ||
                (this.z_previousKeyboardState.IsKeyDown(Keys.D) && keyboardState.IsKeyUp(Keys.D)) &&
                (!(keyboardState.IsKeyDown(Keys.Right) && keyboardState.IsKeyUp(Keys.D))) &&
                (!(keyboardState.IsKeyDown(Keys.D) && keyboardState.IsKeyUp(Keys.Right))))
                this.z_playerShip.setIsSlowingDownX(true);

            if ((this.z_previousKeyboardState.IsKeyDown(Keys.Up) && keyboardState.IsKeyUp(Keys.Up)) ||
                (this.z_previousKeyboardState.IsKeyDown(Keys.W) && keyboardState.IsKeyUp(Keys.W)) &&
                (!(keyboardState.IsKeyDown(Keys.Up) && keyboardState.IsKeyUp(Keys.W))) &&
                (!(keyboardState.IsKeyDown(Keys.W) && keyboardState.IsKeyUp(Keys.Up))))
                this.z_playerShip.setIsSlowingDownY(true);

            else if ((this.z_previousKeyboardState.IsKeyDown(Keys.Down) && keyboardState.IsKeyUp(Keys.Down)) ||
                (this.z_previousKeyboardState.IsKeyDown(Keys.S) && keyboardState.IsKeyUp(Keys.S)) &&
                (!(keyboardState.IsKeyDown(Keys.Down) && keyboardState.IsKeyUp(Keys.S))) &&
                (!(keyboardState.IsKeyDown(Keys.S) && keyboardState.IsKeyUp(Keys.Down))))
                this.z_playerShip.setIsSlowingDownY(true);
            #endregion



            //Perform the Update on The Ship
            this.z_playerShip.playerShipUpdate(gameTime, z_viewportRec);

            //Get if the player is dead
            if (this.z_playerShip.Lives <= 0)
                this.z_isGameOver = true;

            // update the Mission Manager
            this.z_missionManager.update(gameTime);

            //Update Missle Manager
            this.z_missleManager.MissleManagerUpdateFriendlyKeyboard(keyboardState, this.z_previousKeyboardState,
                                                                     this.z_playerShip, this.z_spriteBatch);

            //Update the Enemy Manager
            this.z_enemyManager.mainUpdate(gameTime);


            //End of Keyboard Updates
            this.z_previousKeyboardState = keyboardState;
#endif

        }


        public bool getIsGameOver()
        {
            return this.z_isGameOver;
        }


        //Draw Method --------------------------------------------------------------------------------------------
        public void Draw(GameTime gameTime)
        {
            // draw background and space junk
            foreach (GameObject obj in z_objects)
            {
                if (!obj.DrawOnTop)
                    obj.Draw(this.z_spriteBatch, gameTime);
            }

            //Draw BackgroundManager
            //this.z_backgroundManager.Draw(this.z_spriteBatch);

            //Draw Enemies from EnemyManager
            this.z_enemyManager.draw(this.z_spriteBatch, gameTime);

            //Draw any asteroids from AsteroidManager
            //this.z_asteroidManager.drawAsteroids();

            //Draw Player Ship
            this.z_playerShip.Draw(this.z_spriteBatch, gameTime);

            //Draw Missles
            this.z_missleManager.MissleManagerDrawAllMissles(this.z_spriteBatch, gameTime);

            //Draw Fonts
            this.z_spriteBatch.DrawString(this.z_timerFont, "Time: " + MissionManager.GetInstance().MissionTime,
                                          new Vector2(.01f * z_viewportRec.Width, .01f * z_viewportRec.Height),
                                          Color.Yellow);
            this.z_spriteBatch.DrawString(this.z_livesFont, "Lives: " + this.z_playerShip.Lives,
                                          new Vector2(.01f * z_viewportRec.Width, .05f * z_viewportRec.Height),
                                          Color.White);
            this.z_spriteBatch.DrawString(this.z_livesFont, "Score: " + this.z_playerShip.score,
                                          new Vector2(.01f * z_viewportRec.Width, .1f * z_viewportRec.Height),
                                          Color.White);
            this.z_spriteBatch.DrawString(this.z_livesFont, "Enemies: " + EnemyManager.totalEnemyCount(),
                                          new Vector2(.01f * z_viewportRec.Width, .15f * z_viewportRec.Height),
                                          Color.Red);
            this.z_spriteBatch.DrawString(this.z_livesFont, "Floaters: " + (z_objects.Count-1).ToString(),
                                          new Vector2(.01f * z_viewportRec.Width, .2f * z_viewportRec.Height),
                                          Color.Aquamarine);

            //Draw any achivements
            if (this.z_achivementFail.IsAlive)
                this.z_spriteBatch.Draw(this.z_achivementFail.Sprite, this.z_achivementFail.Position, new Color(1,1,1,.8f));



            // draw scores, achievements, etc...
            foreach (GameObject obj in z_objects)
            {
                if (obj.DrawOnTop)
                    obj.Draw(this.z_spriteBatch, gameTime);
            }
        }


        public void mainReset()
        {
            this.z_gameTimer = 0;
            this.z_isGameOver = false;

            this.z_missionManager.reset();
            this.z_enemyManager.resetAllEnemies();
            z_achivementFailUnlocked = false;
            this.z_achivementFail.IsAlive = false;
            this.z_missleManager.reset();
            this.z_playerShip.reset();
            for (int i = 0; i < z_objects.Count; i++)
            {
                if ((z_objects[i] is FloatingText) || (z_objects[i] is Explosion))
                {
                    z_objects[i].returnToPool();
                    z_objects.RemoveAt(i--);
                }
            }
            Asteroid.Density = Asteroid.AsteroidDensity.Heavy;

        }
    }
}
