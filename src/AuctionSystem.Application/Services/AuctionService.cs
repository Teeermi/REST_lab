using AuctionSystem.Application.DTOs;
using AuctionSystem.Application.Interfaces;
using AuctionSystem.Core.Entities;
using AuctionSystem.Core.Enums;
using AuctionSystem.Core.Interfaces;

namespace AuctionSystem.Application.Services;

public class AuctionService : IAuctionService
{
    private readonly IAuctionRepository _auctionRepository;

    public AuctionService(IAuctionRepository auctionRepository)
    {
        _auctionRepository = auctionRepository;
    }

    public async Task<AuctionDto> CreateAsync(CreateAuctionDto dto, Guid ownerId)
    {
        var auction = new Auction
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            Category = dto.Category,
            StartingPrice = dto.StartingPrice,
            CurrentPrice = dto.StartingPrice,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = dto.StartDate <= DateTime.UtcNow ? AuctionStatus.Active : AuctionStatus.Draft,
            OwnerId = ownerId
        };

        await _auctionRepository.AddAsync(auction);
        await _auctionRepository.SaveChangesAsync();

        var created = await _auctionRepository.GetWithBidsAsync(auction.Id);
        return ToDto(created!);
    }

    public async Task<AuctionDetailDto?> GetByIdAsync(Guid id)
    {
        var auction = await _auctionRepository.GetWithBidsAsync(id);
        if (auction == null) return null;

        return new AuctionDetailDto(
            auction.Id, auction.Title, auction.Description, auction.Category,
            auction.StartingPrice, auction.CurrentPrice, auction.StartDate,
            auction.EndDate, auction.Status, auction.OwnerId, auction.Owner.Username,
            auction.Bids.OrderByDescending(b => b.Amount)
                .Select(b => new BidDto(b.Id, b.Amount, b.CreatedAt, b.BidderId, b.Bidder?.Username ?? ""))
        );
    }

    public async Task<PagedResultDto<AuctionDto>> GetAllAsync(
        int page, int pageSize, Category? category, AuctionStatus? status, string? sortBy)
    {
        var (items, totalCount) = await _auctionRepository.GetPagedAsync(page, pageSize, category, status, sortBy);
        return new PagedResultDto<AuctionDto>(items.Select(ToDto), totalCount, page, pageSize);
    }

    public async Task<AuctionDto?> UpdateAsync(Guid id, UpdateAuctionDto dto, Guid currentUserId)
    {
        var auction = await _auctionRepository.GetWithBidsAsync(id);
        if (auction == null) return null;

        if (auction.OwnerId != currentUserId)
            throw new UnauthorizedAccessException("Not the owner of this auction");

        if (!string.IsNullOrEmpty(dto.Title))
            auction.Title = dto.Title;
        if (!string.IsNullOrEmpty(dto.Description))
            auction.Description = dto.Description;
        if (dto.Category.HasValue)
            auction.Category = dto.Category.Value;
        if (dto.EndDate.HasValue)
            auction.EndDate = dto.EndDate.Value;

        _auctionRepository.Update(auction);
        await _auctionRepository.SaveChangesAsync();

        return ToDto(auction);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid currentUserId)
    {
        var auction = await _auctionRepository.GetByIdAsync(id);
        if (auction == null) return false;

        if (auction.OwnerId != currentUserId)
            throw new UnauthorizedAccessException("Not the owner of this auction");

        _auctionRepository.Remove(auction);
        await _auctionRepository.SaveChangesAsync();
        return true;
    }

    private static AuctionDto ToDto(Auction a) =>
        new(a.Id, a.Title, a.Description, a.Category, a.StartingPrice,
            a.CurrentPrice, a.StartDate, a.EndDate, a.Status, a.OwnerId, a.Owner?.Username ?? "");
}
