using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThyroCareX.Core.Bases;
using ThyroCareX.Core.Feature.Doctors.Queires.Result;

namespace ThyroCareX.Core.Feature.Doctors.Queires.Models
{
    public class GetDoctorListQuery:IRequest<Response<List<GetDoctorListResponse>>>
    {
    }
}
