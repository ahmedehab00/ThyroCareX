using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThyroCareX.Core.Bases;
using ThyroCareX.Core.Feature.Patients.Queries.Models;
using ThyroCareX.Core.Feature.Patients.Queries.Result;
using ThyroCareX.Service.Abstarct;
using ThyroCareX.Core.Feature.Patients.Command.Model;

namespace ThyroCareX.Core.Feature.Patients.Queries.Handler
{
    public class PatientHandler:ResponseHandler
                                          ,IRequestHandler<GetPatientListQuery,Response<List<GetPatientListResponse>> >
                                          ,IRequestHandler<GetPatientListQueryByDoctor,Response<List<GetPatientListResponseByDoctor>>>
                                          ,IRequestHandler<GetPatientByIdQuery,Response<GetPatientByIdResponse>>
                                          ,IRequestHandler<DeletePatientCommand,Response<string>>
    {

        #region Fields
        private readonly IPatientService _patientService;
        private readonly IMapper _mapper;
        #endregion
        #region Constructor
        public PatientHandler(IPatientService patientService, IMapper mapper)
        {
            _patientService = patientService;
            _mapper = mapper;

        }

        #endregion
        #region Handle Functions
        public async Task<Response<List<GetPatientListResponse>>> Handle(GetPatientListQuery request, CancellationToken cancellationToken)
        {
           var patientList=await _patientService.GetAllPatientsIncudelWithDoctorAsync();
            if(patientList==null || patientList.Count==0)
            {
                return NotFound<List<GetPatientListResponse>>("No Patients Found");
            }

            var mappedPatientList=_mapper.Map<List<GetPatientListResponse>>(patientList);
            return Success(mappedPatientList);
        }

        public async Task<Response<List<GetPatientListResponseByDoctor>>> Handle(GetPatientListQueryByDoctor request, CancellationToken cancellationToken)
        {
            //Fetch patients by DoctorID
            var patientListByDoctor =await _patientService.GetAllPatientsByDoctorAsync(request.DoctorID);
            //Check if any patients found
            if (patientListByDoctor == null || patientListByDoctor.Count == 0)
            {
                return NotFound<List<GetPatientListResponseByDoctor>>("No Patients Found");
            }
            //Map the patient list to the response model
            var mappedPatientListByDoctor = _mapper.Map<List<GetPatientListResponseByDoctor>>(patientListByDoctor);
            //Return the successful response with mapped data
            return Success(mappedPatientListByDoctor);

        }

        public async Task<Response<GetPatientByIdResponse>> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
        {
            //Fetch patient by ID
            var patient =await _patientService.GetPatientByIdAsync(request.PatientID);
            // Check if patient exists
            if (patient==null)
            {
                return NotFound<GetPatientByIdResponse>($"No Patient Found with ID: {request.PatientID}");
            }
            //Map the patient to the response model
            var mappedPatient =_mapper.Map<GetPatientByIdResponse>(patient);
            //Return the successful response with mapped data
            return Success(mappedPatient);
        }

        public async Task<Response<string>> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
        {
            var patient = await _patientService.GetPatientByIdAsync(request.PatientID);
            if (patient == null)
            {
                return NotFound<string>("Patient not found");
            }

            var result = await _patientService.DeleteAsync(patient);
            return Success(result);
        }

        #endregion
    }
}
