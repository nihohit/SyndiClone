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
           // Corporate.print();

            return retVal;
        }
    }
}
