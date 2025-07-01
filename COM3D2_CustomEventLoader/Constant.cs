using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM3D2.CustomEventLoader.Plugin
{
    internal class Constant
    {
        public const string LipSyncPattern = "AAAVAAAmAAArAAArAAAoAAAmAAAtAAA3AABVAABWAABcAABZAABRAAAlABMTADkIAFMCADwAADwAADwAABAAAAQAIwIGMQADNwABPAAAOwAANQAANwAANAAANAAANgAAMgAAMQAALgAALAAAKgAAJwAAJQAAJAAAIwAAIQAAMgAAMQAALQAAMAAALgAALQAALgAALAAALQAALQAAKQAAJwAAJQAAIwAAIQAAHwAAHwAAHQAAHAAAGwAAGQAAFwAAFgAAFgAACgAAAgAAAQEjAAA9AABCAABCAAA7AAAUAAALAAsLACgFADADADcAADQAAC8AAC8AAC8AFQsLJgAFJwAAMgAASAAATgAAPQAANQAAMgAAMAAALQAAMwAALQAAFQQEBQMXAgIkAAAxAAAxAAAzAAAyAAAyAAAxAAAxAAAuAAAxAAAuAAAtAAAsAAAsAAAkAAAdAAAVAAAmAAArAAArAAAoAAAmAAAtAAA3AABVAABWAABcAABZAABRAAAlABMTADkIAFMCADwAADwAADwAABAAAAQAIwIGMQADNwABPAAAOwAANQAANwAANAAANAAANgAAMgAAMQAALgAALAAAKgAAJwAAJQAAJAAAIwAAIQAAMgAAMQAALQAAMAAALgAALQAALgAALAAALQAALQAAKQAAJwAAJQAAIwAAIQAAHwAAHwAAHQAAHAAAGwAAGQAAFwAAFgAAFgAACgAAAgAAAQEjAAA9AABCAABCAAA7AAAUAAALAAsLACgFADADADcAADQAAC8AAC8AAC8AFQsLJgAFJwAAMgAASAAATgAAPQAANQAAMgAAMAAALQAAMwAALQAAFQQEBQMXAgIkAAAxAAAxAAAzAAAyAAAyAAAxAAAxAAAuAAAxAAAuAAAtAAAsAAAsAAAkAAAdAAAVAAAmAAArAAArAAAoAAAmAAAtAAA3AABVAABWAABcAABZAABRAAAlABMTADkIAFMCADwAADwAADwAABAAAAQAIwIGMQADNwABPAAAOwAANQAANwAANAAANAAANgAAMgAAMQAALgAALAAAKgAAJwAAJQAAJAAAIwAAIQAAMgAAMQAALQAAMAAALgAALQAALgAALAAALQAALQAAKQAAJwAAJQAAIwAAIQAAHwAAHwAAHQAAHAAAGwAAGQAAFwAAFgAAFgAACgAAAgAAAQEjAAA9AABCAABCAAA7AAAUAAALAAsLACgFADADADcAADQAAC8AAC8AAC8AFQsLJgAFJwAAMgAASAAATgAAPQAANQAAMgAAMAAALQAAMwAALQAAFQQEBQMXAgIkAAAxAAAxAAAzAAAyAAAyAAAxAAAxAAAuAAAxAAAuAAAtAAAsAAAsAAAkAAAdAAAVAAAmAAArAAArAAAoAAAmAAAtAAA3AABVAABWAABcAABZAABRAAAlABMTADkIAFMCADwAADwAADwAABAAAAQAIwIGMQADNwABPAAAOwAANQAANwAANAAANAAANgAAMgAAMQAALgAALAAAKgAAJwAAJQAAJAAAIwAAIQAAMgAAMQAALQAAMAAALgAALQAALgAALAAALQAALQAAKQAAJwAAJQAAIwAAIQAAHwAAHwAAHQAAHAAAGwAAGQAAFwAAFgAAFgAACgAAAgAAAQEjAAA9AABCAABCAAA7AAAUAAALAAsLACgFADADADcAADQAAC8AAC8AAC8AFQsLJgAFJwAAMgAASAAATgAAPQAANQAAMgAAMAAALQAAMwAALQAAFQQEBQMXAgIkAAAxAAAxAAAzAAAyAAAyAAAxAAAxAAAuAAAxAAAuAAAtAAAsAAAsAAAkAAAdAAAVAAAmAAArAAArAAAoAAAmAAAtAAA3AABVAABWAABcAABZAABRAAAlABMTADkIAFMCADwAADwAADwAABAAAAQAIwIGMQADNwABPAAAOwAANQAANwAANAAANAAANgAAMgAAMQAALgAALAAAKgAAJwAAJQAAJAAAIwAAIQAAMgAAMQAALQAAMAAALgAALQAALgAALAAALQAALQAAKQAAJwAAJQAAIwAAIQAAHwAAHwAAHQAAHAAAGwAAGQAAFwAAFgAAFgAACgAAAgAAAQEjAAA9AABCAABCAAA7AAAUAAALAAsLACgFADADADcAADQAAC8AAC8AAC8AFQsLJgAFJwAAMgAASAAATgAAPQAANQAAMgAAMAAALQAAMwAALQAAFQQEBQMXAgIkAAAxAAAxAAAzAAAyAAAyAAAxAAAxAAAuAAAxAAAuAAAtAAAsAAAsAAAkAAAdAAAVAAAmAAArAAArAAAoAAAmAAAtAAA3AABVAABWAABcAABZAABRAAAlABMTADkIAFMCADwAADwAADwAABAAAAQAIwIGMQADNwABPAAAOwAANQAANwAANAAANAAANgAAMgAAMQAALgAALAAAKgAAJwAAJQAAJAAAIwAAIQAAMgAAMQAALQAAMAAALgAALQAALgAALAAALQAALQAAKQAAJwAAJQAAIwAAIQAAHwAAHwAAHQAAHAAAGwAAGQAAFwAAFgAAFgAACgAAAgAAAQEjAAA9AABCAABCAAA7AAAUAAALAAsLACgFADADADcAADQAAC8AAC8AAC8AFQsLJgAFJwAAMgAASAAATgAAPQAANQAAMgAAMAAALQAAMwAALQAAFQQEBQMXAgIkAAAxAAAxAAAzAAAyAAAyAAAxAAAxAAAuAAAxAAAuAAAtAAAsAAAsAAAkAAAdAAAVAAAmAAArAAArAAAoAAAmAAAtAAA3AABVAABWAABcAABZAABRAAAlABMTADkIAFMCADwAADwAADwAABAAAAQAIwIGMQADNwABPAAAOwAANQAANwAANAAANAAANgAAMgAAMQAALgAALAAAKgAAJwAAJQAAJAAAIwAAIQAAMgAAMQAALQAAMAAALgAALQAALgAALAAALQAALQAAKQAAJwAAJQAAIwAAIQAAHwAAHwAAHQAAHAAAGwAAGQAAFwAAFgAAFgAACgAAAgAAAQEjAAA9AABCAABCAAA7AAAUAAALAAsLACgFADADADcAADQAAC8AAC8AAC8AFQsLJgAFJwAAMgAASAAATgAAPQAANQAAMgAAMAAALQAAMwAALQAAFQQEBQMXAgIkAAAxAAAxAAAzAAAyAAAyAAAxAAAxAAAuAAAxAAAuAAAtAAAsAAAsAAAkAAAdAAAVAAAmAAArAAArAAAoAAAmAAAtAAA3AABVAABWAABcAABZAABRAAAlABMTADkIAFMCADwAADwAADwAABAAAAQAIwIGMQADNwABPAAAOwAANQAANwAANAAANAAANgAAMgAAMQAALgAALAAAKgAAJwAAJQAAJAAAIwAAIQAAMgAAMQAALQAAMAAALgAALQAALgAALAAALQAALQAAKQAAJwAAJQAAIwAAIQAAHwAAHwAAHQAAHAAAGwAAGQAAFwAAFgAAFgAACgAAAgAAAQEjAAA9AABCAABCAAA7AAAUAAALAAsLACgFADADADcAADQAAC8AAC8AAC8AFQsLJgAFJwAAMgAASAAATgAAPQAANQAAMgAAMAAALQAAMwAALQAAFQQEBQMXAgIkAAAxAAAxAAAzAAAyAAAyAAAxAAAxAAAuAAAxAAAuAAAtAAAsAAAsAAAkAAAd";

        public const string DefinitionFileName = "def.json";
        public const string StepsFileName = "steps.json";
        public const int AnyPersonality = 0;
        internal const string ClothesSetResetCode = "RESET";

        internal static class PersonalityType
        {
            internal const int Muku = 80;       //無垢
            internal const int Majime = 90;     //真面目
            internal const int Rindere = 100;   //凛デレ
            internal const int Pure = 10;       //純真
            internal const int Pride = 30;      //ツンデレ
            internal const int Cool = 20;       //クーデレ
            internal const int Yandere = 40;    //ヤンデレ
            internal const int Anesan = 50;     //お姉ちゃん
            internal const int Genki = 60;      //ボクっ娘
            internal const int Sadist = 70;     //ドＳ
            internal const int Silent = 110;    //文学少女
            internal const int Devilish = 120;  //小悪魔
            internal const int Ladylike = 130;  //おしとやか
            internal const int Secretary = 140; //メイド秘書
            internal const int Sister = 150;    //ふわふわ妹
            internal const int Curtness = 160;  //無愛想
            internal const int Missy = 170;     //お嬢様
            internal const int Childhood = 180; //幼馴染
            internal const int Masochist = 190; //ド変態ドＭ
            internal const int Cunning = 200;   //腹黒
            internal const int Friendly = 210;  //気さく
            internal const int Dame = 220;      //淑女
            internal const int Gyaru = 230;     //ギャル
        }

        internal static readonly string[] DressingClothingTagArray = {
            ClothingTag.acchat,
            ClothingTag.headset,
            ClothingTag.wear,
            ClothingTag.skirt,
            ClothingTag.onepiece,
            ClothingTag.mizugi,
            ClothingTag.bra,
            ClothingTag.panz,
            ClothingTag.acckubi,
            ClothingTag.acckubiwa,
            ClothingTag.shoes,
            ClothingTag.stkg,
            ClothingTag.accude,
            ClothingTag.accsenaka,
            ClothingTag.glove,
            ClothingTag.accashi,
            ClothingTag.accshippo,
        };



        internal static class EventProgress
        {
            internal const string None = "NONE";
            internal const string Init = "INIT";
            internal const string ADV = "ADV";
            internal const string EndingADV = "ENDING_ADV";
        }

        internal static class ADVType
        {
            internal const string Chara = "Chara";
            internal const string Group = "Group";
            internal const string Talk = "Talk";
            internal const string ChangeBGM = "BGM";
            internal const string PlaySE = "SE";
            internal const string ChangeBackground = "BG";
            internal const string ChangeScene = "Scene";
            internal const string ChangeCamera = "Camera";
            internal const string ShowChoiceList = "List";

            internal const string FadeOut = "FadeOut";          //
            internal const string FadeIn = "FadeIn";                        //Add this type to separate fade in/out from each steps
            internal const string LoadScene = "LoadScene";

            internal const string CharaInit = "CharaInit";

            internal const string Pick = "Pick";
            internal const string MakeGroup = "MakeGroup";  //Assign characters into a group in order to set group motion etc
            internal const string DismissGroup = "DismissGroup";  //Assign characters into a group in order to set group motion etc

            internal const string AddTexture = "AddTexture";
            internal const string RemoveTexture = "RemoveTexture";

            internal const string Shuffle = "Shuffle";
            internal const string TimeWait = "TimeWait";

            internal const string AddObject = "AddObject";
            internal const string RemoveObject = "RemoveObject";

            internal const string Evaluate = "Evaluate";
            internal const string Branch = "Branch";

            internal const string ADVEnd = "ADVEnd";    //End the scenario and return to normal flow
        }

        internal static class ADVTalkSpearkerType
        {
            internal const string Narrative = "Narrative";
            internal const string Owner = "Owner";
            internal const string SelectedMaid = "SelectedMaid";        //Maid 0
            internal const string Maid = "Maid";                        //Require Index position
            internal const string RandomMaid = "RandomMaid";
            internal const string NPCFemale = "NPC_F";
            internal const string NPCMale = "NPC_M";
            internal const string All = "All";
        }

        internal static class TargetType
        {
            internal const string ClubOwner = "Owner";
            internal const string SingleMan = "M";
            internal const string SingleMaid = "F";
            internal const string AllMen = "M_ALL";
            internal const string AllMaids = "F_ALL";
            internal const string NPCFemale = "NPC_F";
            internal const string NPCMale = "NPC_M";
            internal const string AllNPCFemale = "NPC_F_ALL";
            internal const string AllNPCMale = "NPC_M_ALL";

        }

        internal static class WaitingType
        {
            internal const string Auto = "Auto";    //player no need to do anything and the adv scene will proceed to next step
            internal const string Click = "Click";
            internal const string InputChoice = "InputChoice";
            internal const string FadeOut = "FadeOut";
            internal const string CameraPan = "CameraPan";
            internal const string SystemFadeOut = "SystemFadeOut";      //This one is for waiting for the fade out caused by the original system not by this mod

            internal const string Special = "Special";  //Need special handling to proceed to next step (eg. doing some branching). 
        }

        internal static class GroupType
        {
            internal const string FF = "FF";
            internal const string MF = "MF";
            internal const string MMF = "MMF";
            internal const string FFM = "FFM";
            internal const string MMMF = "MMMF";
            internal const string Invalid = "";
        }

        internal enum CharacterType
        {
            Maid,
            Man,
            NPC
        }

        internal static class DefinedColorString
        {
            internal const string White = "White";
        }

        internal static class CameraEaseType
        {
            internal const string EaseOutCubic = "easeOutCubic";
        }

        internal static class DefinedGameObjectNames
        {
            internal const string ModAddedManGameObjectPrefix = "Man_";

            internal const string MaidHeadBoneName = "Bip01 Head";
            internal const string ManHeadBoneName = "ManBip Head";
        }

        internal static class JsonReplaceTextLabels
        {
            internal const string ClubName = "[=ClubName]";
            internal const string ClubOwnerName = "[=ClubOwnerName]";


            /*Format: [=Name,{Source},{Index},{CallMethod}]
             * {Source} :   "Maid" : From SelectedMaidsList; "NPC_F" : From NPCList;  "NPC_M" : From NPCManList
             * {Index}  :   Index position in the source list
             * {CallMethod} :   Options: "CallName", "FullName", "LastName", "FirstName"
             */
            internal const string CharacterNameRegex = @"\[\=Name,(Maid|NPC_F|NPC_M),(\d{1,2}),(CallName|FullName|LastName|FirstName|NickName)\]";

            /*Format: [=Var,{VariableName}]
             * {VariableName} :   The Key in the CustomVariable dictionary
             */
            internal const string VariableRegex = @"\[\=Var,([a-zA-Z0-9_]+)\]";

            ////Format: [=RandomGroup,{GroupIndex},Maid{MaidIndex}Name]
            //internal const string RandomGroupRegex = @"\[\=RandomGroup,(\d{1,2}),([a-zA-Z0-9]+)\]";

            internal static class CharacterNameSourceType
            {
                internal const string Maid = "Maid";
                internal const string NPCFemale = "NPC_F";
                internal const string NPCMale = "NPC_M";
            }
            internal static class CharacterNameDisplayType
            {
                internal const string CallName = "CallName";
                internal const string FullName = "FullName";
                internal const string LastName = "LastName";
                internal const string FirstName = "FirstName";
                internal const string NickName = "NickName";
            }
        }

        internal static class TextureType
        {
            internal const string Semen = "Semen";
            internal const string Candle = "Candle";
            internal const string WhipMark = "WhipMark";
            internal const string SlapMark = "SlapMark";
        }

        internal static class CallScreenName
        {
            internal const string Move = "Move";
            internal const string Title = "SceneToTitle";
        }


        internal static class ClothingTag
        {
            internal const string acchat = "acchat";            //帽子
            internal const string headset = "headset";          //ヘッドドレス
            internal const string acckami = "acckami";          //前髪
            internal const string acckamisub = "acckamisub";    //リボン
            internal const string wear = "wear";                //トップス
            internal const string skirt = "skirt";              //ボトムス
            internal const string megane = "megane";            //メガネ
            internal const string acchead = "acchead";          //アイマスク
            internal const string onepiece = "onepiece";        //ワンピース
            internal const string mizugi = "mizugi";            //水着
            internal const string accmimi = "accmimi";          //耳
            internal const string acchana = "acchana";          //鼻
            internal const string bra = "bra";                  //ブラジャー
            internal const string panz = "panz";                //パンツ
            internal const string acckubi = "acckubi";          //ネックレス
            internal const string acckubiwa = "acckubiwa";      //チョーカー
            internal const string shoes = "shoes";              //靴
            internal const string stkg = "stkg";                //靴下
            internal const string accude = "accude";            //腕
            internal const string accsenaka = "accsenaka";      //背中
            internal const string glove = "glove";              //手袋
            internal const string accashi = "accashi";          //足首
            internal const string accshippo = "accshippo";      //しっぽ
            internal const string accheso = "accheso";          //へそ
            internal const string accnip = "accnip";            //乳首
            internal const string accxxx = "accxxx";            //前穴
        }

        internal static class DefinedClassFieldNames
        {
            internal const string MaidStatusFirstName = "firstName_";
            internal const string MaidStatusLastName = "lastName_";
            internal const string MaidStatusNickName = "nickName_";

        }

        internal static class OperatorType
        {
            internal const string Assignment = "=";
            internal const string Addition = "+";
            internal const string Subtraction = "-";
            internal const string Multiplication = "*";
            internal const string Division = "/";

            internal const string Equal = "==";
            internal const string GreaterThan = ">";
            internal const string GreaterThanEqualTo = ">=";
            internal const string LessThan = "<";
            internal const string LessThanEqualTo = "<=";

            internal const string NotEqual = "!=";
            internal const string LogicalAnd = "&&";
            internal const string LogicalOr = "||";
            internal const string Negation = "!";

            internal const string Concatenation = "&";
        }

        public enum VariableType
        {
            Integer,
            FloatingPoint,
            String,
            Boolean,
        }

        internal static class CharacterStatusField
        {
            internal const string Likability = "likability";
            internal const string Lovely = "lovely";
            internal const string Elegance = "elegance";
            internal const string Charm = "charm";
            internal const string Care = "care";
            internal const string Reception = "reception";
            internal const string Cooking = "cooking";
            internal const string Dance = "dance";
            internal const string Vocal = "vocal";
            internal const string NightWorkCount = "playCountNightWork";

            internal const string Inyoku = "inyoku";
            internal const string MValue = "mvalue";
            internal const string Hentai = "hentai";
            internal const string Houshi = "housi";
            internal const string YotogiCount = "playCountYotogi";

            internal const string HeroineType = "personal.id";
            internal const string SexExperienceVaginal = "seikeiken.vaginal";
            internal const string SexExperienceAnal = "seikeiken.anal";
            internal const string Height = "body.height";
            internal const string Weight = "body.weight";
            internal const string Bust = "body.bust";
            internal const string Waist = "body.waist";
            internal const string Hip = "body.hip";
            internal const string Cup = "body.cup";

            internal const string SexNumOfPeople = "sexPlayNumberOfPeople";
        }
    }
}
