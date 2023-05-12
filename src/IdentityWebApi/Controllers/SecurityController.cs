using System.ComponentModel.DataAnnotations;
using IdentityWebApi.Models;
using IdentityWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityWebApi.Controllers;

[ApiController]
[Route("api/security")]
public class SecurityController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public SecurityController(UserManager<ApplicationUser> userManager) => _userManager = userManager;


    [HttpPost("token")]
    public async Task<IActionResult> SignIn([FromBody] UserLoginInputModel inputModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState.Values);
        }
                        
        var user = await _userManager.FindByNameAsync(inputModel.Username);
        if(user is null)
        {
            return NotFound();
        }

        var allowedSignIn = await _userManager.IsEmailConfirmedAsync(user);
        if(!allowedSignIn)
        {
            return BadRequest("Necessário confirmar o email para realizar o login");
        }

        var passwordIsValid = await _userManager.CheckPasswordAsync(user, inputModel.Password);
        if(!passwordIsValid)
        {
            return BadRequest($"Falha ao fazer login com o usuário {user.UserName}");
        }

        var tokenService = HttpContext.RequestServices.GetRequiredService<ITokenService>();
        var token = tokenService.CreateToken(user);

        return Ok(token);
    }

    [Authorize]
    [HttpGet("current-user")]
    public IActionResult GetCurrentUser()
    {
        return Ok(new
        {
            User.Identity.Name,
            Claims = User.Claims.Select(x => new
            {
                x.Issuer,
                x.Properties,
                x.Type,
                x.Value
            })
        });
    }
}

public record UserLoginInputModel
{
    [Required(ErrorMessage = "O nome de usuário é obrigatório")]
    public string Username { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória")]
    public string Password { get; set; }
};
