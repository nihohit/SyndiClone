using System.Collections.Generic;

namespace Game.Logic.Entities
{
    abstract class Person : MovingEntity
    {

        protected Person(reactionFunction react, int health, Affiliation loyalty, Sight sight, int speed, LinkedList<Point> path) :
            base(react, health, entityType.PERSON, new Vector(1, 1), loyalty, Visibility.REVEALED, sight, speed, path)
        {
        }

        internal override bool blocksVision()
        {
            return false;
        }
    }
}
