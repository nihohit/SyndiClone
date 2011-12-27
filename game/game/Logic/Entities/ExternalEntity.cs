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
        private readonly Vector _position;
        private readonly Entity _ent;

        internal ExternalEntity(Entity ent, Vector position)
        {
            this._loyalty = ent.Loyalty;
            this._position = position;
            this._size = ent.Size;
            this._type = ent.Type;
            this._ent = ent;
        }

        internal ExternalEntity(Entity ent)
        {
            this._loyalty = ent.Loyalty;
            this._size = ent.Size;
            this._type = ent.Type;
            this._ent = ent;
        }

        public override bool Equals(object obj)
        {
            if (obj is ExternalEntity)
                return (this.Ent.Equals(((ExternalEntity)obj).Ent));
            else return false;
        }

        internal entityType Type
        {
            get { return _type; }
        }

        internal Vector Position
        {
            get { return _position; }
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

    }
}
