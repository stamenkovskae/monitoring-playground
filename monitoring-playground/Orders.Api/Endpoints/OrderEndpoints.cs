using Orders.Api.Models;
using Orders.Api.Services.Interface;

namespace Orders.Api.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/orders");
        group.MapGet("", GetAllOrders).WithName(nameof(GetAllOrders));
        group.MapGet("/{orderId:guid}", GetOrderById).WithName(nameof(GetOrderById));
        group.MapPost("", CreateOrder).WithName(nameof(CreateOrder));
        group.MapDelete("/{orderId:guid}", DeleteOrder).WithName(nameof(DeleteOrder));
    }

    private static async Task<IResult> GetAllOrders(IOrderService orderService)
    {
        var orders = await orderService.GetAllOrders();
        return Results.Ok(orders);
    }

    private static async Task<IResult> GetOrderById(Guid orderId, IOrderService orderService)
    {
        var order = await orderService.GetOrderById(orderId);
        return Results.Ok(order);
    }

    private static async Task<IResult> CreateOrder(Order order, IOrderService orderService)
    {
        await orderService.CreateOrder(order);
        return Results.Ok();
    }

    private static async Task<IResult> DeleteOrder(Guid orderId, IOrderService orderService)
    {
        await orderService.DeleteOrder(orderId);
        return Results.Ok();
    }


    
}