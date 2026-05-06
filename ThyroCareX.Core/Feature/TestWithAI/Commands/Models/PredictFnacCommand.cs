using MediatR;
using Microsoft.AspNetCore.Http;
using ThyroCareX.Core.Bases;
using ThyroCareX.Core.Dto.FnacAIResponse;

namespace ThyroCareX.Core.Feature.TestWithAI.Commands.Models
{
    public class PredictFnacCommand : IRequest<Response<FnacAIResponse>>
    {
        public int TestId { get; set; }
        public IFormFile FANC_IMAGE { get; set; }

    }
}
