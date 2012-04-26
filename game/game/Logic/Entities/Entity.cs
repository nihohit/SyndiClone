
namespace Game.Logic.Entities
{
    internal abstract class Entity

    {
        /******************
        class consts
        ****************/        
        
        const int timeToReact = 1000;

        /******************
        class members
        ****************/        
        
        private int _health;
        protected Vector _size;
        private readonly entityType _type;
        private Reaction _reaction;
        private Affiliation _loyalty;
        private Sight _sight;
        private Visibility _visible;
        protected reactionFunction _howReact;
        private UniqueList<Entity> _whatSees;
        private int _threat = 0;
        private readonly int _reactionTime;
        private int _timeAcc = 0;

        /******************
        constructors
        ****************/

        protected Entity(int reactionTime, reactionFunction reaction, int health,entityType type, Vector size, Affiliation loyalty)
        {
            this._health = health;
            this._size = size;
            this._type = type;
            this._loyalty = loyalty;
            this._sight = Sight.instance(SightType.DEFAULT_SIGHT);
            this._reaction = new Reaction (null, Action.IGNORE);
            this._visible = Visibility.REVEALED;
            this._howReact = reaction;
            this._reactionTime = reactionTime;
            this.WhatSees = new UniqueList<Entity>();
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

        protected static Reaction reactionPlaceHolder(System.Collections.Generic.List<Entity> list)
        {
            return new Reaction();
        }

        protected virtual void upgrade(System.Collections.Generic.List<Upgrades> list)
        {
            foreach (Upgrades upgrade in list)
            {
                switch (upgrade)
                {
                    case(Upgrades.BULLETPROOF_VEST):
                        this._health += 7;
                        break;
                    case(Upgrades.VISIBILITY_SOLID):
                        this._visible = Visibility.SOLID;
                        break;
                    case(Upgrades.BUILDING_BLIND):
                        this._sight = Sight.instance(SightType.BLIND);
                        break;

                }

            }
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

        public override string ToString()
        {
            return "Health: " + this._health + " size: " + this.Size.ToString();
        }

    }
}
