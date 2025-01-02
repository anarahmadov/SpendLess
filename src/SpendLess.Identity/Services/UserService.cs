using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SpendLess.Application.Common;
using SpendLess.Application.Contracts.Identity;
using SpendLess.Application.DTOs.Users;
using SpendLess.Application.Exceptions;
using SpendLess.Application.Models.Identity;
using SpendLess.Identity.Factories;
using SpendLess.Identity.Helpers;
using SpendLess.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SpendLess.Identity.Services
{
    public class UserService : IUserService
    {
        private readonly SpendLessIdentityDbContext _identityDbContext;
        private readonly IPasswordHelper _passwordHelper;
        public UserService(SpendLessIdentityDbContext identityDbContext,
                           IPasswordHelper passwordHelper)
        {
            _identityDbContext = identityDbContext;
            _passwordHelper = passwordHelper;
        }

        public async Task<int> CreateUser(CreateUserDto userDto)
        {
            try
            {
                var userEntity = UserFactory.CreateUserEntity(userDto);
                userEntity.PasswordHash = _passwordHelper.HashPassword(user: userEntity, userDto.Password);

                await _identityDbContext.Users.AddAsync(userEntity);
                await _identityDbContext.SaveChangesAsync();

                return userEntity.Id;
            }
            catch (Exception ex)
            {
                throw new BadRequestException($"User could not be created: {ex.Message}");
            }
        }

        public async Task<UserDto> GetUser(int userId)
        {
            var user = await _identityDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
            var role = await _identityDbContext.Roles.FirstOrDefaultAsync(x => x.Id == user.RoleId);

            return new UserDto()
            {
                Id = user.Id,
                RoleId = user.RoleId,
                UserSettingsId = user.UserSettingsId,
                Email = user.Email,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Age = user.Age,
                Username = user.Username,
                Birthday = user.Birthday,
                Password = user.PasswordHash,
                RoleName = role.Name
            };
        }

        public async Task<UserDto?> GetUserByEmail(string email)
        {
            var userEntity = await _identityDbContext.Users
                                        .Include(x => x.Role)
                                        .FirstOrDefaultAsync(x => x.Email == email);

            return UserFactory.CreateUserDto(userEntity);
        }

        public async Task<(bool, UserDto)> CheckUserByEmailAndPassword(string email, string password)
        {
            try
            {
                var userEntityByEmail = await GetUserByEmail(email);
                if (userEntityByEmail == null)
                    throw new NotFoundException(email);

                var isPasswordCorrect = _passwordHelper
                    .VerifyPassword(new ApplicationUser(), userEntityByEmail.Password, password);

                return (isPasswordCorrect, userEntityByEmail);
            }
            catch (Exception ex)
            {
                throw new BadRequestException($"Can't be logged in: {ex.Message}");
            }
        }

        public async Task<UserDto?> GetUserByUsername(string username)
        {
            var userEntity = await _identityDbContext.Users.FirstOrDefaultAsync(x => x.Username == username);

            return UserFactory.CreateUserDto(userEntity);
        }

        public async Task<List<UserDto>> GetUsers()
        {
            var users = _identityDbContext.Users;

            return await users.Select(q => new UserDto
            {
                Id = q.Id,
                Email = q.Email,
                Firstname = q.Firstname,
                Lastname = q.Lastname
            }).ToListAsync();
        }
    }
}
