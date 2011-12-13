using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace Game.City_Generator
{
    /**
     * Gameboard is the basic interface we will need for all kind of maps. the functions here are mostly info-getters. 
     * */
    abstract class GameBoard
    {
        /********************************Fields***************************************/
        protected Tile[,] _grid2; // this is the initial grid. TODO: after we'll have a graphical interface, change this to "_grid" and remove the textual one.

        protected List<Building> _buildings;
        protected Corporate[,] _corpList;
        protected Image _img;
        protected int _len, _wid;

        /********************************Properties***************************************/
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

        internal Image Img
        {
            get { return _img; }
            set { _img = value; }
        }

        internal List<Building> Buildings {
            get { return _buildings; }
        }

    }
}
