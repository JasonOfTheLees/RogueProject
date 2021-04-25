using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DungeonGenerationDemo
{
    /// <summary>
    /// Used interally by the generator. Should not be used elsewhere.
    /// </summary>
    enum Tile
    {
        EMPTY,
        FLOOR,
        PATH,
        VER_WALL,
        HOR_WALL,
        WALL,
        PATH_WALL,
        DOOR
    }

    /// <summary>
    /// Generates a dungeon by placing random rooms and connecting them together with procedural 
    /// paths.
    /// </summary>
    class Generator
    {
        private const int MIN_ROOM_WIDTH = 8;
        private const int MAX_ROOM_WIDTH = 20;
        private const int MIN_ROOM_HEIGHT = 4;
        private const int MAX_ROOM_HEIGHT = 10;
        //Gives the odds the path will change direction. Lower number makes paths curvier
        private const int PATH_DIR_CHANGE = 5;

        private Dungeon dungeon;
        private Tile[,] grid;
        private IGenRoom[,] gridOrigins;
        private Random rand;
        private int width;
        private int height;
        private List<Room> rooms;
        private int sets;
        private List<Monster> monsters;

        public Generator(int width, int height)
        {
            this.width = width;
            this.height = height;
            rand = new Random();
        }

        private void initialize()
        {
            dungeon = new Dungeon(width, height);
            gridOrigins = new IGenRoom[width, height];
            grid = new Tile[width, height];
            rooms = new List<Room>();
            monsters = new List<Monster>();
            dungeon.monsters = monsters;
            dungeon.rand = rand;
        }

        /// <summary>
        /// Finds a valid point somewhere in a room. Only works after Generate has been called.
        /// </summary>
        /// <returns>Valid point in dungeon</returns>
        public Point getValidPoint()
        {
            Room room = rooms[rand.Next(rooms.Count)];

            return new Point(rand.Next(room.MinC.Col + 1, room.MaxC.Col - 1), rand.Next(room.MinC.Row + 1, room.MaxC.Row - 1));
        }

        /// <summary>
        /// Returns the dungeon generated by the generator. 
        /// </summary>
        /// <returns></returns>
        public Dungeon GetDungeon()
        {
            return dungeon;
        }

        /// <summary>
        /// Acts as the entrance point for the rest of the generator. Actually produces a dungeon 
        /// with the given number of rooms, in the space given in the constructor. 
        /// </summary>
        /// <param name="count">Number of rooms to be generated</param>
        public void Generate(int count)
        {
            initialize();

            for (int i = 0; i < count; i++)
            {
                Room room = new Room(this, rand.Next(MIN_ROOM_WIDTH, MAX_ROOM_WIDTH), rand.Next(MIN_ROOM_HEIGHT, MAX_ROOM_HEIGHT));
                Point spawn = room.Random();
                Monster newMonster = new Monster(spawn, rand);
                dungeon.PlaceObject(newMonster, spawn);
                monsters.Add(newMonster);
            }

            while (sets > 1)
            {
                Room current = rooms[rand.Next(rooms.Count)];
                Room near = nearest(current);
                if (near != null)
                    new Path(this, current, near);
            }

            Point point = getValidPoint();
            dungeon.PlaceObject(new Exit(point, dungeon), point);


            Room playerRoom = rooms[rand.Next(rooms.Count)];
            point = playerRoom.Center;
            Player player = new Player(point, dungeon.rand);
            dungeon.Player = player;
            dungeon.PlaceObject(player, point);
            dungeon.SetVisible(playerRoom.Visible, true);
        }

        private void place(Tile tile, Point p)
        {
            place(tile, p, null);
        }

        /// <summary>
        /// Places the given tile in the dungeon, at the same time as adding the tile to the
        /// internal array used for quick access. 
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="p"></param>
        private void place(Tile tile, Point p, IGenRoom origin)
        {
            grid[p.Col, p.Row] = tile;
            gridOrigins[p.Col, p.Row] = origin;

            StaticTile obj;
            switch (tile)
            {
                case Tile.FLOOR:
                    obj = StaticTile.Floor(p);
                    break;
                case Tile.PATH:
                    obj = StaticTile.Path(p);
                    break;
                case Tile.DOOR:
                    obj = StaticTile.Door(p);
                    break;
                case Tile.VER_WALL:
                    obj = StaticTile.VerWall(p);
                    grid[p.Col, p.Row] = Tile.WALL;
                    break;
                case Tile.HOR_WALL:
                    obj = StaticTile.HorWall(p);
                    grid[p.Col, p.Row] = Tile.WALL;
                    break;
                case Tile.PATH_WALL:
                    obj = StaticTile.PathWall(p);
                    grid[p.Col, p.Row] = Tile.EMPTY;
                    break;
                default:
                    obj = new StaticTile(p, false);
                    break;
            }

            dungeon.PlaceObject(obj, p);
        }

        private void placeDoor(Point p, Room origin, List<Point> visible)
        {
            grid[p.Col, p.Row] = Tile.DOOR;
            gridOrigins[p.Col, p.Row] = origin;
            dungeon.PlaceObject(new VisibilityDoor(p, dungeon, visible), p);
        }

        private bool pointValid(int x, int y)
        {
            return (0 < x && x < width) && (0 < y && y < height);
        }

        private Room nearest(Room room)
        {
            int nearestDist = int.MaxValue;
            Room nearest = null;

            foreach (Room el in rooms)
            {
                int elDist = (int)(Math.Pow(el.Center.Col - room.Center.Col, 2) + Math.Pow(el.Center.Row - room.Center.Row, 2));
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
            foreach (Room el in rooms)
            {
                if (el != room && room.Collide(el))
                    return true;
            }

            return false;
        }

        private class Path : IGenRoom
        {
            private Generator gen;
            private Room originRoom;
            private Room targetRoom;
            private Point target;
            private Point current;
            private int travelCol;
            private int travelRow;
            private bool active;
            private int prev;
            public List<Point> Visible { get; }
            public Room Origin { get; }
            private List<Point> firstDoorVisible;

            public Path(Generator gen, Room r1, Room r2)
            {
                this.originRoom = r2;
                this.targetRoom = r1;
                this.gen = gen;
                Visible = new List<Point>();
                firstDoorVisible = new List<Point>();
                Origin = originRoom;

                target = r1.Random();
                current = r2.Random();

                travelCol = target.Col - current.Col;
                travelRow = target.Row - current.Row;

                pathStep();
            }

            void placeWall(Point p)
            {
                if (gen.grid[p.Col, p.Row] == Tile.EMPTY)
                    gen.place(Tile.PATH_WALL, p, this);
            }

            private void pathStep()
            {
                List<int> moves = new List<int>();

                if (travelCol > 0)
                    moves.Add(0);
                if (travelCol < 0)
                    moves.Add(1);
                if (travelRow > 0)
                    moves.Add(2);
                if (travelRow < 0)
                    moves.Add(3);

                if (moves.Count == 0)
                    return;

                int move = moves[gen.rand.Next(moves.Count)];
                foreach (int el in moves)
                {
                    if (el == prev)
                    {
                        if (gen.rand.Next(PATH_DIR_CHANGE) != 0)
                            move = prev;
                    }
                }
                prev = move;
                Point last = current;
                switch (move)
                {
                    case 0:
                        current = new Point(current.Col + 1, current.Row);
                        travelCol--;
                        break;
                    case 1:
                        current = new Point(current.Col - 1, current.Row);
                        travelCol++;
                        break;
                    case 2:
                        current = new Point(current.Col, current.Row + 1);
                        travelRow--;
                        break;
                    case 3:
                        current = new Point(current.Col, current.Row - 1);
                        travelRow++;
                        break;
                }

                if (!active)
                {
                    switch (gen.grid[current.Col, current.Row])
                    {
                        case Tile.EMPTY:
                            active = true;
                            Visible.Add(last);
                            firstDoorVisible.AddRange(originRoom.Visible);
                            gen.placeDoor(last, originRoom, firstDoorVisible);
                            break;
                        case Tile.PATH:
                            originRoom.Connect(gen.gridOrigins[current.Col, current.Row].Origin);
                            return;
                        case Tile.DOOR:
                            originRoom.Connect(gen.gridOrigins[current.Col, current.Row].Origin);
                            return;
                    }
                }

                if (active)
                {
                    switch (gen.grid[current.Col, current.Row])
                    {
                        case Tile.EMPTY:
                            gen.place(Tile.PATH, current, this);
                            Visible.Add(current);
                            break;
                        case Tile.WALL:
                            IGenRoom foundWall = gen.gridOrigins[current.Col, current.Row];
                            originRoom.Connect(foundWall.Origin);

                            Visible.Add(current);
                            firstDoorVisible.AddRange(Visible);

                            List<Point> doorVisible = new List<Point>(Visible);
                            doorVisible.AddRange(foundWall.Visible);
                            gen.placeDoor(current, foundWall.Origin, doorVisible);

                            firstDoorVisible.AddRange(Visible);
                            return;
                        case Tile.PATH:
                            IGenRoom foundPath = gen.gridOrigins[current.Col, current.Row];
                            originRoom.Connect(foundPath.Origin);

                            firstDoorVisible.AddRange(Visible);
                            firstDoorVisible.AddRange(foundPath.Visible);
                            foundPath.Visible.AddRange(Visible);
                            return;
                    }
                }

                placeWall(new Point(current.Col + 1, current.Row));
                placeWall(new Point(current.Col - 1, current.Row));
                placeWall(new Point(current.Col, current.Row + 1));
                placeWall(new Point(current.Col, current.Row - 1));

                pathStep();
            }
        }

        private class Room : IGenRoom
        {
            public Point MinC;
            public Point MaxC;
            public Point Center;
            public int Set;
            public List<Room> connected;
            public List<Point> Visible { get; }
            public Room Origin { get; }
            private Generator gen;

            public Room(Generator gen, int width, int height)
            {
                Debug.Print("Got here");
                this.gen = gen;
                Set = gen.rooms.Count;
                gen.sets++;
                Debug.Print($"Set: {Set} created. Total: {gen.sets}");
                gen.rooms.Add(this);
                connected = new List<Room>();
                Visible = new List<Point>();
                Origin = this;

                do
                {
                    MinC = new Point(gen.rand.Next(gen.width - width - 1), gen.rand.Next(gen.height - height - 1));
                    MaxC = new Point(MinC.Col + width, MinC.Row + height);
                } while (gen.anyCollide(this));

                Center = new Point((int)((MinC.Col + MaxC.Col) * 0.5), (int)((MinC.Row + MaxC.Row) * 0.5));

                place();
            }

            public bool Collide(Room other)
            {
                if (this.MinC.Col > other.MaxC.Col ||
                    this.MaxC.Col < other.MinC.Col ||
                    this.MinC.Row > other.MaxC.Row ||
                    this.MaxC.Row < other.MinC.Row)
                    return false;
                else
                    return true;
            }

            public int Dist(int x, int y)
            {
                int closeX;
                int closeY;

                if (x > MaxC.Col)
                    closeX = MaxC.Col;
                else if (x < MinC.Col)
                    closeX = MinC.Col;
                else
                    closeX = x;

                if (y > MaxC.Row)
                    closeY = MaxC.Row;
                else if (y < MinC.Row)
                    closeY = MinC.Row;
                else
                    closeY = y;

                return (int)Math.Sqrt(Math.Pow(x - closeX, 2) + Math.Pow(y - closeY, 2));
            }

            public Point Random()
            {
                return new Point(gen.rand.Next(MinC.Col + 1, MaxC.Col - 1), gen.rand.Next(MinC.Row + 1, MaxC.Row - 1));
            }

            public void Connect(Room other)
            {
                if (this.connected.Contains(other))
                    return;

                if (this.Set != other.Set)
                {
                    Debug.Print($"Connected {this.Center} ({this.Set}), to {other.Center} ({other.Set}) Total: {gen.sets - 1}");
                    other.setChange(Set);
                    gen.sets--;
                }

                this.connected.Add(other);
                other.connected.Add(this);
            }

            private void setChange(int nextSet)
            {
                Set = nextSet;
                foreach (Room room in connected)
                {
                    if (room.Set != nextSet)
                        room.setChange(nextSet);
                }
            }

            private void place()
            {
                int width = MaxC.Col - MinC.Col;
                int height = MaxC.Row - MinC.Row;
                for (int i = 1; i < width - 1; i++)
                {
                    for (int j = 1; j < height - 1; j++)
                    {
                        Point p = new Point(MinC.Col + i, MinC.Row + j);
                        Visible.Add(p);
                        gen.place(Tile.FLOOR, p, this);
                    }
                }

                for (int i = 1; i < width - 1; i++)
                {
                    Point p = new Point(MinC.Col + i, MinC.Row);
                    Visible.Add(p);
                    gen.place(Tile.HOR_WALL, p, this);
                }

                for (int i = 1; i < width - 1; i++)
                {
                    Point p = new Point(MinC.Col + i, MinC.Row + height - 1);
                    Visible.Add(p);
                    gen.place(Tile.HOR_WALL, p, this);
                }

                for (int i = 1; i < height - 1; i++)
                {
                    Point p = new Point(MinC.Col, MinC.Row + i);
                    Visible.Add(p);
                    gen.place(Tile.VER_WALL, p, this);
                }

                for (int i = 1; i < height - 1; i++)
                {
                    Point p = new Point(MinC.Col + width - 1, MinC.Row + i);
                    Visible.Add(p);
                    gen.place(Tile.VER_WALL, p, this);
                }
            }
        }

        private interface IGenRoom
        {
            public List<Point> Visible { get; }
            public Room Origin { get; }
        }
    }
}