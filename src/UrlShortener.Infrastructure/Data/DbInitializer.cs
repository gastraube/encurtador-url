namespace UrlShortener.Infrastructure.Data;

public class DbInitializer
{
    private readonly UrlShortenerDbContext _context;

    public DbInitializer(UrlShortenerDbContext context)
    {
        _context = context;
    }

    public void Initialize()
    {
        _context.Initialize();
    }
}