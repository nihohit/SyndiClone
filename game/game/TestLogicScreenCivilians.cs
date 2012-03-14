using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    class main
    {
        static int Main(string[] args)
        {
            City_Generator.GameBoard city = City_Generator.CityFactory.createCity(30, 20);
            //City_Generator.GameBoard city = City_Generator.CityFactory.debugCity();

            //Image img = City_Generator.CityImageGenerator.convert_to_image(city);


            Buffers.DisplayBuffer disp = new Buffers.DisplayBuffer();
            Buffers.InputBuffer input = new Buffers.InputBuffer();
            Buffers.SoundBuffer sound = new Buffers.SoundBuffer();
            Graphic_Manager.DisplayManager display = new Graphic_Manager.DisplayManager(30 * 32, 20 * 32, 32, disp, city.Img);
            city.Img.SaveToFile("result.jpg");
            Logic.GameLogic logic = new Logic.GameLogic(disp, input, sound, city,30);
            bool check = true;
            while (check)
            {
                logic.miniLoop();
                display.loop();
            }
            System.Console.ReadKey();
            return 1;
        }

    }
}
