using System;
using System.IO;
using System.Drawing;
using System.Text;
using System.Collections.Generic;
using Game.Logic.Entities;
using System.Drawing.Imaging;

namespace Game.Buffers
{
    //Just some random stuff for the class
    enum BuildingStyle { BLUE, RED, GREEN, YELLOW }

    

    internal class Block
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
    
    abstract class ImageFinder
    {
        internal abstract SFML.Graphics.Sprite getSprite(ExternalEntity ent);
        internal abstract SFML.Graphics.Sprite getSpriteLoop(ExternalEntity ent);
    }

    class SpriteFinder : ImageFinder
    {
        const int TILE_SIZE = 32;

        private Dictionary <ExternalEntity, SFML.Graphics.Sprite> finder = new Dictionary<ExternalEntity, SFML.Graphics.Sprite>();
        private static Dictionary<ExternalEntity, Image> buildings = new Dictionary<ExternalEntity, Image>();
        private static Dictionary<Tuple<Block, BuildingStyle>, Image> templates = new Dictionary<Tuple<Block, BuildingStyle>, Image>(new tupleEqualityComparer());

        internal override SFML.Graphics.Sprite getSprite(ExternalEntity ent)
        {
            SFML.Graphics.Sprite sprite = null;
            if (!finder.ContainsKey(ent))
            {
                sprite = this.generateNewSprite(ent);
            }
            sprite = finder[ent];
            return sprite;
        }

        private SFML.Graphics.Sprite generateNewSprite(ExternalEntity ent)
        {
            SFML.Graphics.Sprite sprite = null;
            switch (ent.Type)
            {
                case(Game.Logic.entityType.BUILDING):
                    sprite = new SFML.Graphics.Sprite(this.getBuildingSFMLImage(ent));
                    break;
                //TODO - cases missing

            }
            finder.Add(ent, sprite);
            return sprite;
        }

        internal override SFML.Graphics.Sprite getSpriteLoop(ExternalEntity ent)
        {
            //TODO - missing function
            return null;
        }

        private SFML.Graphics.Image getBuildingSFMLImage(ExternalEntity building)
        {
            Image image = GetBuildingImage(building);
            /*magic code to convert from the Drawing image to SFML image)*/
            MemoryStream stream = new MemoryStream();
            image.Save(stream, ImageFormat.Png);
            return new SFML.Graphics.Image(stream);
        }

        internal static Image GetBuildingImage(ExternalEntity building)
        {
            var temp = Tuple.Create(new Block(building.Size.X, building.Size.Y), generateStyle(building.Loyalty));
            System.Drawing.Image image = generateBuildingImage(temp);
            buildings[building] = image;
            templates[temp] = image;
            return image;
        }

        private static System.Drawing.Image generateBuildingImage(Tuple<Block, BuildingStyle> temp)
        {
            int width = temp.Item1.X;
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
            Image img = new Bitmap(TILE_SIZE * width, TILE_SIZE * height);
            List<Image> images = new List<Image>();
            for (int i = 1; i <= width; i++)
            {
                for (int j = 1; j <= height; j++)
                {
                    images.Add(getBuildingTile(width, height, i, j, style));
                }
            }

            Graphics graphic = Graphics.FromImage(img);
            graphic.Clear(basic);
            int widthOffset = 0;
            int heightOffset = 0;
            int num = 1;

            foreach (Image image in images)
            {
                graphic.DrawImage(image, new Rectangle(widthOffset, heightOffset, image.Width, image.Height));
                num++;

                heightOffset += TILE_SIZE;
                if (heightOffset == img.Height)
                {
                    widthOffset += TILE_SIZE;
                    heightOffset = 0;
                }
            }

            return img;
        }

        private static Image getBuildingTile(int length, int width, int i, int j, BuildingStyle style)
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
            if (j == width)
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
}
