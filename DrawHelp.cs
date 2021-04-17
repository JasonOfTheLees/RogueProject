using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGenerationDemo
{
    class DrawHelp
    {
        public static void DrawLine(int x1, int y1, int x2, int y2)
        {
            int dist = (int)Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
            int xDif = x2 - x1;
            int yDif = y2 - y1;

            for(int i = 0; i < dist; i++)
            {
                float fraction = (float) i / (float) dist;
                Console.SetCursorPosition(x1 + (int) (xDif * fraction), y1 + (int) (yDif * fraction));
                Console.WriteLine("*");
            }
        }

        public static void DrawRect(int x1, int y1, int x2, int y2)
        {
            int dist = (int)Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
            int xDif = x2 - x1;
            int yDif = y2 - y1;

            for (int i = 0; i < dist; i++)
            {
                float fraction = (float)i / (float)dist;
                Console.SetCursorPosition(x1 + (int)(xDif * fraction), y1 + (int)(yDif * fraction));
                Console.WriteLine("*");
            }
        }
    }
}
