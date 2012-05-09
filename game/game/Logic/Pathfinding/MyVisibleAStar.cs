using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace Game.Logic.Pathfinding
{
    class MyVisibleAStar
    {
        static private readonly Dictionary<Point, AstarNode> points = new Dictionary<Point, AstarNode>();
        static private readonly PriorityQueueB<AstarNode> openSet = new PriorityQueueB<AstarNode>();
        //static private readonly HashSet<AstarNode> closedSet = new HashSet<AstarNode>();
        static private Logic.TerrainType[,] grid;
        static private Logic.MovementType traversalMethod;
        static private Heuristic heuristic;
        static Vector size;
        static RenderWindow window;
        static List<Sprite> sprites;
        static Dictionary<String, SFML.Graphics.Texture> images = new Dictionary<string, Texture>
        {
            {"white", new Texture("images/debug/white_pixel.png")},
            {"red", new Texture("images/debug/red_pixel.png")},
            {"yellow", new Texture("images/debug/yellow_pixel.png")}
        };


        static int amountOfNodesChecked;

        static public List<Direction> findPath(Point entry, Point goal, Vector _size, Logic.TerrainType[,] _grid, Logic.MovementType _traversalMethod, Heuristic _heuristic, Direction dir)
        {
            amountOfNodesChecked = 0;
            entry = new Point(entry, Vector.directionToVector(dir).multiply(_size));
            size = _size;
            heuristic = _heuristic;
            grid = _grid;
            traversalMethod = _traversalMethod;
            points.Add(entry, new AstarNode(entry, heuristic(entry), dir));
            openNode(points[entry]);

            while (openSet.Count > 0)
            {
                AstarNode current = openSet.Pop();
                if (current.Point == goal) return reconstructPath(current);
                closeNode(current);
                checkAllNeighbours(current);
            }

            throw new Exception("open set empty, route impossible");
        }

        public static void setup(RenderWindow _win)
        {
            window = _win;
            Sprite back = new Sprite(new Texture(window.Capture()));
            sprites = new List<Sprite>();
            sprites.Add(back);
        }

        private static void display()
        {
            foreach (Sprite sprite in sprites)
            {
                window.Draw(sprite);
            }
            window.Display();
            if (sprites.Count > 150)
            {
                Sprite temp = new Sprite(new Texture(window.Capture()));
                temp.Position = new SFML.Window.Vector2f(-30,-30);
                sprites.Clear();
                sprites.Add(temp);
            }
        }

        //this function checks over all the relevant neighbours of a point.
        private static void checkAllNeighbours(AstarNode current)
        {

            int x = 0, topX = grid.GetLength(0) - 1, topY = grid.GetLength(1) - 1;
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
                        closeNode(newNode);
                        return;
                    }
                    g += tempG;
                }
                openNode(newNode);
            }
        }

        private static void openNode(AstarNode newNode)
        {
            openSet.Push(newNode);
            Sprite update = new Sprite(images["yellow"]);
            update.Position = new SFML.Window.Vector2f(newNode.Point.X, newNode.Point.Y);
            sprites.Add(update);
            display();
        }

        private static void closeNode(AstarNode newNode)
        {
            newNode.Open = false;
            Sprite update = new Sprite(images["red"]);
            update.Position = new SFML.Window.Vector2f(newNode.Point.X, newNode.Point.Y);
            sprites.Add(update);
            display();
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
            Console.Out.WriteLine("amount of nodes checked " + amountOfNodesChecked);
            List<Direction> ans = new List<Direction>();

            while (current != null)
            {
                Sprite update = new Sprite(images["white"]);
                update.Position = new SFML.Window.Vector2f(current.Point.X, current.Point.Y);
                sprites.Add(update);
                display();
                ans.Insert(0,current.Direction);
                current = current.Parent;

            }
            Console.In.ReadLine();
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
