using System;

namespace DungeonGenerationDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Generator gen = new Generator(100, 30);
            gen.Generate(6);
            Dungeon dungeon = gen.GetDungeon();
            dungeon.PaintAll();

            Console.SetCursorPosition(0, 35); 
        }
    }
}
