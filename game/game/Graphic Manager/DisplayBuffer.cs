using Game.Graphic_Manager;
using Game.Logic;
using Game.Logic.Entities;
using SFML.Graphics;
using System.Collections.Generic;
using Vector2f = SFML.System.Vector2f;

namespace Game.Buffers {

  public class DisplayBuffer : Game.Buffers.Buffer {

    #region consts

    //TODO - debug, remove
    private const int amountOfReapeatingSpritesInAnimation = 1;

    private readonly uint amountOfDecals = FileHandler.GetUintProperty("decal amount", FileAccessor.DISPLAY);

    #endregion consts

    #region private members

    private readonly Sprite m_selection = new Sprite(new Texture("images/UI/selection.png"));
    private readonly HashSet<Sprite> m_displaySprites = new HashSet<Sprite>();
    private readonly HashSet<Sprite> m_removedSprites = new HashSet<Sprite>();
    private readonly List<Decal> m_decals = new List<Decal>();
    private readonly List<Decal> m_doneDecals = new List<Decal>();
    private readonly HashSet<Game.Graphic_Manager.Animation> m_newAnimations = new HashSet<Game.Graphic_Manager.Animation>();
    private readonly SpriteFinder m_finder = new SpriteFinder();
    private readonly HashSet<IBufferEvent> m_actions = new HashSet<IBufferEvent>();
    private List<VisualEntityInformation> m_visibleEntities = new List<VisualEntityInformation>();
    private bool m_updated = false;
    private bool m_selected = false;
    private bool m_deselected = false;
    private VisualEntityInformation m_selectedEntity; //For UI purposes

    #endregion private members

    #region constructors

    public DisplayBuffer() {
      m_selection.Origin = new Vector2f(m_selection.Texture.Size.X / 2, m_selection.Texture.Size.Y / 2);
    }

    #endregion constructors

    #region public methods

    #region output methods

    public void AnalyzeData() {
      ClearInfo();
      AnalyzeEntities();
      AnalyzeActions();
      DisplayDecals();
      UIDisplay();
      RemoveSprites();
    }

    public HashSet<Sprite> NewSpritesToDisplay() {
      return m_displaySprites;
    }

    public HashSet<Sprite> SpritesToRemove() {
      return m_removedSprites;
    }

    public IEnumerable<Animation> GetAnimations() {
      return m_newAnimations;
    }

    public void ClearInfo() {
      m_newAnimations.Clear();
      m_removedSprites.Clear();
      m_displaySprites.Clear();
    }

    public bool Updated {
      get { return m_updated; }
      set { m_updated = value; System.Threading.Monitor.Pulse(this); }
    }

    #endregion output methods

    #region input methods

    public void ReceiveVisibleEntities(List<VisualEntityInformation> visibleExternalEntityList) {
      m_visibleEntities = visibleExternalEntityList;
    }

    public void ReceiveActions(List<IBufferEvent> actions) {
      m_actions.UnionWith(actions);
    }

    #endregion input methods

    #endregion public methods

    #region private methods

    /*
     * This function is a repeat of the Berensham's line algorithm, this time painting a straight line.
     */

    private Animation CreateNewShot(ShotType shot, Point exit, Point target) {
      List<Sprite> ans = new List<Sprite>();
      int x0 = exit.X;
      int y0 = exit.Y;
      int x1 = target.X;
      int y1 = target.Y;
      int dx = System.Math.Abs(x1 - x0);
      int dy = System.Math.Abs(y1 - y0);
      int sx, sy, e2;
      if (x0 < x1) sx = 1;
      else sx = -1;
      if (y0 < y1) sy = 1;
      else sy = -1;
      int err = dx - dy;
      Sprite temp = null;
      while (!(x0 == x1 & y0 == y1)) {
        e2 = 2 * err;
        if (e2 > -dy) {
          err = err - dy;
          x0 = x0 + sx;
        }
        if (e2 < dx) {
          err = err + dx;
          y0 = y0 + sy;
        }

        temp = m_finder.GetShot(shot);
        //TODO - rotate the shot
        temp.Position = new Vector2f(x0, y0);
        for (int i = 0; i < amountOfReapeatingSpritesInAnimation; i++) {
          ans.Add(temp);
        }
      }
      return new Animation(ans);
    }

    private void RemoveSprites() {
      foreach (Sprite temp in m_removedSprites)
        m_displaySprites.Remove(temp);
    }

