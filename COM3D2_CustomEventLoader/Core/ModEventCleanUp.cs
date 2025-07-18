﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace COM3D2.CustomEventLoader.Plugin.Core
{
    internal class ModEventCleanUp
    {
        internal static void ResetModEvent()
        {
            RemoveAddedStockMan(StateManager.Instance.MenList);
            RemoveAddedStockMan(StateManager.Instance.NPCManList);

            ResetAllMaid();

            UnloadCharacters(StateManager.Instance.SelectedMaidsList, Constant.CharacterType.Maid);
            if (StateManager.Instance.ClubOwner != null && StateManager.Instance.MenList != null)
                StateManager.Instance.MenList.Add(StateManager.Instance.ClubOwner);
            UnloadCharacters(StateManager.Instance.MenList, Constant.CharacterType.Man);
            UnloadNPC(StateManager.Instance.NPCList);
            UnloadCharacters(StateManager.Instance.NPCManList, Constant.CharacterType.Man);

            //Just want to destory the the following object so doesnt matter if it is calling BanishmentMaid
            StateManager.Instance.MenList.Remove(StateManager.Instance.ClubOwner);
            UnloadNPC(StateManager.Instance.MenList);
            UnloadNPC(StateManager.Instance.NPCManList);

            RemoveAddedObjects();

            RestoreBackupData();
            
            //Reset all the states
            ResetAllState();
        }



        private static void RemoveAddedStockMan(List<Maid> list)
        {
            if (list == null)
                return;
            //Remove the owner from this list in case it is added here (club owner is not temporarily added stock man)
            if (list.Contains(StateManager.Instance.ClubOwner))
                list.Remove(StateManager.Instance.ClubOwner);
            
            foreach (var chara in list)
            {
                //For the man list, since we have add stock man on purpose and it have expanded the list. Need to remove those stock man list properly to prevent logic error when the game try to init all things.
                var m_listStockMan = Traverse.Create(GameMain.Instance.CharacterMgr).Field("m_listStockMan").GetValue<List<Maid>>();
                if (m_listStockMan != null)
                    m_listStockMan.Remove(chara);
            }
        }

        private static void UnloadCharacters(List<Maid> list, Constant.CharacterType type)
        {
            if (list == null)
                return;
            //The position here doesnt matter much, we just want to use a non-zero index here
            int dummy_position = 2;
            foreach (var chara in list)
            {
                //may have scale it to zero to hide the model
                chara.transform.localScale = Vector3.one;

                //In order to unload the characters properly we use the original logic by KISS. Put the character in the maid list and deactivate it using the CharacterMgr
                //Spoof flag is set as we dont want the game to load all the bones again during set active
                StateManager.Instance.SpoofActivateMaidObjectFlag = true;
                if (type == Constant.CharacterType.Man)
                    GameMain.Instance.CharacterMgr.SetActiveMan(chara, dummy_position);
                else if (type == Constant.CharacterType.Maid)
                    GameMain.Instance.CharacterMgr.SetActiveMaid(chara, dummy_position);
                else if (type == Constant.CharacterType.NPC)
                    GameMain.Instance.CharacterMgr.SetActiveMaid(chara, dummy_position);
                StateManager.Instance.SpoofActivateMaidObjectFlag = false;

                bool isMan = !(type == Constant.CharacterType.Maid || type == Constant.CharacterType.NPC);                
                GameMain.Instance.CharacterMgr.Deactivate(dummy_position, isMan);
            }
        }


        private static void RestoreBackupData()
        {
            if (StateManager.Instance.OriginalManOrderList != null)
            {

                //Restore the backup man list
                for (int i = 0; i < GameMain.Instance.CharacterMgr.GetManCount(); i++)
                {
                    if (i >= StateManager.Instance.OriginalManOrderList.Count)
                        break;

                    Maid man = StateManager.Instance.OriginalManOrderList[i];
                    GameMain.Instance.CharacterMgr.SetActiveMan(man, i);
                    GameMain.Instance.CharacterMgr.CharaVisible(i, false, true);
                }
            }  
        }

        private static void ResetAllState()
        {
            StateManager.Instance.IsRunningCustomEventScreen = false;
            StateManager.Instance.UndergoingModEventID = -1;
            StateManager.Instance.BranchIndex = -1;
            StateManager.Instance.SelectedScenarioID = -1;
            

            Util.ClearGenericCollection(StateManager.Instance.CharacterSelectionMaidList);
            Util.ClearGenericCollection(StateManager.Instance.PartyGroupList);
            Util.ClearGenericCollection(StateManager.Instance.OriginalManOrderList);
            Util.ClearGenericCollection(StateManager.Instance.SelectedMaidsList);
            Util.ClearGenericCollection(StateManager.Instance.MenList);
            Util.ClearGenericCollection(StateManager.Instance.NPCList);
            Util.ClearGenericCollection(StateManager.Instance.NPCManList);
            Util.ClearGenericCollection(StateManager.Instance.ManClothingList);

            Util.ClearGenericCollection(StateManager.Instance.TimeEndTriggerList);
            Util.ClearGenericCollection(StateManager.Instance.AddedGameObjectList);

            Util.ClearGenericCollection(StateManager.Instance.BackupMaidClothingList);
            Util.ClearGenericCollection(StateManager.Instance.ScenarioSteps);
            Util.ClearGenericCollection(StateManager.Instance.WaitForFullLoadList);

            Util.ClearGenericCollection(StateManager.Instance.CustomVariable);
            Util.ClearGenericCollection(StateManager.Instance.ForceLipSyncingList);

            Util.ClearGenericCollection(StateManager.Instance.CustomAnimationList);
            Util.ClearGenericCollection(StateManager.Instance.ClothesSetList);
            Util.ClearGenericCollection(StateManager.Instance.ActiveEffectList);
            

            StateManager.Instance.WaitForUserClick = false;
            StateManager.Instance.WaitForUserInput = false;
            StateManager.Instance.WaitForCameraPanFinish = false;
            StateManager.Instance.WaitForSystemFadeOut = false;
            StateManager.Instance.WaitForMotionChange = false;
            StateManager.Instance.SpoofActivateMaidObjectFlag = false;
            StateManager.Instance.SpoofAudioLoadPlay = false;

            StateManager.Instance.AnimationChangeTrigger = null;
            StateManager.Instance.ClubOwner = null;

            StateManager.Instance.ModEventProgress = Constant.EventProgress.None;
            StateManager.Instance.CurrentADVStepID = "";
            StateManager.Instance.ProcessedADVStepID = "";
            
        }


        internal static void RemoveMaidsFromSelectionList(List<Maid> maidList)
        {
            foreach (var maid in maidList)
            {
                SceneCharacterSelect.chara_guid_stock_list.Remove(maid.status.guid);
            }
        }

        private static void RemoveAddedObjects()
        {
            foreach(var kvp in StateManager.Instance.AddedGameObjectList)
                GameMain.Instance.BgMgr.DelPrefabFromBg(kvp.Key);
        }

        private static void ResetAllMaid()
        {
            foreach (Maid maid in StateManager.Instance.SelectedMaidsList)
            {
                CharacterHandling.RestoreMaidClothesInfo(maid);
                maid.ResetAll();
            }
        }

        private static void UnloadNPC(List<Maid> maidList)
        {
            foreach (Maid maid in maidList)
                GameMain.Instance.CharacterMgr.BanishmentMaid(maid);
        }

        //For the maids that are created by the player, there is a thumb icon. Those injected by this mod does not.
        //Use this difference to remove any injected maids that the mod fail to remove properly in previous version.
        internal static void RemoveInjectedModNPC()
        {
            var stockmaids = GameMain.Instance.CharacterMgr.GetStockMaidList();
            for (int i = stockmaids.Count - 1; i >= 0; i--)
            {
                Maid maid = stockmaids[i];               
                if (maid.GetThumIcon() == null)
                {
                    GameMain.Instance.CharacterMgr.BanishmentMaid(maid);
                }
            }
        }
    }
}
