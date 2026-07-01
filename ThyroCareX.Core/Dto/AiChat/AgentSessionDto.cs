using System;
using System.Collections.Generic;

namespace ThyroCareX.Core.Dto.AiChat
{
    public class AgentSessionDto
    {
        public string Id { get; set; }
        public int PatientId { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
