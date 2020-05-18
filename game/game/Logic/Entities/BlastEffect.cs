using System.Collections.Generic;

namespace Game.Logic.Entities {

  public class BlastEffect : Shot {
    private static readonly Dictionary<BlastType, BlastEffect> s_blasts = new Dictionary<BlastType, BlastEffect>();

    public static BlastEffect Instance(BlastType type) {
      if (!s_blasts.ContainsKey(type)) {
        switch (type) {
          //missing types
        }
      }

      return s_blasts[type];
    }

    //TODO - do we really need to use this?
    public static BlastEffect Instance(int radius, ShotEffect effect, WasBlocked blocked, ShotType type) {
      return new BlastEffect(radius, effect, blocked, type);
    }

    protected BlastEffect(int radius, ShotEffect effect, WasBlocked blocked, ShotType type) : base(null, effect, blocked, type) {
      Radius = radius;
    }

    protected BlastEffect(BlastEffect blast, int radius, ShotEffect effect, WasBlocked blocked, ShotType type) : base(blast, effect, blocked, type) {
      Radius = radius;
    }

    public int Radius { get; private set; }
  }
}
