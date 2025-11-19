using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThyroCareX.Core.Bases;

namespace ThyroCareX.Core.Feature.Doctors.Commands.Models
{
    public class EditDoctorCommand:IRequest<Response<string>>
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Hospital { get; set; }
        public string ImageFile { get; set; }
    }
}
