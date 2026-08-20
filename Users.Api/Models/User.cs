namespace Users.Api.Models;

public record User(
    Guid Id,
    string Name,
    string Email,
    DateTime CreatedAt);