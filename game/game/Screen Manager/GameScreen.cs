using System;
using System.Threading;
using Game.Buffers;
using SFML.Graphics;
using SFML.Window;
using Vector2f = SFML.System.Vector2f;

namespace Game.Screen_Manager {

  static internal class ViewExtensions {

    public static FloatRect pixelRect(this View view) {
      return new FloatRect(view.Center - (view.Size / 2), view.Size);
    }

    public static float Right(this FloatRect rect) {
      return rect.Left + rect.Width;
    }

    public static float Bottom(this FloatRect rect) {
      return rect.Top + rect.Height;
    }
  }

  /// <summary>
  /// This screen is meant to represent a new game, including creating the game.
  /// </summary>
  //TODO - add a loading screen
  internal class GameScreen : IScreen {

    #region fields

    private float m_backgroundX, m_backgroundY;
    private RenderWindow m_mainWindow;
    private Buffers.InputBuffer m_input;
    private Graphic_Manager.GameDisplay m_gameScreen;
    private bool m_inGame = false, m_activeGame = false;
    private float m_xMove, m_yMove;
    private float m_mouseX, m_mouseY;
    private Thread m_logicThread = null;
    private uint m_mouseScrollAmount = 0;
    private int m_mapX, m_mapY, m_civAmount;
    private float m_minY, m_topY, m_topX;

    #endregion fields

    #region file read values

    //currently, at least.

    private readonly float AMOUNT_OF_PIXEL_ALLOWED_OFFSCREEN = FileHandler.GetFloatProperty("pixels allowed off background", FileAccessor.SCREEN);
    private readonly uint SCREEN_EDGE_PERCENT_FOR_MOUSE_SCROLLING = FileHandler.GetUintProperty("mouse scroll range", FileAccessor.SCREEN);
    private readonly uint SPEED_OF_MOUSE_SCROLL = FileHandler.GetUintProperty("mouse scroll speed", FileAccessor.SCREEN);
    private readonly uint FRAME_RATES = FileHandler.GetUintProperty("frame rates", FileAccessor.SCREEN);
    private readonly float MIN_X = FileHandler.GetFloatProperty("minimal view size", FileAccessor.SCREEN); //these represent the bounds on the size of the view

    #endregion file read values

    #region IScreen Implementation

    public void GainControl(SFML.Graphics.RenderWindow window, Gwen.Control.Canvas canvas) {
      ScreenManager.CurrentScreen = this;
      m_mainWindow = window;
      Initialise();
      StartNewGame();
    }

    public void Loop() {
      m_mainWindow.DispatchEvents();
      ScrollScreen();
      if (m_inGame) {
        m_gameScreen.Loop();
      }
    }

    #endregion IScreen Implementation

    #region initialisation

    private void Initialise() {
      m_minY = (MIN_X / m_mainWindow.Size.X) * m_mainWindow.Size.Y;
      IntialiseWindow();
      InitialiseUI();
    }

    private void IntialiseWindow() {
      m_mainWindow.SetFramerateLimit(FRAME_RATES);
      m_mainWindow.SetActive(false);
      m_mainWindow.Closed += OnClosed;
      m_mainWindow.KeyPressed += OnKeyPressed;
      m_mainWindow.GainedFocus += OnGainingFocus;
      m_mainWindow.LostFocus += OnLosingFocus;
      m_mainWindow.MouseWheelScrolled += Zooming;
      m_mainWindow.MouseButtonPressed += MouseClick;
      m_mainWindow.MouseMoved += MouseMoved;
      m_mainWindow.SetMouseCursorVisible(false);
      m_mainWindow.Resized += Resizing;
    }

    private void InitialiseUI() {
      /*Gwen.Renderer.SFML UIrenderer = new Gwen.Renderer.SFML(mainWindow);
      Gwen.Skin.Base skin = new Gwen.Skin.TexturedBase(UIrenderer, "DefaultSkin.png");
      Gwen.Control.Canvas canvas = new Gwen.Control.Canvas(skin);*/
    }

    #endregion initialisation

    #region constructors

    public GameScreen(int mapX, int mapY, int civAmount) {
      m_mapY = mapY;
      m_mapX = mapX;
      m_civAmount = civAmount;
    }

    #endregion constructors

    #region starting new game

