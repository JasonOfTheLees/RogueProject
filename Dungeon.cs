using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGenerationDemo
{
    /// <summary>
    /// Holds all of the game object and allows access and changing.
    /// </summary>
    class Dungeon
    {
        private Stack<IGameObject>[,] map { get; }
        public int Width { get; }
        public int Height { get; }

        /// <summary>
        /// Constructs a dungeon with a 2D array of stacks of IGameObjects. Should never really be 
        /// called outside of generator.
        /// </summary>
        /// <param name="map"></param>
        public Dungeon(int width, int height)
        {
            Width = width;
            Height = height;
            map = new Stack<IGameObject>[width, height];
        }

        /// <summary>
        /// Paints everything in the dungeon. Should not be used in the final game, as rooms will 
        /// be drawn one by one.
        /// </summary>
        public void PaintAll()
        {
            for(int i = 0; i < map.GetLength(0); i++)
            {
                for(int j = 0; j < map.GetLength(1); j++)
                {
                    PaintAt(i, j);
                }
            }
        }

        /// <summary>
        /// Paints a single tile in the dungeon. 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void PaintAt(int row, int col)
        {
            Stack<IGameObject> current = map[row, col];

            if (!IsEmpty(row, col))
                current.Peek().Paint();
        }

        /// <summary>
        /// Returns the game object at the given row and column, or null if that tile is null
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public IGameObject GetObject(int row, int col)
        {
            Stack<IGameObject> current = map[row, col];
            if (current == null)
                return null;
            if (current.Count == 0)
                return null;
            return current.Peek();
        }

        /// <summary>
        /// Returns true is the tile at the given row and column is empty
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public bool IsEmpty(int row, int col)
        {
            Stack<IGameObject> current = map[row, col];
            if (current == null)
                return true;
            if (current.Count == 0)
                return true;

            return false;
        }

        /// <summary>
        /// Places a game object inside of the dungeon on the top of whatever tile is chosen.
        /// Handles stack creation.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void PlaceObject(IGameObject gameObject, int row, int col)
        {
            Stack<IGameObject> current = map[row, col];
            if (current == null)
            {
                map[row, col] = new Stack<IGameObject>();
                current = map[row, col];
            }

            current.Push(gameObject);
        }

        /// <summary>
        /// Returns the internal map. Should be used sparingly, use GetObject if possible. 
        /// </summary>
        /// <returns></returns>
        public Stack<IGameObject>[,] GetInternalMap()
        {
            return map;
        }
    }
}
