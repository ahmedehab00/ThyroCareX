using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThyroCareX.Data.Models
{
    public class GeneralAiSession
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public Identity.User? User { get; set; }

        public string Title { get; set; } = "New Chat";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<GeneralAiChatMessage> Messages { get; set; } = new List<GeneralAiChatMessage>();
    }
}
