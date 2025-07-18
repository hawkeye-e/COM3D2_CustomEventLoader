﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

namespace COM3D2.CustomEventLoader.Plugin
{
    internal class ModNPCFemale : ModNPC
    {
        public PresetFileDetail PresetFile;
        public int Personality = 80;
        public int VoicePitch;
    }

    internal class ModNPCMale : ModNPC
    {
        public string Head;
        public string Clothed;
        public string Nude;
        public int BodySize;        //Range: [0-100]
        public string HexColor;        //ARGB in hex

        public Color Color
        {
            get
            {
                return Util.ConvertHexColorToColor(HexColor);
            }
        }
    }

    internal class ModNPC
    {
        public string FirstName;
        public string LastName;
        public string NickName;
        public CallType WayToCall;

        public enum CallType
        {
            FirstName,
            LastName,
            NickName
        }
    }

    internal class PresetFileDetail
    {
        public string V2_5;
        public string V2;
    }
}
