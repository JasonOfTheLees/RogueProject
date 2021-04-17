using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGenerationDemo
{
    struct Point
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }


        /// <summary>
        /// Sees if the provided object is the same object as this
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Point p = (Point)obj;
                return (X == p.X) && (Y == p.Y);
            }
        }
        public override int GetHashCode()
        {
            return (X << 2) ^ Y;
        }

        public static bool operator ==(Point x, Point y)
        {
            return x.X == y.X && x.Y == y.Y;
        }

        public static bool operator !=(Point x, Point y)
        {
            return !(x == y);
        }

        public static Point operator +(Point x, Point y)
        {
            return new Point(x.X + y.X, x.Y + y.Y);
        }

        public static Point operator -(Point x, Point y)
        {
            return new Point(x.X - y.X, x.Y - y.Y);
        }

        public override string ToString()
        {
            return String.Format("Point({0}, {1})", X, Y);
        }
    }
}
