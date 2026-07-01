using System.Collections.Generic;

namespace ThyroCareX.Data.Healpers.AiChat
{
    public class AgentChatResponseDto
    {
        public string status { get; set; }
        public string query { get; set; }
        public string response { get; set; }
        public List<string> tools_used { get; set; }
    }
}
