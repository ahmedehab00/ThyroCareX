using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThyroCareX.Data.Models
{
    public class MedicalRecord
    {
        [Key]
        public int MedicalRecordID { get; set; }
        public string Dignosis { get; set; }
        public string Notes { get; set; }
        public string AttachmentPath { get; set; }
        public DateTime RecordDate { get; set; } = DateTime.UtcNow;
        public int PatientID { get; set; }
        [ForeignKey("PatientID")]
        public Patient Patient { get; set; }
        public int DoctorID { get; set; }
        [ForeignKey("DoctorID")]
        public Doctor Doctor { get; set; }
    }
}
