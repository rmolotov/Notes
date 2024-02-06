using Microsoft.AspNetCore.Identity;

namespace UserManagement.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string NickName { get; set; }
        public int Level { get; set; }
    }
}
