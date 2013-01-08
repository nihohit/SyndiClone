using System;
using System.Collections.Generic;

namespace Game
{
    enum FileAccessor { GENERAL, DISPLAY, SCREEN, LOGIC }

    static class FileHandler
    {
        static Dictionary<string, string> general = new Dictionary<string, string>();
        static Dictionary<string, string> display = new Dictionary<string, string>();
        static Dictionary<string, string> screen = new Dictionary<string, string>();
        static Dictionary<string, string> logic = new Dictionary<string, string>();
        static Dictionary<FileAccessor, Dictionary<string, string>> navigator = new Dictionary<FileAccessor, Dictionary<string, string>>
        {
            {FileAccessor.SCREEN, screen}, 
            {FileAccessor.DISPLAY, display},
            {FileAccessor.GENERAL, general},
            {FileAccessor.LOGIC, logic}
        };

        public static void init()
        {
            setupGeneral();
            setupScreen();
            setupDisplay();
            setupLogic();
        }

        private static void readFromFile(string str, FileAccessor access)
        {
            char[] delimiters = { '=' };
            string[] text = System.IO.File.ReadAllLines("config/" + str + ".ini");
            foreach (string entry in text)
            {
                string[] temp = entry.Split(delimiters);
                navigator[access].Add(temp[0], temp[1]);
            }
        }

        private static void setupScreen()
        {
            readFromFile("screen", FileAccessor.SCREEN);
        }

                private static void setupLogic()
        {
 	        readFromFile("logic", FileAccessor.LOGIC);
        }

        private static void setupDisplay()
        {
            readFromFile("display", FileAccessor.DISPLAY);
        }

        private static void setupGeneral()
        {
            readFromFile("config", FileAccessor.GENERAL);
        }

        internal static UInt16 getUintProperty(string str, FileAccessor access)
        {
            return Convert.ToUInt16(navigator[access][str]);
        }


        public static float getFloatProperty(string str, FileAccessor access)
        {
            return Convert.ToSingle(navigator[access][str]);
        }
    }
}
