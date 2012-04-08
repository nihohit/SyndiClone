using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using Game.Logic.Entities;
using System.Drawing.Imaging;
using Game.Logic;

namespace Game.Graphic_Manager
{
    //Just some random stuff for the class
    enum BuildingStyle { BLUE, RED, GREEN, YELLOW }

    internal struct Block
    {
        const int TILE_SIZE = 32;

        private readonly int x, y;

        internal Block(int _x, int _y)
        {
            this.x = _x / TILE_SIZE;
            this.y = _y / TILE_SIZE;
        }
    
        internal int Y
        {
            get { return y; }
        }

        internal int X
        {
            get { return x; }
        }

        internal bool EqualSize(Block check)
        {
            return ((this.X == check.X) && (this.y == check.Y));
        }
    }
    
    internal abstract class ImageFinder
    {
        internal abstract SFML.Graphics.Sprite getSprite(ExternalEntity ent);
        internal abstract SFML.Graphics.Sprite getShot(ShotType shot, bool diagonal);
        internal abstract SFML.Graphics.Sprite remove(ExternalEntity ent);
        internal abstract SFML.Graphics.Sprite nextSprite(ExternalEntity mover);
        internal abstract Animation generateDestoryResults(Area area, entityType entityType);
    }

    internal class SpriteFinder : ImageFinder
    {
        const int TILE_SIZE = 32;
        private Dictionary<ExternalEntity, SpriteLoop> spriteLoopFinder = new Dictionary<ExternalEntity, SpriteLoop>();
        private static Dictionary<ExternalEntity, Image> buildings = new Dictionary<ExternalEntity, Image>(new externalEntityEqualityComparer());
        private static Dictionary<Tuple<Block, BuildingStyle>, Image> templates = new Dictionary<Tuple<Block, BuildingStyle>, Image>(new tupleEqualityComparer());
        private static Dictionary<Logic.Affiliation, SFML.Graphics.Image> personFinder = new Dictionary<Logic.Affiliation, SFML.Graphics.Image>
        {
            {Logic.Affiliation.INDEPENDENT, new SFML.Graphics.Image("images/personblue.png")},
            {Logic.Affiliation.CORP1, new SFML.Graphics.Image("images/personpurple.png")},
            {Logic.Affiliation.CORP2, new SFML.Graphics.Image("images/persongreen.png")},
            {Logic.Affiliation.CORP4, new SFML.Graphics.Image("images/personblack.png")},
            {Logic.Affiliation.CORP3, new SFML.Graphics.Image("images/personred.png")},
            {Logic.Affiliation.CIVILIAN, new SFML.Graphics.Image("images/personyellow.png")}
        };
        private static Dictionary<ShotType, SFML.Graphics.Image> straightshots = new Dictionary<ShotType, SFML.Graphics.Image>
        {
            { ShotType.PISTOL_BULLET, new SFML.Graphics.Image("images/bulletshotstraight.png")}

        };
        private static Dictionary<ShotType, SFML.Graphics.Image> diagonalshots = new Dictionary<ShotType, SFML.Graphics.Image>
        {
            { ShotType.PISTOL_BULLET, new SFML.Graphics.Image("images/bulletshotdiagonal.png")}

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
                    sprite = new SFML.Graphics.Sprite(this.getBuildingSFMLImage(ent));
                    break;

                case(Logic.entityType.PERSON):
                    sprite = new SFML.Graphics.Sprite(personFinder[ent.Loyalty]);
                    break;

            }
            return sprite;
        }


        private SFML.Graphics.Image getBuildingSFMLImage(ExternalEntity building)
        {
            Image image = GetBuildingImage(building);
            /*magic code to convert from the Drawing image to SFML image)*/
            MemoryStream stream = new MemoryStream();
            image.Save(stream, ImageFormat.Png);
            SFML.Graphics.Image temp = new SFML.Graphics.Image(stream);
            stream.Dispose();
            return temp;
        }

