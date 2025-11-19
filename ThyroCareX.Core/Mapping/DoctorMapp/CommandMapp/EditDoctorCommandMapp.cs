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
        public void EditDoctorCommandMapp()
        {
            // Add your mapping configurations here in the future
            CreateMap<EditDoctorCommand, Doctor>()
            .ForMember(x => x.DoctorID, opt => opt.MapFrom(src => src.Id))
            .ForMember(x => x.ImagePath, opt => opt.MapFrom(src => src.ImageFile));
            
        }
    }
}
