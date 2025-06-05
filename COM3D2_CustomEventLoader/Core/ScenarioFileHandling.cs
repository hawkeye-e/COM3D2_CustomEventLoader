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
    }
}
