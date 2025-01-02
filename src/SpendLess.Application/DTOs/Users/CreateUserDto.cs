using SpendLess.Application.DTOs.Common;

namespace SpendLess.Application.DTOs.Users
{
    public class CreateUserDto : BaseDto
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        
    }
}
