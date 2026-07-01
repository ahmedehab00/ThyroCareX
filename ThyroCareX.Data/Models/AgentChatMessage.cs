using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThyroCareX.Data.Models
{
    public class AgentChatMessage
    {
        [Key]
        public int Id { get; set; }

        public string SessionId { get; set; }

        [ForeignKey("SessionId")]
        public AgentSession? Session { get; set; }

        public string Role { get; set; } // "user" or "ai" or "system"

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
