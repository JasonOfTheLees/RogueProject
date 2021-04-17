using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGenerationDemo
{
    class Monster : ICreature
    {

        public int Row { get; }
        public int Col { get; }
        public bool Solid { get; }

        public Point Coordinates { get; }
        public char Display { get; } = 'Ϫ';
        public int Health { get; set; }
        public int Attack { get; set; }
        public List<ITempTile> Loot { get; set; }
        public ConsoleColor BackgroundColor { get; set; }
        public ConsoleColor ForegroundColor { get; set; }

        public Monster(Point origin)
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
            return false;
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
