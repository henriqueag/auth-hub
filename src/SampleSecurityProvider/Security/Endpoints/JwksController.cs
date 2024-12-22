using Microsoft.AspNetCore.Mvc;
using SampleSecurityProvider.Security;
using SampleSecurityProvider.Security.Services;

namespace SampleSecurityProvider.Controllers;

[ApiController]
[Route(".well-known/jwks.json")]
public class JwksController(IJwksManager jwksManager) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var jwks = jwksManager.GetJwks();
        var result = new
        {
            Keys = jwks.Keys
                .Select(key => new { key.Kty, key.Kid, key.Use, key.Alg, key.N, key.E })
                .ToList()
        };
    
        return Ok(result);
    }
}