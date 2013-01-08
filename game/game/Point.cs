using System;


namespace Game
{
    public struct Point
    {
        static Random staticRandom = new Random();
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
            _xLoc = staticRandom.Next(minX, maxX);
            _yLoc = staticRandom.Next(minY, maxY);
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

        public SFML.Window.Vector2f toVector2f()
        {
            return new SFML.Window.Vector2f(Convert.ToSingle(this._xLoc), Convert.ToSingle(this._yLoc));
        }

        public override int GetHashCode()
        {
            return this._xLoc.GetHashCode() + this._yLoc.GetHashCode();
        }
    }

    public struct SmallPoint
    {

        private readonly ushort _xLoc, _yLoc;

        public SmallPoint(ushort x, ushort y)
        {
            this._xLoc = x;
            this._yLoc = y;
        }

        public ushort Y
        {
            get { return _yLoc; }
        }

        public ushort X
        {
            get { return _xLoc; }
        }
    }
}
