﻿
namespace Game.Logic.Entities
{
    internal abstract class Entity

    {
        private int _health;

        public int Health
        {
            get { return _health; }
        }
        private readonly Vector _size;
        private readonly entityType _type;
        private Reaction _reaction;
        private Affiliation _loyalty;
        private readonly Sight _sight;
        private Visibility _visible;
        private readonly reactionFunction _howReact;
        private UniqueList<Entity> _whatSees;
        private int _threat = 0;

        public int Threat
        {
            get { return _threat; }
            set { _threat = value; }
        }

        internal reactionFunction HowReact
        {
            get { return _howReact; }
        }

        internal UniqueList<Entity> WhatSees
        {
            get { return _whatSees; }
            set { _whatSees = value; }
        }

        internal Affiliation Loyalty
        {
            get { return _loyalty; }
            set { _loyalty = value; }
        }

        internal Visibility Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        virtual internal Reaction resolveOrders()
        {
            this.Reaction = this.HowReact(this.WhatSees);
            return this.Reaction;
        }

        protected Entity(reactionFunction reaction, int health,entityType type, Vector size, Affiliation loyalty, Sight sight, Visibility visibility)
        {
            this._health = health;
            this._size = size;
            this._type = type;
            this._loyalty = loyalty;
            this._sight = sight;
            this._reaction = new Reaction (null, Action.IGNORE);
            this._visible = visibility;
            this._howReact = reaction;
        }

        internal Reaction Reaction
        {
            get { return _reaction; }
            set { _reaction = value; }
        }

        internal Sight Sight
        {
            get { return _sight; }
        } 

        internal entityType Type
        {
            get { return _type; }
        }

        internal Vector Size
        {
            get { return _size; }
        }

        internal virtual bool hit(int damage)
        {
            this._health -= damage;
            return (this._health <= 0);
        }

        internal abstract bool blocksVision();

    }
}
