using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Game.Maps;

namespace Game
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
           // Application.SetCompatibleTextRenderingDefault(false);
           // Application.Run(new Form1());
            //Console.Out.WriteLine("ho");
            City city = CityFactory.createMap(100,100);
            char[,] grid = city.getGrid();
            //Console.Out.WriteLine("hey!!");
            System.IO.StreamWriter file = new System.IO.StreamWriter("city.mf");
            for (int i=0; i<city.getLen(); ++i) {
                for (int j = 0; j < city.getWid(); ++j)
                {
                    Console.Out.Write(grid[i, j]);
                    file.Write(grid[i, j]);
                }
                Console.Out.Write("\r\n");
                file.Write("\r\n");
            }
            file.Close();

            
           System.Console.ReadKey();
        }
    }
}
