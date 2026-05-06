using MediatR;
using ThyroCareX.Core.Bases;
using ThyroCareX.Core.Feature.TestWithAI.Commands.Models;
using ThyroCareX.Service.Abstarct;
using ThyroCareX.Service.Impelemanation;

namespace ThyroCareX.Core.Feature.TestWithAI.Commands.Handler
{
    public class ValidateImageHandler : ResponseHandler, IRequestHandler<ValidateImageCommand, Response<bool>>
    {
        private readonly IAIService _aiService;
        private readonly IImageService _imageService;

        public ValidateImageHandler(IAIService aiService, IImageService imageService)
        {
            _aiService = aiService;
            _imageService = imageService;
        }

        public async Task<Response<bool>> Handle(ValidateImageCommand request, CancellationToken cancellationToken)
        {
            if (request.ImageFile == null || request.ImageFile.Length == 0)
            {
                return BadRequest<bool>("Image file is required");
            }

            string imagePath;
            try
            {
                imagePath = await _imageService.UploadFileAsync(request.ImageFile);
            }
            catch (Exception ex)
            {
                return BadRequest<bool>($"Failed to upload image: {ex.Message}");
            }

            try
            {
                var isValid = await _aiService.ValidateUltrasoundAsync(imagePath);
                
                // Optional: We can delete the temporary image if we want, but since it might be used later for PredictImage
                // Or PredictImage will upload it again. Let's keep it simple.

                if (isValid)
                {
                    return Success(true, "Image is a valid ultrasound.");
                }
                else
                {
                    return Success(false, "Image does not appear to be a medical ultrasound.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest<bool>($"Ultrasound validation failed: {ex.Message}");
            }
        }
    }
}
