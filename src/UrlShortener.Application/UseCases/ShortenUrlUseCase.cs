using UrlShortener.Application.DTOs;
using UrlShortener.Application.Services;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Interfaces;

namespace UrlShortener.Application.UseCases;

public class ShortenUrlUseCase
{
    private readonly IUrlRepository _repository;
    private readonly Base62Service _base62Service;

    private static readonly SemaphoreSlim _gate = new(1, 1);
    private static long _sequenceCounter = 916132199; // Base62: "jyzab" (5 caracteres)

    public ShortenUrlUseCase(IUrlRepository repository, Base62Service base62Service)
    {
        _repository = repository;
        _base62Service = base62Service;
    }

    public async Task<ShortenedUrlResponse> ExecuteAsync(CreateShortenedUrlRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.OriginalUrl))
            throw new ArgumentException("Original URL is required.");

        await _gate.WaitAsync();
        try
        {
            string alias;

            if (!string.IsNullOrWhiteSpace(request.CustomAlias))
            {
                alias = request.CustomAlias.Trim().ToLower();

                if (await _repository.AliasExistsAsync(alias))
                    throw new InvalidOperationException($"Alias '{alias}' is already in use.");
            }
            else
            {
                alias = _base62Service.ToBase62(_sequenceCounter);
                _sequenceCounter++;

                while (await _repository.AliasExistsAsync(alias))
                {
                    alias = _base62Service.ToBase62(_sequenceCounter);
                    _sequenceCounter++;
                }
            }

            var shortenedUrl = new ShortenedUrl
            {
                OriginalUrl = request.OriginalUrl.Trim(),
                Alias = alias,
                CreatedAt = DateTime.UtcNow,
                AccessCount = 0
            };

            await _repository.AddAsync(shortenedUrl);
            await _repository.SaveChangesAsync();

            return new ShortenedUrlResponse
            {
                Id = shortenedUrl.Id,
                OriginalUrl = shortenedUrl.OriginalUrl,
                Alias = shortenedUrl.Alias,
                ShortenedUrl = $"/{shortenedUrl.Alias}",
                CreatedAt = shortenedUrl.CreatedAt,
                AccessCount = shortenedUrl.AccessCount
            };
        }
        finally
        {
            _gate.Release();
        }
    }
}