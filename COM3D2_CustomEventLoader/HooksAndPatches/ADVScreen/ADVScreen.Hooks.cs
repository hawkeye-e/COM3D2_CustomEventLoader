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

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ScriptManager), nameof(ScriptManager.LoadMotionScript))]
        private static void LoadMotionScriptPre(int sloat, bool is_next, string file_name, string label_name, string maid_guid, string man_guid, bool face_fix, bool valid_pos, bool disable_diff_pos)
        {
            Patches.StartSpoofingLoadMotionScript(label_name, maid_guid, man_guid);
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(ScriptManager), nameof(ScriptManager.LoadMotionScript))]
        private static void LoadMotionScriptPost(ScriptManager __instance, int sloat, bool is_next, string file_name, string label_name, string maid_guid, string man_guid, bool face_fix, bool valid_pos, bool disable_diff_pos)
        {
            Patches.EndSpoofingLoadMotionScript();
            ////When this function is called the system will always try to set the main group to its default position of the map which is not what we want. Apply the position setting again.
            //if (StateManager.Instance.UndergoingModEventID > 0 && StateManager.Instance.ModEventProgress == Constant.EventProgress.YotogiPlay)
            //    if (!Patches.CheckBlockLoadMotionScript(maid_guid))
            //        Util.ResetAllGroupPosition();
        }

        //Logic to make the maid's mouth move for the case that no voice file is attached
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Maid), "Update")]
        private static void MaidUpdate(Maid __instance)
        {
            Patches.ForceMaidLipSync(__instance);
        }
    }
}
