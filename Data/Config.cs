using UnityEngine;
using SMLHelper.V2.Json;

namespace BlueprintRandomizer.Data
{
    class Config : ConfigFile
    {
        public string Seed = "";
        public bool UseAllScanner = false;
        public bool UseAllAnalysis = false;
        public bool UseAllGoals = false;
        public bool RandomStartingBlueprints = false;
        public bool UseProgression = false;
        public bool AllowSoftlocks = false;
        public bool StartingEquipment = false;
    }
}
