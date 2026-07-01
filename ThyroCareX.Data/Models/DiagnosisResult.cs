using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThyroCareX.Data.Models
{
    public class DiagnosisResult
    {
        public int Id { get; set; }

        public int TestId { get; set; }
        [ForeignKey("TestId")]
        public Test Test { get; set; }

        // 🧠 Clinical AI
        public string? FunctionalStatus { get; set; }
        public string? RiskLevel { get; set; }

        public string? ClinicalRecommendation { get; set; }
        public string? NextStep { get; set; }

        // 🖼️ Image AI
        public string? ClassificationLabel { get; set; }
        public double? Confidence { get; set; }
        public string? TiradsStage { get; set; }

        public string? OverlayImageUrl { get; set; }
        public string? MaskImageUrl { get; set; }
        public string? RoiImageUrl { get; set; }

        // 🔬 FNAC AI
        public string? BethesdaCategory { get; set; }
        public string? BethesdaLabel { get; set; }
        public string? MalignancyRisk { get; set; }
        public string? FnacRecommendation { get; set; }

        // 🔥 Raw JSON (اختياري مهم جدًا)
        public string? RawResponse { get; set; }
        public string? FnacRawResponse { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
