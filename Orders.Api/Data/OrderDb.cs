using Microsoft.EntityFrameworkCore;
using Orders.Api.Models;

namespace Orders.Api.Data;

public class OrderDb(DbContextOptions<OrderDb> options) : DbContext(options)
{
    public DbSet<Order> Orders { get; set; }
}