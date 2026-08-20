using Orders.Api.Models;

namespace Orders.Api.Services.Interface;

public interface IOrderService
{
    public Task<List<Order>> GetAllOrders();
    public Task<Order?> GetOrderById(Guid orderId);
    public Task<bool> CreateOrder(Order order);
    public Task<bool> DeleteOrder(Guid orderId);
}