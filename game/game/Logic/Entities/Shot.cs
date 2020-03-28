using System.Collections.Generic;

namespace Game.Logic.Entities {

  public class Shot {
    private static Dictionary<ShotType, Shot> s_shots = new Dictionary<ShotType, Shot>();

    private const int PISTOL_BULLET_DAMAGE = 3;

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

    #endregion constructors

    #region properties

    public ShotType Type { get; set; }

    public BlastEffect Blast { get; set; }

    public ShotEffect Effect { get; set; }

    public WasBlocked Blocked { get; set; }

    #endregion properties

    #region shots effects

    private static void PistolBulletEffect(Entity ent) {
      ent.Hit(PISTOL_BULLET_DAMAGE);
    }

    #endregion shots effects

    #region blocked methods

    private static bool BulletBlocked(Entity ent) {
      if (ent == null) return false;
      return true;
    }

    #endregion blocked methods
  }
}
