using System.Collections.Generic;

namespace Game.Logic.Entities
{
    abstract class Person : MovingEntity
    {

        const int PERSON_HEALTH = 3;
        const int PERSON_REACTION_TIME =  10;
        const int PERSON_SPEED = 10;

        protected Person(reactionFunction react, Affiliation loyalty, LinkedList<Direction> path) :
            base(PERSON_REACTION_TIME, react, PERSON_HEALTH, entityType.PERSON, new Vector(1, 1), loyalty, Visibility.REVEALED, Sight.instance(SightType.DEFAULT_SIGHT), PERSON_SPEED, path)
        {
        }

    }
}
