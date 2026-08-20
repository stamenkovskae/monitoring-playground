using Users.Api.Models;

namespace Users.Api.Services.Interface;

public interface IUserService
{
    public Task<List<User>> GetAllUsers();
    public Task<User?> GetUserById(Guid userId);
    public Task<bool> CreateUser(User user);
    public Task<bool> DeleteUser(Guid userId);
}