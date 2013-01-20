using Game.Logic;
using Game.Logic.Entities;

namespace Game.Buffers
{
    #region enumerators 

    /// <summary>
    /// represents the different queues in the input buffer
    /// </summary>
    public enum InputModuleAccessors { Graphics, Sounds, Logic }

    /// <summary>
    /// This enumerator represents the different buffer types we use. 
    /// TODO - different buffers will use different events - might be useful to seperate to different enumerators.
    /// </summary>
    public enum BufferType { MOVE, SHOT, DESTROY, CREATE, PAUSE, SELECT, DESELECT, UNPAUSE, ENDGAME, MOUSEMOVE, SETPATH, UNIT_SELECT, DISPLAY_IMAGE }

    #endregion

    #region IBufferEvent

    //this interface represents the basic buffer event - it just identifies itself, the rest is in the specific events.
    public interface IBufferEvent
    {
        BufferType type();
    }

    #endregion

    #region buffer events

    #region PauseBufferEvent

    //this event pauses the game.
    public struct PauseBufferEvent : IBufferEvent
    {
        BufferType IBufferEvent.type()
        {
            return BufferType.PAUSE;
        }
    }

    #endregion

    #region SetPathActionBufferEvent

    /// <summary>
    /// this event holds a point and a path from that point. 
    /// is used from logic to display buffer, to display the different paths. 
    /// </summary>
    public class SetPathActionBufferEvent : IBufferEvent
    {
        #region properties

        public System.Collections.Generic.List<Logic.Direction> Path { get; private set; }
        public SFML.Window.Vector2f Position { get; private set; }
        public ExternalEntity Entity { get; private set; }

        #endregion

        public SetPathActionBufferEvent(ExternalEntity ent, System.Collections.Generic.List<Logic.Direction> path, SFML.Window.Vector2f pos)
        {
            Path = path;
            Position = pos;
            Entity = ent;
        }

        BufferType IBufferEvent.type()
        {
            return BufferType.SETPATH;
        }
    }

    #endregion

    #region CancelActionBufferEvent

    //this event represents a cancel action - deselection of units, etc. basically right click.
    public struct CancelActionBufferEvent : IBufferEvent
    {
        BufferType IBufferEvent.type()
        {
            return BufferType.DESELECT;
        }
    }

    #endregion

    #region MouseSelectBufferEvent
    //this event represents a mouse left click on specific window coordinates.
    public struct MouseSelectBufferEvent : IBufferEvent
    {
        private readonly Vector m_coords;

        public Vector Coords
        {
            get { return m_coords; }
        } 

        public MouseSelectBufferEvent(Vector coords)
        {
            m_coords = coords;
        }
        
        BufferType IBufferEvent.type()
        {
            return BufferType.SELECT;
        }
    }

    #endregion

    #region UnitSelectBufferEvent

    public struct UnitSelectBufferEvent : IBufferEvent
    {
        private readonly Vector m_coords;
        private readonly ExternalEntity m_ent;

        public Vector Coords { get { return m_coords; } }
        public ExternalEntity Ent { get { return m_ent; } }

        public UnitSelectBufferEvent(ExternalEntity ent, Vector coords)
        {
            m_coords = coords;
            m_ent = ent;
        }

        BufferType IBufferEvent.type()
        {
            return BufferType.UNIT_SELECT;
        }
    }

    #endregion

    #region MouseMoveBufferEvent

    //this event represents mouse moving. used to display the cursor by the display manager.
    public struct MouseMoveBufferEvent : IBufferEvent
    {
        private readonly SFML.Window.Vector2f m_coords;
        
        BufferType IBufferEvent.type()
        {
            return BufferType.MOUSEMOVE;
        }

        public MouseMoveBufferEvent(float x, float y)
        {
            m_coords = new SFML.Window.Vector2f(x, y);
        }

        public MouseMoveBufferEvent(SFML.Window.Vector2f vec)
        {
            m_coords = vec;
        }

        public SFML.Window.Vector2f Coords { get { return m_coords ;}}
    }

    #endregion

    #region EndGameBufferEvent

    //this event ends a game. 
    public struct EndGameBufferEvent : IBufferEvent
    {
        BufferType IBufferEvent.type()
        {
            return BufferType.ENDGAME;
        }
    }

    #endregion

    #region UnPauseBufferEvent

    public struct UnPauseBufferEvent : IBufferEvent
    {
        BufferType IBufferEvent.type()
        {
            return BufferType.UNPAUSE;
        }
    }

    #endregion

    #region ShotBufferEvent

    //this event represents a shot being fired - entry, exit & type. used from logic to display.
    public class ShotBufferEvent : IBufferEvent
    {
        public ShotType Shot { get; private set; }
        public Point Target { get; private set; }
        public Point Exit { get; private set; }

        public ShotBufferEvent(ShotType shot, Point exit, Point entry)
        {
            Exit = exit;
            Target = entry;
            Shot = shot;
        }

        public BufferType type()
        {
            return BufferType.SHOT;
        }
    }

    #endregion

    #region DestroyBufferEvent

    //this event will be sent by the logic to the output buffers when an entity is destroyed.
    public struct DestroyBufferEvent : IBufferEvent
    {
        private readonly Area m_area;
        private readonly ExternalEntity m_ent;

        public DestroyBufferEvent(Area area, ExternalEntity ent)
        {
            m_area = area;
            m_ent = ent;
        }

        public ExternalEntity Ent
        {
            get { return m_ent; }
        }

        public Area Area
        {
            get { return m_area; }
        }

        BufferType IBufferEvent.type()
        {
            return BufferType.DESTROY;
        }
    }

    #endregion

    #region CreateUnitBufferEvent

    //this event signifies the creation of a new entity.
    public struct CreateUnitBufferEvent : IBufferEvent
    {
        private readonly ExternalEntity m_mover;
        private readonly Area m_location;

        public CreateUnitBufferEvent(ExternalEntity mover, Area location)
        {
            m_mover = mover;
            m_location = location;
        }

        public Area Location
        {
            get { return m_location; }
        }

        public ExternalEntity Mover
        {
            get { return m_mover; }
        }

        public BufferType type()
        {
            return BufferType.CREATE;
        }
    }

    #endregion

    #region DisplayImageBufferEvent

    public class DisplayImageBufferEvent : IBufferEvent
    {
        public DisplayImageBufferEvent(SFML.Graphics.Sprite sprite)
        {
            Sprite = sprite;
        }

        public SFML.Graphics.Sprite Sprite { get; private set; }

        public BufferType type()
        {
            return BufferType.DISPLAY_IMAGE;
        }
    }

    #endregion

    #endregion
}
