using SpendLess.Application.DTOs.Users;
using SpendLess.Application.Models.Identity;
using SpendLess.Identity.Helpers;
using SpendLess.Identity.Models;

namespace SpendLess.Identity.Factories
{
    public class UserFactory
    {
        public static ApplicationUser? CreateUserEntity(CreateUserDto? userDto)
        {
            if (userDto is null)
                return null;

            return new ApplicationUser
            {
                Firstname = userDto.Firstname,
                Lastname = userDto.Lastname,
                Username = userDto.Username,
                Age = 18,
                Birthday = DateTime.UtcNow.AddYears(-18),
                Email = userDto.Email,
                RoleId = 2
            };
        }

        public static CreateUserDto? CreateUserDto(RegistrationRequest? request)
        {
            if (request == null)
                return null;

            return new CreateUserDto
            {
                Firstname = request.FirstName,
                Lastname = request.LastName,
                Username = request.UserName,
                Email = request.Email,
                Password = request.Password
            };
        }

        public static UserDto? CreateUserDto(ApplicationUser? user)
        {
            if (user is null)
                return null;

            return new UserDto()
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Username = user.Username,
                Email = user.Email,
                Password = user.PasswordHash,
                Age = user.Age,
                Birthday = user.Birthday,
                RoleName = user.Role.Name
            };
        }
    }
}