    private void StartNewGame() {
      City_Generator.GameBoard city = City_Generator.CityFactory.CreateCity(m_mapX, m_mapY);
      Buffers.DisplayBuffer disp = new Buffers.DisplayBuffer();
      m_input = new Buffers.InputBuffer();
      Buffers.SoundBuffer sound = new Buffers.SoundBuffer();
      Logic.GameLogic logic = new Logic.GameLogic(disp, m_input, sound, city, m_civAmount);
      Texture background = city.BoardImage;
      m_gameScreen = new Graphic_Manager.GameDisplay(disp, background, m_mainWindow, m_input);
      m_topX = background.Size.X;
      m_topY = (background.Size.X / m_mainWindow.Size.X) * m_mainWindow.Size.Y;
      m_backgroundX = background.Size.X;
      m_backgroundY = background.Size.Y;
      m_inGame = true;
      m_activeGame = true;
      m_logicThread = new Thread(new ThreadStart(logic.Run));
      m_logicThread.Start();
    }

    //TODO - remove, this is a debug tool
    public Logic.GameLogic StartNewGameThreadless(int mapX, int mapY, int civAmount) {
      City_Generator.GameBoard city = City_Generator.CityFactory.CreateCity(mapX, mapY);
      Buffers.DisplayBuffer disp = new Buffers.DisplayBuffer();
      m_input = new Buffers.InputBuffer();
      Buffers.SoundBuffer sound = new Buffers.SoundBuffer();
      Logic.GameLogic logic = new Logic.GameLogic(disp, m_input, sound, city, civAmount);
      Texture background = city.BoardImage;
      m_gameScreen = new Graphic_Manager.GameDisplay(disp, background, m_mainWindow, m_input);
      m_topX = background.Size.X;
      m_topY = (background.Size.X / m_mainWindow.Size.X) * m_mainWindow.Size.Y;
      m_backgroundX = background.Size.X;
      m_backgroundY = background.Size.Y;
      m_inGame = true;
      m_activeGame = true;
      return logic;
    }

    #endregion starting new game

    #region control

    private void OnKeyPressed(object sender, KeyEventArgs e) {
      if (e.Code == Keyboard.Key.Left) {
        CenterNew(-10, 0);
      }

      if (e.Code == Keyboard.Key.Right) {
        CenterNew(10, 0);
      }

      if (e.Code == Keyboard.Key.Up) {
        CenterNew(0, -10);
        return;
      }

      if (e.Code == Keyboard.Key.Down) {
        CenterNew(0, 10);
        return;
      }
      if (e.Code == Keyboard.Key.Space && m_inGame) {
        lock(m_input) {
          if (m_activeGame)
            m_input.EnterEvent(new PauseBufferEvent());
          else
            m_input.EnterEvent(new UnPauseBufferEvent());
          m_activeGame = !m_activeGame;
        }
        return;
      }
      if (e.Code == Keyboard.Key.Escape) {
        OnClosed(sender, e);
        return;
      }
      /*
      if (e.Code == Keyboard.Key.Return)
      {
          startNewGame(40, 30, 200);
          return;
      }*/
    }

    /// <summary>
    /// Function called when the window is closed
    /// </summary>
    private void OnClosed(object sender, EventArgs e) {
      Window window = (Window) sender;
      window.Close();
      m_input.EnterEvent(new EndGameBufferEvent());
      m_logicThread.Join();
      //ScreenManager.Active = false;
    }

    //TODO - why can't we return from alt-tabbing?
    private void OnGainingFocus(object sender, EventArgs e) {
      if (m_input != null) {
        m_input.EnterEvent(new UnPauseBufferEvent());
        m_activeGame = true;
      }
    }

    private void OnLosingFocus(object sender, EventArgs e) {
      if (m_input != null) {
        m_input.EnterEvent(new PauseBufferEvent());
        m_activeGame = false;
      }
    }

    private void MouseClick(object sender, MouseButtonEventArgs e) {
      if (e.Button == Mouse.Button.Left) {
        Vector2f temp = m_mainWindow.MapPixelToCoords(new SFML.System.Vector2i(e.X, e.Y));
        Vector result = new Vector(Convert.ToInt16(temp.X), Convert.ToInt16(temp.Y));
        Console.Out.WriteLine("clicked on " + result.X + " , " + result.Y);
        m_input.EnterEvent(new MouseSelectBufferEvent(result, 1)); //TODO - proper playerId
        return;
      }
      if (e.Button == Mouse.Button.Right) { m_input.EnterEvent(new CancelActionBufferEvent()); }
    }

    #endregion control

    #region view

    private void Resizing(object sender, SizeEventArgs e) {
      Resize(e.Width, e.Height);
    }

    private void Resize(uint width, uint height) {
      View temp = m_mainWindow.GetView();
      temp.Size = new Vector2f(width, height);
    }

