using UrlShortener.Application.Services;
using UrlShortener.Application.UseCases;
using UrlShortener.Domain.Interfaces;
using UrlShortener.Infrastructure.Data;
using UrlShortener.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=shortener.db";

builder.Services.AddSingleton(new UrlShortenerDbContext(connectionString));
builder.Services.AddScoped<IUrlRepository, UrlRepository>();
builder.Services.AddScoped<Base62Service>();
builder.Services.AddScoped<ShortenUrlUseCase>();
builder.Services.AddScoped<DbInitializer>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors("AllowAll");

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    initializer.Initialize();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

app.MapGet("/{alias:regex(^[a-zA-Z0-9]+$)}", async (string alias, IUrlRepository repository) =>
{
    var url = await repository.GetByAliasAsync(alias);

    if (url == null)
        return Results.NotFound(new { message = "URL not found." });

    url.AccessCount++;
    await repository.UpdateAsync(url);
    await repository.SaveChangesAsync();

    return Results.Redirect(url.OriginalUrl, permanent: false);
});

app.Run();