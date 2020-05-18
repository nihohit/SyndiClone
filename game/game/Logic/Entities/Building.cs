using System.Collections.Generic;

namespace Game.Logic.Entities {

  public abstract class Building : Entity, ISelectable {

    #region consts

    protected const int BASE_BUILD_REACTION_TIME = 10;
    protected const int BASE_BUILD_HEALTH = 10;

    #endregion consts

    protected readonly int m_sizeModifier;

    public Vector Exit { get; set; }

    public Building(int sizeModifier, reactionFunction reaction, Vector size, Affiliation loyalty) : base(BASE_BUILD_REACTION_TIME * sizeModifier, reaction, BASE_BUILD_HEALTH * sizeModifier, EntityType.BUILDING, size, loyalty) {
      m_sizeModifier = sizeModifier;
      List<Upgrades> list = new List<Upgrades>();
      list.Add(Upgrades.VISIBILITY_SOLID);
      list.Add(Upgrades.BUILDING_BLIND);
      base.Upgrade(list);
    }

    public override void Destroy() {
      base.Destroy();
    }

    public SelectedEntityInformation Select(Affiliation playersLoyalty) {
      return new SelectedEntityInformation {
        Controlled = playersLoyalty == Loyalty
      };
    }
  }
}
