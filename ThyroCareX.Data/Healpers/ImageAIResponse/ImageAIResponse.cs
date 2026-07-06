using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ThyroCareX.Core.Dto.ImageAIResponse
{
    public class ImageAIResponse
    {
        public string? Filename { get; set; }
        public string Status { get; set; }

        public List<int>? Bbox { get; set; }

        public ClassificationDto Classification { get; set; }

        public ImageUrlsDto Images { get; set; }

        public SegmentationDto? Segmentation { get; set; }

        public string? Message { get; set; }

        [JsonPropertyName("ai_recommendation")]
        public string? AiRecommendation { get; set; }

        [JsonPropertyName("medical_disclaimer")]
        public string? MedicalDisclaimer { get; set; }
    }

    public class ClassificationDto
    {
        public int Prediction { get; set; }

        public string Label { get; set; }

        [JsonPropertyName("confidence_pct")]
        public double Confidence { get; set; }

        [JsonPropertyName("acr_tirads_level")]
        public string Tirads_Stage { get; set; }

        [JsonPropertyName("risk_level")]
        public string RiskLevel { get; set; }

        [JsonPropertyName("clinical_recommendation")]
        public string ClinicalRecommendation { get; set; }

        [JsonPropertyName("next_step")]
        public string? NextStep { get; set; }

        [JsonPropertyName("needs_manual_review")]
        public bool? NeedsManualReview { get; set; }

        [JsonPropertyName("radiomic_features")]
        public RadiomicFeaturesDto? RadiomicFeatures { get; set; }
    }

    public class RadiomicFeaturesDto
    {
        [JsonPropertyName("taller_than_wide")]
        public bool? TallerThanWide { get; set; }

        [JsonPropertyName("solidity")]
        public double? Solidity { get; set; }

        [JsonPropertyName("circularity")]
        public double? Circularity { get; set; }

        [JsonPropertyName("irregular_margin")]
        public bool? IrregularMargin { get; set; }

        [JsonPropertyName("nodule_intensity")]
        public double? NoduleIntensity { get; set; }

        [JsonPropertyName("tissue_intensity")]
        public double? TissueIntensity { get; set; }

        [JsonPropertyName("hypoechoic")]
        public bool? Hypoechoic { get; set; }

        [JsonPropertyName("markedly_hypoechoic")]
        public bool? MarkedlyHypoechoic { get; set; }
    }

    public class SegmentationDto
    {
        [JsonPropertyName("method")]
        public string? Method { get; set; }

        [JsonPropertyName("roi_extraction")]
        public string? RoiExtraction { get; set; }
    }

    public class ImageUrlsDto
    {
        [JsonPropertyName("mask_url")]
        public string? Mask_Url { get; set; }

        [JsonPropertyName("overlay_url")]
        public string? Overlay_Url { get; set; }

        [JsonPropertyName("roi_url")]
        public string? Roi_Url { get; set; }

        [JsonPropertyName("original_url")]
        public string? Original_Url { get; set; }

        [JsonPropertyName("mask_overlay_url")]
        public string? Mask_Overlay_Url { get; set; }

        [JsonPropertyName("annotated_url")]
        public string? Annotated_Url { get; set; }
    }
}
