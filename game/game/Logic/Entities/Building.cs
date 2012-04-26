using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic.Entities
{
    abstract class Building : Entity
    {
        /******************
        class consts
        ****************/

        protected const int BASE_BUILD_REACTION_TIME = 10;
        protected const int BASE_BUILD_HEALTH = 10;
        protected const int AMOUNT_OF_STEPS_BEFORE_BUILDING = 2000;

        private int _sizeModifier;
        private int _timeToConstruct;
        private Vector _exitPoint;
        protected bool _readyToBuild;
        

        public Building(int sizeModifier, reactionFunction reaction, Vector size, Affiliation loyalty)
            : base(BASE_BUILD_REACTION_TIME*sizeModifier, reaction, BASE_BUILD_HEALTH*sizeModifier, entityType.BUILDING, size, loyalty)
        {
            this._sizeModifier = sizeModifier;
            this._timeToConstruct = 0;
            _readyToBuild = true;
            List<Upgrades> list = new List<Upgrades>();
            list.Add(Upgrades.VISIBILITY_SOLID);
            list.Add(Upgrades.BUILDING_BLIND);
            base.upgrade(list);
        }

        internal Vector ExitPoint
        {
            get { return _exitPoint; }
            set { _exitPoint = value; }
        }

        internal override void destroy()
        {
            base.destroy();
        }
    
        internal bool readyToConstruct()
        {
            bool ready = base.reachAffect(AMOUNT_OF_STEPS_BEFORE_BUILDING, this._timeToConstruct, this._sizeModifier) && _readyToBuild;
            if (ready)
            {
                this._timeToConstruct -= AMOUNT_OF_STEPS_BEFORE_BUILDING;
                this._readyToBuild = false;
            }
            else
            {
                this._timeToConstruct += this._sizeModifier;
            }
            return ready;
        }
    }
}
