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
    public class PredictImageHandler : ResponseHandler, IRequestHandler<PredictImageCommand, Response<ImageAIResponse>>
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

        public async Task<Response<ImageAIResponse>> Handle(PredictImageCommand request, CancellationToken cancellationToken)
        {
            if (request.UltraSoundImage == null || request.UltraSoundImage.Length == 0)
            {
                return BadRequest<ImageAIResponse>("Ultrasound image is required");
            }

            var test = await _testService.GetTestByIdAsync(request.TestId);
            if (test == null) return NotFound<ImageAIResponse>("Test not found");

            try
            {
                test.ImagePath = await _imageService.UploadFileAsync(request.UltraSoundImage);
            }
            catch (Exception ex)
            {
                return BadRequest<ImageAIResponse>($"Failed to upload ultrasound image: {ex.Message}");
            }

            test.Status = TestStatus.Processing;
            await _testService.UpdateTestAsync(test);

            bool isValid;
            try
            {
                isValid = await _aiService.ValidateUltrasoundAsync(test.ImagePath);
            }
            catch (Exception ex)
            {
                test.Status = TestStatus.Failed;
                await _testService.UpdateTestAsync(test);
                return BadRequest<ImageAIResponse>($"Ultrasound validation failed: {ex.Message}");
            }

            if (!isValid)
            {
                test.Status = TestStatus.Failed;
                await _testService.UpdateTestAsync(test);

                return BadRequest<ImageAIResponse>("Uploaded image is not a valid ultrasound image");
            }

            ImageAIResponse aiResponse;
            try
            {
                aiResponse = await _aiService.PredictImageAsync(test.ImagePath);
            }
            catch (Exception ex)
            {
                test.Status = TestStatus.Failed;
                await _testService.UpdateTestAsync(test);
                return BadRequest<ImageAIResponse>($"Image prediction failed: {ex.Message}");
            }

            if (aiResponse == null || aiResponse.Status != "success")
                return BadRequest<ImageAIResponse>("AI Service failed to process the image");

            var diagnosis = await _testService.GetDiagnosisByTestIdAsync(request.TestId);

            if (diagnosis == null)
            {
                diagnosis = new DiagnosisResult { TestId = request.TestId };
            }

            diagnosis.ClassificationLabel = aiResponse.Classification.Label;
            diagnosis.Confidence = aiResponse.Classification.Confidence;
            diagnosis.TiradsStage = aiResponse.Classification.Tirads_Stage;
            diagnosis.OverlayImageUrl = aiResponse.Images.Overlay_Url;
            diagnosis.MaskImageUrl = aiResponse.Images.Mask_Url;
            diagnosis.RoiImageUrl = aiResponse.Images.Roi_Url;
            diagnosis.RawResponse = System.Text.Json.JsonSerializer.Serialize(aiResponse);

            if (diagnosis.Id == 0)
                await _testService.SaveDiagnosisAsync(diagnosis);
            else
                await _testService.UpdateDiagnosisAsync(diagnosis);

            test.Status = TestStatus.Completed;
            await _testService.UpdateTestAsync(test);
            return Success(aiResponse);
        }
    }
}
