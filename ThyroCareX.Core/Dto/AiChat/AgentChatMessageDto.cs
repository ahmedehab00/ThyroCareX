using System;

namespace ThyroCareX.Core.Dto.AiChat
{
    public class AgentChatMessageDto
    {
        public int Id { get; set; }
        public string SessionId { get; set; }
        public string Role { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
