using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Users.Commands.CreateUser;
using UserManagement.Application.Users.Commands.UpdateUser;
using UserManagement.Core.Entities;

namespace UserManagementService.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class UsersController(UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpGet]
    public async Task<ApplicationUser?> Get(string username)
    {
        return await userManager.FindByNameAsync(username);
    }

    [HttpGet("all")]
    public IEnumerable<ApplicationUser> GetAll()
    {
        return userManager.Users.AsEnumerable();
    }
    
    [HttpPost]
    public async Task<IdentityResult> Add([FromBody] CreateUserCommand command)
    {
        return await userManager.CreateAsync(
            command.User,
            command.Password);
    }

    [HttpPut]
    public async Task<IdentityResult> Update([FromBody] UpdateUserCommand command)
    {
        var targetUser = await userManager.FindByNameAsync(command.UserName);

        if (targetUser == null)
            return IdentityResult.Failed(
                new IdentityError
                {
                    Description = $"User {command.UserName} not found"
                });

        targetUser.NickName = command.NickName ?? targetUser.NickName;
        targetUser.Email = command.Email ?? targetUser.Email;
        targetUser.Level = command.Level ?? targetUser.Level;

        return await userManager.UpdateAsync(targetUser);
    }

    [HttpDelete]
    public async Task<IdentityResult> Remove([FromBody] string userName)
    {
        var targetUser = await userManager.FindByNameAsync(userName);

        if (targetUser == null)
            return IdentityResult.Failed(
                new IdentityError
                {
                    Description = $"User {userName} not found"
                });
        
        return await userManager.DeleteAsync(targetUser);
    }

    [HttpPut("changePassword")]
    public async Task<IdentityResult> ChangePassword([FromBody] UpdatePasswordCommand command)
    {
        var targetUser = await userManager.FindByNameAsync(command.UserName);
        
        if (targetUser == null)
            return IdentityResult.Failed(
                new IdentityError
                {
                    Description = $"User {command.UserName} not found"
                });

        return await userManager.ChangePasswordAsync(
            targetUser,
            command.CurrentPassword,
            command.NewPassword
        );
    }

    [HttpPut("addRoles")]
    public async Task<IdentityResult> AddRoles([FromBody] UpdateUserRolesCommand command)
    {
        var targetUser = await userManager.FindByNameAsync(command.UserName);
        
        if (targetUser == null)
            return IdentityResult.Failed(
                new IdentityError
                {
                    Description = $"User {command.UserName} not found"
                });
        
        return await userManager.AddToRolesAsync(
            targetUser,
            command.RolesNames);
    }
    
    [HttpPut("removeRoles")]
    public async Task<IdentityResult> RemoveRoles([FromBody] UpdateUserRolesCommand command)
    {
        var targetUser = await userManager.FindByNameAsync(command.UserName);
        
        if (targetUser == null)
            return IdentityResult.Failed(
                new IdentityError
                {
                    Description = $"User {command.UserName} not found"
                });
        
        return await userManager.RemoveFromRolesAsync(
            targetUser,
            command.RolesNames);
    }
}