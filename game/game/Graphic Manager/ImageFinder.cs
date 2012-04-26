using System;
using System.Collections.Generic;
using Game.Logic.Entities;
using Game.Logic;
using SFML.Graphics;

namespace Game.Graphic_Manager
{

    internal abstract class TextureFinder
    {
        internal abstract Sprite getSprite(ExternalEntity ent);
        internal abstract Sprite getShot(ShotType shot, bool diagonal);
        internal abstract Sprite remove(ExternalEntity ent);
        internal abstract Sprite nextSprite(ExternalEntity mover);
        internal abstract Animation generateDestoryResults(Area area, entityType entityType);
    }

    internal class SpriteFinder : TextureFinder
    {
        private Dictionary<ExternalEntity, SpriteLoop> spriteLoopFinder = new Dictionary<ExternalEntity, SpriteLoop>();
        private static Dictionary<Logic.Affiliation, Texture> personFinder = new Dictionary<Logic.Affiliation, Texture>
        {
            {Logic.Affiliation.INDEPENDENT, new Texture("images/Persons/personblue.png")},
            {Logic.Affiliation.CORP1, new Texture("images/Persons/personpurple.png")},
            {Logic.Affiliation.CORP2, new Texture("images/Persons/persongreen.png")},
            {Logic.Affiliation.CORP4, new Texture("images/Persons/personblack.png")},
            {Logic.Affiliation.CORP3, new Texture("images/Persons/personred.png")},
            {Logic.Affiliation.CIVILIAN, new Texture("images/Persons/personyellow.png")}
        };
        private static Dictionary<ShotType, Texture> straightshots = new Dictionary<ShotType, Texture>
        {
            { ShotType.PISTOL_BULLET, new Texture("images/shots/bulletshotstraight.png")}

        };
        private static Dictionary<ShotType, Texture> diagonalshots = new Dictionary<ShotType, Texture>
        {
            { ShotType.PISTOL_BULLET, new Texture("images/shots/bulletshotdiagonal.png")}

        };

        private static List<ExternalEntity> removed = new List<ExternalEntity>();

        internal SpriteFinder()
        {
        }

        internal override Sprite getShot(ShotType shot, bool diagonal)
        {
            if (diagonal) return new Sprite(diagonalshots[shot]);
            return new Sprite(straightshots[shot]);
        }

        internal override Sprite getSprite(ExternalEntity ent)
        {
            if (!spriteLoopFinder.ContainsKey(ent))
            {
                spriteLoopFinder.Add(ent, this.generateNewSpriteLoop(ent));
            }
            Sprite sprite = spriteLoopFinder[ent].Next();
            return sprite;
        }

        internal override Graphic_Manager.Animation generateDestoryResults(Area area, entityType type)
        {
            //TODO - missing function
            return new Graphic_Manager.Animation(area, type);
        }

        private SpriteLoop generateNewSpriteLoop(ExternalEntity ent)
        {
            LinkedList<Sprite> list = new LinkedList<Sprite>();
            if (ent.Type == entityType.VEHICLE)
            {
                //TODO - missing function
            }
            else
            {
                list.AddFirst(this.generateNewSprite(ent));
            }
            return new SpriteLoop(list);
        }

        private Sprite generateNewSprite(ExternalEntity ent)
        {
            Sprite sprite = null;
            switch (ent.Type)
            {
                case(Game.Logic.entityType.BUILDING):
                    sprite = new Sprite(BuildingImageGenerator.getBuildingSFMLTexture(ent));
                    break;

                case(Logic.entityType.PERSON):
                    sprite = new Sprite(personFinder[ent.Loyalty]);
                    break;

            }
            return sprite;
        }

        internal override Sprite remove(ExternalEntity ent)
        {
            Sprite temp = this.spriteLoopFinder[ent].getSprite();
            this.spriteLoopFinder.Remove(ent);
            return temp;
        }

        internal override Sprite nextSprite(ExternalEntity mover)
        {
            return this.spriteLoopFinder[mover].Next();
        }
    }

    class externalEntityEqualityComparer : IEqualityComparer<ExternalEntity>
    {

        public bool Equals(ExternalEntity first, ExternalEntity second)
        {
            return (first.Ent.Equals(second.Ent));
        }


        public int GetHashCode(ExternalEntity item)
        {
            int hCode = item.Ent.GetHashCode();
            return hCode.GetHashCode();
        }

    }
}
