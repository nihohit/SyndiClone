using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.City_Generator
{
    class Building
    {
        Block _dimensions; //holding the location and dimensions of the building in the already existing block type;
       
        internal Building(Block dim) {
            _dimensions = dim;
        }

        public int StartX { 
            get {return _dimensions.StartX;}
        }

        public int StartY {
            get {return _dimensions.StartY;} 
        }

        public int Length {
            get { return _dimensions.Length; }
        }

        public int Width {
            get { return _dimensions.Width; }
        }
    }
}