        internal static Image GetBuildingImage(ExternalEntity building)
        {
            var temp = Tuple.Create(new Block(building.Size.Y, building.Size.X), generateStyle(building.Loyalty));
            System.Drawing.Image image = generateBuildingImage(temp);
            buildings[building] = image;
            templates[temp] = image;
            return image;
        }

        private static System.Drawing.Image generateBuildingImage(Tuple<Block, BuildingStyle> temp)
        {
            int depth = temp.Item1.X;
            int height = temp.Item1.Y;
            BuildingStyle style = temp.Item2;
            System.Drawing.Color basic = System.Drawing.Color.Gray;
            switch (style)
            {
                case BuildingStyle.YELLOW:
                    basic = System.Drawing.Color.Yellow;
                    break;
                case BuildingStyle.RED:
                    basic = System.Drawing.Color.Red;
                    break;
                case BuildingStyle.GREEN:
                    basic = System.Drawing.Color.Green;
                    break;
                case BuildingStyle.BLUE:
                    basic = System.Drawing.Color.Blue;
                    break;

            }
            Image img = new Bitmap(TILE_SIZE * depth, TILE_SIZE * height);
            List<Image> images = new List<Image>();
            for (int i = 1; i <= depth; i++)
            {
                for (int j = 1; j <= height; j++)
                {
                    images.Add(getBuildingTile(depth, height, i, j, style));
                }
            }

            Graphics graphic = Graphics.FromImage(img);
            graphic.Clear(basic);
            int depthOffset = 0;
            int heightOffset = 0;
            int num = 1;

            foreach (Image image in images)
            {
                graphic.DrawImage(image, new Rectangle(depthOffset, heightOffset, image.Width, image.Height));
                num++;

                heightOffset += TILE_SIZE;
                if (heightOffset == img.Height)
                {
                    depthOffset += TILE_SIZE;
                    heightOffset = 0;
                }
            }

            return img;
        }

        private static Image getBuildingTile(int length, int depth, int i, int j, BuildingStyle style)
        {
            //TODO - account for style
            Image img = null;
            int id = 0;
            if (i == 1)
            {
                id += 1;
            }
            if (i == length)
            {
                id += 9;
            }
            if (j == 1)
            {
                id += 10;
            }
            if (j == depth)
            {
                id += 90;
            }
            switch (id)
            {
                case (0):
                    img = new Bitmap(entity_images._0_buildingmiddle);
                    break;
                case (1):
                    img = new Bitmap(entity_images._9_edge);
                    img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case (9):
                    img = new Bitmap(entity_images._9_edge);
                    break;
                case (10):
                    img = new Bitmap(entity_images._9_edge);
                    img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
                case (90):
                    img = new Bitmap(entity_images._9_edge);
                    img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case (11):
                    img = new Bitmap(entity_images._9_corner);
                    img.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    break;
                case (91):
                    img = new Bitmap(entity_images._9_corner);
                    img.RotateFlip(RotateFlipType.Rotate90FlipX);
                    break;
                case (19):
                    img = new Bitmap(entity_images._9_corner);
                    break;
                case (99):
                    img = new Bitmap(entity_images._9_corner);
                    img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
            }

            //TODO - needs testing
            return img;
        }

        private static BuildingStyle generateStyle(Game.Logic.Affiliation aff)
        {
            //TODO - missing function
            int num = (int)aff;
            return (BuildingStyle)num;
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

    //TODO - needs testing
    class tupleEqualityComparer : IEqualityComparer<Tuple<Block, BuildingStyle>>
    {

        public bool Equals(Tuple<Block, BuildingStyle> first, Tuple<Block, BuildingStyle> second)
        {
            return (first.Item1.EqualSize(second.Item1) && first.Item2 == second.Item2);
        }


        public int GetHashCode(Tuple<Block, BuildingStyle> item)
        {
            int hCode = item.Item1.X * item.Item1.Y * item.Item2.GetHashCode();
            return hCode.GetHashCode();
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
