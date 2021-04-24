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
        //Represents the object's position on screen
        public Point Point { get; }
        //Controls collidability. False if the object can be walked through, like a door or floor,
        //and true if it cannot, like a wall
        public bool Solid { get; }

        /// <summary>
        /// Uses the console to draw the game object to the screen. This can be implemented
        /// differently for each game object, but should only affect the object's own point
        /// </summary>
        public void Paint();

        /// <summary>
        /// Does what the object does when the player interacts with the space it's on
        /// </summary>
        /// <returns>Returns true if the action results in the player being able to move</returns>
        public bool OnCollision();
    }
}
