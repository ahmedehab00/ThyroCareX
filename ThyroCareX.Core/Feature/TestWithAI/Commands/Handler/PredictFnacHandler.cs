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
    public class PredictFnacHandler : ResponseHandler, IRequestHandler<PredictFnacCommand, Response<List<FnacAIResponse>>>
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

        public async Task<Response<List<FnacAIResponse>>> Handle(PredictFnacCommand request, CancellationToken cancellationToken)
        {
            // ✅ 1. Get Test
            var test = await _testService.GetTestByIdAsync(request.TestId);
            if (test == null)
                return NotFound<List<FnacAIResponse>>("Test not found");

            // ✅ 2. Upload Images
            if (request.FnacImages == null || !request.FnacImages.Any())
                return BadRequest<List<FnacAIResponse>>("At least one FNAC image is required");

            var imagePaths = new List<string>();
            foreach (var image in request.FnacImages)
            {
                var path = await _imageService.UploadFileAsync(image);
                imagePaths.Add(path);
            }

            // We store the first image path in the test record, or multiple if the schema supported it.
            // For now, let's keep the first one.
            test.FnacImagePath = imagePaths.First();
            test.Status = TestStatus.Processing;
            await _testService.UpdateTestAsync(test);

            // ✅ 3. Call AI
            var aiResponses = await _aiService.PredictFnacAsync(imagePaths, request.SessionId ?? string.Empty, force: false);

            if (aiResponses == null || !aiResponses.Any() || aiResponses.Any(r => r.Status != "success"))
            {
                test.Status = TestStatus.Failed;
                await _testService.UpdateTestAsync(test);
                return BadRequest<List<FnacAIResponse>>("AI Service failed to process FNAC image(s)");
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

            // 🧠 FNAC DATA (using first prediction for summary)
            var firstPred = aiResponses.First();
            diagnosis.BethesdaCategory = firstPred.Classification?.BethesdaCategory;
            diagnosis.BethesdaLabel = firstPred.Classification?.BethesdaLabel;
            diagnosis.MalignancyRisk = firstPred.Classification?.MalignancyRisk;
            diagnosis.FnacRecommendation = firstPred.Classification?.Recommendation;

            // 🔥 مهم جدًا: We do not overwrite RawResponse here because it contains the Ultrasound data.
            // Save the FNAC raw response in the newly created FnacRawResponse column
            diagnosis.FnacRawResponse = JsonSerializer.Serialize(aiResponses);

            // 💾 Save or Update
            if (diagnosis.Id == 0)
                await _testService.SaveDiagnosisAsync(diagnosis);
            else
                await _testService.UpdateDiagnosisAsync(diagnosis);

            // ✅ 5. Update Test Status
            test.Status = TestStatus.Completed;
            await _testService.UpdateTestAsync(test);

            // ✅ 6. Return Full AI Response
            return Success(aiResponses);
        }
    }
}