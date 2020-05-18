using System.Collections.Generic;
using Game.Logic.Entities;

namespace Game.Logic {
  /*
   * This class is in charge of choosing targets to a given shooter out of the list of enemies in range of him.
   */

  public static class Targeters {

    public static Entity ThreatTargeterHigh(IEnumerable<Entity> entities, Affiliation loyalty) {
      Entity target = null;
      int seenThreat = 0;
      foreach (Entity ent in entities) {
        if ((ent.Threat > seenThreat) && (ent.Loyalty != loyalty)) {
          seenThreat = ent.Threat;
          target = ent;
        }
      }
      return target;
    }

    public static Entity SpeedTargeterHigh(IEnumerable<Entity> entities, Affiliation loyalty) {
      Entity target = null;
      int speed = 0;
      foreach (Entity ent in entities) {
        if ((ent is MovingEntity && ((MovingEntity) ent).Speed > speed) || (ent.Type == EntityType.BUILDING && target == null) && (ent.Loyalty != loyalty)) {
          speed = ((MovingEntity) ent).Speed;
          target = ent;
        }
      }

      return target;
    }

    public static Entity HealthTargeterHigh(IEnumerable<Entity> entities, Affiliation loyalty) {
      Entity target = null;
      int health = 0;
      foreach (Entity ent in entities) {
        if ((ent.Health > health) && (ent.Loyalty != loyalty)) {
          health = ent.Health;
          target = ent;
        }
      }

      return target;
    }

    public static Entity ThreatTargeterLow(IEnumerable<Entity> entities, Affiliation loyalty) {
      Entity target = null;
      int seenThreat = 10000;
      foreach (Entity ent in entities) {
        if ((ent.Threat < seenThreat) && (ent.Loyalty != loyalty)) {
          seenThreat = ent.Threat;
          target = ent;
        }
      }

      return target;
    }

    public static Entity SpeedTargeterLow(IEnumerable<Entity> entities, Affiliation loyalty) {
      Entity target = null;
      int speed = 10000;
      foreach (Entity ent in entities) {
        if ((ent is MovingEntity && ((MovingEntity) ent).Speed < speed) || (ent.Type == EntityType.BUILDING && target == null) && (ent.Loyalty != loyalty)) {
          speed = ((MovingEntity) ent).Speed;
          target = ent;
        }
      }

      return target;
    }

    public static Entity HealthTargeterLow(IEnumerable<Entity> entities, Affiliation loyalty) {
      Entity target = null;
      int health = 10000;
      foreach (Entity ent in entities) {
        if ((ent.Health < health) && (ent.Loyalty != loyalty)) {
          health = ent.Health;
          target = ent;
        }
      }

      return target;
    }

    public static Entity SimpleTestTargeter(IEnumerable<Entity> list, Affiliation affiliation) {
      Entity target = null;
      int seenThreat = 10000;
      foreach (Entity ent in list) {
        if ((ent.Threat < seenThreat) && (ent.Loyalty != affiliation) && (ent.Type != EntityType.BUILDING)) {
          seenThreat = ent.Threat;
          target = ent;
        }
      }

      return target;
    }
  }
}
