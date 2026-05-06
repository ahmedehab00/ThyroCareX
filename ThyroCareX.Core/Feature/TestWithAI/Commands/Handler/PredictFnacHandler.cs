using MediatR;
using System.Text.Json;
using ThyroCareX.Core.Bases;
using ThyroCareX.Core.Dto.FnacAIResponse;
using ThyroCareX.Core.Dto.ImageAIResponse;
using ThyroCareX.Core.Feature.TestWithAI.Commands.Models;
using ThyroCareX.Data.Enums;
using ThyroCareX.Data.Models;
using ThyroCareX.Service.Abstarct;

namespace ThyroCareX.Core.Feature.TestWithAI.Commands.Handler
{
    public class PredictFnacHandler : ResponseHandler, IRequestHandler<PredictFnacCommand, Response<FnacAIResponse>>
    {
        private readonly ITestService _testService;
        private readonly IAIService _aiService;
        private readonly IImageService _imageService;

        public PredictFnacHandler(
            ITestService testService,
            IAIService aiService,
            IImageService imageService)
        {
            _testService = testService;
            _aiService = aiService;
            _imageService = imageService;
        }

        public async Task<Response<FnacAIResponse>> Handle(PredictFnacCommand request, CancellationToken cancellationToken)
        {
            // ✅ 1. Get Test
            var test = await _testService.GetTestByIdAsync(request.TestId);
            if (test == null)
                return NotFound<FnacAIResponse>("Test not found");

            // ✅ 2. Upload Image
            if (request.FANC_IMAGE == null)
                return BadRequest<FnacAIResponse>("FNAC image is required");

            test.FnacImagePath = await _imageService.UploadFileAsync(request.FANC_IMAGE);

            test.Status = TestStatus.Processing;
            await _testService.UpdateTestAsync(test);

            

            // ✅ 3. Call AI
            var aiResponse = await _aiService.PredictFnacAsync(test.FnacImagePath);

            if (aiResponse == null || aiResponse.Status != "success")
            {
                test.Status = TestStatus.Failed;
                await _testService.UpdateTestAsync(test);

                return BadRequest<FnacAIResponse>("AI Service failed to process FNAC image");
            }

            // ✅ 4. Save / Update Diagnosis
            var diagnosis = await _testService.GetDiagnosisByTestIdAsync(request.TestId);

            if (diagnosis == null)
            {
                diagnosis = new DiagnosisResult
                {
                    TestId = request.TestId
                };
            }

            // 🧠 FNAC DATA
            diagnosis.BethesdaCategory = aiResponse.Classification?.BethesdaCategory;
            diagnosis.BethesdaLabel = aiResponse.Classification?.BethesdaLabel;
            diagnosis.MalignancyRisk = aiResponse.Classification?.MalignancyRisk;
            diagnosis.FnacRecommendation = aiResponse.Classification?.Recommendation;

            // 🔥 مهم جدًا (تحفظ كل الريسبونس)
            diagnosis.RawResponse = JsonSerializer.Serialize(aiResponse);

            // 💾 Save or Update
            if (diagnosis.Id == 0)
                await _testService.SaveDiagnosisAsync(diagnosis);
            else
                await _testService.UpdateDiagnosisAsync(diagnosis);

            // ✅ 5. Update Test Status
            test.Status = TestStatus.Completed;
            await _testService.UpdateTestAsync(test);

            // ✅ 6. Return Full AI Response
            return Success(aiResponse);
        }
    }
}