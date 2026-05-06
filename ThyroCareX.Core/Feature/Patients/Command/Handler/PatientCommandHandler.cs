using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThyroCareX.Core.Bases;
using ThyroCareX.Core.Feature.Patients.Command.Model;
using ThyroCareX.Data.Models;
using ThyroCareX.Service.Abstarct;

namespace ThyroCareX.Core.Feature.Patients.Command.Handler
{
    public class PatientCommandHandler : ResponseHandler, IRequestHandler<AddPatientCommand, Response<int>>
                                                      , IRequestHandler<EditPatientCommand, Response<string>>
    {
        #region Fields
        private readonly IPatientService _patientService;
        private readonly IDoctorService _doctorService;
        private readonly IUserContextService _userContextService;

        private readonly IMapper _mapper;
        #endregion
        #region Constructor
        public PatientCommandHandler(IPatientService patientService, IUserContextService userContextService,IMapper mapper, IDoctorService doctorService)
        {

            _patientService = patientService;
            _doctorService = doctorService;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        #endregion
        #region Handle Functions
        public async Task<Response<int>> Handle(AddPatientCommand request, CancellationToken cancellationToken)
        {
            var userIdString = _userContextService.UserId;

            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized<int>("Unauthorized");

            if (!int.TryParse(userIdString, out var userId))
                return BadRequest<int>("Invalid UserId");

            var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
            var patient = _mapper.Map<Patient>(request);
            patient.DoctorID= doctor.DoctorID;
            patient.CreatedAt = DateTime.UtcNow;
            if (request.Age > 0)
            {
                // Admission uses Age only; store an approximate DOB so dashboards can compute age consistently.
                patient.DateOfBirth = DateTime.UtcNow.Date.AddYears(-request.Age);
            }


            var result = await _patientService.AddAsync(patient);
            return Success(patient.Id);






        }

        public async Task<Response<string>> Handle(EditPatientCommand request, CancellationToken cancellationToken)
        {
            var patient = await _patientService.GetPatientByIdAsync(request.PatientID);
            if (patient == null)
            {
                return NotFound<string>("Patient not found");
            }

            // Map the Requst to Patient entity
            var PatientMapper = _mapper.Map<Patient>(request);
            // Call the Service to Update Patient
            var result = await _patientService.EditAsync(PatientMapper);
            // Return Response
            return Success(result);
        }

        #endregion
    }
}
