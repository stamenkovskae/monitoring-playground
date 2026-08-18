using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using Users.Api.Data;
using Users.Api.Endpoints;
using Users.Api.Services.Implementation;
using Users.Api.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UserDb>(opt => opt.UseInMemoryDatabase("UserDb"));
builder.Services.AddOpenApi();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddPrometheusExporter());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapUserEndpoints();
app.MapPrometheusScrapingEndpoint();

app.Run();