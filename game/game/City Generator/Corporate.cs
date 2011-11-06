using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.City_Generator
{
    class Corporate
    {
        List<Building> _buildings;
        
        internal Corporate() {
            _buildings = new List<Building>();
        }

        internal void addBuilding (Building b){
            _buildings.Add(b);
        }

        internal void removeBuilding(Building b) {
            _buildings.Remove(b);
        }

        /*HACK (amit):
         * Q: I thought that in order to make the corporates significant, we can decide that no building is usable unless you hold the whole corporate.
         *      this also means that it's relatively easy to harass the other player (which makes defence harder than offence)
         */
        internal bool CanBuild(Building b) { 
            if (b.Corp != this) return false;
            foreach (Building other in _buildings)
                if (b.Owner != other.Owner)
                    return false;
            return true;
        }

    }
}
