using System;
using System.Collections.Generic;
using Game.Logic;
using Game.Logic.Entities;
using SFML.Graphics;

namespace Game.Graphic_Manager {
  class SpriteFinder {
    private enum ImageType { PATH } //this enumerator is used for miscelenae. 

 #region fields
 private readonly uint m_distanceBetweenPathObjects = FileHandler.GetUintProperty("path sprite distance", FileAccessor.DISPLAY);
 private readonly Dictionary<VisualEntityInformation, SpriteLoop> m_spriteLoopFinder = new Dictionary<VisualEntityInformation, SpriteLoop>();
 private readonly Dictionary<VisualEntityInformation, SpriteLoop> m_pathFinder = new Dictionary<VisualEntityInformation, SpriteLoop>();

 private readonly Dictionary<Logic.Affiliation, Texture> m_personFinder = new Dictionary<Logic.Affiliation, Texture> { { Logic.Affiliation.INDEPENDENT, new Texture("images/Persons/personblue.png") },
 { Logic.Affiliation.CORP1, new Texture("images/Persons/personpurple.png") },
 { Logic.Affiliation.CORP2, new Texture("images/Persons/persongreen.png") },
 { Logic.Affiliation.CORP4, new Texture("images/Persons/personblack.png") },
 { Logic.Affiliation.CORP3, new Texture("images/Persons/personred.png") },
 { Logic.Affiliation.CIVILIAN, new Texture("images/Persons/personyellow.png") }
    };

    private readonly Dictionary<ShotType, Texture> m_shots = new Dictionary<ShotType, Texture> { { ShotType.PISTOL_BULLET, new Texture("images/shots/bulletshotstraight.png") }

    };

    private readonly Dictionary<ImageType, Texture> m_miscellaneous = new Dictionary<ImageType, Texture> { { ImageType.PATH, new Texture("images/UI/path.png") }
    };

    private readonly List<VisualEntityInformation> m_removed = new List<VisualEntityInformation>();

    #endregion

    #region constructor

    public SpriteFinder() { }

    #endregion

    #region public methods

    public Sprite GetShot(ShotType shot) {
      return new Sprite(m_shots[shot]);
    }

    public Sprite GetSprite(VisualEntityInformation ent) {
      if (!m_spriteLoopFinder.ContainsKey(ent)) {
        m_spriteLoopFinder.Add(ent, GenerateNewSpriteLoop(ent));
      }
      return m_spriteLoopFinder[ent].nextSprite();
    }

    //this function returns an animation as the result of destroying an entity.
    public Graphic_Manager.Animation GenerateDestoryResults(Area area, EntityType type) {
      //TODO - missing function
      return new Graphic_Manager.Animation(area, type);
    }

    public Sprite Remove(VisualEntityInformation ent) {
      Sprite temp = m_spriteLoopFinder[ent].CurrentSprite();
      m_spriteLoopFinder.Remove(ent);
      return temp;
    }

    public Sprite NextSprite(VisualEntityInformation mover) {
      return m_spriteLoopFinder[mover].nextSprite();
    }

    public Sprite RemovePath(VisualEntityInformation ent) {
      if (m_pathFinder.ContainsKey(ent)) {
        Sprite temp = m_pathFinder[ent].CurrentSprite();
        m_pathFinder.Remove(ent);
        return temp;
      }
      return null;
    }

    public void SetPath(VisualEntityInformation ent, List<Direction> path, SFML.Window.Vector2f position) {
      m_pathFinder.Add(ent, GenerateNewComplexPath(path, position));
    }

    public Sprite GetPath(VisualEntityInformation ent) {
      return m_pathFinder[ent].CurrentSprite();
    }

    public Sprite NextPathStep(VisualEntityInformation ent) {
      return m_pathFinder[ent].nextSprite();
    }

    public bool HasPath(VisualEntityInformation ent) {
      return m_pathFinder.ContainsKey(ent);
    }

    #endregion

    #region private methods

    private SpriteLoop GenerateNewSpriteLoop(VisualEntityInformation ent) {
      List<Sprite> list = new List<Sprite>();
      switch (ent.Type) {
      case (Game.Logic.EntityType.BUILDING):
        list.Add(new Sprite(BuildingImageGenerator.GetBuildingSFMLTexture(ent)));
        break;

      case (Logic.EntityType.PERSON):
        list.Add(new Sprite(m_personFinder[ent.VisibleLoyalty]));
        break;

      case (Logic.EntityType.VEHICLE):
        //TODO - missing function
        break;

      }
      return new SpriteLoop(list);
    }

    #region paths

