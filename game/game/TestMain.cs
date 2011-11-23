using System;
using System.Drawing;

namespace Game
{
    class main
    {
        static int Main(string[] args)
        {
            City_Generator.GameBoard city = City_Generator.CityFactory.createCity(100, 100);
            //City_Generator.GameBoard city = City_Generator.CityFactory.debugCity();

            //Image img = City_Generator.CityImageGenerator.convert_to_image(city);
            Image img = city.Img;
            if (img != null)
            {

                    img.Save("result.jpg");

            }
            else
            {

            }
            //System.Console.ReadKey();
            return 1;
        }

    }
}
