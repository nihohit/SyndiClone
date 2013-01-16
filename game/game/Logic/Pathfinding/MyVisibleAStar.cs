using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace Game.Logic.Pathfinding
{
    class MyVisibleAStar
    {
        #region fields

        private readonly Dictionary<Point, AstarNode> s_points = new Dictionary<Point, AstarNode>();
        private readonly PriorityQueueB<AstarNode> s_openSet = new PriorityQueueB<AstarNode>();
        private Logic.TerrainType[,] s_grid;
        private Logic.MovementType s_traversalMethod;
        private Heuristic s_heuristic;
        Vector s_size;
        RenderWindow s_window;
        List<Sprite> s_sprites;
        int s_amountOfNodesChecked;
        Dictionary<String, SFML.Graphics.Texture> s_images = new Dictionary<string, Texture>
        {
            {"white", new Texture("images/debug/white_pixel.png")},
            {"red", new Texture("images/debug/red_pixel.png")},
            {"yellow", new Texture("images/debug/yellow_pixel.png")}
        };

        #endregion

        #region public methods

        public List<Direction> FindPath(Point entry, Point goal, Vector size, Logic.TerrainType[,] grid, Logic.MovementType traversalMethod, Heuristic heuristic, Direction dir)
        {
            s_amountOfNodesChecked = 0;
            entry = new Point(entry, Vector.DirectionToVector(dir).Multiply(size));
            s_size = size;
            s_heuristic = heuristic;
            s_grid = grid;
            s_traversalMethod = traversalMethod;
            s_points.Add(entry, new AstarNode(entry, s_heuristic(entry), dir));
            OpenNode(s_points[entry]);

            while (s_openSet.Count > 0)
            {
                AstarNode current = s_openSet.Pop();
                if (current.Point == goal) return ReconstructPath(current);
                CloseNode(current);
                CheckAllNeighbours(current);
            }

            throw new Exception("open set empty, route impossible");
        }

        public void Setup(RenderWindow win)
        {
            s_window = win;
            Sprite back = new Sprite(new Texture(s_window.Capture()));
            s_sprites = new List<Sprite>();
            s_sprites.Add(back);
        }

        #endregion

        #region private methods

        private void Display()
        {
            foreach (Sprite sprite in s_sprites)
            {
                s_window.Draw(sprite);
            }
            s_window.Display();
            if (s_sprites.Count > 150)
            {
                Sprite temp = new Sprite(new Texture(s_window.Capture()));
                temp.Position = new SFML.Window.Vector2f(-30,-30);
                s_sprites.Clear();
                s_sprites.Add(temp);
            }
        }

        //this function checks over all the relevant neighbours of a point.
        private void CheckAllNeighbours(AstarNode current)
        {

            int x = 0, topX = s_grid.GetLength(0) - 1, topY = s_grid.GetLength(1) - 1;
            if (current.Point.X > 0)
            {
                x = current.Point.X - 1;

                CheckPoint(new Point(x, current.Point.Y), current);

                if (current.Point.Y > 0)
                {
                    CheckPoint(new Point(x, current.Point.Y - 1), current);
                }

                if (current.Point.Y < topY)
                {
                    CheckPoint(new Point(x, current.Point.Y + 1), current);
                }
            }

            if (current.Point.X < topX)
            {
                x = current.Point.X + 1;

                CheckPoint(new Point(x, current.Point.Y), current);

                if (current.Point.Y > 0)
                {
                    CheckPoint(new Point(x, current.Point.Y - 1), current);
                }

                if (current.Point.Y < topY)
                {
                    CheckPoint(new Point(x, current.Point.Y + 1), current);
                }
            }

            x = current.Point.X;

            if (current.Point.Y > 0)
            {
                CheckPoint(new Point(x, current.Point.Y - 1), current);
            }

            if (current.Point.Y < topY)
            {
                CheckPoint(new Point(x, current.Point.Y + 1), current);
            }
        }

        private void CheckPoint(Point temp, AstarNode current)
        {
            //TODO - handle changing size with changing direction
            s_amountOfNodesChecked++;
            int g = 0, tempG = 0;

            AstarNode newNode = null;
            double costToMove = current.GValue + MoveCostDirection(current.Direction, current.Point, temp);
            if (s_points.ContainsKey(temp))
            {
                newNode = s_points[temp];
                if (newNode == null) return;
                if (costToMove < newNode.GValue)
                {
                    newNode.Parent = current;
                    newNode.GValue = costToMove;
                    //TODO - find a more elegant way to do 
                    s_openSet.RemoveLocation(newNode);
                    s_openSet.Push(newNode);
                }
            }
            else
            {
                newNode = new AstarNode(temp, costToMove, g, s_heuristic(temp), current);
                s_points.Add(temp, newNode);
                Area area = new Area(temp, s_size);
                g = 0;
                foreach (Point point in area.GetPointArea())
                {
                    tempG = CostOfMovement(point);
                    if (tempG == -1)
                    {
                        CloseNode(newNode);
                        return;
                    }
                    g += tempG;
                }
                OpenNode(newNode);
            }
        }

        private void OpenNode(AstarNode newNode)
        {
            s_openSet.Push(newNode);
            Sprite update = new Sprite(s_images["yellow"]);
            update.Position = new SFML.Window.Vector2f(newNode.Point.X, newNode.Point.Y);
            s_sprites.Add(update);
            Display();
        }

        private void CloseNode(AstarNode newNode)
        {
            s_points[newNode.Point] = null;
            Sprite update = new Sprite(s_images["red"]);
            update.Position = new SFML.Window.Vector2f(newNode.Point.X, newNode.Point.Y);
            s_sprites.Add(update);
            Display();
        }

        private double MoveCostDirection(Direction direction, Point point, Point temp)
        {
            Direction newDir = Vector.VectorToDirection(point, temp);
            return SwitchDirectionCost(direction, newDir) + DirectionalMovementCost(newDir);
        }

        private double SwitchDirectionCost(Direction origianlDirection, Direction newDir)
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

        private double DirectionalMovementCost(Direction newDir)
        {
            if (newDir == Direction.DOWNLEFT || newDir == Direction.DOWNRIGHT
                || newDir == Direction.UPLEFT || newDir == Direction.UPRIGHT)
                return Math.Sqrt(2);
            return 1;
        }

        private int CostOfMovement(Point temp)
        {
            if (s_traversalMethod == MovementType.FLYER) return 1;
            switch (s_grid[temp.X, temp.Y])
            {
                case TerrainType.ROAD:
                    return 1;
                case TerrainType.BUILDING:
                    if (s_traversalMethod == MovementType.CRUSHER) return 1;
                    return -1;
                case TerrainType.WATER:
                    if (s_traversalMethod == MovementType.HOVER) return 1;
                    return -1;
                default:
                    return -1;
            }
        }

        private List<Direction> ReconstructPath(AstarNode current)
        {
            s_points.Clear();
            s_openSet.Clear();
            //closedSet.Clear();
            Console.Out.WriteLine("amount of nodes checked " + s_amountOfNodesChecked);
            List<Direction> ans = new List<Direction>();

            while (current != null)
            {
                Sprite update = new Sprite(s_images["white"]);
                update.Position = new SFML.Window.Vector2f(current.Point.X, current.Point.Y);
                s_sprites.Add(update);
                Display();
                ans.Insert(0,current.Direction);
                current = current.Parent;

            }
            Console.In.ReadLine();
            return ans;
        }

        public Heuristic DiagonalTo(Point goal)
        {
            return delegate(Point entry)
            {
                int diagonal = Math.Max(Math.Abs(entry.X - goal.X), Math.Abs(entry.Y - goal.Y));
                int straight = Math.Abs(entry.X - goal.X) + Math.Abs(entry.Y - goal.Y);
                return Math.Sqrt(2) * diagonal + straight - (2 * diagonal);
            };
        }

        int NodeComparer(AstarNode a, AstarNode b)
        {
            if (a.FValue > b.FValue) return 1;
            if (a.FValue < b.FValue) return -1;
            if (a.HValue > b.HValue) return 1;
            if (a.HValue < b.HValue) return -1;
            return 0;
        }

        #endregion
    }
}
