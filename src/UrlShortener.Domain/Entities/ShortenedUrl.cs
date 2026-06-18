namespace UrlShortener.Domain.Entities;

public class ShortenedUrl
{
    public int Id { get; set; }
    public string OriginalUrl { get; set; } = string.Empty;
    public string Alias { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int AccessCount { get; set; } = 0;
}