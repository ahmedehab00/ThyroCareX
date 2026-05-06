using MediatR;
using ThyroCareX.Core.Bases;
using ThyroCareX.Core.Dto;
using ThyroCareX.Data.Healpers.ClinicalAI;
using ThyroCareX.Data.Healpers.ClinicalAIResponse;

namespace ThyroCareX.Core.Feature.TestWithAI.Commands.Models
{
    public class AssessClinicalCommand : IRequest<Response<AssessClinicalResponse>>
    {
        public ClinicalRequest ClinicalRequest { get; set; }
        public AssessClinicalCommand(ClinicalRequest clinicalRequest)
        {
            ClinicalRequest = clinicalRequest;

        }
    }
}
