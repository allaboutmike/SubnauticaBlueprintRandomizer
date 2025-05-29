using HarmonyLib;
using System.Reflection;

namespace BlueprintRandomizer
{
    [HarmonyPatch(typeof(Rocket))]
    [HarmonyPatch("AdvanceRocketStage")]
    internal class RocketPatcher
    {
        [HarmonyPrefix]
        static bool Prefix(ref Rocket __instance)
        {
            ++__instance.currentRocketStage;

            var currentStage = __instance.currentRocketStage;
            if (currentStage == 5)
            {
                // Private field, gotta mess about
                var isFinishedField = typeof(Rocket).GetField("isFinished", BindingFlags.Instance | BindingFlags.NonPublic);
                isFinishedField.SetValue(__instance, true);
                Rocket.IsAnyRocketReady = true;
            }
            else
            {
                var lookupKey = "Rocket|" + currentStage;
                var newTechType = BlueprintRandomizer.mainDictionary.LookupTechType(lookupKey);
                if (newTechType != TechType.None)
                {
                    KnownTech.Add(newTechType);
                }
            }

            return false;
        }
    }
}
