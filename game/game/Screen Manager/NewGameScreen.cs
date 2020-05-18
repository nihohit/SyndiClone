using System;

namespace Game.Screen_Manager {

  /// <summary>
  /// In this screen, you define the parameters for a new game and call its creation.
  /// </summary>
  internal class NewGameScreen : IScreen {

    #region static members

    private static SFML.Graphics.RenderWindow s_window;
    private static Gwen.Control.Canvas s_canvas;
    private static Gwen.Skin.Base s_skin;
    private static NewGameScreen s_instance;
    private static Gwen.Control.NumericUpDown s_xValue, s_yValue, s_civAmount;

    #endregion static members

    #region constructors

    public static NewGameScreen Instance {
      get { if (s_instance == null) s_instance = new NewGameScreen(); return NewGameScreen.s_instance; }
    }

    private NewGameScreen() { }

    #endregion constructors

    #region IScreen

    public void GainControl(SFML.Graphics.RenderWindow window, Gwen.Control.Canvas canvas) {
      if (s_instance == null) s_instance = new NewGameScreen();
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

    #endregion IScreen

    #region private methods

    private void InitialiseUI() {
      var halfX = (int) s_window.Size.X / 2;
      var halfY = (int) s_window.Size.Y / 2;
      var newGameButton = new Gwen.Control.Button(s_canvas) {
        Text = "New Game",
        AutoSizeToContents = true
      };
      newGameButton.Pressed += GenerateNewGame;
      newGameButton.SetBounds(halfX, halfY - 100, 200, 200);

      s_xValue = new Gwen.Control.NumericUpDown(s_canvas) {
        Value = 40,
        Max = 60,
        Min = 40
      };
      s_xValue.SetBounds(halfX, halfY - 60, 100, 30);

      s_civAmount = new Gwen.Control.NumericUpDown(s_canvas) {
        Value = 100,
        Max = 600,
        Min = 100
      };
      s_civAmount.SetBounds(halfX + 120, halfY - 60, 100, 30);

      s_yValue = new Gwen.Control.NumericUpDown(s_canvas) {
        Value = 30,
        Max = 45,
        Min = 30
      };
      s_yValue.SetBounds(halfX - 120, halfY - 60, 100, 30);

      var quitGameButton = new Gwen.Control.Button(s_canvas) {
        Text = "back",
        AutoSizeToContents = true
      };
      quitGameButton.Pressed += GoBackToMainScreen;
      quitGameButton.SetBounds(halfX, halfY + 100, 400, 200);
    }

    static private void GenerateNewGame(Gwen.Control.Base control, EventArgs args) {
      System.Console.Out.WriteLine("new game started");
      IScreen newGame = new GameScreen((int) s_xValue.Value, (int) s_yValue.Value, (int) s_civAmount.Value); //HACK
      EraseScreen();
      newGame.GainControl(s_window, s_canvas);
    }

    static private void GoBackToMainScreen(Gwen.Control.Base control, EventArgs args) {
      System.Console.Out.WriteLine("back to main screen");
      EraseScreen();
      IScreen mainScreen = MainScreen.Instance;
      EraseScreen();
      mainScreen.GainControl(s_window, s_canvas);
    }

    static private void EraseScreen() {
      s_canvas.DeleteAllChildren();
      s_window.Clear();
    }

    #endregion private methods
  }
}
