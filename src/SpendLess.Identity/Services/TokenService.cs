using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SpendLess.Application.Constants;
using SpendLess.Application.Contracts.Identity;
using SpendLess.Application.Contracts.Persistence;
using SpendLess.Application.Contracts.Persistence.Token;
using SpendLess.Application.DTOs.Users;
using SpendLess.Application.Models.Identity;
using SpendLess.Identity.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SpendLess.Identity.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ApplicationTokenSettings _tokenSettings;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<TokenService> _logger;
        public TokenService(IOptions<JwtSettings> jwtSettings,
                            IOptions<ApplicationTokenSettings> tokenSettings,
                            IUnitOfWork uow,
                            ILogger<TokenService> logger)
        {
            _jwtSettings = jwtSettings.Value;
            _tokenSettings = tokenSettings.Value;
            _uow = uow;
            _logger = logger;
        }

        public string GenerateAccessToken<TClaim>(ICollection<TClaim> claims)
            where TClaim : class
        {
            var claimsList = (IList<Claim>)claims;
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claimsList,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        public string GenerateRefreshToken()
        {
            // to do: generating refresh token logic
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }

        public async Task<ApplicationTokenBase> GetToken(string token, bool isRevoked = false)
        {
            return await _uow.TokenRepository.GetToken(token, isRevoked);
        }

        public async Task<IList<ApplicationTokenBase>> GetTokensByUserId(int userId, bool isRevoked = false)
        {
            var tokensList = await _uow.TokenRepository.GetTokensByUserId(userId, isRevoked);
            return await tokensList.ToListAsync();
        }

        public async Task RevokeToken(string token)
        {
            await _uow.TokenRepository.RevokeToken(token);
            await _uow.Save();
        }

        public async Task SaveToken(int userId, string refreshTokenString)
        {
            var applicationToken = new ApplicationToken()
            {
                UserId = userId,
                TokenString = refreshTokenString,
                ExpirationDate = DateTime.UtcNow.AddDays(_tokenSettings.DurationInDays),
                IsRevoked = false
            };

            await _uow.TokenRepository.Save(applicationToken);
            await _uow.Save();
        }
    }
}
