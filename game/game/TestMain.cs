using System;
using System.Drawing;

namespace game
{
    class main
    {
        static int Main(string[] args)
        {
            short[] list1 = {0,1,0};
            short[] list2= {2,3,2};
            short[] list3 = {0,1,0};
            short[][] list = {list1,list2,list3};
            Image img = city_generator.CityImageGenerator.test_convert_to_image(list);
            if (img != null)
            {
                img.Save("images/result.jpg");
            }
            else
            {

            }
            return 1;
        }
    }
}
