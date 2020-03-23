using System;
using System.Collections.Generic;

/**
 * It's a city. a pre-processing phase city, but a city nonetheless.
 * here we make most of the processing needed to create a city. 
 * the phases of creating a city are done by this order:
 * 1) Create a city object.
 * 2) Add roads to the city.
 * 3) Collect data ("translate") about the roads created - direction, Depthth etc. store it in the road-tiles
 * 4) Add Buildings.
 * 5) Divide building into corporates.
 */
namespace Game.City_Generator {
  class City : GameBoard {
    #region constants

    private const int GAP_RATIO = 6;
    private const double TAKEOVER_CHANCE = 0.2;
    private const int BIG_CORPS = 10; //TODO: maybe later we will want to change this to something related to boardsize.
    private const char EMPTY = ' ';
    private const char ROAD_GENERIC = '*'; // a generic char for a road, not knowing direction or it's adjacent squares.
    private const int MIN_BLOCK_SIZE = 0; // the smaller block that will have sub-roads is of size 7X6
    private const int CORP_DIM = 20; //see "add corporates()" for use, signifies the size of each initial corporate
    static readonly Random s_random = new Random();

    #endregion

    #region fields

    private char[,] m_grid;
    private BuildingPlacer m_buildPlacer;

    #endregion

    #region constructors

    public City(int gridL, int gridW) {
      BoardImage = null;
      m_buildPlacer = new BuildingPlacer();
      Length = gridL;
      Depth = gridW;
      m_grid = new char[Length, Depth];
      Grid = new Tile[Length, Depth];

      for (int i = 0; i < Length; ++i)
        for (int j = 0; j < Depth; ++j)
          Grid[i, j] = new Tile();

      CorpList = new Corporate[(1 + (Length / CORP_DIM)), 1 + (Depth / CORP_DIM)];

      for (int i = 0; i < (Length / CORP_DIM) + 1; ++i)
        for (int j = 0; j < (Depth / CORP_DIM) + 1; ++j)
          CorpList[i, j] = new Corporate();

      Buildings = new List<Building>();
    }

    #endregion

    #region adding roads

    /**
     * This method adds roads to a city by calling to "add main roads" recursively, first time with the whole grid, and after that it adds roads block by block.
     * */
    public void AddRoads() {
      List<Block> blocks = new List<Block>();
      AddMainRoads(0, 0, Length, Depth, ref blocks);

      Block temp;
      int cond = blocks.Count;
      while (cond > 0) {
        temp = blocks[0];
        if (temp.Depth * temp.Length > MIN_BLOCK_SIZE) {
          AddMainRoads(temp.StartX, temp.StartY, temp.Length, temp.Depth, ref blocks);
        }
        blocks.RemoveAt(0);
        cond = blocks.Count;
      }
    }

