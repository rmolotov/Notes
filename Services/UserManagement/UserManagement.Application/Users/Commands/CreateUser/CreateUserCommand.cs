using UserManagement.Core.Entities;

namespace UserManagement.Application.Users.Commands.CreateUser;

public record CreateUserCommand
{
    public ApplicationUser User { get; set; }
    public string Password { get; set; }
}