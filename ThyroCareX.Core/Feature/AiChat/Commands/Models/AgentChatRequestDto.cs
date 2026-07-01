using System;

namespace ThyroCareX.Core.Feature.AiChat.Commands.Models
{
    public class AgentChatRequestDto
    {
        public string session_id { get; set; }
        public string user_message { get; set; }
        public int patient_id { get; set; }
    }
}
