using MediatR;
using Microsoft.AspNetCore.Http;
using ThyroCareX.Core.Bases;

namespace ThyroCareX.Core.Feature.TestWithAI.Commands.Models
{
    public class ValidateImageCommand : IRequest<Response<List<ThyroCareX.Service.Impelemanation.UltrasoundValidationResponse>>>
    {
        public List<IFormFile> ImageFiles { get; set; }
    }
}
