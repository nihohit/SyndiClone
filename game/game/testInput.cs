using System.Threading;
using System;
namespace Game
{
    class main
    {
        static int Main(string[] args)
        {
            Screen_Manager.ScreenManager.run();
            Console.In.ReadLine();
            return 1;
        }
    }
}
