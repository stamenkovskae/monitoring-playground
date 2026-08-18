using Microsoft.EntityFrameworkCore;
using Orders.Api.Data;
using Orders.Api.Models;
using Orders.Api.Services.Interface;

namespace Orders.Api.Services.Implementation;

public class OrderService(OrderDb orderDb) : IOrderService
{
    public async Task<List<Order>> GetAllOrders()
    {
        return await orderDb.Orders.ToListAsync();
    }

    public async Task<Order?> GetOrderById(Guid orderId)
    {
        return await orderDb.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<bool> CreateOrder(Order order)
    {
        try
        {
            await orderDb.Orders.AddAsync(order);
            await orderDb.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public async Task<bool> DeleteOrder(Guid orderId)
    {
        var orderToBeDeleted = await GetOrderById(orderId);

        if (orderToBeDeleted != null)
        {
            orderDb.Orders.Remove(orderToBeDeleted);
            await orderDb.SaveChangesAsync();
            return true;
        }

        return false;
    }
}