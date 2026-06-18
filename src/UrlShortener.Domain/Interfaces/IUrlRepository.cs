using UrlShortener.Domain.Entities;

namespace UrlShortener.Domain.Interfaces;

public interface IUrlRepository
{
    Task<ShortenedUrl?> GetByAliasAsync(string alias);
    Task<ShortenedUrl?> GetByIdAsync(int id);
    Task<bool> AliasExistsAsync(string alias);
    Task AddAsync(ShortenedUrl url);
    Task UpdateAsync(ShortenedUrl url);
    Task SaveChangesAsync();
}