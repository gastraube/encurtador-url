using Microsoft.Data.Sqlite;

namespace UrlShortener.Infrastructure.Data;

public class UrlShortenerDbContext
{
    private readonly string _connectionString;

    public UrlShortenerDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Initialize()
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS ShortenedUrls (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    OriginalUrl TEXT NOT NULL,
                    Alias TEXT NOT NULL UNIQUE,
                    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    AccessCount INTEGER NOT NULL DEFAULT 0
                );

                CREATE INDEX IF NOT EXISTS idx_alias ON ShortenedUrls(Alias);
            ";

            command.ExecuteNonQuery();
        }
    }

    public SqliteConnection GetConnection()
    {
        return new SqliteConnection(_connectionString);
    }
}