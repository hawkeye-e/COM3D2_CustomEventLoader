using BepInEx;
using BepInEx.Logging;
using COM3D2.CustomEventLoader.Plugin.Core;
using HarmonyLib;
using MaidStatus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.CustomEventLoader.Plugin.HooksAndPatches.DailyScreen
{
    internal class Hooks
    {
        internal static string GUID = CustomEventLoader.GUID + ".DailyScreen";

        //This function will run everytime the daily screen shown, so need proper flag handling to avoid duplicate setup
        [HarmonyPostfix]
        [HarmonyPatch(typeof(DailyCtrl), nameof(DailyCtrl.Init))]
        private static void DailyCtrlInitPost()
        {
            if (DailyMgr.IsLegacy)
                return;

            Patches.AddButtonInDailyScreen();
        }

        //Due to calling the SceneScenarioSelect is not by tag, some of the value is not properly set. Patch it so that no error is prompted.
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SceneScenarioSelect), "Start")]
        private static void SceneScenarioSelectStartPre(SceneScenarioSelect __instance)
        {
            Patches.InitSceneScenarioSelectScreen();
        }


        //Skip executing the character selection update to speed up the event selection as character selection is to be done in another screen.
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SceneScenarioSelect), "UpdateCharaUI")]
        private static bool UpdateCharaUIPre()
        {
            if (StateManager.Instance.IsRunningCustomEventScreen)
                return false;
            return true;
        }

        //If the user click the cancel button in the event selection screen, clear all mod related states before return to daily screen
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SceneScenarioSelect), "PushCancelButton")]
        internal static void PushCancelButtonPre(SceneScenarioSelect __instance)
        {
            if (StateManager.Instance.IsRunningCustomEventScreen)
                ModEventCleanUp.ResetModEvent();
        }

        //Switch to character selection screen if the user click OK in the event selection scene
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SceneScenarioSelect), "PushOkButton")]
        private static bool PushOkButtonPre()
        {
            return Patches.SceneScenarioSelectOKClick();
        }

        //Record the scenario id selected whenever the user click on an item.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SceneScenarioSelect), "OnSelectScenario")]
        private static void OnSelectScenarioPost(SceneScenarioSelect __instance)
        {
            Patches.UpdateScenarioSelected(__instance);
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(ScenarioSelectMgr), nameof(ScenarioSelectMgr.GetAllScenarioData))]
        private static void GetAllScenarioDataPost(ref ScenarioData[] __result)
        {
            Patches.ReplaceResultByCustomData(ref __result);
        }
    }
}
