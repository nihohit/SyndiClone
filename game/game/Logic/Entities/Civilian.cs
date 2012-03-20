using System.Collections.Generic;
using Game.Logic;


namespace Game.Logic.Entities
{
    class Civilian : Person
    {

        /******************
        Class consts
        ****************/

        const int CIV_HEALTH = 3;
        const int CIV_SPEED = 10;
        const int CIV_REACTION_TIME = 10;
        const int CIV_RUNNING_TIME = 100;

        /******************
        Class fields
        ****************/

        private bool fleeing;
        private int timeRunning = 0;

        /******************
        Constructors
        ****************/

        internal Civilian() :
            base(CIV_REACTION_TIME, civReact, CIV_HEALTH, Affiliation.CIVILIAN, Sight.instance(SightType.CIV_SIGHT), CIV_SPEED, new LinkedList<Direction>())
        {
            this.fleeing = false;
        }

        /******************
        Methods
        ****************/

        internal void runningAway()
        {
            this.fleeing = true;
        }

        public static Reaction civReact(List<Entity> entities)
        {
            Entity threat = Targeters.threatTargeterHigh(entities, Affiliation.CIVILIAN);
            Reaction react;

            if (threat == null) //TODO - add ignoring cops
            {
                react = new Reaction(null, Action.IGNORE);
            }

            else
            {
                react = new Reaction(threat, Action.RUN_AWAY_FROM);
            }

            return react;
        }


        internal override bool doesReact()
        {
            if (this.fleeing)
            {
                if (this.timeRunning < CIV_RUNNING_TIME)
                {
                    this.timeRunning += CIV_REACTION_TIME;
                    return false;
                }
                else
                {
                    this.timeRunning = 0;
                    this.fleeing = false;
                }
            }
            return base.doesReact();
        }

    }
}
