using BepInEx.Logging;
using COM3D2.CustomEventLoader.Plugin.DataClass;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.CustomEventLoader.Plugin
{
    class Util
    {
        public static string GetPersonalityNameByValue(int value)
        {
            switch (value)
            {
                case Constant.PersonalityType.Muku:
                    return nameof(Constant.PersonalityType.Muku);
                case Constant.PersonalityType.Majime:
                    return nameof(Constant.PersonalityType.Majime);
                case Constant.PersonalityType.Rindere:
                    return nameof(Constant.PersonalityType.Rindere);
                case Constant.PersonalityType.Pure:
                    return nameof(Constant.PersonalityType.Pure);
                case Constant.PersonalityType.Pride:
                    return nameof(Constant.PersonalityType.Pride);
                case Constant.PersonalityType.Cool:
                    return nameof(Constant.PersonalityType.Cool);
                case Constant.PersonalityType.Yandere:
                    return nameof(Constant.PersonalityType.Yandere);
                case Constant.PersonalityType.Anesan:
                    return nameof(Constant.PersonalityType.Anesan);
                case Constant.PersonalityType.Genki:
                    return nameof(Constant.PersonalityType.Genki);
                case Constant.PersonalityType.Sadist:
                    return nameof(Constant.PersonalityType.Sadist);
                case Constant.PersonalityType.Silent:
                    return nameof(Constant.PersonalityType.Silent);
                case Constant.PersonalityType.Devilish:
                    return nameof(Constant.PersonalityType.Devilish);
                case Constant.PersonalityType.Ladylike:
                    return nameof(Constant.PersonalityType.Ladylike);
                case Constant.PersonalityType.Secretary:
                    return nameof(Constant.PersonalityType.Secretary);
                case Constant.PersonalityType.Sister:
                    return nameof(Constant.PersonalityType.Sister);
                case Constant.PersonalityType.Curtness:
                    return nameof(Constant.PersonalityType.Curtness);
                case Constant.PersonalityType.Missy:
                    return nameof(Constant.PersonalityType.Missy);
                case Constant.PersonalityType.Childhood:
                    return nameof(Constant.PersonalityType.Childhood);

                case Constant.PersonalityType.Masochist:
                    return nameof(Constant.PersonalityType.Masochist);
                case Constant.PersonalityType.Cunning:
                    return nameof(Constant.PersonalityType.Cunning);
                case Constant.PersonalityType.Friendly:
                    return nameof(Constant.PersonalityType.Friendly);
                case Constant.PersonalityType.Dame:
                    return nameof(Constant.PersonalityType.Dame);
                case Constant.PersonalityType.Gyaru:
                    return nameof(Constant.PersonalityType.Gyaru);
                default:
                    return "Unknown";
            }
        }

        internal static void SmoothMoveMaidPosition(Maid maid, Vector3 targetPosition, Quaternion targetRotation, float time = -1)
        {
            System.Collections.Hashtable args = new System.Collections.Hashtable();
            args.Add("position", targetPosition);
            args.Add("rotation", targetRotation.eulerAngles);
            args.Add("scale", maid.transform.localScale);
            if (time > 0)
                args.Add("time", time);
            iTween.MoveTo(maid.gameObject, args);

            System.Collections.Hashtable argsRotate = new System.Collections.Hashtable();
            argsRotate.Add("position", targetPosition);
            argsRotate.Add("rotation", targetRotation.eulerAngles);
            argsRotate.Add("scale", maid.transform.localScale);
            if (time > 0)
                argsRotate.Add("time", time);
            iTween.RotateTo(maid.gameObject, argsRotate);
        }

        internal static void StopSmoothMove(Maid maid)
        {
            iTween.Stop(maid.gameObject);
        }

        internal static ScenarioDefinition GetCurrentScenarioDefinition()
        {
            return StateManager.Instance.ScenarioList[StateManager.Instance.SelectedScenarioID];
        }

        internal static Maid SearchManCharacterByGUID(string GUID)
        {
            //check also the club owner
            if (StateManager.Instance.ClubOwner != null)
                if (StateManager.Instance.ClubOwner.status.guid == GUID)
                {
                    return StateManager.Instance.ClubOwner;
                }
            //man list
            if (StateManager.Instance.MenList != null)
                foreach (Maid man in StateManager.Instance.MenList)
                    if (man.status.guid == GUID)
                    {
                        return man;
                    }
            //npc list
            if (StateManager.Instance.NPCManList != null)
                foreach (Maid npc in StateManager.Instance.NPCManList)
                    if (npc.status.guid == StateManager.Instance.processingManGUID)
                    {
                        return npc;
                    }

            return null;
        }

        internal static void SetCustomVariable(string name, object value)
        {
            if (StateManager.Instance.CustomVariable.ContainsKey(name))
                StateManager.Instance.CustomVariable.Remove(name);

            StateManager.Instance.CustomVariable.Add(name, value);
        }


        internal static float RoundDegree(float degres)
        {
            if (!(0f <= degres) || !(degres <= 360f))
            {
                degres -= Mathf.Floor(degres / 360f) * 360f;
                return (!Mathf.Approximately(360f, degres)) ? degres : 0f;
            }
            return Mathf.Abs(degres);
        }

        public static bool NearlyEquals(Vector3 v1, Vector3 v2, float unimportantDifference = 0.0001f)
        {
            if (v1 != v2)
            {
                return NearlyEquals(v1.x, v2.x) && NearlyEquals(v1.y, v2.y) && NearlyEquals(v1.z, v2.z);
            }

            return true;
        }

        public static bool NearlyEquals(Vector2 v1, Vector2 v2, float unimportantDifference = 0.0001f)
        {
            if (v1 != v2)
            {
                return NearlyEquals(v1.x, v2.x) && NearlyEquals(v1.y, v2.y);
            }

            return true;
        }

        public static bool NearlyEquals(float value1, float value2, float unimportantDifference = 0.0001f)
        {
            if (value1 != value2)
            {
                return Math.Abs(value1 - value2) < unimportantDifference;
            }

            return true;
        }

        public static bool NearlyEquals(double value1, double value2, float unimportantDifference = 0.0001f)
        {
            if (value1 != value2)
            {
                return Math.Abs(value1 - value2) < unimportantDifference;
            }

            return true;
        }


        public static Vector2 ParseVector2RawString(string vectorInString)
        {
            if (string.IsNullOrEmpty(vectorInString))
                return Vector3.zero;

            var split = vectorInString.Split(',');
            return new Vector2(float.Parse(split[0].Trim(), CultureInfo.InvariantCulture), float.Parse(split[1].Trim(), CultureInfo.InvariantCulture));
        }

        public static Vector3 ParseVector3RawString(string vectorInString)
        {
            if (string.IsNullOrEmpty(vectorInString))
                return Vector3.zero;

            var split = vectorInString.Split(',');
            return new Vector3(float.Parse(split[0].Trim(), CultureInfo.InvariantCulture), float.Parse(split[1].Trim(), CultureInfo.InvariantCulture), float.Parse(split[2].Trim(), CultureInfo.InvariantCulture));
        }

        public static Quaternion ParseQuaternionRawString(string quaternionInString)
        {
            var split = quaternionInString.Split(',');
            return new Quaternion(float.Parse(split[0].Trim(), CultureInfo.InvariantCulture), float.Parse(split[1].Trim(), CultureInfo.InvariantCulture), float.Parse(split[2].Trim(), CultureInfo.InvariantCulture), float.Parse(split[3].Trim(), CultureInfo.InvariantCulture));
        }

        internal static void ClearGenericCollection<T>(List<T> list)
        {
            if (list != null)
                list.Clear();
            list = null;
        }

        internal static void ClearGenericCollection<TKey, TValue>(Dictionary<TKey, TValue> list)
        {
            if (list != null)
                list.Clear();
            list = null;
        }

        internal static Color ConvertHexColorToColor(string hex)
        {
            try
            {
                int a = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                int r = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                int g = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                int b = int.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
                return new Color(r, g, b, a);
            }
            catch
            {
                //something wrong with the input string, return a random color instead
                return new Color(RNG.Random.Next(256), RNG.Random.Next(256), RNG.Random.Next(256));
            }
        }

        internal static object ArithmeticOperation(string operatorType, object input1, object input2)
        {
            if (input1 is int && input2 is int)
            {
                //both int, result need to be int
                int x = (int)input1;
                int y = (int)input2;

                switch (operatorType)
                {
                    case Constant.OperatorType.Addition:
                        return x + y;
                    case Constant.OperatorType.Subtraction:
                        return x - y;
                    case Constant.OperatorType.Multiplication:
                        return x * y;
                    case Constant.OperatorType.Division:
                        return x / y;
                }
            }
            else if (input1 is double  || input2 is double)
            {
                //at least one of them is double
                double x = Convert.ToDouble(input1);
                double y = Convert.ToDouble(input2);

                switch (operatorType)
                {
                    case Constant.OperatorType.Addition:
                        return x + y;
                    case Constant.OperatorType.Subtraction:
                        return x - y;
                    case Constant.OperatorType.Multiplication:
                        return x * y;
                    case Constant.OperatorType.Division:
                        return x / y;
                }
            }

            throw new EvaluateException("[Invalid Arithmetic Operation] No match for input data type and operator");
        }

        internal static bool ComparativeOperation(string operatorType, object input1, object input2)
        {
            if (input1 is int && input2 is int)
            {
                //both int
                int x = (int)input1;
                int y = (int)input2;

                switch (operatorType)
                {
                    case Constant.OperatorType.Equal:
                        return x == y;
                    case Constant.OperatorType.GreaterThan:
                        return x > y;
                    case Constant.OperatorType.GreaterThanEqualTo:
                        return x >= y;
                    case Constant.OperatorType.LessThan:
                        return x < y;
                    case Constant.OperatorType.LessThanEqualTo:
                        return x <= y;
                    case Constant.OperatorType.NotEqual:
                        return x != y;
                }
            }else if (input1 is double || input2 is double)
            {
                //both int
                double x = Convert.ToDouble(input1);
                double y = Convert.ToDouble(input2);

                
                switch (operatorType)
                {
                    case Constant.OperatorType.Equal:
                        return NearlyEquals(x, y);
                    case Constant.OperatorType.GreaterThan:
                        return x > y;
                    case Constant.OperatorType.GreaterThanEqualTo:
                        return x >= y;
                    case Constant.OperatorType.LessThan:
                        return x < y;
                    case Constant.OperatorType.LessThanEqualTo:
                        return x <= y;
                    case Constant.OperatorType.NotEqual:
                        return !NearlyEquals(x, y);
                }
            }
            else if (input1 is string || input2 is string)
            {
                string x = Convert.ToString(input1);
                string y = Convert.ToString(input2);

                switch (operatorType)
                {
                    case Constant.OperatorType.Equal:
                        return x == y;
                    case Constant.OperatorType.NotEqual:
                        return x != y;
                }
            }
            else if (input1 is bool || input2 is bool)
            {
                bool x = Convert.ToBoolean(input1);
                bool y = Convert.ToBoolean(input2);

                switch (operatorType)
                {
                    case Constant.OperatorType.Equal:
                        return x == y;
                    case Constant.OperatorType.NotEqual:
                        return x != y;
                }
            }

            throw new EvaluateException("[Invalid Comparative Operation] No match for input data type and operator");
        }

        internal static bool LogicalOperation(string operatorType, object input1, object input2)
        {
            if (input1 is bool || input2 is bool)
            {
                bool x = Convert.ToBoolean(input1);

                if (operatorType == Constant.OperatorType.Negation)
                    return !x;

                bool y = Convert.ToBoolean(input2);

                switch (operatorType)
                {
                    case Constant.OperatorType.LogicalAnd:
                        return x && y;
                    case Constant.OperatorType.LogicalOr:
                        return x || y;
                }
            }

            throw new EvaluateException("[Invalid Logical Operation] No match for input data type and operator");
            //return false;
        }

        public static void ShowError(string stepID, string message)
        {
            CustomEventLoader.Log.LogError("Step: " + stepID + ", Error: " + message);
        }

        public static string GetFullName(Maid maid)
        {
            if (Product.isJapan)
                return maid.status.fullNameJpStyle;
            else
                return maid.status.fullNameEnStyle;
        }
    }
}
