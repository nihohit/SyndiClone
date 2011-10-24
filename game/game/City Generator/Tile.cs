using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.City_Generator
{
    enum ContentType { ROAD, BUILDING, EMPTY, SPECIAL };
    class Tile
    {
        protected ContentType _type;
        protected int _rotate;

        protected ContentType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public Tile()
        {
            _type = ContentType.EMPTY;
            _rotate = 0;
        }
        public Block getFlip()
        {
            //uses block to hold two points to form a flip-line.
            //TODO
            return null;
        }
        public int getRotate() {
            return _rotate;
        }

        internal void setRotate(int rotate)
        {
            _rotate = rotate;
        }
        public ContentType getType() {
            return _type;
        }

    }
}
