using Microsoft.EntityFrameworkCore;
using Users.Api.Models;

namespace Users.Api.Data;

public class UserDb(DbContextOptions<UserDb> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
}