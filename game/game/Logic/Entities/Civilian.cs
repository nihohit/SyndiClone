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

        private bool newPathFlag;
        private bool fleeing;
        private int _tryToMove = 0;
        private int timeRunning = 0;

        /******************
        Constructors
        ****************/

        internal Civilian() :
            base(CIV_REACTION_TIME, civReact, CIV_HEALTH, Affiliation.INDEPENDENT, Sight.instance(SightType.CIV_SIGHT), CIV_SPEED, new LinkedList<Direction>())
        {
            this.newPathFlag = true;
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
            Entity threat = Targeters.threatTargeterHigh(entities);
            Reaction react;

            if (threat == null)
            {
                react = new Reaction(null, Action.IGNORE);
            }

            else
            {
                react = new Reaction(threat, Action.RUN_AWAY_FROM);
            }

            return react;
        }

        internal bool needNewPath()
        {
            return this.newPathFlag;
        }

        internal void getNewPath(LinkedList<Direction> path)
        {
            this.Path = path;
            newPathFlag = false;
        }

        internal override void moveResult(bool result)
        {
            base.moveResult(result);
            if (!result) 
            {
                this._tryToMove++;
            }
            if (this.Path.Count < 2 || this._tryToMove > 1)
            {
                newPathFlag = true;
                this._tryToMove = 0;
            }
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
