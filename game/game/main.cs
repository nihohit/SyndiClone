using SFML.Graphics;
using SFML.Window;

namespace Game {
  class main {
    private static Gwen.Input.SFML m_Input;
    private static RenderWindow mainWindow;

    public static void Main() {
      mainWindow = new RenderWindow(new VideoMode(600, 400), "main display");
      Gwen.Renderer.SFML UIrenderer = new Gwen.Renderer.SFML(mainWindow);
      Gwen.Skin.TexturedBase skin = new Gwen.Skin.TexturedBase(UIrenderer, "DefaultSkin.png");
      Gwen.Control.Canvas canvas = new Gwen.Control.Canvas(skin);
      canvas.SetSize((int) mainWindow.Size.X, (int) mainWindow.Size.Y);
      canvas.MouseInputEnabled = true;
      m_Input = new Gwen.Input.SFML();
      m_Input.Initialize(canvas, mainWindow);
      Gwen.Control.Button newGameButton = new Gwen.Control.Button(canvas);

      // set up SFML input handlers
      mainWindow.MouseButtonPressed += window_MouseButtonPressed;
      mainWindow.MouseButtonReleased += window_MouseButtonReleased;
      mainWindow.MouseMoved += window_MouseMoved;
      mainWindow.Closed += window_Closed;

      newGameButton.Text = "New Game";
      newGameButton.SetBounds((int) mainWindow.Size.X / 2, (int) mainWindow.Size.Y / 2, 200, 200);
      newGameButton.AutoSizeToContents = true;
      newGameButton.Pressed += newGameScreen;

      while (mainWindow.IsOpen()) // quit if main window is closed
      {
        mainWindow.SetActive();
        mainWindow.DispatchEvents();
        mainWindow.Clear();
        canvas.RenderCanvas();
        mainWindow.Display();
      }
    }

    static void window_Closed(object sender, System.EventArgs e) {
      mainWindow.Close();
    }

    // input handlers - just pass mouse data to Gwen

    static void window_MouseMoved(object sender, MouseMoveEventArgs e) {
      m_Input.ProcessMessage(e);
    }

    static void window_MouseButtonPressed(object sender, MouseButtonEventArgs e) {
      m_Input.ProcessMessage(new Gwen.Input.SFMLMouseButtonEventArgs(e, true));
    }

    static void window_MouseButtonReleased(object sender, MouseButtonEventArgs e) {
      m_Input.ProcessMessage(new Gwen.Input.SFMLMouseButtonEventArgs(e, false));
    }

    static private void newGameScreen(Gwen.Control.Base control) {
      mainWindow.Close(); // modified to do something other to write to console
    }
  }
}