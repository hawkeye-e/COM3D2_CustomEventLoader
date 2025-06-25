using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;

namespace COM3D2.CustomEventLoader.Plugin.HooksAndPatches.DebugUse
{
    internal class Hooks
    {
        internal static string GUID = CustomEventLoader.GUID + ".DebugUse";

        /* B: Clothing; X: Coordinates; Z: dialogue files; C: Camera; V: Apply hardcoded motion 
        */
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ADVKagManager), nameof(ADVKagManager.Update))]
        private static void UpdatePost(ADVKagManager __instance)
        {
            Patches.PrintCameraInfo();
            Patches.PrintObjectInfo();
            Patches.PrintCharacterSetup();
        }

        //for logging down the motion file and label
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ScriptManager), nameof(ScriptManager.LoadMotionScript))]
        private static void LoadMotionScriptPre(ScriptManager __instance, int sloat, bool is_next, string file_name, string label_name, string maid_guid, string man_guid, bool face_fix, bool valid_pos, bool disable_diff_pos)
        {
            if (Config.DebugLogScriptInfo)
                CustomEventLoader.Log.LogInfo("[Load Motion Script] Script FileName: " + file_name + ", Script Label: " + label_name);
        }

        //Log the current clip name for each group playing
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TBody), nameof(TBody.LoadAnime), new Type[] { typeof(string), typeof(AFileSystemBase), typeof(string), typeof(bool), typeof(bool) })]
        private static void LoadAnime(TBody __instance, string tag, AFileSystemBase fileSystem, string filename, bool additive, bool loop)
        {
            if (Config.DebugLogAnimationInfo)
                CustomEventLoader.Log.LogInfo("[Load Animation] Maid: " + __instance?.maid?.status?.fullNameJpStyle + ", Filename: " + filename);
        }

        //log down the corresponding face anime for the motion
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Maid), nameof(Maid.FaceAnime))]
        private static void FaceAnimePre(Maid __instance, string tag, float t, int chkcode)
        {
            if (Config.DebugLogFaceAnimeInfo)
                CustomEventLoader.Log.LogInfo("[Load Face Anime] Maid: " + __instance?.status?.fullNameJpStyle + ", Tag: " + tag);
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(AudioSourceMgr), nameof(AudioSourceMgr.LoadPlay))]
        private static void LoadPlayPre(AudioSourceMgr __instance, string f_strFileName, float f_fFadeTime, bool f_bStreaming, bool f_bLoop)
        {
            if (Config.DebugLogAudioInfo)
            {
                for (int i = 0; i < GameMain.Instance.CharacterMgr.GetMaidCount(); i++)
                {
                    Maid maid = GameMain.Instance.CharacterMgr.GetMaid(i);
                    if (maid != null)
                    {
                        if (maid.AudioMan == __instance)
                        {
                            CustomEventLoader.Log.LogInfo("[Play Audio] Maid: " + Util.GetFullName(maid) + ", FileName: " + f_strFileName + ", Loop: " + f_bLoop);
                        }
                    }
                }
            }
            
                
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(BgMgr), nameof(BgMgr.ChangeBg))]
        private static void ChangeBg(string f_strPrefubName)
        {
            if (Config.DebugLogBackgroundInfo)
                CustomEventLoader.Log.LogInfo("[Load Background] Prefab Name: " + f_strPrefubName);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SoundMgr), nameof(SoundMgr.PlaySe))]
        private static void PlaySe(string f_strFileName, bool f_bLoop)
        {
            if (Config.DebugLogSEInfo)
                CustomEventLoader.Log.LogInfo("[Play SE] FileName: " + f_strFileName + ", Loop:" + f_bLoop);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BgMgr), nameof(BgMgr.AddPrefabToBg))]
        private static void AddPrefabToBg(string f_strSrc, string f_strName, string f_strDest, Vector3 f_vPos, Vector3 f_vRot)
        {
            CustomEventLoader.Log.LogInfo("AddPrefabToBg f_strSrc: " + f_strSrc
                + ", f_strName: " + f_strName
                + ", f_strDest: " + f_strDest
                + ", f_vPos: " + f_vPos
                + ", f_vRot: " + f_vRot
                );

        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(SoundMgr), nameof(SoundMgr.PlayBGM))]
        private static void PlayBGM(string f_strFileName, float f_fTime, bool f_fLoop)
        {
            if (Config.DebugLogBGMInfo)
                CustomEventLoader.Log.LogInfo("[Play BGM] FileName: " + f_strFileName);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SoundMgr), nameof(SoundMgr.PlayBGMLegacy))]
        private static void PlayBGMLegacy(string f_strFileName, float f_fTime, bool f_fLoop)
        {
            if (Config.DebugLogBGMInfo)
                CustomEventLoader.Log.LogInfo("[Play BGM Legacy] FileName: " + f_strFileName);
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(ObjectManagerWindow), nameof(ObjectManagerWindow.Start))]
        private static void ObjectManagerWindowStart(ObjectManagerWindow __instance)
        {
            StateManager.Instance.ObjectWindow = __instance;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PhotoManEditManager.Controller), nameof(PhotoManEditManager.Controller.SetMenu))]
        private static void SetMenu(SceneEdit.SMenuItem set_menu)
        {
            if (Config.DebugLogMaleBodyPartInfo)
                CustomEventLoader.Log.LogInfo("[Male Body Part] File Name: " + set_menu.m_strMenuFileName );
        }
    }
}
