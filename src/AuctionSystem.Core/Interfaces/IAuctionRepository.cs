using AuctionSystem.Core.Entities;
using AuctionSystem.Core.Enums;

namespace AuctionSystem.Core.Interfaces;

public interface IAuctionRepository : IRepository<Auction>
{
    Task<Auction?> GetWithBidsAsync(Guid id);
    Task<IEnumerable<Auction>> GetByStatusAsync(AuctionStatus status);
    Task<IEnumerable<Auction>> GetByCategoryAsync(Category category);
    Task<(IEnumerable<Auction> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, Category? category = null, AuctionStatus? status = null, string? sortBy = null);
}
