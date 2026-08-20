namespace Orders.Api.Models;

public record Order(
    Guid Id,
    decimal Amount,
    string Status,
    DateTime CreatedAt);