    /**
     * this method adds a few roads that cross an entire "block" (whose dimensions are given as parameters)
     * */
    private void AddMainRoads(int startX, int startY, int Lengthgth, int Depthth, ref List<Block> blocks) {

      int LengthRoadsNum = 0, DepthRoadsNum = 0;

      //this gives us the max possible road Depthth for both vertical(Lengthgth) and horizontal(Depthth) roads
      int maxLenRoad = (int)(Math.Log10(Lengthgth));
      int maxDepRoad = (int)(Math.Log10(Depthth));
      if (maxLenRoad > 0) LengthRoadsNum = (Depthth / maxLenRoad) / GAP_RATIO;
      if (maxDepRoad > 0) DepthRoadsNum = (Lengthgth / maxDepRoad) / GAP_RATIO;

      List<int> LengthRoads = new List<int>();
      List<int> DepthRoads = new List<int>();
      int gap;

      gap = maxDepRoad * GAP_RATIO;
      for (int i = 0; i < DepthRoadsNum; ++i) {
        LengthRoads.Add(s_random.Next(maxDepRoad, gap - maxDepRoad) + (i * gap));
      }

      gap = maxLenRoad * GAP_RATIO;
      for (int i = 0; i < LengthRoadsNum; ++i) {
        DepthRoads.Add(s_random.Next(maxLenRoad, gap - maxLenRoad) + (i * gap));
      }

      List<int> LengthBlockEdge = new List<int>();
      List<int> DepthBlockEdge = new List<int>();
      int m;
      LengthBlockEdge.Add(0);
      DepthBlockEdge.Add(0);
      if (maxDepRoad >= 1) //this way the "rand.next()" function will not blow up
      {
        foreach (int i in LengthRoads) {
          m = s_random.Next(1, maxDepRoad);
          LengthBlockEdge.Add(i);
          LengthBlockEdge.Add(i + m);
          for (int j = 0; j < m; ++j)
            for (int k = 0; k < Depthth; ++k)
              if ((startY + i + j < Length) && (startX + k) < Depth) {
                m_grid[startY + i + j, startX + k] = ROAD_GENERIC;
                if (Grid[startY + i + j, startX + k].Type != ContentType.ROAD)
                  Grid[startY + i + j, startX + k] = new RoadTile();
                ((RoadTile)Grid[startY + i + j, startX + k]).HDepth = m;
                ((RoadTile)Grid[startY + i + j, startX + k]).HOffset = j;
              }
        }
        LengthBlockEdge.Add(Lengthgth);
      }

      if (maxLenRoad >= 1) //this way the "rand.next()" function will not blow up
      {
        foreach (int i in DepthRoads) {
          m = s_random.Next(1, maxLenRoad);
          DepthBlockEdge.Add(i);
          DepthBlockEdge.Add(i + m);
          for (int j = 0; j < m; ++j)
            for (int k = 0; k < Lengthgth; ++k) {
              if ((startY + k < Length) && (startX + i + j) < Depth) { //make sure we're not out of the grid
                m_grid[startY + k, startX + i + j] = ROAD_GENERIC;
                if (Grid[startY + k, startX + i + j].Type != ContentType.ROAD)
                  Grid[startY + k, startX + i + j] = new RoadTile();
                ((RoadTile)Grid[startY + k, startX + i + j]).VDepth = m;
                ((RoadTile)Grid[startY + k, startX + i + j]).VOffset = j;
              }
            }
        }
        DepthBlockEdge.Add(Depthth);
      }
      // List<Block> blocks = new List<Block>();
      if (blocks.Count == 0) {
        int blockLength = 0, blockDepthth = 0;
        for (int i = 0; i < LengthBlockEdge.Count - 1; i += 2) {
          for (int j = 0; j < DepthBlockEdge.Count - 1; j += 2) {
            if (LengthBlockEdge.Count > 1)
              blockLength = LengthBlockEdge[i + 1] - LengthBlockEdge[i];
            else blockLength = Lengthgth;

            if (DepthBlockEdge.Count > 1)
              blockDepthth = DepthBlockEdge[j + 1] - DepthBlockEdge[j];
            else blockDepthth = Depthth;
            if ((blockDepthth < 0) || (blockLength < 0))
              Console.Error.WriteLine(startX + " ERROR!!! " + startY); //TODO - remove (debug)
            else blocks.Add(new Block(DepthBlockEdge[j], LengthBlockEdge[i], blockLength, blockDepthth));
            // Console.Out.WriteLine(LengthBlockEdge[x]+"-"+LengthBlockEdge[x+1]+"X"+DepthBlockEdge[y]+"-"+DepthBlockEdge[y+1]+" legnthXDepth =" +blockLength+"X"+blockDepthth);
          }
        }
      }
    }

    /**
     * this method adds some info to the road tiles to determine thier type and their rotate attribute.
     * */
    public void TranslateRoads() {
      RoadTile current;
      for (int j = 0; j < Depth; ++j) {
        for (int i = 0; i < Length; ++i) {
          if (Grid[i, j].Type == ContentType.ROAD) {
            Grid[i, j].Rotate = 0;
            //System.Console.WriteLine("new point: (" + x + "," + y + ")");

            current = (RoadTile)Grid[i, j];
            //set number of exits from the tile

            if (IsConnected(i, j, Directions.NORTH, current.HOffset)) {
              current.AddExit(Directions.NORTH);
              //System.Console.WriteLine("North!");
            }
            if (IsConnected(i, j, Directions.WEST, current.VOffset)) {
              current.AddExit(Directions.WEST);
              //System.Console.WriteLine("West!");
            }
            if (IsConnected(i, j, Directions.SOUTH, current.HDepth - current.HOffset - 1)) //the "-1" part is so that the offset will lead to the edge of the road
            {
              current.AddExit(Directions.SOUTH);
              //System.Console.WriteLine("South!");
            }
            if (IsConnected(i, j, Directions.EAST, current.VDepth - current.VOffset - 1)) {
              current.AddExit(Directions.EAST);
              //System.Console.WriteLine("East!");
            }
          }
        }
      }
    }

