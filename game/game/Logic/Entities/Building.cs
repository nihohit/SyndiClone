using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic.Entities
{
    class Building : Entity
    {
        /******************
        class consts
        ****************/

        protected const int BASE_BUILD_REACTION_TIME = 200;
        protected const int BASE_BUILD_HEALTH = 10;

        private Vector _exitPoint;
        protected const int AMOUNT_OF_STEPS_BEFORE_BUILDING = 100;

        public Building(int reactionTime, reactionFunction reaction, int health,  
            Vector size, Affiliation loyalty, Sight sight)
            : base(reactionTime, reaction, health, entityType.BUILDING, size, loyalty, sight, Visibility.SOLID)
        {
        }

        internal Vector ExitPoint
        {
            get { return _exitPoint; }
            set { _exitPoint = value; }
        }

    }
}
