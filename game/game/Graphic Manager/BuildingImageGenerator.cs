using System;
using System.Collections.Generic;
using Game.Logic.Entities;
using SFML.Graphics;
using Vector2f = SFML.System.Vector2f;

namespace Game.Graphic_Manager {

  #region Block

  //this converts a Vector to smaller sized vectors, based on a set tile size.
  //Didn't use vector, in order to make sure this is used only in this palce.
  public struct Block {
    private static readonly uint TILE_SIZE = FileHandler.GetUintProperty("tile size", FileAccessor.GENERAL);

    private uint m_x, m_y;

    public Block(uint x, uint y) {
      m_x = x / TILE_SIZE;
      m_y = y / TILE_SIZE;
    }

    public uint Y { get { return m_y; } }

    public uint X { get { return m_x; } }

    public bool EqualSize(Block check) {
      return ((X == check.X) && (Y == check.Y));
    }
  }

  #endregion Block

  #region BuildingImageGenerator

  //this class creates the building images
  public static class BuildingImageGenerator {

    #region enumerators

    private enum BuildingParts { EDGE, CORNER, CENTER }

    internal enum BuildingStyle { BLUE, RED, GREEN, YELLOW }

    #endregion enumerators

    #region static fields

    private static readonly uint TILE_SIZE = FileHandler.GetUintProperty("tile size", FileAccessor.GENERAL);
    private static Dictionary<VisualEntityInformation, Texture> s_buildings = new Dictionary<VisualEntityInformation, Texture>(new VisualInformationEqualityComparer());
    private static Dictionary<Tuple<Block, BuildingStyle>, Texture> s_templates = new Dictionary<Tuple<Block, BuildingStyle>, Texture>(new TupleEqualityComparer());

    private static Dictionary<BuildingParts, Texture> s_parts = new Dictionary<BuildingParts, Texture> { { BuildingParts.CENTER, new Texture("images/buildings/0_buildingmiddle.gif") },
      { BuildingParts.CORNER, new Texture("images/buildings/9_corner.gif") },
      { BuildingParts.EDGE, new Texture("images/buildings/9_edge.gif") }
    };

    #endregion static fields

    #region public methods

    public static SFML.Graphics.Texture GetBuildingSFMLTexture(VisualEntityInformation building) {
      return GetBuildingImage(building);
    }

    #endregion public methods

    #region private methods

    //checks if the image exists, if not, then if an equivalent image exist, and if not, creates a new image.
    private static Texture GetBuildingImage(VisualEntityInformation building) {
      if (s_buildings.ContainsKey(building)) return s_buildings[building];
      var temp = Tuple.Create(new Block((uint) building.Size.X, (uint) building.Size.Y), generateStyle(building.VisibleLoyalty));
      Texture image = null;
      if (s_templates.ContainsKey(temp)) {
        image = s_templates[temp];
      } else {
        image = new Texture(GenerateBuildingImage(temp));
        s_templates.Add(temp, image);
      }
      s_buildings.Add(building, image);
      return image;
    }

    //generate a building image from a block
    private static Texture GenerateBuildingImage(Tuple<Block, BuildingStyle> temp) {
      //TODO - if too heavy for realtime computing, try instead of generating an image, generating a rendertexture.
      uint depth = Convert.ToUInt16(temp.Item1.X);
      uint height = Convert.ToUInt16(temp.Item1.Y);
      BuildingStyle style = temp.Item2;
      Color basic = Color.Black;
      switch (style) {
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

      RenderTexture img = new RenderTexture(TILE_SIZE * depth, TILE_SIZE * height);
      img.Draw(new Sprite(new Texture(new Image(TILE_SIZE * depth, TILE_SIZE * height, basic))));
      List<Sprite> sprites = new List<Sprite>();
      for (int i = 1; i <= depth; i++) {
        for (int j = 1; j <= height; j++) {
          sprites.Add(GetBuildingTile(depth, height, i, j, style));
        }
      }

      uint depthOffset = TILE_SIZE / 2;
      uint heightOffset = TILE_SIZE / 2;
      //this copies all the smaller images into the larger one.
      foreach (Sprite sprite in sprites) {
        if (sprite != null) {
          sprite.Position = new Vector2f(depthOffset, heightOffset);
          img.Draw(sprite);
        }

        heightOffset += TILE_SIZE;
        if (heightOffset > img.Size.Y) {
          depthOffset += TILE_SIZE;
          heightOffset = TILE_SIZE / 2;
        }
      }

      img.Display();
      //img.SaveToFile("building_Test.jpg");
      return new Texture(img.Texture);
    }

    private static Sprite GetBuildingTile(uint length, uint depth, int x, int y, BuildingStyle style) {
      //TODO - account for style
      Sprite img = null;
      int id = 0;
      if (x == 1) {
        id += 1;
      }
      if (x == length) {
        id += 9;
      }
      if (y == 1) {
        id += 10;
      }
      if (y == depth) {
        id += 90;
      }
      switch (id) {
        case (0):
          img = new Sprite(s_parts[BuildingParts.CENTER]);
          break;

        case (1):
          img = new Sprite(s_parts[BuildingParts.EDGE]) {
            Rotation = 180f
          };
          break;

        case (9):
          img = new Sprite(s_parts[BuildingParts.EDGE]);
          break;

        case (10):
          img = new Sprite(s_parts[BuildingParts.EDGE]) {
            Rotation = 270f
          };;
          break;

        case (90):
          img = new Sprite(s_parts[BuildingParts.EDGE]) {
            Rotation = 90f
          };
          break;

        case (11):
          img = new Sprite(s_parts[BuildingParts.CORNER]) {
            Rotation = 270f
          };
          break;

        case (91):
          img = new Sprite(s_parts[BuildingParts.CORNER]) {
            Rotation = 180f
          };
          break;

        case (19):
          img = new Sprite(s_parts[BuildingParts.CORNER]);
          break;

        case (99):
          img = new Sprite(s_parts[BuildingParts.CORNER]) {
            Rotation = 90f
          };
          break;

        default:
          break;
      }
      if (img != null) img.Origin = new Vector2f(img.Texture.Size.X / 2, img.Texture.Size.Y / 2);
      else throw new NullReferenceException();
      return img;
    }

    private static BuildingStyle generateStyle(Logic.Affiliation aff) {
      //TODO - missing function
      int num = (int) aff;
      return (BuildingStyle) num;
    }

    #endregion private methods

    #region TupleEqualityComparer

    internal class TupleEqualityComparer : IEqualityComparer<Tuple<Block, BuildingStyle>> {

      public bool Equals(Tuple<Block, BuildingStyle> first, Tuple<Block, BuildingStyle> second) {
        return first.Item1.EqualSize(second.Item1) && first.Item2 == second.Item2;
      }

      public int GetHashCode(Tuple<Block, BuildingStyle> item) {
        int hCode = (int) (item.Item1.X * item.Item1.Y * item.Item2.GetHashCode());
        return hCode.GetHashCode();
      }
    }

    #endregion TupleEqualityComparer
  }

  #endregion BuildingImageGenerator
}
