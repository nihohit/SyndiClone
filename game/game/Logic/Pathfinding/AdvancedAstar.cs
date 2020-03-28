using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game.Logic.Pathfinding {

  //TODO - support using the same board several times.
  public class AdvancedAStar {

    #region fields

    private const int MIN_DISTANCE = 5;

    private static readonly int TILE_SIZE = (int)FileHandler.GetUintProperty("tile size", FileAccessor.GENERAL);
    private readonly Logic.TerrainGrid m_gridHolder;
    private readonly AStar m_internalAStar;
    private readonly AStar m_internalMinimisedAStar;

    #endregion fields

    #region constructors

    public AdvancedAStar(TerrainGrid gridHolder) {
      m_gridHolder = gridHolder;
      m_internalAStar = new AStar(gridHolder);
      m_internalMinimisedAStar = new AStar(MinimiseGrid(gridHolder));
    }

    protected AdvancedAStar(TerrainGrid gridHolder, AStar grid, AStar minimisedGrid) {
      m_gridHolder = gridHolder;
      m_internalAStar = grid;
      m_internalMinimisedAStar = minimisedGrid;
    }

    #endregion constructors

    #region public methods

    public virtual Task<List<Direction>> FindPathAsync(Point entry, Point goal, Direction originalDirection, AStarConfiguration configuration) {
      return Task<List<Direction>>.Factory.StartNew(() => FindPath(entry, goal, originalDirection, configuration));
    }

    #endregion public methods

    protected List<Direction> FindPath(Point entry, Point goal, Direction originalDirection, AStarConfiguration configuration) {
      //if it's a short route, don't bother with the two tiers.
      if (entry.GetDiffVector(goal).Length() < MIN_DISTANCE * TILE_SIZE)
        return m_internalAStar.FindPath(entry, goal, originalDirection, configuration);

      Point newEntry = new Point(entry.X / TILE_SIZE, entry.Y / TILE_SIZE);
      Point newGoal = new Point(goal.X / TILE_SIZE, goal.Y / TILE_SIZE);
      Vector newSize = new Vector((((configuration.Size.X - 1) / TILE_SIZE) + 1), (((configuration.Size.Y - 1) / TILE_SIZE) + 1)); //this is rounded up

      var midwayConfiguration = new AStarConfiguration(
        newSize, configuration.TraversalMethod, Heuristics.ManhattanMovement(newGoal),
        false, false);
      AstarNode rudamentaryList = m_internalMinimisedAStar.FindPathNoReconstruction(newEntry, newGoal, originalDirection, configuration);
      return AnalyseRudimentaryResults(rudamentaryList, entry, goal, originalDirection, configuration);
    }

    //TODO - paths need smoothing, return to private when done debugging with visual
    protected virtual List<Direction> AnalyseRudimentaryResults(AstarNode node, Point entry, Point goal, Direction originaldirection, AStarConfiguration configuration) {
      List<Direction> newList = new List<Direction>();
      Point begin = goal, end = goal;
      Direction dir;
      bool straightCheck;
      //this is checking the course backwards through the nodes
      while (node != null) {
        straightCheck = true;
        dir = node.Direction;
        for (int j = 0; j < MIN_DISTANCE; j++) {
          node = node.Parent;
          if (node != null) {
            straightCheck = straightCheck && dir == node.Direction;
          } else {
            break;
          }
        }
        if (node != null) {
          if (straightCheck) {
            for (int i = 0; i < MIN_DISTANCE * TILE_SIZE; i++) {
              newList.Insert(0, dir);
            }
            end = new Point(end, Vector.DirectionToVector(dir).Multiply(-MIN_DISTANCE * TILE_SIZE));
          } else {
            dir = node.Direction;
            begin = ConvertToCentralPoint(node.Point);
            newList.InsertRange(0, m_internalAStar.FindPath(begin, end, dir, configuration));
            end = begin;
          }
        }
      }
      newList.InsertRange(0, m_internalAStar.FindPath(entry, end, originaldirection, configuration));
      return newList;
    }

    protected Point ConvertToCentralPoint(Point point) {
      int x = (point.X * TILE_SIZE) + (TILE_SIZE / 2);
      int y = (point.Y * TILE_SIZE) + (TILE_SIZE / 2);
      return new Point(x, y);
    }

    //TODO - can we do a minimisation of just a specific part of the board? will this be quicker? return this to private when done debugging
    protected static TerrainGrid MinimiseGrid(TerrainGrid grid) {
      int xOrg = grid.Grid.GetLength(0);
      int yOrg = grid.Grid.GetLength(1);
      int x = xOrg / TILE_SIZE;
      int y = yOrg / TILE_SIZE;
      TerrainGrid newGrid = new TerrainGrid(x, y);
      for (int i = 0; i < x; i++) {
        for (int j = 0; j < y; j++) {
          newGrid.Grid[i, j] = grid.Grid[i * TILE_SIZE, j * TILE_SIZE];
        }
      }

      return newGrid;
    }
  }
}
