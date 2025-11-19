using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThyroCareX.Core.Bases;

namespace ThyroCareX.Core.Feature.Doctors.Commands.Models
{
    public class DeleteDoctorCommand:IRequest<Response<string>>
    {
        public int Id { get; set; }
        public DeleteDoctorCommand(int id)
        {

            Id = id;
        }
    }
}
