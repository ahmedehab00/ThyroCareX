using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace ThyroCareX.Core.Dto
{
    public class AssessClinicalResponse
    {
        [JsonPropertyName("test_id")]
        public int TestId { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("clinical")]
        public ClinicalResultDto Clinical { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
