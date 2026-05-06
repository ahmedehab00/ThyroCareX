using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ThyroCareX.Data.Healpers.ClinicalAIResponse
{
    public class ClinicalAIResponse
    {

        public string Status { get; set; }

        [JsonPropertyName("patient_id")]
        public int PatientId { get; set; }

        [JsonPropertyName("functional_status")]
        public string FunctionalStatus { get; set; }

        public Dictionary<string, double> Probabilities { get; set; }
        
        [JsonPropertyName("model_confidence")]
        public double ModelConfidence { get; set; }

        [JsonPropertyName("needs_manual_review")]
        public bool NeedsManualReview { get; set; }

        [JsonPropertyName("risk_level")]
        public string RiskLevel { get; set; }

        [JsonPropertyName("clinical_recommendation")]
        public string ClinicalRecommendation { get; set; }

        [JsonPropertyName("ai_recommendation")]
        public string AiRecommendation { get; set; }

        [JsonPropertyName("next_step")]
        public string NextStep { get; set; }

        [JsonPropertyName("next_step_details")]
        public Dictionary<string, object>? NextStepDetails { get; set; }

        public string Disclaimer { get; set; }
    }
}
