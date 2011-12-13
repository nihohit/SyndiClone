using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System;
using System.IO;



namespace Game.City_Generator
{
    enum BuildingStyle { BLUE, RED, GREEN, YELLOW }
    class CityImageGenerator
    {

        
        static int TILE_SIZE = 32;
        static Dictionary<Building, Image> buildings = new Dictionary<Building, Image>();
        static Dictionary<Tuple<Block, BuildingStyle>, Image> templates = new Dictionary<Tuple<Block, BuildingStyle>, Image>(new tupleEqualityComparer());
        
        public static SFML.Graphics.Image convert_to_image(GameBoard city){
            Tile[,] grid = city.Grid; 
            //Image img = new Bitmap(TILE_SIZE*grid.GetLength(0),TILE_SIZE*grid.GetLength(1));
            Image img = new Bitmap(TILE_SIZE * city.Length, TILE_SIZE * city.Width);
            List<Image> images = new List<Image>();
            for (int i = 0 ; i < city.Length ; i ++)
            {
                for (int j = 0; j < city.Width; j++)
                {
                    switch(grid[i, j].Type)
                    {
                        case ContentType.ROAD:
                        {
                            images.Add(get_road_image((RoadTile)grid[i, j]));
                            break;
                        }
                        case ContentType.BUILDING:
                        {
                            images.Add(city_images._0_buildingTemp1);
                            break;
                        }
                        //TODO - other tiles?
                        default:
                        {
                            images.Add(new Bitmap(city_images._0_empty));
                            break;
                        }
                    }
                    
                }
            }
            Graphics graphic = Graphics.FromImage(img);
            graphic.Clear(Color.Gray);
            int widthOffset = 0;
            int heightOffset = 0;
            foreach (Image image in images){
                graphic.DrawImage(image, new Rectangle(widthOffset,heightOffset,image.Width, image.Height));
                heightOffset +=TILE_SIZE;
                if (heightOffset == img.Height)
                {
                    widthOffset += TILE_SIZE;
                    heightOffset = 0;
                }
            }//HACK(shachar): correct me if I'm wrong, but the next step is to put the city & building images in the respective objects?
            //TODO - generate destruction animations for the buildings. 
            img.Save("test.jpg");
            foreach (Building build in city.Buildings)
            {
                Image image = GetBuildingImage(build);
                /*magic code to convert from the Drawing image to SFML image)*/
                MemoryStream stream = new MemoryStream();
                image.Save(stream, ImageFormat.Png);
                build.Img = new SFML.Graphics.Image(stream);
                /* That's the code to embed the image of the building in the background
                build.Img = image;
                widthOffset = build.StartY * TILE_SIZE;
                heightOffset = build.StartX * TILE_SIZE;
                graphic.DrawImage(image, new Rectangle(widthOffset, heightOffset, image.Width, image.Height));*/
            }

            MemoryStream stream2 = new MemoryStream();
            img.Save(stream2, ImageFormat.Png);
            return new SFML.Graphics.Image(stream2);
        }

