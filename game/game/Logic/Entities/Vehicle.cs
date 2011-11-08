using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic.Entities
{
    abstract class Vehicle : MovingEntity
    {
        public Vehicle(int reactionTime, reactionFunction reaction, int health, entityType type, Vector size, Affiliation loyalty,
            Visibility visibility, Sight sight, int speed, LinkedList<Direction> path)
            : base(reactionTime, reaction, health, entityType.VEHICLE, size, loyalty, visibility, sight, speed, path)
        {
        }
    }
}
