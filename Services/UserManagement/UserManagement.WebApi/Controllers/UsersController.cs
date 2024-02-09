using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Users.Requests;
using UserManagement.Core.Entities;

namespace UserManagementService.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class UsersController(UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpPost("add")]
    public async Task<IdentityResult> Add(CreateUserRequest request) =>
        await userManager.CreateAsync(
            request.User,
            request.Password);
}