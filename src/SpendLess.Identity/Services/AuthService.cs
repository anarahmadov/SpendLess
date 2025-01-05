using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SpendLess.Application.Constants;
using SpendLess.Application.Contracts.Identity;
using SpendLess.Application.DTOs.Users;
using SpendLess.Application.Exceptions;
using SpendLess.Application.Models.Identity;
using SpendLess.Identity.Factories;
using SpendLess.Identity.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SpendLess.Identity.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<AuthService> _logger;
        public AuthService(IUserService userService,
                           ITokenService tokenService,
                           IHttpContextAccessor contextAccessor,
                           ILogger<AuthService> logger)
        {
            _userService = userService;
            _tokenService = tokenService;
            _contextAccessor = contextAccessor;
            _logger = logger;
        }
        [Authorize]
        public async Task Logout()
        {
            var userId = _contextAccessor?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var userTokens = await _tokenService.GetTokensByUserId(Convert.ToInt32(userId), isRevoked: false);
            foreach (var userToken in userTokens)
            {
                await _tokenService.RevokeToken(userToken.TokenString);
            }

            _logger.LogDebug("User with {UserId} id logged out", userId);
        }
        public async Task<AuthResponse> Login(AuthRequest request)
        {
            var (result, user) = await _userService.CheckUserByEmailAndPassword(request.Email, request.Password);
            if (!result)
                throw new BadRequestException($"Credentials for '{request.Email}' aren't valid.");

            var claims = CreateUserClaims(user);

            var accessToken = _tokenService.GenerateAccessToken(claims.ToList());
            _logger.LogDebug("User {User} generated new access token at {@UtcNow}", 
                new { user.Id, user.Email, user.Username }, DateTime.UtcNow);

            var refreshToken = _tokenService.GenerateRefreshToken();
            _logger.LogDebug("User {User} generated new refresh token at {@UtcNow}",
                new { user.Id, user.Email, user.Username }, DateTime.UtcNow);

            await _tokenService.SaveToken(user.Id, refreshToken);
            _logger.LogDebug("User {User} logged in at {@UtcNow}", 
                new { user.Id, user.Email, user.Username }, DateTime.UtcNow);

            AuthResponse response = new AuthResponse
            {
                Id = user.Id,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Email = user.Email,
                UserName = user.Username
            };

            return response;
        }
        public async Task<RegistrationResponse> Register(RegistrationRequest request)
        {
            var existingUser = await _userService.GetUserByUsername(request.UserName);
            if (existingUser != null)
                throw new BadRequestException($"Username '{request.UserName}' already exists.");

            var existingUserByEmail = await _userService.GetUserByEmail(request.Email);
            if (existingUserByEmail != null)
                throw new BadRequestException($"Email {request.Email} already exists.");

            try
            {
                int userId = await _userService.CreateUser(UserFactory.CreateUserDto(request));
                _logger.LogDebug("User {@User} registered", new { existingUser?.Email, existingUser?.Username, existingUser?.Id });
                return new RegistrationResponse() { UserId = 1 };
            }
            catch (BadRequestException ex)
            {
                throw new BadRequestException($"User could not be registered: {ex.Message}");
            }
        }

        public IEnumerable<Claim> CreateUserClaims(UserDto user)
        {
            var userRole = user.RoleName;
            var roleClaims = new Claim[]{
                new Claim(ClaimTypes.Role, userRole)
            };
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                //new Claim(CustomClaimTypes.Uid, user.Id.ToString())
            }
            .Union(roleClaims);

            return claims;
        }
    }
}
