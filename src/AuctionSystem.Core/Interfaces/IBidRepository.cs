using AuctionSystem.Core.Entities;

namespace AuctionSystem.Core.Interfaces;

public interface IBidRepository : IRepository<Bid>
{
    Task<IEnumerable<Bid>> GetByAuctionIdAsync(Guid auctionId);
    Task<Bid?> GetHighestBidAsync(Guid auctionId);
}
