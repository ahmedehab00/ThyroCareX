using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThyroCareX.Core.Feature.Patients.Queries.Result;
using ThyroCareX.Data.Models;

namespace ThyroCareX.Core.Mapping.PatientMapp
{
    public partial class PatientProfile
    {
        private void GetPatientByIdMapping()
        {
            CreateMap<Patient, GetPatientByIdResponse>()
                .ForMember(dest => dest.PatientID, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.RegistrationAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src =>
                    src.DateOfBirth == default
                        ? 0
                        : (int)((DateTime.UtcNow - src.DateOfBirth).TotalDays / 365.25)))
                .ForMember(dest => dest.Tests, opt => opt.MapFrom(src => src.Tests.Select(t => new PatientTestDto
                {
                    TestId = t.Id,
                    ImagePath = t.ImagePath,
                    CreatedAt = t.CreatedAt,
                    DiagnosisResult = t.DiagnosisResult != null ? t.DiagnosisResult.FunctionalStatus : null,
                    Confidence = t.DiagnosisResult != null ? t.DiagnosisResult.Confidence : null,
                    Classification = t.DiagnosisResult != null ? t.DiagnosisResult.ClassificationLabel : null,
                    BethesdaLabel = t.DiagnosisResult != null ? t.DiagnosisResult.BethesdaLabel : null,
                    NextStep = t.DiagnosisResult != null ? t.DiagnosisResult.NextStep : null,
                    TSH = t.TSH,
                    T3 = t.T3,
                    TT4 = t.TT4,
                    FTI = t.FTI,
                    T4U = t.T4U,
                    NodulePresent = t.NodulePresent,
                    OnThyroxine = t.OnThyroxine,
                    ThyroidSurgery = t.ThyroidSurgery,
                    QueryHyperthyroid = t.QueryHyperthyroid,
                    TiradsStage = t.DiagnosisResult != null ? t.DiagnosisResult.TiradsStage : null,
                    ClinicalRecommendation = t.DiagnosisResult != null ? t.DiagnosisResult.ClinicalRecommendation : null,
                    RiskLevel = t.DiagnosisResult != null ? t.DiagnosisResult.RiskLevel : null,
                    OverlayImageUrl = t.DiagnosisResult != null ? t.DiagnosisResult.OverlayImageUrl : null,
                    MaskImageUrl = t.DiagnosisResult != null ? t.DiagnosisResult.MaskImageUrl : null,
                    RoiImageUrl = t.DiagnosisResult != null ? t.DiagnosisResult.RoiImageUrl : null,
                    AtaLevel = ExtractAtaLevel(t.DiagnosisResult != null ? t.DiagnosisResult.RawResponse : null),
                    NeedsManualReview = ExtractNeedsManualReview(t.DiagnosisResult != null ? t.DiagnosisResult.RawResponse : null),
                    Consensus = ExtractConsensus(t.DiagnosisResult != null ? t.DiagnosisResult.RawResponse : null)
                })));
        }

        private static string? ExtractAtaLevel(string? rawResponse)
        {
            if (string.IsNullOrEmpty(rawResponse)) return null;
            try
            {
                if (rawResponse.TrimStart().StartsWith("["))
                {
                    var images = System.Text.Json.JsonDocument.Parse(rawResponse).RootElement;
                    if (images.ValueKind == System.Text.Json.JsonValueKind.Array && images.GetArrayLength() > 0)
                    {
                        var first = images[0];
                        if (first.TryGetProperty("classification", out var classification) || first.TryGetProperty("Classification", out classification))
                        {
                            if (classification.TryGetProperty("ata_level", out var ataLevel) || classification.TryGetProperty("ATA_Level", out ataLevel))
                            {
                                return ataLevel.GetString();
                            }
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        private static bool ExtractNeedsManualReview(string? rawResponse)
        {
            if (string.IsNullOrEmpty(rawResponse)) return false;
            try
            {
                var doc = System.Text.Json.JsonDocument.Parse(rawResponse).RootElement;
                if (doc.ValueKind == System.Text.Json.JsonValueKind.Object)
                {
                    if (doc.TryGetProperty("needs_manual_review", out var needsReview) || doc.TryGetProperty("NeedsManualReview", out needsReview))
                    {
                        return needsReview.ValueKind == System.Text.Json.JsonValueKind.True || (needsReview.ValueKind == System.Text.Json.JsonValueKind.String && bool.TryParse(needsReview.GetString(), out var parsed) && parsed);
                    }
                }
                else if (doc.ValueKind == System.Text.Json.JsonValueKind.Array && doc.GetArrayLength() > 0)
                {
                    var first = doc[0];
                    if (first.TryGetProperty("classification", out var classification) || first.TryGetProperty("Classification", out classification))
                    {
                        if (classification.TryGetProperty("needs_manual_review", out var needsReview) || classification.TryGetProperty("NeedsManualReview", out needsReview))
                        {
                            return needsReview.ValueKind == System.Text.Json.JsonValueKind.True || (needsReview.ValueKind == System.Text.Json.JsonValueKind.String && bool.TryParse(needsReview.GetString(), out var parsed) && parsed);
                        }
                    }
                }
            }
            catch { }
            return false;
        }

        private static string? ExtractConsensus(string? rawResponse)
        {
            if (string.IsNullOrEmpty(rawResponse)) return null;
            try
            {
                if (rawResponse.TrimStart().StartsWith("["))
                {
                    var images = System.Text.Json.JsonDocument.Parse(rawResponse).RootElement;
                    if (images.ValueKind == System.Text.Json.JsonValueKind.Array && images.GetArrayLength() > 0)
                    {
                        var first = images[0];
                        if (first.TryGetProperty("consensus", out var consensus) || first.TryGetProperty("Consensus", out consensus))
                        {
                            if (consensus.TryGetProperty("label", out var label) || consensus.TryGetProperty("Label", out label))
                            {
                                return label.GetString();
                            }
                        }
                    }
                }
            }
            catch { }
            return null;
        }
    }
}
