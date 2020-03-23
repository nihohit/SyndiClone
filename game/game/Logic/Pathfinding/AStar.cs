using System;
using System.Collections.Generic;

namespace Game.Logic.Pathfinding {
  #region AStar

  public class AStar {
    //TODO - create updates that will save updating the whole board

    private Logic.TerrainGrid m_gridHolder;

    #region constructor

    public AStar(TerrainGrid gridHolder) {
      m_gridHolder = gridHolder;
    }

    #endregion

    #region public methods

    public virtual List<Direction> FindPath(Point entry, Point goal, Direction originalDirection, AStarConfiguration configuration) {
      AStarInternalState internalState = GenerateInternalState(entry, originalDirection, configuration);

      while (internalState.OpenSet.Count > 0) {
        AstarNode current = internalState.OpenSet.Pop();
        if (current.Point == goal) return ReconstructPath(current, internalState);
        current.Open = false;
        CheckAllNeighbours(current, internalState);
      }

      throw new Exception("open set empty, route impossible");
    }

    public virtual AstarNode FindPathNoReconstruction(Point entry, Point goal, Direction originalDirection, AStarConfiguration configuration) {
      var internalState = GenerateInternalState(entry, originalDirection, configuration);

      while (internalState.OpenSet.Count > 0) {
        AstarNode current = internalState.OpenSet.Pop();
        if (current.Point == goal) return current;
        current.Open = false;
        CheckAllNeighbours(current, internalState);
      }

      throw new Exception("open set empty, route impossible");
    }

    #endregion

    #region private methods

