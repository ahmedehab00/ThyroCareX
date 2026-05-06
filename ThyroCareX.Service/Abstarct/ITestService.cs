using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThyroCareX.Data.Models;

namespace ThyroCareX.Service.Abstarct
{
    public interface ITestService
    {
        Task<string> AddTestAsync(Test test);
        Task<string> UpdateTestAsync(Test test);
        Task<Test> GetTestByIdAsync(int id);
        Task<Test?> GetTestByIdWithPatientAsync(int id);
        Task SaveDiagnosisAsync(DiagnosisResult diagnosis);
        Task<DiagnosisResult?> GetDiagnosisByTestIdAsync(int testId);
        Task UpdateDiagnosisAsync(DiagnosisResult diagnosis);
        Task<List<Test>> GetTestsByPatientIdAsync(int patientId);
    }
}
