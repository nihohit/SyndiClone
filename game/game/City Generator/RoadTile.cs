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
    /********************************Enum ***************************************/
    
    enum Directions {N,S,E,W} //just for passing directions.

    class RoadTile : Tile
    {
        /********************************Constants***************************************/
        internal const int WEST = 0, NORTH = 1, EAST = 2, SOUTH = 3;

        /********************************members***************************************/
        //Point _loc;
        int _vDepth,_hDepth;
        int _vOffset,_hOffset; //how many steps till getting to either the north or west corner, depending on direction.
        //TODO: decide whether junctions need info about all their directions.
        private int _exitsNum;
       
       // private int _rotate; //assuming that 0 means exit to west, moving clockwise (north, east, south)

        /********************************Constructor***************************************/
        public RoadTile() : base()
        {
            _type = ContentType.ROAD;
            //_loc = new Point(x, y);
            _hOffset = 0;
            _vOffset = 0;
            _hDepth = 1;
            _vDepth = 1;
        }


        /********************************Properties***************************************/
        internal int HDepth {
            set { _hDepth = value; }
            get { return _hDepth; }
        }

        internal int VDepth
        {
            set { _vDepth = value; }
            get { return _vDepth; }
        }

        internal int HOffset {
            set { _hOffset = value; }
            get { return _hOffset; }
        }

        internal int VOffset
        {
            set { _vOffset = value; }
            get { return _vOffset; }
        }

        
        internal int Exits{
            set { _exitsNum = value; }
            get { return _exitsNum; }
        }

       

        /********************************Simple Methods***************************************/


        /**
         * NOTE! this function assumes that calling is done always in the following order: N,W,S,E.
         * any directions not called in between are assumed to be roadless!
         * */
        internal void addExit(Directions dir) {
            switch (dir){
                case Directions.N: _img = Images.R_DEAD_END; _rotate = 0; break; //road to EAST

                case Directions.W: //Either W or NW
                    if (_exitsNum == 1)
                    {
                        _img = Images.R_CORNER;
                        _rotate = 1;
                    }
                    else {
                        _img = Images.R_DEAD_END;
                        _rotate = 1;
                    }
                        break;

                case Directions.S:
                        if (_exitsNum == 2)
                        {
                            _img = Images.R_3WAY;
                            _rotate = 0;
                        }
                        else if (_exitsNum == 1)
                        {
                            if (_rotate == 0)
                                _img = Images.R_LINE;
                            else
                            {
                                _img = Images.R_CORNER;
                                Rotate = 0;
                            }
                        }
                        else {
                            _img = Images.R_DEAD_END;
                            _rotate = 2;
                        }
                    break;
                case Directions.E:
                    if (_exitsNum == 3)
                    {
                        _img = Images.R_FOURWAY;
                        _rotate = 0;
                    }
                    else if (_exitsNum == 2)
                    {
                        
                        if (_img == Images.R_LINE)
                            _rotate = 2;
                        else
                            if (_rotate == 0)
                                _rotate = 3;
                        _img = Images.R_3WAY;
                    }
                    else if (_exitsNum == 1)
                    {
                        if (_rotate == 1)
                            _img = Images.R_LINE;
                        else
                        {
                            _img = Images.R_CORNER;
                            if (_rotate == 0)
                                _rotate = 2;
                            else if (_rotate == 2)
                                _rotate = 3;
                        }

                    }
                    else// one dead-end road
                    {
                        _img = Images.R_DEAD_END;
                        _rotate = 3;
                    }

      
                    break;
               

            }

            _exitsNum++;
        }

        
    }
}
