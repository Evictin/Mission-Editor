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
    class Circle
    {
        //Variables
        private Vector2 z_center;
        private float z_radiusField;
        private float z_radiusSquared;
        private float z_radius
        {
            get { return z_radiusField; }
            set
            {
                z_radiusField = value;
                z_radiusSquared = value * value;
            }
        }
        private Point z_left 
        { get { return new Point((int)(z_center.X - z_radiusField), (int)z_center.Y); } }
        private Point z_right 
        { get { return new Point((int)(z_center.X + z_radiusField), (int)z_center.Y); } }
        private Point z_top 
        { get { return new Point((int)z_center.X, (int)(z_center.Y - z_radiusField)); } }
        private Point z_bottom 
        { get { return new Point((int)z_center.X, (int)(z_center.Y + z_radiusField)); } }

        //Constructor
        public Circle(Vector2 position, float size)
        {
            this.z_center = position;
            this.z_radius = size;
        }

        //Intersection between two circles
        public bool Intersects(Circle circleOther)
        {
            //Calculate the distance between the two circles.
            //DistanceSquared = (X1 - X2)^2 + (Y1 - Y2)^2
            //Check that the distance is less than than the sum of both radii
            //If it is, then there is a collision
            return (Vector2.DistanceSquared(this.z_center, circleOther.z_center) 
                < (this.z_radiusSquared + circleOther.z_radiusSquared));
        }

        private float distanceSquared(float x, float y)
        {
            return (this.z_center.X - x) * (this.z_center.X - x) + (this.z_center.Y - y) * (this.z_center.Y - y);
        }

        //Intersection between this circle and a Rectangle
        public bool Intersects(Rectangle rectangleOther)
        {
            
            //Check the distance between the center of the circle against each corner of the rectangle
            // Note, I rewrote this using the distance squared and the radius squared as it is MUCH
            // more efficient than using regular distance (because you don't have to take square roots :) )
            if (distanceSquared(rectangleOther.Left, rectangleOther.Top) < this.z_radiusSquared ||
                    distanceSquared(rectangleOther.Right, rectangleOther.Top) < this.z_radiusSquared ||
                    distanceSquared(rectangleOther.Left, rectangleOther.Bottom) < this.z_radiusSquared ||
                    distanceSquared(rectangleOther.Right, rectangleOther.Bottom) < this.z_radiusSquared)
                return true;
            // and I added the next part in case NONE of the corners of the rect are inside the circle, 
            // then you need to check if any of the circle's "corners" are inside the rect
            return (rectangleOther.Contains(z_left) || rectangleOther.Contains(z_right) ||
                rectangleOther.Contains(z_top) || rectangleOther.Contains(z_bottom));

        }

        //Accessors
        public Vector2 getCenter()
        {
            return this.z_center;
        }
        public float getRadius()
        {
            return this.z_radius;
        }

        //Mutators
        public void setCenter(Vector2 newCenter)
        {
            this.z_center = newCenter;
        }

        public void setRadius(float newRadius)
        {
            this.z_radius = newRadius;
        }

    }
}
