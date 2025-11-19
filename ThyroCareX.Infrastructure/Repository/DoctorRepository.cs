using Microsoft.EntityFrameworkCore;
using ThyroCareX.Data.Models;
using ThyroCareX.Infrastructure.Abstarct;
using ThyroCareX.Infrastructure.Context;
using ThyroCareX.Infrastructure.InfrastructureBases;

namespace ThyroCareX.Infrastructure.Repository
{
    public class DoctorRepository: GenericRepositoryAsync<Doctor>, IDoctorRepository
    {
        #region Prop
        public readonly DbSet<Doctor> _doctors;
        #endregion

        #region Constructor
        public DoctorRepository(ApplicationDbContext dbcontext) : base(dbcontext)
        {
            _doctors = dbcontext.Set<Doctor>();

        }
        #endregion

        #region Handle Functions
        public async Task<List<Doctor>> GetAllDoctorAsync()
        {
            return await _doctors.ToListAsync();
        }
        #endregion
    }
}
