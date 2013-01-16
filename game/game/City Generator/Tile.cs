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
    #region enumerators

    public enum ContentType { ROAD, BUILDING, EMPTY, SPECIAL };
    public enum Images {EMPTY,R_DEAD_END,R_CORNER, R_LINE,R_3WAY,R_FOURWAY,B_INNER,B_OUTER};//R means "road", B is building.
    public enum Directions { NORTH, SOUTH, EAST, WEST } //just for passing directions.

    #endregion

    public class Tile
    {
        #region constructors

        public Tile() : this (ContentType.EMPTY, null)
        { }

        public Tile(ContentType contentType) : this(contentType, null)
        { }

        public Tile(ContentType contentType, Building b)
        {
            // TODO: Complete member initialization
            Type = contentType;
            Building = b;
            Rotate = 0;
            TileImage = Images.EMPTY;
        }

        #endregion

        #region properties 

        public ContentType Type { get; private set; }

        public Building Building { get; private set; }

        public int Rotate { get; set; }

        public Images TileImage { get; protected set; }

        #endregion
    }
}
