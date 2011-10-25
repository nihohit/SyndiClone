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
        private const int MIN_BLOCK_SIZE = 0; // the smaller block that will have sub-roads is of size 7X6
        private readonly Random rand;

        private char[,] _grid;
        private Tile[,] _grid2;
        private List<Building> _buildings;
        private List<Point> _intersections;
        private int _len, _wid;

        /**
         * Constructor.
         */
        public City(int gridL, int gridW)
        {
            rand = new Random();
            _len = gridL;
            _wid = gridW;
            _grid = new char[_len, _wid];
            _grid2 = new Tile[_len, _wid];
            for (int i = 0; i < _len; ++i)
                for (int j = 0; j < _wid; ++j)
                    _grid2[i, j] = new Tile();
 

            _buildings = new List<Building>();
            _intersections = new List<Point>();
        }

        /**
         * Constructor.
         */
        public City(int gridL, int gridW, List<Building> building)
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
       /* private void printBlock(int startX, int startY, int len, int wid) {
            System.IO.StreamWriter file = new System.IO.StreamWriter("city" + _count + ".mf");
            Console.Out.WriteLine("block number: " + _count);
            for (int i = 0; i < _len; ++i) {
                for (int j = 0; j < _wid; ++j) {
               
                    file.Write(_grid i,  j]);
                   // Console.Write(_grid i, j]);
                }
                file.Write("\r\n");
                //Console.Write('\n');
             
            }
            file.Close();
            _count++;

        }*/

        private bool isConnected(int x, int y, Directions dir,int offset) { 
             switch (dir)
             {
                 case Directions.E:
                     if (y - offset < 0) return false; //error. print?
                     if (y - offset == 0) return false; //legit.
                     if (_grid2[x, y -offset- 1].getType() == ContentType.ROAD)
                         return true;
                     return false;

                 case Directions.N:
                     if (x - offset < 0) return false; //error.
                     if (x - offset == 0) return false; //legit.
                     if (_grid2[x -offset - 1, y].getType() == ContentType.ROAD)
                         return true;
                     return false;
                     break;
                 case Directions.W:
                     if (y + offset + 1 > _wid) return false; //error
                     if (y + offset+1 == _wid) return false; //legit
                     return (_grid2[x, y + offset+1].getType() == ContentType.ROAD);
                 case Directions.S:
                     if (x + offset + 1 > _wid) return false;//error
                     if (x + offset + 1 == _wid) return false;//legit
                     return (_grid2[x + offset + 1, y].getType() == ContentType.ROAD);
                 default: return false; //error
             }
        }

        internal void translateRoads()
        {
            RoadTile current;
            for (int i = 0; i < _len; ++i)
            {
                for (int j = 0; j < _wid; ++j)
                {
                    if (_grid2[i, j].getType() == ContentType.ROAD)
                    {
                        current = (RoadTile)_grid2[i, j];
                        //set number of exits from the tile
                        if (isConnected(i, j, Directions.E, current.getVOffset()))
                            current.addExit();
                        if (isConnected(i, j, Directions.N, current.getHOffset()))
                            current.addExit();
                        if (isConnected(i, j, Directions.W, current.getVWidth() - current.getVOffset() - 1)) //the "-1" part is so that the offset will lead to the edge of the road
                            current.addExit();
                        if (isConnected(i, j, Directions.S, current.getHWidth() - current.getHOffset() - 1))
                            current.addExit();

                        //set rotate (note that "setRotate(0)" is redundant, but it's done anyway.
                        // rotate 4 means error.
                        switch (current.getExits())
                        {
                            case 1:
                                if (isConnected(i, j, Directions.W, current.getVOffset())) //EW begining
                                    current.setRotate(0);
                                else if (isConnected(i, j, Directions.S, current.getHOffset())) //NS begining
                                    current.setRotate(1);
                                else if (isConnected(i, j, Directions.E, current.getHOffset())) //EW end
                                    current.setRotate(2);
                                else if (isConnected(i, j, Directions.N, current.getHOffset())) //NS end
                                    current.setRotate(3);
                                else current.setRotate(4);
                                break;
                            case 2: //either 90 deg turn or none
                                if (current.getDirection() == Directions.EW)
                                    current.setRotate(1);
                                else if (current.getDirection() == Directions.NS) current.setRotate(0);
                                break;
                            case 3:
                                if (!isConnected(i, j, Directions.W, current.getVOffset()))
                                { //3rd road to the East
                                    current.setRotate(0);
                                    current.setDirection(Directions.NS);
                                }
                                else if (!isConnected(i, j, Directions.S, current.getHOffset()))
                                { //3rd road to the North
                                    current.setRotate(1);
                                    current.setDirection(Directions.EW);
                                }
                                else if (!isConnected(i, j, Directions.E, current.getHOffset()))
                                { //3rd road to the West
                                    current.setRotate(2);
                                    current.setDirection(Directions.NS);
                                }
                                else if (!isConnected(i, j, Directions.N, current.getHOffset()))
                                { //3rd road to the South
                                    current.setRotate(3);
                                    current.setDirection(Directions.EW);
                                }
                                else current.setRotate(4); break;
                            case 4: if (current.getDirection() == Directions.FOURWAY) current.setRotate(0); else current.setRotate(4);
                                break;
                            default: current.setRotate(4); break;
                        }
                    }
                }
            }
        }

        internal void addRoads(int length, int width)
        {
            List<Block> blocks = new List<Block>();
            addMainRoads(0, 0, length, width, ref blocks);

            Block temp;
            int cond = blocks.Count;
            //printBlock(0, 0, _len, _wid);
            while (cond > 0) {
                temp = blocks[0];
              //  Console.Out.WriteLine("Block size: " + temp.getWid() * temp.getLen());
                if (temp.Width * temp.Length > MIN_BLOCK_SIZE)
                {
                    addMainRoads(temp.StartX, temp.StartY, temp.Length, temp.Width, ref blocks);
                   // printBlock(temp.getX, temp.getY, temp.getLen(), temp.getWid());
                }
                blocks.RemoveAt(0);
                cond = blocks.Count;
            }

        }

        private void addMainRoads(int startX, int startY, int length, int width, ref List<Block> blocks)
        {
           
            int lenRoadsNum = 0, widRoadsNum = 0;
            //Random rand = new Random();

            //this gives us the max possible road width for both vertical(length) and horizontal(width) roads
            int maxLenRoad = (int)(Math.Log10(length)); 
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
                            if ((startX + i + j < _len) && (startY + k) < _wid)
                            {
                                _grid[startX + i + j, startY + k] = ROAD_GENERIC;
                                if (_grid2[startX + i + j, startY + k].getType() != ContentType.ROAD)
                                    _grid2[startX + i + j, startY + k] = new RoadTile();
                                ((RoadTile)_grid2[startX + i + j, startY + k]).addDirection(false);
                                ((RoadTile)_grid2[startX + i + j, startY + k]).setHWidth(m);
                                ((RoadTile)_grid2[startX + i + j, startY + k]).setHOffset(j);
                            }
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
                            if ((startX + k < _len) && (startY + i + j) < _wid)
                            {//make sure we're not out of the grid
                                _grid[startX + k, startY + i + j] = ROAD_GENERIC;
                                if (_grid2[startX + k, startY + i + j].getType() != ContentType.ROAD)
                                    _grid2[startX + k, startY + i + j] = new RoadTile();
                                ((RoadTile)_grid2[startX + k, startY + i + j]).addDirection(true);
                                ((RoadTile)_grid2[startX + k, startY + i + j]).setVWidth(m);
                                ((RoadTile)_grid2[startX + k, startY + i + j]).setVOffset(j);

                            }
                        }
                }
                widBlockEdge.Add(width);
            }
            // List<Block> blocks = new List<Block>();
            if (blocks.Count == 0)
            {
                int blockLength = 0, blockwidth = 0;
                for (int i = 0; i < lenBlockEdge.Count - 1; i += 2)
                {
                    for (int j = 0; j < widBlockEdge.Count - 1; j += 2)
                    {
                        if (lenBlockEdge.Count > 1)
                            blockLength = lenBlockEdge[i + 1] - lenBlockEdge[i];
                        else blockLength = length;

                        if (widBlockEdge.Count > 1)
                            blockwidth = widBlockEdge[j + 1] - widBlockEdge[j];
                        else blockwidth = width;
                        if ((blockwidth < 0) || (blockLength < 0))
                            Console.Error.WriteLine(startX + " ERROR!!! " + startY);
                        else blocks.Add(new Block(lenBlockEdge[i], widBlockEdge[j], blockLength, blockwidth));
                        // Console.Out.WriteLine(lenBlockEdge[i]+"-"+lenBlockEdge[i+1]+"X"+widBlockEdge[j]+"-"+widBlockEdge[j+1]+" legnthXwid =" +blockLength+"X"+blockwidth);
                    }
                }
            }
        }


        /****************************************************************************
         * Getters, Setters, simple functions
         * ****************************************************/

        public List<Building> getBuildings() { return _buildings; }
        public int getLen() { return _len; }
        public int getWid() { return _wid; }
        public char[,] getGrid() { return _grid; }
        public Tile[,] getGrid2() { return _grid2; }


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
