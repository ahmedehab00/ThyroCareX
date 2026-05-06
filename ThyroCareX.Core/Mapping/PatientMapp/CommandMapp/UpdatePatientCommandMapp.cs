using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThyroCareX.Core.Feature.Patients.Command.Model;
using ThyroCareX.Data.Models;

namespace ThyroCareX.Core.Mapping.PatientMapp
{
    public partial class PatientProfile
    {
        public void UpdatePatientCommandMapp() 
        {
            CreateMap<EditPatientCommand, Patient>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PatientID))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => DateTime.UtcNow.Date.AddYears(-src.Age)));
        }
    }
}
