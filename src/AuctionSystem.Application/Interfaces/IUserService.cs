using AuctionSystem.Application.DTOs;

namespace AuctionSystem.Application.Interfaces;

public interface IUserService
{
    Task<AuthResponseDto> RegisterAsync(RegisterUserDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<PagedResultDto<UserDto>> GetAllAsync(int page, int pageSize);
    Task<UserDto?> UpdateAsync(Guid id, UpdateUserDto dto, Guid currentUserId);
    Task<bool> DeleteAsync(Guid id, Guid currentUserId);
}