    #endregion

    #region adding Buildings

    /**
     * Adds building to a city using the "add building" method.
     * */
    public void AddBuildings() {
      int id = 0;
      for (int i = 0; i < Length; ++i)
        for (int j = 0; j < Depth; ++j)
          if (Grid[i, j].Type == ContentType.EMPTY) {
            AddBuilding(i, j, id); //y will be bigger in the Depthth of the new building
            id++;
          }
      //Console.Out.WriteLine("Buildings num=" + Buildings.Count);
      ConnectBuildings2Roads();
      //_bp.print();
    }

    /*
     * creates a building, adds it to the Buildings list
     * */
    private void AddBuilding(int y, int x, int id) {
      Block buildingSize;
      Building b;
      int Lengthgth, Depthth;
      Depthth = 0;
      Lengthgth = 0;

      for (;
        ((x + Depthth < Depth) && (Grid[y, x + Depthth].Type == ContentType.EMPTY)); ++Depthth) ;

      Depthth--; //no matter why we've stopped, we need to go one step backwards

      for (;
        ((y + Lengthgth < Length) && (Grid[y + Lengthgth, x + Depthth].Type == ContentType.EMPTY)); ++Lengthgth) ;

      Lengthgth--;

      if (Lengthgth < 1)
        return;
      if (Depthth < 1)
        return;

      buildingSize = new Block(x, y, m_buildPlacer.GetVDimension(Lengthgth), m_buildPlacer.GetHDimension(Depthth));
      b = new Building(buildingSize, id);

      for (int i = y; i < y + buildingSize.Length; ++i) {
        for (int j = x; j < x + buildingSize.Depth; ++j) {
          if (Grid[i, j].Type != ContentType.EMPTY) {
            continue;
          }
          Grid[i, j] = new BuildingTile(b);
        }
      }

      Buildings.Add(b);
    }

    /**
     * this method is helping me find out whether a RoadTile is connected to another road in a certain direction.
     * */

    private bool IsConnected(int y, int x, Directions dir, int offset) {
      switch (dir) {
        case Directions.NORTH:

          if (x - offset < 0) return false; //error. print?

          if (x - offset == 0) return false; //legit.

          return Grid[y, x - offset - 1].Type == ContentType.ROAD;

        case Directions.WEST:

          if (y - offset < 0) return false; //error.

          if (y - offset == 0) return false; //legit.

          return Grid[y - offset - 1, x].Type == ContentType.ROAD;

        case Directions.SOUTH:

          if (x + offset + 1 > Depth) return false; //error

          if (x + offset + 1 == Depth) return false; //legit

          return Grid[y, x + offset + 1].Type == ContentType.ROAD;

        case Directions.EAST:

          if (y + offset + 1 > Length) return false; //error

          if (y + offset + 1 == Length) return false; //legit

          return Grid[y + offset + 1, x].Type == ContentType.ROAD;

        default:
          return false; //error
      }
    }

