using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThyroCareX.Data.Models;
using ThyroCareX.Infrastructure.Abstarct;
using ThyroCareX.Service.Abstarct;

namespace ThyroCareX.Service.Impelemanation
{
    public class PatientService : IPatientService
    {
        #region Prop
        private readonly IPatientRepository _patientRepository;
        #endregion
        #region Constructor
        public PatientService(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        #endregion
        #region Handle Functions
        public Task<List<Patient>> GetAllPatientsIncudelWithDoctorAsync()
        {
            var patients = _patientRepository.GetTableNoTracking()
                .Include(p => p.Doctor)
                .Include(p => p.Tests)
                    .ThenInclude(t => t.DiagnosisResult)
                .ToListAsync();
            return patients;

        }
        public async Task<List<Patient>> GetAllPatientsByDoctorAsync(int doctorId)
        {
            return await _patientRepository.GetTableNoTracking()
                 .Where(p => p.DoctorID == doctorId)
                 .Include(p => p.Tests)
                    .ThenInclude(t => t.DiagnosisResult)
                 .ToListAsync();

        }
        public async Task<Patient> GetPatientByIdAsync(int id)
        {
            var patient = await _patientRepository.GetTableNoTracking()
                .Where(x => x.Id == id)
                .Include(p => p.Tests)
                    .ThenInclude(t => t.DiagnosisResult)
                .FirstOrDefaultAsync();
            return patient;
        }
        public async Task<string> AddAsync(Patient patient)
        {
            await _patientRepository.AddAsync(patient);
            return "Patient Added Successfully";
        }

        public async Task<string> EditAsync(Patient patient)
        {
            await _patientRepository.UpdateAsync(patient);
            return "Patient Updated Successfully";
        }

        public async Task<string> DeleteAsync(Patient patient)
        {
            await _patientRepository.DeleteAsync(patient);
            return "Patient Deleted Successfully";

        }
       

        #endregion
    }
}
