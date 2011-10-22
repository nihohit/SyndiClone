using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * It's a city. a pre-processing phase city, but a city nonetheless.
 */
namespace Game.City_Generator
{
    class City
    {
        private const int GAP_RATIO = 6;
        private const char EMPTY = ' ';
        private const char ROAD_GENERIC = '*'; // a generic char for a road, not knowing direction or it's adjacent squares.
        private const int MIN_BLOCK_SIZE = 100; // the smaller block that will have sub-roads is of size 7X6


        private char[,] _grid;
        private List<Building> _buildings;
        private List<Point> _intersections;
        private int _len, _wid;

        /**
         * Constructor.
         */
        public City(int gridL, int gridW)
        {
            _len = gridL;
            _wid = gridW;
            _grid = new char[_len, _wid];
            _buildings = new List<Building>();
            _intersections = new List<Point>();
        }

        /**
         * Constructor.
         */
        public City(int gridL, int gridW, ref List<Building> building)
        {
            _len = gridL;
            _wid = gridW;
            _grid = new char[_len, _wid];
            _buildings = building;
            _intersections = null;
        }

        /*********************************************************************
         * Map Creation: roads, buildings etc.
         * ******************************************************************/
        internal void addRoads(int length, int width)
        {
            List<Block> blocks = new List<Block>();
            addMainRoads(0, 0, length, width, ref blocks);

            Block temp;
            int cond = blocks.Count;
            while (cond > 0) {
                temp = blocks[0];
              //  Console.Out.WriteLine("Block size: " + temp.getWid() * temp.getLen());
               if (temp.getWid()*temp.getLen()> MIN_BLOCK_SIZE)
                    addMainRoads(temp.getX(), temp.getY(), temp.getLen(), temp.getWid(),ref blocks);
                blocks.RemoveAt(0);
                cond = blocks.Count;
            }

        }

        private void addMainRoads(int startX, int startY, int length, int width, ref List<Block> blocks)
        {
            int lenRoadsNum = 0, widRoadsNum = 0;
            Random rand = new Random();

            //this gives us the max possible road width for both vertical(length) and horizontal(width) roads
            int maxLenRoad = (int)(Math.Log10(length)); //TODO: CHECK!!! dangerous cast?
            int maxWidRoad = (int)(Math.Log10(width));
            if (maxLenRoad > 0) lenRoadsNum = (width / maxLenRoad) / GAP_RATIO;
            if (maxWidRoad > 0) widRoadsNum = (length / maxWidRoad) / GAP_RATIO;
          //  Console.Out.WriteLine(maxLenRoad + "X" + maxWidRoad);

           // Console.Out.WriteLine("lenRoadsNum=" + lenRoadsNum);

            List<int> lenRoads = new List<int>();
            List<int> widRoads = new List<int>();
            int gap;

            gap = maxWidRoad * GAP_RATIO; 
            for (int i = 0; i < widRoadsNum; ++i)
            {
                lenRoads.Add(rand.Next(maxWidRoad, gap - maxWidRoad) + (i * gap));
            }

            gap = maxLenRoad * GAP_RATIO; 
            for (int i = 0; i < lenRoadsNum; ++i)
            {
                widRoads.Add(rand.Next(maxLenRoad, gap - maxLenRoad) + (i * gap));
            }

            List<int> lenBlockEdge = new List<int>();
            List<int> widBlockEdge = new List<int>();
            int m;
            lenBlockEdge.Add(0);
            widBlockEdge.Add(0);
            if (maxWidRoad >= 1) //this way the "rand.next()" function will not blow up
            {
                foreach (int i in lenRoads)
                {
                    m = rand.Next(1, maxWidRoad);
                    lenBlockEdge.Add(i);
                    lenBlockEdge.Add(i + m);
                    for (int j = 0; j < m; ++j)
                        for (int k = 0; k < width; ++k)
                            if ((startX + i+j < _len) && (startY + k) < _wid)
                                _grid[startX + i + j, startY + k] = ROAD_GENERIC;
                }
                lenBlockEdge.Add(length);
            }

            if (maxLenRoad >= 1) //this way the "rand.next()" function will not blow up
            {
                foreach (int i in widRoads)
                {
                    m = rand.Next(1, maxLenRoad);
                    widBlockEdge.Add(i);
                    widBlockEdge.Add(i + m);
                    for (int j = 0; j < m; ++j)
                        for (int k = 0; k < length; ++k)
                        {
                            if ((startX+k<_len) &&(startY+i+j)<_wid)
                            _grid[startX + k, startY + i + j] = ROAD_GENERIC;
                        }
                }
                widBlockEdge.Add(width);
            }
            // List<Block> blocks = new List<Block>();
           
            int blockLength = 0, blockwidth = 0;
            for (int i = 0; i < lenBlockEdge.Count - 1; i += 2)
                for (int j = 0; j < widBlockEdge.Count - 1; j += 2)
                {
                    if (lenBlockEdge.Count > 1)
                        blockLength = lenBlockEdge[i + 1] - lenBlockEdge[i];
                    else blockLength = length;

                    if (widBlockEdge.Count > 1)
                        blockwidth = widBlockEdge[j + 1] - widBlockEdge[j];
                    else blockwidth = width;
                    if ((blockwidth < 0) || (blockLength < 0))
                        Console.Error.WriteLine(startX+" ERROR!!! "+startY);
                    else blocks.Add(new Block(lenBlockEdge[i], widBlockEdge[j], blockLength, blockwidth));
                   // Console.Out.WriteLine(lenBlockEdge[i]+"-"+lenBlockEdge[i+1]+"X"+widBlockEdge[j]+"-"+widBlockEdge[j+1]+" legnthXwid =" +blockLength+"X"+blockwidth);
                }
        }


        /****************************************************************************
         * Getters, Setters, simple functions
         * ****************************************************/
        public char[,] getGrid() { return _grid; }

        public List<Building> getBuildings() { return _buildings; }
        public int getLen() { return _len; }
        public int getWid() { return _wid; }
        public char[,] getGrid() { return _grid; }
        public short[][] getShortGrid() { 
            short[][] retVal = new short[_len][];
            for (int i = 0; i < _len; ++i)
            {
                retVal[i] = new short[_wid];
                for (int j = 0; j < _wid; ++j)
                    retVal[i][j] = (short)_grid[i, j];
            }
            return retVal;

        } 

        public void setGrid(char[,] grid)
        {
            _grid = grid;
        }
        


    }
}
