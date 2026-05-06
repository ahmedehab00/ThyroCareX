using MediatR;
using ThyroCareX.Core.Bases;
using ThyroCareX.Core.Dto;
using ThyroCareX.Core.Feature.AiChat.Commands.Models;
using ThyroCareX.Service.Abstarct;

namespace ThyroCareX.Core.Feature.AiChat.Commands.Handler
{
    public class ChatHandler : ResponseHandler, IRequestHandler<SendChatCommand, Response<ChatAIResponse>>
    {
        private readonly IAIService _aiService;
        private readonly IImageService _imageService;

        public ChatHandler(IAIService aiService, IImageService imageService)
        {
            _aiService = aiService;
            _imageService = imageService;
        }

        public async Task<Response<ChatAIResponse>> Handle(SendChatCommand request, CancellationToken cancellationToken)
        {
            string? uploadedImagePath = null;
            if (request.Image != null && request.Image.Length > 0)
            {
                try
                {
                    uploadedImagePath = await _imageService.UploadFileAsync(request.Image);
                }
                catch (Exception ex)
                {
                    return BadRequest<ChatAIResponse>($"Failed to upload image: {ex.Message}");
                }
            }

            try
            {
                var aiResponse = await _aiService.ChatAsync(request.Query, request.SessionId, request.ChatHistory, uploadedImagePath);
                
                if (aiResponse == null)
                    return BadRequest<ChatAIResponse>("AI Service failed to respond.");

                return Success(aiResponse);
            }
            catch (Exception ex)
            {
                return BadRequest<ChatAIResponse>($"Chat session failed: {ex.Message}");
            }
        }
    }
}
