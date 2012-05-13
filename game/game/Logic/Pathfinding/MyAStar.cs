using System;
using System.Collections.Generic;

namespace Game.Logic.Pathfinding

{
    static class Astar
    {
        
        static private readonly Dictionary<Point, AstarNode> points = new Dictionary<Point, AstarNode>();
        static private readonly PriorityQueueB<AstarNode> openSet = new PriorityQueueB<AstarNode>();
        //static private readonly HashSet<AstarNode> closedSet = new HashSet<AstarNode>();
        static private Logic.TerrainGrid gridHolder;
        static private Logic.MovementType traversalMethod;
        static private Heuristic heuristic;
        static private Vector size;
        static private System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
        static private bool directionCritical = true;
        static private bool diagonalMovement = true;
        static int amountOfNodesChecked;

        static public List<Direction> findPath(Point entry, Point goal, Vector _size, Logic.TerrainGrid _grid, Logic.MovementType _traversalMethod, Heuristic _heuristic, Direction dir)
        {
            timer.Start();
            amountOfNodesChecked = 0;
            //entry = new Point(entry, Vector.directionToVector(dir).multiply(_size));
            size = _size;
            heuristic = _heuristic;
            gridHolder = _grid;
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

        static public AstarNode findPathNoReconstruction(Point entry, Point goal, Vector _size, Logic.TerrainGrid _grid, Logic.MovementType _traversalMethod, Heuristic _heuristic, Direction dir)
        {
            timer.Start();
            amountOfNodesChecked = 0;
            //entry = new Point(entry, Vector.directionToVector(dir).multiply(_size));
            size = _size;
            heuristic = _heuristic;
            gridHolder = _grid;
            traversalMethod = _traversalMethod;
            points.Add(entry, new AstarNode(entry, heuristic(entry), dir));
            openSet.Push(points[entry]);

            while (openSet.Count > 0)
            {
                AstarNode current = openSet.Pop();
                if (current.Point == goal) return current;
                current.Open = false;
                checkAllNeighbours(current);
            }

            throw new Exception("open set empty, route impossible");
        }

        //this function checks over all the relevant neighbours of a point.
        /*
        private static void checkAllNeighboursReduced(AstarNode current)
        {
            int x = current.Point.X, y = current.Point.Y;
            bool down = (current.Point.Y < gridHolder.Grid.GetLength(1) - 1);
            bool up = (current.Point.Y > 0);
            bool left = (current.Point.X > 0);
            bool right = (current.Point.X < gridHolder.Grid.GetLength(0) - 1);
            switch (current.Direction)
            {
                case(Game.Logic.Direction.DOWN):
                    if (down )
                    {
                        y = current.Point.Y + 1;
                        checkPoint(new Point(x, y), current);
                        if (left ) checkPoint(new Point(x - 1, y), current);
                        if (right) checkPoint(new Point(x + 1, y), current);
                    }
                    break;

                case (Game.Logic.Direction.UP):
                    if (up)
                    {
                        y = current.Point.Y - 1;
                        checkPoint(new Point(x, y), current);
                        if (left ) checkPoint(new Point(x - 1, y), current);
                        if (right)checkPoint(new Point(x + 1, y), current);
                    }
                    break;
                case (Game.Logic.Direction.LEFT):
                    if (left)
                    {
                        x = current.Point.X - 1;
                        checkPoint(new Point(x, y), current);
                        if (up)checkPoint(new Point(x, y - 1), current);
                        if (down)checkPoint(new Point(x, y + 1), current);
                    }
                    break;

                case (Game.Logic.Direction.RIGHT):
                    if (right)
                    {
                        x = current.Point.X + 1;
                        checkPoint(new Point(x, y), current);
                        if (up) checkPoint(new Point(x, y - 1), current);
                        if (down) checkPoint(new Point(x, y + 1), current);
                    }
                    break;

                case (Game.Logic.Direction.DOWNLEFT):
                    if (left) checkPoint(new Point(x - 1, y), current);
                    if (down) checkPoint(new Point(x, y + 1), current);
                    if (down && left) checkPoint(new Point(x - 1, y + 1), current);
                    if (up && left) checkPoint(new Point(x - 1, y - 1), current);
                    if (down && right) checkPoint(new Point(x + 1, y + 1), current);
                    break;

                case (Game.Logic.Direction.DOWNRIGHT):
                    if (right) checkPoint(new Point(x + 1, y), current);
                    if (down) checkPoint(new Point(x, y + 1), current);
                    if (down && right) checkPoint(new Point(x + 1, y + 1), current);
                    if (up && right) checkPoint(new Point(x + 1, y - 1), current);
                    if (down && left) checkPoint(new Point(x - 1, y + 1), current);
                    break;

                case (Game.Logic.Direction.UPLEFT):
                    if (left) checkPoint(new Point(x - 1, y), current);
                    if (up) checkPoint(new Point(x, y - 1), current);
                    if (up && left) checkPoint(new Point(x - 1, y - 1), current);
                    if (down && left) checkPoint(new Point(x - 1, y + 1), current);
                    if (up && right) checkPoint(new Point(x + 1, y - 1), current);

                    break;

                case (Game.Logic.Direction.UPRIGHT):
                    if (right) checkPoint(new Point(x + 1, y), current);
                    if (up) checkPoint(new Point(x, y - 1), current);
                    if (up && right) checkPoint(new Point(x + 1, y - 1), current);
                    if (up && left) checkPoint(new Point(x - 1, y - 1), current);
                    if (down && right) checkPoint(new Point(x + 1, y + 1), current);
                    break;
            }
        }*/
        
        private static void checkAllNeighbours(AstarNode current)
        {

            int x = current.Point.X, y;
            bool down = (current.Point.Y < gridHolder.Grid.GetLength(1) - 1);
            bool up = (current.Point.Y > 0);
            bool left = (current.Point.X > 0);
            bool right = (current.Point.X < gridHolder.Grid.GetLength(0) - 1);
            if (up)
            {
                y = y = current.Point.Y - 1;
                checkPoint(new Point(x, y), current);
                if (diagonalMovement)
                {
                    if (left) checkPoint(new Point(x - 1, y), current);
                    if (right) checkPoint(new Point(x + 1, y), current);
                }
            }

            if (down)
            {
                y = y = current.Point.Y + 1;
                checkPoint(new Point(x, y), current);
                if (diagonalMovement)
                {
                    if (left) checkPoint(new Point(x - 1, y), current);
                    if (right) checkPoint(new Point(x + 1, y), current);
                }
            }
            y= current.Point.Y;
            if(left) checkPoint(new Point(x -1 , y), current);
            if (right) checkPoint(new Point(x + 1, y), current);
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
            double check = directionalMovementCost(newDir);
            if (directionCritical) check += switchDirectionCost(direction, newDir);
            return check;
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
            switch (gridHolder.Grid[temp.X, temp.Y])
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
            //Console.Out.WriteLine("amount of nodes checked " + amountOfNodesChecked + " amount of time elapsed " + timer.ElapsedTicks);
            timer.Reset();
            List<Direction> ans = new List<Direction>();

            while(current!= null)
            {
                ans.Insert(0,current.Direction);
                current = current.Parent;
            }

            return ans;
        }

        public static bool DirectionChangeMatters
        {
            get { return Astar.directionCritical; }
            set { Astar.directionCritical = value; }
        }

        public static bool DiagonalMovement
        {
            get { return Astar.diagonalMovement; }
            set { Astar.diagonalMovement = value; }
        }

    }
}
