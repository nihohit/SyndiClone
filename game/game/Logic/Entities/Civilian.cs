using System.Collections.Generic;
using Game.Logic;


namespace Game.Logic.Entities
{
    class Civilian : Person
    {
        #region consts

        const int CIV_RUNNING_TIME = 100;

        #endregion

        #region fields

        private bool m_fleeing;
        private int m_timeRunning = 0;

        #endregion

        #region constructors

        public Civilian(Direction headed) :
            base(CivReact, Affiliation.CIVILIAN, new List<Direction>(), headed)
        {
            m_fleeing = false;
        }

        #endregion

        #region public methods

        public void RunningAway()
        {
            m_fleeing = true;
        }

        public static Reaction CivReact(List<Entity> entities)
        {
            Entity threat = Targeters.ThreatTargeterHigh(entities, Affiliation.CIVILIAN);
            Reaction react;

            if (threat == null) //TODO - add ignoring cops
            {
                react = new IgnoreReaction();
            }

            else
            {
                react = new RunAwayReaction(threat);
            }

            return react;
        }


        public override bool DoesReact()
        {
            if (m_fleeing)
            {
                if (m_timeRunning < CIV_RUNNING_TIME)
                {
                    m_timeRunning += ReactionTime;
                    return false;
                }
                else
                {
                    m_timeRunning = 0;
                    m_fleeing = false;
                }
            }
            return base.DoesReact();
        }

        public override string ToString()
        {
            return "Civilian, " + base.ToString();
        }

        #endregion
    }
}
