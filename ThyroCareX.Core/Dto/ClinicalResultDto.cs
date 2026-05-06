using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace ThyroCareX.Core.Dto
{
    public class ClinicalResultDto
    {
        [JsonPropertyName("functional_status")]
        public string FunctionalStatus { get; set; }
        
        [JsonPropertyName("risk_level")]
        public string RiskLevel { get; set; }
        
        [JsonPropertyName("clinical_recommendation")]
        public string ClinicalRecommendation { get; set; }
        
        [JsonPropertyName("ai_recommendation")]
        public string AiRecommendation { get; set; }
        
        [JsonPropertyName("model_confidence")]
        public double ModelConfidence { get; set; }
        
        [JsonPropertyName("needs_manual_review")]
        public bool NeedsManualReview { get; set; }
        
        [JsonPropertyName("next_step")]
        public string NextStep { get; set; }
        
        [JsonPropertyName("probabilities")]
        public Dictionary<string, double> Probabilities { get; set; }
    }
}
