using Microsoft.Extensions.Configuration;
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
        public TokenService(IOptions<JwtSettings> jwtSettings,
                            IOptions<ApplicationTokenSettings> tokenSettings,
                            IUnitOfWork uow)
        {
            _jwtSettings = jwtSettings.Value;
            _tokenSettings = tokenSettings.Value;
            _uow = uow;
        }

        public IToken GenerateAccessToken(UserDto user)
        {
            var userRole = user.RoleName;

            var roleClaims = new Claim[]{
                new Claim(ClaimTypes.Role, userRole)
            };

            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(CustomClaimTypes.Uid, user.Id.ToString())
            }
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

        public string GenerateRefreshToken()
        {
            // to do: generating refresh token logic
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
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
