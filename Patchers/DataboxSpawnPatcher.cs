using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace BlueprintRandomizer
{
    [HarmonyPatch(typeof(DataboxSpawner), nameof(DataboxSpawner.Start))]
    class DataboxSpawnPatcher
    {
        [HarmonyPrefix]
        static bool PatchDataboxOnSpawn(ref DataboxSpawner __instance)
        {
            BlueprintHandTarget blueprint = __instance.databoxPrefab.GetComponent<BlueprintHandTarget>();
            var lookupKey = "Databox|" + blueprint.unlockTechType;
            var newTechType = BlueprintRandomizer.mainDictionary.LookupTechType(lookupKey);
            if (newTechType != TechType.None)
            {
                blueprint.unlockTechType = newTechType;
            }
            return true;
        }
    }
}
