using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThyroCareX.Data.Models
{
    public class GeneralAiChatMessage
    {
        [Key]
        public int Id { get; set; }

        public string SessionId { get; set; }

        [ForeignKey("SessionId")]
        public GeneralAiSession? Session { get; set; }

        public string Role { get; set; } = string.Empty; // "user" or "ai"
        
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
