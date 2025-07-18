﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.CustomEventLoader.Plugin
{
    internal class IKAttachInfo
    {
        public string PosString;
        public string AttachType;
        public bool PullOff = false;

        public IKCharaInfo Source;
        public IKCharaInfo Target;

        public Vector3 Pos
        {
            get
            {
                var splitPos = PosString.Split(',');
                return new Vector3(float.Parse(splitPos[0].Trim(), CultureInfo.InvariantCulture), float.Parse(splitPos[1].Trim(), CultureInfo.InvariantCulture), float.Parse(splitPos[2].Trim(), CultureInfo.InvariantCulture));
            }
        }

        internal class IKCharaInfo
        {
            public int ArrayIndex;
            public GroupMemberType MemberType;
            public ArrayListType ListType;
            public string Bone;
        }

        internal enum GroupMemberType
        {
            Maid1,
            Maid2,
            Man1,
            Man2
        }

        internal enum ArrayListType
        {
            Maid,
            Man,
            Group,
            ADVMaid,
            ADVMan,
            ADVClubOwner
        }
    }


}
