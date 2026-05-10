using AuctionSystem.Core.Entities;
using AuctionSystem.Core.Enums;
using AuctionSystem.Core.Interfaces;
using AuctionSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuctionSystem.Infrastructure.Repositories;

public class AuctionRepository : Repository<Auction>, IAuctionRepository
{
    public AuctionRepository(AppDbContext context) : base(context) { }

    public async Task<Auction?> GetWithBidsAsync(Guid id)
        => await _dbSet.Include(a => a.Bids).Include(a => a.Owner).FirstOrDefaultAsync(a => a.Id == id);

    public async Task<IEnumerable<Auction>> GetByStatusAsync(AuctionStatus status)
        => await _dbSet.Where(a => a.Status == status).ToListAsync();

    public async Task<IEnumerable<Auction>> GetByCategoryAsync(Category category)
        => await _dbSet.Where(a => a.Category == category).ToListAsync();

    public async Task<(IEnumerable<Auction> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, Category? category = null, AuctionStatus? status = null, string? sortBy = null)
    {
        var query = _dbSet.Include(a => a.Owner).AsQueryable();

        if (category.HasValue)
            query = query.Where(a => a.Category == category.Value);

        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);

        query = sortBy?.ToLower() switch
        {
            "price" => query.OrderBy(a => a.CurrentPrice),
            "price_desc" => query.OrderByDescending(a => a.CurrentPrice),
            "date" => query.OrderBy(a => a.EndDate),
            "date_desc" => query.OrderByDescending(a => a.EndDate),
            _ => query.OrderByDescending(a => a.StartDate)
        };

        var totalCount = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return (items, totalCount);
    }
}
