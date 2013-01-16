using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * Block is a data structure that holds rectangles. it does so by keeping the upper-left corner, a length and a depth. 
 * NOTE: when regarding matrixes, remember that X is the second coordinate (matrix[Y,X]).
 * */
namespace Game.City_Generator
{
    public class Block
    {

        public Block(int x, int y, int len, int dep)
        {
            StartX = x;
            StartY = y;
            Length = len;
            Depth = dep;
        }

        #region properties

        public int Length { get; set; }

        public int Depth { get; set; }

        public int StartY { get; set; }

        public int StartX { get; set; }

        #endregion


        /********************************Methods***************************************/
        
        /*
         * This function checks if two blocks are of equal size. 
         */
        public bool EqualSize(Block b)
        {
            return (Length == b.Length && Depth == b.Depth);
        }

    }
}
