using System;

using System.Collections.Generic;
using Game.Logic.Entities;

using Game.Logic;

namespace Game.Graphic_Manager
{

    
    internal abstract class TextureFinder
    {
        internal abstract SFML.Graphics.Sprite getSprite(ExternalEntity ent);
        internal abstract SFML.Graphics.Sprite getShot(ShotType shot, bool diagonal);
        internal abstract SFML.Graphics.Sprite remove(ExternalEntity ent);
        internal abstract SFML.Graphics.Sprite nextSprite(ExternalEntity mover);
        internal abstract Animation generateDestoryResults(Area area, entityType entityType);
    }

    internal class SpriteFinder : TextureFinder
    {
        private Dictionary<ExternalEntity, SpriteLoop> spriteLoopFinder = new Dictionary<ExternalEntity, SpriteLoop>();
        private static Dictionary<Logic.Affiliation, SFML.Graphics.Texture> personFinder = new Dictionary<Logic.Affiliation, SFML.Graphics.Texture>
        {
            {Logic.Affiliation.INDEPENDENT, new SFML.Graphics.Texture("images/Persons/personblue.png")},
            {Logic.Affiliation.CORP1, new SFML.Graphics.Texture("images/Persons/personpurple.png")},
            {Logic.Affiliation.CORP2, new SFML.Graphics.Texture("images/Persons/persongreen.png")},
            {Logic.Affiliation.CORP4, new SFML.Graphics.Texture("images/Persons/personblack.png")},
            {Logic.Affiliation.CORP3, new SFML.Graphics.Texture("images/Persons/personred.png")},
            {Logic.Affiliation.CIVILIAN, new SFML.Graphics.Texture("images/Persons/personyellow.png")}
        };
        private static Dictionary<ShotType, SFML.Graphics.Texture> straightshots = new Dictionary<ShotType, SFML.Graphics.Texture>
        {
            { ShotType.PISTOL_BULLET, new SFML.Graphics.Texture("images/shots/bulletshotstraight.png")}

        };
        private static Dictionary<ShotType, SFML.Graphics.Texture> diagonalshots = new Dictionary<ShotType, SFML.Graphics.Texture>
        {
            { ShotType.PISTOL_BULLET, new SFML.Graphics.Texture("images/shots/bulletshotdiagonal.png")}

        };

        internal SpriteFinder()
        {
        }

        internal override SFML.Graphics.Sprite getShot(ShotType shot, bool diagonal)
        {
            if (diagonal) return new SFML.Graphics.Sprite(diagonalshots[shot]);
            return new SFML.Graphics.Sprite(straightshots[shot]);
        }

        internal override SFML.Graphics.Sprite getSprite(ExternalEntity ent)
        {
            if (!spriteLoopFinder.ContainsKey(ent))
            {
                spriteLoopFinder.Add(ent, this.generateNewSpriteLoop(ent));
            }
            SFML.Graphics.Sprite sprite = spriteLoopFinder[ent].Next();
            return sprite;
        }

        internal override Graphic_Manager.Animation generateDestoryResults(Area area, entityType type)
        {
            //TODO - missing function
            return new Graphic_Manager.Animation(area, type);
        }

        private SpriteLoop generateNewSpriteLoop(ExternalEntity ent)
        {
            LinkedList<SFML.Graphics.Sprite> list = new LinkedList<SFML.Graphics.Sprite>();
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

        private SFML.Graphics.Sprite generateNewSprite(ExternalEntity ent)
        {
            SFML.Graphics.Sprite sprite = null;
            switch (ent.Type)
            {
                case(Game.Logic.entityType.BUILDING):
                    sprite = new SFML.Graphics.Sprite(BuildingImageGenerator.getBuildingSFMLTexture(ent));
                    break;

                case(Logic.entityType.PERSON):
                    sprite = new SFML.Graphics.Sprite(personFinder[ent.Loyalty]);
                    break;

            }
            return sprite;
        }

        internal override SFML.Graphics.Sprite remove(ExternalEntity ent)
        {
            SFML.Graphics.Sprite temp = this.spriteLoopFinder[ent].getSprite();
            this.spriteLoopFinder.Remove(ent);
            return temp;
        }

        internal override SFML.Graphics.Sprite nextSprite(ExternalEntity mover)
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
