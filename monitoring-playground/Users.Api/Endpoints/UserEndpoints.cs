using Users.Api.Models;
using Users.Api.Services.Interface;

namespace Users.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/users");
        group.MapGet("", GetAllUsers).WithName(nameof(GetAllUsers));
        group.MapGet("/{userId:guid}", GetUserById).WithName(nameof(GetUserById));
        group.MapPost("", CreateUser).WithName(nameof(CreateUser));
        group.MapDelete("/{userId:guid}", DeleteUser).WithName(nameof(DeleteUser));
    }

    private static async Task<IResult> GetAllUsers(IUserService userService)
    {
        var users = await userService.GetAllUsers();
        return Results.Ok(users);
    }

    private static async Task<IResult> GetUserById(Guid userId, IUserService userService)
    {
        var user = await userService.GetUserById(userId);
        return Results.Ok(user);
    }

    private static async Task<IResult> CreateUser(User user, IUserService userService)
    {
        await userService.CreateUser(user);
        return Results.Ok();
    }

    private static async Task<IResult> DeleteUser(Guid userId, IUserService userService)
    {
        await userService.DeleteUser(userId);
        return Results.Ok();
    }
}