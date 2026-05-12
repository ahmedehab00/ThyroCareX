using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThyroCareX.Data.Models
{
    public class Message
    {
        public int Id { get; set; }
        
        public string SenderId { get; set; } // Can be DoctorId or PatientId (as string for simplicity in Hub)
        public string ReceiverId { get; set; }
        
        public string Content { get; set; }
        public string? ImageUrl { get; set; }
        
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;

        // Type to distinguish between doctor and patient if needed
        public string SenderType { get; set; } // "Doctor" or "Patient"

        public int? PatientId { get; set; }
        [ForeignKey("PatientId")]
        public Patient? Patient { get; set; }

        public int? DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public Doctor? Doctor { get; set; }
    }
}
