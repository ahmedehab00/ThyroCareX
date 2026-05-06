using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThyroCareX.Data.Enums;
using ThyroCareX.Data.Responses;
using ThyroCareX.Infrastructure.Abstarct;
using ThyroCareX.Service.Abstarct;

namespace ThyroCareX.Service.Impelemanation
{
    public class DashboardService : IDashboardService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly ITestRepo _testRepository;
        private readonly ISubscriptionPlanRepo _subscriptionPlanRepository;

        public DashboardService(
            IDoctorRepository doctorRepository,
            IPatientRepository patientRepository,
            ITestRepo testRepository,
            ISubscriptionPlanRepo subscriptionPlanRepository)
        {
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
            _testRepository = testRepository;
            _subscriptionPlanRepository = subscriptionPlanRepository;
        }

        public async Task<GetPlatformStatsResponse> GetPlatformStatsAsync()
        {
            var doctorCount = await _doctorRepository.GetTableNoTracking().CountAsync(d => d.Status == DoctorStatus.Approved);
            var patientCount = await _patientRepository.GetTableNoTracking().CountAsync();
            var diagnosisCount = await _testRepository.GetTableNoTracking().CountAsync();
            var activeSubscriptions = await _subscriptionPlanRepository.GetTableNoTracking().CountAsync(s => s.Status == SubscriptionStatus.Active);
            var totalSubscriptions = await _subscriptionPlanRepository.GetTableNoTracking().CountAsync();

            // Recent activity - fetch latest 5 tests with patient info
            var recentTests = await _testRepository.GetTableNoTracking()
                .Include(t => t.Patient)
                .OrderByDescending(t => t.CreatedAt)
                .Take(5)
                .ToListAsync();

            var recentActivities = recentTests.Select(t => new RecentActivityDto
            {
                User = t.Patient?.FullName ?? "Unknown",
                Action = "Completed a thyroid scan",
                Time = GetRelativeTime(t.CreatedAt),
                Type = "diagnosis"
            }).ToList();

            // Weekly Diagnoses (last 7 days)
            var weekAgo = DateTime.UtcNow.Date.AddDays(-7);
            var weeklyTests = await _testRepository.GetTableNoTracking()
                .Where(t => t.CreatedAt >= weekAgo)
                .GroupBy(t => t.CreatedAt.Date)
                .Select(g => new WeeklyDiagnosisDto
                {
                    Day = g.Key.DayOfWeek.ToString().Substring(0, 3),
                    TotalCases = g.Count()
                })
                .ToListAsync();

            // Subscription Distribution
            var subDistribution = await _subscriptionPlanRepository.GetTableNoTracking()
                .Include(s => s.Plan)
                .GroupBy(s => s.Plan.PlanType)
                .Select(g => new SubscriptionStatDto
                {
                    PlanName = g.Key.ToString(),
                    Count = g.Count()
                })
                .ToListAsync();

            return new GetPlatformStatsResponse
            {
                TotalDoctors = doctorCount,
                TotalPatients = patientCount,
                TotalDiagnoses = diagnosisCount,
                ActiveSubscriptions = activeSubscriptions,
                TotalSubscriptions = totalSubscriptions,
                StorageUsed = "1.2 GB", 
                AiAccuracy = 94.5,
                RecentActivities = recentActivities,
                WeeklyDiagnoses = weeklyTests,
                SubscriptionDistribution = subDistribution
            };
        }

        private string GetRelativeTime(DateTime date)
        {
            var span = DateTime.Now - date;
            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes} mins ago";
            if (span.TotalHours < 24) return $"{(int)span.TotalHours} hours ago";
            return $"{(int)span.TotalDays} days ago";
        }
    }
}
