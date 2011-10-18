using System.Drawing;
using System.Collections.Generic;



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
            int width = tileSize * grid.GetLength(0);
            int height = tileSize * grid[0].GetLength(0);
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
            switch (id)
            {
                case 1:
                    img = new Bitmap(city_images.road1);
                    break;
                case 2:
                    img = new Bitmap(city_images.road1);
                    img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 3:
                    img = new Bitmap(city_images.road1mid);
                    break;
                default:
                    img = new Bitmap(city_images.empty);
                    break;
            }

            return img;
        }

    }
}
