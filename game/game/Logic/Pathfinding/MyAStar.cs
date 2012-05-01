using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic.Pathfinding

{
    internal class AstarNode : IComparable<AstarNode>
    {
        private readonly Point point;
        private Direction dir;
        private int f_value;
        private int g_value;
        private readonly int g_total_value; //the value of the size-portion, for clearance
        private readonly int h_value;
        private AstarNode parent;

        public AstarNode(Point value, int g, int g_total, int h, AstarNode _parent)
        {
            this.point = value;
            this.g_total_value = g_total;
            this.g_value = g + g_total;
            this.f_value = this.g_value + h;
            this.h_value = h;
            this.parent = _parent;
            this.dir = new Vector(this.point, this.parent.Point).vectorToDirection();
        }

        //only first node uses this
        public AstarNode(Point value, int h, Direction _dir)
        {
            this.point = value;
            this.g_total_value = 0;
            this.g_value = 0;
            this.f_value = this.g_value + h;
            this.h_value = h;
            this.parent = null;
            this.dir = _dir;
        }

        public AstarNode Parent
        {
            get { return parent; }
            set { parent = value; this.dir = new Vector(this.point, parent.point).vectorToDirection(); }
        }

        public Point Point
        {
            get { return point; }
        }

        public int H
        {
            get { return h_value; }
        }

        internal Direction Dir
        {
            get { return dir; }
            set { dir = value; }
        }

        public int G
        {
            get { return g_value; }
            set { g_value = value; f_value = g_value + h_value + g_total_value; }
        }

        public int F
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
    }

    static class Astar
    {
        delegate List<Direction> pathfindFunction(Point entry, Point Goal, Vector size, Logic.TerrainType[,] grid, Logic.MovementType traversalMethod, Heuristic method);
        public delegate int Heuristic(Point check);

        static private readonly Dictionary<Point, AstarNode> points = new Dictionary<Point, AstarNode>();
        static private readonly PriorityQueueB<AstarNode> openSet = new PriorityQueueB<AstarNode>();
        static private readonly HashSet<AstarNode> closedSet = new HashSet<AstarNode>();
        static private Logic.TerrainType[,] grid;
        static private Logic.MovementType traversalMethod;
        static private Heuristic heuristic;
        static Vector size;

        static public LinkedList<Direction> findPath(Point entry, Point goal, Vector _size, Logic.TerrainType[,] _grid, Logic.MovementType _traversalMethod, Heuristic _heuristic, Direction dir)
        {
            size = _size;
            heuristic = _heuristic;
            grid = _grid;
            traversalMethod = _traversalMethod;
            points.Clear();
            openSet.Clear();
            closedSet.Clear();
            points.Add(entry, new AstarNode(entry, heuristic(entry), dir));
            openSet.Push(points[entry]);

            while (openSet.Count > 0)
            {
                AstarNode current = openSet.Pop();
                if (current.Point == goal) return reconstructPath(current);
                closedSet.Add(current);
                checkAllNeighbours(current);
            }

            throw new Exception();

        }

        //this function checks over all the relevant neighbours of a point.
        private static void checkAllNeighbours(AstarNode current)
        {
            int x = 0, topX = grid.GetLength(0)-1, topY = grid.GetLength(1)-1;
            if (current.Point.X > 0)
            {
                x = current.Point.X - 1;

                checkPoint(new Point(x, current.Point.Y), current);

                if (current.Point.Y > 0)
                {
                    checkPoint(new Point(x, current.Point.Y - 1), current);
                }

                if (current.Point.Y < topY)
                {
                    checkPoint(new Point(x, current.Point.Y + 1), current);
                }
            }

            if (current.Point.X < topX)
            {
                x = current.Point.X + 1;

                checkPoint(new Point(x, current.Point.Y), current);

                if (current.Point.Y > 0)
                {
                    checkPoint(new Point(x, current.Point.Y - 1), current);
                }

                if (current.Point.Y < topY)
                {
                    checkPoint(new Point(x, current.Point.Y + 1), current);
                }
            }

            if (current.Point.Y > 0)
            {
                checkPoint(new Point(x, current.Point.Y - 1), current);
            }

            if (current.Point.Y < topY)
            {
                checkPoint(new Point(x, current.Point.Y + 1), current);
            }
        }

        private static void checkPoint(Point temp, AstarNode current)
        {
            //TODO - handle changing size with changing direction
            int g = 0, tempG = 0;
            Area area = new Area(temp, size);
            AstarNode newNode = null;
            if (points.ContainsKey(temp))
            {
                newNode = points[temp];
                if (closedSet.Contains(newNode)) return;
                if (current.G < newNode.G)
                {
                    newNode.Parent = current;
                    newNode.G = g;
                    //TODO - find a more elegant way to do this.
                    openSet.RemoveLocation(newNode);
                    openSet.Push(newNode);
                }
            }

            else
            {
                g = 0;
                foreach (Point point in area.getPointArea())
                {
                    tempG = costOfMovement(temp);
                    if (tempG == -1) return;
                    g += tempG;
                }
                newNode = new AstarNode(temp, current.G, g, heuristic(temp), current);
                openSet.Push(newNode);
                points.Add(temp, newNode);
            }
        }

        private static int costOfMovement(Point temp)
        {
            if (traversalMethod == MovementType.FLYER) return 1;
            switch (grid[temp.X, temp.Y])
            {
                case TerrainType.ROAD:
                    return 1;
                case TerrainType.BUILDING:
                    if (traversalMethod == MovementType.CRUSHER) return 1;
                    return -1;
                case TerrainType.WATER:
                    if (traversalMethod == MovementType.HOVER) return 1;
                    return -1;
                    return 2;
                default:
                    return -1;
            }
        }

        private static LinkedList<Direction> reconstructPath(AstarNode current)
        {
            LinkedList<Direction> ans = new LinkedList<Direction>();

            while(current.Parent != null)
            {
                ans.AddFirst(current.Dir);
                current = current.Parent;
            }

            return ans;
        }

        public static Heuristic diagonalTo(Point goal)
        {
            return delegate(Point entry)
            {
                return Math.Max(Math.Abs(entry.X - goal.X), Math.Abs(entry.Y - goal.Y));
            };
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
}
