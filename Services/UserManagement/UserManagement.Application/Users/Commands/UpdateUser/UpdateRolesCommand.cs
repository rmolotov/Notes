namespace UserManagement.Application.Users.Commands.UpdateUser;

public record UpdateRolesCommand
{
    public string UserName { get; set; }

    public string[] RolesNames { get; set; }
}