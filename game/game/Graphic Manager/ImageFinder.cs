using System;
using System.Collections.Generic;
using Game.Logic.Entities;
using Game.Logic;
using SFML.Graphics;

namespace Game.Graphic_Manager
{
    //this is the general class for finding textures, just in case I'd like to replace the sprite finder
     interface TextureFinder
    {
        Sprite getSprite(ExternalEntity ent);
        Sprite getShot(ShotType shot);
        Sprite remove(ExternalEntity ent);
        Sprite nextSprite(ExternalEntity mover);
        Sprite getPath(ExternalEntity mover);
        Animation generateDestoryResults(Area area, entityType entityType);
        void setPath(ExternalEntity ent, List<Direction> path, SFML.Window.Vector2f position);
        void removePath(ExternalEntity ent);
        bool hasPath(ExternalEntity ent);

        Sprite nextPathStep(ExternalEntity ent);
    }

     class SpriteFinder : TextureFinder
    {
        enum ImageType {PATH }

        private Dictionary<ExternalEntity, SpriteLoop> spriteLoopFinder = new Dictionary<ExternalEntity, SpriteLoop>();
        private Dictionary<ExternalEntity, SpriteLoop> pathFinder = new Dictionary<ExternalEntity, SpriteLoop>();
        private Dictionary<Logic.Affiliation, Texture> personFinder = new Dictionary<Logic.Affiliation, Texture>
        {
            {Logic.Affiliation.INDEPENDENT, new Texture("images/Persons/personblue.png")},
            {Logic.Affiliation.CORP1, new Texture("images/Persons/personpurple.png")},
            {Logic.Affiliation.CORP2, new Texture("images/Persons/persongreen.png")},
            {Logic.Affiliation.CORP4, new Texture("images/Persons/personblack.png")},
            {Logic.Affiliation.CORP3, new Texture("images/Persons/personred.png")},
            {Logic.Affiliation.CIVILIAN, new Texture("images/Persons/personyellow.png")}
        };

        private Dictionary<ShotType, Texture> shots = new Dictionary<ShotType, Texture>
        {
            { ShotType.PISTOL_BULLET, new Texture("images/shots/bulletshotstraight.png")}

        };

        private Dictionary<ImageType, Texture> miscellaneous = new Dictionary<ImageType, Texture>
        {
            {ImageType.PATH, new Texture("images/UI/path.png")}
        };

        private List<ExternalEntity> removed = new List<ExternalEntity>();

        internal SpriteFinder()
        {
        }

        Sprite TextureFinder.getShot(ShotType shot)
        {
            return new Sprite(shots[shot]);
        }

        Sprite TextureFinder.getSprite(ExternalEntity ent)
        {
            if (!spriteLoopFinder.ContainsKey(ent))
            {
                spriteLoopFinder.Add(ent, this.generateNewSpriteLoop(ent));
            }
            return spriteLoopFinder[ent].Next();
        }

        //this function returns an animation as the result of destroying an entity.
         Graphic_Manager.Animation TextureFinder.generateDestoryResults(Area area, entityType type)
        {
            //TODO - missing function
            return new Graphic_Manager.Animation(area, type);
        }

        private SpriteLoop generateNewSpriteLoop(ExternalEntity ent)
        {
            List<Sprite> list = new List<Sprite>();
            if (ent.Type == entityType.VEHICLE)
            {
                //TODO - missing function
            }
            else
            {
                list.Insert(0,this.generateNewSprite(ent));
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

        Sprite TextureFinder.remove(ExternalEntity ent)
        {
            Sprite temp = this.spriteLoopFinder[ent].getSprite();
            this.spriteLoopFinder.Remove(ent);
            return temp;
        }

        Sprite TextureFinder.nextSprite(ExternalEntity mover)
        {
            return this.spriteLoopFinder[mover].Next();
        }

        /*************
        * PATHS
        *************/
        void TextureFinder.removePath(ExternalEntity ent)
        {
            if(pathFinder.ContainsKey(ent)) pathFinder.Remove(ent);
        }

        void TextureFinder.setPath(ExternalEntity ent, List<Game.Logic.Direction> path, SFML.Window.Vector2f position)
        {
            pathFinder.Add(ent, generateNewPathSprite(path, position));
        }

        private SpriteLoop generateNewPathSprite(List<Direction> path, SFML.Window.Vector2f position)
        {
            List<Sprite> newList = new List<Sprite>();
            Vector initial = new Vector(Convert.ToInt32(position.X), Convert.ToInt32(position.Y));
            Sprite temp;
            foreach (Direction dir in path)
            {
                temp = new Sprite(miscellaneous[ImageType.PATH]);
                switch (dir)
                {
                    case(Direction.UP):
                        break;
                    case (Direction.DOWN):
                        temp.Rotation = 180F;
                        break;
                    case (Direction.LEFT):
                        temp.Rotation = 270;
                        break;
                    case (Direction.RIGHT):
                        temp.Rotation = 90F;
                        break;
                    case (Direction.UPRIGHT):
                        temp.Rotation = 45F;
                        break;
                    case (Direction.UPLEFT):
                        temp.Rotation = 315F;
                        break;
                    case (Direction.DOWNRIGHT):
                        temp.Rotation = 135F;
                        break;
                    case (Direction.DOWNLEFT):
                        temp.Rotation = 225F;
                        break;
                }
                initial = initial.addVector(Vector.directionToVector(dir));
                temp.Position = initial.toVector2f();
                newList.Add(temp);
            }

            return new SpriteLoop(newList);
        }

        Sprite TextureFinder.getPath(ExternalEntity ent)
        {
            return this.pathFinder[ent].getSprite();
        }


        Sprite TextureFinder.nextPathStep(ExternalEntity ent)
        {
            return this.pathFinder[ent].Next();
        }


        bool TextureFinder.hasPath(ExternalEntity ent)
        {
            return pathFinder.ContainsKey(ent);
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
