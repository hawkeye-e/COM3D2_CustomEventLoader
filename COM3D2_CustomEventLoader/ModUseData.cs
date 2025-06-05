using System.Collections.Generic;
using System.Linq;

namespace COM3D2.CustomEventLoader.Plugin
{
    class ModUseData
    {
        
        //Key: body part type, Value: ManBodyPart
        public static Dictionary<string, ManBodyInfo> ManBodyInfoList;

        //Key: EffectID
        public static Dictionary<string, CharacterEffect> CharacterEffectList;

        //Key: Pattern ID
        public static Dictionary<string, SemenPattern> SemenPatternList;

        public ModUseData()
        {
        }

        //Read all the necessary data from resources files
        public static void Init()
        {
            ManBodyInfoList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, ManBodyInfo>>(ModResources.TextResource.RandomizeManSetting);

            CharacterEffectList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, CharacterEffect>>(ModResources.TextResource.CharacterEffect);

            SemenPatternList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, SemenPattern>>(ModResources.TextResource.SemenPattern);
            foreach (var kvp in SemenPatternList)
            {
                kvp.Value.PostInitDataProcess();
            }
        }

    }
}
