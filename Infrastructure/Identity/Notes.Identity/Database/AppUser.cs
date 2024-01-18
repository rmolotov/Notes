using Microsoft.AspNetCore.Identity;

namespace Notes.Identity.Database;

public class AppUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}