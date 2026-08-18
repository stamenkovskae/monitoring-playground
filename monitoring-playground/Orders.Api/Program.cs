using Microsoft.EntityFrameworkCore;
using Orders.Api.Data;
using Orders.Api.Endpoints;
using Orders.Api.Services.Implementation;
using Orders.Api.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrderDb>(opt => opt.UseInMemoryDatabase("OrderDb"));
builder.Services.AddOpenApi();

builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapOrderEndpoints();
app.Run();