using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Logging;
using UnityEngine;
using HarmonyLib;

namespace COM3D2.CustomEventLoader.Plugin.HooksAndPatches.ADVScreen
{
    internal class Patches
    {
        private static ManualLogSource Log = CustomEventLoader.Log;

        internal static void HandleModADVScenarioUserInput()
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                if (StateManager.Instance.WaitForUserClick && !StateManager.Instance.WaitForMotionChange)
                {
                    Core.CustomADVProcessManager.ADVSceneProceedToNextStep();
                }
            }
        }

        internal static void CheckWaitForFullLoadDone(ADVKagManager instance)
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                //Checking if all the game objects we need are all loaded
                if (StateManager.Instance.WaitForFullLoadList.Count > 0)
                {
                    int i = 0;
                    while (i < StateManager.Instance.WaitForFullLoadList.Count)
                    {
                        if (!StateManager.Instance.WaitForFullLoadList[i].IsAllProcPropBusy)
                            StateManager.Instance.WaitForFullLoadList.RemoveAt(i);
                        else
                            i++;
                    }

                }
                
                if (StateManager.Instance.WaitForFullLoadList.Count == 0)
                    Core.CustomADVProcessManager.ProcessADVStep(instance);
            }
        }

        internal static void ReturnToMainTitleHandling()
        {
            if (StateManager.Instance.IsRunningCustomEventScreen)
            {
                Core.ModEventCleanUp.ResetModEvent();
            }
        }

        internal static void StartSpoofingLoadMotionScript(string label_name, string maid_guid, string man_guid)
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {

                StateManager.Instance.processingMaidGUID = maid_guid;
                StateManager.Instance.processingManGUID = man_guid;

                if (maid_guid == "" && man_guid == "")
                    StateManager.Instance.IsMainGroupMotionScriptFlag = true;

            }
        }

        internal static void EndSpoofingLoadMotionScript()
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                StateManager.Instance.processingMaidGUID = "";
                StateManager.Instance.processingManGUID = "";

                StateManager.Instance.IsMainGroupMotionScriptFlag = false;
            }
        }

        internal static void ForceMaidLipSync(Maid maid)
        {
            if (StateManager.Instance.ForceLipSyncingList.Contains(maid))
            {
                if (DateTime.Now < StateManager.Instance.LipSyncEndTime)
                {
                    double t = DateTime.Now.Subtract(StateManager.Instance.LipSyncStartTime).TotalMilliseconds / 1000;
                    maid.FoceKuchipakuUpdate((float)t);
                }
                else
                {
                    maid.StopKuchipakuPattern();
                }
            }
        }
    }
}
