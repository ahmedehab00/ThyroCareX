using System;
using System.Collections.Generic;
using ThyroCareX.Data.Enums;
using ThyroCareX.Data.Models;

namespace ThyroCareX.Core.Feature.Patients.Queries.Result
{
    /// <summary>
    /// Patient data returned to the Syrux mobile app. Read-only, no auth required.
    /// Contains only patient-safe fields — no internal doctor metadata.
    /// </summary>
    public class GetPatientForMobileResponse
    {
        public int PatientID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string GenderLabel => gender == Gender.Male ? "Male" : "Female";
        public Gender gender { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Address { get; set; }
        public DateTime RegistrationAt { get; set; }

        // Physical info
        public double Height { get; set; }
        public double Weight { get; set; }
        public double? BMI => (Height > 0 && Weight > 0)
            ? Math.Round(Weight / Math.Pow(Height / 100.0, 2), 1)
            : null;

        // Medical info
        public string MedicalHistory { get; set; } = string.Empty;
        public string CurrentMedications { get; set; } = string.Empty;
        public string KnownAllergies { get; set; } = string.Empty;

        // Diagnosis results
        public List<MobilePatientTestDto> Tests { get; set; } = new();

        // Summary from latest test
        public string? LatestDiagnosisResult { get; set; }
        public double? LatestConfidence { get; set; }
        public string? LatestClassification { get; set; }
        public string? LatestNextStep { get; set; }
    }

    public class MobilePatientTestDto
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
