using System.ComponentModel.DataAnnotations;
using System.Text;
using IdentityWebApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityWebApi.Controllers;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateUserInputModel inputModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState.Values);
        }

        var user = new ApplicationUser(inputModel.Username, inputModel.Email);
        var result = await _userManager.CreateAsync(user, inputModel.Password);

        return result.Succeeded
            ? Created($"api/account/{user.Id}", user.Id)
            : BadRequest(result.Errors);
    }

    [HttpDelete("{userId:Guid}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid userId)
    {
        var toDelete = await _userManager.FindByIdAsync(userId.ToString());
        if (toDelete is null)
        {
            return NotFound();
        }

        var result = await _userManager.DeleteAsync(toDelete);

        return result.Succeeded
            ? NoContent()
            : BadRequest(result.Errors);
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] Guid? userId, [FromQuery] string email, [FromQuery] string username)
    {
        ApplicationUser userResult;

        if (userId.HasValue)
        {
            userResult = await _userManager.FindByIdAsync(userId.Value.ToString());
            return userResult is not null
                ? Ok(new GetUserViewModel(userResult))
                : NotFound();
        }

        if (!string.IsNullOrEmpty(email))
        {
            userResult = await _userManager.FindByEmailAsync(email);
            return userResult is not null
                ? Ok(new GetUserViewModel(userResult))
                : NotFound();
        }

        if(!string.IsNullOrEmpty(username))
        {
            userResult = await _userManager.FindByNameAsync(username);
            return userResult is not null
                    ? Ok(new GetUserViewModel(userResult))
                    : NotFound();
        }

        var users = await _userManager.Users
            .Select(user => new GetUserViewModel(user))
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("{userId:Guid}/email-confirmation-token")]
    public async Task<IActionResult> GetEmailConfirmationTokenAsync([FromRoute] Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return NotFound();
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var tokenBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));

        return Ok(new { EmailConfirmationToken = tokenBase64 });
    }

    [HttpGet("{userId:Guid}/confirm-email")]
    public async Task<IActionResult> ConfirmEmailAsync([FromRoute] Guid userId, [FromQuery] string confirmationToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return NotFound();
        }

        var tokenBytes = Convert.FromBase64String(confirmationToken);
        var tokenAsString = Encoding.UTF8.GetString(tokenBytes);
        var result = await _userManager.ConfirmEmailAsync(user, tokenAsString);

        return result.Succeeded
            ? NoContent()
            : BadRequest(result.Errors);
    }

    [HttpGet("{userId:Guid}/password-reset-token")]
    public async Task<IActionResult> GetPasswordResetTokenAsync([FromRoute] Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return NotFound();
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var tokenBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));

        return Ok(new { PasswordResetToken = tokenBase64 });
                
        
    }

    [HttpGet("{userId:Guid}/reset-password")]
    public async Task<IActionResult> ResetPasswordAsync([FromRoute] Guid userId, [FromQuery] string passwordResetToken, [FromQuery] string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return NotFound();
        }

        var tokenBytes = Convert.FromBase64String(passwordResetToken);
        var tokenAsString = Encoding.UTF8.GetString(tokenBytes);
        var result = await _userManager.ResetPasswordAsync(user, tokenAsString, newPassword);

        return result.Succeeded
            ? NoContent()
            : BadRequest(result.Errors);
    }

    [HttpPut("{userId:Guid}/add-role")]
    public async Task<IActionResult> AddToRoleAsync([FromRoute] Guid userId, [FromBody] AddOrRemoveRoleInputModel inputModel)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return NotFound();
        }

        var roleExists = await _roleManager.RoleExistsAsync(inputModel.RoleName);
        if(!roleExists)
        {
            return BadRequest(new { Message = $"A role {inputModel.RoleName} não existe" });
        }

        var result = await _userManager.AddToRoleAsync(user, inputModel.RoleName);

        return result.Succeeded
            ? NoContent()
            : BadRequest(result.Errors);
    }

    [HttpPut("{userId:Guid}/remove-role")]
    public async Task<IActionResult> RemoveFromRoleAsync([FromRoute] Guid userId, [FromBody] AddOrRemoveRoleInputModel inputModel)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return NotFound();
        }

        var roleExists = await _roleManager.RoleExistsAsync(inputModel.RoleName);
        if (!roleExists)
        {
            return BadRequest(new { Message = $"A role {inputModel.RoleName} não existe" });
        }

        var result = await _userManager.RemoveFromRoleAsync(user, inputModel.RoleName);

        return result.Succeeded
            ? NoContent()
            : BadRequest(result.Errors);
    }
}

public record CreateUserInputModel
{
    [Required(ErrorMessage = "O nome de usuário é obrigatório")]
    [MinLength(6, ErrorMessage = "O tamanho mínimo para o nome de usuário é 6 caracteres")]
    [MaxLength(14, ErrorMessage = "O tamanho máximo para o nome de usuário é 14 caracteres")]
    public string Username { get; set; }

    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "Formato de e-mail inválido")]
    public string Email { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória")]
    [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
    public string Password { get; set; }

    [Compare(nameof(Password), ErrorMessage = "Senha incorreta")]
    public string ConfirmPassword { get; set; }
}

public record GetUserViewModel
{
    public GetUserViewModel(ApplicationUser user)
    {
        Id = user.Id;
        Username = user.UserName;
        Email = user.Email;
    }

    public string Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
}

public record AddOrRemoveRoleInputModel
{
    public string RoleName { get; set; }
}