    //this function checks over all the relevant neighbours of a point.
    /*
    private void checkAllNeighboursReduced(AstarNode current)
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

    private AStarInternalState GenerateInternalState(Point entry, Direction originalDirection, AStarConfiguration configuration) {
      var internalState = new AStarInternalState(configuration);

      internalState.Points.Add(entry, new AstarNode(entry, configuration.Heuristic(entry), originalDirection));
      internalState.OpenSet.Push(internalState.Points[entry]);

      return internalState;
    }

    private void CheckAllNeighbours(AstarNode current, AStarInternalState state) {
      int x = current.Point.X, y;
      bool down = (current.Point.Y < m_gridHolder.Grid.GetLength(1) - 1);
      bool up = (current.Point.Y > 0);
      bool left = (current.Point.X > 0);
      bool right = (current.Point.X < m_gridHolder.Grid.GetLength(0) - 1);
      if (up) {
        y = y = current.Point.Y - 1;
        CheckPoint(new Point(x, y), current, state);
        if (state.Configuration.DiagonalMovement) {
          if (left) CheckPoint(new Point(x - 1, y), current, state);
          if (right) CheckPoint(new Point(x + 1, y), current, state);
        }
      }

      if (down) {
        y = y = current.Point.Y + 1;
        CheckPoint(new Point(x, y), current, state);
        if (state.Configuration.DiagonalMovement) {
          if (left) CheckPoint(new Point(x - 1, y), current, state);
          if (right) CheckPoint(new Point(x + 1, y), current, state);
        }
      }
      y = current.Point.Y;
      if (left) CheckPoint(new Point(x - 1, y), current, state);
      if (right) CheckPoint(new Point(x + 1, y), current, state);
    }

    private void CheckPoint(Point temp, AstarNode current, AStarInternalState state) {
      //TODO - handle changing size with changing direction for things which aren't squares
      state.AmountOfNodesChecked++;
      int g = 0, tempG = 0;

      AstarNode newNode = null;
      double costToMove = current.GValue + MoveCostDirection(current.Direction, current.Point, temp, state);
      if (state.Points.ContainsKey(temp)) {
        newNode = state.Points[temp];
        if (!newNode.Open) return;
        if (costToMove < newNode.GValue) {
          newNode.Parent = current;
          newNode.GValue = costToMove;
          //TODO - find a more elegant way to do 
          state.OpenSet.RemoveLocation(newNode);
          state.OpenSet.Push(newNode);
        }
      } else {
        newNode = new AstarNode(temp, costToMove, g, state.Configuration.Heuristic(temp), current);
        state.Points.Add(temp, newNode);
        Area area = new Area(temp, state.Configuration.Size);
        g = 0;
        foreach (Point point in area.GetPointArea()) {
          tempG = CostOfMovement(point, state);
          if (tempG == -1) {
            newNode.Open = false;
            return;
          }
          g += tempG;
        }
        state.OpenSet.Push(newNode);
      }
    }

    private double MoveCostDirection(Direction direction, Point point, Point temp, AStarInternalState state) {
      Direction newDir = Vector.VectorToDirection(point, temp);
      double check = DirectionalMovementCost(newDir);
      if (state.Configuration.DirectionChangeMatters) check += SwitchDirectionCost(direction, newDir);
      return check;
    }

    private double SwitchDirectionCost(Direction origianlDirection, Direction newDir) {
      if (origianlDirection == newDir) return 0;
      if ((origianlDirection == Direction.UP && (newDir == Direction.UPLEFT || newDir == Direction.UPRIGHT)) ||
        (origianlDirection == Direction.DOWN && (newDir == Direction.DOWNLEFT || newDir == Direction.DOWNRIGHT)) ||
        (origianlDirection == Direction.RIGHT && (newDir == Direction.DOWNRIGHT || newDir == Direction.UPRIGHT)) ||
        (origianlDirection == Direction.LEFT && (newDir == Direction.DOWNLEFT || newDir == Direction.UPLEFT)) ||
        (origianlDirection == Direction.DOWNLEFT && (newDir == Direction.DOWN || newDir == Direction.LEFT)) ||
        (origianlDirection == Direction.DOWNRIGHT && (newDir == Direction.DOWN || newDir == Direction.RIGHT)) ||
        (origianlDirection == Direction.UPLEFT && (newDir == Direction.LEFT || newDir == Direction.UP)) ||
        (origianlDirection == Direction.UPRIGHT && (newDir == Direction.UP || newDir == Direction.RIGHT)))
        return 0.05;
      if ((origianlDirection == Direction.UP && (newDir == Direction.LEFT || newDir == Direction.RIGHT)) ||
        (origianlDirection == Direction.DOWN && (newDir == Direction.LEFT || newDir == Direction.RIGHT)) ||
        (origianlDirection == Direction.RIGHT && (newDir == Direction.DOWN || newDir == Direction.UP)) ||
        (origianlDirection == Direction.LEFT && (newDir == Direction.DOWN || newDir == Direction.UP)) ||
        (origianlDirection == Direction.DOWNLEFT && (newDir == Direction.DOWNRIGHT || newDir == Direction.UPLEFT)) ||
        (origianlDirection == Direction.DOWNRIGHT && (newDir == Direction.DOWNLEFT || newDir == Direction.UPRIGHT)) ||
        (origianlDirection == Direction.UPLEFT && (newDir == Direction.DOWNLEFT || newDir == Direction.UPRIGHT)) ||
        (origianlDirection == Direction.UPRIGHT && (newDir == Direction.UPLEFT || newDir == Direction.DOWNRIGHT)))
        return 0.15;
      return 0.4;
    }

    private double DirectionalMovementCost(Direction newDir) {
      if (newDir == Direction.DOWNLEFT || newDir == Direction.DOWNRIGHT ||
        newDir == Direction.UPLEFT || newDir == Direction.UPRIGHT)
        return Math.Sqrt(2);
      return 1;
    }

    private int CostOfMovement(Point temp, AStarInternalState state) {
      if (state.Configuration.TraversalMethod == MovementType.FLYER) return 1;
      switch (m_gridHolder.Grid[temp.X, temp.Y]) {
      case TerrainType.ROAD:
        return 1;
      case TerrainType.BUILDING:
        if (state.Configuration.TraversalMethod == MovementType.CRUSHER) return 1;
        return -1;
      case TerrainType.WATER:
        if (state.Configuration.TraversalMethod == MovementType.HOVER) return 1;
        return -1;
      default:
        return -1;
      }
    }

    private List<Direction> ReconstructPath(AstarNode current, AStarInternalState state) {
      List<Direction> ans = new List<Direction>();

      while (current != null) {
        ans.Insert(0, current.Direction);
        current = current.Parent;
      }

      return ans;
    }

    #endregion
  }

  #endregion

  #region AStarConfiguration

  public class AStarConfiguration {
    public AStarConfiguration(Vector size, Logic.MovementType traversalMethod, Heuristic heuristic, bool directionChangeMatters, bool diagonalMovement) {
      Size = size;
      TraversalMethod = traversalMethod;
      Heuristic = heuristic;
      DirectionChangeMatters = directionChangeMatters;
      DiagonalMovement = diagonalMovement;
    }

    public Vector Size { get; private set; }
    public Logic.MovementType TraversalMethod { get; private set; }
    public Heuristic Heuristic { get; private set; }
    public bool DirectionChangeMatters { get; private set; }
    public bool DiagonalMovement { get; private set; }
  }

  #endregion

  #region AStarInternalState

  public class AStarInternalState {
    public AStarInternalState(AStarConfiguration configuration) {
      Configuration = configuration;
      AmountOfNodesChecked = 0;
      Points = new Dictionary<Point, AstarNode>();
      OpenSet = new PriorityQueueB<AstarNode>();
    }

    public AStarConfiguration Configuration { get; private set; }
    public Dictionary<Point, AstarNode> Points { get; private set; }
    public PriorityQueueB<AstarNode> OpenSet { get; private set; }
    public int AmountOfNodesChecked { get; set; }
  }

  #endregion
}