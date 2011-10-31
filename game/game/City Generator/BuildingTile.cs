using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.City_Generator
{

    class BuildingTile:Tile
    {
        Building _building;
        
        internal BuildingTile(Building b): base(){
            _type = ContentType.BUILDING;
            _building = b;
        }
    }
}
