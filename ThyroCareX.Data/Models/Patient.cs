using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ThyroCareX.Data.Enums;

namespace ThyroCareX.Data.Models
{
    public class Patient
    {
        public Patient()
        {
            MedicalRecords = new HashSet<MedicalRecord>();
        }
        [Key]
        public int PatientID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Gender gender { get; set; }
        public int Age { get; set; }
        public int PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime RegistrationAt { get; set; } = DateTime.UtcNow;
        public int DoctorID { get; set; }
        [ForeignKey("DoctorID")]
        public Doctor Doctor { get; set; }
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; }

    }
}
