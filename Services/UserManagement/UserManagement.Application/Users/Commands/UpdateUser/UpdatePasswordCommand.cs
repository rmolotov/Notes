namespace UserManagement.Application.Users.Commands.UpdateUser;

public record UpdatePasswordCommand
{
    public string UserName { get; set; }

    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
}