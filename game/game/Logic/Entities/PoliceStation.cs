using System.Collections.Generic;

namespace Game.Logic.Entities {

  internal class PoliceStation : ConstructorBuilding {

    #region consts

    private const int POLICE_SIZE_MODIFIER = 3;

    #endregion consts

    #region fields

    private readonly int m_policemenCap;
    private int m_amountOfPolicemen;
    private Cop m_toConstruct;

    #endregion fields

    #region properties

    public bool Alert { get; set; }

    #endregion properties

    //TODO - set the whole alert operation. another idea - after alert wanes, begin "destroying" cops?

    #region cosntructor

    public PoliceStation(Game.Vector realSize, int sizeModifier, Vector exit) : base(sizeModifier, Entity.ReactionPlaceHolder, realSize, Affiliation.INDEPENDENT) {
      Path = new List<Direction>();
      base.Exit = exit;
      m_policemenCap = sizeModifier / POLICE_SIZE_MODIFIER;
      m_amountOfPolicemen = 0;
      Alert = false;
      reactionFunction react = delegate (List<Entity> ent) {
        if (m_toConstruct == null) m_toConstruct = new Cop(this, Path);
        m_readyToBuild = true;
        return new ConstructReaction(m_toConstruct);
      };
      ReactionFunction = react;
    }

    #endregion cosntructor

    #region public methods

    public override MovingEntity GetConstruct() {
      Cop temp = m_toConstruct;
      m_toConstruct = new Cop(this, Path);
      m_amountOfPolicemen++;
      return temp;
    }

    public override bool ReadyToConstruct() {
      if (Alert) {
        if (m_amountOfPolicemen < m_policemenCap * 3) {
          return base.ReadyToConstruct();
        } else //TODO - this is the only place where we remove the policestation's alert. should this be so?
        {
          Alert = false;
          return false;
        }
      } else {
        return (base.ReadyToConstruct() && (m_amountOfPolicemen <= m_policemenCap));
      }
    }

    public void PolicemanDestroyed() {
      m_amountOfPolicemen--;
      Alert = true;
    }

    public override string ToString() {
      return "Police station, " + base.ToString();
    }

    #endregion public methods
  }
}
