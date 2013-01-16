using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic.Entities
{
    public class ExternalEntity
    {
        #region properties

        public entityType Type { get; private set; }

        public SFML.Window.Vector2f Position { get; set; }

        public Affiliation Loyalty { get; private set; }

        public Vector Size { get; private set; }

        public Entity InternalEntity { get; private set; }

        #endregion

        #region constructors

        public ExternalEntity(Entity ent, SFML.Window.Vector2f position)
        {
            Loyalty = ent.Loyalty;
            Position = position;
            Size = ent.Size;
            Type = ent.Type;
            InternalEntity = ent;
        }

        public ExternalEntity(Entity ent)
        {
            if (ent != null)
            {
                Loyalty = ent.Loyalty;
                Size = ent.Size;
                Type = ent.Type;
                InternalEntity = ent;
            }
        }

        #endregion

        #region comparison methods

        public override bool Equals(object obj)
        {
            if (obj is ExternalEntity)
            {
                if (InternalEntity == null) return (((ExternalEntity)obj).InternalEntity == null);
                return (InternalEntity.Equals(((ExternalEntity)obj).InternalEntity));
            }
            else return false;
        }

        public bool Equals(ExternalEntity obj)
        {
            return InternalEntity.Equals(obj.InternalEntity);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }
}
