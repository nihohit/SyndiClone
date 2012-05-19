using System;
using System.Collections.Generic;
using Game.Logic.Entities;
using Game.Logic;
using SFML.Graphics;

namespace Game.Graphic_Manager
{
    //this is the general class for finding textures, just in case I'd like to replace the sprite finder
    //HACK - is this interface even needed? 
     interface TextureFinder
    {
        Sprite getSprite(ExternalEntity ent);
        Sprite getShot(ShotType shot);
        Sprite remove(ExternalEntity ent);
        Sprite nextSprite(ExternalEntity mover);
        Sprite getPath(ExternalEntity mover);
        Animation generateDestoryResults(Area area, entityType entityType);
        void setPath(ExternalEntity ent, List<Direction> path, SFML.Window.Vector2f position);
        Sprite removePath(ExternalEntity ent);
        bool hasPath(ExternalEntity ent);
        Sprite nextPathStep(ExternalEntity ent);
    }


    class SpriteFinder : TextureFinder
    {
        enum ImageType {PATH } //this enumerator is used for miscelenae. 

        /***************
         * class fields
         ***************/
        private uint distanceBetweenPathObjects = FileHandler.getUintProperty("path sprite distance", FileAccessor.DISPLAY);
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

        /***************
         * constructor
         ***************/

        internal SpriteFinder()
        {
        }

        /************
         * External functions
         ***********/

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
            return spriteLoopFinder[ent].nextSprite();
        }

        //this function returns an animation as the result of destroying an entity.
        Graphic_Manager.Animation TextureFinder.generateDestoryResults(Area area, entityType type)
        {
            //TODO - missing function
            return new Graphic_Manager.Animation(area, type);
        }

        Sprite TextureFinder.remove(ExternalEntity ent)
        {
            Sprite temp = this.spriteLoopFinder[ent].CurrentSprite();
            this.spriteLoopFinder.Remove(ent);
            return temp;
        }

        Sprite TextureFinder.nextSprite(ExternalEntity mover)
        {
            return this.spriteLoopFinder[mover].nextSprite();
        }

        Sprite TextureFinder.removePath(ExternalEntity ent)
        {
            if (pathFinder.ContainsKey(ent))
            {
                Sprite temp = pathFinder[ent].CurrentSprite();
                pathFinder.Remove(ent);
                return temp;
            }
            return null;
        }

        void TextureFinder.setPath(ExternalEntity ent, List<Direction> path, SFML.Window.Vector2f position)
        {
            pathFinder.Add(ent, generateNewComplexPath(path, position));
        }

        Sprite TextureFinder.getPath(ExternalEntity ent)
        {
            return this.pathFinder[ent].CurrentSprite();
        }


        Sprite TextureFinder.nextPathStep(ExternalEntity ent)
        {
            return this.pathFinder[ent].nextSprite();
        }


        bool TextureFinder.hasPath(ExternalEntity ent)
        {
            return pathFinder.ContainsKey(ent);
        }

        /***********
         * internal methods
         **********/

        private SpriteLoop generateNewSpriteLoop(ExternalEntity ent)
        {
            List<Sprite> list = new List<Sprite>();
            switch (ent.Type)
            {
                case (Game.Logic.entityType.BUILDING):
                    list.Add(new Sprite(BuildingImageGenerator.getBuildingSFMLTexture(ent)));
                    break;

                case (Logic.entityType.PERSON):
                    list.Add(new Sprite(personFinder[ent.Loyalty]));
                    break;

                case (Logic.entityType.VEHICLE):
                    //TODO - missing function
                    break;

            }
            return new SpriteLoop(list);
        }

        /*************
        * PATHS
        *************/

        /*This function receives the parameters for a path, generates its bounding box, and then creates a series of RenderTextures sized as the bounding box, and places the 
         * relevant sprites in RenderTextures. 
         * DistanceBetweenPathObjects basically says how many sprites will be in the resulting loop. 
         */
        private SpriteLoop generateNewComplexPath(List<Direction> path, SFML.Window.Vector2f position)
        {
            Area area = getPathSize(path);
            List<Sprite> list = new List<Sprite>();
            List <Sprite> newSprite = generateNewPathSprite(path, area.Entry);
            
            list.Clear();
            Sprite temp;
            int count;
            RenderTexture texture;
            //In each recursion of this loop a new RenderTexture is created and each sprite that is relevant to that texture is embedded in it. 
            for (uint i = 0; i < distanceBetweenPathObjects; i++)
            {
                texture = new RenderTexture((uint)area.Size.X, (uint)area.Size.Y);
                for (uint j = 0; j < path.Count / distanceBetweenPathObjects; j++)
                {
                    count = (int)(j * distanceBetweenPathObjects + i);
                    if (count < newSprite.Count)
                    {
                        texture.Draw(newSprite[count]);
                    }
                }
                texture.Display();
                temp = new Sprite(new Texture(texture.Texture));
                temp.Origin = new SFML.Window.Vector2f(area.Entry.X, area.Entry.Y);
                temp.Position = position;
                list.Add(temp);
            }

            return new SpriteLoop(list);
        }

        //This function computes the size of a path texture, and its beginning point inside the square.
        private Area getPathSize(List<Direction> path)
        {
            Texture pathTexture = miscellaneous[ImageType.PATH];
            uint maxSize = Math.Max(pathTexture.Size.X, pathTexture.Size.Y);
            uint halfSize = maxSize / 2;
            uint xSize = maxSize, xPos = halfSize, ySize = maxSize, yPos = halfSize, x = halfSize, y = halfSize;
            foreach (Direction dir in path)
            {
                switch (dir)
                {
                    case (Game.Logic.Direction.DOWN):
                        y++;
                        break;
                    case (Game.Logic.Direction.DOWNLEFT):
                        y++;
                        x--;
                        break;
                    case (Game.Logic.Direction.DOWNRIGHT):
                        y++;
                        x++;
                        break;
                    case (Game.Logic.Direction.LEFT):
                        x--;
                        break;
                    case (Game.Logic.Direction.RIGHT):
                        x++;
                        break;
                    case (Game.Logic.Direction.UP):
                        y--;
                        break;
                    case (Game.Logic.Direction.UPLEFT):
                        y--;
                        x--;
                        break;
                    case (Game.Logic.Direction.UPRIGHT):
                        x++;
                        y--;
                        break;
                }
                if (x < halfSize) { xPos++; xSize++; x = halfSize; }
                if (y < halfSize) { yPos++; ySize++; y = halfSize; }
                if (x >= xSize - halfSize) { xSize++; }
                if (y >= ySize - halfSize) { ySize++; }
            }

            return new Area(new Point((int)xPos, (int)yPos), new Vector((int)xSize, (int)ySize));
        }

        //This function returns a new path sprite, rotated to the correct way. 
        private List<Sprite> generateNewPathSprite(List<Direction> path, Point entry)
        {
            List<Sprite> newList = new List<Sprite>();
            Vector initial = new Vector(entry.X, entry.Y);
            Sprite temp;
            Texture texture = miscellaneous[ImageType.PATH];
            uint xSize = texture.Size.X / 2, ySize = texture.Size.Y / 2;
            foreach (Direction dir in path)
            {
                temp = new Sprite(miscellaneous[ImageType.PATH]);
                temp.Origin = new SFML.Window.Vector2f(xSize, ySize);
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

            return newList;
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
