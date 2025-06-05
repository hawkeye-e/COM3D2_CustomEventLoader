using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.CustomEventLoader.Plugin
{
    internal class PartyGroup
    {
        public Maid Maid1;
        public Maid Maid2;
        public Maid Man1 = null;
        public Maid Man2 = null;
        public Maid Man3 = null;
        public Vector3 GroupPosition = Vector3.zero;
        public Vector3 GroupOffsetVector = Vector3.zero;            //from CharaAllOfsetPosPre
        public Vector3 GroupOffsetVector2 = Vector3.zero;           //from TagAllPos
        public Vector3 GroupRotationOffset = Vector3.zero;
        public Quaternion GroupRotation = Quaternion.identity;

        

        public Dictionary<int, Vector3> MaidOffsetList = new Dictionary<int, Vector3>();
        public Dictionary<int, Vector3> ManOffsetList = new Dictionary<int, Vector3>();

        public bool RequireSmoothPositionChange = false;

        public bool ForceCharacterVisibleOnPositionChange = true;


        public PartyGroup() { }


        public int MaidCount
        {
            get
            {
                int count = 1;
                if (Maid2 != null) count++;
                return count;
            }
        }

        public int ManCount
        {
            get
            {
                int count = 0;
                if (Man1 != null) count++;
                if (Man2 != null) count++;
                if (Man3 != null) count++;
                return count;
            }
        }

        public string GroupType
        {
            get
            {
                if (MaidCount == 2 && ManCount == 1) return Constant.GroupType.FFM;
                else if (MaidCount == 1 && ManCount == 2) return Constant.GroupType.MMF;
                else if (MaidCount == 1 && ManCount == 3) return Constant.GroupType.MMMF;
                else if (MaidCount == 1 && ManCount == 1) return Constant.GroupType.MF;
                else if (MaidCount == 2 && ManCount == 0) return Constant.GroupType.FF;
                else
                    return "";
            }

        }

        public void SetGroupPosition(Vector3 pos, Quaternion rot)
        {
            GroupPosition = pos;
            GroupRotation = rot;
            SetGroupPosition();
        }

        public void SetGroupPosition()
        {
            SetCharacterPosition(Maid1, 0);
            SetCharacterPosition(Maid2, 1);
            SetCharacterPosition(Man1, 0);
            SetCharacterPosition(Man2, 1);
            SetCharacterPosition(Man3, 2);
        }

        public Maid GetMaidAtIndex(int index)
        {
            if (index == 0)
                return Maid1;
            else if (index == 1)
                return Maid2;
            return null;
        }

        public Maid GetManAtIndex(int index)
        {
            if (index == 0)
                return Man1;
            else if (index == 1)
                return Man2;
            else if (index == 2)
                return Man3;
            return null;
        }

        public void SetMaidAtIndex(int index, Maid maid)
        {
            if (index == 0)
                Maid1 = maid;
            else if (index == 1)
                Maid2 = maid;
        }
        public void SetManAtIndex(int index, Maid maid)
        {
            if (index == 0)
                Man1 = maid;
            else if (index == 1)
                Man2 = maid;
            else if (index == 2)
                Man3 = maid;
        }

        public void SetCharacterPosition(Maid maid, int indexPosition)
        {
            if (maid != null)
            {
                Vector3 individualOffset = GetIndividualOffset(maid.boMAN, indexPosition);

                Quaternion finalRotation = GroupRotation * Quaternion.Euler(Vector3.up * GroupRotationOffset.y);

                if (RequireSmoothPositionChange)
                {
                    Util.SmoothMoveMaidPosition(maid, GroupPosition + GroupOffsetVector + GroupOffsetVector2 + individualOffset, finalRotation);
                }
                else
                {
                    Util.StopSmoothMove(maid);
                    maid.transform.localPosition = Vector3.zero;
                    maid.transform.position = GroupPosition + GroupOffsetVector + GroupOffsetVector2 + individualOffset;
                    maid.transform.localRotation = new Quaternion(0, 0, 0, 0);
                    maid.transform.rotation = finalRotation;
                    maid.body0.SetBoneHitHeightY(GroupPosition.y);
                }

                if (ForceCharacterVisibleOnPositionChange)
                    maid.Visible = true;
            }
        }

        private Vector3 GetIndividualOffset(bool isMan, int indexPosition)
        {
            Vector3 individualOffset = Vector3.zero;
            Dictionary<int, Vector3> targetDict;
            if (isMan)
                targetDict = ManOffsetList;
            else
                targetDict = MaidOffsetList;

            if (targetDict.ContainsKey(indexPosition))
                individualOffset = targetDict[indexPosition];

            return individualOffset;
        }

        public void StopAudio()
        {
            Maid1.AudioMan.Stop();
            if (Maid2 != null)
                Maid2.AudioMan.Stop();
        }

        public void ReloadAnimation(bool IsSmooth = true)
        {
            ReloadAnimationForMaid(Maid1, IsSmooth);
            ReloadAnimationForMaid(Maid2, IsSmooth);
            ReloadAnimationForMaid(Man1, IsSmooth);
            ReloadAnimationForMaid(Man2, IsSmooth);
            ReloadAnimationForMaid(Man3, IsSmooth);
        }


        private void ReloadAnimationForMaid(Maid maid, bool IsSmooth)
        {
            if (maid != null)
            {
                if (!string.IsNullOrEmpty(maid.body0.LastAnimeFN))
                {
                    float blendTime = IsSmooth ? 0.5f : 0f;
                    maid.body0.CrossFade(maid.body0.LastAnimeFN, GameUty.FileSystem, additive: false, loop: true, boAddQue: false, blendTime);
                }
            }
        }

        public void DetachAllIK()
        {
            DetachAllIK(Maid1);
            DetachAllIK(Maid2);
            DetachAllIK(Man1);
            DetachAllIK(Man2);
            DetachAllIK(Man3);
        }

#if COM3D2_5
#if UNITY_2022_3
        private void DetachAllIK(Maid maid)
        {
            if (maid != null)
                maid.body0.fullBodyIK.AllIKDetach();
        }
#endif
#endif

#if COM3D2
        private void DetachAllIK(Maid maid)
        {
            if (maid != null)
                maid.AllIKDetach();
        }
#endif


        public void ProcAllProp()
        {
            ProcProp(Maid1);
            ProcProp(Maid2);
            ProcProp(Man1);
            ProcProp(Man2);
            ProcProp(Man3);
        }

        public static void ProcProp(Maid maid)
        {
            if (maid != null)
                maid.AllProcProp();
        }

        public int GetMaidOrManIndex(Maid maid)
        {
            int result = -1;
            if (maid.boMAN)
            {
                for (int i = 0; i < ManCount; i++)
                    if (GetManAtIndex(i).status.guid == maid.status.guid)
                        result = i;
            }
            else
            {
                for (int i = 0; i < MaidCount; i++)
                    if (GetMaidAtIndex(i).status.guid == maid.status.guid)
                        result = i;
            }
            return result;
        }


        //For debug use
        public override string ToString()
        {
            string output = "";
            if (Maid1 != null) output += "Maid1: " + Maid1.status.fullNameJpStyle + ", ";
            if (Maid2 != null) output += "Maid2: " + Maid2.status.fullNameJpStyle + ", ";
            if (Man1 != null) output += "Man1: " + Man1.status.fullNameJpStyle + ", ";
            if (Man2 != null) output += "Man2: " + Man2.status.fullNameJpStyle + ", ";
            if (Man3 != null) output += "Man3: " + Man3.status.fullNameJpStyle + ", ";
            return output;
        }



    }
}
