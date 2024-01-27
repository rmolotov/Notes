using System.Security.Claims;
using Notes.Application.Interfaces;

namespace Notes.WebApi.Services;

public class CurrentUserService(IHttpContextAccessor contextAccessor) : ICurrentUserService
{
    public Guid UserId
    {
        get
        {
            var id = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return string.IsNullOrEmpty(id) 
                ? Guid.Empty 
                : Guid.Parse(id);
        }
    }
}