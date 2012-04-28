using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic.Pathfinding

{
    class PathFinding
    {
        delegate List<Direction> pathfindFunction(Point exit, Point Goal, Point[,] grid);
        delegate double heuristic(Point exit, Point Goal);

        static Dictionary<Point, double> f_value = new Dictionary<Point, double>();
        static Dictionary<Point, double> g_value = new Dictionary<Point, double>();
        static Dictionary<Point, double> h_value = new Dictionary<Point, double>();
        /*
        static pathfindFunction aStarWithHeuristic(heuristic h)
        {
            return delegate (Point exit, Point goal, Point[,] grid)
            {
                double f = h(exit,goal);
                g_value.Add(exit, 0);
                h_value.Add(exit, f);
                f_value.Add(exit, f);
                Heap heap = new Heap();

            }
        }*/
        //TODO - I could just keep the array of points. 
        public static LinkedList<Game.Logic.Direction> convertToDirection(List<PathFinderNode> path)
        {
            LinkedList<Game.Logic.Direction> newPath = new LinkedList<Direction>();
            if (path != null)
            {
                foreach (PathFinderNode node in path)
                {
                    switch (node.PX - node.X)
                    {
                        case (0):
                            if (node.PY - node.Y == 1) newPath.AddLast(Direction.LEFT);
                            else if (node.PY - node.Y == -1) newPath.AddLast(Direction.RIGHT);
                            break;
                        case (1):
                            if (node.PY - node.Y == 1) newPath.AddLast(Direction.DOWNLEFT);
                            else if (node.PY - node.Y == -1) newPath.AddLast(Direction.DOWNRIGHT);
                            else if (node.PY - node.Y == 0) newPath.AddLast(Direction.DOWN);
                            break;
                        case (-1):
                            if (node.PY - node.Y == 1) newPath.AddLast(Direction.UPLEFT);
                            else if (node.PY - node.Y == -1) newPath.AddLast(Direction.UPRIGHT);
                            else if (node.PY - node.Y == 0) newPath.AddLast(Direction.UP);
                            break;
                        default: throw new Exception();
                    }
                }
            }
            return newPath;
        }
    }
}
