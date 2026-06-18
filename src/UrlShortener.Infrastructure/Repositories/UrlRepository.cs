using Dapper;
using Microsoft.Data.Sqlite;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Interfaces;
using UrlShortener.Infrastructure.Data;

namespace UrlShortener.Infrastructure.Repositories;

public class UrlRepository : IUrlRepository
{
    private readonly UrlShortenerDbContext _context;

    public UrlRepository(UrlShortenerDbContext context)
    {
        _context = context;
    }

    public async Task<ShortenedUrl?> GetByAliasAsync(string alias)
    {
        using (var connection = _context.GetConnection())
        {
            connection.Open();
            var lowerAlias = alias.ToLower();
            var sql = "SELECT Id, OriginalUrl, Alias, CreatedAt, AccessCount FROM ShortenedUrls WHERE Alias = @Alias COLLATE NOCASE";
            return await connection.QueryFirstOrDefaultAsync<ShortenedUrl>(sql, new { Alias = lowerAlias });
        }
    }

    public async Task<ShortenedUrl?> GetByIdAsync(int id)
    {
        using (var connection = _context.GetConnection())
        {
            connection.Open();
            var sql = "SELECT Id, OriginalUrl, Alias, CreatedAt, AccessCount FROM ShortenedUrls WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<ShortenedUrl>(sql, new { Id = id });
        }
    }
    public async Task<bool> AliasExistsAsync(string alias)
    {
        using (var connection = _context.GetConnection())
        {
            connection.Open();
            var lowerAlias = alias.ToLower();
            var sql = "SELECT COUNT(1) FROM ShortenedUrls WHERE Alias = @Alias COLLATE NOCASE";
            var count = await connection.QueryFirstOrDefaultAsync<int>(sql, new { Alias = lowerAlias });
            return count > 0;
        }
    }

    public async Task AddAsync(ShortenedUrl url)
    {
        using (var connection = _context.GetConnection())
        {
            connection.Open();
            var sql = @"
                INSERT INTO ShortenedUrls (OriginalUrl, Alias, CreatedAt, AccessCount)
                VALUES (@OriginalUrl, @Alias, @CreatedAt, @AccessCount)
            ";
            await connection.ExecuteAsync(sql, url);
        }
    }

    public async Task UpdateAsync(ShortenedUrl url)
    {
        using (var connection = _context.GetConnection())
        {
            connection.Open();
            var sql = @"
                UPDATE ShortenedUrls 
                SET OriginalUrl = @OriginalUrl, Alias = @Alias, AccessCount = @AccessCount 
                WHERE Id = @Id
            ";
            await connection.ExecuteAsync(sql, url);
        }
    }

    public async Task SaveChangesAsync()
    {
        // Dapper não tem savechanges, cada operação é já commitada
        await Task.CompletedTask;
    }
}