    private void ScrollScreen() {
      if (m_mouseScrollAmount < 100)
        m_mouseScrollAmount = m_mouseScrollAmount + SPEED_OF_MOUSE_SCROLL;
      else {
        CenterNew(m_xMove, m_yMove);
        m_mouseScrollAmount = m_mouseScrollAmount - 100;
      }
    }

    private void adjustZoom(float scale, View currentView) {
      var newX = currentView.Size.X * scale;
      var newY = currentView.Size.Y * scale;
      scale = Math.Min(scale, m_topX / newX);
      scale = Math.Min(scale, m_topY / newY);
      scale = Math.Max(scale, MIN_X / newX);
      scale = Math.Max(scale, m_minY / newY);
      currentView.Zoom(scale);
    }

    private void bindViewportToBoard(View currentView) {
      float newX = 0;
      float newY = 0;
      var pixelRect = currentView.pixelRect();
      var right = pixelRect.Right();
      var bottom = pixelRect.Bottom();
      Console.WriteLine(pixelRect);
      Console.WriteLine(currentView.Size);
      Console.WriteLine(currentView.Center);

      if (right > m_topX) {
        newX = right - m_topX;
      } else if (pixelRect.Left < 0) {
        newX = pixelRect.Left;
      }
      if (pixelRect.Top < 0) {
        newY = pixelRect.Top;
      } else if (bottom > m_topY) {
        newY = bottom - m_topY;
      }
      currentView.Center = new Vector2f(currentView.Center.X - newX, currentView.Center.Y - newY);
    }

    private void Zooming(object sender, MouseWheelScrollEventArgs e) {
      float scale = 1 - (e.Delta / 10F);
      if (scale == 1) {
        return;
      }
      View currentView = ((RenderWindow) sender).GetView();
      adjustZoom(scale, currentView);
      bindViewportToBoard(currentView);
    }

    //TODO - known bug, after mouse exits screen, scrolling gets stuck in one direction.
    private void MouseMoved(object sender, MouseMoveEventArgs e) {
      m_mouseX = e.X;
      m_mouseY = e.Y;
      View currentView = ((RenderWindow) sender).GetView();
      var pixelRect = currentView.pixelRect();
      var right = pixelRect.Right();
      var bottom = pixelRect.Bottom();
      var scrollZoneSize = new Vector2f(pixelRect.Width * SCREEN_EDGE_PERCENT_FOR_MOUSE_SCROLLING / 100, pixelRect.Height * SCREEN_EDGE_PERCENT_FOR_MOUSE_SCROLLING / 100);
      m_xMove = 0;
      m_yMove = 0;

      //Console.Out.WriteLine(" mouse location: {0} , {1} , max: {2} , {3}, min: {4} , {5}", m_mouseX, m_mouseY, maxX, maxY, minX, minY);
      if (m_mouseX - pixelRect.Left < scrollZoneSize.X) {
        m_xMove = -(scrollZoneSize.X - m_mouseX + pixelRect.Left) / scrollZoneSize.X;
      }
      if (right - m_mouseX < scrollZoneSize.X) {
        m_xMove = (scrollZoneSize.X - (right - m_mouseX)) / scrollZoneSize.X;
      }
      if (m_mouseY - pixelRect.Top < scrollZoneSize.Y) {
        m_yMove = -(scrollZoneSize.Y - m_mouseY + pixelRect.Top) / scrollZoneSize.Y;
      }
      if (bottom - m_mouseY < scrollZoneSize.Y) {
        m_yMove = (scrollZoneSize.Y - (bottom - m_mouseY)) / scrollZoneSize.Y;
      }
    }

    private void CenterNew(float x, float y) {
      View currentView = m_mainWindow.GetView();
      float halfX = (currentView.Size.X / 2);
      float halfY = (currentView.Size.Y / 2);
      y = Math.Min(currentView.Center.Y + y, m_backgroundY - halfY + AMOUNT_OF_PIXEL_ALLOWED_OFFSCREEN);
      y = Math.Max(y, halfY - AMOUNT_OF_PIXEL_ALLOWED_OFFSCREEN);
      x = Math.Min(currentView.Center.X + x, m_backgroundX - halfX + AMOUNT_OF_PIXEL_ALLOWED_OFFSCREEN);
      x = Math.Max(x, halfX - AMOUNT_OF_PIXEL_ALLOWED_OFFSCREEN);
      currentView.Center = new Vector2f(x, y);
      m_mainWindow.SetView(currentView);
    }

    #endregion view
  }
}
