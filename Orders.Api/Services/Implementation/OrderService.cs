using System.Diagnostics.Metrics;
using Microsoft.EntityFrameworkCore;
using Orders.Api.Data;
using Orders.Api.Models;
using Orders.Api.Services.Interface;

namespace Orders.Api.Services.Implementation;

public static class OrderMetrics
{
    public const string MeterName = "Orders.Api";
    private static readonly Meter Meter = new(MeterName);
    public static readonly Counter<long> Created = Meter.CreateCounter<long>("orders_created_total");
    public static readonly Counter<long> Deleted = Meter.CreateCounter<long>("orders_deleted_total");
}

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
            OrderMetrics.Created.Add(1);
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
            OrderMetrics.Deleted.Add(1);
            return true;
        }

        return false;
    }
}