using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COM3D2.CustomEventLoader.Plugin.DataClass
{
    internal class ScenarioDefinition
    {

        public string Title;
        public string EventContents;
        public string Icon;
        public int EditorVersion;

        public string Author;
        public string Language;
        //Key: Personality, Value: Number of Maid required; Personality value 0 means All
        public List<MaidRequirementInfo> MaidRequirement;

        //System Use
        public string EntryStep;

        //Not Defined in json file
        public string FilePath;

        public class MaidRequirementInfo
        {
            public List<int> PersonalityID;
            public int IndexPosition;
        }
    }
}
