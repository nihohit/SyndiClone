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

        public Vector normalProbability(double deviation)
        {
            int x = computeNormalProbablity(this._x, deviation);
            int y = computeNormalProbablity(this._y, deviation);
            return new Vector(x, y);
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

        public Vector flip()
        {
            int x = this._y;
            int y = this._x;
            return new Vector(x, y);
        }

        public Vector completeToDistance(int dist)
        {
            int total = Convert.ToInt16(this.length());
            dist = dist - total;
            total = _x + _y;
            if (dist > 0 && total > 0)
            {
                int x = dist*_x / total + this._x;
                int y = dist*_y / total + this._y;
                return new Vector(x,y);
            }
            return this;
        }

        public static int abs(int a)
        {
            if (a > 0) return a; else return -a;
        }

        public Point toPoint()
        {
            return new Point(this._x, this._y);
        }

        internal Vector normalise()
        {
            int x = 0, y = 0;
            if (_x != 0) x = _x / abs(_x);
            if (_y != 0 ) y = _y / abs(_y);
            return new Vector(x, y);
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

        //Always presume that the vector is new point - old point;
        public Game.Logic.Direction vectorToDirection()
        {
            if (this.X > 0)
            {
                if (this.Y > 0) return Game.Logic.Direction.DOWNRIGHT;
                if (this.Y < 0) return Game.Logic.Direction.UPRIGHT;
                return Logic.Direction.RIGHT;
            }
            if (this.X < 0) 
            {
                if (this.Y > 0) return Game.Logic.Direction.DOWNLEFT;
                if (this.Y < 0) return Game.Logic.Direction.UPLEFT;
                return Logic.Direction.LEFT;
            }
            if (this.Y > 0) return Game.Logic.Direction.DOWN;
            if (this.Y < 0) return Game.Logic.Direction.UP;
            throw new Exception("same points");
        }
    }

    internal struct Area
    {
        private readonly Point _entry; //the top left of the shape
        private Vector _size;

        internal Area flip()
        {
            return new Area(this._entry, this._size.flip());
        }

        internal Area(Point entry, Vector size)
        {
            this._entry = entry;
            this._size = size;
        }

        internal Area(Area location, Vector vector)
        {
            this._entry = new Point(location.Entry, vector);
            this._size = location.Size;
        }

        internal Vector Size
        {
            get { return _size; }
        }

        public Point Entry
        {
            get { return _entry; }
        }

        internal Point[,] getPointArea()
        {
            Point[,] area = new Point[this._size.X, this._size.Y];

            for (int i = 0; i < this._size.X; i++)
            {
                for (int j = 0; j < this._size.Y; j++)
                {
                    area[i, j] = new Point(this._entry, new Vector(i, j));
                }
            }

            return area;
        }

        public override string ToString()
        {
            return "Area: entry - " + this._entry + " size - " + this._size;
        }

    }
}
