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
using ThyroCareX.Data.Healpers.AiChat;
using ThyroCareX.Data.Models;

namespace ThyroCareX.Service.Abstarct
{
    public interface IAIService
    {
        Task<ClinicalAIResponse> AssessClinicalAsync(ClinicalRequest request);
        Task<List<ImageAIResponse>> PredictImageAsync(IEnumerable<string> imagePaths, string sessionId);
        Task<List<FnacAIResponse>> PredictFnacAsync(IEnumerable<string> imagePaths, string sessionId, bool force = false);
        Task<List<ThyroCareX.Service.Impelemanation.UltrasoundValidationResponse>> ValidateUltrasoundAsync(IEnumerable<string> imagePaths);
        Task<ChatAIResponse> ChatAsync(string query, string sessionId, string chatHistory, string? imagePath = null);
        IAsyncEnumerable<string> StreamChatAsync(string userMessage, string? sessionId, int userId);
        Task<AgentChatResponseDto> AgentChatAsync(string userMessage, string sessionId, int patientId);
        
        // Existing Patient Agent Chat
        Task<List<AgentSession>> GetPatientSessionsAsync(int patientId);
        Task<List<AgentChatMessage>> GetSessionMessagesAsync(string sessionId);

        // General Chat
        Task<List<GeneralAiSession>> GetGeneralSessionsAsync(int userId);
        Task<List<GeneralAiChatMessage>> GetGeneralSessionMessagesAsync(string sessionId);
    }
}
