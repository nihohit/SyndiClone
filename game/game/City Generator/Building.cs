using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.City_Generator
{
    class Building
    {
        Block _dimensions; //holding the location and dimensions of the building in the already existing block type;
        Corporate _corp;
        int _owner; //TODO: decide how will "player" be represented. can be either a number or an object.
        /*
         * HACK (amit)ans
         * (amit) Q: see above - do you want player to be just a number?
         * (Shachar) 
         */
       
        internal Building(Block dim) {
            _dimensions = dim;
            _corp = null;
            _owner = -1;
        }

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

        public Corporate Corp{
            set { _corp = value;}
            get { return _corp; }
        }
    }
}