    /*This function receives the parameters for a path, generates its bounding box, and then creates a series of RenderTextures sized as the bounding box, and places the 
     * relevant sprites in RenderTextures. 
     * DistanceBetweenPathObjects basically says how many sprites will be in the resulting loop. 
     */
    private SpriteLoop GenerateNewComplexPath(List<Direction> path, SFML.Window.Vector2f position) {
      Area area = GetPathSize(path);
      List<Sprite> list = new List<Sprite>();
      List<Sprite> newSprite = GenerateNewPathSprite(path, area.Entry);

      list.Clear();
      Sprite temp;
      int count;
      RenderTexture texture;
      //In each recursion of this loop a new RenderTexture is created and each sprite that is relevant to that texture is embedded in it. 
      for (uint i = 0; i < m_distanceBetweenPathObjects; i++) {
        texture = new RenderTexture((uint) area.Size.X, (uint) area.Size.Y);
        for (uint j = 0; j < path.Count / m_distanceBetweenPathObjects; j++) {
          count = (int) (j * m_distanceBetweenPathObjects + i);
          if (count < newSprite.Count) {
            texture.Draw(newSprite[count]);
          }
        }
        texture.Display();
        temp = new Sprite(new Texture(texture.Texture));
        temp.Origin = new SFML.Window.Vector2f(area.Entry.X, area.Entry.Y);
        temp.Position = position;
        list.Add(temp);
      }

      return new SpriteLoop(list);
    }

    //This function computes the size of a path texture, and its beginning point inside the square.
    private Area GetPathSize(List<Direction> path) {
      Texture pathTexture = m_miscellaneous[ImageType.PATH];
      uint maxSize = Math.Max(pathTexture.Size.X, pathTexture.Size.Y);
      uint halfSize = maxSize / 2;
      uint xSize = maxSize, xPos = halfSize, ySize = maxSize, yPos = halfSize, x = halfSize, y = halfSize;
      foreach (Direction dir in path) {
        switch (dir) {
        case (Game.Logic.Direction.DOWN):
          y++;
          break;
        case (Game.Logic.Direction.DOWNLEFT):
          y++;
          x--;
          break;
        case (Game.Logic.Direction.DOWNRIGHT):
          y++;
          x++;
          break;
        case (Game.Logic.Direction.LEFT):
          x--;
          break;
        case (Game.Logic.Direction.RIGHT):
          x++;
          break;
        case (Game.Logic.Direction.UP):
          y--;
          break;
        case (Game.Logic.Direction.UPLEFT):
          y--;
          x--;
          break;
        case (Game.Logic.Direction.UPRIGHT):
          x++;
          y--;
          break;
        }
        if (x < halfSize) { xPos++; xSize++; x = halfSize; }
        if (y < halfSize) { yPos++; ySize++; y = halfSize; }
        if (x >= xSize - halfSize) { xSize++; }
        if (y >= ySize - halfSize) { ySize++; }
      }

      return new Area(new Point((int) xPos, (int) yPos), new Vector((int) xSize, (int) ySize));
    }

    //This function returns a new path sprite, rotated to the correct way. 
    private List<Sprite> GenerateNewPathSprite(List<Direction> path, Point entry) {
      List<Sprite> newList = new List<Sprite>();
      Vector initial = new Vector(entry.X, entry.Y);
      Sprite temp;
      Texture texture = m_miscellaneous[ImageType.PATH];
      uint xSize = texture.Size.X / 2, ySize = texture.Size.Y / 2;
      foreach (Direction dir in path) {
        temp = new Sprite(m_miscellaneous[ImageType.PATH]);
        temp.Origin = new SFML.Window.Vector2f(xSize, ySize);
        switch (dir) {
        case (Direction.UP):
          break;
        case (Direction.DOWN):
          temp.Rotation = 180F;
          break;
        case (Direction.LEFT):
          temp.Rotation = 270;
          break;
        case (Direction.RIGHT):
          temp.Rotation = 90F;
          break;
        case (Direction.UPRIGHT):
          temp.Rotation = 45F;
          break;
        case (Direction.UPLEFT):
          temp.Rotation = 315F;
          break;
        case (Direction.DOWNRIGHT):
          temp.Rotation = 135F;
          break;
        case (Direction.DOWNLEFT):
          temp.Rotation = 225F;
          break;
        }
        initial = initial.AddVector(Vector.DirectionToVector(dir));
        temp.Position = initial.ToVector2f();
        newList.Add(temp);
      }

      return newList;
    }

    #endregion
    #endregion

  }

  class VisualInformationEqualityComparer : IEqualityComparer<VisualEntityInformation> {

    public bool Equals(VisualEntityInformation first, VisualEntityInformation second) {
      return (first.EntityId.Equals(second.EntityId));
    }

    public int GetHashCode(VisualEntityInformation item) {
      int hCode = item.EntityId.GetHashCode();
      return hCode.GetHashCode();
    }

  }
}