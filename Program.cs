using System;
using static DungeonGenerationDemo.Constants;

namespace DungeonGenerationDemo
{

    class Program
    {

        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            Generator gen = new Generator(100, 30);
            gen.Generate(6);
            Dungeon dungeon = gen.GetDungeon();

            dungeon.PlacePlayer(); // TODO: test code

            dungeon.PaintAll();

            Cardinal newDirection;
            ConsoleKey key;

            do
            {
                
                key = Console.ReadKey(true).Key;
                

                switch (key)
                {
                    case ConsoleKey.A:
                        newDirection = Cardinal.Left;
                        break;
                    case ConsoleKey.D:
                        newDirection = Cardinal.Right;
                        break;
                    case ConsoleKey.W:
                        newDirection = Cardinal.Up;
                        break;
                    case ConsoleKey.S:
                        newDirection = Cardinal.Down;
                        break;
                    case ConsoleKey.X:
                        newDirection = Cardinal.None;
                        break;
                    default:
                        newDirection = Cardinal.None;
                        break;
                }

                if (key == ConsoleKey.Escape) { break; }

                dungeon.MovePlayer(newDirection);

            } while (key != ConsoleKey.Escape);

            key = Console.ReadKey(true).Key;

            Console.SetCursorPosition(0, 35); 
        }
    }
}
