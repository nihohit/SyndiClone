using System.Drawing;
using System.Collections.Generic;
using System;



namespace Game.City_Generator
{
    class CityImageGenerator
    {

        enum Style {GENERIC}
        static int tileSize = 32;
        static Dictionary<Building, Image> buildings = new Dictionary<Building, Image>();
        static Dictionary<Tuple<Block, Style>, Image> templates = new Dictionary<Tuple<Block, Style>, Image>();
        
        public static Image convert_to_image(GameBoard city){
            Tile[,] grid = city.Grid; 
            Image img = new Bitmap(tileSize*grid.GetLength(0),tileSize*grid.GetLength(1));
            List<Image> images = new List<Image>();
            for (int i = 0 ; i < grid.GetLength(0) ; i ++)
            {
                for (int j = 0; j < grid.GetLength(0); j++)
                {
                    images.Add(get_image(grid[i,j]));
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

            return img; 
        }

        private static Image get_image(Tile tile)
        {
            Image img = null;
            
            //TODO - enter all types of tiles here. should we tile in buildings, and change the whole picture when they get destryed?
            /*code:
            switch (id/100)
            {
                case 11:
                    img = new Bitmap(city_images._1_road1);
                    break;
                case 12:
                    img = new Bitmap(city_images._2_road1intersect);
                    break;
                case 13:
                    img = new Bitmap(city_images._3_road1mid);
                    break;
                case 14: 
                    img = new Bitmap(city_images._4_road2intersect);
                    break;
                case 15:
                    img = new Bitmap(city_images._5_road2side);
                    break;
                case 16:
                    img = new Bitmap(city_images._6_road3intersect);
                    break;
                case 17:
                    img = new Bitmap(city_images._7_road3middle);
                    break;
                case 18:
                    img = new Bitmap(city_images._8_road3side);
                    break;
                default:
                    img = new Bitmap(city_images._0_empty);
                    break;
            }
            switch (id % 10)
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
            switch ((id % 100) / 10)
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

        private static Image generateBuildingImage(Tuple<Block, Style> temp)
        {
            int length = temp.Item1.Length;
            int width = temp.Item1.Width;
            Style style = temp.Item2;
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

        private static Image getBuildingTile(int length, int width, int i, int j, Style style)
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

            //TODO - needs checking
            return img;
        }

        private static Style generateStyle(Corporate corp)
        {
            //TODO - missing function
            return Style.GENERIC;
        }

    }
}
