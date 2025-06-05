using BepInEx.Logging;
using COM3D2.CustomEventLoader.Plugin.DataClass;
using HarmonyLib;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.CustomEventLoader.Plugin.Core
{
    class SceneHandling
    {
        internal static void ShowEventListScreen(EventDelegate followUpAction = null)
        {
            GameMain.Instance.MainCamera.FadeOut(0.5f, false, () =>
            {
                if (followUpAction != null)
                    followUpAction.Execute();

                GameMain.Instance.LoadScene("SceneScenarioSelect");
                GameMain.Instance.MainCamera.FadeIn();
            });
        }

        internal static void ShowADVScreen()
        {
            GameMain.Instance.MainCamera.FadeOut(0.5f, false, () =>
            {
                //Load the scenario steps data to memory
                InitScenario();

                GameMain.Instance.LoadScene("SceneADV");
                //GameMain.Instance.MainCamera.FadeIn();
            });
        }

        private static void InitScenario()
        {
            StateManager.Instance.ModEventProgress = Constant.EventProgress.Init;

            ScenarioDefinition scnDef = Util.GetCurrentScenarioDefinition();

            int backupCodePage = ZipConstants.DefaultCodePage;
            ZipConstants.DefaultCodePage = System.Text.Encoding.UTF8.CodePage;
            StateManager.Instance.ScenarioSteps = ScenarioFileHandling.ReadZipFileSteps(scnDef.FilePath);
            ZipConstants.DefaultCodePage = backupCodePage;

            StateManager.Instance.CurrentADVStepID = scnDef.EntryStep;
            StateManager.Instance.UndergoingModEventID = StateManager.Instance.SelectedScenarioID;

            StateManager.Instance.ModEventProgress = Constant.EventProgress.ADV;
        }
    }
}
