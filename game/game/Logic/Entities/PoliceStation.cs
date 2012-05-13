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
        class members
        ****************/

        private readonly int policemenCap;
        private int amountOfPolicemen;
        private bool onAlert;
        private Cop toConstruct;
        private List<Direction> _path = new List<Direction>(); //TODO - delete after I won't need to control the plice

        //TODO - set the whole alert operation. another idea - after alert wanes, begin "destroying" cops?

        /***********
         * constructor
         **********/
        internal PoliceStation(Game.Vector realSize, int sizeModifier, Vector exit)
            : base(sizeModifier, Entity.reactionPlaceHolder, realSize, Affiliation.INDEPENDENT)
        {
            
            base.ExitPoint = exit;
            this.policemenCap = sizeModifier / POLICE_SIZE_MODIFIER;
            this.amountOfPolicemen = 0;
            this.onAlert = false;
            reactionFunction react = delegate(List<Entity> ent)
                {
                    if (toConstruct == null) toConstruct = new Cop(this, this._path);
                    this._readyToBuild = true;
                    return new ConstructReaction(toConstruct);
                };
            this.ReactionFunction = react;
        }

        /******************
        Methods
        ****************/

        public MovingEntity getConstruct()
        {
            Cop temp = this.toConstruct;
            this.toConstruct = new Cop(this, this._path);
            this.amountOfPolicemen++;
            return temp;
        }

        internal List<Direction> Path
        {
            get { return _path; }
            set { _path = value; }
        }

        bool Constructor.readyToConstruct()
        {
            if (Path.Count > 0)
            {
                if (onAlert)
                {
                    if (this.amountOfPolicemen < this.policemenCap * 3)
                    {
                        return base.readyToConstruct();
                    }
                    else //TODO - this is the only place where we remove the policestation's alert. should this be so?
                    {
                        this.onAlert = false;
                        return false;
                    }
                }
                else
                {
                    return (base.readyToConstruct() && (this.amountOfPolicemen <= this.policemenCap));
                }
            }
            return false;
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

        public override string ToString()
        {
            return "Police station, " + base.ToString();
        }
}
}
