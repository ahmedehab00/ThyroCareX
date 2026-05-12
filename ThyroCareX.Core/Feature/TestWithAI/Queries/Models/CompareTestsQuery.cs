using MediatR;
using System;
using ThyroCareX.Core.Bases;

namespace ThyroCareX.Core.Feature.TestWithAI.Queries.Models
{
    public class CompareTestsQuery : IRequest<Response<CompareTestsResponse>>
    {
        public int TestId1 { get; set; }
        public int TestId2 { get; set; }

        public CompareTestsQuery(int testId1, int testId2)
        {
            TestId1 = testId1;
            TestId2 = testId2;
        }
    }

    public class CompareTestsResponse
    {
        public TestDetail Before { get; set; }
        public TestDetail After { get; set; }
        public ComparisonSummary Summary { get; set; }
    }

    public class TestDetail
    {
        public int TestId { get; set; }
        public DateTime Date { get; set; }
        public double? TSH { get; set; }
        public double? T3 { get; set; }
        public double? TT4 { get; set; }
        public double? FTI { get; set; }
        public string Result { get; set; }
        public string RiskLevel { get; set; }
        public double? Confidence { get; set; }
    }

    public class ComparisonSummary
    {
        public string TSHChange { get; set; }
        public string OverallTrend { get; set; } // Stable, Improving, Worsening
        public string OverallTrendAr { get; set; } // مستقرة، تتحسن، تزداد سوءاً
        public string Message { get; set; }
        public string MessageAr { get; set; }
        public List<string> AnalysisDetails { get; set; } = new();
        public List<string> AnalysisDetailsAr { get; set; } = new();
    }
}
