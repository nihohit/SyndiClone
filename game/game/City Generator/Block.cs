using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * Block is a data structure that holds rectangles. it does so by keeping the upper-left corner, a length and a width. 
 * NOTE: when regarding matrixes, remember that X is the second coordinate (matrix[Y,X]).
 * */
namespace Game.City_Generator
{
    class Block
    {
        /********************************fields***************************************/
        private  int _len, _wid,_startX,_startY;

        /********************************Constructor***************************************/
        internal Block(int x, int y, int len, int wid)
        {
            _startX = x;
            _startY = y;
            _len = len;
            _wid = wid;
        }

        /********************************Properties***************************************/
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


        /********************************Methods***************************************/
        //HACK (amit): what is that and why do we use that? 
        public override bool Equals(object obj)
        {
            return ((obj is Block) && (((Block)obj).Length == this.Length) && (((Block)obj).Width == this.Width) && (((Block)obj).StartX == this.StartX) 
                && (((Block)obj).StartY == this.StartY));
        }

    }
}
