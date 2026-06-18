namespace UrlShortener.Application.DTOs;

public class CreateShortenedUrlRequest
{
    public string OriginalUrl { get; set; } = string.Empty;
    public string? CustomAlias { get; set; }
}