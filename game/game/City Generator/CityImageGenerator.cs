using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System;
using System.IO;



namespace Game.City_Generator
{
    
    class CityImageGenerator
    {

        
        static int TILE_SIZE = 32;
        
        public static SFML.Graphics.Image convert_to_image(GameBoard city){
            Tile[,] grid = city.Grid; 
            //Image img = new Bitmap(TILE_SIZE*grid.GetLength(0),TILE_SIZE*grid.GetLength(1));
            Image img = new Bitmap(TILE_SIZE * city.Length, TILE_SIZE * city.Depth);
            List<Image> images = new List<Image>();
            for (int i = 0 ; i < city.Length ; i ++)
            {
                for (int j = 0; j < city.Depth; j++)
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
                            images.Add(generateBuildingTile((BuildingTile)grid[i,j]));
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
            int depthOffset = 0;
            int heightOffset = 0;
            foreach (Image image in images){
                graphic.DrawImage(image, new Rectangle(depthOffset,heightOffset,image.Width, image.Height));
                heightOffset +=TILE_SIZE;
                if (heightOffset == img.Height)
                {
                    depthOffset += TILE_SIZE;
                    heightOffset = 0;
                }
            }//HACK(shachar): correct me if I'm wrong, but the next step is to put the city & building images in the respective objects?
            //TODO - generate destruction animations for the buildings. 
            img.Save("test.jpg");

            MemoryStream stream2 = new MemoryStream();
            img.Save(stream2, ImageFormat.Png);
            return new SFML.Graphics.Image(stream2);
        }

        private static Image generateBuildingTile(BuildingTile buildingTile)
        {
            int num = buildingTile.Building.Id;
            Image img = new Bitmap(32, 32);
            Graphics graphic = Graphics.FromImage(img);
            switch (num / 10)
            {
                case(1):
                    graphic.DrawImage(new Bitmap(city_images._1),0,0,16,32);
                    break;
                case (2):
                    graphic.DrawImage(new Bitmap(city_images._2), 0, 0, 16, 32);
                    break;
                case (3):
                    graphic.DrawImage(new Bitmap(city_images._3), 0, 0, 16, 32);
                    break;
                case (4):
                    graphic.DrawImage(new Bitmap(city_images._4), 0, 0, 16, 32);
                    break;
                case (5):
                    graphic.DrawImage(new Bitmap(city_images._5), 0, 0, 16, 32);
                    break;
                case (6):
                    graphic.DrawImage(new Bitmap(city_images._6), 0, 0, 16, 32);
                    break;
                case (7):
                    graphic.DrawImage(new Bitmap(city_images._7), 0, 0, 16, 32);
                    break;
                case (8):
                    graphic.DrawImage(new Bitmap(city_images._8), 0, 0, 16, 32);
                    break;
                case (9):
                    graphic.DrawImage(new Bitmap(city_images._9), 0, 0, 16, 32);
                    break;
                case (0):
                    graphic.DrawImage(new Bitmap(city_images._0), 0, 0, 16, 32);
                    break;
                default:
                    break;
            }

            switch (num % 10)
            {
                case (1):
                    graphic.DrawImage(new Bitmap(city_images._1), 16, 0, 16, 32);
                    break;
                case (2):
                    graphic.DrawImage(new Bitmap(city_images._2), 16, 0, 16, 32);
                    break;
                case (3):
                    graphic.DrawImage(new Bitmap(city_images._3), 16, 0, 16, 32);
                    break;
                case (4):
                    graphic.DrawImage(new Bitmap(city_images._4), 16, 0, 16, 32);
                    break;
                case (5):
                    graphic.DrawImage(new Bitmap(city_images._5), 16, 0, 16, 32);
                    break;
                case (6):
                    graphic.DrawImage(new Bitmap(city_images._6), 16, 0, 16, 32);
                    break;
                case (7):
                    graphic.DrawImage(new Bitmap(city_images._7), 16, 0, 16, 32);
                    break;
                case (8):
                    graphic.DrawImage(new Bitmap(city_images._8), 16, 0, 16, 32);
                    break;
                case (9):
                    graphic.DrawImage(new Bitmap(city_images._9), 16, 0, 16, 32);
                    break;
                case (0):
                    graphic.DrawImage(new Bitmap(city_images._0), 16, 0, 16, 32);
                    break;
                default:
                    break;
            }

            return img;
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
                        if (tile.HDepth == 1)
                            img = new Bitmap(city_images._1_road1);
                        else
                        {
                            if ((tile.HOffset == 0) || (tile.HOffset == tile.HDepth - 1)){
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
                        if (tile.VDepth == 1)
                        {
                            img = new Bitmap(city_images._1_road1);

                        }
                        else {
                            if ((tile.VOffset == 0) || (tile.VOffset == tile.VDepth - 1)) {
                                img = new Bitmap(city_images._5_road2side);
                                if (tile.VOffset == tile.VDepth - 1)
                                    img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            }
                            else img = new Bitmap(city_images._7_road3middle);
                        }
                            
                    }
                    break;

                case Images.R_3WAY:
                    //System.Console.WriteLine("3Way. VDepth: " + tile.VDepth +" VOffset: "+tile.VOffset+" HDepth: " + tile.HDepth+" HOffset: "+tile.HOffset+" ROTATE: "+tile.Rotate);
                    switch (tile.Rotate)
                    {
                        case 0: //road connect on east
                            if (tile.VDepth == 1)
                                img = new Bitmap(city_images._2_road1intersect);
                            else {
                              //  System.Console.WriteLine("rotate is 0!");
                                if (tile.VOffset == 0)
                                    img = new Bitmap(city_images._4_road2intersect);
                                else if (tile.VOffset == tile.VDepth - 1)
                                {
                                    img = new Bitmap(city_images._5_road2side);
                                    img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                }
                                else img = new Bitmap(city_images._7_road3middle);                               
                            }
                            break;


                        case 1: //road on north
                            if (tile.HDepth == 1)
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
                                else if (tile.HOffset == tile.HDepth - 1)
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
                            if (tile.VDepth == 1)
                            {
                                img = new Bitmap(city_images._2_road1intersect);
                                img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            }
                            else
                            {
                                //System.Console.WriteLine("rotate is 2!");
                                if (tile.VOffset == tile.VDepth - 1)
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
                            if (tile.HDepth == 1)
                            {
                                img = new Bitmap(city_images._2_road1intersect);
                                img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            }
                            else {
                                //System.Console.WriteLine("rotate is 3!");
                                if (tile.HOffset == tile.HDepth - 1)
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

    }
}
