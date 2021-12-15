using HarmonyLib;
using Story;
using System;
using System.Collections.Generic;

namespace BlueprintRandomizer
{
    [HarmonyPatch(typeof(OnGoalUnlockTracker))]
    [HarmonyPatch("Initialize")]
    internal class OnGoalUnlockTrackerPatcher
    {
        [HarmonyPrefix]
        static bool Prefix(OnGoalUnlockTracker __instance, HashSet<string> completedGoals)
        {
            for (int index = 0; index < __instance.unlockData.onGoalUnlocks.Length; index++)
            {
                OnGoalUnlock onGoalUnlock = __instance.unlockData.onGoalUnlocks[index];
                UnlockBlueprintData[] unlockData = onGoalUnlock.blueprints;
                var baseLookupKey = "Goal|" + onGoalUnlock.goal;
                for (int index2 = 0; index2 < unlockData.Length; index2++)
                {
                    var lookupKey = baseLookupKey + "|" + index2;
                    var newTechType = BlueprintRandomizer.mainDictionary.LookupTechType(lookupKey);
                    if (newTechType != TechType.None)
                    {
                        unlockData[index2].techType = newTechType;
                    }
                }
            }

            return true;
        }
    }
}
