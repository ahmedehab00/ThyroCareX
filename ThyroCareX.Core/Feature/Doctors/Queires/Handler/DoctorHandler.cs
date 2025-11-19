using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThyroCareX.Core.Bases;
using ThyroCareX.Core.Feature.Doctors.Queires.Models;
using ThyroCareX.Core.Feature.Doctors.Queires.Result;
using ThyroCareX.Service.Abstarct;

namespace ThyroCareX.Core.Feature.Doctors.Queires.Handler
{
    public class DoctorHandler :ResponseHandler, IRequestHandler<GetDoctorListQuery,Response< List<GetDoctorListResponse>>>,
                                                 IRequestHandler<GetDoctorByIdQuery,Response<GetSingleDoctorResponse>>
    {
        #region Fields
        private readonly IDoctorService _doctorService;
         private readonly IMapper _mapper;
        #endregion
        #region Constructor
        public DoctorHandler(IDoctorService doctorService, IMapper mapper)
        {
            _doctorService = doctorService;
            _mapper = mapper;
        }
        #endregion
        #region Handle Functions
        public async Task<Response<List<GetDoctorListResponse>>> Handle(GetDoctorListQuery request, CancellationToken cancellationToken)
        {
            var doctorList = await _doctorService.GetAllDoctorsAsync();
            var mappedDoctorList = _mapper.Map<List<GetDoctorListResponse>>(doctorList);
            return Success(mappedDoctorList);
        }

        public async Task<Response<GetSingleDoctorResponse>> Handle(GetDoctorByIdQuery request, CancellationToken cancellationToken)
        {
            var Doctor = await _doctorService.GetDoctorByIdWithIncludeAsync(request.Id);
            if(Doctor == null)return NotFound<GetSingleDoctorResponse>("Docotr Not Found");
            var result=_mapper.Map<GetSingleDoctorResponse>(Doctor);
            return Success(result);
        }

        #endregion

    }
}
