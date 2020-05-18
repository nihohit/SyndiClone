using Vector2f = SFML.System.Vector2f;

namespace Game.Logic.Entities {

  public class VisualEntityInformation {

    #region properties

    public EntityType Type { get; private set; }

    public Vector2f Position { get; set; }

    public Affiliation VisibleLoyalty { get; set; }

    public Vector Size { get; private set; }

    public int EntityId { get; private set; }

    #endregion properties

    #region constructors

    public VisualEntityInformation(EntityType type, Affiliation visibleLoyalty, Vector size, int entityId, Vector2f position) {
      Position = position;
      VisibleLoyalty = visibleLoyalty;
      Size = size;
      Type = type;
      EntityId = entityId;
    }

    public VisualEntityInformation(EntityType type, Affiliation visibleLoyalty, Vector size, int entityId) : this(type, visibleLoyalty, size, entityId, new Vector2f(-1, -1)) { }

    #endregion constructors

    #region comparison methods

    public override bool Equals(object obj) {
      if (obj is VisualEntityInformation) {
        return EntityId == ((VisualEntityInformation) obj).EntityId;
      } else return false;
    }

    public override int GetHashCode() {
      return base.GetHashCode();
    }

    #endregion comparison methods
  }
}
