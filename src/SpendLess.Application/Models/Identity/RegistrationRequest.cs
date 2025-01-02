using SpendLess.Application.Models.Identity;
using System.ComponentModel.DataAnnotations;

namespace SpendLess.Application.Models.Identity
{
    public class RegistrationRequest
    {
        [Required] 
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        public string UserName { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }
    }
}
