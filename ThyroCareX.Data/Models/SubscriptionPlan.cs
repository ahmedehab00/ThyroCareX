using System.ComponentModel.DataAnnotations;

namespace ThyroCareX.Data.Models
{
    public class SubscriptionPlan
    {
        public SubscriptionPlan()
        {
            Doctors = new HashSet<Doctor>();
            Payments = new HashSet<Payment>();
        }
        [Key]
        public int SubscriptionPlanID { get; set; }
        public string PlanName { get; set; }
        public string PlanDescription { get; set; }
        public decimal Price { get; set; }
        public string DurationInMonths { get; set; }
        public virtual ICollection<Doctor> Doctors { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }

    }
}
