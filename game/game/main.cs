namespace Game {

  internal class main {

    private static int Main(string[] args) {
      FileHandler.Init();
      Screen_Manager.ScreenManager.Run();
      return 1;
    }
  }
}
