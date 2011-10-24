using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.City_Generator
{
    enum Directions {NS,EW,FOURWAY, N,S,E,W,NONE} //N,S,E,W are for either 3-way junctions or dead-end roads, in the first case they note the "lone" direction
    class RoadTile : Tile
    {
        Point _loc;
        Directions _dir;
        int _vWidth,_hWidth;
        int _vOffset,_hOffset; //how many steps till getting to either the north or west corner, depending on direction.
        //TODO: decide whether junctions need info about all their directions.
        private int _exitsNum;
       // private int _rotate; //assuming that 0 means exit to west, moving clockwise (north, east, south)
        internal const int WEST = 0, NORTH = 1, EAST = 2, SOUTH = 3;

        public RoadTile() : base()
        {
            Type = ContentType.ROAD;
            //_loc = new Point(x, y);
            _dir = Directions.NONE;
            _hOffset = 0;
            _vOffset = 0;
        }

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

        internal Directions getDirection() {
            return _dir;
        }
        
         
        internal void setDirection(Directions d) {
            _dir = d;
        }

        internal void setHWidth(int width) {
             _hWidth = width;
        }
        internal void setVWidth(int width)
        {
            _vWidth = width;
        }
        internal int getHWidth() { return _hWidth; }
        internal int getVWidth() { return _vWidth; }



        internal void setExits(int exits) {
            _exitsNum = exits;
        }

        internal int getExits() {
            return _exitsNum;
        }

        internal void addExit() {
            _exitsNum++;
        }

        internal void setHOffset(int offset) { _hOffset = offset; }
        internal int getHOffset() { return _hOffset; }
        internal void setVOffset(int offset) { _vOffset = offset; }
        internal int getVOffset() { return _vOffset; }
    }
}
