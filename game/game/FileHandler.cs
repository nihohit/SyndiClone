using System;
using System.Collections.Generic;

namespace Game
{
    enum FileAccessor { GENERAL, DISPLAY, SCREEN, LOGIC }

    //TODO - replace all of this complexity with GraphicConfiguration, LogicConfiguration etc. classes which will be loaded from files and written back to them.

    static class FileHandler
    {
        #region static dictionaries

        private static Dictionary<string, string> s_general = new Dictionary<string, string>();
        private static Dictionary<string, string> s_display = new Dictionary<string, string>();
        private static Dictionary<string, string> s_screen = new Dictionary<string, string>();
        private static Dictionary<string, string> s_logic = new Dictionary<string, string>();
        static Dictionary<FileAccessor, Dictionary<string, string>> s_navigator = new Dictionary<FileAccessor, Dictionary<string, string>>
        {
            {FileAccessor.SCREEN, s_screen}, 
            {FileAccessor.DISPLAY, s_display},
            {FileAccessor.GENERAL, s_general},
            {FileAccessor.LOGIC, s_logic}
        };

        #endregion

        #region public methods

        public static void Init()
        {
            SetupGeneral();
            SetupScreen();
            SetupDisplay();
            SetupLogic();
        }

        public static UInt16 GetUintProperty(string str, FileAccessor access)
        {
            return Convert.ToUInt16(s_navigator[access][str]);
        }

        public static Int32 GetIntProperty(string str, FileAccessor access)
        {
            return Convert.ToInt32(s_navigator[access][str]);
        }

        public static float GetFloatProperty(string str, FileAccessor access)
        {
            return Convert.ToSingle(s_navigator[access][str]);
        }

        #endregion

        #region private methods

        private static void ReadFromFile(string str, FileAccessor access)
        {
            char[] delimiters = { '=' };
            string[] text = System.IO.File.ReadAllLines("config/" + str + ".ini");
            foreach (string entry in text)
            {
                string[] temp = entry.Split(delimiters);
                s_navigator[access].Add(temp[0], temp[1]);
            }
        }

        private static void SetupScreen()
        {
            ReadFromFile("screen", FileAccessor.SCREEN);
        }

        private static void SetupLogic()
        {
 	        ReadFromFile("logic", FileAccessor.LOGIC);
        }

        private static void SetupDisplay()
        {
            ReadFromFile("display", FileAccessor.DISPLAY);
        }

        private static void SetupGeneral()
        {
            ReadFromFile("config", FileAccessor.GENERAL);
        }

        #endregion
    }
}
