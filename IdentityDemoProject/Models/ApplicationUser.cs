using Microsoft.AspNetCore.Identity;

namespace IdentityDemoProject.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string Lastname { get; set; }
    }
}