    private void AnalyzeActions() {
      foreach (IBufferEvent action in m_actions) {
        switch (action.Type()) {
          case BufferType.EXTERNAL_DESTROY:
            VisualEntityInformation visualInfo = ((ExternalDestroyBufferEvent)action).VisualInfo;
            Sprite temp = m_finder.Remove(visualInfo);
            m_removedSprites.Add(temp);
            if (visualInfo.Type != EntityType.PERSON)
              m_newAnimations.Add(m_finder.GenerateDestoryResults(((ExternalDestroyBufferEvent)action).Area, visualInfo.Type));
            Decal decal = null;
            switch (visualInfo.Type) {
              case (EntityType.BUILDING):
                decal = new Decal(DecalType.RUBBLE);
                break;

              case (EntityType.VEHICLE):
                decal = new Decal(DecalType.WRECKAGE);
                break;

              case (EntityType.PERSON):
                decal = new Decal(DecalType.BLOOD);
                break;
            }

            decal.SetLocation(visualInfo.Position);
            AddDecal(decal);
            break;

          case BufferType.EXTERNAL_CREATE:
            //TODO
            break;

          case BufferType.SHOT:
            m_newAnimations.Add(CreateNewShot(((ShotBufferEvent)action).Shot, ((ShotBufferEvent)action).Exit, ((ShotBufferEvent)action).Target));
            break;

          case BufferType.UNIT_SELECT:
            m_selection.Position = ((UnitSelectBufferEvent)action).Coords.ToVector2f();
            m_selectedEntity = ((UnitSelectBufferEvent)action).VisibleInfo;
            m_selected = true;
            break;

          case BufferType.DESELECT:
            m_selected = false;
            m_deselected = true;
            break;

          case BufferType.SETPATH:
            Sprite toRemove = m_finder.RemovePath(((SetPathActionBufferEvent)action).Entity);
            if (toRemove != null) {
              m_removedSprites.Add(toRemove);
            }
            m_finder.SetPath(((SetPathActionBufferEvent)action).Entity, ((SetPathActionBufferEvent)action).Path, ((SetPathActionBufferEvent)action).Position);
            break;

          case BufferType.DISPLAY_IMAGE:
            m_displaySprites.Add(((DisplayImageBufferEvent)action).Sprite);
            break;

          default:
            throw new System.ArgumentException("the action is not relevant to the display buffer");
        }
      }
      m_actions.Clear();
    }

    private void AddDecal(Decal decal) {
      m_decals.Add(decal);
      if (m_decals.Count > amountOfDecals) {
        Decal removed = m_decals[0];
        m_decals.Remove(removed);
        m_removedSprites.Add(removed.GetDecal());
      }
    }

    private void DisplayDecals() {
      foreach (Decal decal in m_decals) {
        if (decal.IsDone()) {
          m_doneDecals.Add(decal);
          m_removedSprites.Add(decal.GetDecal());
        } else
          m_displaySprites.Add(decal.GetDecal());
      }
      foreach (Decal decal in m_doneDecals)
        m_decals.Remove(decal);
      m_doneDecals.Clear();
    }

    private void UIDisplay() {
      if (m_selected) {
        m_displaySprites.Add(m_selection);
        DisplayAdditionalInfo(m_selectedEntity);
        return;
      }
      if (m_deselected && m_selectedEntity != null) {
        m_removedSprites.Add(m_selection);
        RemoveAdditionalInfo(m_selectedEntity);
        m_deselected = false;
      }
    }

    private void RemoveAdditionalInfo(VisualEntityInformation selectedEntity) {
      if (m_finder.HasPath(selectedEntity)) {
        m_removedSprites.Add(m_finder.GetPath(selectedEntity));
      }
    }

    private void DisplayAdditionalInfo(VisualEntityInformation selectedEntity) {
      if (m_finder.HasPath(selectedEntity)) {
        m_removedSprites.Add(m_finder.GetPath(selectedEntity));
        m_displaySprites.Add(m_finder.NextPathStep(selectedEntity));
      }
    }

    private void AnalyzeEntities() {
      foreach (VisualEntityInformation ent in m_visibleEntities) {
        Sprite temp = m_finder.GetSprite(ent);
        temp.Position = new Vector2f(ent.Position.X, ent.Position.Y);
        m_displaySprites.Add(temp);
      }
      m_visibleEntities.Clear();
    }

    #endregion private methods
  }
}
