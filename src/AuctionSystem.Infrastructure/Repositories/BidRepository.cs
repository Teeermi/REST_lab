using AuctionSystem.Core.Entities;
using AuctionSystem.Core.Interfaces;
using AuctionSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuctionSystem.Infrastructure.Repositories;

public class BidRepository : Repository<Bid>, IBidRepository
{
    public BidRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Bid>> GetByAuctionIdAsync(Guid auctionId)
        => await _dbSet.Where(b => b.AuctionId == auctionId)
            .OrderByDescending(b => b.Amount)
            .Include(b => b.Bidder)
            .ToListAsync();

    public async Task<Bid?> GetHighestBidAsync(Guid auctionId)
        => await _dbSet.Where(b => b.AuctionId == auctionId)
            .OrderByDescending(b => b.Amount)
            .FirstOrDefaultAsync();
}
