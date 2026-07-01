using MediatR;
using Microsoft.AspNetCore.Http;
using ThyroCareX.Core.Bases;
using ThyroCareX.Core.Dto.FnacAIResponse;

namespace ThyroCareX.Core.Feature.TestWithAI.Commands.Models
{
    public class PredictFnacCommand : IRequest<Response<List<FnacAIResponse>>>
    {
        public int TestId { get; set; }
        public string? SessionId { get; set; }
        public IEnumerable<IFormFile> FnacImages { get; set; }
    }
}
