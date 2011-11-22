using System;
using System.Drawing;

namespace Game
{
    class main
    {
        static int Main(string[] args)
        {
            Image img = City_Generator.CityImageGenerator.convert_to_image(City_Generator.CityFactory.createCity());
            if (img != null)
            {

                    img.Save("result.jpg");

            }
            else
            {

            }
            return 1;
        }

    }
}
