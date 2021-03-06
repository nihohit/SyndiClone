/**
 * Currently, city factory knows to give just one type of city.
 * later it might be able to give other types (mazes?)
 * */

namespace Game.City_Generator {

  internal static class CityFactory {

    #region constants

    private const int DEF_LEN = 100;
    private const int DEF_WID = 100; //default city length and depth

    #endregion constants

    #region public methods

    /**
     * a city factory method.
     * */

    public static GameBoard CreateCity(int length = DEF_LEN, int depth = DEF_WID) {
      City retVal = new City(length, depth);
      retVal.AddRoads();
      retVal.TranslateRoads();
      retVal.AddBuildings();
      retVal.AddCorporates();
      retVal.BoardImage = CityImageGenerator.ConvertToImage(retVal);

      // Corporate.print();

      return retVal;
    }

    public static GameBoard DebugCity() {
      City retVal = new City(10, 10);

      // retVal.Grid[2, 0] = new RoadTile();

      //retVal.Grid[1, 2] = new RoadTile();
      //((RoadTile)retVal.Grid[1, 2]).HOffset = 1;
      //((RoadTile)retVal.Grid[1, 2]).HDepth = 2;

      retVal.Grid[1, 1] = new RoadTile();
      ((RoadTile)retVal.Grid[1, 1]).VOffset = 0;
      ((RoadTile)retVal.Grid[1, 1]).VDepth = 2;
      retVal.Grid[1, 2] = new RoadTile();
      ((RoadTile)retVal.Grid[1, 2]).VOffset = 0;
      ((RoadTile)retVal.Grid[1, 2]).VDepth = 2;
      retVal.Grid[1, 3] = new RoadTile();
      ((RoadTile)retVal.Grid[1, 3]).VOffset = 0;
      ((RoadTile)retVal.Grid[1, 3]).VDepth = 2;
      retVal.Grid[2, 1] = new RoadTile();
      ((RoadTile)retVal.Grid[2, 1]).VOffset = 1;
      ((RoadTile)retVal.Grid[2, 1]).VDepth = 2;
      retVal.Grid[2, 2] = new RoadTile();
      ((RoadTile)retVal.Grid[2, 2]).VOffset = 1;
      ((RoadTile)retVal.Grid[2, 2]).VDepth = 2;
      retVal.Grid[2, 3] = new RoadTile();
      ((RoadTile)retVal.Grid[2, 3]).VOffset = 1;
      ((RoadTile)retVal.Grid[2, 3]).VDepth = 2;
      //retVal.Grid[3, 1] = new RoadTile();
      //((RoadTile)retVal.Grid[3, 1]).VOffset = 2;
      //((RoadTile)retVal.Grid[3, 1]).VDepth = 3;
      //retVal.Grid[3, 2] = new RoadTile();
      //((RoadTile)retVal.Grid[3, 2]).VOffset = 2;
      //((RoadTile)retVal.Grid[3, 2]).VDepth = 3;
      //retVal.Grid[3, 3] = new RoadTile();
      //((RoadTile)retVal.Grid[3, 3]).VOffset = 2;
      //((RoadTile)retVal.Grid[3, 3]).VDepth = 3;

      // retVal.Grid[0, 2] = new RoadTile();
      retVal.Grid[3, 2] = new RoadTile();

      //retVal.addRoads();
      retVal.TranslateRoads();
      for (int i = 0; i < retVal.Length; ++i) {
        for (int j = 0; j < retVal.Depth; ++j) {
          if (retVal.Grid[i, j].Type == ContentType.ROAD)
            System.Console.Write('*');
          else System.Console.Write(' ');
        }
        System.Console.Write('\n');
      }
      //System.Console.ReadKey();
      return retVal;
    }

    #endregion public methods
  }
}
