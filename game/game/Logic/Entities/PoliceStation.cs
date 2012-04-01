using Game.Logic;
using System.Collections.Generic;

namespace Game.Logic.Entities 
{
    class PoliceStation : Building, Constructor
    {
        /******************
        class consts
        ****************/
        const int POLICE_SIZE_MODIFIER = 3;

        /******************
        class fields
        ****************/

        private readonly int policemenCap;
        private int amountOfPolicemen;
        private bool onAlert;

        //TODO - set the whole alert operation. another idea - after alert wanes, begin "destroying" cops?

        /***********
         * constructor
         **********/
        internal PoliceStation(Game.Vector realSize, int sizeModifier, Vector exit)
            : base(BASE_BUILD_REACTION_TIME / sizeModifier, 
            policeBuildReact, BASE_BUILD_HEALTH * sizeModifier, realSize, Affiliation.INDEPENDENT, Sight.instance(SightType.POLICE_SIGHT))
        {
            
            base.ExitPoint = exit;
            this.policemenCap = sizeModifier / POLICE_SIZE_MODIFIER;
            this.amountOfPolicemen = 0;
            this.onAlert = false;
            reactionFunction react = delegate(List<Entity> ent)
                {
                    Cop temp = new Cop(this);
                    this.amountOfPolicemen++;
                    return new Reaction(temp, Action.CREATE_ENTITY);
                };
            this.ReactionFunction = react;
        }

        /******************
        Methods
        ****************/

        /*
         * This function is just a placeholder.
         */
        public static Reaction policeBuildReact(List<Entity> ent)
        {
            Cop temp = null;
            return new Reaction(temp, Action.CREATE_ENTITY);
        }

        public MovingEntity getConstruct()
        {
            return (MovingEntity)this.Reaction.Focus;
        }

        bool Constructor.readyToConstruct()
        {
            if (onAlert)
            {
                if (this.amountOfPolicemen < this.policemenCap * 3)
                    return true;
                else //TODO - this is the only place where we remove the policestation's alert. should this be so?
                {
                    this.onAlert = false;
                    return false;
                }
            }
            else
                return this.amountOfPolicemen <= this.policemenCap;
        }

        public Vector exitPoint()
        {
            return base.ExitPoint;
        }

        public bool Alert
        {
            get { return onAlert; }
            set { onAlert = value; }
        }

        internal void policemanDestroyed()
        {
 	        this.amountOfPolicemen--;
            this.Alert = true;
        }

    
}
}
