using Game.Logic;
using Game.Logic.Entities;

namespace Game.Buffers {
  #region enumerators 

  /// <summary>
  /// represents the different queues in the input buffer
  /// </summary>
  public enum InputModuleAccessors { Graphics, Sounds, Logic }

  /// <summary>
  /// This enumerator represents the different buffer types we use. 
  /// TODO - different buffers will use different events - might be useful to seperate to different enumerators.
  /// </summary>
  public enum BufferType { MOVE, SHOT, EXTERNAL_DESTROY, EXTERNAL_CREATE, PAUSE, SELECT, DESELECT, UNPAUSE, ENDGAME, MOUSEMOVE, SETPATH, UNIT_SELECT, DISPLAY_IMAGE, LOGIC_INTERNAL_CREATE, LOGIC_INTERNAL_DESTROY }

  #endregion

  #region IBufferEvent

  //this interface represents the basic buffer event - it just identifies itself, the rest is in the specific events.
  public interface IBufferEvent {
    BufferType Type();
  }

  #endregion

  #region buffer events

  #region PauseBufferEvent

  //this event pauses the game.
  public struct PauseBufferEvent : IBufferEvent {
    BufferType IBufferEvent.Type() {
      return BufferType.PAUSE;
    }
  }

  #endregion

  #region SetPathActionBufferEvent

  /// <summary>
  /// this event holds a point and a path from that point. 
  /// is used from logic to display buffer, to display the different paths. 
  /// </summary>
  public class SetPathActionBufferEvent : IBufferEvent {
    #region properties

    public System.Collections.Generic.List<Logic.Direction> Path { get; private set; }
    public SFML.Window.Vector2f Position { get; private set; }
    public VisualEntityInformation Entity { get; private set; }

    #endregion

    public SetPathActionBufferEvent(VisualEntityInformation ent, System.Collections.Generic.List<Logic.Direction> path, SFML.Window.Vector2f pos) {
      Path = path;
      Position = pos;
      Entity = ent;
    }

    BufferType IBufferEvent.Type() {
      return BufferType.SETPATH;
    }
  }

  #endregion

  #region CancelActionBufferEvent

  //this event represents a cancel action - deselection of units, etc. basically right click.
  public struct CancelActionBufferEvent : IBufferEvent {
    BufferType IBufferEvent.Type() {
      return BufferType.DESELECT;
    }
  }

  #endregion

  #region MouseSelectBufferEvent
  //this event represents a mouse left click on specific window coordinates.
  public struct MouseSelectBufferEvent : IBufferEvent {
    public Vector Coords { get; }

    public int PlayerId { get; }

    public MouseSelectBufferEvent(Vector coords, int playerId) {
      Coords = coords;
      PlayerId = playerId;
    }

    BufferType IBufferEvent.Type() {
      return BufferType.SELECT;
    }
  }

  #endregion

  #region UnitSelectBufferEvent

  public struct UnitSelectBufferEvent : IBufferEvent {
    private readonly VisualEntityInformation m_entInfo;
    private readonly SelectedEntityInformation m_selectedInfo;

    public Vector Coords { get; }
    public VisualEntityInformation VisibleInfo { get { return m_entInfo; } }
    public SelectedEntityInformation SelectedInfo { get { return m_selectedInfo; } }

    public UnitSelectBufferEvent(VisualEntityInformation ent, SelectedEntityInformation selectedInfo, Vector coords) {
      Coords = coords;
      m_entInfo = ent;
      m_selectedInfo = selectedInfo;
    }

    BufferType IBufferEvent.Type() {
      return BufferType.UNIT_SELECT;
    }
  }

  #endregion

  #region MouseMoveBufferEvent

  //this event represents mouse moving. used to display the cursor by the display manager.
  public struct MouseMoveBufferEvent : IBufferEvent {
    private readonly SFML.Window.Vector2f m_coords;

    BufferType IBufferEvent.Type() {
      return BufferType.MOUSEMOVE;
    }

    public MouseMoveBufferEvent(float x, float y) {
      m_coords = new SFML.Window.Vector2f(x, y);
    }

