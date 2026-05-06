using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThyroCareX.Data.Models;
using ThyroCareX.Infrastructure.InfrastructureBases;

namespace ThyroCareX.Infrastructure.Abstarct
{
    public interface ITestRepo: IGenericRepositoryAsync<Test>
    {
        Task<Test?> GetTestByIdWithPatientAsync(int id);
        Task<List<Test>> GetTestsByPatientIdWithResultsAsync(int patientId);
    }
}
