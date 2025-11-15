using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ThyroCareX.Data.Enums;

namespace ThyroCareX.Data.Models
{
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }

        public int DoctorID { get; set; }
        [ForeignKey("DoctorID")]
        public Doctor Doctor { get; set; }

        public int SubscriptionPlanID { get; set; }
        [ForeignKey("SubscriptionPlanID")]
        public SubscriptionPlan SubscriptionPlan { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public DateTime? VerifiedAt { get; set; }

        public bool IsRefunded { get; set; } = false;
    }
}
