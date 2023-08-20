using HarmonyLib;
using System;
using System.Collections.Generic;

namespace BlueprintRandomizer
{    
    [HarmonyPatch(typeof(PDAData))]
    [HarmonyPatch("Initialize")]
    internal class PDADataPatcher
    {
        private const String SCANNER_PREFIX = "Scanner";
        private const String ANALYSIS_PREFIX = "Analysis";
        private const String DEFAULT_PREFIX = "Default";

        [HarmonyPrefix]
        static bool Prefix(PDAData __instance, ref PDAData pdaData)
        {
            List<TechType> defaultTech = pdaData.defaultTech;
            for (int index = 0; index < defaultTech.Count; index++)
            {                                
                var lookupKey = DEFAULT_PREFIX + "|" + index;
                var newTechType = BlueprintRandomizer.mainDictionary.LookupTechType(lookupKey);
                if (newTechType != TechType.None)
                {
                    defaultTech[index] = newTechType;
                }
            }

            List<PDAScanner.EntryData> scanner = pdaData.scanner;
            for (int index = 0; index < scanner.Count; index++)
            {
                var lookupKey = SCANNER_PREFIX + "|" + scanner[index].key;
                var newTechType = BlueprintRandomizer.mainDictionary.LookupTechType(lookupKey);
                if (newTechType != TechType.None)
                {
                    scanner[index].blueprint = newTechType;
                }
            }

            List<KnownTech.AnalysisTech> analysis = pdaData.analysisTech;
            for (int index = 0; index < analysis.Count; index++)
            {
                var type = analysis[index].techType;
                var unlocks = analysis[index].unlockTechTypes;
                
                for (int index2 = 0; index2 < unlocks.Count; index2++)
                {
                    var lookupKey = ANALYSIS_PREFIX + "|" + type + "|" + index2;
                    var newTechType = BlueprintRandomizer.mainDictionary.LookupTechType(lookupKey);
                    if (newTechType != TechType.None)
                    {
                        unlocks[index2] = newTechType;
                    }
                }
            }

            return true;
        }

    }
}
