using System;
using System.Collections.Generic;

namespace Game
{
    class FileHandler
    {
        Dictionary<String, string> config;

        public FileHandler(string name)
        {
            config = new Dictionary<string, string>();
            char[] delimiters = { '=' };
            string[] text = System.IO.File.ReadAllLines("config/" + name + ".ini");
            foreach (string entry in text)
            {
                string[] temp = entry.Split(delimiters);
                config.Add(temp[0], temp[1]);
            }
        }

        internal uint getUintProperty(string str)
        {
            return Convert.ToUInt16(config[str]);
        }


        public float getFloatProperty(string str)
        {
            return Convert.ToSingle(config[str]);
        }
    }
}
