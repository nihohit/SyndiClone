using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic.Entities
{
    internal class ExternalEntity
    {
        private readonly Vector _size;
        private readonly entityType _type;
        private readonly Affiliation _loyalty;
        private SFML.Window.Vector2f _position;
        private readonly Entity _ent;

        internal ExternalEntity(Entity ent, SFML.Window.Vector2f position)
        {
            this._loyalty = ent.Loyalty;
            this._position = position;
            this._size = ent.Size;
            this._type = ent.Type;
            this._ent = ent;
        }

        internal ExternalEntity(Entity ent)
        {
            if (ent != null)
            {
                this._loyalty = ent.Loyalty;
                this._size = ent.Size;
                this._type = ent.Type;
                this._ent = ent;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is ExternalEntity)
            {
                if (this._ent == null) return (((ExternalEntity)obj).Ent == null);
                return (this.Ent.Equals(((ExternalEntity)obj).Ent));
            }
            else return false;
        }

        internal entityType Type
        {
            get { return _type; }
        }

        internal SFML.Window.Vector2f Position
        {
            get { return _position; }
            set { _position = value; }
        }

        internal Affiliation Loyalty
        {
            get { return _loyalty; }
        }

        internal Vector Size
        {
            get { return _size; }
        }

        internal Entity Ent
        {
            get { return _ent; }
        } 

        public bool Equals(ExternalEntity obj)
        {
            return this.Ent.Equals(obj.Ent);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
