using AuctionSystem.Application.DTOs;

namespace AuctionSystem.Application.Interfaces;

public interface IBidService
{
    Task<BidDto> CreateAsync(Guid auctionId, CreateBidDto dto, Guid bidderId);
    Task<IEnumerable<BidDto>> GetByAuctionIdAsync(Guid auctionId);
}
