using System;
using System.Threading;
using Game.Buffers;
using Gwen;
using SFML.Graphics;
using SFML.Window;

namespace Game.Screen_Manager {
  /// <summary>
  /// This is the manager of the screens - it checks which screen is currently loaded, and loops through it.
  /// </summary>
  static class ScreenManager {
    #region static members

    private static Gwen.Input.SFML s_Input;
    private static RenderWindow s_window;

    #endregion

    #region properties

    public static IScreen CurrentScreen { get; set; }

    #endregion

    #region public methods

    public static void Run() {
      s_window = new RenderWindow(new VideoMode(600, 400), "main display");
      Gwen.Renderer.SFML UIrenderer = new Gwen.Renderer.SFML(s_window);
      Gwen.Skin.TexturedBase skin = new Gwen.Skin.TexturedBase(UIrenderer, "DefaultSkin.png");
      Gwen.Control.Canvas canvas = new Gwen.Control.Canvas(skin);
      canvas.SetSize((int) s_window.Size.X, (int) s_window.Size.Y);
      canvas.MouseInputEnabled = true;
      s_Input = new Gwen.Input.SFML();
      s_Input.Initialize(canvas, s_window);

      // set up SFML input handlers
      s_window.MouseButtonPressed += WindowMouseButtonPressed;
      s_window.MouseButtonReleased += WindowMouseButtonReleased;
      s_window.MouseMoved += WindowMouseMoved;
      s_window.Closed += WindowClosed;
      CurrentScreen = MainScreen.Instance;
      CurrentScreen.GainControl(s_window, canvas);

      while (s_window.IsOpen()) // quit if main window is closed
      {
        CurrentScreen.Loop();
      }
    }

    #endregion

    #region input handlers
    // input handlers - just pass mouse data to Gwen

    private static void WindowClosed(object sender, System.EventArgs e) {
      s_window.Close();
    }

    static void WindowMouseMoved(object sender, MouseMoveEventArgs e) {
      s_Input.ProcessMessage(e);
    }

    static void WindowMouseButtonPressed(object sender, MouseButtonEventArgs e) {
      s_Input.ProcessMessage(new Gwen.Input.SFMLMouseButtonEventArgs(e, true));
    }

    static void WindowMouseButtonReleased(object sender, MouseButtonEventArgs e) {
      s_Input.ProcessMessage(new Gwen.Input.SFMLMouseButtonEventArgs(e, false));
    }

    #endregion
  }
}