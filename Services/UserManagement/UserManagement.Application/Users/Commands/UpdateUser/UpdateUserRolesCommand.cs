namespace UserManagement.Application.Users.Commands.UpdateUser;

public record UpdateUserRolesCommand
{
    public string UserName { get; set; }

    public string[] RolesNames { get; set; }
}