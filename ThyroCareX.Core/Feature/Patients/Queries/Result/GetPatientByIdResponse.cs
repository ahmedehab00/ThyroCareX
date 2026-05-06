using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThyroCareX.Data.Enums;

namespace ThyroCareX.Core.Feature.Patients.Queries.Result
{
    public class GetPatientByIdResponse
    {
        public int PatientID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public Gender gender { get; set; }
        public int Age { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Address { get; set; }
        public DateTime RegistrationAt { get; set; } = DateTime.UtcNow;
        public int? DoctorID { get; set; }

        // Admission / Patient details
        public DateTime DateOfBirth { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public string MedicalHistory { get; set; } = string.Empty;
        public string CurrentMedications { get; set; } = string.Empty;
        public string KnownAllergies { get; set; } = string.Empty;
        public string? AttachmentPath { get; set; }
        public List<PatientTestDto> Tests { get; set; } = new();
    }

    public class PatientTestDto
    {
        public int TestId { get; set; }
        public string? ImagePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? DiagnosisResult { get; set; }
        public double? Confidence { get; set; }
        public string? Classification { get; set; }
        public string? BethesdaLabel { get; set; }
        public string? NextStep { get; set; }

        // 🧪 Lab Data
        public double? TSH { get; set; }
        public double? T3 { get; set; }
        public double? TT4 { get; set; }
        public double? FTI { get; set; }
        public double? T4U { get; set; }

        // Clinical Info
        public bool NodulePresent { get; set; }
        public int OnThyroxine { get; set; }
        public int ThyroidSurgery { get; set; }
        public int QueryHyperthyroid { get; set; }

        // Extra AI fields
        public string? TiradsStage { get; set; }
        public string? ClinicalRecommendation { get; set; }
        public string? RiskLevel { get; set; }
        public string? OverlayImageUrl { get; set; }
        public string? MaskImageUrl { get; set; }
        public string? RoiImageUrl { get; set; }
    }
}
