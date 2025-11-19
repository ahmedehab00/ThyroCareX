using MediatR;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThyroCareX.Core.Bases;

namespace ThyroCareX.Core.Feature.Doctors.Commands.Models
{
    public class AddDoctorCommand: IRequest<Response<string>>
    {
        
        public string FullName { get; set; }
        public string Email { get; set; }
        
    }
}
