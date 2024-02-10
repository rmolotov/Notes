namespace UserManagement.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand
{
    public string UserName { get; set; }

    public string? NickName { get; set; }
    public string? Email { get; set; }
    public int? Level { get; set; }
}