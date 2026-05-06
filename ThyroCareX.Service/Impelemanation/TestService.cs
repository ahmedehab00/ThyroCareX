using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThyroCareX.Data.Models;
using ThyroCareX.Infrastructure.Abstarct;
using ThyroCareX.Infrastructure.InfrastructureBases;
using ThyroCareX.Service.Abstarct;

namespace ThyroCareX.Service.Impelemanation
{
    public class TestService : ITestService
    {
        private readonly ITestRepo _testRepository;
        private readonly IGenericRepositoryAsync<DiagnosisResult> _diagnosisRepository;

        public TestService(ITestRepo testRepository, IGenericRepositoryAsync<DiagnosisResult> diagnosisRepository)
        {
            _testRepository = testRepository;
            _diagnosisRepository = diagnosisRepository;
        }

        public async Task<string> AddTestAsync(Test test)
        {
            await _testRepository.AddAsync(test);
            return "Test added successfully.";
        }

        public async Task<Test> GetTestByIdAsync(int id)
        {
            return await _testRepository.GetByIdAsync(id);
        }

        public async Task<Test?> GetTestByIdWithPatientAsync(int id)
        {
            return await _testRepository.GetTestByIdWithPatientAsync(id);
        }

        public async Task SaveDiagnosisAsync(DiagnosisResult diagnosis)
        {
            await _diagnosisRepository.AddAsync(diagnosis);
        }

        public async Task<DiagnosisResult?> GetDiagnosisByTestIdAsync(int testId)
        {
            return _diagnosisRepository.GetTableAsTracking()
                .FirstOrDefault(x => x.TestId == testId);
        }

        public async Task UpdateDiagnosisAsync(DiagnosisResult diagnosis)
        {
            await _diagnosisRepository.UpdateAsync(diagnosis);
        }

        public async Task<string> UpdateTestAsync(Test test)
        {
            await _testRepository.UpdateAsync(test);
            return "Test updated successfully.";
        }

        public async Task<List<Test>> GetTestsByPatientIdAsync(int patientId)
        {
            return await _testRepository.GetTestsByPatientIdWithResultsAsync(patientId);
        }
    }
}
