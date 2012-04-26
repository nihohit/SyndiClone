using System;

namespace Game
{
    struct Vector
    {
        private int _x, _y;

        static private Random rand = new Random();

        public int Y
        {
            get { return _y; }
        }

        public Vector addVector(Vector add)
        {
            return new Vector(this._x + add.X, this._y + add.Y);
        }

        public void normalProbability(double deviation)
        {
            this._x = computeNormalProbablity(this._x, deviation);
            this._y = computeNormalProbablity(this._y, deviation);
        }

        public double length()
        {
            return Math.Sqrt(Math.Pow(this._x, 2) + Math.Pow(this._y, 2));
        }

        private int computeNormalProbablity(double mean, double deviation)
        {
            double u1 = rand.NextDouble(); //these are uniform(0,1) random doubles
            double u2 = rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal = mean + deviation * randStdNormal; //random normal(mean,stdDev^2)
            return Convert.ToInt16(randNormal);
        }

        public int X
        {
            get { return _x; }
        } 

        public Vector (int x, int y){
            this._x = x;
            this._y = y;
        }

        public Vector(Point b, Point a)
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
            int total = Convert.ToInt16(this.length());
            dist = dist - total;
            total = _x + _y;
            if (dist > 0 && total > 0)
            {
                _x += dist*_x / total;
                _y += dist*_y / total;
            }
        }

        public static int abs(int a)
        {
            if (a > 0) return a; else return -a;
        }

        public Point toPoint()
        {
            return new Point(this._x, this._y);
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
