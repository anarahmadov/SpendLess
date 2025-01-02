using SpendLess.Application.DTOs.Users;
using SpendLess.Application.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SpendLess.Application.Contracts.Identity
{
    public interface ITokenService
    {
        IToken GenerateAccessToken(UserDto user);
        string GenerateRefreshToken();
        Task SaveToken(int userId, string refreshTokenString);
        //CustomToken? ValidateToken(string token);
    }
}
