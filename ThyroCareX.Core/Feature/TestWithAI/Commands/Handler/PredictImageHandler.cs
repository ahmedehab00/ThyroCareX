using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ThyroCareX.Core.Bases;
using ThyroCareX.Core.Dto.ImageAIResponse;
using ThyroCareX.Core.Feature.TestWithAI.Commands.Models;
using ThyroCareX.Data.Enums;
using ThyroCareX.Data.Models;
using ThyroCareX.Service.Abstarct;
using ThyroCareX.Service.Impelemanation;

namespace ThyroCareX.Core.Feature.TestWithAI.Commands.Handler
{
    public class PredictImageHandler : ResponseHandler, IRequestHandler<PredictImageCommand, Response<List<ImageAIResponse>>>
    {
        private readonly ITestService _testService;
        private readonly IAIService _aiService;
        private readonly IImageService _imageService;

        public PredictImageHandler(ITestService testService, IAIService aiService, IImageService imageService)
        {
            _testService = testService;
            _aiService = aiService;
            _imageService = imageService;
        }

        public async Task<Response<List<ImageAIResponse>>> Handle(PredictImageCommand request, CancellationToken cancellationToken)
        {
            if (request.UltraSoundImages == null || request.UltraSoundImages.Count == 0)
            {
                return BadRequest<List<ImageAIResponse>>("Ultrasound images are required");
            }

            var test = await _testService.GetTestByIdAsync(request.TestId);
            if (test == null) return NotFound<List<ImageAIResponse>>("Test not found");

            var newImagePaths = new List<string>();
            try
            {
                foreach(var img in request.UltraSoundImages)
                {
                    var path = await _imageService.UploadFileAsync(img);
                    newImagePaths.Add(path);
                    test.ImagePath = string.IsNullOrEmpty(test.ImagePath) ? path : test.ImagePath + "," + path;
                }
            }
            catch (Exception ex)
            {
                return BadRequest<List<ImageAIResponse>>($"Failed to upload ultrasound images: {ex.Message}");
            }

            test.Status = TestStatus.Processing;
            await _testService.UpdateTestAsync(test);

            bool isValid = false;
            try
            {
                var valResults = await _aiService.ValidateUltrasoundAsync(newImagePaths);
                isValid = valResults != null && valResults.Any() && valResults.All(v => v.IsUltrasound);
            }
            catch (Exception ex)
            {
                test.Status = TestStatus.Failed;
                await _testService.UpdateTestAsync(test);
                return BadRequest<List<ImageAIResponse>>($"Ultrasound validation failed: {ex.Message}");
            }

            if (!isValid)
            {
                test.Status = TestStatus.Failed;
                await _testService.UpdateTestAsync(test);

                return BadRequest<List<ImageAIResponse>>("Uploaded images include invalid ultrasound images");
            }

            List<ImageAIResponse> aiResponses;
            try
            {
                aiResponses = await _aiService.PredictImageAsync(newImagePaths);
            }
            catch (Exception ex)
            {
                test.Status = TestStatus.Failed;
                await _testService.UpdateTestAsync(test);
                return BadRequest<List<ImageAIResponse>>($"Image prediction failed: {ex.Message}");
            }

            if (aiResponses == null || aiResponses.Count == 0 || !aiResponses.All(r => r.Status == "success"))
                return BadRequest<List<ImageAIResponse>>("AI Service failed to process the images");

            var diagnosis = await _testService.GetDiagnosisByTestIdAsync(request.TestId);

            if (diagnosis == null)
            {
                diagnosis = new DiagnosisResult { TestId = request.TestId };
            }

            foreach(var aiResponse in aiResponses)
            {
                diagnosis.ClassificationLabel = string.IsNullOrEmpty(diagnosis.ClassificationLabel) ? aiResponse.Classification.Label : diagnosis.ClassificationLabel + "," + aiResponse.Classification.Label;
                diagnosis.Confidence = aiResponse.Classification.Confidence; // keep latest
                diagnosis.TiradsStage = string.IsNullOrEmpty(diagnosis.TiradsStage) ? aiResponse.Classification.Tirads_Stage : diagnosis.TiradsStage + "," + aiResponse.Classification.Tirads_Stage;
                
                diagnosis.OverlayImageUrl = string.IsNullOrEmpty(diagnosis.OverlayImageUrl) ? (aiResponse.Images.Overlay_Url ?? "") : diagnosis.OverlayImageUrl + "," + (aiResponse.Images.Overlay_Url ?? "");
                diagnosis.MaskImageUrl = string.IsNullOrEmpty(diagnosis.MaskImageUrl) ? (aiResponse.Images.Mask_Url ?? "") : diagnosis.MaskImageUrl + "," + (aiResponse.Images.Mask_Url ?? "");
                diagnosis.RoiImageUrl = string.IsNullOrEmpty(diagnosis.RoiImageUrl) ? (aiResponse.Images.Roi_Url ?? "") : diagnosis.RoiImageUrl + "," + (aiResponse.Images.Roi_Url ?? "");
            }

            List<ImageAIResponse> allImageResponses = new List<ImageAIResponse>();
            if (!string.IsNullOrEmpty(diagnosis.RawResponse))
            {
                try
                {
                    if (diagnosis.RawResponse.TrimStart().StartsWith("["))
                    {
                        var existing = System.Text.Json.JsonSerializer.Deserialize<List<ImageAIResponse>>(diagnosis.RawResponse);
                        if (existing != null)
                        {
                            allImageResponses.AddRange(existing);
                        }
                    }
                }
                catch { }
            }
            allImageResponses.AddRange(aiResponses);

            diagnosis.RawResponse = System.Text.Json.JsonSerializer.Serialize(allImageResponses);

            if (diagnosis.Id == 0)
                await _testService.SaveDiagnosisAsync(diagnosis);
            else
                await _testService.UpdateDiagnosisAsync(diagnosis);

            test.Status = TestStatus.Completed;
            await _testService.UpdateTestAsync(test);
            return Success(aiResponses);
        }
    }
}
