using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    class Vector
    {
        private readonly int _x, _y;

        public int Y
        {
            get { return _y; }
        } 


        public int X
        {
            get { return _x; }
        } 

        public Vector (int x, int y){
            this._x = x;
            this._y = y;
        }
    }
}
