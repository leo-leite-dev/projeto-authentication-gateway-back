namespace AuthService.Application.DTOs.Users;

public sealed record UserDto(Guid Id, string Username, string Email);
