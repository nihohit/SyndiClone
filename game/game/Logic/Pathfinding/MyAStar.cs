﻿using System;
using System.Collections.Generic;

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
            this.f_value = this.g_value + h  + g_total;
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
    }

    delegate List<Direction> pathfindFunction(Point entry, Point goal, Vector _size, Logic.TerrainType[,] _grid, Logic.MovementType _traversalMethod, Heuristic _heuristic, Direction dir);
    public delegate double Heuristic(Point check);

    static class Astar
    {
        

        static private readonly Dictionary<Point, AstarNode> points = new Dictionary<Point, AstarNode>();
        static private readonly PriorityQueueB<AstarNode> openSet = new PriorityQueueB<AstarNode>();
        //static private readonly HashSet<AstarNode> closedSet = new HashSet<AstarNode>();
        static private Logic.TerrainType[,] grid;
        static private Logic.MovementType traversalMethod;
        static private Heuristic heuristic;
        static private Vector size;
        static private System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

        static int amountOfNodesChecked;

        static public List<Direction> findPath(Point entry, Point goal, Vector _size, Logic.TerrainType[,] _grid, Logic.MovementType _traversalMethod, Heuristic _heuristic, Direction dir)
        {
            timer.Start();
            amountOfNodesChecked = 0;
            entry = new Point(entry, Vector.directionToVector(dir).multiply(_size));
            size = _size;
            heuristic = _heuristic;
            grid = _grid;
            traversalMethod = _traversalMethod;
            points.Add(entry, new AstarNode(entry, heuristic(entry), dir));
            openSet.Push(points[entry]);

            while (openSet.Count > 0)
            {
                AstarNode current = openSet.Pop();
                if (current.Point == goal) return reconstructPath(current);
                current.Open = false;
                checkAllNeighbours(current);
            }

            throw new Exception("open set empty, route impossible");
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

            x = current.Point.X;

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
            amountOfNodesChecked++;
            int g = 0, tempG = 0;
            
            AstarNode newNode = null;
            double costToMove = current.G + moveCostDirection(current.Direction, current.Point, temp);
            if (points.ContainsKey(temp))
            {
                newNode = points[temp];
                if (!newNode.Open) return;
                if (costToMove < newNode.G) 
                {
                    newNode.Parent = current;
                    newNode.G = costToMove;
                    //TODO - find a more elegant way to do this.
                    openSet.RemoveLocation(newNode);
                    openSet.Push(newNode);
                }
            }
            else
            {
                newNode = new AstarNode(temp, costToMove, g, heuristic(temp), current);
                points.Add(temp, newNode);
                Area area = new Area(temp, size);
                g = 0;
                foreach (Point point in area.getPointArea())
                {
                    tempG = costOfMovement(point);
                    if (tempG == -1)
                    {
                        newNode.Open = false;
                        return;
                    }
                    g += tempG;
                }
                openSet.Push(newNode);
            }
        }

        private static double moveCostDirection(Direction direction, Point point, Point temp)
        {
            Direction newDir = Vector.vectorToDirection(point, temp);
            return switchDirectionCost(direction, newDir) + directionalMovementCost(newDir);
        }

        private static double switchDirectionCost(Direction origianlDirection, Direction newDir)
        {
            if (origianlDirection == newDir) return 0;
            if ((origianlDirection == Direction.UP && (newDir == Direction.UPLEFT || newDir == Direction.UPRIGHT))
                || (origianlDirection == Direction.DOWN && (newDir == Direction.DOWNLEFT || newDir == Direction.DOWNRIGHT))
                || (origianlDirection == Direction.RIGHT && (newDir == Direction.DOWNRIGHT || newDir == Direction.UPRIGHT))
                || (origianlDirection == Direction.LEFT && (newDir == Direction.DOWNLEFT || newDir == Direction.UPLEFT))
                || (origianlDirection == Direction.DOWNLEFT && (newDir == Direction.DOWN || newDir == Direction.LEFT))
                || (origianlDirection == Direction.DOWNRIGHT && (newDir == Direction.DOWN || newDir == Direction.RIGHT))
                || (origianlDirection == Direction.UPLEFT && (newDir == Direction.LEFT || newDir == Direction.UP))
                || (origianlDirection == Direction.UPRIGHT && (newDir == Direction.UP || newDir == Direction.RIGHT)))
                return 0.05;
            if ((origianlDirection == Direction.UP && (newDir == Direction.LEFT || newDir == Direction.RIGHT))
                || (origianlDirection == Direction.DOWN && (newDir == Direction.LEFT || newDir == Direction.RIGHT))
                || (origianlDirection == Direction.RIGHT && (newDir == Direction.DOWN || newDir == Direction.UP))
                || (origianlDirection == Direction.LEFT && (newDir == Direction.DOWN || newDir == Direction.UP))
                || (origianlDirection == Direction.DOWNLEFT && (newDir == Direction.DOWNRIGHT || newDir == Direction.UPLEFT))
                || (origianlDirection == Direction.DOWNRIGHT && (newDir == Direction.DOWNLEFT || newDir == Direction.UPRIGHT))
                || (origianlDirection == Direction.UPLEFT && (newDir == Direction.DOWNLEFT || newDir == Direction.UPRIGHT))
                || (origianlDirection == Direction.UPRIGHT && (newDir == Direction.UPLEFT || newDir == Direction.DOWNRIGHT)))
                return 0.15;
            return 0.4;
        }

        private static double directionalMovementCost(Direction newDir)
        {
            if (newDir == Direction.DOWNLEFT || newDir == Direction.DOWNRIGHT
                || newDir == Direction.UPLEFT || newDir == Direction.UPRIGHT)
                return Math.Sqrt(2);
            return 1;
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
                default:
                    return -1;
            }
        }

        private static List<Direction> reconstructPath(AstarNode current)
        {
            points.Clear();
            openSet.Clear();
            //closedSet.Clear();
            timer.Stop();
            Console.Out.WriteLine("amount of nodes checked " + amountOfNodesChecked + " amount of time elapsed " + timer.ElapsedTicks + " time per point " + timer.ElapsedTicks / amountOfNodesChecked);
            timer.Reset();
            List<Direction> ans = new List<Direction>();

            while(current!= null)
            {
                ans.Insert(0,current.Direction);
                current = current.Parent;

            }

            return ans;
        }

        public static Heuristic diagonalTo(Point goal)
        {
            return delegate(Point entry)
            {
                int diagonal = Math.Max(Math.Abs(entry.X - goal.X), Math.Abs(entry.Y - goal.Y));
                int straight = Math.Abs(entry.X - goal.X) + Math.Abs(entry.Y - goal.Y);
                return Math.Sqrt(2) * diagonal + straight - (2 * diagonal);
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
