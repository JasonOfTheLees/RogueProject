using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGenerationDemo
{
    interface ITempTile
    {

        Point Coordinates { get; protected set; }

        char Display { get; } // the symbol that will be printed on the board to represent the object

        ConsoleColor BackgroundColor { get; set; }
        ConsoleColor ForegroundColor { get; set; }

        /// <summary>
        /// Draws the object on the board at its coordinates
        /// </summary>
        void Paint();

        /// <summary>
        /// Determines whether the provided point intersects with this object
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        bool Collision(Point obstacle);


    }

    /// <summary>
    /// Objects that can move and die
    /// </summary>
    interface ICreature : IGameObject
    {
        int Health { get; set; }
        int Attack { get; set; }
        List<ITempTile> Loot { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        bool Move(Point destination);
    }

    /// <summary>
    /// Represents the hero of our story and all that they contain
    /// </summary>
    class Player : ICreature
    {

        public int Row { get; }
        public int Col { get; }
        public bool Solid { get; }

        public Point Coordinates { get; set; }
        public char Display { get; } = '☺';
        public int Health { get; set; }
        public int Attack { get; set; }
        public List<ITempTile> Loot { get; set; }
        public ConsoleColor BackgroundColor { get; set; }
        public ConsoleColor ForegroundColor { get; set; }

        public Player(Point origin)
        {
            Coordinates = origin;

            BackgroundColor = ConsoleColor.Black;
            ForegroundColor = ConsoleColor.White;
        }

        public void Paint() 
        {
            Console.BackgroundColor = BackgroundColor;
            Console.ForegroundColor = ForegroundColor;

            Console.SetCursorPosition(Coordinates.X, Coordinates.Y);
            Console.Write(Display);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.DarkGray;
        }

        public bool Move(Point destination)
        {
            Coordinates = destination;
            return true;
        }

        public bool Collision(int x, int y) 
        {
            return this.Collision(new Point(x, y));
        }

        public bool Collision(Point obstacle)
        {
            return obstacle == Coordinates;
        }

        public bool OnCollision()
        {
            return false;

        }
    }
}
