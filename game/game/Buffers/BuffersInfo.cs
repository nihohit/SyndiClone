using Game.Logic;
using Game.Logic.Entities;

namespace Game.Buffers
{
    internal enum BufferType { MOVE, SHOT, DESTROY, CREATE, PAUSE } //different buffers for the actions that the grid returns after each loop

    internal interface BufferEvent
    {
        BufferType type();
    }

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

    internal struct MoveEvent : BufferEvent
    {

        private readonly Area _entry;
        private readonly ExternalEntity _mover;
        private readonly int _rotation;

        internal MoveEvent(Area entry, ExternalEntity mover, int rotation)
        {
            this._entry = entry;
            this._mover = mover;
            this._rotation = rotation;
        }

        public int Rotation
        {
            get { return _rotation; }
        }

        internal ExternalEntity Mover
        {
            get { return _mover; }
        }

        internal Area Entry
        {
            get { return _entry; }
        }

        public BufferType type()
        {
            return BufferType.MOVE;
        }
    }

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
