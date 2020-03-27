using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace Game.Graphic_Manager {
  #region enumerators
  public enum DisplayCommand { MOVE_VIEW, ZOOM_VIEW, ADD_ENTITY, MOVE_ENTITY, ADD_SHOT, DESTROY_ENTITY }
  public enum DecalType { WRECKAGE, BLOOD, RUBBLE } //TODO - different vehicles wreckages for different vehicles?
 #endregion

 #region Decal

 //this struct represents decals - temporary static pictures.
 public class Decal {
 #region fields

 static readonly uint DECAL_STAY_TIME = FileHandler.GetUintProperty("decal stay time", FileAccessor.DISPLAY);
 static readonly Dictionary<DecalType, Texture> s_decals = new Dictionary<DecalType, Texture> {
 //{DecalType.EXPLOSION, 
 { DecalType.BLOOD, new Texture("images/Decals/bloodsplatter.png") }
 //{DecalType.RUBBLE, 
    };

    private readonly Sprite m_sprite;
    private uint m_stayTime;

    #endregion

    #region public methods

    public Decal(DecalType type) {
      m_sprite = new Sprite(s_decals[type]);
      m_stayTime = DECAL_STAY_TIME;
    }

    public bool IsDone() {
      return m_stayTime == 0;
    }

    public Sprite GetDecal() {
      m_stayTime--;
      return m_sprite;
    }

    public void SetLocation(SFML.System.Vector2f vector) {
      m_sprite.Position = vector;
    }

    #endregion
  }

  #endregion

  #region Animation

  //this struct is in charge of changing Sprites with a limited amount of appearances. Also, supposed not to be connected to an ExternalEntity, but held in a different list. 
  //TODO - replace with a spriteloop and a timer? 
  public struct Animation {
    private readonly List<Sprite> m_order;

    public Animation(List<Sprite> list) {
      m_order = new List<Sprite>(list);
    }

    public Animation(Area area, Logic.EntityType type) {
      // TODO - missing function. The basic idea is to generate an explosion sprite based on size, and whether it's a person, building or vehicle
      m_order = null;
    }

    public Sprite GetNext() {
      m_order.RemoveAt(0);
      Sprite temp = m_order[0];
      return temp;
    }

    public Sprite Current() {
      Sprite temp = m_order[0];
      return temp;
    }

    public bool IsDone() {
      return m_order.Count <= 1;
    }
  }

  #endregion

  #region SpriteLoop

  //this represents a series of Sprites that are repeated one after the other. 
  public struct SpriteLoop {
    private readonly LoopedList<SFML.Graphics.Sprite> m_list;

    public SpriteLoop(List<SFML.Graphics.Sprite> list) {
      m_list = new LoopedList<SFML.Graphics.Sprite>(list);
    }

    public SFML.Graphics.Sprite nextSprite() {
      m_list.Next();
      return CurrentSprite();
    }

    public SFML.Graphics.Sprite CurrentSprite() {
      return m_list.GetValue();
    }

  }

  #endregion

  #region LoopedList

  public class LoopedList<T> {
    private int m_currentIndex;
    private readonly List<T> m_list;

    public LoopedList(List<T> list) {
      m_currentIndex = 0;
      m_list = new List<T>(list);
    }

    public T GetValue() {
      return m_list[m_currentIndex];
    }

    public void Next() {
      m_currentIndex = m_currentIndex + 1;
      if (m_currentIndex == m_list.Count)
        m_currentIndex = 0;
    }
  }

  #endregion
}