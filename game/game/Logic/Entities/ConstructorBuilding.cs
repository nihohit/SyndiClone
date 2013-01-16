using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic.Entities
{
    public abstract class ConstructorBuilding : Building
    {
        protected const int AMOUNT_OF_STEPS_BEFORE_BUILDING = 2000;

        private int m_timeToConstruct;
        protected bool m_readyToBuild;

        public ConstructorBuilding (int sizeModifier, reactionFunction reaction, Vector size, Affiliation loyalty) 
        : base (sizeModifier, reaction, size, loyalty)
        {
            m_timeToConstruct = 0;
            m_readyToBuild = true;
        }

        virtual public bool ReadyToConstruct()
        {
            bool ready = base.ReachAffect(AMOUNT_OF_STEPS_BEFORE_BUILDING, m_timeToConstruct, m_sizeModifier) && m_readyToBuild;
            if (ready)
            {
                m_timeToConstruct -= AMOUNT_OF_STEPS_BEFORE_BUILDING;
                m_readyToBuild = false;
            }
            else
            {
                m_timeToConstruct += m_sizeModifier;
            }
            return ready;
        }
    }
}