    /**
     * if there's a building that is one row remote from a road - enlarge this building so that it reaches a road.
     * */
    private void ConnectBuildings2Roads() {
      bool cond = true;
      bool connected;
      while (cond) //this while allows me to wreak havoc to the Buildings list without ruining my iterator.
      {
        cond = false;
        foreach (Building b in Buildings) {
          connected = false;
          if (b.Dimensions.StartY + b.Dimensions.Length < Length) {
            if (Grid[b.Dimensions.StartY + b.Dimensions.Length, b.Dimensions.StartX].Type == ContentType.ROAD) {
              //Console.Out.WriteLine("(" + b.Dimensions.StartY + "," + b.Dimensions.StartX + ") ---ROAD Below");
              //continue;
              b.ExitDirection = 3;
              connected = true;
            }
          }

          if (b.Dimensions.StartY > 0) {
            if (Grid[b.Dimensions.StartY - 1, b.Dimensions.StartX].Type == ContentType.ROAD) {
              // Console.Out.WriteLine("("+b.Dimensions.StartY+","+b.Dimensions.StartX+") ---ROAD above");
              //continue;
              b.ExitDirection = 2;
              connected = true;
            }
          }
          if (b.Dimensions.StartX > 0) {
            if (Grid[b.Dimensions.StartY, b.Dimensions.StartX - 1].Type == ContentType.ROAD) {
              //Console.Out.WriteLine("(" + b.Dimensions.StartY + "," + b.Dimensions.StartX + ") ---ROAD to Left");
              //continue;
              b.ExitDirection = 0;
              connected = true;
            }
          }

          if (b.Dimensions.StartX + b.Dimensions.Depth < Depth) {
            if (Grid[b.Dimensions.StartY, b.Dimensions.StartX + b.Dimensions.Depth].Type == ContentType.ROAD) {
              // Console.Out.WriteLine("(" + b.Dimensions.StartY + "," + b.Dimensions.StartX + ") ---ROAD to Right");
              //continue;
              b.ExitDirection = 1;
              connected = true;
            }
          }
          if (ExpandToRoad(b) || connected)
            continue;

          for (int i = 0; i < b.Dimensions.Length; ++i) {
            for (int j = 0; j < b.Dimensions.Depth; ++j) {
              Grid[b.Dimensions.StartY + i, b.Dimensions.StartX + j] = new Tile();
              m_grid[b.Dimensions.StartY + i, b.Dimensions.StartX + j] = '/';
            }
          }

          Buildings.Remove(b);
          cond = true;
          break;
        }
      }
    }

    /**
     * this method connects a building to all roads within 1 step of it.
     * */
    private bool ExpandToRoad(Building b) {
      bool retVal = false;

      if (b.Dimensions.StartX + b.Dimensions.Depth + 1 < Depth) {
        if ((Grid[b.Dimensions.StartY, b.Dimensions.StartX + b.Dimensions.Depth].Type == ContentType.EMPTY) && (Grid[b.Dimensions.StartY, b.Dimensions.StartX + b.Dimensions.Depth + 1].Type == ContentType.ROAD)) {
          for (int i = b.Dimensions.StartY; i < b.Dimensions.StartY + b.Dimensions.Length; ++i) {
            if (Grid[i, b.Dimensions.StartX + b.Dimensions.Depth].Type != ContentType.EMPTY) {
              Console.Out.WriteLine("trying to overwrite (" + (b.Dimensions.StartY - 1) + "," + i + ")");
            } else {
              Grid[i, b.Dimensions.StartX + b.Dimensions.Depth] = new BuildingTile(b);
              m_grid[i, b.Dimensions.StartX + b.Dimensions.Depth] = '#'; //_grid[b.Dimensions.StartY, b.Dimensions.StartX];
            }
          }
          b.Dimensions.Depth++;
          retVal = true;
        }
      }

      if (b.Dimensions.StartY > 1 && (Grid[b.Dimensions.StartY - 1, b.Dimensions.StartX].Type == ContentType.EMPTY) && (Grid[b.Dimensions.StartY - 2, b.Dimensions.StartX].Type == ContentType.ROAD)) {
        for (int i = b.Dimensions.StartX; i < b.Dimensions.StartX + b.Dimensions.Depth; ++i) {
          if (Grid[b.Dimensions.StartY - 1, i].Type != ContentType.EMPTY) {
            Console.Out.WriteLine("trying to override (" + (b.Dimensions.StartY - 1) + "," + i + ")");
          } else {
            Grid[b.Dimensions.StartY - 1, i] = new BuildingTile(b);
            m_grid[b.Dimensions.StartY - 1, i] = '#'; //_grid[b.Dimensions.StartY, x];
          }
        }
        b.Dimensions.Length++;
        b.Dimensions.StartY--;
        retVal = true;
      }

      if (b.Dimensions.StartX > 1 && (Grid[b.Dimensions.StartY, b.Dimensions.StartX - 1].Type == ContentType.EMPTY) && (Grid[b.Dimensions.StartY, b.Dimensions.StartX - 2].Type == ContentType.ROAD)) {
        for (int i = b.Dimensions.StartY; i < b.Dimensions.StartY + b.Dimensions.Length; ++i) {
          if (Grid[i, b.Dimensions.StartX - 1].Type != ContentType.EMPTY) {
            Console.Out.WriteLine("trying to override (" + (b.Dimensions.StartY - 1) + "," + i + ")");
          } else {
            Grid[i, b.Dimensions.StartX - 1] = new BuildingTile(b);
            m_grid[i, b.Dimensions.StartX - 1] = '#'; // grid[b.Dimensions.StartY, b.Dimensions.StartX];
          }
        }
        b.Dimensions.Depth++;
        b.Dimensions.StartX--;
        retVal = true;
      }

      if (b.Dimensions.StartY + b.Dimensions.Length + 1 < Length)
        if ((Grid[b.Dimensions.StartY + b.Dimensions.Length, b.Dimensions.StartX].Type == ContentType.EMPTY) && (Grid[b.Dimensions.StartY + b.Dimensions.Length + 1, b.Dimensions.StartX].Type == ContentType.ROAD)) {
          for (int i = b.Dimensions.StartX; i < b.Dimensions.StartX + b.Dimensions.Depth; ++i) {
            if (Grid[b.Dimensions.StartY + b.Dimensions.Length, i].Type != ContentType.EMPTY)
              Console.Out.WriteLine("trying to override (" + (b.Dimensions.StartY - 1) + "," + i + ")");
            else {
              Grid[b.Dimensions.StartY + b.Dimensions.Length, i] = new BuildingTile(b);
              m_grid[b.Dimensions.StartY + b.Dimensions.Length, i] = '#'; //_grid[b.Dimensions.StartY, b.Dimensions.StartX];
            }
          }
          b.Dimensions.Length++;
          retVal = true;
        }
      return retVal;
    }

