using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic.Pathfinding
{
    class AdvancedAstar
    {
        const int MIN_DISTANCE = 4;

        static int TILE_SIZE = (int)FileHandler.getUintProperty("tile size", FileAccessor.GENERAL);
        static private Point entry;
        static private Point goal;
        static private Vector size;
        static private Logic.TerrainGrid gridHolder;
        static private Logic.MovementType traversalMethod;
        static private Heuristic heuristic;
        static private Direction origianlDirection;
        static private System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

        static public List<Direction> findPath(Point _entry, Point _goal, Vector _size, TerrainGrid _grid, Logic.MovementType _traversalMethod, Heuristic _heuristic, Direction dir)
        {
            timer.Start();
            entry = _entry;
            goal = _goal;
            gridHolder = _grid;
            traversalMethod = _traversalMethod;
            heuristic = _heuristic;
            size = _size;
            origianlDirection = dir;

            //if it's a short route, don't bother with the two tiers.
            if (entry.getDiffVector(goal).length() < MIN_DISTANCE * TILE_SIZE) 
                return Astar.findPath(entry, goal, _size, _grid, _traversalMethod, _heuristic, dir);

            Logic.TerrainGrid newGrid = minimiseGrid(_grid);
            Point newEntry = new Point(entry.X / TILE_SIZE, entry.Y / TILE_SIZE);
            Point newGoal = new Point(goal.X / TILE_SIZE, goal.Y / TILE_SIZE);
            Vector newSize = new Vector((((_size.X - 1) / TILE_SIZE) + 1), (((_size.Y -1) / TILE_SIZE) +1 )); //this is rounded up

            Astar.DirectionChangeMatters = false;
            Astar.DiagonalMovement = false;
            AstarNode rudamentaryList = Astar.findPathNoReconstruction(newEntry, newGoal, newSize, newGrid, _traversalMethod, Heuristics.ManhattanMovement(newGoal), dir);
            Astar.DirectionChangeMatters = true;
            Astar.DiagonalMovement = true;
            return analyseRudimentaryResults(rudamentaryList);
        }

        //paths need smoothing
        private static List<Direction> analyseRudimentaryResults(AstarNode node)
        {
            List<Direction> newList = new List<Direction>();
            Point begin = goal, end = goal;
            Direction dir;
            bool straightCheck;
            //this is checking the course backwards through the nodes
            while(node != null)
            {
                straightCheck = true;
                dir = node.Direction;
                for (int j = 0; j < MIN_DISTANCE ; j++)
                {
                    node = node.Parent;
                    if (node != null)
                    {
                        straightCheck = straightCheck && dir == node.Direction;
                    }
                    else
                    {
                        break;
                    }
                }
                if (node != null)
                {
                    if (straightCheck)
                    {
                        for (int i = 0; i < MIN_DISTANCE * TILE_SIZE; i++)
                        {
                            newList.Insert(0, dir);
                        }
                        end = new Point(end, Vector.directionToVector(dir).multiply(-MIN_DISTANCE * TILE_SIZE));
                    }
                    else
                    {
                        dir = node.Direction;
                        begin = convertToCentralPoint(node.Point);
                        newList.InsertRange(0, Astar.findPath(begin, end, size, gridHolder, traversalMethod, heuristic, dir));
                        end = begin;
                    }
                }
            }
            newList.InsertRange(0,Astar.findPath(entry, end, size, gridHolder, traversalMethod, heuristic, origianlDirection));
            timer.Stop();
            Console.Out.WriteLine("for distance ," + entry.getDiffVector(goal).length() + ", time was ," + timer.ElapsedTicks + ", and length was ," + newList.Count);
            return newList;
        }

        private static Point convertToCentralPoint(Point point)
        {
            int x = (point.X * TILE_SIZE) + (TILE_SIZE / 2);
            int y = (point.Y * TILE_SIZE) + (TILE_SIZE / 2);
            return new Point(x, y);
        }

        private static TerrainGrid minimiseGrid(TerrainGrid _grid)
        {
            int xOrg =  _grid.Grid.GetLength(0);
            int yOrg = _grid.Grid.GetLength(1);
            int x = xOrg / TILE_SIZE;
            int y = yOrg / TILE_SIZE;
            TerrainGrid newGrid = new TerrainGrid(x,y);
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    newGrid.Grid[i,j] = _grid.Grid[i*TILE_SIZE, j*TILE_SIZE];
                }
            }

            return newGrid;
        }

    }
}
