using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ThyroCareX.Data.Models;
using ThyroCareX.Infrastructure.Abstarct;
using ThyroCareX.Service.Abstarct;

namespace ThyroCareX.Service.Impelemanation
{
    public class DoctorService : IDoctorService
    {
        #region Prop
        private readonly IDoctorRepository _doctorRepository;    
        #endregion
        #region Constructor
        public DoctorService(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        #endregion
        #region Handle Functions

        public async Task<List<Doctor>> GetAllDoctorsAsync()
        {
          return await  _doctorRepository.GetAllDoctorAsync();
        }

        public async Task<Doctor> GetDoctorByIdWithIncludeAsync(int id)
        {
            var Doctor=await _doctorRepository.GetTableNoTracking()
                .Include(x=>x.SubscriptionPlan).Where(x=>x.DoctorID.Equals(id)).FirstOrDefaultAsync();

            return Doctor;
        }
        public async Task<string> EditAsync(Doctor doctor)
        {
            await _doctorRepository.UpdateAsync(doctor);
            return "Doctor Updated Successfully";
        }
        public async Task<bool> IsEmailExistExcludeSelf(string email, int id)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return await _doctorRepository
                .GetTableNoTracking()
                .AnyAsync(x => x.Email == email && x.DoctorID != id); 
        }

        public async Task<Doctor> GetDoctorByIdAsync(int id)
        {
          var doctor= await _doctorRepository.GetByIdAsync(id);
            return doctor;
        }

        public async Task<string> DeleteAsync(Doctor doctor)
        {
            var trans = _doctorRepository.BeginTransaction();
            try
            {
                await _doctorRepository.DeleteAsync(doctor);
                await trans.CommitAsync();
                return "Success";
            }
            catch
            {
                await trans.RollbackAsync();
                return "Failed";
            }
        }

        public async Task<string> AddAsync(Doctor doctor)
        {
            await _doctorRepository.AddAsync(doctor);
            return "Doctor Added Successfully";
        }


            #endregion

        }
}
