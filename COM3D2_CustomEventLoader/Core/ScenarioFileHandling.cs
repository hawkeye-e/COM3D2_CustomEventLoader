using BepInEx.Logging;
using COM3D2.CustomEventLoader.Plugin.DataClass;
using HarmonyLib;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using static CharacterMgr;

namespace COM3D2.CustomEventLoader.Plugin.Core
{
    class ScenarioFileHandling
    {
        internal static Dictionary<string, ADVStep> ReadZipFileSteps(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    using (var zipFile = new ZipFile(fileStream))
                    {
                        var zipEntry = zipFile.GetEntry(Constant.StepsFileName);
                        if (zipEntry == null)
                        {
                            //Incorrect format
                            return null;
                        }

                        using (var s = zipFile.GetInputStream(zipEntry))
                        {
                            StreamReader reader = new StreamReader(s, System.Text.Encoding.UTF8);
                            string fileContent = reader.ReadToEnd();

                            Dictionary<string, ADVStep> steps = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, ADVStep>>(fileContent);

                            return steps;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //In case there are non zip files in the folder
                    return null;
                }
            }
        }

        internal static ScenarioDefinition ReadZipFileDefinition(string filePath)
        {

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    using (var zipFile = new ZipFile(fileStream))
                    {
                        var zipEntry = zipFile.GetEntry(Constant.DefinitionFileName);
                        if (zipEntry == null)
                        {
                            //Incorrect format
                            return null;
                        }

                        using (var s = zipFile.GetInputStream(zipEntry))
                        {
                            StreamReader reader = new StreamReader(s, System.Text.Encoding.UTF8);
                            string fileContent = reader.ReadToEnd();

                            ScenarioDefinition scnDef = Newtonsoft.Json.JsonConvert.DeserializeObject<ScenarioDefinition>(fileContent);

                            return scnDef;
                        }
                    }
                }
                catch (Exception)
                {
                    //In case there are non zip files in the folder
                    return null;
                }
            }
        }

        internal static byte[] GetCustomEventFileContentInByteArray(string zipFilePath, string fileName)
        {
            byte[] result;

            using (var fileStream = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    using (var zipFile = new ZipFile(fileStream))
                    {
                        var zipEntry = zipFile.GetEntry(fileName);
                        if (zipEntry == null)
                        {
                            //file not found
                            return null;
                        }

                        using (var s = zipFile.GetInputStream(zipEntry))
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
#if COM3D2_5
#if UNITY_2022_3
                                s.CopyTo(ms);
#endif
#endif

#if COM3D2
                                CopyStream(s, ms);
#endif
                                result = ms.ToArray();
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    //In case there are non zip files in the folder
                    return null;
                }
            }

            return result;
        }

        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[81920];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }
    }
}
