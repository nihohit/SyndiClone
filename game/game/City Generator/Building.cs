﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

/**
 * this class holds a bulding object - it has place on the initial grid (represented by a "block"). 
 * it can also preform basic operations (such as join a corporation). 
 * */
namespace Game.City_Generator
{
    class Building
    {
        //fields:
        Block _dimensions; //holding the location and dimensions of the building in the already existing block type;
        Corporate _corp;
        int _owner;
        Image _img;

       /********************************constructor***************************************/
        internal Building(Block dim) {
            _dimensions = dim;
            _corp = null;
            _owner = -1;
        }

        /********************************Properties***************************************/
        public int StartX { 
            get {return _dimensions.StartX;}
            set { _dimensions.StartX = value; }
        }

        public int StartY {
            get {return _dimensions.StartY;}
            set { _dimensions.StartY = value; }
        }

        public int Length {
            get { return _dimensions.Length;}
            set { _dimensions.Length = value; }
        }

        public int Width {
            get { return _dimensions.Width; }
            set { _dimensions.Width = value; }
        }

        public int Owner {
            get { return _owner; }
            set { _owner = value; }
        }

        internal Block Dimensions
        {
            get { return _dimensions; }
            set { _dimensions = value; }
        }

        public Corporate Corp{
            set { _corp = value;}
            get { return _corp; }
        }

        internal Image Img
        {
            get { return _img; }
            set { _img = value; }
        }


        /**********************************Simple methods*********************************/
        internal bool hasCorp() { return _corp != null; }

        internal void joinCorp(Corporate c)
        {
            if (_corp != null)
                _corp.removeBuilding(this);
            _corp = c;
            if (_corp != null)
                _corp.addBuilding(this);
        }
    }
}
