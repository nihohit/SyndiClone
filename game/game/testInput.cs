using System.Threading;
using System;
using SFML.Window;
using SFML.Graphics;
using System.Collections.Generic;

namespace Game
{

    class main
    {

        static int Main(string[] args)
        {
            FileHandler.Init();
            Screen_Manager.ScreenManager.Run();
            return 1;
        }
    }


}
