using Game.Logic;
using Game.Logic.Entities;

namespace Game.Buffers
{
    /*This enumerator represents the different buffer types we use. 
     * TODO - different buffers will use different events - might be useful to seperate to different enumerators.
     */
    internal enum BufferType { MOVE, SHOT, DESTROY, CREATE, PAUSE, SELECT, DESELECT, UNPAUSE, ENDGAME, MOUSEMOVE, SETPATH, UNIT_SELECT } 

    //this interface represents the basic buffer event - it just identifies itself, the rest is in the specific events.
    internal interface BufferEvent
    {
        BufferType type();
    }

    //this event pauses the game.
    internal struct PauseEvent : BufferEvent
    {
        BufferType BufferEvent.type()
        {
            return BufferType.PAUSE;
        }
    }

    /*this event holds a point and a path from that point. 
     * is used from logic to display buffer, to display the different paths. 
     */
    internal struct BufferSetPathActionEvent : BufferEvent
    {
        BufferType BufferEvent.type()
        {
            return BufferType.SETPATH;
        }

        private readonly System.Collections.Generic.List<Logic.Direction> _path;
        private readonly SFML.Window.Vector2f _position;
        private readonly ExternalEntity _ent;

        internal BufferSetPathActionEvent(ExternalEntity ent, System.Collections.Generic.List<Logic.Direction> path, SFML.Window.Vector2f pos)
        {
            this._path = path;
            this._position = pos;
            this._ent = ent;
        }

        public System.Collections.Generic.List<Logic.Direction> Path
        {
            get { return _path; }
        }

        public SFML.Window.Vector2f Position
        {
            get { return _position; }
        }

        public ExternalEntity Ent { get { return _ent; } }
    }

    //this event represents a cancel action - deselection of units, etc. basically right click.
    internal struct BufferCancelActionEvent : BufferEvent
    {
        BufferType BufferEvent.type()
        {
            return BufferType.DESELECT;
        }
    }

    //this event represents a mouse left click on specific window coordinates.
    internal struct BufferMouseSelectEvent : BufferEvent
    {
        private readonly Vector coords;

        internal Vector Coords
        {
            get { return coords; }
        } 

        internal BufferMouseSelectEvent(Vector _coords)
        {
            this.coords = _coords;
        }
        
        BufferType BufferEvent.type()
        {
            return BufferType.SELECT;
        }

    }

    internal struct BufferUnitSelectEvent : BufferEvent
    {
        private readonly Vector coords;
        private readonly ExternalEntity _ent;

        internal Vector Coords
        {
            get { return coords; }
        }

        internal BufferUnitSelectEvent(ExternalEntity ent, Vector _coords)
        {
            this.coords = _coords;
            this._ent = ent;
        }

        public ExternalEntity Ent { get { return _ent; } }

        BufferType BufferEvent.type()
        {
            return BufferType.UNIT_SELECT;
        }

    }

    //this event represents mouse moving. used to display the cursor by the display manager.
    internal struct BufferMouseMoveEvent : BufferEvent
    {
        private readonly SFML.Window.Vector2f coords;
        
        BufferType BufferEvent.type()
        {
            return BufferType.MOUSEMOVE;
        }

        public BufferMouseMoveEvent(float _x, float _y)
        {
            coords = new SFML.Window.Vector2f(_x, _y);
        }

        public BufferMouseMoveEvent(SFML.Window.Vector2f vec)
        {
            coords = vec;
        }

        public SFML.Window.Vector2f Coords { get { return this.coords ;}}
    }

    //this event ends a game. 
    internal struct EndGameEvent : BufferEvent
    {
        BufferType BufferEvent.type()
        {
            return BufferType.ENDGAME;
        }
    }

    internal struct UnPauseEvent : BufferEvent
    {
        BufferType BufferEvent.type()
        {
            return BufferType.UNPAUSE;
        }
    }

    //this event represents a shot being fired - entry, exit & type. used from logic to display.
    internal struct ShotEvent : BufferEvent
    {
        private readonly ShotType _shot;
        private readonly Point _exit;
        private readonly Point _target;

        internal ShotEvent(ShotType shot, Point exit, Point entry)
        {
            this._exit = exit;
            this._target = entry;
            this._shot = shot;
        }

        internal ShotType Shot
        {
            get { return _shot; }
        }

        internal Point Target
        {
            get { return _target; }
        }

        internal Point Exit
        {
            get { return _exit; }
        }

        public BufferType type()
        {
            return BufferType.SHOT;
        }

    }

    //this event will be sent by the logic to the output buffers when an entity is destroyed.
    internal struct DestroyEvent : BufferEvent
    {
        private readonly Area _area;
        private readonly ExternalEntity _ent;

        internal DestroyEvent(Area area, ExternalEntity ent)
        {
            this._area = area;
            this._ent = ent;
        }

        internal ExternalEntity Ent
        {
            get { return _ent; }
        }

        internal Area Area
        {
            get { return _area; }
        }

        BufferType BufferEvent.type()
        {
            return BufferType.DESTROY;
        }
    }


    //this event signifies the creation of a new entity.
    internal struct CreateEvent : BufferEvent
    {
        private readonly ExternalEntity _mover;
        private readonly Area _location;

        internal CreateEvent(ExternalEntity _mover, Area location)
        {
            this._mover = _mover;
            this._location = location;
        }

        internal Area Location
        {
            get { return _location; }
        }

        internal ExternalEntity Mover
        {
            get { return _mover; }
        }

        public BufferType type()
        {
            return BufferType.CREATE;
        }
    }
}
