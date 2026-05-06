using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ThyroCareX.Core.Bases;
using ThyroCareX.Core.Feature.TestWithAI.Queries.Models;
using ThyroCareX.Data.Models;
using ThyroCareX.Service.Abstarct;

namespace ThyroCareX.Core.Feature.TestWithAI.Queries.Handler
{
    public class GetPatientTestHistoryHandler : ResponseHandler, IRequestHandler<GetPatientTestHistoryQuery, Response<List<Test>>>
    {
        private readonly ITestService _testService;

        public GetPatientTestHistoryHandler(ITestService testService)
        {
            _testService = testService;
        }

        public async Task<Response<List<Test>>> Handle(GetPatientTestHistoryQuery request, CancellationToken cancellationToken)
        {
            var tests = await _testService.GetTestsByPatientIdAsync(request.PatientId);
            return Success(tests);
        }
    }
}
