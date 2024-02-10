using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Roles.UpdateRole;

namespace UserManagementService.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class RolesController(RoleManager<IdentityRole> roleManager) : ControllerBase
{
    [HttpGet]
    public async Task<IdentityRole?> Get(string roleName)
    {
        return await roleManager.FindByNameAsync(roleName);
    }

    [HttpGet("all")]
    public IEnumerable<IdentityRole> GetAll()
    {
        return roleManager.Roles.AsEnumerable();
    }
    
    [HttpPost]
    public async Task<IdentityResult> Add([FromBody] string roleName)
    {
        return await roleManager.CreateAsync(new IdentityRole(roleName));
    }
    
    [HttpPut]
    public async Task<IdentityResult> Update([FromBody] UpdateRoleCommand command)
    {
        var targetRole = await roleManager.FindByNameAsync(command.currentRoleName);

        if (targetRole == null)
            return IdentityResult.Failed(
                new IdentityError
                {
                    Description = $"Role {command.currentRoleName} not found"
                });

        targetRole.Name = command.NewRoleName;

        return await roleManager.UpdateAsync(targetRole);
    }
    
    [HttpDelete]
    public async Task<IdentityResult> Remove([FromBody] string roleName)
    {
        var targetRole = await roleManager.FindByNameAsync(roleName);

        if (targetRole == null)
            return IdentityResult.Failed(
                new IdentityError
                {
                    Description = $"Role {roleName} not found"
                });
        
        return await roleManager.DeleteAsync(targetRole);
    }
}