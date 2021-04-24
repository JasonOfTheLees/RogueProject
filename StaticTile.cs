using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGenerationDemo
{
    struct StaticTile : IGameObject
    {
        public Point Point { get; }
        public bool Solid { get; }
        private char character;
        private ConsoleColor foreground;
        private ConsoleColor background;

        public static StaticTile Floor(Point p)
        {
            return new StaticTile(p, false, '.');
        }

        public static StaticTile Path(Point p)
        {
            return new StaticTile(p, false, '#');
        }

        public static StaticTile Door(Point p)
        {
            return new StaticTile(p, false, '+');
        }

        public static StaticTile VerWall(Point p)
        {
            return new StaticTile(p, true, '|');
        }

        public static StaticTile HorWall(Point p)
        {
            return new StaticTile(p, true, '-');
        }

        public static StaticTile PathWall(Point p)
        {
            return new StaticTile(p, true, ' ');
        }

        public static StaticTile Exit(Point p)
        {
            return new StaticTile(p, false, 'X');
        }

        public StaticTile(Point p, bool solid, char character = '?', 
            ConsoleColor foreground = ConsoleColor.White, 
            ConsoleColor background = ConsoleColor.Black)
        {
            Point = p;
            this.Solid = solid;
            this.character = character;
            this.foreground = foreground;
            this.background = background;
        }

        public void Paint()
        {
            Console.SetCursorPosition(Point.Col, Point.Row);
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
            Console.Write(character);
        }

        public bool OnCollision()
        {
            return true;
        }
    }
}
