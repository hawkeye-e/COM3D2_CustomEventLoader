using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace COM3D2.CustomEventLoader.Plugin
{
    internal class Config
    {
        private const string GENERAL = "1. General";
        private const string DEVELOPER = "2. Developer Used";

        private static readonly string DEFAULT_CUSTOM_EVENT_PATH = "CustomEvent";

        internal static bool Enabled { get { return _enabled.Value; } }
        private static ConfigEntry<bool> _enabled;

        internal static string CustomEventPath { get { return _customEventPath.Value; } }
        private static ConfigEntry<string> _customEventPath;

        internal static bool DeveloperMode { get { return _developerMode.Value; } }
        private static ConfigEntry<bool> _developerMode;

        internal static bool DebugIgnoreADVForceTimeWait { get { return _debugIgnoreADVForceTimeWait.Value; } }
        private static ConfigEntry<bool> _debugIgnoreADVForceTimeWait;

        internal static KeyboardShortcut DeveloperModeCameraKey { get { return _developerModeCameraKey.Value; } }
        private static ConfigEntry<KeyboardShortcut> _developerModeCameraKey;

        internal static KeyboardShortcut DeveloperModeWorldObjectDataKey { get { return _developerModeWorldObjectDataKey.Value; } }
        private static ConfigEntry<KeyboardShortcut> _developerModeWorldObjectDataKey;

        internal static KeyboardShortcut DeveloperModeCharaPlacementDataKey { get { return _developerModeCharaPlacementDataKey.Value; } }
        private static ConfigEntry<KeyboardShortcut> _developerModeCharaPlacementDataKey;

        internal static KeyboardShortcut DeveloperModeClothesSetDataKey { get { return _developerModeClothesSetDataKey.Value; } }
        private static ConfigEntry<KeyboardShortcut> _developerModeClothesSetDataKey;

        internal static bool DebugAlwaysReloadEventList { get { return _debugAlwaysReloadEventList.Value; } }
        private static ConfigEntry<bool> _debugAlwaysReloadEventList;

        internal static bool DebugLogAudioInfo { get { return _debugLogAudioInfo.Value; } }
        private static ConfigEntry<bool> _debugLogAudioInfo;

        internal static bool DebugLogScriptInfo { get { return _debugLogScriptInfo.Value; } }
        private static ConfigEntry<bool> _debugLogScriptInfo;

        internal static bool DebugLogAnimationInfo { get { return _debugLogAnimationInfo.Value; } }
        private static ConfigEntry<bool> _debugLogAnimationInfo;

        internal static bool DebugLogFaceAnimeInfo { get { return _debugLogFaceAnimeInfo.Value; } }
        private static ConfigEntry<bool> _debugLogFaceAnimeInfo;

        internal static bool DebugLogBackgroundInfo { get { return _debugLogBackgroundInfo.Value; } }
        private static ConfigEntry<bool> _debugLogBackgroundInfo;

        internal static bool DebugLogBGMInfo { get { return _debugLogBGMInfo.Value; } }
        private static ConfigEntry<bool> _debugLogBGMInfo;

        internal static bool DebugLogSEInfo { get { return _debugLogSEInfo.Value; } }
        private static ConfigEntry<bool> _debugLogSEInfo;

        internal static bool DebugLogMaleBodyPartInfo { get { return _debugLogMaleBodyPartInfo.Value; } }
        private static ConfigEntry<bool> _debugLogMaleBodyPartInfo;



        internal static void Init(BaseUnityPlugin plugin)
        {
            AddGeneralConfigs(plugin);
            AddDeveloperRelatedConfigs(plugin);
        }

        private static void AddGeneralConfigs(BaseUnityPlugin plugin)
        {
            _enabled = plugin.Config.Bind(GENERAL, "1. Enable this plugin", true, "If false, this plugin will do nothing (requires game restart)");

            _customEventPath = plugin.Config.Bind(GENERAL, "2. Custom Event Folder Relative Path", DEFAULT_CUSTOM_EVENT_PATH,
                "The folder path that contains all the custom events in zip file format. \n\n" + 
                "For example if the value here is 'MyPath', the mod will try to locate the zip files in the path '" + Directory.GetCurrentDirectory() + "\\MyPath'. \n\n" +
                "You need to create this path yourself if this is the first time you run this mod."
                );
        }



        private static void AddDeveloperRelatedConfigs(BaseUnityPlugin plugin)
        {
            _developerMode = plugin.Config.Bind(DEVELOPER, "_Developer Mode", true, "Turn this on if you want the information for the editor (requires game restart)");

            _debugAlwaysReloadEventList = plugin.Config.Bind(DEVELOPER, "Always Reload Event List", false, "The system will cache the event list so that it does not scan the custom event folder every time when the custom event screen is shown. Turn this on if you do not want to restart the game when you are creating and testing your events.");

            _developerModeCameraKey = plugin.Config.Bind(DEVELOPER, "Print Camera Info Key", new KeyboardShortcut(UnityEngine.KeyCode.C), "The key to display camera information required for the camera step.");

            _developerModeWorldObjectDataKey = plugin.Config.Bind(DEVELOPER, "Print Object Info List Key", new KeyboardShortcut(UnityEngine.KeyCode.O), "The key to display object list information required for the World Object step. Works in studio mode only.");

            _developerModeCharaPlacementDataKey = plugin.Config.Bind(DEVELOPER, "Print Character Placement Key", new KeyboardShortcut(UnityEngine.KeyCode.X), "The key to display character placement information required for the Chara step. Works in studio mode only.");

            _developerModeClothesSetDataKey = plugin.Config.Bind(DEVELOPER, "Print Clothes Set Key", new KeyboardShortcut(UnityEngine.KeyCode.B), "The key to display the clothes set information of the maid for the Chara Init step. Works best in Edit Maid mode.");

            _debugIgnoreADVForceTimeWait = plugin.Config.Bind(DEVELOPER, "Ignore ADV Time Wait Setting", false, "Skip all those time wait setting in ADV to speed up the debug process.");

            _debugLogScriptInfo = plugin.Config.Bind(DEVELOPER, "Log Load Script Info", false, "Log the script info whenever it is loaded in the game.");

            _debugLogAnimationInfo = plugin.Config.Bind(DEVELOPER, "Log Load Animation Info", false, "Log the animation info whenever an animation file is loaded for a object in the game.");

            _debugLogAudioInfo = plugin.Config.Bind(DEVELOPER, "Log Load Audio Info", false, "Log the audio info whenever an audio file is loaded for a maid in the game.");

            _debugLogFaceAnimeInfo = plugin.Config.Bind(DEVELOPER, "Log Load Face Anime Info", false, "Log the face anime info whenever the facial expression changed for a maid in the game.");

            _debugLogBackgroundInfo = plugin.Config.Bind(DEVELOPER, "Log Load Background Info", false, "Log the background info whenever it is changed in the game.");

            _debugLogBGMInfo = plugin.Config.Bind(DEVELOPER, "Log Load BGM Info", false, "Log the BGM info whenever it is changed in the game.");

            _debugLogSEInfo = plugin.Config.Bind(DEVELOPER, "Log Load SE Info", false, "Log the sound effect info whenever it is changed in the game.");

            _debugLogMaleBodyPartInfo = plugin.Config.Bind(DEVELOPER, "Log Male Body Part Info", false, "Log the male body part info whenever it is changed in the male edit screen.");
        }



    }
}
