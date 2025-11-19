using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThyroCareX.Bases;
using ThyroCareX.Core.Feature.Doctors.Commands.Models;
using ThyroCareX.Core.Feature.Doctors.Queires.Models;

namespace ThyroCareX.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : AppControllerBase
    {
        

        [HttpGet("DisplayAllDoctors")]
        public async Task<IActionResult> DisplayAllDoctors()
        {
            var Response = await Mediator.Send(new GetDoctorListQuery());
            return Ok(Response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingleDoctorById([FromRoute]int id) 
        {
            var Response=await Mediator.Send(new GetDoctorByIdQuery(id));
            return Ok(Response);
        }
        [HttpPost("AddDoctor")]
        public async Task<IActionResult> AddDoctor([FromBody] AddDoctorCommand command)
        {
            var Response = await Mediator.Send(command);
            return Ok(Response);
        }
        [HttpPut("EditDoctor")]
        public async Task<IActionResult> EditDoctor([FromBody] EditDoctorCommand command)
        {
            var Response = await Mediator.Send(command);
            return Ok(Response);
        }
        [HttpDelete("DeleteDoctor/{id}")]
        public async Task<IActionResult> DeleteDoctor([FromRoute] int id)
        {
            var Response = await Mediator.Send(new DeleteDoctorCommand(id));
            return Ok(Response);
        }
    }
}
