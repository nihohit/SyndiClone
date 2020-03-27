using Gwen;
using Gwen.Control;
using Gwen.ControlInternal;
using Gwen.Input;
using System;

namespace Game.Screen_Manager {
  /// <summary>
  /// This screen is the main screen - the first interactive screen that the game loads.
  /// </summary>
  class MainScreen : IScreen {
    #region static members

    static SFML.Graphics.RenderWindow s_window;
    static MainScreen s_instance;
    static Gwen.Control.Canvas s_canvas;

    #endregion

    #region constructors

    public static MainScreen Instance {
      get { if (s_instance == null) s_instance = new MainScreen(); return MainScreen.s_instance; }
    }

    private MainScreen() { }

    #endregion

    #region IScreen Implementation

    public void GainControl(SFML.Graphics.RenderWindow window, Gwen.Control.Canvas canvas) {
      System.Console.Out.WriteLine("entered main");
      if (s_instance == null) s_instance = new MainScreen();
      ScreenManager.CurrentScreen = s_instance;
      s_window = window;
      s_canvas = canvas;
      InitialiseUI();
    }

    public void Loop() {
      s_window.SetActive();
      s_window.DispatchEvents();
      s_window.Clear();
      s_canvas.RenderCanvas();
      s_window.Display();
    }

    #endregion

    #region private methods

    private void InitialiseUI() {
      var newGameButton = new Gwen.Control.Button(s_canvas);
      newGameButton.Text = "New Game";
      newGameButton.SetBounds((int) s_window.Size.X / 2, (int) s_window.Size.Y / 2 - 100, 200, 200);
      newGameButton.AutoSizeToContents = true;
      newGameButton.Pressed += GenerateNewGameScreen;

      var quitGameButton = new Gwen.Control.Button(s_canvas);
      quitGameButton.Text = "Quit Game";
      quitGameButton.SetBounds((int) s_window.Size.X / 2, (int) s_window.Size.Y / 2 + 100, 400, 200);
      quitGameButton.AutoSizeToContents = true;
      quitGameButton.Pressed += QuitGame;
    }

    static private void GenerateNewGameScreen(Gwen.Control.Base control, EventArgs args) {
      System.Console.Out.WriteLine("button clicked");
      IScreen newGame = NewGameScreen.Instance;
      EraseScreen();
      newGame.GainControl(s_window, s_canvas);
    }

    static private void QuitGame(Gwen.Control.Base control, EventArgs args) {
      System.Console.Out.WriteLine("game ended");
      EraseScreen();
      s_canvas.Dispose();
      s_window.Close();
    }

    static private void EraseScreen() {
      var childs = s_canvas.Children;
      s_canvas.DeleteAllChildren();
      s_window.Clear();
    }

    #endregion
  }
}