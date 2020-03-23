namespace Game.Screen_Manager {
  /// <summary>
  /// In this screen, you define the parameters for a new game and call its creation.
  /// </summary>
  class NewGameScreen : IScreen {
    #region static members

    static SFML.Graphics.RenderWindow s_window;
    static Gwen.Control.Canvas s_canvas;
    static Gwen.Skin.Base s_skin;
    static NewGameScreen s_instance;
    static Gwen.Control.NumericUpDown s_xValue, s_yValue, s_civAmount;

    #endregion

    #region constructors

    public static NewGameScreen Instance {
      get { if (s_instance == null) s_instance = new NewGameScreen(); return NewGameScreen.s_instance; }
    }

    private NewGameScreen() { }

    #endregion

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

    #endregion

    #region private methods

    private void InitialiseUI() {
      var newGameButton = new Gwen.Control.Button(s_canvas);
      newGameButton.Text = "New Game";
      newGameButton.SetBounds((int) s_window.Size.X / 2, (int) s_window.Size.Y / 2 - 100, 200, 200);
      newGameButton.AutoSizeToContents = true;
      newGameButton.Pressed += GenerateNewGame;

      s_xValue = new Gwen.Control.NumericUpDown(s_canvas);
      s_xValue.SetBounds((int) s_window.Size.X / 2, (int) s_window.Size.Y / 2 - 60, 100, 30);
      s_xValue.Value = 40;
      s_xValue.Max = 60;
      s_xValue.Min = 40;

      s_civAmount = new Gwen.Control.NumericUpDown(s_canvas);
      s_civAmount.SetBounds((int) s_window.Size.X / 2 + 120, (int) s_window.Size.Y / 2 - 60, 100, 30);
      s_civAmount.Value = 100;
      s_civAmount.Max = 600;
      s_civAmount.Min = 100;

      s_yValue = new Gwen.Control.NumericUpDown(s_canvas);
      s_yValue.SetBounds((int) s_window.Size.X / 2 - 120, (int) s_window.Size.Y / 2 - 60, 100, 30);
      s_yValue.Value = 30;
      s_yValue.Max = 45;
      s_yValue.Min = 30;

      var quitGameButton = new Gwen.Control.Button(s_canvas);
      quitGameButton.Text = "back";
      quitGameButton.SetBounds((int) s_window.Size.X / 2, (int) s_window.Size.Y / 2 + 100, 400, 200);
      quitGameButton.AutoSizeToContents = true;
      quitGameButton.Pressed += GoBackToMainScreen;
    }

    static private void GenerateNewGame(Gwen.Control.Base control) {
      System.Console.Out.WriteLine("new game started");
      IScreen newGame = new GameScreen((int) s_xValue.Value, (int) s_yValue.Value, (int) s_civAmount.Value); //HACK
      EraseScreen();
      newGame.GainControl(s_window, s_canvas);
    }

    static private void GoBackToMainScreen(Gwen.Control.Base control) {
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

    #endregion
  }
}