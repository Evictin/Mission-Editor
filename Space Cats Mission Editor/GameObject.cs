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
    class GameObject
    {
        //Declare Instance Variables ----------------------------------------------------------------------------
        private int z_ID;
        private Texture2D z_sprite;
        private Vector2 z_position;
        private Vector2 z_velocity;
        private float z_speed;
        private bool z_isAlive;
        //For Hit Detection Purposes
        private bool z_isKillerObject;
        private bool z_isPickUp;
        private Rectangle z_hitRec;
        private static int z_nextID=0;


        //Constructor -------------------------------------------------------------------------------------------
        public GameObject(Texture2D loadedTexture)
        {
            //Initialize all instance variables
            this.z_sprite = loadedTexture;
            this.z_position = Vector2.Zero;
            this.z_velocity = Vector2.Zero;
            this.z_speed = 1.0f;
            this.z_isAlive = false;
            this.z_hitRec = new Rectangle(0, 0, 0, 0);
            this.z_isKillerObject = false;
            this.z_isPickUp = false;
        }


        //Access Methods ----------------------------------------------------------------------------------------
        public Texture2D getSprite()
        {
            return this.z_sprite;
        }
        public Vector2 getPosition()
        {
            return this.z_position;
        }
        public Vector2 getVelocity()
        {
            return this.z_velocity;
        }
        public Vector2 getVelocityWithSpeed()
        {
            return new Vector2(this.z_velocity.X * this.z_speed,
                                           this.z_velocity.Y * this.z_speed);
        }
        public bool isAlive()
        {
            return this.z_isAlive;
        }
        public float getSpeed()
        {
            return this.z_speed;
        }
        public bool isKillerObject()
        {
            return this.z_isKillerObject;
        }
        public bool getIsPickUp()
        {
            return this.z_isPickUp;
        }
        public virtual Rectangle getHitRec()
        {
            return new Rectangle((int)this.z_position.X, (int)this.z_position.Y, this.z_sprite.Width, this.z_sprite.Height);
        }
        public int getID()
        {
            return z_ID;
        }
        public static int getNextID()
        {
            return ++z_nextID;
        }

        virtual public List<string> getPropertyList()
        {
            List<string> list = new List<string>();
            list.Add("ID");
            list.Add("Location");
            list.Add("Speed");
            list.Add("isKillerObject");
            list.Add("isPickUp");
            return list;
        }

        virtual public string getProperty(string propertyName)
        {
            string prop = propertyName.ToUpper();
            if (prop.CompareTo("ID") == 0)
                return z_ID.ToString();
            else if (prop.CompareTo("LOCATION") == 0)
                return String.Format("{0}, {1}", z_position.X, z_position.Y);
            else if (prop.CompareTo("SPEED") == 0)
                return z_speed.ToString();
            else if (prop.CompareTo("ISKILLEROBJECT") == 0)
                return z_isKillerObject.ToString();
            else if (prop.CompareTo("ISPICKUP") == 0)
                return z_isPickUp.ToString();
            return null;
        }

        virtual public bool setProperty(string propertyName, string value)
        {
            string prop = propertyName.ToUpper();
            string[] words;
            if (prop.CompareTo("ID") == 0)
                return int.TryParse(value, out z_ID);
            else if (prop.CompareTo("LOCATION") == 0)
            {
                words = prop.Split(',');
                return (float.TryParse(words[0], out z_position.X) && float.TryParse(words[1], out z_position.Y));
            }
            else if (prop.CompareTo("SPEED") == 0)
                return float.TryParse(value, out z_speed);
            else if (prop.CompareTo("ISKILLEROBJECT") == 0)
                return bool.TryParse(value, out z_isKillerObject);
            else if (prop.CompareTo("ISPICKUP") == 0)
                return bool.TryParse(value, out z_isPickUp);
            return false;
        }


        //Mutator Methods ---------------------------------------------------------------------------------------
        public void setSprite(Texture2D newSprite)
        {
            this.z_sprite = newSprite;
        }
        public void setPosition(Vector2 newPosition)
        {
            this.z_position = newPosition;
        }
        public void setVelocity(Vector2 newVelocity)
        {
            this.z_velocity = newVelocity;
        }
        public void setIsAlive(bool isAlive)
        {
            this.z_isAlive = isAlive;
        }
        public void setSpeed(float newSpeed)
        {
            this.z_speed = newSpeed;
        }
        public void setIsKillerObject(bool isKiller)
        {
            if (isKiller == true && this.z_isPickUp)
                this.z_isPickUp = false;
            this.z_isKillerObject = isKiller;
        }
        public void setIsPickUp(bool isPickup)
        {
            if (this.z_isKillerObject && isPickup == true)
                this.z_isKillerObject = false;
            this.z_isPickUp = isPickup;
        }
        public void setHitRec(Rectangle newHitRec)
        {
            this.z_hitRec = newHitRec;
        }
        public void setID(int id)
        {
            z_ID = id;
        }


        //Other Methods -----------------------------------------------------------------------------------------
        public void upDatePosition()
        {
            this.z_position += this.z_velocity;
        }

        //Use this method for updating position if a speed is set
        public void upDatePositionWithSpeed()
        {
            this.z_position += new Vector2(this.z_velocity.X * this.z_speed,
                                           this.z_velocity.Y * this.z_speed);
        }




    }
}
