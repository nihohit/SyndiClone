using System.Threading;

namespace Game
{
    class main
    {
        static int Main(string[] args)
        {
            City_Generator.GameBoard city = City_Generator.CityFactory.createCity(30, 20);

            Buffers.DisplayBuffer disp = new Buffers.DisplayBuffer();
            Buffers.InputBuffer input = new Buffers.InputBuffer();
            Buffers.SoundBuffer sound = new Buffers.SoundBuffer();
            Graphic_Manager.DisplayManager display = new Graphic_Manager.DisplayManager(30 * 32, 20 * 32, 32, disp, city.Img);
            Logic.GameLogic logic = new Logic.GameLogic(disp, input, sound, city, 100);

            /*
            while (true)
            {
                logic.loop();
                display.loop();
            }*/

            
            Thread logicThread = new Thread(new ThreadStart(logic.run));
            Thread graphicThread = new Thread(new ThreadStart(display.run));
            logicThread.Start();
            graphicThread.Start();
            logicThread.Join();
            graphicThread.Join();
            

            return 1;
        }

    }
}
