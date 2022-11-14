using Microsoft.AspNetCore.Identity;

namespace API_JWT.Entities
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; } = null!;
    }
}
