using Microsoft.EntityFrameworkCore;
using ThyroCareX.Data.Models;

namespace ThyroCareX.Infrastructure.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {

        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<Payment> Payments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // منع الـ Cascade Delete بين Payment و SubscriptionPlan
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.SubscriptionPlan)
                .WithMany(sp => sp.Payments)
                .HasForeignKey(p => p.SubscriptionPlanID)
                .OnDelete(DeleteBehavior.Restrict);  // أو NoAction

            // منع الـ Cascade Delete بين MedicalRecord و Patient
            modelBuilder.Entity<MedicalRecord>()
               .HasOne(m => m.Patient)
               .WithMany(p => p.MedicalRecords)
               .HasForeignKey(m => m.PatientID)
               .OnDelete(DeleteBehavior.Restrict);

            // (اختياري) تأكد كمان إن علاقة MedicalRecord مع Doctor مش بتعمل Cascade
            modelBuilder.Entity<MedicalRecord>()
                .HasOne(m => m.Doctor)
                .WithMany()
                .HasForeignKey(m => m.DoctorID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
