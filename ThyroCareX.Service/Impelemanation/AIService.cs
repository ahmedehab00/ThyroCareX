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
using ThyroCareX.Service.Abstarct;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace ThyroCareX.Service.Impelemanation
{
    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public AIService(HttpClient httpClient, IConfiguration configuration, IWebHostEnvironment env)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _env = env;

            var apiKey = _configuration["AISettings:ApiKey"];
            _httpClient.DefaultRequestHeaders.Add("X-AI-Service-Key", apiKey);
        }
        public async Task<ImageAIResponse> PredictImageAsync(string imagePath)
        {
            var fullPath = Path.Combine(_env.WebRootPath ?? "wwwroot", imagePath.TrimStart('/'));
            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"Image file not found at: {fullPath}");

            using var form = new MultipartFormDataContent();
            var fileBytes = await File.ReadAllBytesAsync(fullPath);
            var byteContent = new ByteArrayContent(fileBytes);
            
            // Set Content-Type
            var extension = Path.GetExtension(fullPath).ToLower();
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                extension == ".png" ? "image/png" : "image/jpeg"
            );

            form.Add(byteContent, "file", Path.GetFileName(fullPath));

            var response = await _httpClient.PostAsync(
                "https://amer003100-thyraxcdss.hf.space/image/predict",
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
                var parsed = JsonSerializer.Deserialize<ImageAIResponse>(raw, options);
                if (parsed == null)
                    throw new JsonException("Empty JSON response from AI image endpoint.");

                NormalizeImageUrls(parsed);
                return parsed;
            }
            catch (JsonException)
            {
                // Fallback parser for schema drift (e.g. classification as string instead of object).
                using var doc = JsonDocument.Parse(raw);
                var root = doc.RootElement;

                var parsed = new ImageAIResponse
                {
                    Status = TryGetString(root, "status") ?? "success",
                    Message = TryGetString(root, "message"),
                    Bbox = TryGetIntList(root, "bbox"),
                    Classification = ParseClassification(root),
                    Images = ParseImages(root)
                };

                NormalizeImageUrls(parsed);
                return parsed;
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

        public async Task<FnacAIResponse> PredictFnacAsync(string imagePath)
        {
            var fullPath = Path.Combine(_env.WebRootPath ?? "wwwroot", imagePath.TrimStart('/'));
            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"FNAC image file not found at: {fullPath}");

            using var form = new MultipartFormDataContent();
            var fileBytes = await File.ReadAllBytesAsync(fullPath);
            var byteContent = new ByteArrayContent(fileBytes);
            
            var extension = Path.GetExtension(fullPath).ToLower();
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                extension == ".png" ? "image/png" : "image/jpeg"
            );

            form.Add(byteContent, "file", Path.GetFileName(fullPath));

            var response = await _httpClient.PostAsync(
                "https://amer003100-thyraxcdss.hf.space/fnac/predict",
                form
            );

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"AI Service FNAC Predict Error ({response.StatusCode}): {errorBody}");
            }

            return await response.Content.ReadFromJsonAsync<FnacAIResponse>();
        }
        public async Task<bool> ValidateUltrasoundAsync(string imagePath)
        {
            var fullPath = Path.Combine(_env.WebRootPath ?? "wwwroot", imagePath.TrimStart('/'));

            using var form = new MultipartFormDataContent();
            var fileBytes = await File.ReadAllBytesAsync(fullPath);
            var byteContent = new ByteArrayContent(fileBytes);

            var extension = Path.GetExtension(fullPath).ToLower();
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                extension == ".png" ? "image/png" : "image/jpeg"
            );

            form.Add(byteContent, "file", Path.GetFileName(fullPath));

            var response = await _httpClient.PostAsync(
                "https://amer003100-thyraxcdss.hf.space/image/validate",
                form
            );

            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadFromJsonAsync<UltrasoundValidationResponse>();

            return result?.IsUltrasound == true;
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

            if (!value.StartsWith("/")) value = "/" + value;
            return $"https://amer003100-thyraxcdss.hf.space{value}";
        }

    }

    internal class UltrasoundValidationResponse
    {
        [JsonPropertyName("is_ultrasound")]
        public bool IsUltrasound { get; set; }
    }
}