    #endregion

    #region adding corporations

    /**
     * divides the Buildings between corporates according to geographical location
     * TODO: not tested (waiting for graphical interface)
     * */
    public void AddCorporates() {
      Building b;
      foreach (Tile t in Grid)
        if (t.Type == ContentType.BUILDING) {
          b = ((BuildingTile)t).Building;
          if (!b.HasCorp()) {
            b.JoinCorp(CorpList[(b.Dimensions.StartY + (b.Dimensions.Length / 2)) / CORP_DIM, (b.Dimensions.StartX + (b.Dimensions.Depth / 2)) / CORP_DIM]);
          }
        }
      for (int i = 0; i < BIG_CORPS; ++i)
        Takeover(s_random.Next(Length / CORP_DIM), s_random.Next(Depth / CORP_DIM));
    }

    /**
     * it's a way to make some corporates bigger then the others.
     * */
    private void Takeover(int iLoc, int jLoc) {

      for (int i = iLoc - 1; i <= iLoc + 1; ++i) {
        if (i < 0) continue;
        if (i >= CorpList.GetLength(0)) break;
        for (int j = jLoc - 1; j <= jLoc + 1; ++j) {
          if (j < 0) continue;
          if (j >= CorpList.GetLength(1)) break;
          if (s_random.NextDouble() < TAKEOVER_CHANCE) {
            CorpList[iLoc, jLoc].Takeover(CorpList[i, j]);
            CorpList[i, j] = CorpList[iLoc, jLoc];
          }
        }
      }
    }

    #endregion

    #region BuildingPlacer
    /*
     * this class is a tool that helps me create new Buildings.
     * By theory, it was meant to keep a changing probability to decide the Lengthgth and Depthth of the building considering the past, so the more Buildings there are of a certain
     * Lengthgth, the less likely this Lengthgth to appear. 
     * However, after testing it a little, I found that in the end the probabilities to get a big building reaches 100% (due to place constraints). 
     * Maybe I will think of a different strategy later.
     * */
    public class BuildingPlacer {
      #region constants

      private const double DECREASE_FACTOR = 0.01,
        LOWER_HALF_PROB = 0.25;
      private const int ARR_SIZE = 12;

      #endregion

      #region fields

      private double[] hPlaces, vPlaces;

      #endregion

      #region constructor

