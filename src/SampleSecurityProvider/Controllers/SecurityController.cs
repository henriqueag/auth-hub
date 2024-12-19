using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SampleSecurityProvider.Models;
using SampleSecurityProvider.Security.Services;

namespace SampleSecurityProvider.Controllers;

[ApiController]
[Route("api/security")]
public class SecurityController(UserManager<ApplicationUser> userManager, IAuthenticationService authService) : ControllerBase
{
    [HttpPost("token")]
    public async Task<IResult> CreateTokenAsync([FromForm] IDictionary<string, string?> payload)
    {
        var tokenRequest = new TokenRequest(payload);
        
        if (tokenRequest.GrantType != "password")
        {
            return Results.BadRequest("Invaild grant type");
        }
        
        var user = await userManager.FindByEmailAsync(tokenRequest.Email ?? string.Empty);
        if (user is null)
        {
            return Results.NotFound();
        }

        var token = await authService.CreateTokenAsync(user, tokenRequest.Password ?? string.Empty);
        return Results.Ok(token);
    }

    [Authorize]
    [HttpPost("refresh-token")]
    public async Task<IResult> RefreshTokenAsync([FromForm] IDictionary<string, string?> payload)
    {
        var tokenRequest = new TokenRequest(payload);
        
        if (tokenRequest.GrantType != "refresh_token")
        {
            return Results.BadRequest("Invaild grant type");
        }

        if (string.IsNullOrEmpty(tokenRequest.RefreshToken))
        {
            return Results.BadRequest("Refresh token required");
        }
        
        var user = await userManager.FindByIdAsync(User.Identity!.Name ?? Guid.Empty.ToString());
        if (user is null)
        {
            return Results.NotFound();
        }

        var token = await authService.RefreshTokenAsync(user, tokenRequest.RefreshToken);
        return Results.Ok(token);
    }
}

public record TokenRequest(IDictionary<string, string?> Data)
{
    public string? GrantType => GetValueOrDefault("grant_type");
    public string? Email => GetValueOrDefault("email");
    public string? Password => GetValueOrDefault("password");
    public string? RefreshToken => GetValueOrDefault("refresh_token");
    
    private string? GetValueOrDefault(string key) => Data.TryGetValue(key, out var value) ? value : null;
};