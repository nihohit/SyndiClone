using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms; //TODO: remove using - it's here so I can use Messagebox

/**
 * It's a city. a pre-processing phase city, but a city nonetheless.
 * here we make most of the processing needed to create a city. 
 * the phases of creating a city are done by this order:
 * 1) Create a city object.
 * 2) Add roads to the city.
 * 3) Collect data ("translate") about the roads created - direction, width etc. store it in the road-tiles
 * 4) Add Buildings.
 * 5) Divide building into corporates.
 */
namespace Game.City_Generator
{
    class City : GameBoard
    {
        /********************************Constants***************************************/
        private const int GAP_RATIO = 6;
        private const double TAKEOVER_CHANCE = 0.2;
        private const int BIG_CORPS = 10; //TODO: maybe later we will want to change this to something related to boardsize.
        private const char EMPTY = ' ';
        private const char ROAD_GENERIC = '*'; // a generic char for a road, not knowing direction or it's adjacent squares.
        private const int MIN_BLOCK_SIZE = 0; // the smaller block that will have sub-roads is of size 7X6
        private const int CORP_DIM = 20; //see "add corporates()" for use, signifies the size of each initial corporate
        private static readonly Random _rand = new Random();
        private static char c = 'A';//TODO: remove after debug phase


        /********************************Fields***************************************/
        private char[,] _grid;
        private BuildingPlacer _bp;
        

        /****************************************************************************************************************
         * ***************************************INNER CLASS************************************************************
         * **************************************************************************************************************/
        /*
         * this class is a tool that helps me create new buildings.
         * By theory, it was meant to keep a changing probability to decide the length and width of the building considering the past, so the more buildings there are of a certain
         * length, the less likely this length to appear. 
         * However, after testing it a little, I found that in the end the probabilities to get a big building reaches 100% (due to place constraints). 
         * Maybe I will think of a different strategy later.
         * */
        public class BuildingPlacer {
            /********************************Constants***************************************/
            private const double DECREASE_FACTOR = 0.01,LOWER_HALF_PROB = 0.25;
            private const int ARR_SIZE=12;
            /********************************Fields***************************************/
            private double[] _hPlaces,_vPlaces;

            /********************************Constructor***************************************/
            internal BuildingPlacer() {
                _hPlaces = new double[ARR_SIZE];
                _vPlaces = new double[ARR_SIZE];// these two arrays hold the probability to get an i length wall.

                //initialize the arrays.
                _vPlaces[0] = _vPlaces[1] = _hPlaces[0] = _hPlaces[1] = 0;
                int i;
                int middle = ARR_SIZE/2;
                double quota = LOWER_HALF_PROB/(middle-1); //lower half initial probability.

                for (i = 2; i <= middle; ++i)
                { 
                        _hPlaces[i] = _vPlaces[i] = (i-1)*quota;
                    }
                quota = (1-LOWER_HALF_PROB)/(middle-1+(ARR_SIZE%2)); //the modulo is to deal with both even and odd sized arrays.
                for (;i<ARR_SIZE;++i){
                    _hPlaces[i] = _vPlaces[i] = LOWER_HALF_PROB+(i-middle)*quota;
                }
               // print();
            }

