using System.Drawing;
using System.Collections.Generic;
using System;



namespace game.city_generator
{
    class CityImageGenerator
    {
        static int tileSize = 32;
        
        public static Image convert_to_image(City city){
            short[][] grid = city.get_grid();
            Image img = new Bitmap(32*grid.GetLength(0),32*grid.GetLength(1));
            List<Image> images = new List<Image>();
            foreach (short[] list in grid)
            {
                foreach (short num in list)
                {
                    images.Add(get_image(num));
                }
            }
            Graphics graphic = Graphics.FromImage(img);
            graphic.Clear(Color.Gray);
            int widthOffset = 0;
            int heightOffset = 0;
            foreach (Image image in images){
                graphic.DrawImage(image, new Rectangle(widthOffset,heightOffset,image.Width, image.Height));
                widthOffset +=32;
                heightOffset +=32;
            }

            return img; 
        }

        public static Image test_convert_to_image(short[][] grid)
        {
            int width = tileSize * grid[0].GetLength(0);
            int height = tileSize * grid.GetLength(0);
            Image img = new Bitmap(width,height);
            
            List<Image> images = new List<Image>();
            foreach (short[] list in grid)
            {
                foreach (short num in list)
                {
                    images.Add(get_image(num));
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
                if (widthOffset == width)
                {
                    heightOffset += tileSize;
                    widthOffset = 0;
                }
                
            }

            return img;
        }

        private static Image get_image(short id)
        {
            Image img = null;
            //TODO - enter all types of tiles here. should we tile in buildings, and change the whole picture when they get destryed?
            /*code:
             * WXYZ - 
             * W - type?
             * X - which pictures (1-8)
             * Y - needs flip? 0 - no, 1 - X, 2 - Y, 3 - XY
             * Z - needs Rotate? 0 - no, 1 - 90, 2 - 180, 3 - 270
             * */
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
            }

            return img;
        }

    }
}
