using System;
using System.Collections.Generic;
using System.IO;

namespace BlueprintRandomizer
{
    class Blueprint
    {
        private static string FILE_NAME = "blueprints.csv";

        public TechType type;
        public bool startingTech;

        public Blueprint(TechType type, bool startingTech)
        {
            this.type = type;
            this.startingTech = startingTech;
        }

        public static List<Blueprint> LoadBlueprints()
        {
            var fullPath = Path.Combine(BlueprintRandomizer.baseDirectory, FILE_NAME);
            string[] lines;

            lines = File.ReadAllLines(fullPath);

            var blueprintList = new List<Blueprint>();

            bool header = true;
            foreach (var line in lines)
            {
                if (!header && !String.IsNullOrEmpty(line))
                {
                    string[] values = line.Split(',');
                    if (values.Length < 2)
                    {
                        throw new Exception("Unable to parse blueprints, invalid line length: " + values);
                    }
                    var type = StringToTechType(values[0]);
                    var startingTech = values[1] == "1";

                    blueprintList.Add(new Blueprint(type, startingTech));
                } else
                {
                    header = false;
                }
            }

            return blueprintList;
        }

        private static TechType StringToTechType(string str)
        {
            try
            {
                return (TechType)Enum.Parse(typeof(TechType), str, true);
            }
            catch (Exception)
            {
                throw new ArgumentException("Failed to parse TechType from string: " + str);
            }
        }
    }
}
