using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.City_Generator
{
    
    class CityFactory
    {
        private const int DEF_LEN =100;
        private const int DEF_WID=100; //default city length and width
        /**
         * Constructor
         */
        CityFactory() { 
        }

        /**
         * returns a city with some roads in it. in future, may also contain buildings
         */
        public static City createMap(int length=DEF_LEN,int width=DEF_WID){
            City retVal = new City(length,width);
            retVal.addRoads(length, width);
           // retVal.translateMap();



            return retVal;
        }
    }
}
