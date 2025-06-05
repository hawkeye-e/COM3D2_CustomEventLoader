using BepInEx.Logging;
using COM3D2.CustomEventLoader.Plugin.Core;
using COM3D2.CustomEventLoader.Plugin.DataClass;
using HarmonyLib;
using ICSharpCode.SharpZipLib.Zip;
using MaidStatus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using static OVRLipSync;

namespace COM3D2.CustomEventLoader.Plugin.HooksAndPatches.CharacterSelectScreen
{
    internal class Patches
    {
        private static ManualLogSource Log = CustomEventLoader.Log;

        internal static void GetSelectionMaidList(IEnumerable<Maid> maidList, ref List<Maid> originalResult)
        {
            if (StateManager.Instance.IsRunningCustomEventScreen)
            {
                List<Maid> list = new List<Maid>();
                foreach (Maid maid in maidList)
                {
                    if (!(maid == null) && maid.status.heroineType != HeroineType.Sub)
                    {
                        list.Add(maid);
                    }
                }

                originalResult = list;
            }
        }

        internal static void UpdateCharacterSelectScreen(CharacterSelectMain instance)
        {
            if (StateManager.Instance.IsRunningCustomEventScreen)
            {
                var chara_select_mgr_ = Traverse.Create(instance).Field("chara_select_mgr_").GetValue<CharacterSelectManager>();

                FilterCharacterSelectList(chara_select_mgr_.MaidPlateParentGrid);

                UpdateCharacterScreenHeaderText(instance);
            }
        }

        internal static void FixCharacterScreenTagData(SceneCharacterSelect instance)
        {
            if (StateManager.Instance.IsRunningCustomEventScreen)
            {
                var adv_kag_ = Traverse.Create(instance).Field("adv_kag_").GetValue<ADVKagManager>();

                adv_kag_.tag_backup.Clear();
                adv_kag_.tag_backup.Add("cancel_label", "*キャラクター選択完了");
                adv_kag_.tag_backup.Add("name", "SceneCharacterSelect");
                adv_kag_.tag_backup.Add("type", "LifeModeRecollection");
                adv_kag_.tag_backup.Add("label", "*ライフモード用キャラ選択完了");
            }
        }


        //This function is to update the maid plate selectability according to the definition setting of the scenario
        internal static void FilterCharacterSelectList(UIGrid maidPlateParentGrid)
        {
            if (maidPlateParentGrid == null)
                return;

            List<int> currentRequiredPersonalityList = GetCurrentPossiblePersonalityList();

            foreach (Transform maidPlateTransform in maidPlateParentGrid.GetChildList())
            {
                //Here control which personality is selectable

                MaidPlate maidPlate = maidPlateTransform.GetComponent<MaidPlate>();
                UIButton maidPlateButton = maidPlate.GetComponentInChildren<UIButton>(includeInactive: true);

                bool isShown = true;

                if (maidPlate.maid == null)
                    isShown = false;
                else if (!currentRequiredPersonalityList.Contains(maidPlate.maid.status.personal.id) && !currentRequiredPersonalityList.Contains(Constant.AnyPersonality))
                    isShown = false;

                maidPlateButton.defaultColor = (isShown ? new Color(0f, 0f, 0f, 0f) : new Color(0f, 0f, 0f, 0.5f));
                maidPlateButton.gameObject.SetActive(isShown);
            }
        }

        internal static void UpdateCharacterScreenHeaderText(CharacterSelectMain instance)
        {
            int firstkey = GetSelectedMaidListFirstEmptyKey();
            if (firstkey >= 0)
                instance.SetExplanatoryLabel(string.Format(ModResources.DisplayTextResource.CharacterSelectHeaderTextPleaseSelect, firstkey + 1));
            else
                instance.SetExplanatoryLabel(ModResources.DisplayTextResource.CharacterSelectHeaderTextAllReady);
        }

        internal static List<int> GetCurrentPossiblePersonalityList()
        {
            ScenarioDefinition scnDef = Util.GetCurrentScenarioDefinition();

            //var firstEmptyPair = StateManager.Instance.SelectedMaidList.Where(x => x.Value == null).FirstOrDefault();
            //Log.LogInfo("firstEmptyPair: " + firstEmptyPair.Key + "");
            //if(firstEmptyPair.Equals(default))
            //    return new List<int>();

            int firstEmptyKey = GetSelectedMaidListFirstEmptyKey();

            if (firstEmptyKey < 0)
                return new List<int>();

            var requirementInfo = scnDef.MaidRequirement.Where(x => x.IndexPosition == firstEmptyKey).FirstOrDefault();

            if (requirementInfo != null)
                return requirementInfo.PersonalityID;
            else
                return new List<int>();
        }

        internal static int GetSelectedMaidListFirstEmptyKey()
        {
            int firstEmptyKey = -1;
            foreach (var kvp in StateManager.Instance.CharacterSelectionMaidList)
            {
                if (kvp.Value == null)
                {
                    firstEmptyKey = kvp.Key;
                    break;
                }
            }

            return firstEmptyKey;
        }

        //Result: continue to execute the original flow or not
        internal static bool CharacterSelectScreenButtonClick(CharacterSelectMain instance)
        {
            if (StateManager.Instance.IsRunningCustomEventScreen)
            {
                var button_dic_ = Traverse.Create(instance).Field("button_dic_").GetValue<Dictionary<string, UIButton>>();
                string btnText = "";
                foreach (KeyValuePair<string, UIButton> item in button_dic_)
                {
                    if (item.Value == UIButton.current)
                    {
                        btnText = item.Key;
                        break;
                    }
                }

                if (btnText == "Cancel")
                {
                    //Cancel flow, go back to the scenario select scene
                    Core.SceneHandling.ShowEventListScreen();
                    return false;
                }
                else if (btnText == "OK")
                {
                    //Proceed to play custom scenario
                    Core.SceneHandling.ShowADVScreen();
                    return false;
                }
            }
            return true;
        }

        internal static void CharacterSelectScreenNamePlateClick()
        {
            if (StateManager.Instance.IsRunningCustomEventScreen)
            {
                //get the maid selected and update the selected maid dictionary
                MaidPlate component = UIButton.current.transform.parent.gameObject.GetComponent<MaidPlate>();

                if (UIWFSelectButton.selected)
                {
                    //This click action is select action
                    int firstKey = StateManager.Instance.CharacterSelectionMaidList.Where(x => x.Value == null).First().Key;
                    StateManager.Instance.CharacterSelectionMaidList[firstKey] = component.maid;
                }
                else
                {
                    //This click action is deselect action
                    int keyToClearValue = StateManager.Instance.CharacterSelectionMaidList.Where(x => x.Value == component.maid).First().Key;
                    StateManager.Instance.CharacterSelectionMaidList[keyToClearValue] = null;
                }
            }
        }

        internal static void UpdateOKButtonStatus(CharacterSelectMain instance)
        {
            if (StateManager.Instance.IsRunningCustomEventScreen)
            {
                var button_dic_ = Traverse.Create(instance).Field("button_dic_").GetValue<Dictionary<string, UIButton>>();

                button_dic_["OK"].isEnabled = StateManager.Instance.CharacterSelectionMaidList.Count(x => x.Value == null) == 0;
            }
        }
    }
}
