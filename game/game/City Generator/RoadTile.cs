using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * RoadTile is one of the possible tiles in the initial grid. 
 * it holds the road-tile, the direction in which the road is going, the roads depth
 * */
namespace Game.City_Generator
{
    class RoadTile : Tile
    {
        #region constructor 

        public RoadTile() : base(ContentType.ROAD)
        {
            //_loc = new Point(x, y);
            HOffset = 0;
            VOffset = 0;
            HDepth = 1;
            VDepth = 1;
        }

        #endregion

        #region properties

        public int HDepth { get; set; }

        public int VDepth { get; set; }

        public int HOffset { get; set; }

        public int VOffset { get; set; }

        public int Exits { get; set; }

        #endregion

        #region public methods

        /**
         * NOTE! this function assumes that calling is done always in the following order: N,W,S,E.
         * any directions not called in between are assumed to be roadless!
         * */
        public void AddExit(Directions dir) {
            switch (dir){
                case Directions.NORTH: TileImage = Images.R_DEAD_END; Rotate = 0; break; //road to EAST

                case Directions.WEST: //Either W or NW
                    if (Exits == 1)
                    {
                        TileImage = Images.R_CORNER;
                        Rotate = 1;
                    }
                    else {
                        TileImage = Images.R_DEAD_END;
                        Rotate = 1;
                    }
                    break;

                case Directions.SOUTH:
                    if (Exits == 2)
                    {
                        TileImage = Images.R_3WAY;
                        Rotate = 0;
                    }
                    else if (Exits == 1)
                    {
                        if (Rotate == 0)
                            TileImage = Images.R_LINE;
                        else
                        {
                            TileImage = Images.R_CORNER;
                            Rotate = 0;
                        }
                    }
                    else {
                        TileImage = Images.R_DEAD_END;
                        Rotate = 2;
                    }
                    break;

                case Directions.EAST:
                    if (Exits == 3)
                    {
                        TileImage = Images.R_FOURWAY;
                        Rotate = 0;
                    }
                    else if (Exits == 2)
                    {
                        if (TileImage == Images.R_LINE)
                            Rotate = 2;
                        else
                            if (Rotate == 0)
                                Rotate = 3;
                        TileImage = Images.R_3WAY;
                    }
                    else if (Exits == 1)
                    {
                        if (Rotate == 1)
                            TileImage = Images.R_LINE;
                        else
                        {
                            TileImage = Images.R_CORNER;
                            if (Rotate == 0)
                                Rotate = 2;
                            else if (Rotate == 2)
                                Rotate = 3;
                        }
                    }
                    else// one dead-end road
                    {
                        TileImage = Images.R_DEAD_END;
                        Rotate = 3;
                    }
                    break;
            }

            Exits++;
        }

        #endregion
    }
}
