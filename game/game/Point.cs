using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Maps
{
    class Point
    {
        int _xLoc, _yLoc;
        /** 
         * This constructor creates a point with x,y parameters
         */
        internal Point(int x, int y) {
            _xLoc = x;
            _yLoc = y;
        }

        /** 
         * this constructor gives the point a random value in the range minX-maxX and minY-maxY
         */
        internal Point(int minX, int maxX, int minY, int maxY) {
            Random rand = new Random();
            _xLoc = rand.Next(minX, maxX);
            _yLoc = rand.Next(minY, maxY);
        }

        public String ToString() {
            return "(" + _xLoc + "," + _yLoc + ")";
        }



        public static void run (){
            Point p1 = new Point(4, 7);
            Point p2 = new Point(5, 10, 1, 6);
            System.Console.WriteLine("BOO");
            System.Console.WriteLine(p1.ToString());
            System.Windows.Forms.MessageBox.Show(p2.ToString(),"p2");
            

        }
    }
}
