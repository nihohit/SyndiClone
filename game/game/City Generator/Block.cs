using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.City_Generator
{
    class Block
    {
       // Point _corner;
        private  int _len, _wid,_startX,_startY;

        public int Length
        {
          get { return _len; }
          set { _len = value; }
        } 


        public int Width
        {
          get { return _wid; }
          set { _wid = value; }
        } 


        public int StartY
        {
          get { return _startY; }
          set { _startY = value; }
        }

        public int StartX
        {
          get { return _startX; }
          set { _startX = value; }
        }

        internal Block(int x, int y, int len, int wid) {
          //  _corner = new Point(x,y);
            _startX = x;
            _startY = y;
            _len = len;
            _wid = wid;
        }

    }
}
