using Microsoft.AspNetCore.Identity;
using SpendLess.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendLess.Identity.Helpers
{
    public interface IPasswordHelper
    {
        string HashPassword(ApplicationUser user, string password);
        bool VerifyPassword(ApplicationUser user, string hashedPassword, string password);
    }

    public class PasswordHelper : IPasswordHelper
    {
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        public PasswordHelper(IPasswordHasher<ApplicationUser> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public string HashPassword(ApplicationUser user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public bool VerifyPassword(ApplicationUser user, string hashedPassword, string password)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, password);
            // if required, you can handle if result is SuccessRehashNeeded
            return result == PasswordVerificationResult.Success;
        }
    }
}
