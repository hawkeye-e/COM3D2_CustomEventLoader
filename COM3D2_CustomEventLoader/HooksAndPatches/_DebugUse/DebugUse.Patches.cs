using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;


namespace COM3D2.CustomEventLoader.Plugin.HooksAndPatches.DebugUse
{
    internal class Patches
    {
        internal static void PrintCameraInfo()
        {
            if (Input.GetKeyDown(Config.DeveloperModeCameraKey.MainKey))
            {
                //for printing the current camera setup information
                Vector3 pos = GameMain.Instance.MainCamera.GetPos();
                Vector3 targetPos = GameMain.Instance.MainCamera.GetTargetPos();
                Vector2 aroundAngle = GameMain.Instance.MainCamera.GetAroundAngle();
                float distance = GameMain.Instance.MainCamera.GetDistance();

                CustomEventLoader.Log.LogInfo("[Camera Data] Camera Pos: " + pos);
                CustomEventLoader.Log.LogInfo("[Camera Data] Camera target Pos: " + targetPos);
                CustomEventLoader.Log.LogInfo("[Camera Data] Camera angle: " + aroundAngle);
                CustomEventLoader.Log.LogInfo("[Camera Data] Camera distance: " + GameMain.Instance.MainCamera.GetDistance());
                
                
                CustomEventLoader.Log.LogInfo("[Camera Data][For Camera Data Parser]: " 
                    + "( "
                    + pos.x.ToString("0.000") + ", " + pos.y.ToString("0.000") + ", " + pos.z.ToString("0.000") + "|"
                    + targetPos.x.ToString("0.000") + ", " + targetPos.y.ToString("0.000") + ", " + targetPos.z.ToString("0.000") + "|"
                    + aroundAngle.x.ToString("0.000") + ", " + aroundAngle.y.ToString("0.000") + "|"
                    + distance
                    + " )");
            }
        }

        internal static void PrintObjectInfo()
        {
            
            if (Input.GetKeyDown(Config.DeveloperModeWorldObjectDataKey.MainKey))
            {
                if (StateManager.Instance.ObjectWindow != null)
                {
                    Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();
                    StateManager.Instance.ObjectWindow.createBgObjectWindow.OnSerializeEvent(ref data);

                    CustomEventLoader.Log.LogInfo("================= ");
                    foreach (var kvp in data)
                    {
                        var objData = PhotoBGObjectData.data.Where(x => x.id.ToString() == kvp.Value["id"]).First();
                        var srcName = string.IsNullOrEmpty(objData.create_prefab_name) ? objData.create_asset_bundle_name : objData.create_prefab_name;
                        Vector3 pos = Util.ParseVector3RawString(kvp.Value["position"].Trim('(').Trim(')'));
                        Quaternion rot = Util.ParseQuaternionRawString(kvp.Value["rotation"].Trim('(').Trim(')'));
                        Vector3 scale = Util.ParseVector3RawString(kvp.Value["scale"].Trim('(').Trim(')'));

                        CustomEventLoader.Log.LogInfo("[World Object] Object Name : " + objData.name);
                        CustomEventLoader.Log.LogInfo("[World Object] Source Name : " + srcName);
                        CustomEventLoader.Log.LogInfo("[World Object] Position : ( " + pos.x.ToString("0.000") + ", " + pos.y.ToString("0.000") + ", " + pos.z.ToString("0.000") + " )");
                        CustomEventLoader.Log.LogInfo("[World Object] Rotation : ( " + rot.x.ToString("0.000") + ", " + rot.y.ToString("0.000") + ", " + rot.z.ToString("0.000") + ", " + rot.w.ToString("0.000") +  " )");
                        CustomEventLoader.Log.LogInfo("[World Object] Scale : " + scale.x.ToString("0.000"));

                        CustomEventLoader.Log.LogInfo("------------------ ");

                    }
                    
                    CustomEventLoader.Log.LogInfo("================= ");
                    
                }

            }
        }

        internal static void PrintCharacterSetup()
        {
            if (Input.GetKeyDown(Config.DeveloperModeCharaPlacementDataKey.MainKey))
            {
                //For printing the coorindate information for editor use

                for (int i = 0; i < GameMain.Instance.CharacterMgr.GetMaidCount(); i++)
                {
                    Maid maid = GameMain.Instance.CharacterMgr.GetMaid(i);
                    if (maid != null && maid.Visible)
                    {
                        CustomEventLoader.Log.LogInfo("[Chara Info] Name: " + Util.GetFullName(maid));
                        CustomEventLoader.Log.LogInfo("[Chara Info] Position: ( " 
                            + maid.transform.position.x.ToString("0.000") + ", "
                            + maid.transform.position.y.ToString("0.000") + ", "
                            + maid.transform.position.z.ToString("0.000") + " )"
                            );
                        CustomEventLoader.Log.LogInfo("[Chara Info] Rotation: ( "
                            + maid.transform.rotation.x.ToString("0.000") + ", "
                            + maid.transform.rotation.y.ToString("0.000") + ", "
                            + maid.transform.rotation.z.ToString("0.000") + ", "
                            + maid.transform.rotation.w.ToString("0.000") + " )"
                            );
                    }
                }

                for (int i = 0; i < GameMain.Instance.CharacterMgr.GetManCount(); i++)
                {
                    Maid maid = GameMain.Instance.CharacterMgr.GetMan(i);
                    if (maid != null && maid.Visible)
                    {
                        CustomEventLoader.Log.LogInfo("[Chara Info] Name: " + Util.GetFullName(maid));
                        CustomEventLoader.Log.LogInfo("[Chara Info] Position: ( "
                            + maid.transform.position.x.ToString("0.000") + ", "
                            + maid.transform.position.y.ToString("0.000") + ", "
                            + maid.transform.position.z.ToString("0.000") + " )"
                            );
                        CustomEventLoader.Log.LogInfo("[Chara Info] Rotation: ( "
                            + maid.transform.rotation.x.ToString("0.000") + ", "
                            + maid.transform.rotation.y.ToString("0.000") + ", "
                            + maid.transform.rotation.z.ToString("0.000") + ", "
                            + maid.transform.rotation.w.ToString("0.000") + " )"
                            );
                    }
                }
            }else if (Input.GetKeyDown(Config.DeveloperModeClothesSetDataKey.MainKey))
            {
                Maid maid = GameMain.Instance.CharacterMgr.GetMaid(0);

                if (maid == null)
                    return;

                CustomEventLoader.Log.LogInfo("==============================");

                Type type = typeof(Constant.ClothingTag);
                Dictionary<string, string> dictClothesSet = new Dictionary<string, string>();
                foreach (var p in type.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic))
                {
                    var tag = p.GetValue(null).ToString();
                    var mp = maid.GetProp(tag);
                    CustomEventLoader.Log.LogInfo("[Clothes Set Info] " + Constant.ClothesPartMapping[tag] + ": " + mp.strFileName);
                    dictClothesSet.Add(tag, mp.strFileName);
                }

                string serializeString = Newtonsoft.Json.JsonConvert.SerializeObject(dictClothesSet);
                CustomEventLoader.Log.LogInfo("------------------------------");
                CustomEventLoader.Log.LogInfo("[Clothes Set Info] For Parser: " + serializeString);
                CustomEventLoader.Log.LogInfo("==============================");
            }
        }

    }
}
