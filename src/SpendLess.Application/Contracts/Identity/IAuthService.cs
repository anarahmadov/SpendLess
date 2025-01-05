using SpendLess.Application.DTOs.Users;
using SpendLess.Application.Models.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SpendLess.Application.Contracts.Identity
{
    public interface IAuthService
    {
        Task Logout();
        Task<AuthResponse> Login(AuthRequest request);
        Task<RegistrationResponse> Register(RegistrationRequest request);
        IEnumerable<Claim> CreateUserClaims(UserDto user);
    }
}