        private static Image get_road_image(RoadTile tile)
        {
            Image img = null;
            switch (tile.Image)
            {
                case Images.R_LINE: //it's the same as dead-end, so no breaking here.
                case Images.R_DEAD_END:
                    if ((tile.Rotate == 1)||(tile.Rotate==3))
                    { //Horizontal road
                        if (tile.HWidth == 1)
                            img = new Bitmap(city_images._1_road1);
                        else
                        {
                            if ((tile.HOffset == 0) || (tile.HOffset == tile.HWidth - 1)){
                                img = new Bitmap(city_images._5_road2side);
                                if (tile.HOffset != 0)
                                    img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                }
                            else
                                img = img = new Bitmap(city_images._7_road3middle);
                        }
                        img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    }
                    else { //Vertical road
                        if (tile.VWidth == 1)
                        {
                            img = new Bitmap(city_images._1_road1);

                        }
                        else {
                            if ((tile.VOffset == 0) || (tile.VOffset == tile.VWidth - 1)) {
                                img = new Bitmap(city_images._5_road2side);
                                if (tile.VOffset == tile.VWidth - 1)
                                    img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            }
                            else img = new Bitmap(city_images._7_road3middle);
                        }
                            
                    }
                    break;

                case Images.R_3WAY:
                    //System.Console.WriteLine("3Way. VWidth: " + tile.VWidth +" VOffset: "+tile.VOffset+" HWidth: " + tile.HWidth+" HOffset: "+tile.HOffset+" ROTATE: "+tile.Rotate);
                    switch (tile.Rotate)
                    {
                        case 0: //road connect on east
                            if (tile.VWidth == 1)
                                img = new Bitmap(city_images._2_road1intersect);
                            else {
                              //  System.Console.WriteLine("rotate is 0!");
                                if (tile.VOffset == 0)
                                    img = new Bitmap(city_images._4_road2intersect);
                                else if (tile.VOffset == tile.VWidth - 1)
                                {
                                    img = new Bitmap(city_images._5_road2side);
                                    img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                }
                                else img = new Bitmap(city_images._7_road3middle);                               
                            }
                            break;


                        case 1: //road on north
                            if (tile.HWidth == 1)
                            {
                                img = new Bitmap(city_images._2_road1intersect);
                                img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            }

                            else
                            {
                                //System.Console.WriteLine("rotate is 1!");
                                if (tile.HOffset == 0) {
                                    img = new Bitmap(city_images._4_road2intersect);
                                    img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                }
                                else if (tile.HOffset == tile.HWidth - 1)
                                {
                                    img = new Bitmap(city_images._5_road2side);
                                    img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                }
                                else {
                                    img = new Bitmap(city_images._7_road3middle);
                                    img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                }
                            }
                            break;


                        case 2: //road connects on west side
                            if (tile.VWidth == 1)
                            {
                                img = new Bitmap(city_images._2_road1intersect);
                                img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            }
                            else
                            {
                                //System.Console.WriteLine("rotate is 2!");
                                if (tile.VOffset == tile.VWidth - 1)
                                {
                                    img = new Bitmap(city_images._4_road2intersect);
                                    img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                }
                                else if (tile.VOffset == 0)
                                {
                                    img = new Bitmap(city_images._5_road2side);
                                }
                                else img = new Bitmap(city_images._7_road3middle);
                            }
                            break;
                        case 3: //road connects on south
                            if (tile.HWidth == 1)
                            {
                                img = new Bitmap(city_images._2_road1intersect);
                                img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            }
                            else {
                                //System.Console.WriteLine("rotate is 3!");
                                if (tile.HOffset == tile.HWidth - 1)
                                {
                                    img = new Bitmap(city_images._4_road2intersect);
                                    img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                }
                                else if (tile.HOffset == 0)
                                {
                                    img = new Bitmap(city_images._5_road2side);
                                    img.RotateFlip(RotateFlipType.Rotate90FlipNone); 
                                }
                                else
                                {
                                    img = new Bitmap(city_images._7_road3middle);
                                    img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                }
                            }
                            break;
                        default: img = new Bitmap(city_images._0_empty); break;
                    }

                    
                    break;
                case Images.R_CORNER: 
                    img = new Bitmap(city_images._0_buildingTemp1);//TODO: draw corners and fix this.
                    break;
                case Images.R_FOURWAY:
                    img = new Bitmap(city_images._3_road1mid);//TODO: test. I think we need to draw another 4way images with other sidewalk formations.
                    break;
               
               
                default:
                    img = new Bitmap(city_images._0_empty);
                    break;
            }
            
            /*switch (tile.Flip)
            {
                case 1:
                    img.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    break;
                case 2:
                    img.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    break;
                case 3:
                    img.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                    break;
            }*/

            return img;
        }

        internal static Image GetBuildingImage(Building building)
        {
            var temp = Tuple.Create(building.Dimensions, generateStyle(building.Corp));
            /*if (buildings.ContainsKey(building))
            {
                return buildings[building];
            }
            if (templates.ContainsKey(temp))
            {
                return templates[temp];
            }*/
            Image image = generateBuildingImage(temp);
            buildings[building] = image;
            templates[temp] = image;
            return image;
        }

        private static Image generateBuildingImage(Tuple<Block, BuildingStyle> temp)
        {
            int width = temp.Item1.Length;
            int height = temp.Item1.Width;
            BuildingStyle style = temp.Item2;
            Color basic = Color.Gray;
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
                case(0):
                    img = new Bitmap(city_images._0_buildingmiddle);
                    break;
                case(1):
                    img = new Bitmap(city_images._9_edge);
                    img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case (9):
                    img = new Bitmap(city_images._9_edge);
                    break;
                case (10):
                    img = new Bitmap(city_images._9_edge);
                    img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
                case (90):
                    img = new Bitmap(city_images._9_edge);
                    img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case (11):
                    img = new Bitmap(city_images._9_corner);
                    img.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    break;
                case (91):
                    img = new Bitmap(city_images._9_corner);
                    img.RotateFlip(RotateFlipType.Rotate90FlipX);
                    break;
                case (19):
                    img = new Bitmap(city_images._9_corner);
                    break;
                case (99):
                    img = new Bitmap(city_images._9_corner);
                    img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
            }

            //TODO - needs testing
            return img;
        }

        private static BuildingStyle generateStyle(Corporate corp)
        {
            //TODO - missing function
            int num = corp.Id % 4;
            return (BuildingStyle)num;
        }

    }

    //TODO - needs testing
    class tupleEqualityComparer : IEqualityComparer<Tuple<Block,BuildingStyle>>
    {

        public bool Equals(Tuple<Block, BuildingStyle> first, Tuple<Block, BuildingStyle> second)
        {
            return (first.Item1.EqualSize(second.Item1) && first.Item2 == second.Item2);
        }


        public int GetHashCode(Tuple<Block, BuildingStyle> item)
        {
            int hCode = item.Item1.Length * item.Item1.StartX * item.Item1.StartY * item.Item1.Width *item.Item2.GetHashCode();
            return hCode.GetHashCode();
        }

    }
}
