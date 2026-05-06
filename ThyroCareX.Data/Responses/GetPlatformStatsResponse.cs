using System.Collections.Generic;

namespace ThyroCareX.Data.Responses
{
    public class GetPlatformStatsResponse
    {
        public int TotalDoctors { get; set; }
        public int TotalPatients { get; set; }
        public int TotalDiagnoses { get; set; }
        public int ActiveSubscriptions { get; set; }
        public int TotalSubscriptions { get; set; }
        public string StorageUsed { get; set; } = "0 GB";
        public double AiAccuracy { get; set; } = 94.2;
        public List<RecentActivityDto> RecentActivities { get; set; } = new();
        public List<WeeklyDiagnosisDto> WeeklyDiagnoses { get; set; } = new();
        public List<SubscriptionStatDto> SubscriptionDistribution { get; set; } = new();
    }

    public class WeeklyDiagnosisDto
    {
        public string Day { get; set; } = string.Empty;
        public int TotalCases { get; set; }
    }

    public class SubscriptionStatDto
    {
        public string PlanName { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class RecentActivityDto
    {
        public string User { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}
