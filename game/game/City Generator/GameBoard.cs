using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace Game.City_Generator {
  /**
   * Gameboard is the basic interface we will need for all kind of maps. the functions here are mostly info-getters. 
   * */
  public abstract class GameBoard {
    #region properties

    public Tile[, ] Grid { get; protected set; }

    public int Length { get; protected set; }

    public int Depth { get; protected set; }

    public Texture BoardImage { get; set; }

    public List<Building> Buildings { get; protected set; }

    public Corporate[, ] CorpList { get; protected set; }

    #endregion
  }
}