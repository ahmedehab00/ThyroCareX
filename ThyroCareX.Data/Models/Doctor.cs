using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ThyroCareX.Data.Enums;

namespace ThyroCareX.Data.Models
{
    public class Doctor
    {
        public Doctor()
        {
            Patients = new HashSet<Patient>();
            MedicalRecords = new HashSet<MedicalRecord>();
            Payments = new HashSet<Payment>();

        }
        [Key]
        public int DoctorID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Specialization { get; set; }
        public Gender gender { get; set; }
        public int NationalID { get; set; }
        public int PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Hospital { get; set; }
        public string ImagePath { get; set; }
        public DateTime RegistrationAt { get; set; }
        public int SubscriptionPlanID { get; set; }
        [ForeignKey("SubscriptionPlanID")]
        public SubscriptionPlan SubscriptionPlan { get; set; }

        [InverseProperty("Doctor")]
        public virtual ICollection<Patient> Patients { get; set; }
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }






    }
}