            public void print() { //TODO: remove method.
                Console.Out.Write("HPlaces: ");
                Console.Out.WriteLine();
                for (int i = 1; i < ARR_SIZE; ++i)
                {
                    Console.Out.Write(i + ":" + String.Format("{0:F2}", _hPlaces[i] - _hPlaces[i - 1]) + "\\ "); //chances
                    Console.Out.Write(i + ":" + String.Format("{0:F2}", _hPlaces[i]) + ", ");                 //actual values
                    Console.Out.WriteLine();
                }
                Console.Out.Write("\nVPlaces:\n  ");
                for (int i = 1; i < ARR_SIZE; ++i)
                {
                    Console.Out.Write(i + ":" + String.Format("{0:F2}", _vPlaces[i] - _vPlaces[i - 1]) + "\\ "); //chances              
                    Console.Out.Write(i + ":" + String.Format("{0:F2}", _vPlaces[i]) + ", ");                 //actual values
                    Console.Out.WriteLine();
                }
                Console.Out.WriteLine();
            }

           
            internal int getVDimension(int max)
            {
                if (max < 1) {
                  //  MessageBox.Show("line 71, max is:" + max);
                    return 0; 
                }
                int retVal;
                max = Math.Min(max, ARR_SIZE - 1);

                double step = DECREASE_FACTOR / (ARR_SIZE - 3);//"-3" is meant to represent the fact that I'm not increasing the size of places 0,1 and i;
                double total = 0;
                double rand = _rand.NextDouble() * _vPlaces[Math.Min(max, ARR_SIZE - 1)]; 
                for (retVal = 2; ((rand > _vPlaces[retVal])&&(retVal<=max)); ++retVal) ; //make sure that retVal is not higher than max (in case of non-positive probabilities)
               /* for (int j = 2; j < ARR_SIZE; ++j)
                {
                    if (j == retVal)
                    {
                        total -= (DECREASE_FACTOR + step);
                        //MessageBox.Show("total = " + total+" STEP="+step);
                    }
                    total += step;
                    _vPlaces[j] += total;
                    if (_vPlaces[j] < 0) _vPlaces[j] = 0;
                    
                }*/
                //TODO: decide if the code above is useful or not.
                return retVal;
            }

            internal int getHDimension(int max)
            {
                if (max < 1) return 0;
             //   if (max >= ARR_SIZE)
                 //   Console.Out.WriteLine("H: Overshot. max is:" + max + " and the array is just:" + ARR_SIZE);//TODO - remove (debug)

                int retVal;
                double step = DECREASE_FACTOR / (ARR_SIZE - 3);//"-3" is meant to represent the fact that I'm not increasing the size of places 0,1 and i;
                double total = 0;
                max = Math.Min(max, ARR_SIZE - 1);
                double rand = _rand.NextDouble() * _hPlaces[max];
                for (retVal = 2; (rand > _hPlaces[retVal])&&(retVal<=max); ++retVal) ;
                /*for (int j = 2; j < ARR_SIZE; ++j)
                {
                    if (j == retVal)
                    {
                        total -= (DECREASE_FACTOR+step);
                        //MessageBox.Show("total = " + total+" STEP="+step);
                    }
                    total += step;
                    _hPlaces[j] += total;
                    if (_hPlaces[j] < 0) _hPlaces[j] = 0;
                }*/
                //TODO: decide what to do with the code above
                return retVal;
            }
        }
        /*********************************************************************************************
         * *************************************END OF INNER CLASS************************************
         * *******************************************************************************************/




        /********************************Constructor***************************************/
        public City(int gridL, int gridW)
        {
            //_rand = new Random();
            _bp = new BuildingPlacer();
            _len = gridL;
            _wid = gridW;
            _grid = new char[_len, _wid];
            _grid2 = new Tile[_len, _wid];
            for (int i = 0; i < _len; ++i)
                for (int j = 0; j < _wid; ++j)
                    _grid2[i, j] = new Tile();
            _corpList = new Corporate[(1+ (_len / CORP_DIM)), 1+(_wid / CORP_DIM)];
            for (int i = 0; i < (_len / CORP_DIM)+1 ; ++i)
                for (int j = 0; j < (_wid / CORP_DIM)+1 ; ++j)
                {
                    _corpList[i, j] = new Corporate();
                }
 
            _buildings = new List<Building>();
            
        }

