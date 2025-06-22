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

namespace COM3D2.CustomEventLoader.Plugin.HooksAndPatches.DailyScreen
{
    internal class Patches
    {
        private static ManualLogSource Log = CustomEventLoader.Log;

        internal static void AddButtonInDailyScreen()
        {
            GameObject dailyPanel = GameObject.Find("/UI Root/DailyPanel");
            GameObject buttonToClone = GameObject.Find("/UI Root/DailyPanel/VerticalLine1/Schedule");

            GameObject clonedButton = GameObject.Instantiate(buttonToClone);

            clonedButton.name = "MOD_CustomEvent";
            clonedButton.transform.parent = dailyPanel.transform;
            clonedButton.transform.localPosition = new Vector3(874, 253);
            clonedButton.transform.localScale = Vector3.one;

            Transform clonedButtonValue = clonedButton.transform.Find("Value");
            clonedButtonValue.GetComponent<UILabel>().text = ModResources.DisplayTextResource.DailyScreenButtonText;

            UIButton compButton = clonedButton.GetComponent<UIButton>();
            compButton.normalSprite = "main_buttom";
            compButton.onClick = new List<EventDelegate>();
            EventDelegate eventDelegate = new EventDelegate(() => {
                StateManager.Instance.IsRunningCustomEventScreen = true;
                Core.SceneHandling.ShowEventListScreen();

            });
            compButton.onClick.Add(eventDelegate);
        }

        internal static void InitSceneScenarioSelectScreen()
        {
            if (StateManager.Instance.IsRunningCustomEventScreen)
            {
                string cancelLabelName = "cancel_label";

                string cancelLabelValue;
                if (GameMain.Instance.CharacterMgr.status.isDaytime)
                    cancelLabelValue = "*昼メニュー";
                else
                    cancelLabelValue = "*夜メニュー";

                var adv_kag = GameMain.Instance.ScriptMgr.adv_kag;

                if (adv_kag.tag_backup.ContainsKey(cancelLabelName))
                    adv_kag.tag_backup[cancelLabelName] = cancelLabelValue;
                else
                    adv_kag.tag_backup.Add(cancelLabelName, cancelLabelValue);
            }
        }


        internal static void ReplaceResultByCustomData(ref ScenarioData[] result)
        {
            if (StateManager.Instance.IsRunningCustomEventScreen)
            {
                CreateCustomScenarioList();

                if (StateManager.Instance.ScenarioList.Count == 0)
                    return;

                List<ScenarioData> customScenarioList = new List<ScenarioData>();

                int runningID = 1;
                foreach (var kvp in StateManager.Instance.ScenarioList)
                {
                    //TODO: Update the logic here to show the scenario info better

                    ScenarioData sData = new ScenarioData();
                    Traverse.Create(sData).Field("ID").SetValue(runningID);
                    Traverse.Create(sData).Field("NotLineTitle").SetValue(kvp.Value.Title);
                    Traverse.Create(sData).Field("Title").SetValue(kvp.Value.Title);
                    Traverse.Create(sData).Field("EventContents").SetValue(kvp.Value.EventContents);
                    Traverse.Create(sData).Field("IconName").SetValue(kvp.Value.Icon);

                    List<string> conditionText = new List<string>();
                    conditionText.Add(string.Format(ModResources.DisplayTextResource.DisplayAuthorFormat, kvp.Value.Author));
                    conditionText.Add(string.Format(ModResources.DisplayTextResource.DisplayLanguageFormat, kvp.Value.Language));
                    if (kvp.Value.MaidRequirement != null)
                        conditionText.Add(string.Format(ModResources.DisplayTextResource.DisplayNumberOfMaidsFormat, kvp.Value.MaidRequirement.Count));
                    
                    Traverse.Create(sData).Field("ConditionText").SetValue(conditionText.ToArray());

                    Traverse.Create(sData).Field("EventMaidNum").SetValue(0);
                    Traverse.Create(sData).Field("NotPlayAgain").SetValue(false);
                    Traverse.Create(sData).Field("IsOncePlayOnly").SetValue(false);
                    Traverse.Create(sData).Field("IsPlayable").SetValue(true);

                    customScenarioList.Add(sData);

                    runningID++;
                }

                result = customScenarioList.ToArray();
            }
        }



        internal static void CreateCustomScenarioList()
        {
            if (StateManager.Instance.IsScenarioListCreated && !Config.DebugAlwaysReloadEventList)
                return;

            Dictionary<int, ScenarioDefinition> result = new Dictionary<int, ScenarioDefinition>();
            
            string scenarioFolderPath = Directory.GetCurrentDirectory() + "\\" + Plugin.Config.CustomEventPath;

            string[] allFiles = Directory.GetFiles(scenarioFolderPath, "*.*", SearchOption.AllDirectories);

            int backupCodePage = ZipConstants.DefaultCodePage;
            ZipConstants.DefaultCodePage = System.Text.Encoding.UTF8.CodePage;
            int id = 1;
            foreach (string filePath in allFiles)
            {
                ScenarioDefinition scnDef = ScenarioFileHandling.ReadZipFileDefinition(filePath);

                if (scnDef != null)
                {
                    scnDef.FilePath = filePath;
                    result.Add(id++, scnDef);
                }
            }

            //Restore the code page just in case
            ZipConstants.DefaultCodePage = backupCodePage;

            StateManager.Instance.ScenarioList = result;

            StateManager.Instance.IsScenarioListCreated = true;
        }

        //Return Value: continue to execute the remaining function or not
        internal static bool SceneScenarioSelectOKClick()
        {
            if (StateManager.Instance.IsRunningCustomEventScreen)
            {

                //Init array for character selection
                var maidRequirementInfo = Util.GetCurrentScenarioDefinition().MaidRequirement;
                StateManager.Instance.CharacterSelectionMaidList = new Dictionary<int, Maid>();
                foreach (var info in maidRequirementInfo)
                    StateManager.Instance.CharacterSelectionMaidList.Add(info.IndexPosition, null);

                GameMain.Instance.MainCamera.FadeOut(0.5f, false, () =>
                {
                    GameMain.Instance.LoadScene("SceneCharacterSelect");
                    GameMain.Instance.MainCamera.FadeIn();
                });

                return false;
            }

            return true;
        }

        internal static void UpdateScenarioSelected(SceneScenarioSelect instance)
        {
            if (StateManager.Instance.IsRunningCustomEventScreen)
            {
                StateManager.Instance.SelectedScenarioID = Traverse.Create(instance).Field("m_CurrentScenario").GetValue<ScenarioData>().ID;
            }
        }
    }
}
