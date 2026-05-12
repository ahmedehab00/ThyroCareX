using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using ThyroCareX.Core.Bases;
using ThyroCareX.Core.Feature.TestWithAI.Queries.Models;
using ThyroCareX.Data.Models;
using ThyroCareX.Service.Abstarct;

namespace ThyroCareX.Core.Feature.TestWithAI.Queries.Handler
{
    public class CompareTestsHandler : ResponseHandler, IRequestHandler<CompareTestsQuery, Response<CompareTestsResponse>>
    {
        private readonly ITestService _testService;

        public CompareTestsHandler(ITestService testService)
        {
            _testService = testService;
        }

        public async Task<Response<CompareTestsResponse>> Handle(CompareTestsQuery request, CancellationToken cancellationToken)
        {
            var test1 = await _testService.GetTestByIdAsync(request.TestId1);
            var test2 = await _testService.GetTestByIdAsync(request.TestId2);

            if (test1 == null || test2 == null)
            {
                return BadRequest<CompareTestsResponse>("One or both tests not found.");
            }

            var diag1 = await _testService.GetDiagnosisByTestIdAsync(test1.Id);
            var diag2 = await _testService.GetDiagnosisByTestIdAsync(test2.Id);

            // Determine chronological order
            var beforeTest = test1.CreatedAt < test2.CreatedAt ? test1 : test2;
            var afterTest = test1.CreatedAt < test2.CreatedAt ? test2 : test1;
            var beforeDiag = test1.CreatedAt < test2.CreatedAt ? diag1 : diag2;
            var afterDiag = test1.CreatedAt < test2.CreatedAt ? diag2 : diag1;

            var response = new CompareTestsResponse
            {
                Before = MapToDetail(beforeTest, beforeDiag),
                After = MapToDetail(afterTest, afterDiag),
                Summary = CalculateSummary(beforeTest, afterTest, beforeDiag, afterDiag)
            };

            return Success(response);
        }

        private TestDetail MapToDetail(Test test, DiagnosisResult diag)
        {
            return new TestDetail
            {
                TestId = test.Id,
                Date = test.CreatedAt,
                TSH = test.TSH,
                T3 = test.T3,
                TT4 = test.TT4,
                FTI = test.FTI,
                Result = diag?.ClassificationLabel ?? diag?.FunctionalStatus ?? "N/A",
                RiskLevel = diag?.RiskLevel ?? "N/A",
                Confidence = diag?.Confidence
            };
        }

        private ComparisonSummary CalculateSummary(Test b, Test a, DiagnosisResult bd, DiagnosisResult ad)
        {
            var summary = new ComparisonSummary();
            bool isImproving = false;
            bool isWorsening = false;

            // TSH Change
            if (b.TSH.HasValue && a.TSH.HasValue && b.TSH.Value != 0)
            {
                var diff = a.TSH.Value - b.TSH.Value;
                var pct = (diff / b.TSH.Value) * 100;
                summary.TSHChange = $"{(diff >= 0 ? "+" : "")}{pct:F1}%";
            }
            else
            {
                summary.TSHChange = "N/A";
            }

            // TSH Analysis
            if (b.TSH.HasValue && a.TSH.HasValue)
            {
                double bVal = b.TSH.Value;
                double aVal = a.TSH.Value;
                double diff = aVal - bVal;

                if (Math.Abs(diff) > 0.05)
                {
                    string direction = diff > 0 ? "increased" : "decreased";
                    string directionAr = diff > 0 ? "ارتفع" : "انخفض";
                    summary.AnalysisDetails.Add($"TSH level has {direction} by {Math.Abs(diff):F2} mIU/L (from {bVal} to {aVal}).");
                    summary.AnalysisDetailsAr.Add($"{directionAr} مستوى TSH بمقدار {Math.Abs(diff):F2} mIU/L (من {bVal} إلى {aVal}).");
                }

                // Improving if moving towards normal range (0.4 - 4.0)
                if (bVal > 4.0 && aVal < bVal) isImproving = true;
                if (bVal < 0.4 && aVal > bVal) isImproving = true;

                // Worsening if moving away from normal range
                if (bVal >= 0.4 && bVal <= 4.0 && (aVal > 4.0 || aVal < 0.4)) isWorsening = true;
                if (bVal > 4.0 && aVal > bVal) isWorsening = true;
                if (bVal < 0.4 && aVal < bVal) isWorsening = true;
            }

            // Risk Level Analysis
            if (bd != null && ad != null)
            {
                if (bd.RiskLevel != ad.RiskLevel)
                {
                    summary.AnalysisDetails.Add($"Risk level shifted from {bd.RiskLevel} to {ad.RiskLevel}.");
                    summary.AnalysisDetailsAr.Add($"تغير مستوى المخاطر من {bd.RiskLevel} إلى {ad.RiskLevel}.");
                    
                    if (bd.RiskLevel == "High" && ad.RiskLevel == "Low") isImproving = true;
                    if (bd.RiskLevel == "Low" && ad.RiskLevel == "High") isWorsening = true;
                }
            }

            if (isImproving)
            {
                summary.OverallTrend = "Improving";
                summary.OverallTrendAr = "تتحسن";
                summary.Message = "The condition is showing signs of improvement based on biomarker trends and AI analysis.";
                summary.MessageAr = "الحالة تظهر علامات تحسن بناءً على اتجاهات المؤشرات الحيوية وتحليل الذكاء الاصطناعي.";
            }
            else if (isWorsening)
            {
                summary.OverallTrend = "Worsening";
                summary.OverallTrendAr = "تزداد سوءاً";
                summary.Message = "The condition seems to be declining. A clinical follow-up is highly recommended to adjust treatment.";
                summary.MessageAr = "يبدو أن الحالة تتراجع. يوصى بشدة بمتابعة سريرية لتعديل العلاج.";
            }
            else
            {
                summary.OverallTrend = "Stable";
                summary.OverallTrendAr = "مستقرة";
                summary.Message = "The condition remains stable. Continue with the current monitoring plan.";
                summary.MessageAr = "الحالة لا تزال مستقرة. استمر في خطة المتابعة الحالية.";
            }

            return summary;
        }
    }
}
