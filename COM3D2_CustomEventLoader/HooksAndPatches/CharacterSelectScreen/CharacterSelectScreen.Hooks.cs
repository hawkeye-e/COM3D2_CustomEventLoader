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

namespace COM3D2.CustomEventLoader.Plugin.HooksAndPatches.CharacterSelectScreen
{
    internal class Hooks
    {
        internal static string GUID = CustomEventLoader.GUID + ".CharacterSelectScreen";


        //The character select screen is reusing the one of Empire Life Mode. By design it excludes Ex-pack characters.
        //We dont want this filtering by replacing the result list.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(EmpireLifeModeAPI), nameof(EmpireLifeModeAPI.SelectionMaidList))]
        private static void SelectionMaidList(IEnumerable<Maid> maidList, ref List<Maid> __result)
        {
            Patches.GetSelectionMaidList(maidList, ref __result);
        }

        //Alter the data to be displayed in the Character select screen
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CharacterSelectMain), "OnCall")]
        private static void CharacterSelectMainOnCallPost(CharacterSelectMain __instance)
        {
            Patches.UpdateCharacterSelectScreen(__instance);
        }

        //Fix the cancel button in the screen
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SceneCharacterSelect), nameof(SceneCharacterSelect.Start))]
        private static void SceneCharacterSelectStart(SceneCharacterSelect __instance)
        {
            Patches.FixCharacterScreenTagData(__instance);
        }

        //Handle the OK and Cancel button click
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CharacterSelectMain), "OnClickButton")]
        private static bool CharacterSelectMainOnClickButton(CharacterSelectMain __instance)
        {
            return Patches.CharacterSelectScreenButtonClick(__instance);
        }

        //Handle internal data update when the user select or deselect a maid
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CharacterSelectManager), "OnSelect")]
        private static void OnSelect(CharacterSelectManager __instance)
        {
            Patches.CharacterSelectScreenNamePlateClick();
        }

        //Update the screen whenever the user select or deselect a maid 
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CharacterSelectMain), "UpdateSelectedMaidPanelOfLifeModeRecollection")]
        private static void UpdateSelectedMaidPanelOfLifeModeRecollectionPost(CharacterSelectMain __instance)
        {
            Patches.UpdateCharacterSelectScreen(__instance);
        }

        //The button button behaviour should be different from the original logic as we need the players select all required maid before he can click ok.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CharacterSelectMain), "OnMultiSelectCharaLifeModeRecollection")]
        private static void OnMultiSelectCharaLifeModeRecollectionPost(CharacterSelectMain __instance)
        {
            Patches.UpdateOKButtonStatus(__instance);
        }
    }
}
