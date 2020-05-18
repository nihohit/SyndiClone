using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Game.Buffers;
using SFML.Graphics;
using SFML.Window;
using Vector2f = SFML.System.Vector2f;

namespace Game.Graphic_Manager {

  //This class is in charge of displaying an actual game instance.
  internal class GameDisplay {
    /*TODO - right now there's a central problem, that some UI elements are handled by the manager
     * and some by the buffer (basically, what depends on whether they demand knowledge of logic events is handled by the buffer and what
     * demands knowledge of screen events is handled by the manager).
     * This should be remedied - maybe move everything to the buffer.
     */

    #region fields

    private readonly Sprite m_crosshair = new Sprite(new Texture("images/UI/crosshairs.png"));
    private readonly HashSet<Sprite> m_displayedSprites = new HashSet<Sprite>();
    private readonly HashSet<Sprite> m_removedSprites = new HashSet<Sprite>();
    private readonly HashSet<Animation> m_animations = new HashSet<Animation>();

    private readonly RenderWindow m_mainWindow;
    private readonly DisplayBuffer m_buffer;
    private readonly InputBuffer m_input; //HACK - is this needed? maybe put that in the display buffer too?
    private readonly Sprite m_background;
    private readonly View m_UIview;

    //DEBUG & PERFORMANCE TOOLS
    //TODO - remove.
    private readonly Stopwatch remove = new Stopwatch();

    private readonly Stopwatch DisplayWatch = new Stopwatch();
    private readonly Stopwatch synch = new Stopwatch();
    private readonly Stopwatch update = new Stopwatch();
    private readonly Stopwatch other = new Stopwatch();
    private int runs = 0;

    #endregion fields

    #region constructor

    public GameDisplay(Game.Buffers.DisplayBuffer buffer, Texture background, RenderWindow window, InputBuffer input) {
      m_buffer = buffer;
      m_input = input;
      m_background = new Sprite(background);
      m_mainWindow = window;
      m_crosshair.Origin = new Vector2f(m_crosshair.Texture.Size.X / 2, m_crosshair.Texture.Size.Y / 2);
      m_UIview = new View(new Vector2f(background.Size.X / 2, background.Size.Y / 2), new Vector2f(background.Size.X, background.Size.Y));
      DisplayWatch.Start();
      /*TODO - remove
#if DEBUG
            Logic.Pathfinding.AdvancedVisibleAStar.Setup(buffer);
#endif
             */
    }

    #endregion constructor

    #region public methods

    public void Loop() {
      other.Start();
      m_mainWindow.Clear();
      m_mainWindow.Draw(m_background);
      other.Stop();
      UpdateInfo();
      DisplayWatch.Start();
      Display();
      DisplayWatch.Stop();
      runs++;
    }

    public void Display() {
      m_mainWindow.Display();
    }

    //TODO - debug, remove
    public void DisplayStats() {
      DisplayWatch.Stop();
      Console.Out.WriteLine("synch was " + synch.Elapsed + " , display was " + DisplayWatch.Elapsed + " , update was " + update.Elapsed + " , remove was " + remove.Elapsed + " , other was " + other.Elapsed);
      Console.Out.WriteLine("amount of graphic loops: " + runs + " average milliseconds per frame: " + DisplayWatch.ElapsedMilliseconds / runs);
    }

    #endregion public methods

    #region private methods

    #region communication

    //This is the central
    private void UpdateInfo() {
      HandleInputBuffer();
      HandleDisplayBuffer();
      remove.Start();
      RemoveSprites();
      remove.Stop();
      update.Start();
      EnterAnimations();
      DisplaySprites();
      DrawUI();
      update.Stop();
    }

    private void HandleDisplayBuffer() {
      synch.Start();

      lock(m_buffer) {
        if (m_buffer.Updated) {
          //TODO - needs reviewing
          m_buffer.AnalyzeData();
          FindSpritesToRemove();
          FindSpritesToDisplay();
          UpdateAnimations();
          m_buffer.Updated = false;
        }
      }
      synch.Stop();
    }

    private void HandleInputBuffer() {
      lock(m_input) {
        if (m_input.GraphicInput) {
          List<IBufferEvent> list = m_input.GetEvents(InputModuleAccessors.Graphics);
          foreach (IBufferEvent action in list) {
            if (action.Type() == BufferType.ENDGAME)
              DisplayStats();
          }
        }
      }
    }

    private void DrawUI() {
      m_crosshair.Position = m_mainWindow.MapPixelToCoords(Mouse.GetPosition(m_mainWindow));
      m_mainWindow.Draw(m_crosshair);
    }

    private void FindSpritesToDisplay() {
      m_displayedSprites.UnionWith(m_buffer.NewSpritesToDisplay());
    }

    private void FindSpritesToRemove() {
      m_removedSprites.UnionWith(m_buffer.SpritesToRemove());
    }

    private void UpdateAnimations() {
      m_animations.UnionWith(m_buffer.GetAnimations());
    }

    #endregion communication

    #region info handling

    private void DisplaySprites() {
      foreach (Sprite sprite in m_displayedSprites) {
        m_mainWindow.Draw(sprite);
      }
    }

    private void EnterAnimations() {
      foreach (Animation animation in m_animations.ToList()) {
        m_removedSprites.Add(animation.Current());
        if (animation.IsDone()) {
          m_animations.Remove(animation);
        } else {
          m_displayedSprites.Add(animation.GetNext());
        }
      }
    }

    private void RemoveSprites() {
      foreach (Sprite sprite in m_removedSprites) {
        m_displayedSprites.Remove(sprite);
      }
      m_removedSprites.Clear();
    }

    #endregion info handling

    #endregion private methods
  }
}