      public BuildingPlacer() {
        hPlaces = new double[ARR_SIZE];
        vPlaces = new double[ARR_SIZE]; // these two arrays hold the probability to get an x Lengthgth wall.

        //initialize the arrays.
        vPlaces[0] = vPlaces[1] = hPlaces[0] = hPlaces[1] = 0;
        int i;
        int middle = ARR_SIZE / 2;
        double quota = LOWER_HALF_PROB / (middle - 1); //lower half initial probability.

        for (i = 2; i <= middle; ++i) {
          hPlaces[i] = vPlaces[i] = (i - 1) * quota;
        }
        quota = (1 - LOWER_HALF_PROB) / (middle - 1 + (ARR_SIZE % 2)); //the modulo is to deal with both even and odd sized arrays.
        for (; i < ARR_SIZE; ++i) {
          hPlaces[i] = vPlaces[i] = LOWER_HALF_PROB + (i - middle) * quota;
        }
        // print();
      }

      #endregion

      #region public methods

      public void Print() { //TODO: remove method.
        Console.Out.Write("HPlaces: ");
        Console.Out.WriteLine();
        for (int i = 1; i < ARR_SIZE; ++i) {
          Console.Out.Write(i + ":" + String.Format("{0:F2}", hPlaces[i] - hPlaces[i - 1]) + "\\ "); //chances
          Console.Out.Write(i + ":" + String.Format("{0:F2}", hPlaces[i]) + ", "); //actual values
          Console.Out.WriteLine();
        }
        Console.Out.Write("\nVPlaces:\n  ");
        for (int i = 1; i < ARR_SIZE; ++i) {
          Console.Out.Write(i + ":" + String.Format("{0:F2}", vPlaces[i] - vPlaces[i - 1]) + "\\ "); //chances              
          Console.Out.Write(i + ":" + String.Format("{0:F2}", vPlaces[i]) + ", "); //actual values
          Console.Out.WriteLine();
        }
        Console.Out.WriteLine();
      }

      public int GetVDimension(int max) {
        if (max < 1) {
          //  MessageBox.Show("line 71, max is:" + max);
          return 0;
        }
        int retVal;
        max = Math.Min(max, ARR_SIZE - 1);

        //double step = DECREASE_FACTOR / (ARR_SIZE - 3);//"-3" is meant to represent the fact that I'm not increasing the size of places 0,1 and x;
        //double total = 0;
        double rand = s_random.NextDouble() * vPlaces[Math.Min(max, ARR_SIZE - 1)];
        for (retVal = 2;
          ((rand > vPlaces[retVal]) && (retVal <= max)); ++retVal) ; //make sure that retVal is not higher than max (in case of non-positive probabilities)
        /* for (int y = 2; y < ARR_SIZE; ++y)
         {
             if (y == retVal)
             {
                 total -= (DECREASE_FACTOR + step);
                 //MessageBox.Show("total = " + total+" STEP="+step);
             }
             total += step;
             vPlaces[y] += total;
             if (vPlaces[y] < 0) vPlaces[y] = 0;
            
         }*/
        //TODO: decide if the code above is useful or not.
        return retVal;
      }

      public int GetHDimension(int max) {
        if (max < 1) return 0;
        //   if (max >= ARR_SIZE)
        //   Console.Out.WriteLine("H: Overshot. max is:" + max + " and the array is just:" + ARR_SIZE);//TODO - remove (debug)

        int retVal;
        //double step = DECREASE_FACTOR / (ARR_SIZE - 3);//"-3" is meant to represent the fact that I'm not increasing the size of places 0,1 and x;
        //double total = 0;
        max = Math.Min(max, ARR_SIZE - 1);
        double rand = s_random.NextDouble() * hPlaces[max];
        for (retVal = 2;
          (rand > hPlaces[retVal]) && (retVal <= max); ++retVal) ;
        /*for (int y = 2; y < ARR_SIZE; ++y)
        {
            if (y == retVal)
            {
                total -= (DECREASE_FACTOR+step);
                //MessageBox.Show("total = " + total+" STEP="+step);
            }
            total += step;
            hPlaces[y] += total;
            if (hPlaces[y] < 0) hPlaces[y] = 0;
        }*/
        //TODO: decide what to do with the code above
        return retVal;
      }

      #endregion
    }

    #endregion
  }
}