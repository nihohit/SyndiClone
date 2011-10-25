using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.City_Generator
{
    class Block
    {
       // Point _corner;
        readonly int _len, _wid,startX,startY;

        public int Length
        {
          get { return _len; }  
        } 


        public int Width
        {
          get { return _wid; }  
        } 


        public int StartY
        {
          get { return startY; }
        }

        public int StartX
        {
          get { return startX; }
        }

        internal Block(int x, int y, int len, int wid) {
          //  _corner = new Point(x,y);
            startX = x;
            startY = y;
            _len = len;
            _wid = wid;
        }

    }
}
