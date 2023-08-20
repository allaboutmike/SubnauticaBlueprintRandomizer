using HarmonyLib;
using System;
using System.Collections;
using UnityEngine;

namespace BlueprintRandomizer
{
    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("Start")]
    internal class PlayerPatcher
    {
        private static readonly Player.InitialEquipment[] startingEquipment = new Player.InitialEquipment[7]
        {
            new Player.InitialEquipment()
            {
                techType = TechType.Scanner,
                count = 1
            },
            new Player.InitialEquipment()
            {
                techType = TechType.Knife,
                count = 1
            },
            new Player.InitialEquipment()
            {
                techType = TechType.Seaglide,
                count = 1
            },
            new Player.InitialEquipment()
            {
                techType = TechType.Builder,
                count = 1
            },
            new Player.InitialEquipment()
            {
                techType = TechType.Tank,
                count = 1
            },
            new Player.InitialEquipment()
            {
                techType = TechType.Fins,
                count = 1
            },
            new Player.InitialEquipment()
            {
                techType = TechType.Rebreather,
                count = 1
            }
        };

        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            if (BlueprintRandomizer.Config.StartingEquipment && !Utils.GetContinueMode())
            {
                __instance.StartCoroutine(GiveStartingEquipment());
            }
        }

        private static IEnumerator GiveStartingEquipment()
        {
            while (!uGUI.main.hud.active)
            {
                yield return (object)null;
            }
            yield return (object)new WaitForSeconds(5f);
            foreach (Player.InitialEquipment initialEquipment in PlayerPatcher.startingEquipment)
            {
                CraftData.AddToInventory(initialEquipment.techType, initialEquipment.count);
            }
            Inventory.main.quickSlots.Select(0);
        }
    }
}