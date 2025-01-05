using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpendLess.Application.Contracts.Identity;
using SpendLess.Application.Models.Identity;
using System.Security.Claims;

namespace SpendLess.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokensController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly ILogger<TokensController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        public TokensController(ITokenService tokenService,
                                IUserService userService,
                                IAuthService authService,
                                ILogger<TokensController> logger,
                                IHttpContextAccessor contextAccessor)
        {
            _tokenService = tokenService;
            _userService = userService;
            _authService = authService;
            _logger = logger;
            _contextAccessor = contextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
        {
            var applicationToken = await _tokenService.GetToken(request.Token);

            if (applicationToken is null || applicationToken.ExpirationDate < DateTime.UtcNow)
                throw new SpendLess.Application.Exceptions.UnauthorizedAccessException($"User session has expired");

            // revoke old one
            await _tokenService.RevokeToken(request.Token);

            // generate new tokens
            var user = await _userService.GetUser(applicationToken.UserId);
            _logger.LogDebug("User {@User} revoked refresh token at {@UtcNow}",
                                    new { user.Id, user.Email, user.Username }, DateTime.UtcNow);

            var claims = _authService.CreateUserClaims(user);
            var newAccessToken = _tokenService.GenerateAccessToken(claims.ToList());
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            _logger.LogDebug("User {@User} generated new tokens at {@UtcNow}",
                                    new { user.Id, user.Email, user.Username }, DateTime.UtcNow);

            // save new refresh token
            await _tokenService.SaveToken(user.Id, newRefreshToken);
            _logger.LogDebug("New refresh token saved successfully at {@UtcNow}", DateTime.UtcNow);

            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
    }
}
