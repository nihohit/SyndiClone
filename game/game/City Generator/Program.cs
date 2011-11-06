﻿using System;
using System.Collections.Generic;
using System.Linq;
using Game.City_Generator;

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
            //Random rand = new Random();
            //Application.EnableVisualStyles();
           // Application.SetCompatibleTextRenderingDefault(false);
           // Application.Run(new Form1());
            
            GameBoard city = CityFactory.createMap(100,100);
            //City.BuildingPlacer bp = new City.BuildingPlacer();
            //bp.print();

            //for (int i = 0; i < 900000; ++i)
            //{
            //    bp.getHDimension(7);
            //    bp.getVDimension(7);
            //}
            //// Console.Out.WriteLine("\nGetting a random H num: "+bp.getHDimension(8));
            //bp.print();


            char[,] grid = ((City)city).getGrid();
           
            System.IO.StreamWriter file = new System.IO.StreamWriter("city.mf");
            for (int i = 0; i < city.Length; ++i)
            {
                for (int j = 0; j < city.Width; ++j)
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
