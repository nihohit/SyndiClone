using System;

namespace Game
{
    struct Vector
    {
        private int _x, _y;

        public int Y
        {
            get { return _y; }
        }

        public Vector addVector(Vector add)
        {
            return new Vector(this._x + add.X, this._y + add.Y);
        }

        public int X
        {
            get { return _x; }
        } 

        public Vector (int x, int y){
            this._x = x;
            this._y = y;
        }

        public Vector(Point a, Point b)
        {
            this._x = a.X - b.X;
            this._y = a.Y - b.Y;
        }

        public Vector(Point a)
        {
            this._x = a.X;
            this._y = a.Y;
        }

        public void flip()
        {
            int temp = this._x;
            this._x = this._y;
            this._y = temp;
        }

        public void completeToDistance(int dist)
        {
            int total = this.length();
            dist = dist - total;
            if (dist > 0)
            {
                _x += dist*_x / total;
                _y += dist*_y / total;
            }
        }

        public static int abs(int a)
        {
            if (a > 0) return a; else return -a;
        }

        public int length()
        {
            return abs(_x) + abs(_y);
        }


        internal void normalise()
        {
            if(abs(_x) > abs(_y)) {
                _y = 0;
                _x = _x / abs(_x);
            }
            else 
            {
                if (abs(_x) < abs(_y))
                {
                    _x = 0;
                    _y = _y / abs(_y);
                }
                else throw new Exception ("Vector is equal");
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector)) { return false; }
            else { return ((this._x == ((Vector)obj).X) && (this._y == ((Vector)obj).Y)); }
        }

        public override int GetHashCode()
        {
            return _x.GetHashCode()+_y.GetHashCode();
        }

        public override string ToString()
        {
            return "Vector " + this.X + " , " + this.Y;
        }
    }
}
