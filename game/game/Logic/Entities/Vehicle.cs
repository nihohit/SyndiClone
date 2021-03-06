using System.Collections.Generic;

namespace Game.Logic.Entities {

  internal abstract class Vehicle : MovingEntity {

    public Vehicle(int reactionTime, reactionFunction reaction, int health, EntityType type, Vector size, Affiliation loyalty,
      Visibility visibility, Sight sight, int speed, List<Direction> path, Direction headed) : base(reactionTime, reaction, health, EntityType.VEHICLE, size, loyalty, visibility, sight, speed, path, headed) { }
  }
}
