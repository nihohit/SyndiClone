using System.Collections.Generic;

namespace Game.Logic.Entities {

  internal class Civilian : Person {

    #region consts

    private const int CIV_RUNNING_TIME = 100;

    #endregion consts

    #region fields

    private bool m_fleeing;
    private int m_timeRunning = 0;

    #endregion fields

    #region constructors

    public Civilian(Direction headed):
      base(CivReact, Affiliation.CIVILIAN, new List<Direction>(), headed) {
        m_fleeing = false;
      }

    #endregion constructors

    #region public methods

    public void RunningAway() {
      m_fleeing = true;
    }

    public static Reaction CivReact(IEnumerable<Entity> entities) {
      Entity threat = Targeters.ThreatTargeterHigh(entities, Affiliation.CIVILIAN);
      Reaction react;

      if (threat == null) //TODO - add ignoring cops
      {
        react = new IgnoreReaction();
      } else {
        react = new RunAwayReaction(threat);
      }

      return react;
    }

    public override bool DoesReact() {
      if (m_fleeing) {
        if (m_timeRunning < CIV_RUNNING_TIME) {
          m_timeRunning += ReactionTime;
          return false;
        } else {
          m_timeRunning = 0;
          m_fleeing = false;
        }
      }
      return base.DoesReact();
    }

    public override string ToString() {
      return "Civilian, " + base.ToString();
    }

    #endregion public methods
  }
}
