using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic.Pathfinding
{
    #region delegates

    delegate List<Direction> pathfindFunction(Point entry, Point goal, Vector size, TerrainGrid grid, Logic.MovementType traversalMethod, Heuristic heuristic, Direction dir);
    public delegate double Heuristic(Point check);

    #endregion

    #region AstarNode

    public class AstarNode : IComparable<AstarNode>
    {
        private readonly int m_gTotalValue; //the value of the size-portion, for clearance
        private double m_gValue;
        private AstarNode m_parent;

        #region constructors

        public AstarNode(Point value, double g, int g_total, double h, AstarNode parent)
        {
            Point = value;
            m_gTotalValue = g_total;
            GValue = g;
            FValue = GValue + h + g_total;
            HValue = h;
            Parent = parent;
            Direction = Vector.VectorToDirection(Point, Parent.Point);
            Open = true;
        }

        //only first node uses this
        public AstarNode(Point value, double h, Direction dir)
        {
            Point = value;
            m_gTotalValue = 0;
            m_gValue = 0;
            FValue = GValue + h;
            HValue = h;
            m_parent = null;
            Direction = dir;
            Open = true;
        }

        #endregion

        #region properties

        public AstarNode Parent {
            get { return m_parent; }
            set { m_parent = value; Direction = Vector.VectorToDirection(Point, m_parent.Point); } 
        }

        public Point Point { get; private set; }

        public bool Open { get; set; }

        public double HValue { get; private set; }

        public Direction Direction { get; set; }

        public double GValue
        {
            get { return m_gValue; }
            set { m_gValue = value; FValue = GValue + HValue + m_gTotalValue; }
        }

        public double FValue { get; private set; }

        #endregion

        #region comparers

        int IComparable<AstarNode>.CompareTo(AstarNode other)
        {
            if (FValue > other.FValue) return 1;
            if (FValue < other.FValue) return -1;
            if (HValue > other.HValue) return 1;
            if (HValue < other.HValue) return -1;
            return 0;
        }

        static int NodeComparer(AstarNode a, AstarNode b)
        {
            if (a.FValue > b.FValue) return 1;
            if (a.FValue < b.FValue) return -1;
            if (a.HValue > b.HValue) return 1;
            if (a.HValue < b.HValue) return -1;
            return 0;
        }

        #endregion
    }

    #endregion

    #region Heuristics

    public class Heuristics
    {
        public static Heuristic DiagonalTo(Point goal)
        {
            return delegate(Point entry)
            {
                int diagonal = Math.Max(Math.Abs(entry.X - goal.X), Math.Abs(entry.Y - goal.Y));
                int straight = Math.Abs(entry.X - goal.X) + Math.Abs(entry.Y - goal.Y);
                return Math.Sqrt(2) * diagonal + straight - (2 * diagonal);
            };
        }

        public static Heuristic ManhattanMovement(Point goal)
        {
            return delegate(Point entry)
            {
                return Math.Abs(goal.X - entry.X) + Math.Abs(goal.Y - entry.Y);
            };
        }

    }

    #endregion
}
