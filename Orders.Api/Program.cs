using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using Orders.Api.Data;
using Orders.Api.Endpoints;
using Orders.Api.Services.Implementation;
using Orders.Api.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrderDb>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Database")));

builder.Services.AddOpenApi();

builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation()
        .AddProcessInstrumentation()
        .AddMeter(OrderMetrics.MeterName)
        .AddPrometheusExporter());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapOrderEndpoints();
app.MapPrometheusScrapingEndpoint();

app.Run();