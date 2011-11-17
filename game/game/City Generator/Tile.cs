using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * tile is the basic Grid object. it holds any type of data required to build the map. 
 * note - just "Tile" is an empty tile. Make sure to replace it with a child object of the relevant type.
 * */
namespace Game.City_Generator
{
    enum ContentType { ROAD, BUILDING, EMPTY, SPECIAL };
    class Tile
    {
        /********************************Fields***************************************/
        protected ContentType _type;
        protected int _rotate;
        protected Building _building;

        /********************************Constructor***************************************/
        public Tile()
        {
            _type = ContentType.EMPTY;
            _rotate = 0;
            _building = null;
        }

        /********************************Properties***************************************/
        internal ContentType Type
        {
            get { return _type; }
            set { }//this way I'm hoping to make "type" immutable. HACK(amit): do you think it'll work?
        }

        internal virtual Building Building
        {
            get { return _building; }
            set { }
        }

        internal int Rotate {
            set { _rotate = value; }
            get { return _rotate; }
        }

        internal virtual Block Flip {
            get { return null; }//TODO: if this will be required, make something real here.
        }

    }
}
