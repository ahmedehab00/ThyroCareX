using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThyroCareX.Data.Models;
using ThyroCareX.Infrastructure.Abstarct;
using ThyroCareX.Infrastructure.Context;
using ThyroCareX.Infrastructure.InfrastructureBases;

namespace ThyroCareX.Infrastructure.Repository
{
    public class TestRepo: GenericRepositoryAsync<Test>, ITestRepo
    {
        private readonly DbSet<Test> _tests;
        public TestRepo(ApplicationDbContext dbContext) : base(dbContext)
        {
            _tests = dbContext.Set<Test>();
        }

        public async Task<Test?> GetTestByIdWithPatientAsync(int id)
        {
            return await _tests
                .Include(t => t.Patient)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Test>> GetTestsByPatientIdWithResultsAsync(int patientId)
        {
            return await _tests
                .Include(t => t.DiagnosisResult)
                .Where(t => t.PatientId == patientId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    }
}
