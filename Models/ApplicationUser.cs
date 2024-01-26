using System.Security.Claims;
using blog_api.Enums;
using Microsoft.AspNet.Identity.EntityFramework;

namespace blog_api.Models
{
    public class ApplicationUser : IdentityUser
    {
        public Role Role { get; set; }
    }
}
