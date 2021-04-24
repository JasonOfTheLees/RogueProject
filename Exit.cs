using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGenerationDemo
{
    class Exit : IGameObject
    {
        public Point Point { get; }
        public bool Solid { get; } = false;
        private char character = 'X';
        private ConsoleColor foreground = ConsoleColor.Yellow;
        private ConsoleColor background = ConsoleColor.Black;
        private Dungeon dungeon;

        public Exit(Point p, Dungeon d)
        {
            Point = p;
            dungeon = d;
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
            dungeon.atExit = true;
            Debug.Print("At Exit");
            return true;
        }
    }
}
