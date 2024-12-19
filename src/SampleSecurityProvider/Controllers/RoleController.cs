using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SampleSecurityProvider.Controllers;

[ApiController]
[Route("api/roles")]
public class RoleController(RoleManager<IdentityRole> roleManager) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateRoleInputModel inputModel)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState.Values);
        }

        var role = new IdentityRole(inputModel.RoleName);
        var result = await roleManager.CreateAsync(role);

        return result.Succeeded
            ? Created($"/api/roles/{role.Name}", role.Name)
            : BadRequest(result.Errors);
    }

    [HttpPut("{roleName}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] string roleName, [FromBody] UpdateRoleInputModel inputModel) 
    {
        var toUpdate = await roleManager.FindByNameAsync(roleName);
        if (toUpdate is null)
        {
            return NotFound();
        }

        toUpdate.Name = inputModel.RoleName;
        var result = await roleManager.UpdateAsync(toUpdate);

        return result.Succeeded
            ? Ok(toUpdate)
            : BadRequest(result.Errors);
    }

    [HttpDelete("{roleName}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] string roleName)
    {
        var toDelete = await roleManager.FindByNameAsync(roleName);
        if(toDelete is null)
        {
            return NoContent();
        }

        var result = await roleManager.DeleteAsync(toDelete);

        return result.Succeeded
            ? NoContent()
            : BadRequest(result.Errors);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        var roles = await roleManager.Roles.ToListAsync(cancellationToken);
        return Ok(roles);
    }

    [HttpGet("{roleName}")]
    public async Task<IActionResult> GetByNameAsync([FromRoute] string roleName)
    {
        var role = await roleManager.FindByNameAsync(roleName);

        return role is null
            ? NotFound()
            : Ok(role);
    }
}

public record CreateRoleInputModel
{
    [Required(ErrorMessage = "O nome da role deve ser informado")]
    [MinLength(6, ErrorMessage = "O nome da role deve ter no mínimo 4 caracteres")]
    public string RoleName { get; init; } = null!;
}

public record UpdateRoleInputModel
{
    [Required(ErrorMessage = "O nome da role deve ser informado")]
    [MinLength(6, ErrorMessage = "O nome da role deve ter no mínimo 4 caracteres")]
    public string RoleName { get; init; } = null!;
}