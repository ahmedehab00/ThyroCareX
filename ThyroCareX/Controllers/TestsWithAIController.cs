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
        /// <summary>
        /// Compares two diagnostic tests and provides a trend analysis.
        /// </summary>
        /// <param name="testId1">First test ID.</param>
        /// <param name="testId2">Second test ID.</param>
        /// <returns>Comparison result including biomarker trends and overall status.</returns>
        [HttpGet("CompareTests/{testId1}/{testId2}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CompareTests([FromRoute] int testId1, [FromRoute] int testId2)
        {
            var response = await Mediator.Send(new CompareTestsQuery(testId1, testId2));
            return Ok(response);
        }

        /// <summary>
        /// Fetches the AI diagnostic image through the backend to securely pass the AI API key.
        /// </summary>
        /// <param name="imageId">Image ID from the AI service.</param>
        /// <returns>The diagnostic image stream.</returns>
        [HttpGet("ViewImage/{imageId}")]
        [AllowAnonymous]
        public async Task<IActionResult> ViewImage(string imageId)
        {
            try
            {
                var httpClient = new HttpClient();
                var config = HttpContext.RequestServices.GetService<Microsoft.Extensions.Configuration.IConfiguration>();
                var apiKey = config?["AISettings:ApiKey"];
                httpClient.DefaultRequestHeaders.Add("X-AI-Service-Key", apiKey);

                var response = await httpClient.GetAsync($"https://amer003100-thyraxcdss.hf.space/image/view/{imageId}");
                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Failed to retrieve image from AI service");
                }

                var stream = await response.Content.ReadAsStreamAsync();
                var contentType = response.Content.Headers.ContentType?.ToString() ?? "image/jpeg";
                return File(stream, contentType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
