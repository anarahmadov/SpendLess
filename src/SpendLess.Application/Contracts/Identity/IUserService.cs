using SpendLess.Application.DTOs.Users;

namespace SpendLess.Application.Contracts.Identity
{
    public interface IUserService
    {
        Task<int> CreateUser(CreateUserDto userDto);
        Task<(bool, UserDto)> CheckUserByEmailAndPassword(string email, string password);

        Task<List<UserDto>> GetUsers();
        Task<UserDto?> GetUserByEmail(string email);
        Task<UserDto?> GetUserByUsername(string username);
        Task<UserDto> GetUser(int userId);
    }
}
