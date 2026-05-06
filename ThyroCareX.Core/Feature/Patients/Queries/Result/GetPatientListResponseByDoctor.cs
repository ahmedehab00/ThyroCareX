using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThyroCareX.Core.Feature.Patients.Queries.Result
{
    public class GetPatientListResponseByDoctor
    {
        public int PatientID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        // Diagnostic Summary
        public string LatestStatus { get; set; }
        public string CancerClassification { get; set; }
        public string NextStep { get; set; }
        public double? RiskConfidence { get; set; }
    }
}
