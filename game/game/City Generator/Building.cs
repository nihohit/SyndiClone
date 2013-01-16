using System;
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
    public class Building
    {
        #region properties

        public int Id { get; set; }

        public int Owner { get; set; }

        public Block Dimensions { get; set; }

        public Corporate Corp { get; set; }

        //TODO - known bug - sometimes the exit spot isn't on a road. Currently overridden elsewhere.
        public int ExitDirection { get; set; }

        #endregion

        #region constructor

        public Building(Block dim, int id) {
            Dimensions = dim;
            Corp = null;
            Owner = -1;
            ExitDirection = 0;
            Id = id;
        }

        #endregion

        #region public methods

        public bool HasCorp() { return Corp != null; }

        public void JoinCorp(Corporate c)
        {
            if (Corp != null)
                Corp.RemoveBuilding(this);
            Corp = c;
            if (Corp != null)
                Corp.AddBuilding(this);
        }

        #endregion
    }
}
