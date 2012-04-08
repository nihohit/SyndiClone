
namespace Game.Logic.Entities
{
    internal abstract class Entity

    {
        /******************
        class consts
        ****************/        
        
        const int timeToReact = 1000;

        /******************
        class fields
        ****************/        
        
        private int _health;
        protected Vector _size;
        private readonly entityType _type;
        private Reaction _reaction;
        private Affiliation _loyalty;
        private readonly Sight _sight;
        private Visibility _visible;
        protected reactionFunction _howReact;
        private UniqueList<Entity> _whatSees;
        private int _threat = 0;
        private readonly int _reactionTime;
        private int _timeAcc = 0;

        /******************
        constructors
        ****************/

        protected Entity(int reactionTime, reactionFunction reaction, int health,entityType type, Vector size, Affiliation loyalty, Sight sight, Visibility visibility)
        {
            this._health = health;
            this._size = size;
            this._type = type;
            this._loyalty = loyalty;
            this._sight = sight;
            this._reaction = new Reaction (null, Action.IGNORE);
            this._visible = visibility;
            this._howReact = reaction;
            this._reactionTime = reactionTime;
        }

        /******************
        Methods
        ****************/

        internal virtual void hit(int damage)
        {
            this._health -= damage;
        }

        virtual internal void resolveOrders()
        {
            this.Reaction = this.ReactionFunction(this.WhatSees);
        }

        internal virtual bool doesReact()
        {
            bool ans = reachAffect(timeToReact, _timeAcc, _reactionTime);
            if (ans)
            {
                _timeAcc -= timeToReact;
            }
            else
            {
                _timeAcc += _reactionTime;
            }
            return ans;
        }

        public bool reachAffect(int topLevel, int currentAcc, int increaseBy)
        {
            return (currentAcc + increaseBy >= topLevel); 
        }

        internal bool destroyed()
        {
            return this._health <= 0;
        }

        internal virtual void destroy()
        {
        }

        /******************
        Getters & setters
        ****************/
        public int ReactionTime
        {
            get { return _reactionTime; }
        } 


        internal Reaction Reaction
        {
            get { return _reaction; }
            set { _reaction = value; }
        }

        public int Threat
        {
            get { return _threat; }
            set { _threat = value; }
        }

        public int Health
        {
            get { return _health; }
        }

        internal reactionFunction ReactionFunction
        {
            get { return _howReact; }
            set { _howReact = value; }
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



    }
}
