using System.ComponentModel.DataAnnotations;

namespace IdentityDemoProject.Models.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
