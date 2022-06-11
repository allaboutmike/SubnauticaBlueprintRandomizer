using QModManager.API.ModLoading;
using System;
using System.Reflection;
using HarmonyLib;
using System.IO;
using BlueprintRandomizer.Logic;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BlueprintRandomizer
{
    [QModCore]
    public class BlueprintRandomizer
    {
        internal static Data.Config Config { get; } = new Data.Config();

        internal static Assembly myAssembly = Assembly.GetExecutingAssembly();
        internal static string baseDirectory = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName;
        internal static BlueprintDictionary mainDictionary = new BlueprintDictionary();

        [QModPatch]
        public static void Patch()
        {
            Console.WriteLine("BlueprintRandomizer.Patch() Start");
            Config.Load();

            var seedPath = GetSeedFilePath();
            if (MainDictionaryExists(seedPath))
            {
                Console.WriteLine("LOADING DICTIONARY FROM " + seedPath);
                mainDictionary = LoadDictionary(seedPath);
            }
            else
            {
                var unlocks = Unlock.LoadUnlocks();
                var blueprints = Blueprint.LoadBlueprints();

                var logic = new BasicLogic(Config, blueprints, unlocks);
                mainDictionary = logic.Generate();

                SaveMainDictionary(seedPath);
                GenerateSpoilerLog();
                Console.WriteLine("DICTIONARY CREATED: " + seedPath);
            }

            Harmony.CreateAndPatchAll(myAssembly, "subnautica.mod.blueprintrandomizer");
            Console.WriteLine("BlueprintRandomizer.Patch() End");
        }

        private static async Task GenerateSpoilerLog()
        {           
            var spoilerFileName = "Spoiler_" + GetSeedName() + ".txt";

            var contents = new List<string>();

            contents.AddRange(new string[] { 
                "************************************************************************************",
                "**************** BLUEPRINT RANDOMIZER SPOILER LOG **********************************",
                "************************************************************************************",
            });

            contents.AddRange(new string[]
            {
                "",
                "Config:",
                "\tSeed: " + Config.Seed,
                "\tUse all scanner entries: " + Config.UseAllScanner,
                "\tUse all analysis entries: " + Config.UseAllAnalysis,
                "\tUse all goals: " + Config.UseAllGoals,
                "\tRandomize Starting Blueprints: " + Config.RandomStartingBlueprints,
                "\tUse Progression: " + Config.UseProgression,
                "\tAllow Softlocks: " + Config.AllowSoftlocks,
                "*************************************************************************************",
                "",
                "Blueprint Mapping:"
            });

            foreach (KeyValuePair<string, TechType> pair in mainDictionary.blueprintLookup)
            {
                if (pair.Value != TechType.None) { 
                    contents.Add("\t" + pair.Key + " unlocks " + pair.Value);
                }
            }

            using (StreamWriter file = new StreamWriter(Path.Combine(baseDirectory, spoilerFileName)))
            {
                foreach (var line in contents)
                {
                    await file.WriteLineAsync(line);
                }
            }

            Console.WriteLine("Wrote spoiler log to " + spoilerFileName);
        }

        private static bool MainDictionaryExists(string seedPath)
        {
            return File.Exists(seedPath);            
        }

        private static BlueprintDictionary LoadDictionary(string seedPath)
        {
            var base64Str = File.ReadAllText(seedPath);
            return BlueprintDictionary.FromBase64String(base64Str);
        }

        private static void SaveMainDictionary(string seedPath)
        {
            File.WriteAllText(seedPath, mainDictionary.ToBase64String());
        }        

        private static string GetSeedFilePath()
        {
            return Path.Combine(baseDirectory, GetSeedName()) + "-data.dat";
        }

        private static string GetSeedName()
        {
            var seedName = Config.Seed;
            if (String.IsNullOrEmpty(seedName))
            {
                seedName = "RandomSeed";
            }

            return seedName;
        }
    }
}
