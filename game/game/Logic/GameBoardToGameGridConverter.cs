using System;
using Game.City_Generator;

namespace Game.Logic {

  internal static class GameBoardToGameGridConverter {
    private static readonly int TILE_SIZE_CONVERSION = FileHandler.GetIntProperty("tile size", FileAccessor.GENERAL);

    #region public methods

    /*
     * This function converts a gameBaord to a grid. It creates a new grid, and populates it according to the
     * building list in the original board.
     */

    public static Grid ConvertBoard(GameBoard board) {
      int y = board.Length * TILE_SIZE_CONVERSION;
      int x = board.Depth * TILE_SIZE_CONVERSION;

      Grid grid = new Grid(y, x);
      int amountOfPoliceBuildings = Convert.ToInt32(System.Math.Log(board.Buildings.Count, 2));
      int ratio = board.Buildings.Count / amountOfPoliceBuildings;
      int i = 0;

      foreach (Game.City_Generator.Building origin in board.Buildings) {
        Game.Logic.Entities.Building result = null;
        if (i != ratio) {
          result = ConvertToCivilianBuilding(origin);
          i++;
        } else {
          i = 0;
          result = ConvertToPoliceStation(origin);
        }
        Area area = ConvertToArea(origin);
        grid.AddEntity(result, area);
      }

      grid.InitialiseTerrainGrid();
      grid.InitialiseExitPoints();
      //TODO - insert police/other buildings

      return grid;
    }

    /*
     * converts a generation-building to a game-building
     */

    public static Game.Logic.Entities.Building ConvertToCivilianBuilding(Game.City_Generator.Building build) {
      Vector realSize = new Vector(build.Dimensions.Length * TILE_SIZE_CONVERSION, build.Dimensions.Depth * TILE_SIZE_CONVERSION);
      int sizeModifier = build.Dimensions.Depth * build.Dimensions.Length;
      return new Game.Logic.Entities.CivilianBuilding(realSize, sizeModifier, GetExitVector(build));
    }

    private static Entities.Building ConvertToPoliceStation(City_Generator.Building build) {
      Vector realSize = new Vector(build.Dimensions.Length * TILE_SIZE_CONVERSION, build.Dimensions.Depth * TILE_SIZE_CONVERSION);
      int sizeModifier = build.Dimensions.Depth * build.Dimensions.Length;
      return new Game.Logic.Entities.PoliceStation(realSize, sizeModifier, GetExitVector(build));
    }

    private static Vector GetExitVector(City_Generator.Building build) {
      int x = 0, y = 0;
      switch (build.ExitDirection) {
        case (0):
          y = -1;
          break;

        case (1):
          y = 1;
          break;

        case (2):
          x = -1;
          break;

        case (3):
          x = 1;
          break;
      }

      //TODO - something here is wrong, and the default exit locations seem rather random.
      return new Vector(
        (short) (x * build.Dimensions.Length * TILE_SIZE_CONVERSION / 2),
        (short) (y * build.Dimensions.Depth * TILE_SIZE_CONVERSION / 2));
    }

    #endregion public methods

    #region private methods

    /*
     * Finds the area of a generation bulding.
     */

    private static Area ConvertToArea(Game.City_Generator.Building build) {
      return new Area(
        new Point((short) (build.Dimensions.StartY * TILE_SIZE_CONVERSION), (short) (build.Dimensions.StartX * TILE_SIZE_CONVERSION)),
        new Vector((short) (build.Dimensions.Length * TILE_SIZE_CONVERSION), (short) (build.Dimensions.Depth * TILE_SIZE_CONVERSION)));
    }

    #endregion private methods
  }
}
