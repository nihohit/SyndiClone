using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.City_Generator
{
    abstract class GameBoard
    {
        protected Tile[,] _grid2;

        protected List<Building> _buildings;
       // protected List<Corporate> _corps;
        protected Corporate[,] _corpList;
        /* HACK (amit)ans
         * (amit) I see no reason to keep a list of corporates, they are accessable through the building. do you want them for any reason?
         * (Shachar) : just ease of usage. In order to get all the corporations I'll need to iterate through all the buildings.
         * If we really see that corps are never accessed directly, we'll remove this.
         *         
         */
        protected int _len, _wid;


        internal Tile[,] Grid 
        {
            get { return _grid2; }
        }

        internal int Length
        {
            get { return _len; }
        }

        internal int Width
        {
            get { return _wid; }
        }

        internal List<Building> Buildings {
            get { return _buildings; }
        }

    }
}
