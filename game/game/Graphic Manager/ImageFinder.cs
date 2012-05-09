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
        internal abstract Sprite getPath(List<Direction> path);
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

        /*************
        * PATHS
        *************/
        internal override Sprite getPath(List<Game.Logic.Direction> path)
        {
            Area size = getPathSize(path);
            Image img = new Image((uint)size.Size.X, (uint)size.Size.Y, Color.White);

            uint x = (uint)size.Entry.X, y = (uint)size.Entry.Y;
            foreach (Game.Logic.Direction dir in path)
            {
                if (x == 0 || y == 0 || x >= img.Size.X - 1 || y >= img.Size.Y - 1)
                {
                    x--;
                }
                img.SetPixel(x, y, Color.Green);
                img.SetPixel(x, y + 1, Color.Green);
                img.SetPixel(x, y - 1, Color.Green);
                img.SetPixel(x - 1, y, Color.Green);
                img.SetPixel(x - 1, y + 1, Color.Green);
                img.SetPixel(x - 1, y - 1, Color.Green);
                img.SetPixel(x + 1, y, Color.Green);
                img.SetPixel(x + 1, y - 1, Color.Green);
                img.SetPixel(x + 1, y + 1, Color.Green);
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
            }
            //img.CreateMaskFromColor(Color.White);
            for (uint i = 0; i < img.Size.X; i++)
            {
                for (uint j = 0; j < img.Size.Y; j++)
                {
                    if (img.GetPixel(i, j).Equals(Color.White))
                    {
                        img.SetPixel(i,j,new Color(0,0,0,0));
                    }
                }

            }
            Sprite ans = new Sprite(new Texture(img));
            ans.Origin = new SFML.Window.Vector2f(size.Entry.X, size.Entry.Y);
            return ans;
        }

        private Area getPathSize(List<Direction> path)
        {
            int xSize = 3, xPos = 1, ySize = 3, yPos = 1, x = 1, y = 1;
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
                if (x < 1) { xPos++; xSize++; x = 1; }
                if (y < 1) { yPos++; ySize++; y = 1; }
                if (x >= xSize-1) { xSize++; }
                if (y >= ySize-1) { ySize++; }
            }

            return new Area(new Point(xPos, yPos), new Vector(xSize, ySize));
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
