namespace Auth.API.Application.DTOs;

public record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    DateTime CreatedAt
);