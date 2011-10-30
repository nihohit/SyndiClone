using System.Collections.Generic;
using Game.Logic;


namespace Game.Logic.Entities
{
    class Civilian : Person
    {

        const int CIV_HEALTH = 3;
        const int CIV_SPEED = 10;
        const int CIV_REACTION = 10;
        private bool newPathFlag;
        private int _tryToMove = 0;

        internal Civilian() : 
            base(CIV_REACTION, civReact, CIV_HEALTH, Affiliation.INDEPENDENT, Sight.instance(SightType.CIV_SIGHT), CIV_SPEED, new LinkedList<Point>())
        {
            newPathFlag = true;
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

        internal void getNewPath(LinkedList<Point> path)
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
    }
}
