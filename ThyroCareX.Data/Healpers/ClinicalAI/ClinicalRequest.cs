using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ThyroCareX.Data.Healpers.ClinicalAI
{
    public class ClinicalRequest
    {
        [JsonPropertyName("patient_id")]
        public string PatientId { get; set; }

        [JsonPropertyName("session_id")]
        public string SessionId { get; set; }

        public int Age { get; set; }

        [JsonPropertyName("on_thyroxine")]
        public int OnThyroxine { get; set; } // 0 or 1

        [JsonPropertyName("thyroid_surgery")]
        public int ThyroidSurgery { get; set; } // 0 or 1

        [JsonPropertyName("query_hyperthyroid")]
        public int QueryHyperthyroid { get; set; } // 0 or 1

        [JsonPropertyName("TSH")]
        public double? TSH { get; set; }
        [JsonPropertyName("T3")]
        public double? T3 { get; set; }
        [JsonPropertyName("TT4")]
        public double? TT4 { get; set; }
        [JsonPropertyName("FTI")]
        public double? FTI { get; set; }
        [JsonPropertyName("T4U")]
        public double? T4U { get; set; }

        [JsonPropertyName("nodule_present")]
        public bool NodulePresent { get; set; }
    }
}
