namespace Game.Screen_Manager {

  /// <summary>
  /// the interface for every screen run by the screen manager
  /// </summary>
  public interface IScreen {

    /// <summary>
    /// This method is called when a screen is loaded as the main screen.
    /// </summary>
    /// <param name="window"></param>
    /// <param name="canvas"></param>
    void GainControl(SFML.Graphics.RenderWindow window, Gwen.Control.Canvas canvas);

    /// <summary>
    /// This is the main method that runs the screen
    /// </summary>
    void Loop();
  }
}
