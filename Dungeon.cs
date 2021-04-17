﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DungeonGenerationDemo.Constants;

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

        public Player Player { get; set; }

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
        public bool IsEmpty(Point point)
        {
            return IsEmpty(point.X, point.Y);
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

        /// <summary>
        /// Move whatever is on the top of the position onto a new position
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="Destination"></param>
        /// <returns></returns>
        public bool MoveObject(Point origin, Point Destination)
        {

            // If there's time for the chaos of local methods in local delegates
            //public Action<Point, Point> MoveDelegate() { return MoveObject; }
            //Action<Point> draw = ConsoleDrawing.Triangle;

            //bool inside(string stuff) { return false; }

            map[Destination.X, Destination.Y].Push(map[Player.Coordinates.X, Player.Coordinates.Y].Pop());

            PaintAt(Player.Coordinates.X, Player.Coordinates.Y);

            Player.Coordinates = Destination;

            PaintAt(Destination.X, Destination.Y);

            return false;
        }

        /// <summary>
        /// Have the player attempt to move, and either interact with what's in the new direction
        /// or move in the new direction
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public bool MovePlayer(Cardinal direction)
        {
            Point destination = Player.Coordinates + DirectionVectors[(int)direction];
            if (!IsEmpty(destination) &&
                !map[destination.X, destination.Y].Peek().Solid &&
                map[destination.X, destination.Y].Peek().OnCollision()) // if the object gets destroyed/picked up it returns true
            {
                MoveObject(Player.Coordinates, destination);
            }
            return false;
        }

        public void PlacePlayer()
        {

            // TODO: adding a player to test movement
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (!IsEmpty(i, j)
                        //&& map[i,j].Peek().Solid
                        )
                    {
                        Player = new Player(new Point(i, j));
                        map[i, j].Push(Player);
                        i = Width; j = Height;
                    }
                }
            }

        }
    }
}
