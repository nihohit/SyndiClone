using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.City_Generator
{
    abstract class GameBoard
    {
        protected Tile[,] _grid2;
        /*HACK (Shachar) (ans)
         * Shachar: Tile doesn't hold the information I need. I need a pointer to a building if there is one in the tile, or 
         * null if not. 
         * (amit) I can do that (just did), but isn't is more safe (null-pointer) to force you to cast it to a BuildigTile? 
         *          also, object-oriented-wise, "building" is not a natural property of "tile" (of corse, if it's easier for you, theory can always be put aside.
         * 
         */
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
