using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic.Pathfinding
{
    internal class AstarNode : IComparable<AstarNode>
    {
        private readonly Point point;
        private bool open;
        private Direction dir;
        private double f_value;
        private double g_value;
        private readonly int g_total_value; //the value of the size-portion, for clearance
        private readonly double h_value;
        private AstarNode parent;

        public AstarNode(Point value, double g, int g_total, double h, AstarNode _parent)
        {
            this.point = value;
            this.g_total_value = g_total;
            this.g_value = g;
            this.f_value = this.g_value + h + g_total;
            this.h_value = h;
            this.parent = _parent;
            this.dir = Vector.vectorToDirection(this.point, this.parent.Point);
            this.open = true;
        }

        //only first node uses this
        public AstarNode(Point value, double h, Direction _dir)
        {
            this.point = value;
            this.g_total_value = 0;
            this.g_value = 0;
            this.f_value = this.g_value + h;
            this.h_value = h;
            this.parent = null;
            this.dir = _dir;
            this.open = true;
        }

        public AstarNode Parent
        {
            get { return parent; }
            set { parent = value; this.dir = Vector.vectorToDirection(this.point, parent.point); }
        }

        public Point Point
        {
            get { return point; }
        }

        public bool Open
        {
            get { return open; }
            set { open = value; }
        }

        public double H
        {
            get { return h_value; }
        }

        internal Direction Direction
        {
            get { return dir; }
            set { dir = value; }
        }

        public double G
        {
            get { return g_value; }
            set { g_value = value; f_value = g_value + h_value + g_total_value; }
        }

        public double F
        {
            get { return f_value; }
        }

        int IComparable<AstarNode>.CompareTo(AstarNode other)
        {
            if (this.F > other.F) return 1;
            if (this.F < other.F) return -1;
            if (this.H > other.H) return 1;
            if (this.H < other.H) return -1;
            return 0;
        }

        static int nodeComparer(AstarNode a, AstarNode b)
        {
            if (a.F > b.F) return 1;
            if (a.F < b.F) return -1;
            if (a.H > b.H) return 1;
            if (a.H < b.H) return -1;
            return 0;
        }
    }

    public class Heuristics
    {
        public static Heuristic diagonalTo(Point goal)
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

    delegate List<Direction> pathfindFunction(Point entry, Point goal, Vector _size, TerrainGrid _grid, Logic.MovementType _traversalMethod, Heuristic _heuristic, Direction dir);
    public delegate double Heuristic(Point check);
}
