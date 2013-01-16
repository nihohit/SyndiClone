using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * Building tile is one of the possible tiles in the initial grid. it holds a pointer to the building placed on top of it. 
 * note that the "Building" pointer is part of the general tile. this is done to save the need of explicit casting.
 * */
namespace Game.City_Generator
{
    class BuildingTile:Tile
    {
        /********************************Constructor***************************************/
        public BuildingTile(Building b)
            : base(ContentType.BUILDING, b)
        { }
    }
}
