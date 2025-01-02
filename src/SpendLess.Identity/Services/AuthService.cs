using SpendLess.Application.Contracts.Identity;
using SpendLess.Application.Exceptions;
using SpendLess.Application.Models.Identity;
using SpendLess.Identity.Factories;
using SpendLess.Identity.Models;
using System.IdentityModel.Tokens.Jwt;

namespace SpendLess.Identity.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        public AuthService(IUserService userService,
                           ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        public async Task<AuthResponse> Login(AuthRequest request)
        {
            var (result, user) = await _userService.CheckUserByEmailAndPassword(request.Email, request.Password);
            if (!result)
                throw new BadRequestException($"Credentials for '{request.Email}' aren't valid.");
            
            var accessToken = _tokenService.GenerateAccessToken(user) as JwtToken;
            var refreshToken = _tokenService.GenerateRefreshToken();
            await _tokenService.SaveToken(user.Id, refreshToken);

            AuthResponse response = new AuthResponse
            {
                Id = user.Id,
                AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
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
            {
                throw new BadRequestException($"Username '{request.UserName}' already exists.");
            }

            var existingEmail = await _userService.GetUserByEmail(request.Email);
            if (existingEmail == null)
            {
                try
                {
                    int userId = await _userService.CreateUser(UserFactory.CreateUserDto(request));
                    return new RegistrationResponse() { UserId = 1 };
                }
                catch (BadRequestException ex)
                {
                    throw new BadRequestException($"User could not be registered: {ex.Message}");
                }
            }
            else
            {
                throw new BadRequestException($"Email {request.Email} already exists.");
            }
        }
    }
}
