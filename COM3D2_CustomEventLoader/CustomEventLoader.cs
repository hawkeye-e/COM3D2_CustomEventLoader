using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;


namespace COM3D2.CustomEventLoader.Plugin
{
    [BepInPlugin(GUID, PluginName, Version)]
    public class CustomEventLoader : BaseUnityPlugin
    {
        public const string PluginName = "CustomEvent";
        public const string GUID = "COM3D2.CustomEvent.Plugin";
        public const string Version = "0.1.0";

        internal static ManualLogSource Log;

        private void Awake()
        {
            // Plugin startup logic
            Log = base.Logger;
            Log.LogInfo($"Plugin {GUID} is loaded!");

            Plugin.Config.Init(this);

            if (Plugin.Config.Enabled)
            {
                try
                {
                    StateManager.Instance = new StateManager();
                    Harmony.CreateAndPatchAll(typeof(HooksAndPatches.ADVScreen.Hooks), HooksAndPatches.ADVScreen.Hooks.GUID);
                    Harmony.CreateAndPatchAll(typeof(HooksAndPatches.DailyScreen.Hooks), HooksAndPatches.DailyScreen.Hooks.GUID);
                    Harmony.CreateAndPatchAll(typeof(HooksAndPatches.CharacterSelectScreen.Hooks), HooksAndPatches.CharacterSelectScreen.Hooks.GUID);

                    Harmony.CreateAndPatchAll(typeof(Hooks), GUID);

                    ModUseData.Init();
                }
                catch (Exception ex)
                {
                    Log.LogInfo(ex.StackTrace);
                }
            }
        }



        internal static class Hooks
        {

        }
    }
}
