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

    }
}
