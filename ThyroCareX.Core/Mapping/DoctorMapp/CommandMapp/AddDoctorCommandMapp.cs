using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThyroCareX.Core.Feature.Doctors.Commands.Models;
using ThyroCareX.Data.Models;

namespace ThyroCareX.Core.Mapping.DoctorMapp
{
    public partial class DoctorProfiles
    {
        public void AddDoctorCommandMapp()
        {
            // Add your mapping configurations here in the future
            CreateMap<AddDoctorCommand, Doctor>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));


        }
    }
}
