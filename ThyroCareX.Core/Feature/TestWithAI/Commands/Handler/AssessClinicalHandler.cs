using AutoMapper;
using MediatR;
using ThyroCareX.Core.Bases;
using ThyroCareX.Core.Dto;
using ThyroCareX.Core.Feature.TestWithAI.Commands.Models;
using ThyroCareX.Core.Feature.TestWithAI.Dto;
using ThyroCareX.Data.Enums;
using ThyroCareX.Data.Healpers.ClinicalAI;
using ThyroCareX.Data.Healpers.ClinicalAIResponse;
using ThyroCareX.Data.Models;
using ThyroCareX.Service.Abstarct;
using ThyroCareX.Service.Impelemanation;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ThyroCareX.Core.Feature.TestWithAI.Commands.Handler
{
    public class AssessClinicalHandler : ResponseHandler, IRequestHandler<AssessClinicalCommand, Response<AssessClinicalResponse>>
    {
        private readonly ITestService _testService;
        private readonly IAIService _aiService;
        private readonly IUserContextService _userContextService;
        private readonly IDoctorService _doctorService;
        private readonly IPatientService _PatientService;
        private readonly IMapper _mapper;
        

        public AssessClinicalHandler(ITestService testService, IMapper mapper, IPatientService PatientService, IDoctorService doctorService, IAIService aiService,IUserContextService userContextService)
        {
            _testService = testService;
            _aiService = aiService;
            _userContextService = userContextService;
            _doctorService = doctorService;
            _PatientService = PatientService;
            _mapper = mapper;

        }

        public async Task<Response<AssessClinicalResponse>> Handle(AssessClinicalCommand request, CancellationToken cancellationToken)
        {
            var userIdString = _userContextService.UserId;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
                return Unauthorized<AssessClinicalResponse>("Unauthorized");

            var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);



            // Normalize inputs for AI (ensure 0 or 1)
            request.ClinicalRequest.OnThyroxine = request.ClinicalRequest.OnThyroxine > 0 ? 1 : 0;
            request.ClinicalRequest.ThyroidSurgery = request.ClinicalRequest.ThyroidSurgery > 0 ? 1 : 0;
            request.ClinicalRequest.QueryHyperthyroid = request.ClinicalRequest.QueryHyperthyroid > 0 ? 1 : 0;

            var test = new Test
            {
                PatientId = request.ClinicalRequest.PatientId,
                OnThyroxine = request.ClinicalRequest.OnThyroxine,
                ThyroidSurgery = request.ClinicalRequest.ThyroidSurgery,
                QueryHyperthyroid = request.ClinicalRequest.QueryHyperthyroid,
                TSH = request.ClinicalRequest.TSH,
                T3 = request.ClinicalRequest.T3,
                TT4 = request.ClinicalRequest.TT4,
                FTI = request.ClinicalRequest.FTI,
                T4U = request.ClinicalRequest.T4U,
                NodulePresent = request.ClinicalRequest.NodulePresent,
                DoctorId = doctor.DoctorID
            };
            test.Status = TestStatus.Processing;

            await _testService.AddTestAsync(test);

            try 
            {
                // 🧠 1. Call AI
                var aiResponse = await _aiService.AssessClinicalAsync(request.ClinicalRequest);

                // 💾 2. Save Diagnosis
                var diagnosis = new DiagnosisResult
                {
                    TestId = test.Id,
                    FunctionalStatus = aiResponse.FunctionalStatus,
                    RiskLevel = aiResponse.RiskLevel,
                    ClinicalRecommendation = aiResponse.ClinicalRecommendation,
                    NextStep = aiResponse.NextStep,
                    RawResponse = System.Text.Json.JsonSerializer.Serialize(aiResponse)
                };

                await _testService.SaveDiagnosisAsync(diagnosis);

                test.Status = TestStatus.Completed;
                await _testService.UpdateTestAsync(test);
                
                // 🎯 Mapping Response
                var response = new AssessClinicalResponse
                {
                    TestId = test.Id,
                    Status = aiResponse.Status,
                    Clinical = new ClinicalResultDto
                    {
                        FunctionalStatus = aiResponse.FunctionalStatus,
                        RiskLevel = aiResponse.RiskLevel,
                        ClinicalRecommendation = aiResponse.ClinicalRecommendation,
                        AiRecommendation = aiResponse.AiRecommendation,
                        ModelConfidence = aiResponse.ModelConfidence,
                        NeedsManualReview = aiResponse.NeedsManualReview,
                        NextStep = aiResponse.NextStep,
                        Probabilities = aiResponse.Probabilities
                    }
                };

                return Success(response);
            }
            catch (Exception ex)
            {
                test.Status = TestStatus.Failed;
                await _testService.UpdateTestAsync(test);
                return BadRequest<AssessClinicalResponse>($"AI Service Error: {ex.Message}");
            }
        }
       

    }
}
