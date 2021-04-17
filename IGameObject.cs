using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGenerationDemo
{
    /// <summary>
    /// The interface used for every object that exists on the game grid. Needs to exist within a
    /// Dungeon object to really function.
    /// </summary>
    interface IGameObject
    {
        //Represents the object's row, x position, or distance from the left side of the screen
        public int Row { get; }
        //Represents the object's column, y position, or distance from the yop side of the screen
        public int Col { get; }
        //Controls collidability. False if the object can be walked through, like a door or floor,
        //and true if it cannot, like a wall
        public bool Solid { get; }

        /// <summary>
        /// Uses the console to draw the game object to the screen. This can be implemented
        /// differently for each game object, but should only affect the object's own row and col
        /// </summary>
        public void Paint();
    }
}
