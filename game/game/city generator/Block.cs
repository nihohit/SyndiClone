using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.City_Generator
{
    class Block
    {
       // Point _corner;
        int _len, _wid,startX,startY;

        internal Block(int x, int y, int len, int wid) {
          //  _corner = new Point(x,y);
            startX = x;
            startY = y;
            _len = len;
            _wid = wid;
        }

      //  internal Point getCorner() { return _corner; }
        internal int getLen() { return _len; }
        internal int getWid() { return _wid; }
        internal int getX() { return startX; }
        internal int getY() { return startY; }
    }
}
