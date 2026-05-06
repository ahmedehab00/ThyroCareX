using MediatR;
using Microsoft.AspNetCore.Http;
using ThyroCareX.Core.Bases;
using ThyroCareX.Core.Dto;

namespace ThyroCareX.Core.Feature.AiChat.Commands.Models
{
    public class SendChatCommand : IRequest<Response<ChatAIResponse>>
    {
        public string Query { get; set; }
        public string SessionId { get; set; }
        public string ChatHistory { get; set; }
        public IFormFile? Image { get; set; }
    }
}
