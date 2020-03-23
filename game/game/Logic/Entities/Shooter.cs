using System.Collections.Generic;

namespace Game.Logic.Entities {
  public interface IShooter {

    Weapons Weapon();

    bool ReadyToShoot();

    Entity Target();

    targetChooser Targeter();

  }
}