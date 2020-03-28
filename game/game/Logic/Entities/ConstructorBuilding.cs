using System.Collections.Generic;

namespace Game.Logic.Entities {

  #region ConstructorBuilding

  public abstract class ConstructorBuilding : Building, IConstructor {
    protected const int AMOUNT_OF_STEPS_BEFORE_BUILDING = 2000;

    private int m_timeToConstruct;
    protected bool m_readyToBuild;

    public ConstructorBuilding(int sizeModifier, reactionFunction reaction, Vector size, Affiliation loyalty) : base(sizeModifier, reaction, size, loyalty) {
      Path = Path = new List<Direction>();
      m_timeToConstruct = 0;
      m_readyToBuild = true;
    }

    virtual public bool ReadyToConstruct() {
      bool ready = base.ReachAffect(AMOUNT_OF_STEPS_BEFORE_BUILDING, m_timeToConstruct, m_sizeModifier) && m_readyToBuild;
      if (ready) {
        m_timeToConstruct -= AMOUNT_OF_STEPS_BEFORE_BUILDING;
        m_readyToBuild = false;
      } else {
        m_timeToConstruct += m_sizeModifier;
      }
      return ready;
    }

    public abstract MovingEntity GetConstruct();

    public List<Direction> Path { get; set; }
  }

  #endregion ConstructorBuilding
}
