using System.Collections.Generic;

namespace Game.Logic.Entities {

  internal class Cop : Person, IShooter {

    #region static members

    static private Reaction copReact(IEnumerable<Entity> list) {
      Entity temp = Targeters.ThreatTargeterHigh(list, Affiliation.INDEPENDENT);
      if (temp == null) {
        return new IgnoreReaction();
      }
      return new PursueReaction(temp);
    }

    private static Entity copTargeter(IEnumerable<Entity> list) {
      return Targeters.ThreatTargeterHigh(list, Affiliation.INDEPENDENT);
    }

    private static Weapons copWeapon = Weapons.Instance(WeaponType.PISTOL);

    #endregion static members

    #region consts

    private const int COP_SHOOT_TIME = 2000;

    #endregion consts

    #region fields

    private int m_timeBeforeShot;
    private readonly PoliceStation m_station;

    #endregion fields

    #region constructors

    public Cop(PoliceStation station):
      base(copReact, Affiliation.INDEPENDENT, new List<Direction>(), station.Exit.VectorToDirection()) {
        m_station = station;
        m_timeBeforeShot = 0;
        List<Upgrades> list = new List<Upgrades>();
        list.Add(Upgrades.BULLETPROOF_VEST);
        base.Upgrade(list);
      }

    public Cop(PoliceStation station, List<Direction> path):
      base(copReact, Affiliation.INDEPENDENT, new List<Direction>(path), station.Exit.VectorToDirection()) {
        m_station = station;
        m_timeBeforeShot = 0;
        List<Upgrades> list = new List<Upgrades>();
        list.Add(Upgrades.BULLETPROOF_VEST);
        base.Upgrade(list);
      }

    #endregion constructors

    #region IShooter

    public Weapons Weapon() {
      return copWeapon;
    }

    public bool ReadyToShoot() {
      bool result = ReachAffect(COP_SHOOT_TIME, m_timeBeforeShot, copWeapon.RateOfFire);
      if (result) {
        m_timeBeforeShot -= COP_SHOOT_TIME;
      } else {
        m_timeBeforeShot += copWeapon.RateOfFire;
      }
      return result;
    }

    public Entity Target() {
      return ((ShootReaction) Reaction).Focus;
    }

    public targetChooser Targeter() {
      return copTargeter;
    }

    #endregion IShooter

    public override void Destroy() {
      base.Destroy();
      m_station.PolicemanDestroyed();
    }

    public override string ToString() {
      return "Cop, " + base.ToString();
    }
  }
}