        /********************************Properties***************************************/
        //TODO: remove.
        //this section exists in the "GameBoard" parent class. here are just the redundant things. 
        public char[,] getGrid() { return _grid; } //This is left here till later. TODO: remove.
        public short[][] getShortGrid()
        {
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



        /********************************Adding Roads To a City***************************************/
        
        /**
         * This method adds roads to a city by calling to "add main roads" recursively, first time with the whole grid, and after that it adds roads block by block.
         * */
        internal void addRoads()
        {
            List<Block> blocks = new List<Block>();
            addMainRoads(0, 0, _len, _wid, ref blocks);

            Block temp;
            int cond = blocks.Count;
            while (cond > 0)
            {
                temp = blocks[0];
                if (temp.Width * temp.Length > MIN_BLOCK_SIZE)
                {
                    addMainRoads(temp.StartX, temp.StartY, temp.Length, temp.Width, ref blocks);
                }
                blocks.RemoveAt(0);
                cond = blocks.Count;
            }
        }

        /**
         * this method adds a few roads that cross an entire "block" (whose dimensions are given as parameters)
         * */
        private void addMainRoads(int startX, int startY, int length, int width, ref List<Block> blocks)
        {

            int lenRoadsNum = 0, widRoadsNum = 0;
            //Random rand = new Random();

            //this gives us the max possible road width for both vertical(length) and horizontal(width) roads
            int maxLenRoad = (int)(Math.Log10(length));
            int maxWidRoad = (int)(Math.Log10(width));
            if (maxLenRoad > 0) lenRoadsNum = (width / maxLenRoad) / GAP_RATIO;
            if (maxWidRoad > 0) widRoadsNum = (length / maxWidRoad) / GAP_RATIO;

            List<int> lenRoads = new List<int>();
            List<int> widRoads = new List<int>();
            int gap;

            gap = maxWidRoad * GAP_RATIO;
            for (int i = 0; i < widRoadsNum; ++i)
            {
                lenRoads.Add(_rand.Next(maxWidRoad, gap - maxWidRoad) + (i * gap));
            }

            gap = maxLenRoad * GAP_RATIO;
            for (int i = 0; i < lenRoadsNum; ++i)
            {
                widRoads.Add(_rand.Next(maxLenRoad, gap - maxLenRoad) + (i * gap));
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
                    m = _rand.Next(1, maxWidRoad);
                    lenBlockEdge.Add(i);
                    lenBlockEdge.Add(i + m);
                    for (int j = 0; j < m; ++j)
                        for (int k = 0; k < width; ++k)
                            if ((startY + i + j < _len) && (startX + k) < _wid)
                            {
                                _grid[startY + i + j, startX + k] = ROAD_GENERIC;
                                if (_grid2[startY + i + j, startX + k].Type != ContentType.ROAD)
                                    _grid2[startY + i + j, startX + k] = new RoadTile();
                                ((RoadTile)_grid2[startY + i + j, startX + k]).addDirection(false);
                                ((RoadTile)_grid2[startY + i + j, startX + k]).HWidth = m;
                                ((RoadTile)_grid2[startY + i + j, startX + k]).HOffset = j;
                            }
                }
                lenBlockEdge.Add(length);
            }

            if (maxLenRoad >= 1) //this way the "rand.next()" function will not blow up
            {
                foreach (int i in widRoads)
                {
                    m = _rand.Next(1, maxLenRoad);
                    widBlockEdge.Add(i);
                    widBlockEdge.Add(i + m);
                    for (int j = 0; j < m; ++j)
                        for (int k = 0; k < length; ++k)
                        {
                            if ((startY + k < _len) && (startX + i + j) < _wid)
                            {//make sure we're not out of the grid
                                _grid[startY + k, startX + i + j] = ROAD_GENERIC;
                                if (_grid2[startY + k, startX + i + j].Type != ContentType.ROAD)
                                    _grid2[startY + k, startX + i + j] = new RoadTile();
                                ((RoadTile)_grid2[startY + k, startX + i + j]).addDirection(true);
                                ((RoadTile)_grid2[startY + k, startX + i + j]).VWidth = m;
                                ((RoadTile)_grid2[startY + k, startX + i + j]).VOffset = j;

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
                            Console.Error.WriteLine(startX + " ERROR!!! " + startY); //TODO - remove (debug)
                        else blocks.Add(new Block(widBlockEdge[j],lenBlockEdge[i], blockLength, blockwidth));
                        // Console.Out.WriteLine(lenBlockEdge[i]+"-"+lenBlockEdge[i+1]+"X"+widBlockEdge[j]+"-"+widBlockEdge[j+1]+" legnthXwid =" +blockLength+"X"+blockwidth);
                    }
                }
            }
        }


        /**
         * this method adds some info to the road tiles to determin thier type and their rotate attribute.
         * */
        internal void translateRoads()
        {
            RoadTile current;
            for (int i = 0; i < _len; ++i)
            {
                for (int j = 0; j < _wid; ++j)
                {
                    if (_grid2[i, j].Type == ContentType.ROAD)
                    {
                        current = (RoadTile)_grid2[i, j];
                        //set number of exits from the tile
                        if (isConnected(i, j, Directions.E, current.VOffset))
                            current.addExit();
                        if (isConnected(i, j, Directions.N, current.HOffset))
                            current.addExit();
                        if (isConnected(i, j, Directions.W, current.VWidth - current.VOffset - 1)) //the "-1" part is so that the offset will lead to the edge of the road
                            current.addExit();
                        if (isConnected(i, j, Directions.S, current.HWidth - current.HOffset - 1))
                            current.addExit();

                        //set rotate (note that "Rotate=0" is redundant, but it's done anyway.
                        // rotate 4 means error.
                        switch (current.Exits)
                        {
                            case 1:
                                if (isConnected(i, j, Directions.W, current.VOffset)) //EW begining
                                    current.Rotate = 0;
                                else if (isConnected(i, j, Directions.S, current.HOffset)) //NS begining
                                    current.Rotate = 1;
                                else if (isConnected(i, j, Directions.E, current.HOffset)) //EW end
                                    current.Rotate = 2;
                                else if (isConnected(i, j, Directions.N, current.HOffset)) //NS end
                                    current.Rotate = 3;
                                else current.Rotate = 4;
                                break;
                            case 2: //either 90 deg turn or none
                                //TODO: fix. I don't deal here with corners...
                                if (current.Direction == Directions.EW)
                                    current.Rotate = 1;
                                else if (current.Direction == Directions.NS) current.Rotate = 0;
                                break;
                            case 3:
                                if (!isConnected(i, j, Directions.W, current.VOffset))
                                { //3rd road to the East
                                    current.Rotate = 0;
                                    current.Direction = Directions.NS;
                                }
                                else if (!isConnected(i, j, Directions.S, current.HOffset))
                                { //3rd road to the North
                                    current.Rotate = 1;
                                    current.Direction = Directions.EW;
                                }
                                else if (!isConnected(i, j, Directions.E, current.HOffset))
                                { //3rd road to the West
                                    current.Rotate = 2;
                                    current.Direction = Directions.NS;
                                }
                                else if (!isConnected(i, j, Directions.N, current.HOffset))
                                { //3rd road to the South
                                    current.Rotate = 3;
                                    current.Direction = Directions.EW;
                                }
                                else current.Rotate = 4; break;
                            case 4: if (current.Direction == Directions.FOURWAY) current.Rotate = 0; else current.Rotate = 4;
                                break;
                            default: current.Rotate = 4; break;
                        }
                    }
                }
            }
        }




        /********************************Adding Buildings To a City***************************************/
        
        /**
         * Adds building to a city using the "add building" method.
         * */
        internal void addBuildings()
        {
            for (int i = 0; i < _len; ++i)
                for (int j = 0; j < _wid; ++j)
                    if (_grid2[i, j].Type == ContentType.EMPTY)
                        addBuilding(i, j); //j will be bigger in the width of the new building
            Console.Out.WriteLine("buildings num=" + _buildings.Count);
           connectBuildings2Roads();
            //_bp.print();
        }


        /*
         * creates a building, adds it to the buildings list
         * */
        private void addBuilding(int y, int x)
        {
            Block buildingSize;
            Building b;
            int length, width;
            width = 0;
            length = 0;
            for (; ((x + width < _wid) && (_grid2[y, x + width].Type == ContentType.EMPTY)); ++width) ;
            width--; //no matter why we've stopped, we need to go one step backwards
            for (; ((y + length < _len) && (_grid2[y + length, x + width].Type == ContentType.EMPTY)); ++length) ;
            length--;


            if (length < 1)
                return;
            if (width < 1)
                return;

            buildingSize = new Block(x, y, _bp.getVDimension(length), _bp.getHDimension(width));
            b = new Building(buildingSize);
            for (int i = y; i < y + buildingSize.Length; ++i)
                for (int j = x; j < x + buildingSize.Width; ++j)
                {
                    if (_grid2[i, j].Type != ContentType.EMPTY)
                        continue;
                    _grid[i, j] = c;
                    _grid2[i, j] = new BuildingTile(b);
                }
            _buildings.Add(b);
            c++;
            if (c > 'Z')
                c = 'A';

        }


        /**
         * this method is helping me find out whether a building is connected to a road in a certain direction.
         * */
        private bool isConnected(int x, int y, Directions dir, int offset)
        {
            switch (dir)
            {
                case Directions.E:
                    if (y - offset < 0) return false; //error. print?
                    if (y - offset == 0) return false; //legit.
                    if (_grid2[x, y - offset - 1].Type == ContentType.ROAD)
                        return true;
                    return false;

                case Directions.N:
                    if (x - offset < 0) return false; //error.
                    if (x - offset == 0) return false; //legit.
                    if (_grid2[x - offset - 1, y].Type == ContentType.ROAD)
                        return true;
                    return false;

                case Directions.W:
                    if (y + offset + 1 > _wid) return false; //error
                    if (y + offset + 1 == _wid) return false; //legit
                    return (_grid2[x, y + offset + 1].Type == ContentType.ROAD);
                case Directions.S:
                    if (x + offset + 1 > _wid) return false;//error
                    if (x + offset + 1 == _wid) return false;//legit
                    return (_grid2[x + offset + 1, y].Type == ContentType.ROAD);
                default: return false; //error
            }
        }

        /**
         * if there's a building that is one row remote from a road - enlarge this building so that it reaches a road.
         * */
        private void connectBuildings2Roads()
        {
            bool cond = true;
            bool connected;
            while (cond) //this while allows me to wreak havoc to the _buildings list without ruining my iterator.
            {
                cond = false;
                foreach (Building b in _buildings)
                {
                    connected = false;
                    if (b.StartY + b.Length < _len)
                    {
                        if (_grid2[b.StartY + b.Length, b.StartX].Type == ContentType.ROAD)
                        {
                            //Console.Out.WriteLine("(" + b.StartY + "," + b.StartX + ") ---ROAD Below");
                            //continue;
                            connected = true;
                        }
                    }

                    if (b.StartY > 0)
                    {
                        if (_grid2[b.StartY - 1, b.StartX].Type == ContentType.ROAD)
                        {
                           // Console.Out.WriteLine("("+b.StartY+","+b.StartX+") ---ROAD above");
                            //continue;
                            connected = true;
                        }
                    }
                    if (b.StartX > 0)
                    {
                        if (_grid2[b.StartY, b.StartX - 1].Type == ContentType.ROAD)
                        {
                            //Console.Out.WriteLine("(" + b.StartY + "," + b.StartX + ") ---ROAD to Left");
                            //continue;
                            connected = true;
                        }
                    }

                    if (b.StartX + b.Width < _wid)
                    {
                        if (_grid2[b.StartY, b.StartX + b.Width].Type == ContentType.ROAD)
                        {
                            // Console.Out.WriteLine("(" + b.StartY + "," + b.StartX + ") ---ROAD to Right");
                            //continue;
                            connected = true;
                        }
                    }
                    if (expandToRoad(b)||connected)
                        continue;
                
                   for (int i = 0; i < b.Length; ++i)
                        for (int j = 0; j < b.Width; ++j)
                        {
                            _grid2[b.StartY + i, b.StartX + j] = new Tile();
                            _grid[b.StartY + i, b.StartX + j] = '/';
                        }
                    _buildings.Remove(b);
                    cond = true;
                    break;
                }
            }
        }

        /**
         * this method connects a building to all roads within 1 step of it.
         * */
        private bool expandToRoad(Building b)
        {
            bool retVal = false;

            if (b.StartX + b.Width + 1 < _wid)
                if ((_grid2[b.StartY, b.StartX + b.Width].Type == ContentType.EMPTY) && (_grid2[b.StartY, b.StartX + b.Width + 1].Type == ContentType.ROAD))
                {
                    for (int i = b.StartY; i < b.StartY + b.Length; ++i)
                    {
                        if (_grid2[i, b.StartX + b.Width].Type != ContentType.EMPTY)
                            Console.Out.WriteLine("trying to overwrite (" + (b.StartY - 1) + "," + i + ")");
                        else
                        {
                            _grid2[i, b.StartX + b.Width] = new BuildingTile(b);
                            _grid[i, b.StartX + b.Width] = '#';//_grid[b.StartY, b.StartX];
                        }
                    }
                    b.Width++;
                    retVal = true;
                    
                }

            if (b.StartY > 1)
                if ((_grid2[b.StartY - 1, b.StartX].Type == ContentType.EMPTY) && (_grid2[b.StartY - 2, b.StartX].Type == ContentType.ROAD))
                {
                    for (int i = b.StartX; i < b.StartX + b.Width; ++i)
                    {
                        if (_grid2[b.StartY - 1, i].Type != ContentType.EMPTY)
                            Console.Out.WriteLine("trying to override (" + (b.StartY - 1) + "," + i + ")");
                        else
                        {
                            _grid2[b.StartY - 1, i] = new BuildingTile(b);
                            _grid[b.StartY - 1, i] = '#';//_grid[b.StartY, i];
                        }
                    }
                    b.Length++;
                    b.StartY--;
                    retVal = true;
                    
                }

            if (b.StartX > 1)
                if ((_grid2[b.StartY, b.StartX - 1].Type == ContentType.EMPTY) && (_grid2[b.StartY, b.StartX - 2].Type == ContentType.ROAD))
                {
                    for (int i = b.StartY; i < b.StartY + b.Length; ++i)
                    {
                        if (_grid2[i, b.StartX - 1].Type != ContentType.EMPTY)
                            Console.Out.WriteLine("trying to override (" + (b.StartY - 1) + "," + i + ")");
                        else
                        {
                            _grid2[i, b.StartX - 1] = new BuildingTile(b);
                            _grid[i, b.StartX - 1] = '#';// _grid[b.StartY, b.StartX];
                        }
                    }
                    b.Width++;
                    b.StartX--;
                    retVal =  true;
                
                }



            if (b.StartY + b.Length + 1 < _len)
                if ((_grid2[b.StartY + b.Length, b.StartX].Type == ContentType.EMPTY) && (_grid2[b.StartY + b.Length + 1, b.StartX].Type == ContentType.ROAD))
                {
                    for (int i = b.StartX; i < b.StartX + b.Width; ++i)
                    {
                        if (_grid2[b.StartY + b.Length, i].Type != ContentType.EMPTY)
                            Console.Out.WriteLine("trying to override (" + (b.StartY - 1) + "," + i + ")");
                        else
                        {
                            _grid2[b.StartY + b.Length, i] = new BuildingTile(b);
                            _grid[b.StartY + b.Length, i] = '#';//_grid[b.StartY, b.StartX];
                        }
                    }
                    b.Length++;
                    retVal = true;
                   
                }
            return retVal;
        }




        /********************************Adding Corporations To a City***************************************/
    /**
     * divides the buildings between corporates according to geographical location
     * TODO: not tested (waiting for graphical interface)
     * */
        internal void addCorporates() {
            Building b;
            foreach (Tile t in _grid2)
                if (t.Type == ContentType.BUILDING) {
                    b = ((BuildingTile)t).Building;
                    if (!b.hasCorp()) {
                        b.joinCorp(_corpList[(b.StartY + (b.Length / 2)) / CORP_DIM, (b.StartX + (b.Width / 2)) / CORP_DIM]);
                    }
                }
            for (int i = 0; i < BIG_CORPS; ++i)
                takeover(_rand.Next(_len / CORP_DIM), _rand.Next(_wid / CORP_DIM));
        }

        /**
         * it's a way to make some corporates bigger then the others.
         * */
        private void takeover(int iLoc, int jLoc) {

            for (int i = iLoc - 1; i <= iLoc + 1; ++i) {
                if (i < 0) continue;
                if (i>= _corpList.GetLength(0)) break;
                for (int j = jLoc - 1; j <= jLoc + 1; ++j) {
                    if (j < 0) continue;
                    if (j >= _corpList.GetLength(1)) break;
                    if (_rand.NextDouble() <TAKEOVER_CHANCE)
                    {
                        _corpList[iLoc, jLoc].takeover(_corpList[i, j]);
                        _corpList[i, j] = _corpList[iLoc, jLoc];
                    }
                }
            }
        }  
    }
}
