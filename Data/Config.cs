using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace BlueprintRandomizer.Data
{
    class Config
    {
        [JsonPropertyName("seed")]
        public string Seed { get; set; }

        [JsonPropertyName("use_all_scanner_entries")]
        public bool UseAllScanner { get; set; }

        [JsonPropertyName("use_all_analysis_entries")]
        public bool UseAllAnalysis { get; set; }

        [JsonPropertyName("use_all_goal_entries")]
        public bool UseAllGoals { get; set; }

        [JsonPropertyName("randomize_starting_blueprints")]
        public bool RandomStartingBlueprints { get; set; } 

        [JsonPropertyName("use_progression")]
        public bool UseProgression { get; set; }

        [JsonPropertyName("allow_softlocks")]
        public bool allowSoftlocks { get; set; }


    }
}
