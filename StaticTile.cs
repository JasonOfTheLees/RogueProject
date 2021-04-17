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
        public int Row { get; }
        public int Col { get; }
        public bool Solid { get; }
        private char character;
        private ConsoleColor foreground;
        private ConsoleColor background;

        public static StaticTile Floor(int row, int col)
        {
            return new StaticTile(row, col, false, '.');
        }

        public static StaticTile Path(int row, int col)
        {
            return new StaticTile(row, col, false, '#');
        }

        public static StaticTile Door(int row, int col)
        {
            return new StaticTile(row, col, false, 'X');
        }

        public StaticTile(int row, int col, bool solid, char character = '?', 
            ConsoleColor foreground = ConsoleColor.White, 
            ConsoleColor background = ConsoleColor.Black)
        {
            Row = row;
            Col = col;
            this.Solid = solid;
            this.character = character;
            this.foreground = foreground;
            this.background = background;
        }

        public void Paint()
        {
            Console.SetCursorPosition(Row, Col);
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
            Console.Write(character);
        }
    }
}
