using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;
using ThyroCareX.Data.Models;
using ThyroCareX.Data.Models.Identity;

namespace ThyroCareX.Infrastructure.Context
{
    public class ApplicationDbContext : IdentityDbContext<User,IdentityRole<int>, int, IdentityUserClaim<int>, IdentityUserRole<int>, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public ApplicationDbContext()
        {

        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<MedicalHistory> MedicalHistory { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
       public DbSet<Plan> Plans { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<DiagnosisResult> DiagnosisResults { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<AgentSession> AgentSessions { get; set; }
        public DbSet<AgentChatMessage> AgentChatMessages { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PostLike>()
             .HasOne(pl => pl.Post)
             .WithMany(p => p.PostLikes)
             .HasForeignKey(pl => pl.PostId)
             .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PostLike>()
                .HasOne(pl => pl.doctor)
                .WithMany(d => d.PostLikes)
                .HasForeignKey(pl => pl.DoctorId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Doctor)
                .WithMany(d => d.Comments)
                .HasForeignKey(c => c.DoctorId)
                .OnDelete(DeleteBehavior.NoAction);

      

            // منع الـ Cascade Delete بين MedicalRecord و Patient
            modelBuilder.Entity<MedicalHistory>()
               .HasOne(m => m.Patient)
               .WithMany(u=>u.MedicalHistories)
               .HasForeignKey(m => m.PatientID)
               .OnDelete(DeleteBehavior.NoAction);

            // (اختياري) تأكد كمان إن علاقة MedicalRecord مع Doctor مش بتعمل Cascade
           

            // إعداد علاقة 1-1 بين Doctor و User مع Cascade Delete
           modelBuilder.Entity<Doctor>()
            .HasOne(d => d.User)
            .WithOne() // كل User ممكن يكون له Doctor واحد
            .HasForeignKey<Doctor>(d => d.UserId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Test>()
            .HasOne(t => t.Patient)
            .WithMany(p => p.Tests)
            .HasForeignKey(t => t.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Test>()
                .HasOne(t => t.Doctor)
                .WithMany(d => d.Tests)
                .HasForeignKey(t => t.DoctorId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<DiagnosisResult>()
                .HasOne(d => d.Test)
                .WithOne(t => t.DiagnosisResult)
                .HasForeignKey<DiagnosisResult>(d => d.TestId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<PostLike>()
                .HasIndex(pl => new { pl.PostId, pl.DoctorId })
                .IsUnique();

            modelBuilder.Entity<Plan>()
                .Property(p => p.Features)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                    v => v.Trim().StartsWith("[") 
                        ? (System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions)null) ?? new List<string>())
                        : (string.IsNullOrEmpty(v) ? new List<string>() : new List<string> { v })
                );

            modelBuilder.Entity<Contact>().ToTable("ContactMessages");
        }
    }
}
