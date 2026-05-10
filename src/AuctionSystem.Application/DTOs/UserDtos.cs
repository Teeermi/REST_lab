namespace AuctionSystem.Application.DTOs;

public record RegisterUserDto(string Email, string Username, string Password);

public record LoginDto(string Email, string Password);

public record UpdateUserDto(string? Username);

public record UserDto(Guid Id, string Email, string Username, DateTime CreatedAt);

public record AuthResponseDto(string Token, UserDto User);
