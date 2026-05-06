using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ThyroCareX.Bases;
using ThyroCareX.Core.Feature.Patients.Queries.Models;
using ThyroCareX.Core.Feature.Patients.Queries.Result;
using ThyroCareX.Core.Feature.TestWithAI.Queries.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace ThyroCareX.Controllers
{
    /// <summary>
    /// Mobile API endpoints for the Syrux patient-facing app.
    /// These endpoints do NOT require authentication — patients use their ID code directly.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("_mobileCors")]
    public class MobileController : AppControllerBase
    {
        /// <summary>
        /// [Syrux App] Lookup a patient by their shared ID code.
        /// The doctor shares this numeric ID with the patient. No login required.
        /// </summary>
        /// <param name="id">The numeric Patient ID shared by the doctor.</param>
        /// <returns>Full patient dashboard data including diagnosis results.</returns>
        [HttpGet("PatientLookup/{id}")]
        [ProducesResponseType(typeof(GetPatientForMobileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PatientLookup([FromRoute] int id)
        {
            // Re-use the existing query — maps to GetPatientByIdResponse
            var patientResult = await Mediator.Send(new GetPatientByIdQuery(id));

            if (!patientResult.Succeeded)
                return NotFound(patientResult);

            var data = patientResult.Data;

            // Map to mobile-safe DTO
            var latestTest = data.Tests?.OrderByDescending(t => t.CreatedAt).FirstOrDefault();

            var testHistoryResponse = await Mediator.Send(new GetPatientTestHistoryQuery(id));

            var mobileResponse = new GetPatientForMobileResponse
            {
                PatientID          = data.PatientID,
                FullName           = data.FullName,
                Age                = data.Age,
                gender             = data.gender,
                PhoneNumber        = data.PhoneNumber,
                Address            = data.Address,
                RegistrationAt     = data.RegistrationAt,
                Height             = data.Height,
                Weight             = data.Weight,
                MedicalHistory     = data.MedicalHistory,
                CurrentMedications = data.CurrentMedications,
                KnownAllergies     = data.KnownAllergies,
                Tests = data.Tests?.Select(t => new MobilePatientTestDto
                {
                    TestId         = t.TestId,
                    ImagePath      = t.ImagePath,
                    CreatedAt      = t.CreatedAt,
                    DiagnosisResult = t.DiagnosisResult,
                    Confidence     = t.Confidence,
                    Classification = t.Classification,
                    BethesdaLabel  = t.BethesdaLabel,
                    NextStep       = t.NextStep,
                    TSH = t.TSH,
                    T3 = t.T3,
                    TT4 = t.TT4,
                    FTI = t.FTI,
                    T4U = t.T4U,
                    NodulePresent = t.NodulePresent,
                    OnThyroxine = t.OnThyroxine,
                    ThyroidSurgery = t.ThyroidSurgery,
                    QueryHyperthyroid = t.QueryHyperthyroid,
                    TiradsStage = t.TiradsStage,
                    ClinicalRecommendation = t.ClinicalRecommendation,
                    RiskLevel = t.RiskLevel,
                    OverlayImageUrl = t.OverlayImageUrl,
                    MaskImageUrl = t.MaskImageUrl,
                    RoiImageUrl = t.RoiImageUrl
                }).ToList() ?? new(),
                LatestDiagnosisResult = latestTest?.DiagnosisResult,
                LatestConfidence      = latestTest?.Confidence,
                LatestClassification  = latestTest?.Classification,
                LatestNextStep        = latestTest?.NextStep,
            };

            return Ok(new { succeeded = true, data = mobileResponse });
        }
    }
}
