using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThyroCareX.Core.Dto.FnacAIResponse;
using ThyroCareX.Core.Dto.ImageAIResponse;
using ThyroCareX.Data.Healpers.ClinicalAI;
using ThyroCareX.Data.Healpers.ClinicalAIResponse;
using ThyroCareX.Core.Dto;

namespace ThyroCareX.Service.Abstarct
{
    public interface IAIService
    {
        Task<ImageAIResponse> PredictImageAsync(string imagePath);
        Task<ClinicalAIResponse> AssessClinicalAsync(ClinicalRequest request);
        Task<FnacAIResponse> PredictFnacAsync(string imagePath);
        Task<bool> ValidateUltrasoundAsync(string imagePath);
        Task<ChatAIResponse> ChatAsync(string query, string sessionId, string chatHistory, string? imagePath = null);
    }
}
