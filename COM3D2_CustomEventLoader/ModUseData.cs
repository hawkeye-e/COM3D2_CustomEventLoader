using System.Collections;
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

        //Key: Constant.TextureType, Inner Key: PatternID
        public static Dictionary<string, Dictionary<string, TexturePattern>> TexturePatternList;

        public ModUseData()
        {
        }

        //Read all the necessary data from resources files
        public static void Init()
        {
            ManBodyInfoList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, ManBodyInfo>>(ModResources.TextResource.RandomizeManSetting);

            CharacterEffectList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, CharacterEffect>>(ModResources.TextResource.CharacterEffect);

            var semenPatternList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, TexturePattern>>(ModResources.TextResource.SemenPattern);
            foreach (var kvp in semenPatternList)
                kvp.Value.PostInitDataProcess();

            var whipMarkPatternList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, TexturePattern>>(ModResources.TextResource.WhipMarkPattern);
            foreach (var kvp in whipMarkPatternList)
                kvp.Value.PostInitDataProcess();

            var candlePatternList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, TexturePattern>>(ModResources.TextResource.CandlePattern);
            foreach (var kvp in candlePatternList)
                kvp.Value.PostInitDataProcess();

            var slapMarkPatternList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, TexturePattern>>(ModResources.TextResource.SlapMarkPattern);
            foreach (var kvp in slapMarkPatternList)
                kvp.Value.PostInitDataProcess();

            var lotionPatternList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, TexturePattern>>(ModResources.TextResource.LotionPattern);
            foreach (var kvp in lotionPatternList)
                kvp.Value.PostInitDataProcess();

            TexturePatternList = new Dictionary<string, Dictionary<string, TexturePattern>>();
            TexturePatternList.Add(Constant.TextureType.Semen, semenPatternList);
            TexturePatternList.Add(Constant.TextureType.WhipMark, whipMarkPatternList);
            TexturePatternList.Add(Constant.TextureType.Candle, candlePatternList);
            TexturePatternList.Add(Constant.TextureType.SlapMark, slapMarkPatternList);
            TexturePatternList.Add(Constant.TextureType.Lotion, lotionPatternList);
        }

    }
}
