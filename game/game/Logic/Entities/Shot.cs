using System.Collections.Generic;
using Game.Logic;

namespace Game.Logic.Entities {
  public class Shot {
    static Dictionary<ShotType, Shot> s_shots = new Dictionary<ShotType, Shot>();

    const int PISTOL_BULLET_DAMAGE = 3;

    #region constructors

    public static Shot instance(ShotType type) {
      if (!s_shots.ContainsKey(type)) {
        switch (type) {
        case (ShotType.SIGHT):
          break;
        case (ShotType.PISTOL_BULLET):
          s_shots.Add(type, new Shot(null, PistolBulletEffect, BulletBlocked, type));
          break;
          //TODO - missing types

        }
      }

      return s_shots[type];
    }

    protected Shot(BlastEffect blast, ShotEffect effect, WasBlocked blocked, ShotType type) {
      Blast = blast;
      Blocked = blocked;
      Effect = effect;
      Type = type;
    }

    #endregion

    #region properties

    public ShotType Type { get; set; }

    public BlastEffect Blast { get; set; }

    public ShotEffect Effect { get; set; }

    public WasBlocked Blocked { get; set; }

    #endregion

    #region shots effects

    static void PistolBulletEffect(Entity ent) {
      ent.Hit(PISTOL_BULLET_DAMAGE);
    }

    #endregion

    #region blocked methods

    static bool BulletBlocked(Entity ent) {
      if (ent == null) return false;
      return true;
    }

    #endregion
  }
}