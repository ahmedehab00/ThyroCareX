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
                    RoiImageUrl = t.DiagnosisResult != null ? t.DiagnosisResult.RoiImageUrl : null
                })));
        }
    }
}
