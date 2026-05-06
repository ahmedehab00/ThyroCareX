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
        public void GetPatientListByDoctorMapping()
        {
            CreateMap<Patient, GetPatientListResponseByDoctor>()
                .ForMember(dest => dest.PatientID, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src =>
                    src.DateOfBirth == default
                        ? 0
                        : (int)((DateTime.UtcNow - src.DateOfBirth).TotalDays / 365.25)))
                .ForMember(dest => dest.LatestStatus, opt => opt.MapFrom(src =>
                    src.Tests.OrderByDescending(t => t.Id).Where(t => t.DiagnosisResult != null).Select(t => t.DiagnosisResult.FunctionalStatus ?? t.DiagnosisResult.ClassificationLabel).FirstOrDefault()))
                .ForMember(dest => dest.CancerClassification, opt => opt.MapFrom(src =>
                    src.Tests.OrderByDescending(t => t.Id)
                        .Where(t => t.DiagnosisResult != null)
                        .Select(t => t.DiagnosisResult.ClassificationLabel ?? t.DiagnosisResult.RiskLevel ?? t.DiagnosisResult.BethesdaLabel)
                        .FirstOrDefault()))
                .ForMember(dest => dest.NextStep, opt => opt.MapFrom(src =>
                    src.Tests.OrderByDescending(t => t.Id).Where(t => t.DiagnosisResult != null).Select(t => t.DiagnosisResult.NextStep).FirstOrDefault()))
                .ForMember(dest => dest.RiskConfidence, opt => opt.MapFrom(src =>
                    src.Tests.OrderByDescending(t => t.Id).Where(t => t.DiagnosisResult != null).Select(t => t.DiagnosisResult.Confidence).FirstOrDefault()));
        }
    }
}
