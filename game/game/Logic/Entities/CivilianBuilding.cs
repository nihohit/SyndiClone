using System.Collections.Generic;

namespace Game.Logic.Entities {

  public class CivilianBuilding : ConstructorBuilding, IConstructor {

    public CivilianBuilding(Game.Vector realSize, int sizeModifier, Vector exit) : base(sizeModifier, Entity.ReactionPlaceHolder, realSize, Affiliation.CIVILIAN) {
      base.Exit = exit;
      ReactionFunction = (IEnumerable<Entity> ent) => {
        Civilian temp = new Civilian(exit.VectorToDirection());
        m_readyToBuild = true;
        return new ConstructReaction(temp);
      }; 
    }

    #region public methods

    /*
     * This function is just the basic reaction function for the basic civic buildings.
     */

    public Reaction civBuildReact(List<Entity> ent) {
      Civilian temp = new Civilian(base.Exit.VectorToDirection());
      m_readyToBuild = true;
      return new ConstructReaction(temp);
    }

    public override MovingEntity GetConstruct() {
      return ((ConstructReaction)Reaction).Focus;
    }

    public override bool ReadyToConstruct() {
      return base.ReadyToConstruct();
    }

    public override string ToString() {
      return "Civilian Building, " + base.ToString();
    }

    #endregion public methods
  }
}
