using System;
using System.Drawing;
using System.Collections.Generic;

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
            Logic.GameLogic logic = new Logic.GameLogic(disp, input, sound, city);
            city.Img.SaveToFile("result.jpg");
            Graphic_Manager.DisplayManager display = new Graphic_Manager.DisplayManager(30*32, 20*32, 32, disp, city.Img);
            List<Logic.Entities.Entity> buildings = new List<Logic.Entities.Entity>();
            foreach (City_Generator.Building build in city.Buildings)
            {
                buildings.Add(Logic.GameBoardToGameGridConverter.convertBuilding(build));
            }
            disp.receiveVisibleEntities(buildings);

            display.loop();

            Console.ReadKey();
            //System.Console.ReadKey();
            return 1;
        }

    }
}
