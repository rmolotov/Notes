using UserManagement.Core.Entities;

namespace UserManagement.Application.Users.Requests;

public class CreateUserRequest
{
    public ApplicationUser User { get; set; }
    public string Password { get; set; }
}