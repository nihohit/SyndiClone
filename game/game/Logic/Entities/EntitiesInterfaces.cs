namespace Game.Logic.Entities {

  public interface IConstructor {

    MovingEntity GetConstruct();

    bool ReadyToConstruct();
  }

  public interface ISelectable {

    SelectedEntityInformation Select(Logic.Affiliation playersLoyalty);
  }
}
