using HarmonyLib;

namespace BlueprintRandomizer
{
    [HarmonyPatch(typeof(Rocket))]
    [HarmonyPatch("GetCurrentStageTech")]
    internal class RocketPatcher
    {
        [HarmonyPrefix]
        static bool Prefix(Rocket __instance)
        {
            var currentStage = __instance.currentRocketStage;

            var lookupKey = "Rocket|" + currentStage;
            var newTechType = BlueprintRandomizer.mainDictionary.LookupTechType(lookupKey);
            if (newTechType != TechType.None)
            {
                __instance.stageTech[currentStage] = newTechType;
            }

            return true;
        }
    }
}
