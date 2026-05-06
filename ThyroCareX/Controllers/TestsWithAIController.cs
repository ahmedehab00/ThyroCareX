using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThyroCareX.Bases;
using ThyroCareX.Core.Feature.TestWithAI.Commands.Models;
using ThyroCareX.Core.Feature.TestWithAI.Queries.Models;
using ThyroCareX.Data.Healpers.ClinicalAI;

namespace ThyroCareX.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Doctor")]
    public class TestsWithAIController : AppControllerBase
    {
        /// <summary>
        /// Analyzes a thyroid ultrasound image for nodule segmentation and risk assessment.
        /// </summary>
        /// <param name="command">Command containing the image and test ID.</param>
        /// <returns>Diagnosis results including segmented images and risk probabilities.</returns>
        [HttpPost("ProcessImage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ProcessImage([FromForm] PredictImageCommand command)
        {
            var response = await Mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// Evaluates thyroid clinical laboratory results (TSH, T3, T4, etc.) using AI.
        /// </summary>
        /// <param name="request">Clinical metrics including biomarkers and symptoms.</param>
        /// <returns>Functional status classification and clinical recommendations.</returns>
        [HttpPost("ProcessClinical")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ProcessClinical([FromBody] ClinicalRequest request)
        {
            var response = await Mediator.Send(new AssessClinicalCommand(request));
            return Ok(response);
        }

        /// <summary>
        /// Analyzes FNAC (Fine Needle Aspiration Cytology) reports to determine Bethesda category and malignancy risk.
        /// </summary>
        /// <param name="command">Command containing the Bethesda report data.</param>
        /// <returns>Malignancy risk assessment based on Bethesda classification.</returns>
        [HttpPost("ProcessFnac")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ProcessFnac([FromForm]PredictFnacCommand command)
        {
            var response = await Mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// Retrieves the full diagnostic history (Clinical + Imaging) for a specific patient.
        /// </summary>
        /// <param name="patientId">The unique ID of the patient.</param>
        /// <returns>List of historical diagnostic sessions.</returns>
        [HttpGet("GetPatientTestHistory/{patientId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPatientTestHistory([FromRoute] int patientId)
        {
            var response = await Mediator.Send(new GetPatientTestHistoryQuery(patientId));
            return Ok(response);
        }

        /// <summary>
        /// Validates if the uploaded image is a medical ultrasound.
        /// </summary>
        /// <param name="command">Command containing the image file.</param>
        /// <returns>Validation result (true if valid ultrasound).</returns>
        [HttpPost("ValidateImage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ValidateImage([FromForm] ValidateImageCommand command)
        {
            var response = await Mediator.Send(command);
            return Ok(response);
        }
    }
}
