using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Screen_Manager
{
    class FileReader
    {
        Dictionary<String, string> config;
        
        public FileReader()
        {
            config = new Dictionary<string, string>();
            char[] delimiters = {'='};
            string[] text = System.IO.File.ReadAllLines("config/config.ini");
            foreach (string entry in text)
            {
                string[] temp = entry.Split(delimiters);
                config.Add(temp[0], temp[1]);
            }
        }

        internal uint getScreenWidth()
        {
            return Convert.ToUInt16(config["screen width"]);
        }

        internal uint GetScreenHeight()
        {
            return Convert.ToUInt16(config["screen height"]);
        }

        internal uint getBits()
        {
            return Convert.ToUInt16(config["bits"]);
        }
    }
}
