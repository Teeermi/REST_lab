using AuctionSystem.Application.DTOs;
using AuctionSystem.Application.Interfaces;
using AuctionSystem.Core.Entities;
using AuctionSystem.Core.Interfaces;

namespace AuctionSystem.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public UserService(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterUserDto dto)
    {
        if (await _userRepository.EmailExistsAsync(dto.Email))
            throw new InvalidOperationException("Email already exists");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user);
        return new AuthResponseDto(token, ToDto(user));
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        var token = _jwtService.GenerateToken(user);
        return new AuthResponseDto(token, ToDto(user));
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user != null ? ToDto(user) : null;
    }

    public async Task<UserDto?> UpdateAsync(Guid id, UpdateUserDto dto, Guid currentUserId)
    {
        if (id != currentUserId)
            throw new UnauthorizedAccessException("Cannot update other user's profile");

        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return null;

        if (!string.IsNullOrEmpty(dto.Username))
            user.Username = dto.Username;

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        return ToDto(user);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid currentUserId)
    {
        if (id != currentUserId)
            throw new UnauthorizedAccessException("Cannot delete other user's account");

        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return false;

        _userRepository.Remove(user);
        await _userRepository.SaveChangesAsync();
        return true;
    }

    private static UserDto ToDto(User user) =>
        new(user.Id, user.Email, user.Username, user.CreatedAt);
}
