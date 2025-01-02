using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SpendLess.Application.Contracts.Identity;
using SpendLess.Application.Models.Identity;
using SpendLess.Identity.Helpers;
using SpendLess.Identity.Models;
using SpendLess.Identity.Services;

namespace SpendLess.Identity
{
    public static class IdentityServicesRegistration
    {
        public static IServiceCollection ConfigureIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            services.AddDbContext<SpendLessIdentityDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("SpendLessIdentityConnectionString"),
                b => b.MigrationsAssembly(typeof(SpendLessIdentityDbContext).Assembly.FullName)));

            services.AddTransient<IPasswordHasher<ApplicationUser>, PasswordHasher<ApplicationUser>>();
            services.AddTransient<IPasswordHelper, PasswordHelper>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ITokenService, TokenService>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
              {
                  o.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuerSigningKey = true,
                      ValidateIssuer = true,
                      ValidateAudience = true,
                      ValidateLifetime = true,
                      ClockSkew = TimeSpan.Zero,
                      ValidIssuer = configuration["JwtSettings:Issuer"],
                      ValidAudience = configuration["JwtSettings:Audience"],
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]))
                  };
              });

            return services;
        }
    }
}