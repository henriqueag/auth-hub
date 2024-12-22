using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleSecurityProvider.Users.Entities;

namespace SampleSecurityProvider.Users.Endpoints.Accounts;

[ApiController]
[Route("api/account")]
public class AccountController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateUserInputModel inputModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState.Values);
        }

        var user = new User(inputModel.Username, inputModel.Email)
        {
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(user, inputModel.Password);

        return result.Succeeded
            ? Created($"api/account/{user.Id}", user.Id)
            : BadRequest(result.Errors);
    }

    [HttpDelete("{userId:Guid}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid userId)
    {
        var toDelete = await userManager.FindByIdAsync(userId.ToString());
        if (toDelete is null)
        {
            return NoContent();
        }

        var result = await userManager.DeleteAsync(toDelete);

        return result.Succeeded
            ? NoContent()
            : BadRequest(result.Errors);
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] Guid? userId, [FromQuery] string? email, [FromQuery] string? username)
    {
        User? userResult;

        if (userId.HasValue)
        {
            userResult = await userManager.FindByIdAsync(userId.Value.ToString());
            return userResult is not null
                ? Ok(new GetUserViewModel(userResult))
                : NotFound();
        }

        if (!string.IsNullOrEmpty(email))
        {
            userResult = await userManager.FindByEmailAsync(email);
            return userResult is not null
                ? Ok(new GetUserViewModel(userResult))
                : NotFound();
        }

        if(!string.IsNullOrEmpty(username))
        {
            userResult = await userManager.FindByNameAsync(username);
            return userResult is not null
                    ? Ok(new GetUserViewModel(userResult))
                    : NotFound();
        }

        var users = await userManager.Users
            .Select(user => new GetUserViewModel(user))
            .ToListAsync();

        return Ok(users);
    }

    [HttpPut("{userId:Guid}/add-to-roles")]
    public async Task<IActionResult> AddToRolesAsync([FromRoute] Guid userId, [FromBody] IEnumerable<string> roles)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return NotFound();
        }

        foreach (var role in roles)
        {
            var roleExists = await roleManager.RoleExistsAsync(role);
            if(!roleExists)
            {
                return BadRequest(new { Message = $"A role {role} não existe" });
            }
            
            var result = await userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
        }

        return NoContent();
    }

    [HttpPut("{userId:Guid}/remove-from-roles")]
    public async Task<IActionResult> RemoveFromRolesAsync([FromRoute] Guid userId, [FromBody] IEnumerable<string> roles)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return NotFound();
        }

        foreach (var role in roles)
        {
            var roleExists = await roleManager.RoleExistsAsync(role);
            if (!roleExists)
            {
                return BadRequest(new { Message = $"A role {role} não existe" });
            }

            var result = await userManager.RemoveFromRoleAsync(user, role);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
        }
        
        return NoContent();
    }
}

public record CreateUserInputModel
{
    [Required(ErrorMessage = "O nome de usuário é obrigatório")]
    [MinLength(6, ErrorMessage = "O tamanho mínimo para o nome de usuário é 4 caracteres")]
    [MaxLength(14, ErrorMessage = "O tamanho máximo para o nome de usuário é 14 caracteres")]
    public string Username { get; init; } = string.Empty;

    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "Formato de e-mail inválido")]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória")]
    [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
    public string Password { get; init; } = string.Empty;

    [Compare(nameof(Password), ErrorMessage = "Senha incorreta")]
    public string ConfirmPassword { get; init; } = string.Empty;
}

public record GetUserViewModel
{
    public GetUserViewModel(User user)
    {
        Id = user.Id;
        Username = user.UserName!;
        Email = user.Email!;
    }

    public string Id { get; init; }
    public string Username { get; init; }
    public string Email { get; init; }
}