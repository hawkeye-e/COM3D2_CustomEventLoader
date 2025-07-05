using BepInEx.Logging;
using HarmonyLib;
using RenderHeads.Media.AVProVideo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.CustomEventLoader.Plugin.Core
{
    internal class CharacterHandling
    {
        private static ManualLogSource Log = CustomEventLoader.Log;

        internal static void InitSelectedMaids()
        {
            //prepare the maids for the scene
            StateManager.Instance.SelectedMaidsList = new List<Maid>();

            foreach(var kvp in StateManager.Instance.CharacterSelectionMaidList.OrderBy(x => x.Key))
            {
                StateManager.Instance.SelectedMaidsList.Add(kvp.Value);
            }
            
            GameMain.Instance.CharacterMgr.SetActiveMaid(StateManager.Instance.SelectedMaidsList[0], 0);

            Maid mainMaid = GameMain.Instance.CharacterMgr.GetMaid(0);

            //Backup all maid clothes information. We will use this info to restore the clothing information when the mod event is reset
            foreach (Maid maid in StateManager.Instance.SelectedMaidsList)
            {
                //if (maid != mainMaid)
                InitMaid(maid);
                BackupMaidClothesInfo(maid);
            }

        }

        internal static void InitMaid(Maid maid)
        {
            //For maid, no need to create new. we take it from stock
            maid.gameObject.transform.SetParent(GameMain.Instance.CharacterMgr.GetMaid(0).gameObject.transform.parent, false);
            RenderMaidAfterInit(maid);
            StateManager.Instance.WaitForFullLoadList.Add(maid);
        }

        internal static Maid InitMan(int manSlot, List<string> manTypeKeyList)
        {
            Maid man;

            man = GameMain.Instance.CharacterMgr.AddStockMan();

            man.gameObject.transform.SetParent(GameMain.Instance.CharacterMgr.GetMaid(0).gameObject.transform.parent, false);
            man.gameObject.name = Constant.DefinedGameObjectNames.ModAddedManGameObjectPrefix + manSlot;

            RandomizeManBody(man, manTypeKeyList);

            RenderMaidAfterInit(man);

            StateManager.Instance.WaitForFullLoadList.Add(man);

            return man;
        }

        internal static Maid InitNPCMaid(string key)
        {
            Maid maid = GameMain.Instance.CharacterMgr.AddStockMaid();

            foreach (var kvp in CharacterMgr.npcDatas)
            {
                if (kvp.Key == key)
                {
                    kvp.Value.Apply(maid);
                    break;
                }
            }

            RenderMaidAfterInit(maid);
            StateManager.Instance.WaitForFullLoadList.Add(maid);

            return maid;
        }

        internal static Maid InitModNPCFemale(ModNPCFemale npcData)
        {
            Maid maid = GameMain.Instance.CharacterMgr.AddStockMaid();
#if COM3D2_5
#if UNITY_2022_3
            CharacterMgr.Preset preset = LoadPreset(npcData.PresetFile.V2_5);
#endif
#endif

#if COM3D2
            CharacterMgr.Preset preset = LoadPreset(npcData.PresetFile.V2);
#endif

            GameMain.Instance.CharacterMgr.PresetSet(maid, preset);

            if (maid != null)
            {
                //Doesnt matter for personality as there is no yotogi scene?
                maid.status.SetPersonal(npcData.Personality);
                maid.VoicePitch = npcData.VoicePitch;

                Traverse.Create(maid.status).Field(Constant.DefinedClassFieldNames.MaidStatusFirstName).SetValue(npcData.FirstName);
                Traverse.Create(maid.status).Field(Constant.DefinedClassFieldNames.MaidStatusLastName).SetValue(npcData.LastName);
                Traverse.Create(maid.status).Field(Constant.DefinedClassFieldNames.MaidStatusNickName).SetValue(npcData.NickName);

                maid.status.isNickNameCall = false;
                maid.status.isFirstNameCall = false;
                if (npcData.WayToCall == ModNPC.CallType.NickName)
                    maid.status.isNickNameCall = true;
                else if (npcData.WayToCall == ModNPC.CallType.FirstName)
                    maid.status.isFirstNameCall = true;

                RenderMaidAfterInit(maid);

                StateManager.Instance.WaitForFullLoadList.Add(maid);
            }

            return maid;
        }



        internal static CharacterMgr.Preset LoadPreset(string presetFileName)
        {
            var scnDef = Util.GetCurrentScenarioDefinition();
            byte[] presetData = ScenarioFileHandling.GetCustomEventFileContentInByteArray(scnDef.FilePath, presetFileName);

            BinaryReader binaryReader = new BinaryReader(new MemoryStream(presetData));
#if COM3D2_5
#if UNITY_2022_3
            CharacterMgr.Preset result = CharacterMgr.PresetLoad(binaryReader, Path.GetFileName(presetFileName));
#endif
#endif

#if COM3D2
            CharacterMgr.Preset result = GameMain.Instance.CharacterMgr.PresetLoad(binaryReader, Path.GetFileName(presetFileName));
#endif
            binaryReader.Close();

            return result;
        }

        internal static Maid InitModNPCMale(ModNPCMale npcData)
        {
            Maid man = GameMain.Instance.CharacterMgr.AddStockMan();

            SetManBody(man, npcData.Color, npcData.BodySize, npcData.Head, npcData.Clothed);

            Traverse.Create(man.status).Field(Constant.DefinedClassFieldNames.MaidStatusFirstName).SetValue(npcData.FirstName);
            Traverse.Create(man.status).Field(Constant.DefinedClassFieldNames.MaidStatusLastName).SetValue(npcData.LastName);
            Traverse.Create(man.status).Field(Constant.DefinedClassFieldNames.MaidStatusNickName).SetValue(npcData.NickName);

            man.status.isNickNameCall = false;
            man.status.isFirstNameCall = false;
            if (npcData.WayToCall == ModNPC.CallType.NickName)
                man.status.isNickNameCall = true;
            else if (npcData.WayToCall == ModNPC.CallType.FirstName)
                man.status.isFirstNameCall = true;

            RenderMaidAfterInit(man);

            StateManager.Instance.WaitForFullLoadList.Add(man);

            ManClothingInfo manClothingInfo = new ManClothingInfo();
            manClothingInfo.IsNude = false;
            manClothingInfo.Clothed = npcData.Clothed.Trim();
            manClothingInfo.Nude = npcData.Nude.Trim();
            StateManager.Instance.ManClothingList.Add(man.status.guid, manClothingInfo);


            return man;
        }

        internal static void RenderMaidAfterInit(Maid maid)
        {
            if (maid != null)
            {
                maid.gameObject.name = maid.status.fullNameJpStyle;
                maid.transform.localPosition = new Vector3(-999f, -999f, -999f);
                maid.Visible = true;
                maid.DutPropAll();
                maid.AllProcPropSeqStart();
            }
        }

        private static void RandomizeManBody(Maid man, List<string> manTypeKeyList)
        {
            string pickedType = manTypeKeyList[RNG.Random.Next(manTypeKeyList.Count)];
            ManBodyInfo pickedInfo = ModUseData.ManBodyInfoList[pickedType];
            ManBodyInfo.BodyInfo pickedBodyInfo = pickedInfo.Body[RNG.Random.Next(pickedInfo.Body.Count)];

            //Body color
            Color manColor = new Color(RNG.Random.Next(256) / 256f, RNG.Random.Next(256) / 256f, RNG.Random.Next(256) / 256f);

            //How slim or fat
            int hara = pickedBodyInfo.Min + RNG.Random.Next(pickedBodyInfo.Max - pickedBodyInfo.Min);


            //head
            int head = RNG.Random.Next(pickedInfo.Head.Count);
            string strHeadFileName = pickedInfo.Head[head].Trim();

            //body
            string strBodyFileName = pickedBodyInfo.Clothed.Trim();

            SetManBody(man, manColor, hara, strHeadFileName, strBodyFileName);

            //put the info into the dictionary
            ManClothingInfo manClothingInfo = new ManClothingInfo();
            manClothingInfo.IsNude = false;
            manClothingInfo.Clothed = pickedBodyInfo.Clothed.Trim();
            manClothingInfo.Nude = pickedBodyInfo.Nude.Trim();
            StateManager.Instance.ManClothingList.Add(man.status.guid, manClothingInfo);
        }

        private static void SetManBody(Maid man, Color manColor, int bodySize, string headFile, string bodyFile)
        {
            //Body color
            man.ManColor = manColor;

            //How slim or fat
            man.SetProp(MPN.Hara, bodySize);

            int ridHead = Path.GetFileName(headFile).ToLower().GetHashCode();
            man.SetProp(MPN.head, headFile, ridHead);

            int ridBody = Path.GetFileName(bodyFile).ToLower().GetHashCode();
            man.SetProp(MPN.body, bodyFile, ridBody);
        }

        internal static void StopAllMaidSound()
        {
            foreach (Maid maid in StateManager.Instance.SelectedMaidsList)
                StopMaidSound(maid);
            if (StateManager.Instance.NPCList != null)
                foreach (Maid npc in StateManager.Instance.NPCList)
                    StopMaidSound(npc);
        }

        internal static void StopMaidSound(Maid maid)
        {
            if (maid != null)
            {
                if (maid.AudioMan != null)
                {
                    maid.AudioMan.standAloneVoice = false;
                    maid.AudioMan.Stop();
                }
            }
        }

        internal static void SetFemaleClothing(Maid maid, string clothesSetID)
        {
            if (maid == null || string.IsNullOrEmpty(clothesSetID))
                return;

            if (clothesSetID == Constant.ClothesSetResetCode)
            {
                RestoreMaidClothesInfo(maid);
            }
            else
            {
                if (StateManager.Instance.ClothesSetList.ContainsKey(clothesSetID))
                {
                    foreach(var kvp in StateManager.Instance.ClothesSetList[clothesSetID])
                    {
                        string slotName = kvp.Key;
                        string fileName = kvp.Value;

                        maid.ResetProp(slotName, true);
                        maid.AllProcProp();
                        maid.SetProp(slotName, fileName, 0, false);
                        maid.AllProcProp();
                    }
                }

            }
        }

        internal static void RestoreMaidClothesInfo(Maid maid)
        {
            if (maid == null)
                return;
            if (!StateManager.Instance.BackupMaidClothingList.ContainsKey(maid.status.guid))
                return;

            Dictionary<string, string> maidClothesDict = StateManager.Instance.BackupMaidClothingList[maid.status.guid];
            foreach (var kvp in maidClothesDict)
            {
                maid.SetProp(kvp.Key, kvp.Value, 0, false);
            }
            maid.AllProcProp();
        }

        private static void BackupMaidClothesInfo(Maid maid)
        {
            Dictionary<string, string> maidClothesDict = new Dictionary<string, string>();

            for (int i = 0; i < Constant.DressingClothingTagArray.Length; i++)
            {
                maidClothesDict.Add(Constant.DressingClothingTagArray[i], maid.GetProp(Constant.DressingClothingTagArray[i]).strFileName);
            }

            if (!StateManager.Instance.BackupMaidClothingList.ContainsKey(maid.status.guid))
                StateManager.Instance.BackupMaidClothingList.Add(maid.status.guid, maidClothesDict);
            else
                StateManager.Instance.BackupMaidClothingList[maid.status.guid] = maidClothesDict;
        }

        internal static void BackupManOrder()
        {
            for (int i = 0; i < GameMain.Instance.CharacterMgr.GetManCount(); i++)
            {
                Maid tmp = GameMain.Instance.CharacterMgr.GetMan(i);
                if (tmp != null)
                {
                    StateManager.Instance.OriginalManOrderList.Add(tmp);
                }
            }
        }

        internal static void UpdateCharacterEffect(Maid maid, List<string> activeEffectList)
        {
            if (maid == null)
                return;
            if (activeEffectList == null)
                return;

            if (!StateManager.Instance.ActiveEffectList.ContainsKey(maid))
                StateManager.Instance.ActiveEffectList.Add(maid, new List<string>());

            //Compare the effect list with the ActiveEffectList
            List<string> toBeRemoved = StateManager.Instance.ActiveEffectList[maid].Except(activeEffectList).ToList();
            List<string> toBeAdded = activeEffectList.Except(StateManager.Instance.ActiveEffectList[maid]).ToList();

            RemoveCharacterEffect(maid, toBeRemoved);
            AddCharacterEffect(maid, toBeAdded);

            //Update the List in memory
            foreach (var effect in toBeRemoved)
                StateManager.Instance.ActiveEffectList[maid].Remove(effect);
            foreach (var effect in toBeAdded)
                StateManager.Instance.ActiveEffectList[maid].Add(effect);
        }

        private static void AddCharacterEffect(Maid maid, List<string> effectList)
        {
            if (maid == null)
                return;
            if (effectList == null)
                return;

            foreach (string effectID in effectList)
            {
                if (!ModUseData.CharacterEffectList.ContainsKey(effectID))
                    continue;
                
                CharacterEffect effect = ModUseData.CharacterEffectList[effectID];
                maid.AddPrefab(effect.Prefab, effect.Name, effect.TargetBone, effect.Offset.Pos, effect.Offset.Rot);
            }
        }

        private static void RemoveCharacterEffect(Maid maid, List<string> effectList)
        {
            if (maid == null)
                return;
            if (effectList == null)
                return;

            foreach (string effectID in effectList)
            {
                if (!ModUseData.CharacterEffectList.ContainsKey(effectID))
                    continue;
                
                CharacterEffect effect = ModUseData.CharacterEffectList[effectID];
                maid.DelPrefab(effect.Name);
            }
        }

        internal static void SetManClothing(Maid man, bool isNude)
        {
            if (man == null)
                return;
            if (!man.boMAN)
                return;

            //Penis
            man.body0.SetChinkoVisible(isNude);

            //Clothes
            if (StateManager.Instance.ManClothingList.ContainsKey(man.status.guid))
            {

                if (StateManager.Instance.ManClothingList[man.status.guid].IsNude == isNude)
                    return;
                else
                {
                    string fileName;
                    if (isNude)
                        fileName = StateManager.Instance.ManClothingList[man.status.guid].Nude;
                    else
                        fileName = StateManager.Instance.ManClothingList[man.status.guid].Clothed;

                    int ridBody = Path.GetFileName(fileName).ToLower().GetHashCode();
                    man.SetProp(MPN.body, fileName, ridBody);
                    man.AllProcProp();
                }
            }
        }

        internal static void ApplyMotionInfoToCharacter(Maid maid, MotionInfo motionInfo)
        {
            if (motionInfo == null || maid == null)
                return;

            if (!string.IsNullOrEmpty(motionInfo.CustomMotionFile))
            {
                byte[] customAnimationContent = StateManager.Instance.CustomAnimationList[motionInfo.CustomMotionFile];
                PlayCustomAnimation(maid, customAnimationContent, motionInfo.MotionTag, motionInfo.IsLoopMotion, motionInfo.IsBlend, motionInfo.IsQueued);
            }
            else
            {

                //if (!string.IsNullOrEmpty(motionInfo.RandomMotion))
                //{
                //    //Replace the variable if random motion is set
                //    motionInfo = RandomList.Motion.GetRandomMotionByCode(motionInfo.RandomMotion, maid.boMAN);
                //}

                if (!string.IsNullOrEmpty(motionInfo.ScriptFile))
                {
                    string maidGUID = "";
                    string manGUID = "";

                    if (maid.boMAN)
                        manGUID = maid.status.guid;
                    else
                        maidGUID = maid.status.guid;

                    LoadMotionScript(0, false, motionInfo.ScriptFile, motionInfo.ScriptLabel, maidGUID, manGUID, false, false, false, false);
                }
                else
                {
                    PlayAnimation(maid, motionInfo.MotionFile, motionInfo.MotionTag, motionInfo.IsLoopMotion, motionInfo.IsBlend, motionInfo.IsQueued);
                }
            }
        }

        internal static void PlayAnimation(Maid maid, string fileName, string tag, bool isLoop = true, bool isBlend = false, bool isQueued = false)
        {
            if (maid == null)
                return;

            float fade = 0f;
            if (isBlend)
                fade = ConfigurableValue.AnimationBlendTime;
            else
                maid.body0.StopAnime();

            maid.body0.LoadAnime(tag, GameUty.FileSystem, fileName, false, isLoop);
            maid.body0.CrossFade(maid.body0.LastAnimeFN, GameUty.FileSystem, additive: false, loop: isLoop, boAddQue: isQueued, fade: fade);
        }

        internal static void PlayCustomAnimation(Maid maid, byte[] animContent, string tag, bool isLoop = true, bool isBlend = false, bool isQueued = false)
        {
            if (maid == null)
                return;

            float fade = 0f;
            if (isBlend)
                fade = ConfigurableValue.AnimationBlendTime;
            else
                maid.body0.StopAnime();

            maid.body0.LoadAnime(tag, animContent, false, isLoop);
            maid.body0.CrossFade(maid.body0.LastAnimeFN, animContent, additive: false, loop: isLoop, boAddQue: isQueued, fade: fade);
        }

        //function definition copied from KISS code. All load motion script call from the mod should call here so that there is no need to handle the V2 compatible version everywhere
        internal static void LoadMotionScript(int sloat, bool is_next, string file_name, string label_name = "", string maid_guid = "", string man_guid = "", bool face_fix = false, bool valid_pos = true, bool disable_diff_pos = false, bool body_mix_ok = false)
        {
#if COM3D2_5
#if UNITY_2022_3
            GameMain.Instance.ScriptMgr.LoadMotionScript(sloat, is_next, file_name, label_name, maid_guid, man_guid, face_fix, valid_pos, disable_diff_pos, body_mix_ok);
#endif
#endif

#if COM3D2
            GameMain.Instance.ScriptMgr.LoadMotionScript(sloat, is_next, file_name, label_name, maid_guid, man_guid, face_fix, valid_pos, disable_diff_pos);
#endif
        }

        internal static void HandleExtraObject(Maid maid, ExtraItemObject item)
        {
            if (maid == null) 
                return;
            if (item == null)
                return;

            if (!string.IsNullOrEmpty(item.ItemFile))
                AttachObjectToCharacter(maid, item);
            else
                RemoveObjectFromCharacter(maid, item.Target);
        }

        internal static void AttachObjectToCharacter(Maid maid, ExtraItemObject item)
        {
            if (maid == null)
                return;

            if (item != null)
            {
                //the objects dont show up if it is not reset first...
                maid.ResetProp(item.Target, true);
                maid.AllProcProp();

                maid.SetProp(item.Target, item.ItemFile, 0, item.IsTemp);
                maid.AllProcProp();
            }
        }

        internal static void RemoveObjectFromCharacter(Maid maid, string positionToRemove)
        {
            if (positionToRemove != null)
            {
                maid.ResetProp(positionToRemove, true);
                maid.AllProcProp();
            }
        }

        /*
         * Due to TagTexMulAdd will call GetMaidAndMan function which will mess up the spoofing logic for other parts, I dont directly call TagTexMulAdd but follow the logic inside.
           Drawback: There are some special handlings on CRC body in TagTexMulAdd, so the effect wont apply in 3.0 version(I have no idea whether this mod will work or not in v3.0 anyway)
         */
        internal static void AddTexture(Maid maid, TexturePattern pattern)
        {
            if (maid == null)
                return;

            List<string> slotToBeProc = new List<string>();

            for (int i = 0; i < pattern.SplashCount; i++)
            {
                int xValue = RNG.Random.Next(pattern.XRange[i].MinValue, pattern.XRange[i].MaxValue);
                int yValue = RNG.Random.Next(pattern.YRange[i].MinValue, pattern.YRange[i].MaxValue);
                float rotValue = RNG.Random.Next((int)(pattern.RotRange[i].MinValue * 100), (int)(pattern.RotRange[i].MaxValue * 100)) / 100.0f;
                float scaleValue = RNG.Random.Next((int)(pattern.Scale[i].MinValue * 100), (int)(pattern.Scale[i].MaxValue * 100)) / 100.0f;

                int rnd = RNG.Random.Next(pattern.FileName[i].Count);
                string fileName = pattern.FileName[i][rnd];

                foreach (var propName in pattern.PropName)
                {
                    maid.body0.MulTexSet(pattern.Slotname, pattern.MatNo, propName, pattern.LayerNo, fileName, pattern.BlendMode, pattern.Add,
                        xValue, yValue, rotValue, scaleValue, pattern.NoTransform, pattern.SubProp, pattern.Alpha, pattern.TargetBodyTexSize);
                }
                if (!slotToBeProc.Contains(pattern.Slotname))
                    slotToBeProc.Add(pattern.Slotname);
            }

            foreach (var slot in slotToBeProc)
                maid.body0.MulTexProc(slot);
        }

        internal static void RemoveTexture(Maid maid, int layer)
        {
            if (maid != null)
            {
                maid.body0.MulTexRemove(TexturePattern.SlotType.Body, TexturePattern.MaterialType.Body, TexturePattern.PropType.MainTexture, layer);
                maid.body0.MulTexRemove(TexturePattern.SlotType.Body, TexturePattern.MaterialType.Body, TexturePattern.PropType.ShadowTexture, layer);
                maid.body0.MulTexRemove(TexturePattern.SlotType.Head, TexturePattern.MaterialType.Head, TexturePattern.PropType.MainTexture, layer);
                maid.body0.MulTexRemove(TexturePattern.SlotType.Head, TexturePattern.MaterialType.Head, TexturePattern.PropType.ShadowTexture, layer);
                maid.body0.MulTexProc(TexturePattern.SlotType.Body);
                maid.body0.MulTexProc(TexturePattern.SlotType.Head);
            }
        }

    }
}
