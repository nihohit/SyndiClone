using System.Collections.Generic;
using Game.Logic;

namespace Game.Logic.Entities {
  public class BlastEffect : Shot {
    static Dictionary<BlastType, BlastEffect> s_blasts = new Dictionary<BlastType, BlastEffect>();

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