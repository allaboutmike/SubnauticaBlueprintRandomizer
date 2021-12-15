using System;
using System.Collections.Generic;
using System.IO;

namespace BlueprintRandomizer
{
    class Unlock
    {
        private static string FILE_NAME = "unlocks.csv";

        public string randomizerKey;
        public bool standardSet;
        public List<TechType> invalidTechTypes;

        public Unlock(string key, bool standardSet, List<TechType> invalid)
        {
            this.randomizerKey = key;
            this.standardSet = standardSet;
            this.invalidTechTypes = invalid;
        }

        public static List<Unlock> LoadUnlocks()
        {
            var fullPath = Path.Combine(BlueprintRandomizer.baseDirectory, FILE_NAME);
            string[] lines;

            lines = File.ReadAllLines(fullPath);

            var unlockList = new List<Unlock>();

            bool header = true;
            foreach (var line in lines)
            {
                if (!header && !String.IsNullOrEmpty(line))
                {
                    string[] values = line.Split(',');
                    if (values.Length < 3)
                    {
                        throw new Exception("Unable to parse unlocks, invalid line length: " + values);
                    }
                    
                    var key = values[0];
                    var standardSet = values[1] == "1";
                    var invalidTypes = StringToTechTypeList(values[2]);

                    unlockList.Add(new Unlock(key, standardSet, invalidTypes));
                }
                else
                {
                    header = false;
                }
            }

            return unlockList;
        }

        private static List<TechType> StringToTechTypeList(string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                // Nothing invalid, return empty list
                return new List<TechType>();
            }

            var values = str.Split('|');
            var list = new List<TechType>();

            foreach (string v in values)
            {
                try
                {
                    list.Add((TechType)Enum.Parse(typeof(TechType), v, true));
                }
                catch (Exception)
                {
                    throw new ArgumentException("Failed to parse TechType from string: " + v + " in techtype list: " + str);
                }
            }

            return list;
        }
    }
}
