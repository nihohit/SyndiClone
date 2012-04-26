using System.Collections.Generic;
using Game.Logic.Entities;
using System;


namespace Game.Logic
{
    internal delegate bool EntityChecker(Entity ent); //These functions check basic questions about entities and return a bool
    internal delegate void Effect(Entity ent); //These functions simulate effects on entities. mostly will be damage
    internal delegate bool wasBlocked(Entity ent); //These functions check whether an entitiy blocks a certain effect
    internal delegate Entity targetChooser(List<Entity> targets); //These functions choose which entity, out of the list of possible entities, to target
    internal delegate Reaction reactionFunction(List<Entity> ent); //These functions set the reaction of entities

    internal enum Action { FIRE_AT, IGNORE, RUN_AWAY_FROM, MOVE_TOWARDS, MOVE_WHILE_SHOOT, CREATE_ENTITY } //This enum checks the possible actions entities can take
    internal enum entityType { PERSON, VEHICLE, BUILDING} //the different types of entities
    internal enum Visibility { CLOAKED, MASKED, REVEALED, SOLID } //the visibility of an entity
    internal enum Affiliation { INDEPENDENT, CORP1, CORP2, CORP3, CORP4, CIVILIAN } //to which player each entity belongs
    internal enum SightType { DEFAULT_SIGHT, BLIND } //different sights
    internal enum WeaponType { PISTOL, ASSAULT, BAZOOKA, SNIPER, RAILGUN } //different weapons
    internal enum BlastType { } //different blast effect
    internal enum ShotType { SIGHT, PISTOL_BULLET }
    internal enum Direction { LEFT, RIGHT, UP, DOWN }
    internal enum Corporations { BIOTECH, STEALTH, ARMS, VEHICLES, VISION }
    internal enum Upgrades { BULLETPROOF_VEST, VISIBILITY_SOLID, BUILDING_BLIND } 

    internal struct Area
    {
        private readonly Point _entry; //the top left of the shape
        private readonly Vector _size;

        internal void flip()
        {
            this._size.flip();
        }

        internal Area(Point entry, Vector size)
        {
            this._entry = entry;
            this._size = size;
        }

        internal Area(Area location, Vector vector)
        {
            this._entry = new Point(location.Entry, vector);
            this._size = location.Size;
        }

        internal Vector Size
        {
            get { return _size; }
        }

        public Point Entry
        {
            get { return _entry; }
        }

        internal Point[,] getPointArea()
        {
            Point[,] area = new Point[this._size.X, this._size.Y];

            for (int i = 0; i < this._size.X; i++)
            {
                for (int j = 0; j < this._size.Y; j++)
                {
                    area[i, j] = new Point(this._entry, new Vector(i, j));
                }
            }

            return area;
        }

        public override string ToString()
        {
            return "Area: entry - " + this._entry + " size - " + this._size;
        }

    }

    internal struct Reaction{

        private readonly Entity _focus;
        private readonly Action _actionChosen;

        internal Action ActionChosen
        {
            get { return _actionChosen; }
        }

        internal Entity Focus
        {
            get { return _focus; }
        } 

        internal Reaction(Entity ent, Action action)
        {
            this._actionChosen = action;
            this._focus = ent;
        }
    }

    internal static class Targeters 
    {

        public static Entity threatTargeterHigh(List<Entity> entities, Affiliation loyalty)
        {
            Entity target = null;
            int seenThreat = 0;
            foreach (Entity ent in entities)
            {
                if ((ent.Threat > seenThreat)  && (ent.Loyalty != loyalty))
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
                if ((ent.Health > health)  && (ent.Loyalty != loyalty))
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

                if ((ent.Threat < seenThreat)  && (ent.Loyalty != loyalty))
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
                if ((ent is MovingEntity && ((MovingEntity)ent).Speed < speed) || (ent.Type == entityType.BUILDING && target == null)  && (ent.Loyalty != loyalty))
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

    class UniqueList<T> : List<T>
    {
        public UniqueList()
            : base()
        {
        }

        public UniqueList(List<T> old)
            : base(old)
        {
        }

        public void uniqueAdd(T obj)
        {
            if (!base.Contains(obj)) base.Add(obj);
        }

        public void listAdd(UniqueList<T> list)
        {
            foreach (T t in list)
            {
                this.uniqueAdd(t);
            }
        }
    }

    internal class LocationFullException : System.ApplicationException
    {
        internal LocationFullException() { }
        internal LocationFullException(string message) { }

        // Constructor needed for serialization 
        // when exception propagates from a remoting server to the client.
        protected LocationFullException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }

}
