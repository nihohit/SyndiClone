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
    internal enum MovementType { GROUND, HOVER, FLYER, CRUSHER }
    internal enum TerrainType { ROAD, WATER, BUILDING } 
    internal enum BlastType { } //different blast effect
    internal enum ShotType { SIGHT, PISTOL_BULLET }
    internal enum Direction { LEFT, RIGHT, UP, DOWN, UPLEFT, UPRIGHT, DOWNLEFT, DOWNRIGHT }
    internal enum Corporations { BIOTECH, STEALTH, ARMS, VEHICLES, VISION }
    internal enum Upgrades { BULLETPROOF_VEST, VISIBILITY_SOLID, BUILDING_BLIND, FLYER, HOVER, CRUSHER } 

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
