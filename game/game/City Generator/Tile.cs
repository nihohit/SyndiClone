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
    internal enum ContentType { ROAD, BUILDING, EMPTY, SPECIAL };
    internal enum Images {EMPTY,R_DEAD_END,R_CORNER, R_LINE,R_3WAY,R_FOURWAY,B_INNER,B_OUTER};//R means "road", B is building.
    class Tile
    {
        /********************************Fields***************************************/
        protected ContentType _type;
        protected int _rotate;
        protected Building _building;
        protected Images _img;

        /********************************Constructor***************************************/
        public Tile()
        {
            _type = ContentType.EMPTY;
            _rotate = 0;
            _building = null;
            _img = Images.EMPTY;
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

        internal Images Image {
           get { return _img;}
            set { _img = value; }

        }

    }
}
