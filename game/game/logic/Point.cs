
namespace game.logic
{
    class Point
    {

        private int _xLoc, _yLoc;

        /** 
         * This constructor creates a point with x,y parameters
         */
        public Point(int x, int y) {
            _xLoc = x;
            _yLoc = y;
        }

        public int getX()
        {
            return _xLoc;
        }

        public int getY()
        {
            return _yLoc;
        }

        public void add(int x, int y)
        {
            _xLoc += x;
            _yLoc += y;
        }

    }
}
