using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThyroCareX.Data.Enums;

namespace ThyroCareX.Core.Feature.Doctors.Queires.Result
{
    public class GetDoctorByIdResponse
    {
        public int DoctorID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Specialization { get; set; }
        public string SubscriptionPlanName { get; set; }
        public Gender gender { get; set; }
        public int NationalID { get; set; }
        public int PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Hospital { get; set; }
        public string ImagePath { get; set; }
        public DateTime RegistrationAt { get; set; }
    }
}
