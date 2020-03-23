using System;
using System.Collections.Generic;
using System.Threading;
using SFML.Graphics;
using SFML.Window;

namespace Game {
  internal class main {
    static int Main(string[] args) {
      FileHandler.Init();
      Screen_Manager.ScreenManager.Run();
      return 1;
    }
  }
}