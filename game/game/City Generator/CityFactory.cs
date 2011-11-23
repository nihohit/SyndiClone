using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * Currently, city factory knows to give just one type of city. 
 * later it might be able to give other types (mazes?)
 * */
namespace Game.City_Generator
{
    
    static class CityFactory
    {
        /********************************Constants***************************************/
        private const int DEF_LEN =100;
        private const int DEF_WID=100; //default city length and width



        /********************************simple methods***************************************/

        /**
         * a city factory method.
         * */
        public static GameBoard createCity(int length=DEF_LEN,int width=DEF_WID){
            City retVal = new City(length,width);
            retVal.addRoads();
            retVal.translateRoads();
            retVal.addBuildings();
            retVal.addCorporates();
            retVal.Img = CityImageGenerator.convert_to_image(retVal);
            
           // Corporate.print();

            return retVal;
        }

        public static GameBoard debugCity() {
            City retVal = new City(10, 10);

           // retVal.Grid[2, 0] = new RoadTile();

            //retVal.Grid[1, 2] = new RoadTile();
            //((RoadTile)retVal.Grid[1, 2]).HOffset = 1;
            //((RoadTile)retVal.Grid[1, 2]).HWidth = 2;

            retVal.Grid[1, 1] = new RoadTile();
            ((RoadTile)retVal.Grid[1, 1]).VOffset = 0;
            ((RoadTile)retVal.Grid[1, 1]).VWidth = 2;
            retVal.Grid[1, 2] = new RoadTile();
            ((RoadTile)retVal.Grid[1, 2]).VOffset = 0;
            ((RoadTile)retVal.Grid[1, 2]).VWidth = 2;
            retVal.Grid[1, 3] = new RoadTile();
            ((RoadTile)retVal.Grid[1, 3]).VOffset = 0;
            ((RoadTile)retVal.Grid[1, 3]).VWidth = 2;
            retVal.Grid[2, 1] = new RoadTile();
            ((RoadTile)retVal.Grid[2, 1]).VOffset = 1;
            ((RoadTile)retVal.Grid[2, 1]).VWidth = 2;
            retVal.Grid[2, 2] = new RoadTile();
            ((RoadTile)retVal.Grid[2, 2]).VOffset = 1;
            ((RoadTile)retVal.Grid[2, 2]).VWidth = 2;
            retVal.Grid[2, 3] = new RoadTile();
            ((RoadTile)retVal.Grid[2, 3]).VOffset = 1;
            ((RoadTile)retVal.Grid[2, 3]).VWidth = 2;
            //retVal.Grid[3, 1] = new RoadTile();
            //((RoadTile)retVal.Grid[3, 1]).VOffset = 2;
            //((RoadTile)retVal.Grid[3, 1]).VWidth = 3;
            //retVal.Grid[3, 2] = new RoadTile();
            //((RoadTile)retVal.Grid[3, 2]).VOffset = 2;
            //((RoadTile)retVal.Grid[3, 2]).VWidth = 3;
            //retVal.Grid[3, 3] = new RoadTile();
            //((RoadTile)retVal.Grid[3, 3]).VOffset = 2;
            //((RoadTile)retVal.Grid[3, 3]).VWidth = 3;
            
            
           // retVal.Grid[0, 2] = new RoadTile();
            retVal.Grid[3, 2] = new RoadTile();
            
            
            //retVal.addRoads();
            retVal.translateRoads();
            for (int i = 0; i < retVal.Length; ++i)
            {
                for (int j = 0; j < retVal.Width; ++j)
                {
                    if (retVal.Grid[i, j].Type == ContentType.ROAD)
                        System.Console.Write('*');
                    else System.Console.Write(' ');
                }
                System.Console.Write('\n');
            }
            //System.Console.ReadKey();
            return retVal;
        
        }
    }
}
