using System.Drawing;
using System.Collections.Generic;
using System;



namespace Game.City_Generator
{
    enum BuildingStyle { GENERIC }
    class CityImageGenerator
    {

        
        static int tileSize = 32;
        static Dictionary<Building, Image> buildings = new Dictionary<Building, Image>();
        static Dictionary<Tuple<Block, BuildingStyle>, Image> templates = new Dictionary<Tuple<Block, BuildingStyle>, Image>(new tupleEqualityComparer());
        
        public static Image convert_to_image(GameBoard city){
            Tile[,] grid = city.Grid; 
            Image img = new Bitmap(tileSize*grid.GetLength(0),tileSize*grid.GetLength(1));
            List<Image> images = new List<Image>();
            for (int i = 0 ; i < grid.GetLength(0) ; i ++)
            {
                for (int j = 0; j < grid.GetLength(0); j++)
                {
                    switch(grid[i, j].Type)
                    {
                        case ContentType.ROAD:
                        {
                            images.Add(get_road_image((RoadTile)grid[i, j]));
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
                widthOffset +=tileSize;
                heightOffset +=tileSize;
            }
            foreach (Building build in city.Buildings)
            {
                Image image = GetBuildingImage(build);
                widthOffset = build.StartX * tileSize;
                heightOffset = build.StartY * tileSize;
                graphic.DrawImage(image, new Rectangle(widthOffset, heightOffset, image.Width, image.Height));
            }

            return img ;
        }

        private static Image get_road_image(RoadTile tile)
        {
            Image img = null;
            switch (tile.Image)
            {
                case Images.R_LINE: //it's the same as dead-end, so no breaking here.
                case Images.R_DEAD_END:
                    if (tile.Rotate == 1)
                    { //vertical road
                        if (tile.VWidth == 1)
                            img = new Bitmap(city_images._1_road1);
                        else
                        {
                            if ((tile.VOffset == 0) || (tile.VOffset == tile.VWidth - 1)){
                                img = new Bitmap(city_images._5_road2side);
                                }
                            else
                                img = img = new Bitmap(city_images._7_road3middle);
                        }
                    }
                    else { //horizontal road
                        if (tile.HWidth == 1)
                        {
                            img = new Bitmap(city_images._1_road1);

                        }
                        else {
                            if ((tile.HOffset == 0) || (tile.HOffset == tile.HWidth - 1)) {
                                img = new Bitmap(city_images._5_road2side);
                                if (tile.HOffset == tile.HWidth - 1)
                                    img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            }
                            else img = new Bitmap(city_images._7_road3middle);
                        }
                            img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    }
                    break;

                case Images.R_3WAY:
                    switch (tile.Rotate)
                    {
                        case 0: //road connect on east
                            if (tile.VWidth == 1)
                                img = new Bitmap(city_images._2_road1intersect);
                            else {
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
                            if (tile.VWidth == 1)
                            {
                                img = new Bitmap(city_images._2_road1intersect);
                                img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            }
                            else {
                                if (tile.HOffset == tile.HWidth - 1)
                                {
                                    img = new Bitmap(city_images._2_road1intersect);
                                    img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                }
                                else if (tile.HOffset == 0)
                                {
                                    img = new Bitmap(city_images._5_road2side);
                                    img.RotateFlip(RotateFlipType.Rotate270FlipNone); //could be 90 as well
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
                    img = new Bitmap(city_images._0_empty);//TODO: draw corners and fix this.
                    break;
                case Images.R_FOURWAY:
                    img = new Bitmap(city_images._3_road1mid);//TODO: test. I think we need to draw another 4way images with other sidewalk formations.
                    break;
               
               
                default:
                    img = new Bitmap(city_images._0_empty);
                    break;
            }
            switch (tile.Rotate)
            {
                case 1:
                    img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 2:
                    img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case 3:
                    img.RotateFlip(RotateFlipType.Rotate270FlipNone);
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
            if (buildings.ContainsKey(building))
            {
                return buildings[building];
            }
            var temp = Tuple.Create(building.Dimensions, generateStyle(building.Corp));
            if (templates.ContainsKey(temp))
            {
                return templates[temp];
            }
            Image image = generateBuildingImage(temp);
            buildings[building] = image;
            templates[temp] = image;
            return image;
        }

        private static Image generateBuildingImage(Tuple<Block, BuildingStyle> temp)
        {
            int length = temp.Item1.Length;
            int width = temp.Item1.Width;
            BuildingStyle style = temp.Item2;
            Image img = new Bitmap(tileSize * length, tileSize * width);
            List<Image> images = new List<Image>();
            for (int i = 1; i <= length; i++)
            {
                for (int j = 1; j <= width; j++)
                {
                    images.Add(getBuildingTile(length,width,i,j,style));
                }
            }
            Graphics graphic = Graphics.FromImage(img);
            graphic.Clear(Color.Gray);
            int widthOffset = 0;
            int heightOffset = 0;
            foreach (Image image in images)
            {
                graphic.DrawImage(image, new Rectangle(widthOffset, heightOffset, image.Width, image.Height));
                widthOffset += tileSize;
                heightOffset += tileSize;
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
            return BuildingStyle.GENERIC;
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
