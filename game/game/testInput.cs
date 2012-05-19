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
            FileHandler.init();
            Screen_Manager.ScreenManager.run();
            Console.In.ReadLine();
            return 1;
        }
    }


}
