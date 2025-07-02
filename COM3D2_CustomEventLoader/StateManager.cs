using COM3D2.CustomEventLoader.Plugin.DataClass;
using COM3D2.CustomEventLoader.Plugin.Trigger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.CustomEventLoader.Plugin
{
    class StateManager
    {
        public StateManager()
        {

        }

        internal static StateManager Instance;

        internal bool IsRunningCustomEventScreen = false;
        internal bool IsScenarioListCreated = false;

        internal Dictionary<int, ScenarioDefinition> ScenarioList = new Dictionary<int, ScenarioDefinition>();

        internal int SelectedScenarioID = -1;
        internal int UndergoingModEventID = -1;
        internal int BranchIndex = -1;                          //For ADV processing
        internal Dictionary<int, Maid> CharacterSelectionMaidList = new Dictionary<int, Maid>();

        internal List<PartyGroup> PartyGroupList = new List<PartyGroup>();
        internal Maid ClubOwner;

        //Key: Maid Guid, Value.Key: ClothingTag, Value.Value: clothes file name 
        internal Dictionary<string, Dictionary<string, string>> BackupMaidClothingList = new Dictionary<string, Dictionary<string, string>>();
        internal List<Maid> OriginalManOrderList = new List<Maid>();

        internal string ModEventProgress = Constant.EventProgress.None;
        internal string CurrentADVStepID = "";
        internal string ProcessedADVStepID = "";
        internal Dictionary<string, ADVStep> ScenarioSteps = new Dictionary<string, ADVStep>();

        internal List<Maid> SelectedMaidsList = new List<Maid>();
        internal List<Maid> MenList = new List<Maid>();
        internal List<Maid> NPCList = new List<Maid>();                         //For female NPC (both in-game or mod added) only
        internal List<Maid> NPCManList = new List<Maid>();                      //For male NPC only

        internal Dictionary<string, ManClothingInfo> ManClothingList = new Dictionary<string, ManClothingInfo>();   //For storing the info of clothed and nude body of a man

        //Key: Object ID defined in json
        internal Dictionary<string, GameObject> AddedGameObjectList = new Dictionary<string, GameObject>();

        internal bool SpoofActivateMaidObjectFlag = false;          //flag for prevent the system to uninit a maid object when shuffling
        internal bool SpoofAudioLoadPlay = false;

        internal bool WaitForUserClick = false;
        internal bool WaitForUserInput = false;
        internal bool WaitForCameraPanFinish = false;
        internal bool WaitForSystemFadeOut = false;
        internal bool WaitForMotionChange = false;
        internal List<Maid> WaitForFullLoadList = new List<Maid>();                //flag for waiting the scene to load the required characters etc.
        internal CameraView TargetCameraAfterAnimation = null;

        //these 2 are for return the correct maid / man due to not using the list implemented by KISS
        internal string processingMaidGUID = "";
        internal string processingManGUID = "";

        internal bool IsMainGroupMotionScriptFlag = false;

        //For holding custom variables that the scenario creator needs.
        internal Dictionary<string, object> CustomVariable = new Dictionary<string, object>();

        internal List<Maid> ForceLipSyncingList = new List<Maid>();
        internal DateTime LipSyncStartTime = DateTime.MinValue;
        internal DateTime LipSyncEndTime = DateTime.MinValue;

        //Triggers
        internal AnimationEndTrigger AnimationChangeTrigger = null;
        internal List<TimeEndTrigger> TimeEndTriggerList = new List<TimeEndTrigger>();

        internal DateTime ADVResumeTime = DateTime.MinValue;


        //Key: Animation Key, Value: file name
        internal Dictionary<string, byte[]> CustomAnimationList = new Dictionary<string, byte[]>();
        //Key: Clothes Set Key, Value: Clothing Slot Dictionary
        internal Dictionary<string, Dictionary<string, string>> ClothesSetList = new Dictionary<string, Dictionary<string, string>>();

        //Developer mode use
        internal ObjectManagerWindow ObjectWindow;

    }
}
