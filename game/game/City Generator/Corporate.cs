using System;
using System.Collections.Generic;
using System.Linq;

/**
 * a Corporate is a collection of buildings.
 * */

namespace Game.City_Generator {

  internal enum CorporateNames { ZOMBIE_LTD, BRAINS, UPGRADES, MINING, ARMORY, RND, DEFENCE, DIGGING } //TODO: figure out what kinds of corporates do we want.

  public class Corporate {

    #region static fields

    private static int s_counter = 0;
    private static Random s_random = new Random();

    #endregion static fields

    #region fields

    private CorporateNames m_type;

    #endregion fields

    #region constructor

    public Corporate() {
      m_type = (CorporateNames)s_random.Next(Enum.GetValues(typeof(CorporateNames)).Length);
      Id = s_counter;
      s_counter++;
      Buildings = new List<Building>();
    }

    #endregion constructor

    #region properties

    public List<Building> Buildings { get; private set; }

    public int Id { get; private set; }

    #endregion properties

    #region public methods

    public void AddBuilding(Building b) {
      Buildings.Add(b);
    }

    public void RemoveBuilding(Building b) {
      Buildings.Remove(b);
    }

    public bool CanBuild(Building b) {
      if (b.Corp != this) return false;
      foreach (Building other in Buildings)
        if (b.Owner != other.Owner)
          return false;
      return true;
    }

    /**
     * This method merges the "other" corporate into the current one.
     * after this method is done, other corporate will still exist, but will be empty (so it's better to remove him)
     * */

    public void Takeover(Corporate other) {
      if (other == this)
        return;
      //   Console.Out.WriteLine("merging!");
      //  counter--;
      while (other.Buildings.Count > 0)
        other.Buildings.First().JoinCorp(this);
    }

    #endregion public methods
  }
}
