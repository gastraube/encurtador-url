namespace UrlShortener.Application.DTOs;

public class ShortenedUrlResponse
{
    public int Id { get; set; }
    public string OriginalUrl { get; set; } = string.Empty;
    public string ShortenedUrl { get; set; } = string.Empty;
    public string Alias { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int AccessCount { get; set; }
}