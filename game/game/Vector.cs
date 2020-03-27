using System;
using Vector2f = SFML.System.Vector2f;

namespace Game {
  #region Vector

  public struct Vector {
    private static Random s_staticRandom = new Random();
    private readonly int m_x, m_y;

    #region constructors

    public Vector(int x, int y) {
      m_x = x;
      m_y = y;
    }

    public Vector(Point a, Point b) {
      m_x = a.X - b.X;
      m_y = a.Y - b.Y;
    }

    public Vector(Point a) {
      m_x = a.X;
      m_y = a.Y;
    }

    #endregion

    #region properties

    public int X { get { return m_x; } }

    public int Y { get { return m_y; } }

    #endregion

    #region conversion to other datatypes

    public Point ToPoint() {
      return new Point(m_x, m_y);
    }

    public Vector2f ToVector2f() {
      return new Vector2f(Convert.ToSingle(m_x), Convert.ToSingle(m_y));
    }

    //Always presume that the vector is new point - old point;
    public Game.Logic.Direction VectorToDirection() {
      if (X > 0) {
        if (Y > 0) return Game.Logic.Direction.DOWNRIGHT;
        if (Y < 0) return Game.Logic.Direction.UPRIGHT;
        return Logic.Direction.RIGHT;
      }
      if (X < 0) {
        if (Y > 0) return Game.Logic.Direction.DOWNLEFT;
        if (Y < 0) return Game.Logic.Direction.UPLEFT;
        return Logic.Direction.LEFT;
      }
      if (Y > 0) return Game.Logic.Direction.DOWN;
      if (Y < 0) return Game.Logic.Direction.UP;
      throw new Exception("same points");
    }

    public static Game.Logic.Direction VectorToDirection(Point point, Point temp) {
      int x = point.X - temp.X;
      int y = point.Y - temp.Y;
      if (x > 0) {
        if (y > 0) return Game.Logic.Direction.DOWNRIGHT;
        if (y < 0) return Game.Logic.Direction.UPRIGHT;
        return Logic.Direction.RIGHT;
      }
      if (x < 0) {
        if (y > 0) return Game.Logic.Direction.DOWNLEFT;
        if (y < 0) return Game.Logic.Direction.UPLEFT;
        return Logic.Direction.LEFT;
      }
      if (y > 0) return Game.Logic.Direction.DOWN;
      if (y < 0) return Game.Logic.Direction.UP;
      throw new Exception("same points");
    }

    static public Vector DirectionToVector(Game.Logic.Direction direction) {
      switch (direction) {
      case (Game.Logic.Direction.UP):
        return new Vector(0, -1);

      case (Game.Logic.Direction.DOWN):
        return new Vector(0, 1);

      case (Game.Logic.Direction.LEFT):
        return new Vector(-1, 0);

      case (Game.Logic.Direction.RIGHT):
        return new Vector(1, 0);

      case (Game.Logic.Direction.UPRIGHT):
        return new Vector(1, -1);

      case (Game.Logic.Direction.DOWNRIGHT):
        return new Vector(1, 1);

      case (Game.Logic.Direction.UPLEFT):
        return new Vector(-1, -1);

      case (Game.Logic.Direction.DOWNLEFT):
        return new Vector(-1, 1);

      default:
        throw new Exception("not valid direction found");
      }
    }

    #endregion

    #region calculations

    public Vector Flip() {
      int x = m_y;
      int y = m_x;
      return new Vector(x, y);
    }

    public Vector AddVector(Vector add) {
      return new Vector(m_x + add.X, m_y + add.Y);
    }

    public Vector normalProbability(double deviation) {
      int x = ComputeNormalProbablity(m_x, deviation);
      int y = ComputeNormalProbablity(m_y, deviation);
      return new Vector(x, y);
    }

    public double Length() {
      return Math.Sqrt(Math.Pow(m_x, 2) + Math.Pow(m_y, 2));
    }

    private int ComputeNormalProbablity(double mean, double deviation) {
      double u1 = s_staticRandom.NextDouble(); //these are uniform(0,1) random doubles
      double u2 = s_staticRandom.NextDouble();
      double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
      double randNormal = mean + deviation * randStdNormal; //random normal(mean,stdDev^2)
      return Convert.ToInt16(randNormal);
    }

    public Vector CompleteToDistance(int dist) {
      int total = Convert.ToInt16(Length());
      dist = dist - total;
      total = m_x + m_y;
      if (dist > 0 && total > 0) {
        int x = dist * m_x / total + m_x;
        int y = dist * m_y / total + m_y;
        return new Vector(x, y);
      }
      return this;
    }

    public Vector Normalise() {
      int x = 0, y = 0;
      if (m_x != 0) x = m_x / System.Math.Abs(m_x);
      if (m_y != 0) y = m_y / System.Math.Abs(m_y);
      return new Vector(x, y);
    }

    public Vector Multiply(Vector size) {
      return new Vector(m_x * size.X, m_y * size.Y);
    }

    public Vector Multiply(int size) {
      return new Vector(m_x * size, m_y * size);
    }

    #endregion

    #region standard public methods

    public override bool Equals(object obj) {
      if (!(obj is Vector)) { return false; } else { return ((m_x == ((Vector) obj).X) && (m_y == ((Vector) obj).Y)); }
    }

    public override int GetHashCode() {
      return m_x.GetHashCode() + m_y.GetHashCode();
    }

    public override string ToString() {
      return "Vector " + X + " , " + Y;
    }

    #endregion
  }

  #endregion

  #region Area

  public struct Area {
    private readonly Point m_entry; //the top left of the shape
    private readonly Vector m_size;

    #region constructors

    public Area(Point entry, Vector size) {
      m_entry = entry;
      m_size = size;
    }

    public Area(Area location, Vector vector) {
      m_entry = new Point(location.Entry, vector);
      m_size = location.Size;
    }

    #endregion

    #region properties 

    public Vector Size { get { return m_size; } }

    public Point Entry { get { return m_entry; } }

    #endregion

    #region public methods

    public Area Flip() {
      return new Area(m_entry, m_size.Flip());
    }

    public Point[, ] GetPointArea() {
      Point[, ] area = new Point[m_size.X, m_size.Y];

      for (int i = 0; i < m_size.X; i++) {
        for (int j = 0; j < m_size.Y; j++) {
          area[i, j] = new Point(m_entry, new Vector(i, j));
        }
      }

      return area;
    }

    public override string ToString() {
      return "Area: entry - " + m_entry + " size - " + m_size;
    }

    #endregion
  }

  #endregion
}