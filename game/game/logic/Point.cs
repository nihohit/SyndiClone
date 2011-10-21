using System;


namespace game.logic
{
    class Point
    {
        private int _xLoc, _yLoc;
        private Entity _content;
        /** 
         * This constructor creates a point with x,y parameters
         */
        internal Point(int x, int y) {
            _xLoc = x;
            _yLoc = y;
            this._content = null;
        }

        internal Point(int x, int y, Entity content)
        {
            this._xLoc = x;
            this._yLoc = y;
            this._content = content;
        }

        public int getX()
        {
            return _xLoc;
        }

        public int getY()
        {
            return _yLoc;
        }

        public void update(int x, int y)
        {
            _xLoc = x;
            _yLoc = y;
        }

        override public String ToString() {
            return "(" + _xLoc + "," + _yLoc + ")";
        }



    }
}
