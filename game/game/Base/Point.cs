using System;
using Vector2f = SFML.System.Vector2f;

namespace Game {

  public struct Point {
    private readonly int m_xLoc, m_yLoc;

    public int Y { get { return m_yLoc; } }
    public int X { get { return m_xLoc; } }

    #region constructors

    /**
     * This constructor creates a point with x,y parameters
     */

    public Point(int x, int y) {
      m_xLoc = x;
      m_yLoc = y;
    }

    public Point(Point origin, Vector transfer) {
      m_xLoc = origin.X + transfer.X;
      m_yLoc = origin.Y + transfer.Y;
    }

    /**
     * this constructor gives the point a random value in the range minX-maxX and minY-maxY
     */

    public Point(int minX, int maxX, int minY, int maxY) {
      m_xLoc = Randomizer.Next(minX, maxX);
      m_yLoc = Randomizer.Next(minY, maxY);
    }

    #endregion constructors

    #region public methods

    public override String ToString() {
      return "(" + m_xLoc + "," + m_yLoc + ")";
    }

    public Vector GetDiffVector(Point point) {
      return new Vector(X - point.X, Y - point.Y);
    }

    public override bool Equals(object obj) {
      return ((obj is Point) && ((Point) obj == this));
    }

    public Vector2f ToVector2f() {
      return new Vector2f(Convert.ToSingle(m_xLoc), Convert.ToSingle(m_yLoc));
    }

    public override int GetHashCode() {
      return m_xLoc.GetHashCode() + m_yLoc.GetHashCode();
    }

    public int GetDistance(Point target) {
      return Convert.ToInt32(Math.Sqrt(Math.Pow(X - target.X, 2) + Math.Pow(Y - target.Y, 2)));
    }

    #endregion public methods

    #region operators

    public static bool operator ==(Point a, Point b) {
      return (a.X == b.X && (a.Y == b.Y));
    }

    public static bool operator !=(Point a, Point b) {
      return (a.X != b.X || (a.Y != b.Y));
    }

    #endregion operators
  }
}
