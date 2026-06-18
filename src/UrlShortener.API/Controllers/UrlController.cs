using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.DTOs;
using UrlShortener.Application.UseCases;

namespace UrlShortener.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UrlController : ControllerBase
{
    private readonly ShortenUrlUseCase _shortenUrlUseCase;

    public UrlController(ShortenUrlUseCase shortenUrlUseCase)
    {
        _shortenUrlUseCase = shortenUrlUseCase;
    }

    [HttpPost("shorten")]
    public async Task<ActionResult<ShortenedUrlResponse>> ShortenUrl([FromBody] CreateShortenedUrlRequest request)
    {
        try
        {
            var result = await _shortenUrlUseCase.ExecuteAsync(request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

}