using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ThyroCareX.Core.Dto.FnacAIResponse;
using ThyroCareX.Core.Dto.ImageAIResponse;
using ThyroCareX.Data.Healpers.ClinicalAI;
using ThyroCareX.Data.Healpers.ClinicalAIResponse;
using ThyroCareX.Core.Dto;
using ThyroCareX.Data.Healpers.AiChat;
using ThyroCareX.Service.Abstarct;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using ThyroCareX.Infrastructure.Context;
using ThyroCareX.Data.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace ThyroCareX.Service.Impelemanation
{
    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _dbContext;

        public AIService(HttpClient httpClient, IConfiguration configuration, IWebHostEnvironment env, ApplicationDbContext dbContext)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _env = env;
            _dbContext = dbContext;

            var apiKey = _configuration["AISettings:ApiKey"];
            _httpClient.DefaultRequestHeaders.Add("X-AI-Service-Key", apiKey);
        }
        public async Task<List<ImageAIResponse>> PredictImageAsync(IEnumerable<string> imagePaths, string sessionId)
        {
            using var form = new MultipartFormDataContent();
            
            foreach (var imagePath in imagePaths)
            {
                var fullPath = Path.Combine(_env.WebRootPath ?? "wwwroot", imagePath.TrimStart('/'));
                if (!File.Exists(fullPath)) continue;

                var fileBytes = await File.ReadAllBytesAsync(fullPath);
                var byteContent = new ByteArrayContent(fileBytes);
                
                // Set Content-Type
                var extension = Path.GetExtension(fullPath).ToLower();
                byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                    extension == ".png" ? "image/png" : "image/jpeg"
                );

                form.Add(byteContent, "files", Path.GetFileName(fullPath));
            }

            form.Add(new StringContent(sessionId ?? string.Empty), "session_id");

            var response = await _httpClient.PostAsync(
                "https://amer003100-thyraxcdss.hf.space/image/predict?force=false",
                form
            );

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"AI Service Image Predict Error ({response.StatusCode}): {errorBody}");
            }

            var raw = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            try
            {
                var parsedList = JsonSerializer.Deserialize<List<ImageAIResponse>>(raw, options);
                if (parsedList == null)
                    throw new JsonException("Empty JSON response from AI image endpoint.");

                foreach (var parsed in parsedList)
                {
                    NormalizeImageUrls(parsed);
                }
                return parsedList;
            }
            catch (JsonException)
            {
                // Fallback parser for schema drift if array parsing fails
                var list = new List<ImageAIResponse>();
                using var doc = JsonDocument.Parse(raw);
                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var root in doc.RootElement.EnumerateArray())
                    {
                        var parsed = new ImageAIResponse
                        {
                            Status = TryGetString(root, "status") ?? "success",
                            Message = TryGetString(root, "message") ?? TryGetString(root, "ai_recommendation"),
                            Bbox = TryGetIntList(root, "bbox"),
                            Classification = ParseClassification(root),
                            Images = ParseImages(root)
                        };

                        NormalizeImageUrls(parsed);
                        list.Add(parsed);
                    }
                }
                return list;
            }
        }
        public async Task<ClinicalAIResponse> AssessClinicalAsync(ClinicalRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "https://amer003100-thyraxcdss.hf.space/clinical/assess",
                request
            );

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"AI Service Clinical Assess Error ({response.StatusCode}): {errorBody}");
            }

            return await response.Content.ReadFromJsonAsync<ClinicalAIResponse>();
        }

        public async Task<List<FnacAIResponse>> PredictFnacAsync(IEnumerable<string> imagePaths, string sessionId, bool force = false)
        {
            using var form = new MultipartFormDataContent();

            foreach (var imagePath in imagePaths)
            {
                var fullPath = Path.Combine(_env.WebRootPath ?? "wwwroot", imagePath.TrimStart('/'));
                if (!File.Exists(fullPath)) continue;

                var fileBytes = await File.ReadAllBytesAsync(fullPath);
                var byteContent = new ByteArrayContent(fileBytes);
                
                var extension = Path.GetExtension(fullPath).ToLower();
                byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                    extension == ".png" ? "image/png" : "image/jpeg"
                );

                form.Add(byteContent, "files", Path.GetFileName(fullPath));
            }

            form.Add(new StringContent(sessionId ?? string.Empty), "session_id");
            form.Add(new StringContent(force.ToString().ToLower()), "force");

            var response = await _httpClient.PostAsync(
                "https://amer003100-thyraxcdss.hf.space/fnac/predict",
                form
            );

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"AI Service FNAC Predict Error ({response.StatusCode}): {errorBody}");
            }

            var result = await response.Content.ReadFromJsonAsync<List<FnacAIResponse>>();
            return result ?? new List<FnacAIResponse>();
        }
        public async Task<List<UltrasoundValidationResponse>> ValidateUltrasoundAsync(IEnumerable<string> imagePaths)
        {
            using var form = new MultipartFormDataContent();

            foreach (var imagePath in imagePaths)
            {
                var fullPath = Path.Combine(_env.WebRootPath ?? "wwwroot", imagePath.TrimStart('/'));
                if (!File.Exists(fullPath)) continue;

                var fileBytes = await File.ReadAllBytesAsync(fullPath);
                var byteContent = new ByteArrayContent(fileBytes);

                var extension = Path.GetExtension(fullPath).ToLower();
                byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                    extension == ".png" ? "image/png" : "image/jpeg"
                );

                form.Add(byteContent, "files", Path.GetFileName(fullPath));
            }

            var response = await _httpClient.PostAsync(
                "https://amer003100-thyraxcdss.hf.space/image/validate?force=false",
                form
            );

            if (!response.IsSuccessStatusCode)
                return new List<UltrasoundValidationResponse>();

            var result = await response.Content.ReadFromJsonAsync<List<UltrasoundValidationResponse>>();

            return result ?? new List<UltrasoundValidationResponse>();
        }
        public async Task<ChatAIResponse> ChatAsync(string query, string sessionId, string chatHistory, string? imagePath = null)
        {
            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(query ?? ""), "query");
            form.Add(new StringContent(sessionId ?? "string"), "session_id");
            form.Add(new StringContent(chatHistory ?? "[]"), "chat_history");

            if (!string.IsNullOrEmpty(imagePath))
            {
                var fullPath = Path.Combine(_env.WebRootPath ?? "wwwroot", imagePath.TrimStart('/'));
                if (File.Exists(fullPath))
                {
                    var fileBytes = await File.ReadAllBytesAsync(fullPath);
                    var byteContent = new ByteArrayContent(fileBytes);
                    var extension = Path.GetExtension(fullPath).ToLower();
                    byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                        extension == ".png" ? "image/png" : "image/jpeg"
                    );
                    form.Add(byteContent, "image", Path.GetFileName(fullPath));
                }
            }

            var response = await _httpClient.PostAsync(
                "https://amer003100-thyraxcdss.hf.space/agent/chat",
                form
            );

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"AI Agent Chat Error ({response.StatusCode}): {errorBody}");
            }

            return await response.Content.ReadFromJsonAsync<ChatAIResponse>();
        }

        public async IAsyncEnumerable<string> StreamChatAsync(string userMessage, string? sessionId, int userId)
        {
            var isNewSession = string.IsNullOrEmpty(sessionId) || !await _dbContext.GeneralAiSessions.AnyAsync(s => s.Id == sessionId);
            if (string.IsNullOrEmpty(sessionId)) sessionId = Guid.NewGuid().ToString();

            var session = await _dbContext.GeneralAiSessions.FindAsync(sessionId);
            if (session == null)
            {
                session = new GeneralAiSession
                {
                    Id = sessionId,
                    UserId = userId,
                    Title = userMessage.Length > 30 ? userMessage.Substring(0, 30) + "..." : userMessage,
                    CreatedAt = DateTime.UtcNow
                };
                _dbContext.GeneralAiSessions.Add(session);
            }

            _dbContext.GeneralAiChatMessages.Add(new GeneralAiChatMessage
            {
                SessionId = sessionId,
                Role = "user",
                Content = userMessage,
                CreatedAt = DateTime.UtcNow
            });
            await _dbContext.SaveChangesAsync();

            var payload = new
            {
                session_id = sessionId,
                user_message = userMessage
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://amer003100-thyraxcdss.hf.space/ai/chat");
            request.Headers.Add("accept", "application/json");
            request.Content = JsonContent.Create(payload);

            HttpResponseMessage response = null;
            bool isError = false;
            string errorText = "";
            try
            {
                response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                isError = true;
                errorText = ex.Message.Replace("\"", "\\\"").Replace("\n", " ");
            }

            if (isError)
            {
                yield return $"data: {{\"status\": \"error\", \"message\": \"AI connection failed: {errorText}. Please try again.\"}}\n\n";
                yield break;
            }

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);
            var aiResponseBuilder = new StringBuilder();

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (!string.IsNullOrEmpty(line))
                {
                    yield return line + "\n\n";
                    if (line.StartsWith("data: ") && line != "data: [DONE]")
                    {
                        try
                        {
                            var jsonString = line.Substring(6);
                            using var doc = JsonDocument.Parse(jsonString);
                            if (doc.RootElement.TryGetProperty("token", out var token))
                            {
                                aiResponseBuilder.Append(token.GetString());
                            }
                        }
                        catch { }
                    }
                }
            }

            // Save AI response
            var fullResponse = aiResponseBuilder.ToString();
            if (!string.IsNullOrEmpty(fullResponse))
            {
                _dbContext.GeneralAiChatMessages.Add(new GeneralAiChatMessage
                {
                    SessionId = sessionId,
                    Role = "ai",
                    Content = fullResponse,
                    CreatedAt = DateTime.UtcNow
                });
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<GeneralAiSession>> GetGeneralSessionsAsync(int userId)
        {
            return await _dbContext.GeneralAiSessions
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<GeneralAiChatMessage>> GetGeneralSessionMessagesAsync(string sessionId)
        {
            return await _dbContext.GeneralAiChatMessages
                .Where(m => m.SessionId == sessionId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }

        private static string? TryGetString(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out var value))
                return null;

            return value.ValueKind switch
            {
                JsonValueKind.String => value.GetString(),
                JsonValueKind.Number => value.ToString(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                _ => null
            };
        }

        private static List<int>? TryGetIntList(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out var value) || value.ValueKind != JsonValueKind.Array)
                return null;

            var list = new List<int>();
            foreach (var item in value.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.Number && item.TryGetInt32(out var i))
                    list.Add(i);
            }
            return list.Count > 0 ? list : null;
        }

        private static ClassificationDto ParseClassification(JsonElement root)
        {
            if (root.TryGetProperty("classification", out var classification))
            {
                if (classification.ValueKind == JsonValueKind.Object)
                {
                    return new ClassificationDto
                    {
                        Prediction = classification.TryGetProperty("prediction", out var p) && p.TryGetInt32(out var pred) ? pred : 0,
                        Label = TryGetString(classification, "label") ?? "Unknown",
                        Confidence = classification.TryGetProperty("confidence_pct", out var c) && c.TryGetDouble(out var conf) ? conf : 0,
                        Tirads_Stage = TryGetString(classification, "acr_tirads_level") ?? "N/A",
                        RiskLevel = TryGetString(classification, "risk_level") ?? "Unknown",
                        ClinicalRecommendation = TryGetString(classification, "clinical_recommendation") ?? "No recommendation provided."
                    };
                }

                if (classification.ValueKind == JsonValueKind.String)
                {
                    return new ClassificationDto
                    {
                        Label = classification.GetString() ?? "Unknown",
                        Prediction = 0,
                        Confidence = 0,
                        Tirads_Stage = "N/A",
                        RiskLevel = "Unknown",
                        ClinicalRecommendation = "No recommendation provided."
                    };
                }
            }

            return new ClassificationDto
            {
                Label = "Unknown",
                Prediction = 0,
                Confidence = 0,
                Tirads_Stage = "N/A",
                RiskLevel = "Unknown",
                ClinicalRecommendation = "No recommendation provided."
            };
        }

        private static ImageUrlsDto ParseImages(JsonElement root)
        {
            if (root.TryGetProperty("images", out var images) && images.ValueKind == JsonValueKind.Object)
            {
                return new ImageUrlsDto
                {
                    Overlay_Url = TryGetString(images, "overlay_url") ?? string.Empty,
                    Mask_Url = TryGetString(images, "mask_url") ?? string.Empty,
                    Roi_Url = TryGetString(images, "roi_url") ?? string.Empty
                };
            }

            return new ImageUrlsDto
            {
                Overlay_Url = string.Empty,
                Mask_Url = string.Empty,
                Roi_Url = string.Empty
            };
        }

        private static void NormalizeImageUrls(ImageAIResponse response)
        {
            if (response.Images == null) return;

            response.Images.Overlay_Url = NormalizeUrl(response.Images.Overlay_Url);
            response.Images.Mask_Url = NormalizeUrl(response.Images.Mask_Url);
            response.Images.Roi_Url = NormalizeUrl(response.Images.Roi_Url);
        }

        private static string NormalizeUrl(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            if (value.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                value.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                value.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            {
                return value;
            }

            var parts = value.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var imageId = parts.LastOrDefault() ?? "0";
            return $"/api/TestsWithAI/ViewImage/{imageId}";
        }

        public async Task<AgentChatResponseDto> AgentChatAsync(string userMessage, string sessionId, int patientId)
        {
            var payload = new { session_id = sessionId, user_message = userMessage };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://amer003100-thyraxcdss.hf.space/agent/chat", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"AI Agent Chat Error ({response.StatusCode}): {error}");
            }

            var result = await response.Content.ReadFromJsonAsync<AgentChatResponseDto>();
            
            if (result != null && result.status == "success")
            {
                var session = await _dbContext.AgentSessions.FindAsync(sessionId);
                if (session == null)
                {
                    session = new ThyroCareX.Data.Models.AgentSession
                    {
                        Id = sessionId,
                        PatientId = patientId,
                        Title = userMessage.Length > 30 ? userMessage.Substring(0, 30) + "..." : userMessage,
                        CreatedAt = DateTime.UtcNow
                    };
                    _dbContext.AgentSessions.Add(session);
                }

                _dbContext.AgentChatMessages.Add(new ThyroCareX.Data.Models.AgentChatMessage
                {
                    SessionId = sessionId,
                    Role = "user",
                    Content = userMessage,
                    CreatedAt = DateTime.UtcNow
                });

                _dbContext.AgentChatMessages.Add(new ThyroCareX.Data.Models.AgentChatMessage
                {
                    SessionId = sessionId,
                    Role = "ai",
                    Content = result.response,
                    CreatedAt = DateTime.UtcNow.AddMilliseconds(100) // Ensure AI message comes strictly after
                });

                await _dbContext.SaveChangesAsync();
            }

            return result ?? new AgentChatResponseDto { status = "error", response = "Failed to parse response." };
        }

        public async Task<List<ThyroCareX.Data.Models.AgentSession>> GetPatientSessionsAsync(int patientId)
        {
            return await _dbContext.AgentSessions
                .Where(s => s.PatientId == patientId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ThyroCareX.Data.Models.AgentChatMessage>> GetSessionMessagesAsync(string sessionId)
        {
            return await _dbContext.AgentChatMessages
                .Where(m => m.SessionId == sessionId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }

    }

    public class UltrasoundValidationResponse
    {
        [JsonPropertyName("filename")]
        public string Filename { get; set; }

        [JsonPropertyName("is_ultrasound")]
        public bool IsUltrasound { get; set; }

        [JsonPropertyName("confidence")]
        public double Confidence { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
