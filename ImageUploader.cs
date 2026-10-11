// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 TheMonHub

using System.Text.Json;

namespace JimmyRewrite;

public static class ImageUploader
{
    private static readonly string? ApiKey = Program.ConfigManager["Imgbb:Key"];
    private static readonly string BaseUrl = $"https://api.imgbb.com/1/upload?key={ApiKey}";
    
    public class ImageUploadFailedException : Exception
    {
        public ImageUploadFailedException(string message)
            : base(message) { }
        public ImageUploadFailedException(string message, Exception innerException)
            : base(message, innerException) { }
        public ImageUploadFailedException() { }
    }
    
    public static async Task<string> UploadFromUrl(string url)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ApiKey))
            {
                throw new ImageUploadFailedException("Imgbb API key is missing.");
            }
        
            var request = new HttpRequestMessage(HttpMethod.Post, BaseUrl);
        
            request.Content = new MultipartFormDataContent
            {
                { new StringContent(url), "image" }
            };
        
            var response = await Program.Client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);

            if (jsonDoc.RootElement.GetProperty("success").GetBoolean())
            {
                return jsonDoc.RootElement.GetProperty("data").GetProperty("url").GetString() ?? throw new ImageUploadFailedException();
            }
            throw new ImageUploadFailedException();
        }
        catch (Exception e)
        {
            throw new ImageUploadFailedException("Cannot upload image", e);
        }
    }
}