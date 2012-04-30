using System;


namespace Game
{
    public struct Point
    {
        static Random rand = new Random();
        
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
            _xLoc = rand.Next(minX, maxX);
            _yLoc = rand.Next(minY, maxY);
        }

        public override String ToString()
        {
            return "(" + _xLoc + "," + _yLoc + ")";
        }

        internal Vector getDiffVector(Point point)
        {
            return new Vector(this.X - point.X, this.Y - point.Y);
        }

        public static bool operator == (Point a, Point b)
        {
            return (a.X == b.X && (a.Y == b.Y));
        }

        public static bool operator !=(Point a, Point b)
        {
            return (a.X != b.X || (a.Y != b.Y));
        }

        public override bool Equals(object obj)
        {
            return ((obj is Point) && ((Point)obj == this));
        }

        public override int GetHashCode()
        {
            return this._xLoc.GetHashCode() + this._yLoc.GetHashCode();
        }
    }
}
