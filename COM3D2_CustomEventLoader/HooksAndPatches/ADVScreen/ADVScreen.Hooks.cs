using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace COM3D2.CustomEventLoader.Plugin.HooksAndPatches.ADVScreen
{
    internal class Hooks
    {
        internal static string GUID = CustomEventLoader.GUID + ".ADVScreen";


        [HarmonyPostfix]
        [HarmonyPatch(typeof(ADVKagManager), nameof(ADVKagManager.Update))]
        private static void UpdatePost(ADVKagManager __instance)
        {
            if (__instance.skip_mode)
                Patches.HandleModADVScenarioUserInput();

            Patches.CheckWaitForFullLoadDone(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ADVKagManager), nameof(ADVKagManager.OnClickEvent))]
        private static void OnClickEventPost(ADVKagManager __instance)
        {
            Patches.HandleModADVScenarioUserInput();
        }

        //Force the system to reset all mod related flags and setup if the system has gone back to the main title screen (the player may have terminated the game progress during mod event)
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameMain), nameof(GameMain.LoadScene))]
        private static void LoadScenePre(string f_strSceneName)
        {
            if (f_strSceneName == Constant.CallScreenName.Title)
                Patches.ReturnToMainTitleHandling();
        }
    }
}
