using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COM3D2.CustomEventLoader.Plugin
{
    internal class Config
    {
        private const string GENERAL = "1. General";
        private const string DEVELOPER = "2. Developer Used";

        private static readonly string DEFAULT_CUSTOM_SCENARIO_PATH = "CustomScenario";

        internal static bool Enabled { get { return _enabled.Value; } }
        private static ConfigEntry<bool> _enabled;

        internal static string CustomScenarioPath { get { return _customScenarioPath.Value; } }
        private static ConfigEntry<string> _customScenarioPath;

        internal static bool DeveloperMode { get { return _developerMode.Value; } }
        private static ConfigEntry<bool> _developerMode;

        internal static bool DebugIgnoreADVForceTimeWait { get { return _debugIgnoreADVForceTimeWait.Value; } }
        private static ConfigEntry<bool> _debugIgnoreADVForceTimeWait;


        internal static void Init(BaseUnityPlugin plugin)
        {
            AddGeneralConfigs(plugin);
            AddDeveloperRelatedConfigs(plugin);
        }

        private static void AddGeneralConfigs(BaseUnityPlugin plugin)
        {
            _enabled = plugin.Config.Bind(GENERAL, "1. Enable this plugin", true, "If false, this plugin will do nothing (requires game restart)");

            _customScenarioPath = plugin.Config.Bind(GENERAL, "2. Custom Scenario Folder Relative Path", DEFAULT_CUSTOM_SCENARIO_PATH,
                "The folder path that contains all the custom scenarios in zip file format. \n\n" + 
                "For example if the value here is 'MyPath', the mod will try to locate the zip files in the path '" + AppDomain.CurrentDomain.BaseDirectory + "\\MyPath'. \n\n" +
                "You need to create this path yourself if this is the first time you run this mod."
                );
        }



        private static void AddDeveloperRelatedConfigs(BaseUnityPlugin plugin)
        {
            _developerMode = plugin.Config.Bind(DEVELOPER, "Developer Mode", false, "Leave this unchecked if you have no idea what it is (requires game restart)");

            //_debugLogMotionData = plugin.Config.Bind(DEVELOPER, "Log Motion Data", false, "Leave this unchecked if you have no idea what it is");

            //_debugCaptureDialogues = plugin.Config.Bind(DEVELOPER, "Log All Dialogues", false, "Leave this unchecked if you have no idea what it is");

            _debugIgnoreADVForceTimeWait = plugin.Config.Bind(DEVELOPER, "Ignore ADV Time Wait Setting", false, "Skip all those time wait setting in ADV to speed up the debug process. Leave this unchecked if you have no idea what it is");

            //_debugLogScriptInfo = plugin.Config.Bind(DEVELOPER, "Log Load Script Info", false, "Log the script info whenever it is loaded in the game. Leave this unchecked if you have no idea what it is");

            //_debugLogAnimationInfo = plugin.Config.Bind(DEVELOPER, "Log Load Animation Info", false, "Log the animation info whenever an animation file is loaded for a object in the game. Leave this unchecked if you have no idea what it is");

            //_debugLogAudioInfo = plugin.Config.Bind(DEVELOPER, "Log Load Audio Info", false, "Log the audio info whenever an audio file is loaded for a maid in the game. Leave this unchecked if you have no idea what it is");

            //_debugLogFaceAnimeInfo = plugin.Config.Bind(DEVELOPER, "Log Load Face Anime Info", false, "Log the face anime info whenever the facial expression changed for a maid in the game. Leave this unchecked if you have no idea what it is");

            //_debugLogBackgroundInfo = plugin.Config.Bind(DEVELOPER, "Log Load Background Info", false, "Log the background info whenever it is changed in the game. Leave this unchecked if you have no idea what it is");

            //_debugLogBGMInfo = plugin.Config.Bind(DEVELOPER, "Log Load BGM Info", false, "Log the BGM info whenever it is changed in the game. Leave this unchecked if you have no idea what it is");

            //_debugLogSEInfo = plugin.Config.Bind(DEVELOPER, "Log Load SE Info", false, "Log the sound effect info whenever it is changed in the game. Leave this unchecked if you have no idea what it is");
        }



    }
}
