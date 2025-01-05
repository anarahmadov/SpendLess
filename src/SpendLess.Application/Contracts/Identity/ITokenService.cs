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
        string GenerateAccessToken<TClaim>(ICollection<TClaim> claims) where TClaim : class;
        string GenerateRefreshToken();
        Task SaveToken(int userId, string refreshTokenString);
        Task RevokeToken(string token);
        Task<ApplicationTokenBase> GetToken(string token, bool isRevoked = false);
        Task<IList<ApplicationTokenBase>> GetTokensByUserId(int userId, bool isRevoked = false);
    }
}
