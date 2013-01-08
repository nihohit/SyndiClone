using Game.Logic.Entities;
using System.Collections.Generic;

namespace Game.Logic
{
    /*
     * This class is in charge of choosing targets to a given shooter out of the list of enemies in range of him. 
     */
    internal static class Targeters
    {

        public static Entity threatTargeterHigh(List<Entity> entities, Affiliation loyalty)
        {
            Entity target = null;
            int seenThreat = 0;
            foreach (Entity ent in entities)
            {
                if ((ent.Threat > seenThreat) && (ent.Loyalty != loyalty))
                {
                    seenThreat = ent.Threat;
                    target = ent;
                }
            }
            return target;
        }

        public static Entity speedTargeterHigh(List<Entity> entities, Affiliation loyalty)
        {
            Entity target = null;
            int speed = 0;
            foreach (Entity ent in entities)
            {
                if ((ent is MovingEntity && ((MovingEntity)ent).Speed > speed) || (ent.Type == entityType.BUILDING && target == null) && (ent.Loyalty != loyalty))
                {
                    speed = ((MovingEntity)ent).Speed;
                    target = ent;
                }
            }

            return target;
        }

        public static Entity healthTargeterHigh(List<Entity> entities, Affiliation loyalty)
        {
            Entity target = null;
            int health = 0;
            foreach (Entity ent in entities)
            {
                if ((ent.Health > health) && (ent.Loyalty != loyalty))
                {
                    health = ent.Health;
                    target = ent;
                }
            }

            return target;
        }

        public static Entity threatTargeterLow(List<Entity> entities, Affiliation loyalty)
        {
            Entity target = null;
            int seenThreat = 10000;
            foreach (Entity ent in entities)
            {

                if ((ent.Threat < seenThreat) && (ent.Loyalty != loyalty))
                {
                    seenThreat = ent.Threat;
                    target = ent;
                }
            }

            return target;
        }

        public static Entity speedTargeterLow(List<Entity> entities, Affiliation loyalty)
        {
            Entity target = null;
            int speed = 10000;
            foreach (Entity ent in entities)
            {
                if ((ent is MovingEntity && ((MovingEntity)ent).Speed < speed) || (ent.Type == entityType.BUILDING && target == null) && (ent.Loyalty != loyalty))
                {
                    speed = ((MovingEntity)ent).Speed;
                    target = ent;
                }
            }

            return target;
        }

        public static Entity healthTargeterLow(List<Entity> entities, Affiliation loyalty)
        {
            Entity target = null;
            int health = 10000;
            foreach (Entity ent in entities)
            {
                if ((ent.Health < health) && (ent.Loyalty != loyalty))
                {
                    health = ent.Health;
                    target = ent;
                }
            }

            return target;
        }

        internal static Entity simpleTestTargeter(List<Entity> list, Affiliation affiliation)
        {
            Entity target = null;
            int seenThreat = 10000;
            foreach (Entity ent in list)
            {

                if ((ent.Threat < seenThreat) && (ent.Loyalty != affiliation) && (ent.Type != entityType.BUILDING))
                {
                    seenThreat = ent.Threat;
                    target = ent;
                }
            }

            return target;
        }
    }
}
