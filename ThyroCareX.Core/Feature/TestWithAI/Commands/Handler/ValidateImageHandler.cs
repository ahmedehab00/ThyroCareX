using MediatR;
using ThyroCareX.Core.Bases;
using ThyroCareX.Core.Feature.TestWithAI.Commands.Models;
using ThyroCareX.Service.Abstarct;
using ThyroCareX.Service.Impelemanation;

namespace ThyroCareX.Core.Feature.TestWithAI.Commands.Handler
{
    public class ValidateImageHandler : ResponseHandler, IRequestHandler<ValidateImageCommand, Response<List<UltrasoundValidationResponse>>>
    {
        private readonly IAIService _aiService;
        private readonly IImageService _imageService;

        public ValidateImageHandler(IAIService aiService, IImageService imageService)
        {
            _aiService = aiService;
            _imageService = imageService;
        }

        public async Task<Response<List<UltrasoundValidationResponse>>> Handle(ValidateImageCommand request, CancellationToken cancellationToken)
        {
            if (request.ImageFiles == null || request.ImageFiles.Count == 0)
            {
                return BadRequest<List<UltrasoundValidationResponse>>("Image files are required");
            }

            var imagePaths = new List<string>();
            try
            {
                foreach (var file in request.ImageFiles)
                {
                    var path = await _imageService.UploadFileAsync(file);
                    imagePaths.Add(path);
                }
            }
            catch (Exception ex)
            {
                return BadRequest<List<UltrasoundValidationResponse>>($"Failed to upload images: {ex.Message}");
            }

            try
            {
                var validationResults = await _aiService.ValidateUltrasoundAsync(imagePaths);
                
                return Success(validationResults, "Images validated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest<List<UltrasoundValidationResponse>>($"Ultrasound validation failed: {ex.Message}");
            }
        }
    }
}
