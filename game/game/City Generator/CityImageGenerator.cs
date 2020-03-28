using SFML.Graphics;
using System;
using System.Collections.Generic;

namespace Game.City_Generator {

  internal static class CityImageGenerator {
    private static readonly uint TILE_SIZE = FileHandler.GetUintProperty("tile size", FileAccessor.GENERAL);

    #region public methods

    public static SFML.Graphics.Texture ConvertToImage(GameBoard city) {
      //TODO - set up all the images ahead of time in a dictionary, so we won't load them all the time.

      Tile[,] grid = city.Grid;
      //Image img = new Image(TILE_SIZE*grid.GetLength(0),TILE_SIZE*grid.GetLength(1));
      uint x = Convert.ToUInt16(TILE_SIZE * city.Length);
      uint y = Convert.ToUInt16(TILE_SIZE * city.Depth);
      Image img = new Image(x, y);
      List<Image> images = new List<Image>();
      for (int i = 0; i < city.Length; i++) {
        for (int j = 0; j < city.Depth; j++) {
          switch (grid[i, j].Type) {
            case ContentType.ROAD: {
              images.Add(GetRoadImage((RoadTile)grid[i, j]));
              break;
            }
            case ContentType.BUILDING: {
              images.Add(GenerateBuildingTile((BuildingTile)grid[i, j]));
              break;
            }
            //TODO - other tiles?
            default: {
              images.Add(new Image("images/City/0_empty.jpg"));
              break;
            }
          }
        }
      }
      uint depthOffset = 0;
      uint heightOffset = 0;
      foreach (Image image in images) {
        img.Copy(image, depthOffset, heightOffset);
        heightOffset += TILE_SIZE;
        if (heightOffset == img.Size.Y) {
          depthOffset += TILE_SIZE;
          heightOffset = 0;
        }
      }
      //img.SaveToFile("result.png");
      return new Texture(img);
    }

    #endregion public methods

    #region private methods

    private static Image GenerateBuildingTile(BuildingTile buildingTile) {
      int num = buildingTile.Building.Id;
      Image img = new Image(32, 32);
      switch (num / 10) {
        case (1):
          img.Copy(new Image("images/City/1.png"), 0, 0);
          break;

        case (2):
          img.Copy(new Image("images/City/2.png"), 0, 0);
          break;

        case (3):
          img.Copy(new Image("images/City/3.png"), 0, 0);
          break;

        case (4):
          img.Copy(new Image("images/City/4.png"), 0, 0);
          break;

        case (5):
          img.Copy(new Image("images/City/5.png"), 0, 0);
          break;

        case (6):
          img.Copy(new Image("images/City/6.png"), 0, 0);
          break;

        case (7):
          img.Copy(new Image("images/City/7.png"), 0, 0);
          break;

        case (8):
          img.Copy(new Image("images/City/8.png"), 0, 0);
          break;

        case (9):
          img.Copy(new Image("images/City/9.png"), 0, 0);
          break;

        case (0):
          img.Copy(new Image("images/City/0.png"), 0, 0);
          break;

        default:
          break;
      }

      switch (num % 10) {
        case (1):
          img.Copy(new Image("images/City/1.png"), 16, 0);
          break;

        case (2):
          img.Copy(new Image("images/City/2.png"), 16, 0);
          break;

        case (3):
          img.Copy(new Image("images/City/3.png"), 16, 0);
          break;

        case (4):
          img.Copy(new Image("images/City/4.png"), 16, 0);
          break;

        case (5):
          img.Copy(new Image("images/City/5.png"), 16, 0);
          break;

        case (6):
          img.Copy(new Image("images/City/6.png"), 16, 0);
          break;

        case (7):
          img.Copy(new Image("images/City/7.png"), 16, 0);
          break;

        case (8):
          img.Copy(new Image("images/City/8.png"), 16, 0);
          break;

        case (9):
          img.Copy(new Image("images/City/9.png"), 16, 0);
          break;

        case (0):
          img.Copy(new Image("images/City/0.png"), 16, 0);
          break;

        default:
          break;
      }

      return img;
    }

