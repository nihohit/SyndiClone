using System.Collections.Generic;
using System;
using Game.Logic.Entities;
using Game.Logic;
using SFML.Graphics;


namespace Game.Graphic_Manager
{
    //Just some random stuff for the class
    enum BuildingStyle { BLUE, RED, GREEN, YELLOW }

    //this 
    internal struct Block
    {
        static readonly uint TILE_SIZE = FileHandler.getUintProperty("tile size", FileAccessor.GENERAL);

        private readonly uint x, y;

        internal Block(uint _x, uint _y)
        {
            this.x = _x / TILE_SIZE;
            this.y = _y / TILE_SIZE;
        }

        internal uint Y
        {
            get { return y; }
        }

        internal uint X
        {
            get { return x; }
        }

        internal bool EqualSize(Block check)
        {
            return ((this.X == check.X) && (this.y == check.Y));
        }
    }

    //this class creates the building images
    internal static class BuildingImageGenerator
    {
        static readonly uint TILE_SIZE = FileHandler.getUintProperty("tile size", FileAccessor.GENERAL);
        private static Dictionary<ExternalEntity, Texture> buildings = new Dictionary<ExternalEntity, Texture>(new externalEntityEqualityComparer());
        private static Dictionary<Tuple<Block, BuildingStyle>, Texture> templates = new Dictionary<Tuple<Block, BuildingStyle>, Texture>(new tupleEqualityComparer());

        //checks if the image exists, if not, then if an equivalent image exist, and if not, creates a new image. 
        private static Texture GetBuildingImage(ExternalEntity building)
        {
            if (buildings.ContainsKey(building)) return buildings[building];
            var temp = Tuple.Create(new Block((uint)building.Size.Y, (uint)building.Size.X), generateStyle(building.Loyalty));
            Texture image = null;
            if (templates.ContainsKey(temp))
            {
                image = templates[temp];
            }
            else
            {
                image = new Texture(generateBuildingImage(temp));
                templates.Add(temp, image);
            }
            buildings.Add(building,image);
            return image;
        }

        //generate 
        private static Image generateBuildingImage(Tuple<Block, BuildingStyle> temp)
        {
            //TODO - if too heavy for realtime computing, try instead of generating an image, generating a rendertexture.
            uint depth = Convert.ToUInt16(temp.Item1.X);
            uint height = Convert.ToUInt16(temp.Item1.Y);
            BuildingStyle style = temp.Item2;
            Color basic = Color.Black;
            switch (style)
            {
                case BuildingStyle.YELLOW:
                    basic = Color.Yellow;
                    break;
                case BuildingStyle.RED:
                    basic = Color.Red;
                    break;
                case BuildingStyle.GREEN:
                    basic = Color.Green;
                    break;
                case BuildingStyle.BLUE:
                    basic = Color.Blue;
                    break;
            }

            Image img = new Image(TILE_SIZE * depth, TILE_SIZE * height, basic);
            List<Image> Images = new List<Image>();
            for (int i = 1; i <= depth; i++)
            {
                for (int j = 1; j <= height; j++)
                {
                    Images.Add(getBuildingTile(depth, height, i, j, style));
                }
            }

            uint depthOffset = 0;
            uint heightOffset = 0;
            int num = 1;

            foreach (Image Image in Images)
            {
                img.Copy(Image, depthOffset, heightOffset);
                num++;

                heightOffset += TILE_SIZE;
                if (heightOffset == img.Size.Y)
                {
                    depthOffset += TILE_SIZE;
                    heightOffset = 0;
                }
            }

            //this replaces the invisible part of the building with the basic color
            //TODO - see if I can overcome this. 
            for(uint x = 0 ; x < img.Size.X ; x++)
            {
                for (uint y = 0; y < img.Size.Y; y++)
                {
                    if (img.GetPixel(x, y).A < 255)
                        img.SetPixel(x, y, basic);
                }
            }
            //img.SaveToFile("building_Test.jpg");
            return img;
        }


        private static Image getBuildingTile(uint length, uint depth, int i, int j, BuildingStyle style)
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
            //TODO - put all the parts in a dictionary, so we won't need to reload them into new images. 
            switch (id)
            {
                case (0):
                    img = new Image("images/buildings/0_buildingmiddle.gif");
                    break;
                case (1):
                    img = new Image("images/buildings/9_edge.gif");
                    //img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    img = rotate180(img);
                    break;
                case (9):
                    img = new Image("images/buildings/9_edge.gif");
                    break;
                case (10):
                    img = new Image("images/buildings/9_edge.gif");
                    //img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    img = rotate270(img);
                    break;
                case (90):
                    img = new Image("images/buildings/9_edge.gif");
                    //img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    img = rotate90(img);
                    break;
                case (11):
                    img = new Image("images/buildings/9_corner.gif");
                    //img.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    img.FlipHorizontally();
                    break;
                case (91):
                    img = new Image("images/buildings/9_corner.gif");
                    //img.RotateFlip(RotateFlipType.Rotate90FlipX);
                    img = rotate90(img);
                    img.FlipHorizontally();
                    break;
                case (19):
                    img = new Image("images/buildings/9_corner.gif");
                    break;
                case (99):
                    img = new Image("images/buildings/9_corner.gif");
                    //img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    img = rotate90(img);
                    break;
                default:
                    break;
            }
            return img;
        }

        private static BuildingStyle generateStyle(Game.Logic.Affiliation aff)
        {
            //TODO - missing function
            int num = (int)aff;
            return (BuildingStyle)num;
        }

        internal static SFML.Graphics.Texture getBuildingSFMLTexture(ExternalEntity building)
        {
            Texture temp = GetBuildingImage(building);
            return temp;
        }

        private static Image rotate270(Image temp)
        {
            uint maxY = temp.Size.Y;
            uint maxX = temp.Size.X;
            Image val = new Image(maxY, maxX);
            for (uint x = 0; x < maxX; x++)
            {
                for (uint y = 0; y < maxY; y++)
                {
                    val.SetPixel(y, maxX-1-x, temp.GetPixel(x, y));
                }
            }
            return val;
        }

        private static Image rotate90(Image temp)
        {
            uint maxY = temp.Size.Y;
            uint maxX = temp.Size.X;
            Image val = new Image(maxY, maxX);
            for (uint x = 0; x < maxX; x++)
            {
                for (uint y = 0; y < maxY; y++)
                {
                    val.SetPixel(maxY-1-y, x, temp.GetPixel(x, y));
                }
            }
            return val;
        }

        private static Image rotate180(Image temp)
        {
            uint maxY = temp.Size.Y;
            uint maxX = temp.Size.X;
            Image val = new Image(maxX, maxY);
            for (uint x = 0; x < maxX; x++)
            {
                for (uint y = 0; y < maxY; y++)
                {
                    val.SetPixel(maxX-1-x, maxY-1-y, temp.GetPixel(x, y));
                }
            }
            return val;
        }
    }

    class tupleEqualityComparer : IEqualityComparer<Tuple<Block, BuildingStyle>>
    {

        public bool Equals(Tuple<Block, BuildingStyle> first, Tuple<Block, BuildingStyle> second)
        {
            return (first.Item1.EqualSize(second.Item1) && first.Item2 == second.Item2);
        }


        public int GetHashCode(Tuple<Block, BuildingStyle> item)
        {
            int hCode = (int)(item.Item1.X * item.Item1.Y * item.Item2.GetHashCode());
            return hCode.GetHashCode();
        }

    }
}
