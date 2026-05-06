using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThyroCareX.Bases;
using ThyroCareX.Core.Feature.Patients.Command.Model;
using ThyroCareX.Core.Feature.Patients.Queries.Models;

namespace ThyroCareX.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : AppControllerBase
    {
        /// <summary>
        /// Retrieves a list of all patients in the system.
        /// </summary>
        /// <returns>Collection of patient summaries.</returns>
        [HttpGet("DisplayAllPatients")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DisplayAllPatients()
        {
            var Response = await Mediator.Send(new GetPatientListQuery());
            return Ok(Response);
        }

        /// <summary>
        /// Retrieves patients assigned to a specific doctor.
        /// </summary>
        /// <param name="doctorId">Unique ID of the doctor.</param>
        /// <returns>List of patients managed by the doctor.</returns>
        [HttpGet("DisplayPatientsByDoctor/{doctorId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DisplayPatientsByDoctor([FromRoute] int doctorId)
        {
            var Response = await Mediator.Send(new GetPatientListQueryByDoctor (doctorId));
            return Ok(Response);
        }

        /// <summary>
        /// Retrieves detailed information for a single patient by ID.
        /// </summary>
        /// <param name="id">Unique ID of the patient.</param>
        /// <returns>Full patient profile data.</returns>
        [HttpGet("DisplayPatientById/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DisplayPatientById([FromRoute] int id)
        {
            var Response = await Mediator.Send(new GetPatientByIdQuery(id));
            return Ok(Response);
        }

        /// <summary>
        /// Registers a new patient with optional ultrasound imaging.
        /// </summary>
        /// <param name="command">Command containing patient profile and imaging data.</param>
        /// <returns>ID of the newly created patient.</returns>
        [Authorize(Roles = "Doctor")]
        [HttpPost("AddPatient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddPatient([FromForm]AddPatientCommand command)
        {
            var Response = await Mediator.Send(command);
            return Ok(Response);
        }

        /// <summary>
        /// Updates an existing patient's profile information.
        /// </summary>
        /// <param name="Id">ID of the patient to edit.</param>
        /// <param name="command">Command with updated profile details.</param>
        /// <returns>Success message upon update.</returns>
        [HttpPut("EditPatient/{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EditPatient([FromRoute] int Id, [FromBody] EditPatientCommand command)
        {
            command.PatientID = Id;
            var Response = await Mediator.Send(command);
            return Ok(Response);
        }

        /// <summary>
        /// Deletes a patient and all their associated medical data from the system.
        /// </summary>
        /// <param name="Id">ID of the patient to remove.</param>
        /// <returns>Success message upon deletion.</returns>
        [HttpDelete("DeletePatient/{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePatient([FromRoute] int Id)
        {
            var Response = await Mediator.Send(new DeletePatientCommand(Id));
            return Ok(Response);
        }
    }
}
