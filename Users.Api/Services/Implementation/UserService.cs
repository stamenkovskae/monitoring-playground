using Microsoft.EntityFrameworkCore;
using Users.Api.Data;
using Users.Api.Models;
using Users.Api.Services.Interface;

namespace Users.Api.Services.Implementation;

public class UserService(UserDb userDb) : IUserService
{
    public async Task<List<User>> GetAllUsers()
    {
        return await userDb.Users.ToListAsync();
    }

    public async Task<User?> GetUserById(Guid userId)
    {
        return await userDb.Users.FirstOrDefaultAsync(o => o.Id == userId);
    }

    public async Task<bool> CreateUser(User user)
    {
        try
        {
            await userDb.Users.AddAsync(user);
            await userDb.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public async Task<bool> DeleteUser(Guid userId)
    {
        var userToBeDeleted = await GetUserById(userId);

        if (userToBeDeleted != null)
        {
            userDb.Users.Remove(userToBeDeleted);
            await userDb.SaveChangesAsync();
            return true;
        }

        return false;
    }
}