    private static Image GetRoadImage(RoadTile tile) {
      Image img;
      switch (tile.TileImage) {
        case Images.R_LINE: //it's the same as dead-end, so no breaking here.
        case Images.R_DEAD_END:
          if ((tile.Rotate == 1) || (tile.Rotate == 3)) { //Horizontal road
            if (tile.HDepth == 1) {
              img = new Image("images/City/1_road1.jpg");
            } else {
              if ((tile.HOffset == 0) || (tile.HOffset == tile.HDepth - 1)) {
                img = new Image("images/City/5_road2side.jpg");
                if (tile.HOffset != 0)
                  //img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                  img.FlipHorizontally();
              } else {
                img = img = new Image("images/City/7_road3middle.jpg");
              }
            }
            img = Rotate90(img);
          } else { //Vertical road
            if (tile.VDepth == 1) {
              img = new Image("images/City/1_road1.jpg");
            } else {
              if ((tile.VOffset == 0) || (tile.VOffset == tile.VDepth - 1)) {
                img = new Image("images/City/5_road2side.jpg");
                if (tile.VOffset == tile.VDepth - 1)
                  //img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                  img.FlipHorizontally();
              } else img = new Image("images/City/7_road3middle.jpg");
            }
          }
          break;

        case Images.R_3WAY:
          //System.Console.WriteLine("3Way. VDepth: " + tile.VDepth +" VOffset: "+tile.VOffset+" HDepth: " + tile.HDepth+" HOffset: "+tile.HOffset+" ROTATE: "+tile.Rotate);
          switch (tile.Rotate) {
            case 0: //road connect on east
              if (tile.VDepth == 1)
                img = new Image("images/City/2_road1intersect.jpg");
              else {
                //  System.Console.WriteLine("rotate is 0!");
                if (tile.VOffset == 0)
                  img = new Image("images/City/4_road2intersect.jpg");
                else if (tile.VOffset == tile.VDepth - 1) {
                  img = new Image("images/City/5_road2side.jpg");
                  //img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                  img.FlipHorizontally();
                } else img = new Image("images/City/7_road3middle.jpg");
              }
              break;

            case 1: //road on north
              if (tile.HDepth == 1) {
                img = new Image("images/City/2_road1intersect.jpg");

                //img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                img = Rotate90(img);
              } else {
                //System.Console.WriteLine("rotate is 1!");
                if (tile.HOffset == 0) {
                  img = new Image("images/City/4_road2intersect.jpg");
                  img = Rotate90(img);
                } else if (tile.HOffset == tile.HDepth - 1) {
                  img = new Image("images/City/5_road2side.jpg");
                  //img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                  img = Rotate270(img);
                } else {
                  img = new Image("images/City/7_road3middle.jpg");
                  //img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                  img = Rotate90(img);
                }
              }
              break;

            case 2: //road connects on west side
              if (tile.VDepth == 1) {
                img = new Image("images/City/2_road1intersect.jpg");
                img.FlipHorizontally();
              } else {
                //System.Console.WriteLine("rotate is 2!");
                if (tile.VOffset == tile.VDepth - 1) {
                  img = new Image("images/City/4_road2intersect.jpg");
                  img.FlipHorizontally();
                } else if (tile.VOffset == 0) {
                  img = new Image("images/City/5_road2side.jpg");
                } else img = new Image("images/City/7_road3middle.jpg");
              }
              break;

            case 3: //road connects on south
              if (tile.HDepth == 1) {
                img = new Image("images/City/2_road1intersect.jpg");
                //img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                img = Rotate270(img);
              } else {
                //System.Console.WriteLine("rotate is 3!");
                if (tile.HOffset == tile.HDepth - 1) {
                  img = new Image("images/City/4_road2intersect.jpg");
                  //img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                  img = Rotate270(img);
                } else if (tile.HOffset == 0) {
                  img = new Image("images/City/5_road2side.jpg");
                  //img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                  img = Rotate90(img);
                } else {
                  img = new Image("images/City/7_road3middle.jpg");
                  //img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                  img = Rotate270(img);
                }
              }
              break;

            default:
              img = new Image("images/City/0_empty.jpg");
              break;
          }
          break;

        case Images.R_CORNER:
          img = new Image("images/City/0_buildingTemp1.jpg"); //TODO: draw corners and fix
          break;

        case Images.R_FOURWAY:
          img = new Image("images/City/3_road1mid.jpg"); //TODO: test. I think we need to draw another 4way images with other sidewalk formations.
          break;

        default:
          img = new Image("images/City/0_empty.jpg");
          break;
      }

      return img;
    }

    private static Image Rotate270(Image temp) {
      uint maxY = temp.Size.Y;
      uint maxX = temp.Size.X;
      Image val = new Image(maxY, maxX);
      for (uint x = 0; x < maxX; x++) {
        for (uint y = 0; y < maxY; y++) {
          val.SetPixel(y, maxX - 1 - x, temp.GetPixel(x, y));
        }
      }
      return val;
    }

    private static Image Rotate90(Image temp) {
      uint maxY = temp.Size.Y;
      uint maxX = temp.Size.X;
      Image val = new Image(maxY, maxX);
      for (uint x = 0; x < maxX; x++) {
        for (uint y = 0; y < maxY; y++) {
          val.SetPixel(maxY - 1 - y, x, temp.GetPixel(x, y));
        }
      }
      return val;
    }

    private static Image Rotate180(Image temp) {
      uint maxY = temp.Size.Y;
      uint maxX = temp.Size.X;
      Image val = new Image(maxX, maxY);
      for (uint x = 0; x < maxX; x++) {
        for (uint y = 0; y < maxY; y++) {
          val.SetPixel(maxX - 1 - x, maxY - 1 - y, temp.GetPixel(x, y));
        }
      }
      return val;
    }

    #endregion private methods
  }
}
