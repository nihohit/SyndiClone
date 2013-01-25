using System;

namespace Game.Logic.Entities
{
    public abstract class Entity
    {
        #region static members

        private static int s_Id = 0;

        #endregion

        #region consts

        const int TIME_TO_REACT = 1000;

        #endregion

        #region fields

        private int m_timeAcc = 0;

        #endregion

        #region properties

        public int ReactionTime { get; private set; }

        public Reaction Reaction { get; set; }

        public int Threat { get; set; }

        public int Health { get; private set; }

        public reactionFunction ReactionFunction { get; set; }

        public UniqueList<Entity> WhatSees { get; set; }

        public Affiliation Loyalty { get; set; }

        public Visibility VisibleStatus { get; set; }

        public Sight SightSystem { get; private set; }

        public EntityType Type { get; private set; }

        public Vector Size { get; set; }

        public int Id { get; private set;  }

        public VisualEntityInformation VisualInfo { get; protected set; }

        #endregion

        #region constructor

        protected Entity(int reactionTime, reactionFunction reaction, int health, EntityType type, Vector size, Affiliation loyalty)
        {
            Health = health;
            Size = size;
            Type = type;
            Loyalty = loyalty;
            SightSystem = Sight.instance(SightType.DEFAULT_SIGHT);
            Reaction = new IgnoreReaction();
            VisibleStatus = Visibility.REVEALED;
            ReactionFunction = reaction;
            ReactionTime = reactionTime;
            WhatSees = new UniqueList<Entity>();
            s_Id++;
            Id = s_Id;
            VisualInfo = new VisualEntityInformation(Type, Loyalty, size, Id);
        }

        #endregion

        #region public methods

        public bool ReachAffect(int topLevel, int currentAcc, int increaseBy)
        {
            return (currentAcc + increaseBy >= topLevel); 
        }

        public bool Destroyed()
        {
            return Health <= 0;
        }

        public override string ToString()
        {
            return String.Format("Id: {0}, Health: {1}, Size: {2}", Id, Health, Size.ToString());
        }

        #region virtual methods

        public virtual void Hit(int damage)
        {
            Health -= damage;
        }

        virtual public void ResolveOrders()
        {
            Reaction = ReactionFunction(WhatSees);
        }

        public virtual bool DoesReact()
        {
            bool ans = ReachAffect(TIME_TO_REACT, m_timeAcc, ReactionTime);
            if (ans)
            {
                m_timeAcc -= TIME_TO_REACT;
            }
            else
            {
                m_timeAcc += ReactionTime;
            }
            return ans;
        }

        public virtual void Destroy()
        {
        }

        protected virtual void Upgrade(System.Collections.Generic.List<Upgrades> list)
        {
            foreach (Upgrades upgrade in list)
            {
                switch (upgrade)
                {
                    case(Upgrades.BULLETPROOF_VEST):
                        Health += 7;
                        break;
                    case(Upgrades.VISIBILITY_SOLID):
                        VisibleStatus = Visibility.SOLID;
                        break;
                    case(Upgrades.BUILDING_BLIND):
                        SightSystem = Sight.instance(SightType.BLIND);
                        break;
                }
            }
        }
        
        #endregion

        #region static methods

        protected static Reaction ReactionPlaceHolder(System.Collections.Generic.List<Entity> list)
        {
            return new IgnoreReaction();
        }

        #endregion

        #endregion
    }
}
