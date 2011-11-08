using System.Collections.Generic;

namespace Game.Logic.Entities
{
    abstract class Person : MovingEntity
    {

        protected Person(int reactionTime, reactionFunction react, int health, Affiliation loyalty, Sight sight, int speed, LinkedList<Direction> path) :
            base(reactionTime, react, health, entityType.PERSON, new Vector(1, 1), loyalty, Visibility.REVEALED, sight, speed, path)
        {
        }

    }
}
