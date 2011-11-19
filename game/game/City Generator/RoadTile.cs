using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * RoadTile is one of the possible tiles in the initial grid. 
 * it holds the road-tile, the direction in which the road is going, the roads width
 * */
namespace Game.City_Generator
{
    /********************************Enum ***************************************/
    /*HACK (shachar): please fill this up, because I don't get how your variables work. 
     * when to use each tile:
     * 1-way, regular, east-to-west - 
     * 1-way, regular, north-to-south - 
     * 1-way, turn-to-west, north-to-south - 
     * 1-way, turn-to-east, north-to-south - 
     * 1-way, turn-to-north, east-to-west - 
     * 1-way, turn-to-south, east-to-west - 
     * 1-way cross - 
     * 2-way, northern, east-to-west - 
     * 2-way, southern, east-to-west - 
     * 2-way, western, north-to-south - 
     * 2-way, eastern, north-to-south -
     * 2-way, turn-to-west, north-to-south - 
     * 2-way, turn-to-east, north-to-south - 
     * 2-way, turn-to-north, east-to-west - 
     * 2-way, turn-to-south, east-to-west - 
     * 3-way, middle, east-to-west - 
     * 3-way, middle, north-to-south -
     * 3-way, northern, east-to-west - 
     * 3-way, southern, east-to-west - 
     * 3-way, western, north-to-south - 
     * 3-way, eastern, north-to-south -
     * 3-way, turn-to-west, north-to-south - 
     * 3-way, turn-to-east, north-to-south - 
     * 3-way, turn-to-north, east-to-west - 
     * 3-way, turn-to-south, east-to-west - 
     */
    enum Directions {NS,EW,FOURWAY, N,S,E,W,NONE} //N,S,E,W are for either 3-way junctions or dead-end roads, in the first case they note the "lone" direction

    class RoadTile : Tile
    {
        /********************************Constants***************************************/
        internal const int WEST = 0, NORTH = 1, EAST = 2, SOUTH = 3;

        /********************************Fields***************************************/
        Point _loc;
        Directions _dir;
        int _vWidth,_hWidth;
        int _vOffset,_hOffset; //how many steps till getting to either the north or west corner, depending on direction.
        //TODO: decide whether junctions need info about all their directions.
        private int _exitsNum;
       // private int _rotate; //assuming that 0 means exit to west, moving clockwise (north, east, south)

        /********************************Constructor***************************************/
        public RoadTile() : base()
        {
            _type = ContentType.ROAD;
            //_loc = new Point(x, y);
            _dir = Directions.NONE;
            _hOffset = 0;
            _vOffset = 0;
        }


        /********************************Properties***************************************/
        internal int HWidth {
            set { _hWidth = value; }
            get { return _hWidth; }
        }

        internal int VWidth
        {
            set { _vWidth = value; }
            get { return _vWidth; }
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

        internal Directions Direction
        {
            set { _dir = value; }
            get { return _dir; }

        }
        internal int Exits{
            set { _exitsNum = value; }
            get { return _exitsNum; }
        }

        /********************************Simple Methods***************************************/


        /**
         * this is just an initial direction. It's assumed that after that there will be a pass over the grid to correct the data
         */
        internal void addDirection(bool isVertical)
        {
            if (_dir == Directions.FOURWAY) return;

            if (isVertical)
            {
                if (_dir == Directions.EW)
                    _dir = Directions.FOURWAY;
                else _dir = Directions.NS;
            }
            else {
                if (_dir == Directions.NS)
                    _dir = Directions.FOURWAY;
                else _dir = Directions.EW;
            }
        }

        
         




        internal void addExit() {
            _exitsNum++;
        }

        
    }
}
