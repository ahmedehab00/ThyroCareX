using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThyroCareX.Core.Bases;
using ThyroCareX.Data.Enums;

namespace ThyroCareX.Core.Feature.Patients.Command.Model
{
    public class EditPatientCommand:IRequest<Response<string>>
    {
        public int PatientID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Gender Gender { get; set; }
        public int Age { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public int DoctorID { get; set; }
    }
}
