using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThyroCareX.Data.Models
{
    public class AgentSession
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString(); // This will act as the session_id for HuggingFace

        public int PatientId { get; set; }
        
        [ForeignKey("PatientId")]
        public Patient? Patient { get; set; }

        public string Title { get; set; } = "New Chat";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<AgentChatMessage> Messages { get; set; } = new List<AgentChatMessage>();
    }
}
