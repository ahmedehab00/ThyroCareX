using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using ThyroCareX.Core.Bases;
using ThyroCareX.Core.Dto.ImageAIResponse;

namespace ThyroCareX.Core.Feature.TestWithAI.Commands.Models
{
    public class PredictImageCommand : IRequest<Response<ImageAIResponse>>
    {
        public int TestId { get; set; }
       public IFormFile UltraSoundImage { get; set;  }
    }
}
