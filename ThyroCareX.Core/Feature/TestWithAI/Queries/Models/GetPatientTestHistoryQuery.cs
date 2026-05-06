using MediatR;
using System.Collections.Generic;
using ThyroCareX.Core.Bases;
using ThyroCareX.Data.Models;

namespace ThyroCareX.Core.Feature.TestWithAI.Queries.Models
{
    public class GetPatientTestHistoryQuery : IRequest<Response<List<Test>>>
    {
        public int PatientId { get; set; }

        public GetPatientTestHistoryQuery(int patientId)
        {
            PatientId = patientId;
        }
    }
}
