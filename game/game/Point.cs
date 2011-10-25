using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    class Point
    {
        private readonly int _xLoc, _yLoc;

        public int Y
        {
            get { return _yLoc; }
        } 


        public int X
        {
            get { return _xLoc; }
        } 


        /** 
         * This constructor creates a point with x,y parameters
         */
        internal Point(int x, int y)
        {
            _xLoc = x;
            _yLoc = y;
        }

        internal Point(Point origin, Vector transfer)
        {
            this._xLoc = origin.X + transfer.X;
            this._yLoc = origin.Y + transfer.Y;

        }

        /** 
         * this constructor gives the point a random value in the range minX-maxX and minY-maxY
         */
        internal Point(int minX, int maxX, int minY, int maxY)
        {
            Random rand = new Random();
            _xLoc = rand.Next(minX, maxX);
            _yLoc = rand.Next(minY, maxY);
        }

        public override String ToString()
        {
            return "(" + _xLoc + "," + _yLoc + ")";
        }


    }
}
