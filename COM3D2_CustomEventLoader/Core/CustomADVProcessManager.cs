using BepInEx.Logging;
using COM3D2.CustomEventLoader.Plugin.Trigger;
using HarmonyLib;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace COM3D2.CustomEventLoader.Plugin.Core
{
    internal partial class CustomADVProcessManager
    {
        private static ManualLogSource Log = CustomEventLoader.Log;

        internal static void ProcessADVStep(ADVKagManager instance)
        {
            

            if (StateManager.Instance.UndergoingModEventID > 0 && StateManager.Instance.ModEventProgress != Constant.EventProgress.Init)
            {
                //If there is no such step found (wrong setup), terminate the mod event.
                if (!StateManager.Instance.ScenarioSteps.ContainsKey(StateManager.Instance.CurrentADVStepID))
                {
                    StateManager.Instance.UndergoingModEventID = -1;
                    return;
                }

                CheckCameraPanFinish();
                CheckTimeWaitFinish();
                CheckSystemFadeOutFinish();

                //We dont want to process this over and over again if we are waiting for user input
                if (StateManager.Instance.ProcessedADVStepID != StateManager.Instance.CurrentADVStepID)
                {

                    ADVStep thisStep = StateManager.Instance.ScenarioSteps[StateManager.Instance.CurrentADVStepID];

                    if (Config.DeveloperMode)
                        Log.LogInfo("Current Step ID: " + thisStep.ID + ", Step Name: " + thisStep.Name );

                    switch (thisStep.Type)
                    {
                        case Constant.ADVType.ChangeBGM:
                            ProcessADVChangeBGM(instance, thisStep);
                            break;
                        case Constant.ADVType.PlaySE:
                            ProcessADVPlaySE(instance, thisStep);
                            break;
                        case Constant.ADVType.ChangeBackground:
                            ProcessADVChangeBackground(instance, thisStep);
                            break;
                        case Constant.ADVType.ChangeCamera:
                            ProcessADVChangeCamera(instance, thisStep);
                            break;
                        case Constant.ADVType.FadeOut:
                            ProcessADVFadeOut(instance, thisStep);
                            break;
                        case Constant.ADVType.Talk:
                            ProcessADVTalk(instance, thisStep);
                            break;
                        case Constant.ADVType.Chara:
                            ProcessADVCharacter(instance, thisStep);
                            break;
                        case Constant.ADVType.Group:
                            ProcessADVGroup(instance, thisStep);
                            break;
                        case Constant.ADVType.CharaInit:
                            ProcessADVCharaInit(instance, thisStep);
                            break;
                        case Constant.ADVType.Branch:
                            ProcessADVBranch(instance, thisStep);
                            break;

                        case Constant.ADVType.MakeGroup:
                            ProcessADVMakeGroup(instance, thisStep);
                            break;
                        case Constant.ADVType.DismissGroup:
                            ProcessADVDismissGroup(instance, thisStep);
                            break;
                        case Constant.ADVType.AddTexture:
                            ProcessADVAddTexture(instance, thisStep);
                            break;
                        case Constant.ADVType.RemoveTexture:
                            ProcessADVRemoveTexture(instance, thisStep);
                            break;
                        case Constant.ADVType.AddObject:
                            ProcessADVAddObject(instance, thisStep);
                            break;
                        case Constant.ADVType.RemoveObject:
                            ProcessADVRemoveObject(instance, thisStep);
                            break;
                        case Constant.ADVType.ShowChoiceList:
                            ProcessADVShowChoiceList(instance, thisStep);
                            break;
                        case Constant.ADVType.TimeWait:
                            ProcessADVTimeWait(instance, thisStep);
                            break;
                        case Constant.ADVType.Evaluate:
                            ProcessADVEvaluate(instance, thisStep);
                            break;
                        case Constant.ADVType.ADVEnd:
                            ProcessADVEnd(instance);
                            break;
                    }


                    if (thisStep.FadeData != null)
                    {
                        if (thisStep.FadeData.IsFadeIn)
                            GameMain.Instance.MainCamera.FadeIn(f_fTime: thisStep.FadeData.Time);
                        else if (thisStep.FadeData.IsFadeOut)
                        {
                            CameraMain.dgOnCompleteFade dg = null;
                            if (thisStep.WaitingType == Constant.WaitingType.FadeOut)
                            {
                                dg = delegate
                                {
                                    ADVSceneProceedToNextStep();
                                };
                            }
                            CharacterHandling.StopAllMaidSound();
                            GameMain.Instance.MainCamera.FadeOut(f_dg: dg, f_fTime: thisStep.FadeData.Time , f_color: thisStep.FadeData.Color);
                        }
                    }



                    switch (thisStep.WaitingType)
                    {
                        case Constant.WaitingType.Auto:
                            ADVSceneProceedToNextStep();
                            break;
                        case Constant.WaitingType.Click:
                            StateManager.Instance.WaitForUserClick = true;
                            break;
                        case Constant.WaitingType.InputChoice:
                            StateManager.Instance.WaitForUserInput = true;
                            break;
                        case Constant.WaitingType.CameraPan:
                            StateManager.Instance.WaitForCameraPanFinish = true;
                            break;
                        case Constant.WaitingType.SystemFadeOut:
                            StateManager.Instance.WaitForSystemFadeOut = true;
                            break;
                    }

                    StateManager.Instance.ProcessedADVStepID = thisStep.ID;
                }
            }
        }

        private static void ProcessADVCharaInit(ADVKagManager instance, ADVStep step)
        {
            CharacterHandling.InitSelectedMaids();
            CharacterHandling.BackupManOrder();


            List<string> validManTypes = step.CharaInitData.ValidManType;

            //init man
            for (int i = 0; i < step.CharaInitData.ManRequired; i++)
            {
                var man = Core.CharacterHandling.InitMan(i, validManTypes);
                StateManager.Instance.MenList.Add(man);
            }
            
            //init the club owner
            StateManager.Instance.ClubOwner = StateManager.Instance.OriginalManOrderList[0];
            StateManager.Instance.ClubOwner.Visible = true;
            StateManager.Instance.ClubOwner.DutPropAll();
            StateManager.Instance.ClubOwner.AllProcPropSeqStart();
            StateManager.Instance.ClubOwner.transform.localPosition = new Vector3(-999f, -999f, -999f);
            
            //init NPC
            StateManager.Instance.NPCList = new List<Maid>();
            if (step.CharaInitData.NPCFemale != null)
            {
                foreach (var npcRequest in step.CharaInitData.NPCFemale.OrderBy(x => x.Index))
                {
                    Maid npc;
                    if (npcRequest.Type == ADVStep.CharaInit.NPCFemaleData.NPCType.Official)
                    {
                        npc = CharacterHandling.InitNPCMaid(npcRequest.Key);
                    }
                    else
                    {
                        npc = CharacterHandling.InitModNPCFemale(npcRequest.CustomData);
                    }

                    StateManager.Instance.NPCList.Insert(npcRequest.Index, npc);
                }
            }

            StateManager.Instance.NPCManList = new List<Maid>();
            if (step.CharaInitData.NPCMale != null)
            {
                foreach (var npcRequest in step.CharaInitData.NPCMale.OrderBy(x => x.Index))
                {
                    Maid npc;

                    npc = CharacterHandling.InitModNPCMale(npcRequest.MaleData);

                    StateManager.Instance.NPCManList.Insert(npcRequest.Index, npc);
                }
            }

            //Init Custom Anim List
            StateManager.Instance.CustomAnimationList = new Dictionary<string, byte[]>();
            if (step.CharaInitData.CustomAnim != null)
            {
                int backupCodePage = ZipConstants.DefaultCodePage;
                ZipConstants.DefaultCodePage = System.Text.Encoding.UTF8.CodePage;

                foreach (var anim in step.CharaInitData.CustomAnim)
                {
                    var scnDef = Util.GetCurrentScenarioDefinition();
                    
                    byte[] fileContent = ScenarioFileHandling.GetCustomEventFileContentInByteArray(scnDef.FilePath, anim.FileName);

                    if (StateManager.Instance.CustomAnimationList.ContainsKey(anim.Key))
                        StateManager.Instance.CustomAnimationList[anim.Key] = fileContent;
                    else
                        StateManager.Instance.CustomAnimationList.Add(anim.Key, fileContent);
                }

                ZipConstants.DefaultCodePage = backupCodePage;
            }

            //Init Clothes Set List
            StateManager.Instance.ClothesSetList = new Dictionary<string, Dictionary<string, string>>();
            if (step.CharaInitData.ClothesSet != null)
            {
                foreach (var clothing in step.CharaInitData.ClothesSet)
                {
                    if (StateManager.Instance.ClothesSetList.ContainsKey(clothing.Key))
                        StateManager.Instance.ClothesSetList[clothing.Key] = clothing.Slots;
                    else
                        StateManager.Instance.ClothesSetList.Add(clothing.Key, clothing.Slots);
                }
            }
        }

        private static void ProcessADVChangeBGM(ADVKagManager instance, ADVStep step)
        {
            GameMain.Instance.SoundMgr.PlayBGM(step.Tag, 1);
        }

        private static void ProcessADVPlaySE(ADVKagManager instance, ADVStep step)
        {
            if (step.SEData.Stop)
                GameMain.Instance.SoundMgr.StopSe();
            if(!string.IsNullOrEmpty(step.SEData.FileName))
                GameMain.Instance.SoundMgr.PlaySe(step.SEData.FileName, step.SEData.IsLoop);
        }

        private static void ProcessADVChangeBackground(ADVKagManager instance, ADVStep step)
        {
            //When change background, should also stop all voice to prevent the character is still talking in a completely new scene if the player is skipping part of the ADV
            CharacterHandling.StopAllMaidSound();

            if (!GameMain.Instance.CharacterMgr.status.isDaytime && !string.IsNullOrEmpty(step.TagForNight))
                instance.TagSetBg(CreateChangeBackgroundTag(step.TagForNight));
            else
                instance.TagSetBg(CreateChangeBackgroundTag(step.Tag));
        }

        private static void ProcessADVFadeOut(ADVKagManager instance, ADVStep step)
        {
            instance.MessageWindowMgr.CloseMessageWindowPanel();
        }

        private static void ProcessADVChangeCamera(ADVKagManager instance, ADVStep step)
        {
            if (step.CameraData.Type == ADVStep.Camera.CameraType.FixedPoint)
            {
                if (step.CameraData.MoveType == ADVStep.Camera.CameraMoveType.Instant)
                {
                    GameMain.Instance.MainCamera.SetPos(step.CameraData.FixedPointData.Pos);
                    GameMain.Instance.MainCamera.SetTargetPos(step.CameraData.FixedPointData.TargetPos);
                    GameMain.Instance.MainCamera.SetDistance(step.CameraData.FixedPointData.Distance);
                    GameMain.Instance.MainCamera.SetAroundAngle(step.CameraData.FixedPointData.AroundAngle);
                }
                else
                {
                    CameraHandling.AnimateCameraToLookAt(step.CameraData.FixedPointData.TargetPos, step.CameraData.FixedPointData.Distance,
                        step.CameraData.FixedPointData.AroundAngle.x, step.CameraData.FixedPointData.AroundAngle.y, step.CameraData.AnimationTime);
                }
            }
            else if (step.CameraData.Type == ADVStep.Camera.CameraType.LookAt)
            {

                Maid target = null;
                if (step.CameraData.LookAtData.Target == ADVStep.Camera.TargetType.Owner)
                    target = StateManager.Instance.ClubOwner;
                else
                {
                    int index = step.CameraData.LookAtData.ArrayPosition;
                    
                    if (step.CameraData.LookAtData.Target == ADVStep.Camera.TargetType.Maid)
                        target = StateManager.Instance.SelectedMaidsList[index];
                    else if (step.CameraData.LookAtData.Target == ADVStep.Camera.TargetType.Man)
                        target = StateManager.Instance.MenList[index];
                    else if (step.CameraData.LookAtData.Target == ADVStep.Camera.TargetType.GroupMaid1)
                        target = StateManager.Instance.PartyGroupList[index].Maid1;
                    else if (step.CameraData.LookAtData.Target == ADVStep.Camera.TargetType.GroupMaid2)
                        target = StateManager.Instance.PartyGroupList[index].Maid2;
                }

                if (step.CameraData.LookAtData.UseDefaultCameraWorkSetting)
                {
                    if (step.CameraData.MoveType == ADVStep.Camera.CameraMoveType.Instant)
                        CameraHandling.SetCameraLookAt(target);
                    else
                        CameraHandling.AnimateCameraToLookAt(target);
                }
                else
                {
                    if (step.CameraData.MoveType == ADVStep.Camera.CameraMoveType.Instant)
                        CameraHandling.SetCameraLookAt(target, step.CameraData.LookAtData.OffsetData.Pos, step.CameraData.LookAtData.OffsetData.TargetPos, 
                            step.CameraData.LookAtData.OffsetData.Distance, step.CameraData.LookAtData.OffsetData.AroundAngle.x, step.CameraData.LookAtData.OffsetData.AroundAngle.y);
                    else
                        CameraHandling.AnimateCameraToLookAt(target, step.CameraData.LookAtData.OffsetData.Pos, 
                            step.CameraData.LookAtData.OffsetData.Distance, step.CameraData.LookAtData.OffsetData.AroundAngle.x, step.CameraData.LookAtData.OffsetData.AroundAngle.y);
                }

            }

            if(step.CameraData.LockCamera != null)
            {
                GameMain.Instance.MainCamera.SetControl(!step.CameraData.LockCamera.IsLock);
            }
            if(step.CameraData.BlurCamera != null)
            {
                GameMain.Instance.MainCamera.Blur(step.CameraData.BlurCamera.IsBlur);
            }
        }


        private static void ProcessADVCharacter(ADVKagManager instance, ADVStep step)
        {
            if (step.CharaData != null)
            {
                for (int i = 0; i < step.CharaData.Length; i++)
                {

                    List<Maid> targetList;

                    if (step.CharaData[i].Type == Constant.TargetType.SingleMan)
                    {
                        if (step.CharaData[i].IsMaster)
                        {
                            targetList = new List<Maid>();
                            targetList.Add(StateManager.Instance.ClubOwner);
                        }
                        else
                            targetList = StateManager.Instance.MenList;

                    }
                    else if (step.CharaData[i].Type == Constant.TargetType.NPCFemale)
                        targetList = StateManager.Instance.NPCList;
                    else if (step.CharaData[i].Type == Constant.TargetType.NPCMale)
                        targetList = StateManager.Instance.NPCManList;
                    else
                        targetList = StateManager.Instance.SelectedMaidsList;
                    
                    if (step.CharaData[i].Type == Constant.TargetType.AllMaids)
                    {
                        foreach (var maid in StateManager.Instance.SelectedMaidsList)
                        {
                            SetADVCharaDataToCharacter(maid, step.CharaData[i], false);
                        }
                    }
                    else if (step.CharaData[i].Type == Constant.TargetType.AllMen)
                    {
                        foreach (var man in StateManager.Instance.MenList)
                        {
                            SetADVCharaDataToCharacter(man, step.CharaData[i], true);
                        }
                        if (StateManager.Instance.ClubOwner != null)
                        {
                            SetADVCharaDataToCharacter(StateManager.Instance.ClubOwner, step.CharaData[i], true);
                        }
                    }
                    else if (step.CharaData[i].Type == Constant.TargetType.AllNPCFemale)
                    {
                        foreach (var maid in StateManager.Instance.NPCList)
                        {
                            SetADVCharaDataToCharacter(maid, step.CharaData[i], false);
                        }
                    }
                    else if (step.CharaData[i].Type == Constant.TargetType.AllNPCMale)
                    {
                        foreach (var man in StateManager.Instance.NPCManList)
                        {
                            SetADVCharaDataToCharacter(man, step.CharaData[i], true);
                        }
                    }
                    else
                    {
                        int index = step.CharaData[i].ArrayPosition;
                        if (step.CharaData[i].UseBranchIndex)
                            index = StateManager.Instance.BranchIndex;
                        if (targetList.Count > index)
                        {
                            SetADVCharaDataToCharacter(targetList[index], step.CharaData[i], targetList[index].boMAN);

                        }
                    }

                }
            }
        }


        internal static void SetCharacterEyeSight(Maid maid, EyeSightSetting eyeSightSetting)
        {
            if (maid == null)
                return;
            if (eyeSightSetting == null)
                return;

            if (eyeSightSetting.Type == EyeSightSetting.EyeSightType.ToCamera)
            {
                maid.EyeToCamera((Maid.EyeMoveType)eyeSightSetting.EyeToCameraSetting.MoveType);
            }
            else if (eyeSightSetting.Type == EyeSightSetting.EyeSightType.ToChara)
            {
                Maid target = null;
                if (eyeSightSetting.EyeToCharaSetting.Type == EyeSightSetting.EyeToCharaSettingDetail.TargetType.ClubOwner)
                    target = StateManager.Instance.ClubOwner;
                else if (eyeSightSetting.EyeToCharaSetting.Type == EyeSightSetting.EyeToCharaSettingDetail.TargetType.Man)
                    target = StateManager.Instance.MenList[eyeSightSetting.EyeToCharaSetting.ArrayPosition];
                else if (eyeSightSetting.EyeToCharaSetting.Type == EyeSightSetting.EyeToCharaSettingDetail.TargetType.Maid)
                    target = StateManager.Instance.SelectedMaidsList[eyeSightSetting.EyeToCharaSetting.ArrayPosition];
                else if (eyeSightSetting.EyeToCharaSetting.Type == EyeSightSetting.EyeToCharaSettingDetail.TargetType.NPCMale)
                    target = StateManager.Instance.NPCManList[eyeSightSetting.EyeToCharaSetting.ArrayPosition];
                else if (eyeSightSetting.EyeToCharaSetting.Type == EyeSightSetting.EyeToCharaSettingDetail.TargetType.NPCFemale)
                    target = StateManager.Instance.NPCList[eyeSightSetting.EyeToCharaSetting.ArrayPosition];
                else if (eyeSightSetting.EyeToCharaSetting.Type == EyeSightSetting.EyeToCharaSettingDetail.TargetType.GroupMember)
                {
                    PartyGroup group = StateManager.Instance.PartyGroupList[eyeSightSetting.EyeToCharaSetting.ArrayPosition];
                    if (eyeSightSetting.EyeToCharaSetting.TargetGroupMember == EyeSightSetting.GroupMemberType.Maid1)
                        target = group.Maid1;
                    else if (eyeSightSetting.EyeToCharaSetting.TargetGroupMember == EyeSightSetting.GroupMemberType.Maid2)
                        target = group.Maid2;
                    else if (eyeSightSetting.EyeToCharaSetting.TargetGroupMember == EyeSightSetting.GroupMemberType.Man1)
                        target = group.Man1;
                    else if (eyeSightSetting.EyeToCharaSetting.TargetGroupMember == EyeSightSetting.GroupMemberType.Man2)
                        target = group.Man2;
                    else if (eyeSightSetting.EyeToCharaSetting.TargetGroupMember == EyeSightSetting.GroupMemberType.Man3)
                        target = group.Man3;
                }


                if (target != null)
                {
                    //The logic in Maid.EyeToTarget is wrong. The parameter in the replace function for man case is swapped. We have to supply the name of the bone to avoid the problem.
                    string targetBone = Constant.DefinedGameObjectNames.MaidHeadBoneName;
                    if (target.boMAN)
                        targetBone = Constant.DefinedGameObjectNames.ManHeadBoneName;

                    maid.EyeToTarget(target, 0.5f, targetBone);
                }
            }
            else if (eyeSightSetting.Type == EyeSightSetting.EyeSightType.ToObject)
            {
                maid.EyeToTargetObject(eyeSightSetting.EyeToObjectSetting.Target.transform);
            }
            else if (eyeSightSetting.Type == EyeSightSetting.EyeSightType.Reset)
            {
                maid.EyeToReset();
            }
        }

        private static void SetADVCharaDataToCharacter(Maid maid, ADVStep.ShowChara charaData, bool isMan = false)
        {
            maid.Visible = charaData.Visible;
            if (charaData.Visible)
                maid.transform.localScale = Vector3.one;

            if (!isMan)
            {
                if (!string.IsNullOrEmpty(charaData.FaceAnime))
                {
                    SetFaceAnimeToMaid(maid, charaData.FaceAnime);
                }

                if (!string.IsNullOrEmpty(charaData.FaceBlend))
                {
                    maid.FaceBlend(charaData.FaceBlend);
                }

                maid.OpenMouth(charaData.OpenMouth);

                CharacterHandling.SetFemaleClothing(maid, charaData.ClothesSetID);

            }
            else
            {
                CharacterHandling.SetManClothing(maid, charaData.IsManNude);
            }

            if (charaData.Effect != null)
                CharacterHandling.UpdateCharacterEffect(maid, charaData.Effect.ActiveEffects);
            

            SetCharacterEyeSight(maid, charaData.EyeSight);

            CharacterHandling.ApplyMotionInfoToCharacter(maid, charaData.MotionInfo);


            if (charaData.PosRot != null)
            {
                Util.StopSmoothMove(maid);
                if (charaData.SmoothMovement != null)
                {
                    Util.SmoothMoveMaidPosition(maid, charaData.PosRot.Pos, charaData.PosRot.Rot, charaData.SmoothMovement.Time );
                }
                else
                {
                    maid.transform.localPosition = Vector3.zero;
                    maid.transform.position = charaData.PosRot.Pos;
                    maid.transform.localRotation = new Quaternion(0, 0, 0, 0);
                    maid.transform.rotation = charaData.PosRot.Rot;
                }
                //Need to call the following to fix the gravity
                maid.body0.SetBoneHitHeightY(charaData.PosRot.Pos.y);
            }

            if (charaData.ExtraObjectsInfo != null)
            {
                CharacterHandling.RemoveObjectFromCharacter(maid, charaData.ExtraObjectsInfo.RemoveObjects);
                CharacterHandling.AttachObjectToCharacter(maid, charaData.ExtraObjectsInfo.AddObjects);
            }

            StateManager.Instance.WaitForFullLoadList.Add(maid);
        }

        private static void ProcessADVGroupIndividual(Maid maid, ADVStep.ShowGroupMotion.DetailSetup setupData)
        {
            if (maid == null || setupData == null)
                return;

            maid.Visible = setupData.Visible;
            if (maid.Visible)
            {
                CharacterHandling.SetManClothing(maid, setupData.IsManNude);
                maid.OpenMouth(setupData.OpenMouth);

                if (!string.IsNullOrEmpty(setupData.FaceAnime))
                {
                    SetFaceAnimeToMaid(maid, setupData.FaceAnime);
                }

                if (!string.IsNullOrEmpty(setupData.FaceBlend))
                    maid.FaceBlend(setupData.FaceBlend);

                SetCharacterEyeSight(maid, setupData.EyeSight);

                if (setupData.PosRot != null)
                {
                    maid.transform.localPosition = Vector3.zero;
                    maid.transform.position = setupData.PosRot.Pos;
                    maid.body0.SetBoneHitHeightY(setupData.PosRot.Pos.y);
                    maid.SetRot(Vector3.zero);
                    maid.transform.rotation = setupData.PosRot.Rot;
                }
            }
            else
            {
                maid.transform.localScale = Vector3.zero;
                CharacterHandling.SetManClothing(maid, false);
            }
        }

        internal static void SetFaceAnimeToMaid(Maid maid, string faceAnime)
        {
            if (maid == null)
                return;

            maid.FaceAnime(faceAnime);
        }

        private static void ProcessADVGroup(ADVKagManager instance, ADVStep step)
        {
            if (step.GroupData != null)
            {
                for (int i = 0; i < step.GroupData.Length; i++)
                {
                    int targetGroupIndex = step.GroupData[i].ArrayPosition;
                    //if (step.GroupData[i].UseRandomPick)
                    //    targetGroupIndex = StateManager.Instance.RandomPickIndexList[step.GroupData[i].ArrayPosition];

                    
                    PartyGroup group = StateManager.Instance.PartyGroupList[targetGroupIndex];
                    
                    //could be a empty group due to not enough character selected
                    if (group.GroupType != Constant.GroupType.Invalid)
                    {
                        if (!string.IsNullOrEmpty(step.GroupData[i].ScriptFile) && !string.IsNullOrEmpty(step.GroupData[i].ScriptLabel))
                        {
                            string manID = "";
                            if (group.Man1 != null)
                                manID = group.Man1.status.guid;
                            
                            
                            CharacterHandling.LoadMotionScript(0, false, step.GroupData[i].ScriptFile, step.GroupData[i].ScriptLabel, group.Maid1.status.guid, manID,
                                false, true, false, false);

                        }
                        
                        ProcessADVGroupIndividual(group.Maid1, step.GroupData[i].Maid1);
                        ProcessADVGroupIndividual(group.Maid2, step.GroupData[i].Maid2);
                        ProcessADVGroupIndividual(group.Man1, step.GroupData[i].Man1);
                        ProcessADVGroupIndividual(group.Man2, step.GroupData[i].Man2);
                        ProcessADVGroupIndividual(group.Man3, step.GroupData[i].Man3);
                        
                        StateManager.Instance.WaitForFullLoadList.Add(group.Maid1);
                        if (group.Maid2 != null)
                            StateManager.Instance.WaitForFullLoadList.Add(group.Maid2);
                        if (group.Man1 != null)
                            StateManager.Instance.WaitForFullLoadList.Add(group.Man1);
                        if (group.Man2 != null)
                            StateManager.Instance.WaitForFullLoadList.Add(group.Man2);
                        if (group.Man3 != null)
                           StateManager.Instance.WaitForFullLoadList.Add(group.Man3);
                        
                        if (step.GroupData[i].PosRot != null)
                            group.SetGroupPosition(step.GroupData[i].PosRot.Pos, step.GroupData[i].PosRot.Rot);
                        
                        group.SetGroupPosition();
                        
                        if (step.GroupData[i].BlockInputUntilMotionChange && !Config.DebugIgnoreADVForceTimeWait)
                        {
                            StateManager.Instance.WaitForMotionChange = true;
                            AnimationEndTrigger trigger = new AnimationEndTrigger(group.Maid1, new EventDelegate(ADVMotionChangeComplete));
                            StateManager.Instance.AnimationChangeTrigger = trigger;
                        }
                    }
                }
            }
        }


        private static void ProcessADVTalk(ADVKagManager instance, ADVStep step)
        {
            string speakerName;
            int voicePitch = 0;
            List<Maid> lstMaidToSpeak = null;

            switch (step.TalkData.SpecificSpeaker)
            {
                case Constant.ADVTalkSpearkerType.All:
                    lstMaidToSpeak = StateManager.Instance.SelectedMaidsList;
                    speakerName = step.TalkData.SpeakerName;
                    break;
                case Constant.ADVTalkSpearkerType.Narrative:
                    speakerName = "";
                    break;
                case Constant.ADVTalkSpearkerType.Owner:
                    speakerName = GameMain.Instance.CharacterMgr.status.playerName;
                    break;
                case Constant.ADVTalkSpearkerType.SelectedMaid:
                    Maid mainMaid = StateManager.Instance.SelectedMaidsList[0];
                    lstMaidToSpeak = new List<Maid>() { mainMaid };
                    speakerName = mainMaid.status.callName;
                    break;
                case Constant.ADVTalkSpearkerType.Maid:
                    int index = step.TalkData.Index;
                    if (step.TalkData.UseBranchIndex)
                        index = StateManager.Instance.BranchIndex;
                    Maid maid = StateManager.Instance.SelectedMaidsList[index];
                    lstMaidToSpeak = new List<Maid>() { maid };
                    speakerName = maid.status.callName;
                    break;
                case Constant.ADVTalkSpearkerType.NPCFemale:
                    Maid npcMaid = StateManager.Instance.NPCList[step.TalkData.Index];
                    lstMaidToSpeak = new List<Maid>() { npcMaid };
                    speakerName = npcMaid.status.callName;
                    break;
                case Constant.ADVTalkSpearkerType.NPCMale:
                    if (Product.isJapan)
                        speakerName = StateManager.Instance.NPCManList[step.TalkData.Index].status.fullNameJpStyle;
                    else
                        speakerName = StateManager.Instance.NPCManList[step.TalkData.Index].status.fullNameEnStyle;
                    break;
                default:
                    speakerName = step.TalkData.SpeakerName;
                    break;
            }
            
            if (lstMaidToSpeak == null)
            {
                //there is no audio to be played in this step, narrative or a man speaking
                DisplayAdvText(instance, speakerName, step.TalkData.Text, "", voicePitch, AudioSourceMgr.Type.Voice);
            }
            else
            {
                CharacterHandling.StopAllMaidSound();

                bool isAudioChopped = false;
                ADVStep.Talk.Voice voiceInfo = null;

                //add audio to maids
                bool isAll = step.TalkData.SpecificSpeaker == Constant.ADVTalkSpearkerType.All;
                
                if (step.TalkData.VoiceData != null)
                {
                    
                    foreach (var maid in lstMaidToSpeak)
                    {
                        //get the correct voice file by personality
                        if (step.TalkData.SpecificSpeaker == Constant.ADVTalkSpearkerType.NPCFemale)
                            voiceInfo = step.TalkData.VoiceData.First().Value;
                        else
                        {
                            if (step.TalkData.VoiceData.ContainsKey(Util.GetPersonalityNameByValue(maid.status.personal.id)))
                                voiceInfo = step.TalkData.VoiceData[Util.GetPersonalityNameByValue(maid.status.personal.id)];
                        }

                        if (voiceInfo != null)
                        {
                            if (voiceInfo.IsChoppingAudio)
                            {
                                isAudioChopped = true;

                                Helper.AudioChoppingManager.PlaySubClip(maid, step.ID, voiceInfo.VoiceFile, voiceInfo.StartTime, voiceInfo.EndTime, isAll);
                            }
                            else
                            {
                                maid.AudioMan.LoadPlay(voiceInfo.VoiceFile, 0f, false);
                            }
                            maid.AudioMan.audiosource.volume = voiceInfo.Volume;
                        }
                        
                    }
                }
                else
                {
                    foreach (var maid in lstMaidToSpeak)
                    {
                        if (!maid.boMAN)
                        {
                            //no voice file assigned, try to make mouth movement
                            StateManager.Instance.LipSyncStartTime = DateTime.Now;
                            StateManager.Instance.LipSyncEndTime = DateTime.Now.AddSeconds(step.TalkData.Text.Length / 4);
                            
                            StateManager.Instance.ForceLipSyncingList.Add(maid);
                            
                            maid.StartKuchipakuPattern(0f, Constant.LipSyncPattern, true);
                        }
                    }
                }

                if (!isAll)
                {
                    string voiceFile = "";
                    if (voiceInfo != null)
                        voiceFile = voiceInfo.VoiceFile;

                    //if it is chopped we use the id instead to reload from our own subclip library
                    if (isAudioChopped)
                        voiceFile = step.ID;
                    //single maid speak and no chopping
                    voicePitch = lstMaidToSpeak[0].VoicePitch;
                    speakerName = lstMaidToSpeak[0].status.callName;

                    DisplayAdvText(instance, speakerName, step.TalkData.Text, voiceFile, voicePitch, AudioSourceMgr.Type.VoiceHeroine);
                }
                else
                {
                    //the case of hard to set the replay, skip it
                    DisplayAdvText(instance, speakerName, step.TalkData.Text, "", voicePitch, AudioSourceMgr.Type.VoiceHeroine);
                }

            }
        }


        private static string PrepareDialogueText(string text)
        {
            text = PrepareCharacterNameText(text);
            text = PrepareVariableText(text);
            //text = PrepareRandomGroupCharacterName(text);

            text = text.Replace(Constant.JsonReplaceTextLabels.ClubName, GameMain.Instance.CharacterMgr.status.clubName);
            text = text.Replace(Constant.JsonReplaceTextLabels.ClubOwnerName, GameMain.Instance.CharacterMgr.status.playerName);

            return text;
        }

        private static string PrepareCharacterNameText(string text)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(Constant.JsonReplaceTextLabels.CharacterNameRegex, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            var matches = regex.Matches(text);
            foreach (System.Text.RegularExpressions.Match match in matches)
            {

                List<Maid> sourceList = null;
                if (match.Groups[1].Value == Constant.JsonReplaceTextLabels.CharacterNameSourceType.NPCMale)
                    sourceList = StateManager.Instance.NPCManList;
                else if (match.Groups[1].Value == Constant.JsonReplaceTextLabels.CharacterNameSourceType.NPCFemale)
                    sourceList = StateManager.Instance.NPCList;
                else if (match.Groups[1].Value == Constant.JsonReplaceTextLabels.CharacterNameSourceType.Maid)
                    sourceList = StateManager.Instance.SelectedMaidsList;

                if (sourceList == null)
                    return text;

                Maid maid = sourceList[int.Parse(match.Groups[2].Value)];

                string displayName = "";
                if (match.Groups[3].Value == Constant.JsonReplaceTextLabels.CharacterNameDisplayType.FullName)
                {
                    if (Product.isJapan)
                        displayName = maid.status.fullNameJpStyle;
                    else
                        displayName = maid.status.fullNameEnStyle;
                }
                else if (match.Groups[3].Value == Constant.JsonReplaceTextLabels.CharacterNameDisplayType.FirstName)
                    displayName = maid.status.firstName;
                else if (match.Groups[3].Value == Constant.JsonReplaceTextLabels.CharacterNameDisplayType.LastName)
                    displayName = maid.status.lastName;
                else if (match.Groups[3].Value == Constant.JsonReplaceTextLabels.CharacterNameDisplayType.NickName)
                    displayName = maid.status.nickName;
                else if (match.Groups[3].Value == Constant.JsonReplaceTextLabels.CharacterNameDisplayType.CallName)
                    displayName = maid.status.callName;

                text = text.Replace(match.Groups[0].Value, displayName);

            }

            return text;
        }

        private static string PrepareVariableText(string text)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(Constant.JsonReplaceTextLabels.VariableRegex, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            var matches = regex.Matches(text);
            
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                if (!StateManager.Instance.CustomVariable.ContainsKey(match.Groups[1].Value))
                    continue;
                
                text = text.Replace(match.Groups[0].Value, StateManager.Instance.CustomVariable[match.Groups[1].Value].ToString());
            }

            return text;
        }

        private static void ProcessADVEnd(ADVKagManager instance)
        {
            instance.MessageWindowMgr.CloseMessageWindowPanel();
            SceneHandling.ShowEventListScreen(
                new EventDelegate(() => 
                {
                    ModEventCleanUp.ResetModEvent();
                    StateManager.Instance.IsRunningCustomEventScreen = true;
                })
            );
        }

        private static void ProcessADVBranch(ADVKagManager instance, ADVStep step)
        {
            try
            {
                if (step.BranchData != null)
                {
                    object varObject = StateManager.Instance.CustomVariable[step.BranchData.VariableName];

                    var branchList = step.BranchData.BranchList.OrderBy(x => x.Value).ToList();
                    if (step.BranchData.CompareMethod == Constant.OperatorType.GreaterThan || step.BranchData.CompareMethod == Constant.OperatorType.GreaterThanEqualTo)
                    {
                        branchList = step.BranchData.BranchList.OrderByDescending(x => x.Value).ToList();
                    }

                    foreach (var branchItem in branchList)
                    {
                        //by default it is a string
                        object valueToCompare = branchItem.Value;
                        if (step.BranchData.VariableType == Constant.VariableType.Integer)
                            valueToCompare = int.Parse(branchItem.Value);
                        else if (step.BranchData.VariableType == Constant.VariableType.FloatingPoint)
                            valueToCompare = double.Parse(branchItem.Value);
                        else if (step.BranchData.VariableType == Constant.VariableType.Boolean)
                            valueToCompare = bool.Parse(branchItem.Value);

                        if (Util.ComparativeOperation(step.BranchData.CompareMethod, varObject, valueToCompare))
                        {
                            ADVSceneProceedToNextStep(branchItem.NextStepID);
                            break;
                        }
                    }

                }
            }catch (EvaluateException e)
            {
                Util.ShowError(step.ID, e.Message);
            }
            
        }


        private static void ProcessADVMakeGroup(ADVKagManager instance, ADVStep step)
        {
            if (step.GroupFormat != null)
            {
                foreach (ADVStep.MakeGroupFormat groupInfo in step.GroupFormat)
                {
                    PartyGroup group;

                    while (StateManager.Instance.PartyGroupList.Count <= groupInfo.GroupIndex)
                        StateManager.Instance.PartyGroupList.Add(new PartyGroup());

                    group = new PartyGroup();
                    StateManager.Instance.PartyGroupList[groupInfo.GroupIndex] = group;

                    group.Maid1 = GetMaidForMakeGroupFormatRequest(groupInfo.Maid1, false);
                    group.Maid2 = GetMaidForMakeGroupFormatRequest(groupInfo.Maid2, false);

                    group.Man1 = GetMaidForMakeGroupFormatRequest(groupInfo.Man1, true);
                    group.Man2 = GetMaidForMakeGroupFormatRequest(groupInfo.Man2, true);
                    group.Man3 = GetMaidForMakeGroupFormatRequest(groupInfo.Man3, true);

                    group.GroupOffsetVector = Vector3.zero;
                    if(group.Maid1 != null)
                        group.SetGroupPosition(group.Maid1.transform.position, group.Maid1.transform.rotation);

                }
            }

        }

        private static Maid GetMaidForMakeGroupFormatRequest(ADVStep.MakeGroupFormat.GroupMemberInfo requestInfo, bool isMan)
        {
            if (requestInfo == null)
                return null;

            if (isMan)
            {
                if (requestInfo.Type == ADVStep.MakeGroupFormat.MemberType.Owner)
                    return StateManager.Instance.ClubOwner;
                List<Maid> targetList;
                if (requestInfo.Type == ADVStep.MakeGroupFormat.MemberType.NPC)
                    targetList = StateManager.Instance.NPCManList;
                else
                    targetList = StateManager.Instance.MenList;

                if (targetList.Count > requestInfo.ArrayPosition)
                    return targetList[requestInfo.ArrayPosition];
            }
            else
            {
                List<Maid> targetList;
                if (requestInfo.Type == ADVStep.MakeGroupFormat.MemberType.NPC)
                    targetList = StateManager.Instance.NPCList;
                else
                    targetList = StateManager.Instance.SelectedMaidsList;

                if (targetList.Count > requestInfo.ArrayPosition)
                    return targetList[requestInfo.ArrayPosition];
            }

            return null;
        }

        private static void ProcessADVDismissGroup(ADVKagManager instance, ADVStep step)
        {
            if (step.Tag == "ALL")
            {
                foreach (var group in StateManager.Instance.PartyGroupList)
                    group.DetachAllIK();
                StateManager.Instance.PartyGroupList.Clear();
            }
            else
            {
                int groupIndex = int.Parse(step.Tag);
                StateManager.Instance.PartyGroupList[groupIndex].DetachAllIK();
                StateManager.Instance.PartyGroupList.RemoveAt(groupIndex);
            }
        }

        private static void ProcessADVAddTexture(ADVKagManager instance, ADVStep step)
        {
            if (step.TextureData != null)
            {
                foreach (ADVStep.Texture data in step.TextureData)
                {
                    Dictionary<string, TexturePattern> dictPattern = null;
                    if (data.Type == Constant.TextureType.Semen)
                        dictPattern = ModUseData.TexturePatternList[Constant.TextureType.Semen];
                    else if (data.Type == Constant.TextureType.WhipMark)
                        dictPattern = ModUseData.TexturePatternList[Constant.TextureType.WhipMark];
                    else if (data.Type == Constant.TextureType.Candle)
                        dictPattern = ModUseData.TexturePatternList[Constant.TextureType.Candle];
                    else if (data.Type == Constant.TextureType.SlapMark)
                        dictPattern = ModUseData.TexturePatternList[Constant.TextureType.SlapMark];

                    //skip if invalid pattern type
                    if (dictPattern == null)
                        continue;

                    if (data.TargetType == Constant.TargetType.AllMaids)
                    {
                        foreach (Maid maid in StateManager.Instance.SelectedMaidsList)
                        {
                            foreach (string bodyPart in data.BodyTarget)
                                if (dictPattern.ContainsKey(bodyPart))
                                    CharacterHandling.AddTexture(maid, dictPattern[bodyPart]);
                        }

                    }
                    else
                    {
                        Maid maid = null;
                        if (data.TargetType == Constant.TargetType.SingleMaid)
                            maid = StateManager.Instance.SelectedMaidsList[data.IndexPosition];
                        else if (data.TargetType == Constant.TargetType.NPCFemale)
                            maid = StateManager.Instance.NPCList[data.IndexPosition];

                        foreach (string bodyPart in data.BodyTarget)
                            if (dictPattern.ContainsKey(bodyPart))
                                CharacterHandling.AddTexture(maid, dictPattern[bodyPart]);
                    }
                }
            }
        }

        private static void ProcessADVRemoveTexture(ADVKagManager instance, ADVStep step)
        {
            if (step.TextureData != null)
            {
                foreach (ADVStep.Texture data in step.TextureData)
                {
                    int layer = -1;
                    if (data.Type == Constant.TextureType.Semen)
                        layer = TexturePattern.SemenLayer;
                    else if (data.Type == Constant.TextureType.WhipMark)
                        layer = TexturePattern.WhipMarkLayer;
                    else if (data.Type == Constant.TextureType.Candle)
                        layer = TexturePattern.CandleLayer;
                    else if (data.Type == Constant.TextureType.SlapMark)
                        layer = TexturePattern.SlapMarkLayer;

                    if (layer < 0)
                        continue;

                    if (data.TargetType == Constant.TargetType.AllMaids)
                    {
                        foreach (Maid maid in StateManager.Instance.SelectedMaidsList)
                            CharacterHandling.RemoveTexture(maid, layer);
                    }
                    else
                    {
                        Maid maid = null;
                        if (data.TargetType == Constant.TargetType.SingleMaid)
                            maid = StateManager.Instance.SelectedMaidsList[data.IndexPosition];
                        else if (data.TargetType == Constant.TargetType.NPCFemale)
                            maid = StateManager.Instance.NPCList[data.IndexPosition];

                        CharacterHandling.RemoveTexture(maid, layer);
                    }
                }
            }
        }

        private static void ProcessADVAddObject(ADVKagManager instance, ADVStep step)
        {
            if(step.WorldObjectData != null)
            {
                foreach(var objData in step.WorldObjectData)
                {
                    GameObject addedObject = GameMain.Instance.BgMgr.AddPrefabToBg(objData.Src, objData.ObjectID, "", Vector3.zero, Vector3.zero);

                    if (addedObject != null)
                    {
                        addedObject.transform.position = objData.PosRot.Pos;
                        addedObject.transform.rotation = objData.PosRot.Rot;
                        addedObject.transform.localScale = new Vector3(objData.Scale, objData.Scale, objData.Scale);
                        StateManager.Instance.AddedGameObjectList.Add(objData.ObjectID, addedObject);
                    }
                }
            }
        }

        private static void ProcessADVRemoveObject(ADVKagManager instance, ADVStep step)
        {
            if (step.WorldObjectData != null)
            {
                foreach (var objData in step.WorldObjectData)
                {
                    GameMain.Instance.BgMgr.DelPrefabFromBg(objData.ObjectID);
                    
                    StateManager.Instance.AddedGameObjectList.Remove(objData.ObjectID);
                }
            }
        }

        private static void ProcessADVShowChoiceList(ADVKagManager instance, ADVStep step)
        {
            if (step.ChoiceData != null)
            {
                //Key: Display Text;    Value.Key: Value;    Value.Value: IsEnabled
                List<KeyValuePair<string, KeyValuePair<string, bool>>> lstChoice = new List<KeyValuePair<string, KeyValuePair<string, bool>>>();

                foreach (var choice in step.ChoiceData.Options)
                {
                    lstChoice.Add(new KeyValuePair<string, KeyValuePair<string, bool>>(choice.Value, new KeyValuePair<string, bool>(choice.Key, true)));
                }


                Action<string, string> onClickCallBack = delegate (string displayText, string value)
                {
                    //proceed to next step
                    Util.SetCustomVariable(step.ChoiceData.Variable, value);
                    ADVSceneProceedToNextStep();
                };

                instance.MessageWindowMgr.CreateSelectButtons(lstChoice, onClickCallBack);
            }
        }

        private static void ProcessADVEvaluate(ADVKagManager instance, ADVStep step)
        {
            if (step.EvalData != null)
            {
                object input1 = GetEvalInputValue(step.EvalData.Input1);
                object input2 = GetEvalInputValue(step.EvalData.Input2);
                
                object result = null;

                try
                {

                    switch (step.EvalData.Operator)
                    {
                        case Constant.OperatorType.Assignment:
                            result = input1;
                            break;
                        case Constant.OperatorType.Addition:
                        case Constant.OperatorType.Subtraction:
                        case Constant.OperatorType.Multiplication:
                        case Constant.OperatorType.Division:
                            result = Util.ArithmeticOperation(step.EvalData.Operator, input1, input2);
                            break;
                        case Constant.OperatorType.Equal:
                        case Constant.OperatorType.NotEqual:
                        case Constant.OperatorType.GreaterThanEqualTo:
                        case Constant.OperatorType.GreaterThan:
                        case Constant.OperatorType.LessThanEqualTo:
                        case Constant.OperatorType.LessThan:
                        
                            result = Util.ComparativeOperation(step.EvalData.Operator, input1, input2);
                            break;
                        case Constant.OperatorType.LogicalAnd:
                        case Constant.OperatorType.LogicalOr:
                        case Constant.OperatorType.Negation:
                            result = Util.LogicalOperation(step.EvalData.Operator, input1, input2);
                            break;
                        case Constant.OperatorType.Concatenation:
                            result = input1?.ToString() + input2?.ToString();
                            break;
                    }

                    StateManager.Instance.CustomVariable.Remove(step.EvalData.ResultVariableName);
                    StateManager.Instance.CustomVariable.Add(step.EvalData.ResultVariableName, result);
                }
                catch(EvaluateException e)
                {
                    Util.ShowError(step.ID, e.Message);
                }

                
            }
        }


        private static object GetEvalInputValue(ADVStep.Evaluate.InputDetail inputDetail)
        {
            if (inputDetail == null)
                return null;

            if (inputDetail.SourceType == ADVStep.Evaluate.SourceType.Variable)
            {
                if (StateManager.Instance.CustomVariable.ContainsKey(inputDetail.Variable.VariableName))
                    return StateManager.Instance.CustomVariable[inputDetail.Variable.VariableName];
            }
            else if (inputDetail.SourceType == ADVStep.Evaluate.SourceType.CharcterStatus)
            {
                Maid target = null;
                //Get the target
                if (inputDetail.CharaStatus.ListType == Constant.TargetType.SingleMaid)
                    target = StateManager.Instance.SelectedMaidsList[inputDetail.CharaStatus.ArrayPosition];
                else if (inputDetail.CharaStatus.ListType == Constant.TargetType.NPCFemale)
                    target = StateManager.Instance.NPCList[inputDetail.CharaStatus.ArrayPosition];

                return GetCharacterStatusField(target, inputDetail.CharaStatus.FieldName);
            }
            else if (inputDetail.SourceType == ADVStep.Evaluate.SourceType.FixedValue)
            {
                switch (inputDetail.FixedValue.FixedValueType)
                {
                    case Constant.VariableType.Integer:
                        return int.Parse(inputDetail.FixedValue.FixedValue);
                    case Constant.VariableType.FloatingPoint:
                        return double.Parse(inputDetail.FixedValue.FixedValue);
                    case Constant.VariableType.Boolean:
                        return bool.Parse(inputDetail.FixedValue.FixedValue);
                    default:
                        return inputDetail.FixedValue.FixedValue;
                }
            }
            else if (inputDetail.SourceType == ADVStep.Evaluate.SourceType.RandomNumber)
            {
                return RNG.Random.Next(inputDetail.RandomNumber.MaxValue);
            }

            return null;
        }

        private static object GetCharacterStatusField(Maid maid, string status_field)
        {
            switch (status_field)
            {
                case Constant.CharacterStatusField.Likability:
                    return maid.status.likability;
                case Constant.CharacterStatusField.Lovely:
                    return maid.status.lovely;
                case Constant.CharacterStatusField.Elegance:
                    return maid.status.elegance;
                case Constant.CharacterStatusField.Charm:
                    return maid.status.charm;
                case Constant.CharacterStatusField.Care:
                    return maid.status.care;
                case Constant.CharacterStatusField.Reception:
                    return maid.status.reception;
                case Constant.CharacterStatusField.Cooking:
                    return maid.status.cooking;
                case Constant.CharacterStatusField.Dance:
                    return maid.status.dance;
                case Constant.CharacterStatusField.Vocal:
                    return maid.status.vocal;
                case Constant.CharacterStatusField.NightWorkCount:
                    return maid.status.playCountNightWork;

                case Constant.CharacterStatusField.Inyoku:
                    return maid.status.inyoku;
                case Constant.CharacterStatusField.MValue:
                    return maid.status.mvalue;
                case Constant.CharacterStatusField.Hentai:
                    return maid.status.hentai;
                case Constant.CharacterStatusField.Houshi:
                    return maid.status.housi;
                case Constant.CharacterStatusField.YotogiCount:
                    return maid.status.playCountYotogi;

                case Constant.CharacterStatusField.HeroineType:
                    return maid.status.personal.id;
                case Constant.CharacterStatusField.SexExperienceVaginal:
                    return maid.status.seikeiken == MaidStatus.Seikeiken.Yes_No || maid.status.seikeiken == MaidStatus.Seikeiken.Yes_Yes;
                case Constant.CharacterStatusField.SexExperienceAnal:
                    return maid.status.seikeiken == MaidStatus.Seikeiken.No_Yes || maid.status.seikeiken == MaidStatus.Seikeiken.Yes_Yes;

                case Constant.CharacterStatusField.Height:
                    return maid.status.body.height;
                case Constant.CharacterStatusField.Weight:
                    return maid.status.body.weight;
                case Constant.CharacterStatusField.Bust:
                    return maid.status.body.bust;
                case Constant.CharacterStatusField.Waist:
                    return maid.status.body.waist;
                case Constant.CharacterStatusField.Hip:
                    return maid.status.body.hip;
                case Constant.CharacterStatusField.Cup:
                    return maid.status.body.cup;

                case Constant.CharacterStatusField.SexNumOfPeople:
                    return maid.status.sexPlayNumberOfPeople;
            }
            throw new EvaluateException("[CharacterStatusFieldError]: Unknown field name");
        }
        


        internal static void ProcessADVListUpdate(ADVKagManager instance, ADVStep step)
        {
            if (step.ListUpdateData != null)
            {
                if (step.ListUpdateData.Add != null)
                {
                    foreach (var addData in step.ListUpdateData.Add)
                    {
                        Maid maid = null;
                        List<Maid> targetList;
                        List<Maid> sourceList = null;
                        if (addData.Type == Constant.TargetType.ClubOwner)
                        {
                            maid = StateManager.Instance.ClubOwner;
                            targetList = StateManager.Instance.MenList;
                        }
                        else if (addData.Type == Constant.TargetType.SingleMan)
                        {
                            sourceList = StateManager.Instance.MenList;
                            targetList = StateManager.Instance.MenList;
                        }
                        else if (addData.Type == Constant.TargetType.NPCMale)
                        {
                            sourceList = StateManager.Instance.NPCManList;
                            targetList = StateManager.Instance.MenList;
                        }
                        else
                        {
                            sourceList = StateManager.Instance.NPCList;
                            targetList = StateManager.Instance.SelectedMaidsList;
                        }

                        if (sourceList != null)
                        {
                            if (sourceList.Count <= addData.SrcPosition)
                                continue;
                            maid = sourceList[addData.SrcPosition];
                        }

                        if (maid == null)
                            continue;

                        if (targetList.Contains(maid))
                            targetList.Remove(maid);
                        targetList.Insert(addData.PositionToInsert, maid);
                    }
                }

                if (step.ListUpdateData.Remove != null)
                {
                    foreach (var removeData in step.ListUpdateData.Remove)
                    {
                        Maid maid = null;
                        List<Maid> targetList;
                        List<Maid> sourceList = null;
                        if (removeData.Type == Constant.TargetType.ClubOwner)
                        {
                            maid = StateManager.Instance.ClubOwner;
                            targetList = StateManager.Instance.MenList;
                        }
                        else if (removeData.Type == Constant.TargetType.SingleMan)
                        {
                            sourceList = StateManager.Instance.MenList;
                            targetList = StateManager.Instance.MenList;
                        }
                        else if (removeData.Type == Constant.TargetType.NPCMale)
                        {
                            sourceList = StateManager.Instance.NPCManList;
                            targetList = StateManager.Instance.MenList;
                        }
                        else
                        {
                            sourceList = StateManager.Instance.NPCList;
                            targetList = StateManager.Instance.SelectedMaidsList;
                        }

                        if (sourceList != null)
                        {
                            if (sourceList.Count <= removeData.SrcPosition)
                                continue;
                            maid = sourceList[removeData.SrcPosition];
                        }

                        if (maid == null)
                            continue;

                        targetList.Remove(maid);
                    }
                }
            }
        }

        internal static void ProcessADVTimeWait(ADVKagManager instance, ADVStep step)
        {
            Double secondToWait = Double.Parse(step.Tag);
            if (Config.DebugIgnoreADVForceTimeWait)
                secondToWait = 0;

            StateManager.Instance.ADVResumeTime = DateTime.Now.AddSeconds(secondToWait);
        }


        internal static void ADVSceneProceedToNextStep(string nextStepID = "")
        {
            if (StateManager.Instance.UndergoingModEventID <= 0 || string.IsNullOrEmpty(StateManager.Instance.CurrentADVStepID))
                return;

            ResetLipSyncing();

            if (nextStepID == "")
                StateManager.Instance.CurrentADVStepID = StateManager.Instance.ScenarioSteps[StateManager.Instance.CurrentADVStepID].NextStepID;
            else
                StateManager.Instance.CurrentADVStepID = nextStepID;

            StateManager.Instance.WaitForUserClick = false;
            StateManager.Instance.WaitForUserInput = false;
            StateManager.Instance.WaitForCameraPanFinish = false;
        }

        private static void ResetLipSyncing()
        {
            foreach (var maid in StateManager.Instance.ForceLipSyncingList)
                maid.StopKuchipakuPattern();
            StateManager.Instance.ForceLipSyncingList.Clear();
        }



        private static ScriptManagerFast.KagTagSupportFast CreateChangeBackgroundTag(string bgname)
        {
            ScriptManagerFast.KagTagSupportFast tag = new ScriptManagerFast.KagTagSupportFast();
            tag.AddTagProperty("file", bgname);
            tag.AddTagProperty("tagname", "setbg");
            return tag;
        }

        private static void DisplayAdvText(ADVKagManager advMgr, string speaker_name, string text, string voice_file, int pitch, AudioSourceMgr.Type type)
        {
            ScriptManagerFast.KagTagSupportFast tag = new ScriptManagerFast.KagTagSupportFast();
            tag.AddTagProperty("tagname", "talk");

            advMgr.MessageWindowMgr.SetText(speaker_name, PrepareDialogueText(text), voice_file, pitch, type);
            advMgr.TagTalk(tag);
        }

        private static void CheckCameraPanFinish()
        {
            //Check if camera pan is finish. The checking is put here due to the player could have chosen "skip" in the scene and thus the camera pan motion is not triggered
            if (StateManager.Instance.WaitForCameraPanFinish)
            {
                if (StateManager.Instance.TargetCameraAfterAnimation != null)
                {
                    //check the camera value against the target set
                    if (Util.NearlyEquals(GameMain.Instance.MainCamera.GetAroundAngle(), StateManager.Instance.TargetCameraAfterAnimation.AroundAngle)
                        && Util.NearlyEquals(GameMain.Instance.MainCamera.GetDistance(), StateManager.Instance.TargetCameraAfterAnimation.Distance)
                        && Util.NearlyEquals(GameMain.Instance.MainCamera.GetTargetPos(), StateManager.Instance.TargetCameraAfterAnimation.TargetPosition))
                    {
                        ADVSceneProceedToNextStep();
                    }
                }
            }
        }

        private static void CheckTimeWaitFinish()
        {
            if ((DateTime.Now > StateManager.Instance.ADVResumeTime && StateManager.Instance.ADVResumeTime != DateTime.MinValue))
            {
                StateManager.Instance.ADVResumeTime = DateTime.MinValue;
                ADVSceneProceedToNextStep();
            }
        }

        private static void CheckSystemFadeOutFinish()
        {
            if (StateManager.Instance.WaitForSystemFadeOut)
            {
                if (GameMain.Instance.MainCamera.GetFadeState() == CameraMain.FadeState.Out)
                {
                    StateManager.Instance.WaitForSystemFadeOut = false;
                    ADVSceneProceedToNextStep();
                }
            }
        }

        private static void ADVMotionChangeComplete()
        {
            StateManager.Instance.WaitForMotionChange = false;
        }
    }
}