    public MouseMoveBufferEvent(SFML.Window.Vector2f vec) {
      m_coords = vec;
    }

    public SFML.Window.Vector2f Coords { get { return m_coords; } }
  }

  #endregion

  #region EndGameBufferEvent

  //this event ends a game. 
  public struct EndGameBufferEvent : IBufferEvent {
    BufferType IBufferEvent.Type() {
      return BufferType.ENDGAME;
    }
  }

  #endregion

  #region UnPauseBufferEvent

  public struct UnPauseBufferEvent : IBufferEvent {
    BufferType IBufferEvent.Type() {
      return BufferType.UNPAUSE;
    }
  }

  #endregion

  #region ShotBufferEvent

  //this event represents a shot being fired - entry, exit & type. used from logic to display.
  public class ShotBufferEvent : IBufferEvent {
    public ShotType Shot { get; private set; }
    public Point Target { get; private set; }
    public Point Exit { get; private set; }

    public ShotBufferEvent(ShotType shot, Point exit, Point entry) {
      Exit = exit;
      Target = entry;
      Shot = shot;
    }

    public BufferType Type() {
      return BufferType.SHOT;
    }
  }

  #endregion

  #region InternalDestroyBufferEvent

  //this event will be sent by the logic to the output buffers when an entity is destroyed.
  public struct InternalDestroyBufferEvent : IBufferEvent {
    private readonly Area m_area;
    private readonly Entity m_ent;

    public InternalDestroyBufferEvent(Area area, Entity ent) {
      m_area = area;
      m_ent = ent;
    }

    public Entity DestroyedEntity {
      get { return m_ent; }
    }

    public Area Area {
      get { return m_area; }
    }

    BufferType IBufferEvent.Type() {
      return BufferType.LOGIC_INTERNAL_DESTROY;
    }
  }

  #endregion

  #region ExternalDestroyBufferEvent

  //this event will be sent by the logic to the output buffers when an entity is destroyed.
  public struct ExternalDestroyBufferEvent : IBufferEvent {
    public ExternalDestroyBufferEvent(InternalDestroyBufferEvent internalDestroyed) {
      Area = internalDestroyed.Area;
      VisualInfo = internalDestroyed.DestroyedEntity.VisualInfo;
    }

    public VisualEntityInformation VisualInfo { get; }

    public Area Area { get; }

    BufferType IBufferEvent.Type() {
      return BufferType.EXTERNAL_DESTROY;
    }
  }

  #endregion

  #region ExternalCreateUnitBufferEvent

  //this event signifies the creation of a new entity.
  public class ExternalCreateUnitBufferEvent : IBufferEvent {
    public ExternalCreateUnitBufferEvent(InternalCreateUnitBufferEvent internalCreationEvent) {
      VisibleInfo = internalCreationEvent.CreatedEntity.VisualInfo;
      Location = internalCreationEvent.Location;
    }

    public Area Location { get; private set; }

    public VisualEntityInformation VisibleInfo { get; private set; }

    public BufferType Type() {
      return BufferType.EXTERNAL_CREATE;
    }
  }

  #endregion

  #region InternalCreateUnitBufferEvent

  //this event signifies the creation of a new entity.
  public class InternalCreateUnitBufferEvent : IBufferEvent {
    public InternalCreateUnitBufferEvent(Entity ent, Area location) {
      CreatedEntity = ent;
      Location = location;
    }

    public Area Location { get; private set; }

    public Entity CreatedEntity { get; private set; }

    public BufferType Type() {
      return BufferType.LOGIC_INTERNAL_CREATE;
    }
  }

  #endregion

  #region DisplayImageBufferEvent

  public class DisplayImageBufferEvent : IBufferEvent {
    public DisplayImageBufferEvent(SFML.Graphics.Sprite sprite) {
      Sprite = sprite;
    }

    public SFML.Graphics.Sprite Sprite { get; private set; }

    public BufferType Type() {
      return BufferType.DISPLAY_IMAGE;
    }
  }

  #endregion

  #endregion
}