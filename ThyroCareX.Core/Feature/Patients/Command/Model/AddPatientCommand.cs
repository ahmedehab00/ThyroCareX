using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThyroCareX.Core.Bases;
using ThyroCareX.Core.Dto;
using ThyroCareX.Data.Enums;

namespace ThyroCareX.Core.Feature.Patients.Command.Model
{
    public class AddPatientCommand:IRequest<Response<int>>
    {
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public Gender Gender { get;  set; }

        public double Height { get;  set; }
        public double Weight { get;  set; }

        public string PhoneNumber { get;  set; }
        public string? Address { get;  set; }
        public string MedicalHistory { get;  set; } = string.Empty;
        public string CurrentMedications { get;  set; } = string.Empty;
        public string KnownAllergies { get;  set; } = string.Empty;
        //public string AttachmentPath { get; set; } = string.Empty;
    }
}
