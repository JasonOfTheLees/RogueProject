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
            Col = x;
            Row = y;
        }

        public int Col { get; }
        public int Row { get; }


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
                return (Col == p.Col) && (Row == p.Row);
            }
        }
        public override int GetHashCode()
        {
            return (Col << 2) ^ Row;
        }

        public static bool operator ==(Point x, Point y)
        {
            return x.Col == y.Col && x.Row == y.Row;
        }

        public static bool operator !=(Point x, Point y)
        {
            return !(x == y);
        }

        public static Point operator +(Point x, Point y)
        {
            return new Point(x.Col + y.Col, x.Row + y.Row);
        }

        public static Point operator -(Point x, Point y)
        {
            return new Point(x.Col - y.Col, x.Row - y.Row);
        }

        public override string ToString()
        {
            return String.Format("Point({0}, {1})", Col, Row);
        }
    }
}
