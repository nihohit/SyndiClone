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

        protected const int BASE_BUILD_REACTION_TIME = 200;
        protected const int BASE_BUILD_HEALTH = 10;
        protected const int AMOUNT_OF_STEPS_BEFORE_BUILDING = 1000;

        private int _sizeModifier;
        private int _timeToConstruct;
        private Vector _exitPoint;
        protected bool _readyToBuild;
        

        public Building(int reactionTime, reactionFunction reaction, int health, Vector size, Affiliation loyalty, Sight sight)
            : base(BASE_BUILD_REACTION_TIME/reactionTime, reaction, BASE_BUILD_HEALTH*health, entityType.BUILDING, size, loyalty, sight, Visibility.SOLID)
        {
            this._sizeModifier = size.X * size.Y / 300;
            this._timeToConstruct = 0;
            _readyToBuild = true;
        }

        internal Vector ExitPoint
        {
            get { return _exitPoint; }
            set { _exitPoint = value; }
        }

        internal override void destroy()
        {
            System.Console.Out.WriteLine("building destroyed");
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
