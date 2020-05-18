using System.Collections.Generic;

namespace Game.Logic.Entities {

  internal abstract class Person : MovingEntity {
    private const int PERSON_HEALTH = 3;
    private const int PERSON_REACTION_TIME = 10;
    private const int PERSON_SPEED = 10;

    protected Person(reactionFunction react, Affiliation loyalty, List<Direction> path, Direction headed):
      base(PERSON_REACTION_TIME, react, PERSON_HEALTH, EntityType.PERSON, new Vector(1, 1), loyalty, Visibility.REVEALED, Sight.instance(SightType.DEFAULT_SIGHT), PERSON_SPEED, path, headed) { }
  }
}
