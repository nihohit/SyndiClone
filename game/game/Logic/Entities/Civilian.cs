using System.Collections.Generic;
using Game.Logic;


namespace Game.Logic.Entities
{
    class Civilian : Person
    {

        const int CIV_HEALTH = 3;
        const int CIV_SPEED = 10;
        private bool newPathFlag;

        internal Civilian() : 
            base(CIV_HEALTH, Affiliation.INDEPENDENT, Sight.instance(SightType.CIV_SIGHT), CIV_SPEED, new LinkedList<Point>())
        {
            newPathFlag = true;
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

        internal override Point move()
        {
            if (this.Path.Count < 2)
            {
                newPathFlag = true;
            }
            return base.move();
        }
    }
}
