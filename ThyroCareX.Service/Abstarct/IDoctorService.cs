using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThyroCareX.Data.Models;

namespace ThyroCareX.Service.Abstarct
{
    public interface IDoctorService
    {
        Task<List<Doctor>> GetAllDoctorsAsync();
        Task<Doctor>GetDoctorByIdWithIncludeAsync(int id);
        Task <string> EditAsync(Doctor doctor);
        Task<Doctor> GetDoctorByIdAsync(int id);
        Task<bool> IsEmailExistExcludeSelf(string email, int id);
        Task<string> DeleteAsync(Doctor doctor);
        Task<string> AddAsync(Doctor doctor);
    }
}
