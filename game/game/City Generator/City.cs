using System;
using System.Collections.Generic;


/**
 * It's a city. a pre-processing phase city, but a city nonetheless.
 * here we make most of the processing needed to create a city. 
 * the phases of creating a city are done by this order:
 * 1) Create a city object.
 * 2) Add roads to the city.
 * 3) Collect data ("translate") about the roads created - direction, depth etc. store it in the road-tiles
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


        /********************************Fields***************************************/
        private char[,] _grid;
        private BuildingPlacer _bp;
        

        /****************************************************************************************************************
         * ***************************************INNER CLASS************************************************************
         * **************************************************************************************************************/
        /*
         * this class is a tool that helps me create new buildings.
         * By theory, it was meant to keep a changing probability to decide the length and depth of the building considering the past, so the more buildings there are of a certain
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

                //double step = DECREASE_FACTOR / (ARR_SIZE - 3);//"-3" is meant to represent the fact that I'm not increasing the size of places 0,1 and i;
                //double total = 0;
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
                //double step = DECREASE_FACTOR / (ARR_SIZE - 3);//"-3" is meant to represent the fact that I'm not increasing the size of places 0,1 and i;
                //double total = 0;
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
            _img = null;
            _bp = new BuildingPlacer();
            _len = gridL;
            _dep = gridW;
            _grid = new char[_len, _dep];
            _grid2 = new Tile[_len, _dep];
            for (int i = 0; i < _len; ++i)
                for (int j = 0; j < _dep; ++j)
                    _grid2[i, j] = new Tile();
            _corpList = new Corporate[(1+ (_len / CORP_DIM)), 1+(_dep / CORP_DIM)];
            for (int i = 0; i < (_len / CORP_DIM)+1 ; ++i)
                for (int j = 0; j < (_dep / CORP_DIM)+1 ; ++j)
                {
                    _corpList[i, j] = new Corporate();
                }
 
            _buildings = new List<Building>();
            
        }

        /********************************Properties***************************************/


        /********************************Adding Roads To a City***************************************/
        
        /**
         * This method adds roads to a city by calling to "add main roads" recursively, first time with the whole grid, and after that it adds roads block by block.
         * */
        internal void addRoads()
        {
            List<Block> blocks = new List<Block>();
            addMainRoads(0, 0, _len, _dep, ref blocks);

            Block temp;
            int cond = blocks.Count;
            while (cond > 0)
            {
                temp = blocks[0];
                if (temp.Depth * temp.Length > MIN_BLOCK_SIZE)
                {
                    addMainRoads(temp.StartX, temp.StartY, temp.Length, temp.Depth, ref blocks);
                }
                blocks.RemoveAt(0);
                cond = blocks.Count;
            }
        }

        /**
         * this method adds a few roads that cross an entire "block" (whose dimensions are given as parameters)
         * */
        private void addMainRoads(int startX, int startY, int length, int depth, ref List<Block> blocks)
        {

            int lenRoadsNum = 0, depRoadsNum = 0;
            //Random rand = new Random();

            //this gives us the max possible road depth for both vertical(length) and horizontal(depth) roads
            int maxLenRoad = (int)(Math.Log10(length));
            int maxDepRoad = (int)(Math.Log10(depth));
            if (maxLenRoad > 0) lenRoadsNum = (depth / maxLenRoad) / GAP_RATIO;
            if (maxDepRoad > 0) depRoadsNum = (length / maxDepRoad) / GAP_RATIO;

            List<int> lenRoads = new List<int>();
            List<int> depRoads = new List<int>();
            int gap;

            gap = maxDepRoad * GAP_RATIO;
            for (int i = 0; i < depRoadsNum; ++i)
            {
                lenRoads.Add(_rand.Next(maxDepRoad, gap - maxDepRoad) + (i * gap));
            }

            gap = maxLenRoad * GAP_RATIO;
            for (int i = 0; i < lenRoadsNum; ++i)
            {
                depRoads.Add(_rand.Next(maxLenRoad, gap - maxLenRoad) + (i * gap));
            }

            List<int> lenBlockEdge = new List<int>();
            List<int> depBlockEdge = new List<int>();
            int m;
            lenBlockEdge.Add(0);
            depBlockEdge.Add(0);
            if (maxDepRoad >= 1) //this way the "rand.next()" function will not blow up
            {
                foreach (int i in lenRoads)
                {
                    m = _rand.Next(1, maxDepRoad);
                    lenBlockEdge.Add(i);
                    lenBlockEdge.Add(i + m);
                    for (int j = 0; j < m; ++j)
                        for (int k = 0; k < depth; ++k)
                            if ((startY + i + j < _len) && (startX + k) < _dep)
                            {
                                _grid[startY + i + j, startX + k] = ROAD_GENERIC;
                                if (_grid2[startY + i + j, startX + k].Type != ContentType.ROAD)
                                    _grid2[startY + i + j, startX + k] = new RoadTile();
                                ((RoadTile)_grid2[startY + i + j, startX + k]).HDepth = m;
                                ((RoadTile)_grid2[startY + i + j, startX + k]).HOffset = j;
                            }
                }
                lenBlockEdge.Add(length);
            }

            if (maxLenRoad >= 1) //this way the "rand.next()" function will not blow up
            {
                foreach (int i in depRoads)
                {
                    m = _rand.Next(1, maxLenRoad);
                    depBlockEdge.Add(i);
                    depBlockEdge.Add(i + m);
                    for (int j = 0; j < m; ++j)
                        for (int k = 0; k < length; ++k)
                        {
                            if ((startY + k < _len) && (startX + i + j) < _dep)
                            {//make sure we're not out of the grid
                                _grid[startY + k, startX + i + j] = ROAD_GENERIC;
                                if (_grid2[startY + k, startX + i + j].Type != ContentType.ROAD)
                                    _grid2[startY + k, startX + i + j] = new RoadTile();
                                ((RoadTile)_grid2[startY + k, startX + i + j]).VDepth = m;
                                ((RoadTile)_grid2[startY + k, startX + i + j]).VOffset = j;
                            }
                        }
                }
                depBlockEdge.Add(depth);
            }
            // List<Block> blocks = new List<Block>();
            if (blocks.Count == 0)
            {
                int blockLength = 0, blockdepth = 0;
                for (int i = 0; i < lenBlockEdge.Count - 1; i += 2)
                {
                    for (int j = 0; j < depBlockEdge.Count - 1; j += 2)
                    {
                        if (lenBlockEdge.Count > 1)
                            blockLength = lenBlockEdge[i + 1] - lenBlockEdge[i];
                        else blockLength = length;

                        if (depBlockEdge.Count > 1)
                            blockdepth = depBlockEdge[j + 1] - depBlockEdge[j];
                        else blockdepth = depth;
                        if ((blockdepth < 0) || (blockLength < 0))
                            Console.Error.WriteLine(startX + " ERROR!!! " + startY); //TODO - remove (debug)
                        else blocks.Add(new Block(depBlockEdge[j],lenBlockEdge[i], blockLength, blockdepth));
                        // Console.Out.WriteLine(lenBlockEdge[i]+"-"+lenBlockEdge[i+1]+"X"+depBlockEdge[j]+"-"+depBlockEdge[j+1]+" legnthXdep =" +blockLength+"X"+blockdepth);
                    }
                }
            }
        }


        /**
         * this method adds some info to the road tiles to determine thier type and their rotate attribute.
         * */
        internal void translateRoads()
        {
            RoadTile current;
            for (int j = 0; j < _dep; ++j)
          
            {
                for (int i = 0; i < _len; ++i)
                {
                    if (_grid2[i, j].Type == ContentType.ROAD)
                    {
                        _grid2[i, j].Rotate = 0;
                        //System.Console.WriteLine("new point: (" + i + "," + j + ")");

                        current = (RoadTile)_grid2[i, j];
                        //set number of exits from the tile

                        if (isConnected(i, j, Directions.N, current.HOffset))
                        {
                            current.addExit(Directions.N);
                            //System.Console.WriteLine("North!");
                        }
                        if (isConnected(i, j, Directions.W, current.VOffset))
                        {
                            current.addExit(Directions.W);
                            //System.Console.WriteLine("West!");
                        }
                        if (isConnected(i, j, Directions.S, current.HDepth - current.HOffset - 1))//the "-1" part is so that the offset will lead to the edge of the road
                        {
                            current.addExit(Directions.S);
                            //System.Console.WriteLine("South!");
                        }
                        if (isConnected(i, j, Directions.E, current.VDepth - current.VOffset - 1))
                        {
                            current.addExit(Directions.E);
                            //System.Console.WriteLine("East!");
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
            int id = 0;
            for (int i = 0; i < _len; ++i)
                for (int j = 0; j < _dep; ++j)
                    if (_grid2[i, j].Type == ContentType.EMPTY)
                    {
                        addBuilding(i, j, id); //j will be bigger in the depth of the new building
                        id++;
                    }
            //Console.Out.WriteLine("buildings num=" + _buildings.Count);
           connectBuildings2Roads();
            //_bp.print();
        }


        /*
         * creates a building, adds it to the buildings list
         * */
        private void addBuilding(int y, int x, int id)
        {
            Block buildingSize;
            Building b;
            int length, depth;
            depth = 0;
            length = 0;
            for (; ((x + depth < _dep) && (_grid2[y, x + depth].Type == ContentType.EMPTY)); ++depth) ;
            depth--; //no matter why we've stopped, we need to go one step backwards
            for (; ((y + length < _len) && (_grid2[y + length, x + depth].Type == ContentType.EMPTY)); ++length) ;
            length--;


            if (length < 1)
                return;
            if (depth < 1)
                return;

            buildingSize = new Block(x, y, _bp.getVDimension(length), _bp.getHDimension(depth));
            b = new Building(buildingSize, id);
            for (int i = y; i < y + buildingSize.Length; ++i)
                for (int j = x; j < x + buildingSize.Depth; ++j)
                {
                    if (_grid2[i, j].Type != ContentType.EMPTY)
                        continue;
                    _grid2[i, j] = new BuildingTile(b);
                }
            _buildings.Add(b);

        }


        /**

* this method is helping me find out whether a RoadTile is connected to another road in a certain direction.

* */

        private bool isConnected(int y, int x, Directions dir, int offset)
        {

            switch (dir)
            {

                case Directions.N:

                    if (x - offset < 0) return false; //error. print?

                    if (x - offset == 0) return false; //legit.

                    if (_grid2[y, x - offset - 1].Type == ContentType.ROAD)

                        return true;

                    return false;

                case Directions.W:

                    if (y - offset < 0) return false; //error.

                    if (y - offset == 0) return false; //legit.

                    if (_grid2[y - offset - 1, x].Type == ContentType.ROAD)

                        return true;

                    return false;

                case Directions.S:

                    if (x + offset + 1 > _dep) return false; //error

                    if (x + offset + 1 == _dep) return false; //legit

                    return (_grid2[y, x + offset + 1].Type == ContentType.ROAD);

                case Directions.E:

                    if (y + offset + 1 > _len) return false;//error

                    if (y + offset + 1 == _len) return false;//legit

                    return (_grid2[y + offset + 1, x].Type == ContentType.ROAD);

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
                            b.ExitDirection = 3;
                            connected = true;
                        }
                    }

                    if (b.StartY > 0)
                    {
                        if (_grid2[b.StartY - 1, b.StartX].Type == ContentType.ROAD)
                        {
                           // Console.Out.WriteLine("("+b.StartY+","+b.StartX+") ---ROAD above");
                            //continue;
                            b.ExitDirection = 2;
                            connected = true;
                        }
                    }
                    if (b.StartX > 0)
                    {
                        if (_grid2[b.StartY, b.StartX - 1].Type == ContentType.ROAD)
                        {
                            //Console.Out.WriteLine("(" + b.StartY + "," + b.StartX + ") ---ROAD to Left");
                            //continue;
                            b.ExitDirection = 0;
                            connected = true;
                        }
                    }

                    if (b.StartX + b.Depth < _dep)
                    {
                        if (_grid2[b.StartY, b.StartX + b.Depth].Type == ContentType.ROAD)
                        {
                            // Console.Out.WriteLine("(" + b.StartY + "," + b.StartX + ") ---ROAD to Right");
                            //continue;
                            b.ExitDirection = 1;
                            connected = true;
                        }
                    }
                    if (expandToRoad(b)||connected)
                        continue;
                
                   for (int i = 0; i < b.Length; ++i)
                        for (int j = 0; j < b.Depth; ++j)
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

            if (b.StartX + b.Depth + 1 < _dep)
                if ((_grid2[b.StartY, b.StartX + b.Depth].Type == ContentType.EMPTY) && (_grid2[b.StartY, b.StartX + b.Depth + 1].Type == ContentType.ROAD))
                {
                    for (int i = b.StartY; i < b.StartY + b.Length; ++i)
                    {
                        if (_grid2[i, b.StartX + b.Depth].Type != ContentType.EMPTY)
                            Console.Out.WriteLine("trying to overwrite (" + (b.StartY - 1) + "," + i + ")");
                        else
                        {
                            _grid2[i, b.StartX + b.Depth] = new BuildingTile(b);
                            _grid[i, b.StartX + b.Depth] = '#';//_grid[b.StartY, b.StartX];
                        }
                    }
                    b.Depth++;
                    retVal = true;
                    
                }

            if (b.StartY > 1)
                if ((_grid2[b.StartY - 1, b.StartX].Type == ContentType.EMPTY) && (_grid2[b.StartY - 2, b.StartX].Type == ContentType.ROAD))
                {
                    for (int i = b.StartX; i < b.StartX + b.Depth; ++i)
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
                    b.Depth++;
                    b.StartX--;
                    retVal =  true;
                
                }



            if (b.StartY + b.Length + 1 < _len)
                if ((_grid2[b.StartY + b.Length, b.StartX].Type == ContentType.EMPTY) && (_grid2[b.StartY + b.Length + 1, b.StartX].Type == ContentType.ROAD))
                {
                    for (int i = b.StartX; i < b.StartX + b.Depth; ++i)
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
                        b.joinCorp(_corpList[(b.StartY + (b.Length / 2)) / CORP_DIM, (b.StartX + (b.Depth / 2)) / CORP_DIM]);
                    }
                }
            for (int i = 0; i < BIG_CORPS; ++i)
                takeover(_rand.Next(_len / CORP_DIM), _rand.Next(_dep / CORP_DIM));
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
