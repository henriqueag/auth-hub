using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityWebApi.Controllers;

[ApiController]
[Route("api/roles")]
public class RoleController : ControllerBase
{    
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateRoleInputModel inputModel)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState.Values);
        }

        var role = new IdentityRole(inputModel.RoleName);
        var result = await _roleManager.CreateAsync(role);

        return result.Succeeded
            ? Created($"/api/roles/{role.Name}", role.Name)
            : BadRequest(result.Errors);
    }

    [HttpPut("{roleName}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] string roleName, [FromBody] UpdateRoleInputModel inputModel) 
    {
        var toUpdate = await _roleManager.FindByNameAsync(roleName);
        if (toUpdate is null)
        {
            return NotFound();
        }

        toUpdate.Name = inputModel.RoleName;
        var result = await _roleManager.UpdateAsync(toUpdate);

        return result.Succeeded
            ? Ok(toUpdate)
            : BadRequest(result.Errors);
    }

    [HttpDelete("{roleName}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] string roleName)
    {
        var toDelete = await _roleManager.FindByNameAsync(roleName);
        if(toDelete is null)
        {
            return NotFound();
        }

        var result = await _roleManager.DeleteAsync(toDelete);

        return result.Succeeded
            ? NoContent()
            : BadRequest(result.Errors);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        var roles = await _roleManager.Roles.ToListAsync(cancellationToken);
        return Ok(roles);
    }

    [HttpGet("{roleName}")]
    public async Task<IActionResult> GetByNameAsync([FromRoute] string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);

        return role is null
            ? NotFound()
            : Ok(role);
    }
}

public record CreateRoleInputModel
{
    [Required(ErrorMessage = "O nome da role deve ser informado")]
    [MinLength(6, ErrorMessage = "O nome da role deve ter no mínimo 6 caracteres")]
    public string RoleName { get; set; }
}

public record UpdateRoleInputModel
{
    [Required(ErrorMessage = "O nome da role deve ser informado")]
    [MinLength(6, ErrorMessage = "O nome da role deve ter no mínimo 6 caracteres")]
    public string RoleName { get; set; }
}