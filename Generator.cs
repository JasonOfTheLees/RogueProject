using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DungeonGenerationDemo
{
    enum Tile
    {
        EMPTY,
        FLOOR,
        PATH,
        DOOR
    }

    class Generator
    {
        private Dungeon dungeon;
        private Tile[,] grid;
        private Random rand;
        private int width;
        private int height;
        private List<Room> rooms;
        private int sets;

        public Generator(int width, int height)
        {
            dungeon = new Dungeon(width, height);
            grid = new Tile[width, height];
            this.width = width;
            this.height = height;
            rand = new Random();
            rooms = new List<Room>();
        }

        public Dungeon GetDungeon()
        {
            return dungeon;
        }

        public void Generate(int count)
        {
            for (int i = 0; i < count; i++)
                new Room(this, rand.Next(8, 20), rand.Next(4, 10));

            foreach(Room el in rooms)
            {
                Room near = nearest(el);
                if(near != null)
                    el.Connect(near);
            }
        }

        private void place(Tile tile, int x, int y)
        {
            grid[x, y] = tile;
            StaticTile obj;
            switch(tile)
            {
                case Tile.FLOOR:
                    obj = StaticTile.Floor(x, y);
                    break;
                case Tile.PATH:
                    obj = StaticTile.Path(x, y);
                    break;
                case Tile.DOOR:
                    obj = StaticTile.Door(x, y);
                    break;
                default:
                    obj = new StaticTile(x, y, false);
                    break;
            }

            dungeon.PlaceObject(obj, x, y);
        }

        private bool pointValid(int x, int y)
        {
            return (0 < x && x < width) && (0 < y && y < height);
        }

        private Room nearest(Room room)
        {
            int nearestDist = int.MaxValue;
            Room nearest = null;

            foreach(Room el in rooms)
            {
                int elDist = (int) (Math.Pow(el.centerX - room.centerX, 2) + Math.Pow(el.centerY - room.centerY, 2));
                if (el != room && !el.connected.Contains(room) && elDist < nearestDist)
                {
                    nearestDist = elDist;
                    nearest = el;
                }
            }

            return nearest;
        }

        private bool anyCollide(Room room)
        {
            foreach(Room el in rooms)
            {
                if (el != room && room.Collide(el))
                    return true;
            }

            return false;
        }

        private class Path
        {
            private Generator gen;
            private int targetX;
            private int targetY;
            private int currentX;
            private int currentY;
            private int travelX;
            private int travelY;
            private bool active;

            public Path(Generator gen, Room r1, Room r2)
            {
                this.gen = gen;

                targetX = r1.centerX;
                targetY = r1.centerY;
                currentX = r2.centerX;
                currentY = r2.centerY;

                travelX = targetX - currentX;
                travelY = targetY - currentY;

                pathStep();
            }

            private void pathStep()
            {
                List<int> moves = new List<int>();

                if (travelX > 0)
                    moves.Add(0);
                if (travelX < 0)
                    moves.Add(1);
                if (travelY > 0)
                    moves.Add(2);
                if (travelY < 0)
                    moves.Add(3);

                if (moves.Count == 0)
                    return;

                int lastX = currentX;
                int lastY = currentY;

                int move = moves[gen.rand.Next(moves.Count)];
                switch (move)
                {
                    case 0:
                        currentX++;
                        travelX--;
                        break;
                    case 1:
                        currentX--;
                        travelX++;
                        break;
                    case 2:
                        currentY++;
                        travelY--;
                        break;
                    case 3:
                        currentY--;
                        travelY++;
                        break;
                }

                if (!active)
                {
                    if (gen.grid[currentX, currentY] == Tile.EMPTY)
                    {
                        active = true;
                        gen.place(Tile.DOOR, lastX, lastY);
                    }
                }

                if (active)
                {
                    switch (gen.grid[currentX, currentY])
                    {
                        case Tile.EMPTY:
                            gen.place(Tile.PATH, currentX, currentY);
                            break;
                        case Tile.FLOOR:
                            gen.place(Tile.DOOR, currentX, currentY);
                            return;
                        case Tile.PATH:
                            return;
                    }
                }

                pathStep();
            }
        }

        private class Room
        {
            public int minX;
            public int minY;
            public int maxX;
            public int maxY;
            public int centerX;
            public int centerY;
            public int Set;
            public List<Room> connected;
            private Generator gen;

            public Room(Generator gen, int width, int height)
            {
                this.gen = gen;
                Set = gen.rooms.Count;
                gen.sets++;
                gen.rooms.Add(this);
                connected = new List<Room>();

                do
                {
                    minX = gen.rand.Next(gen.width - width - 1);
                    minY = gen.rand.Next(gen.height - height - 1);
                    maxX = minX + width;
                    maxY = minY + height;
                } while (gen.anyCollide(this));

                this.centerX = (int)((minX + maxX) * 0.5);
                this.centerY = (int)((minY + maxY) * 0.5);

                place();
            }

            public bool Collide(Room other)
            {
                if (this.minX > other.maxX ||
                    this.maxX < other.minX ||
                    this.minY > other.maxY ||
                    this.maxY < other.minY)
                    return false;
                else
                    return true;
            }
            
            public int Dist(int x, int y)
            {
                int closeX;
                int closeY;

                if (x > maxX)
                    closeX = maxX;
                else if (x < minX)
                    closeX = minX;
                else
                    closeX = x;

                if (y > maxY)
                    closeY = maxY;
                else if (y < minY)
                    closeY = minY;
                else
                    closeY = y;

                return (int) Math.Sqrt(Math.Pow(x - closeX, 2) + Math.Pow(y - closeY, 2));
            }

            public void Connect(Room other)
            {
                if (this.connected.Contains(other))
                    return;

                if (this.Set != other.Set)
                {
                    other.Set = this.Set;
                    gen.sets--;
                }

                this.connected.Add(other);
                other.connected.Add(this);
                new Path(gen, this, other);
            }

            private void place()
            {
                int width = maxX - minX;
                int height = maxY - minY;
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        gen.place(Tile.FLOOR, minX + i, minY + j);
                    }
                }
            }
        }
    }
}
