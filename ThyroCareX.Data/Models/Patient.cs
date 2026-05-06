using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ThyroCareX.Data.Enums;
using static System.Net.Mime.MediaTypeNames;

namespace ThyroCareX.Data.Models
{
    public class Patient
    {
        
        public int Id { get;  set; }

        public string FullName { get;  set; }
        public DateTime DateOfBirth { get;  set; }
        public Gender Gender { get;  set; }

        public double Height { get;  set; }
        public double Weight { get;  set; }

        public string PhoneNumber { get;  set; }
        public string? Address { get;  set; }
        public string MedicalHistory { get;  set; } = string.Empty;
        public string CurrentMedications { get;  set; } = string.Empty;
        public string KnownAllergies { get;  set; } = string.Empty;
        public string? AttachmentPath { get; set; } = string.Empty;
        public DateTime CreatedAt { get;  set; }

        public int DoctorID { get;  set; }
            [ForeignKey("DoctorID")]
        public Doctor? Doctor { get;  set; }

        public ICollection<MedicalHistory> MedicalHistories { get;  set; } = new List<MedicalHistory>();
        public ICollection<DiagnosisResult> DiagnosisResults { get;  set; } = new List<DiagnosisResult>();
        public ICollection<Test> Tests { get;  set; } = new List<Test>();
      

    }
}
