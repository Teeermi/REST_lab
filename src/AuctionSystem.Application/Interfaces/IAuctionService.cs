using AuctionSystem.Application.DTOs;
using AuctionSystem.Core.Enums;

namespace AuctionSystem.Application.Interfaces;

public interface IAuctionService
{
    Task<AuctionDto> CreateAsync(CreateAuctionDto dto, Guid ownerId);
    Task<AuctionDetailDto?> GetByIdAsync(Guid id);
    Task<PagedResultDto<AuctionDto>> GetAllAsync(int page, int pageSize, Category? category, AuctionStatus? status, string? sortBy);
    Task<AuctionDto?> UpdateAsync(Guid id, UpdateAuctionDto dto, Guid currentUserId);
    Task<bool> DeleteAsync(Guid id, Guid currentUserId);
}
