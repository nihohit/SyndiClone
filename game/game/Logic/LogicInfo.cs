﻿using System.Collections.Generic;
using Game.Logic.Entities;


namespace Game.Logic
{
    internal delegate bool EntityChecker(Entity ent);
    internal delegate Point HitFunction(Point target);
    internal delegate void Effect(Entity ent);
    internal delegate bool wasBlocked(Visibility ent);
    internal delegate Entity targetChooser(List<Entity> targets);
    internal delegate Reaction reactionFunction(List<Entity> ent);

    internal enum Action { FIRE_AT, IGNORE, RUN_AWAY_FROM, MOVE_TOWARDS, MOVE_WHILE_SHOOT, CREATE_ENTITY }
    internal enum entityType { PERSON, VEHICLE, BUILDING}
    internal enum Visibility { CLOAKED, MASKED, REVEALED, SOLID }
    internal enum Affiliation { INDEPENDENT, CORP1, CORP2, CORP3, CORP4 }
    internal enum SightType { CIV_SIGHT }
    internal enum WeaponType { PISTOL, ASSAULT, BAZOOKA, SNIPER, RAILGUN }
    internal enum BufferType { MOVE, SHOT, DESTROY, CREATE }
    internal enum TypesOfShot { SIGHT }
    internal enum Direction { LEFT, RIGHT, UP, DOWN }

    internal struct Area
    {
        private readonly Point _entry; //the top left of the shape
        private readonly Vector _size;

        internal void flip()
        {
            this._size.flip();
        }



        internal Vector Size
        {
            get { return _size; }
        }

        public Point Entry
        {
            get { return _entry; }
        }

        internal Area(Point entry, Vector size)
        {
            this._entry = entry;
            this._size = size;
        }

        internal Area(Area location, Vector vector)
        {
            this._entry = new Point(location.Entry, vector);
            this._size = location.Size.addVector(vector);
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

        public static Entity threatTargeterHigh(List<Entity> entities)
        {
            Entity target = null;
            int seenThreat = 0;
            foreach (Entity ent in entities)
            {
                if (ent.Threat > seenThreat)
                {
                    seenThreat = ent.Threat;
                    target = ent;
                }
            }

            return target;
        }

        public static Entity speedTargeterHigh(List<Entity> entities)
        {
            Entity target = null;
            int speed = 0;
            foreach (Entity ent in entities)
            {
                if ((ent is MovingEntity && ((MovingEntity)ent).Speed > speed) || (ent.Type == entityType.BUILDING && target == null))
                {
                    speed = ((MovingEntity)ent).Speed;
                    target = ent;
                }
            }

            return target;
        }

        public static Entity healthTargeterHigh(List<Entity> entities)
        {
            Entity target = null;
            int health = 0;
            foreach (Entity ent in entities)
            {
                if (ent.Health > health)
                {
                    health = ent.Health;
                    target = ent;
                }
            }

            return target;
        }

        public static Entity threatTargeterLow(List<Entity> entities)
        {
            Entity target = null;
            int seenThreat = 10000;
            foreach (Entity ent in entities)
            {
                if (ent.Threat < seenThreat)
                {
                    seenThreat = ent.Threat;
                    target = ent;
                }
            }

            return target;
        }

        public static Entity speedTargeterLow(List<Entity> entities)
        {
            Entity target = null;
            int speed = 10000;
            foreach (Entity ent in entities)
            {
                if ((ent is MovingEntity && ((MovingEntity)ent).Speed < speed) || (ent.Type == entityType.BUILDING && target == null))
                {
                    speed = ((MovingEntity)ent).Speed;
                    target = ent;
                }
            }

            return target;
        }

        public static Entity healthTargeterLow(List<Entity> entities)
        {
            Entity target = null;
            int health = 10000;
            foreach (Entity ent in entities)
            {
                if (ent.Health < health)
                {
                    health = ent.Health;
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

    internal interface BufferEvent
    {
        BufferType type();
    }

    internal struct ShotEvent : BufferEvent
    {
        private readonly TypesOfShot _shot;
        private readonly Point _exit;
        private readonly Point _target;

        internal ShotEvent(TypesOfShot shot, Point exit, Point entry)
        {
            this._exit = exit;
            this._target = entry;
            this._shot = shot;
        }

        internal TypesOfShot Shot
        {
            get { return _shot; }
        }

        internal Point Target
        {
            get { return _target; }
        }

        internal Point Exit
        {
            get { return _exit; }
        }

        public BufferType type()
        {
            return BufferType.SHOT;
        }

    }

    internal struct DestroyEvent : BufferEvent
    {
        private readonly Area _area;
        private readonly Entity _ent;

        internal DestroyEvent(Area area, Entity ent)
        {
            this._area = area;
            this._ent = ent;
        }

        internal Entity Ent
        {
            get { return _ent; }
        }

        internal Area Area
        {
            get { return _area; }
        }

        BufferType BufferEvent.type()
        {
            return BufferType.DESTROY;
        }
    }

    internal struct MoveEvent : BufferEvent
    {
        private readonly Area _exit;
        private readonly Area _entry;
        private readonly MovingEntity _mover;

        internal MoveEvent(Area exit, Area entry, MovingEntity mover)
        {
            this._entry = entry;
            this._exit = exit;
            this._mover = mover;
        }

        internal MovingEntity Mover
        {
            get { return _mover; }
        }

        internal Area Entry
        {
            get { return _entry; }
        }

        public Area Exit
        {
            get { return _exit; }
        }

        public BufferType type()
        {
            return BufferType.MOVE;
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