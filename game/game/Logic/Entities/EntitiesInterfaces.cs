using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic.Entities {
  public interface IConstructor {
    MovingEntity GetConstruct();

    bool ReadyToConstruct();
  }

  public interface ISelectable {
    SelectedEntityInformation Select(Logic.Affiliation playersLoyalty);
  }
}