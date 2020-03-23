using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic.Entities {
  public class VisualEntityInformation {
    #region properties

    public EntityType Type { get; private set; }

    public SFML.Window.Vector2f Position { get; set; }

    public Affiliation VisibleLoyalty { get; set; }

    public Vector Size { get; private set; }

    public int EntityId { get; private set; }

    #endregion

    #region constructors

    public VisualEntityInformation(EntityType type, Affiliation visibleLoyalty, Vector size, int entityId, SFML.Window.Vector2f position) {
      Position = position;
      VisibleLoyalty = visibleLoyalty;
      Size = size;
      Type = type;
      EntityId = entityId;
    }

    public VisualEntityInformation(EntityType type, Affiliation visibleLoyalty, Vector size, int entityId) : this(type, visibleLoyalty, size, entityId, new SFML.Window.Vector2f(-1, -1)) { }

    #endregion

    #region comparison methods

    public override bool Equals(object obj) {
      if (obj is VisualEntityInformation) {
        return EntityId == ((VisualEntityInformation) obj).EntityId;
      } else return false;
    }

    public override int GetHashCode() {
      return base.GetHashCode();
    }

    #endregion
  }
}