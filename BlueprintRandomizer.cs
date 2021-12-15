using QModManager.API.ModLoading;
using System;
using System.Reflection;
using HarmonyLib;
using System.IO;
using BlueprintRandomizer.Logic;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;

namespace BlueprintRandomizer
{
    [QModCore]
    public class BlueprintRandomizer
    {
        internal static Assembly myAssembly = Assembly.GetExecutingAssembly();
        internal static string baseDirectory = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName;
        internal static BlueprintDictionary mainDictionary = new BlueprintDictionary();

        [QModPatch]
        public static void Patch()
        {
            var config = LoadConfig();

            var seedPath = GetSeedFilePath(config);
            if (MainDictionaryExists(seedPath))
            {
                Console.WriteLine("LOADING DICTIONARY FROM " + seedPath);
                mainDictionary = LoadDictionary(seedPath);
            }
            else
            {
                var unlocks = Unlock.LoadUnlocks();
                var blueprints = Blueprint.LoadBlueprints();

                var logic = new BasicLogic(config, blueprints, unlocks);
                mainDictionary = logic.Generate();

                SaveMainDictionary(seedPath);
                GenerateSpoilerLog(config);
            }

            Harmony.CreateAndPatchAll(myAssembly, "subnautica.mod.blueprintrandomizer");
        }

        private static Data.Config LoadConfig()
        {
            var path = Path.Combine(baseDirectory, "config.json");
            return JsonSerializer.Deserialize<Data.Config>(File.ReadAllText(path));
        }

        private static async Task GenerateSpoilerLog(Data.Config config)
        {           
            var spoilerFileName = "Spoiler_" + GetSeedName(config) + ".txt";

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
                "\tSeed: " + config.Seed,
                "\tUse all scanner entries: " + config.UseAllScanner,
                "\tUse all analysis entries: " + config.UseAllAnalysis,
                "\tUse all goals: " + config.UseAllGoals,
                "\tRandomize Starting Blueprints: " + config.RandomStartingBlueprints,
                "\tUse Progression: " + config.UseProgression,
                "\tAllow Softlocks: " + config.allowSoftlocks,
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
            File.WriteAllText(seedPath + "-data.dat", mainDictionary.ToBase64String());
        }        

        private static string GetSeedFilePath(Data.Config config)
        {
            return Path.Combine(baseDirectory, GetSeedName(config));
        }

        private static string GetSeedName(Data.Config config)
        {
            var seedName = config.Seed;
            if (String.IsNullOrEmpty(seedName))
            {
                seedName = "RandomSeed";
            }

            return seedName;
        }
    }
}
