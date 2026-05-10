using AuctionSystem.Application.DTOs;
using AuctionSystem.Application.Interfaces;
using AuctionSystem.Core.Entities;
using AuctionSystem.Core.Enums;
using AuctionSystem.Core.Interfaces;

namespace AuctionSystem.Application.Services;

public class BidService : IBidService
{
    private readonly IBidRepository _bidRepository;
    private readonly IAuctionRepository _auctionRepository;

    public BidService(IBidRepository bidRepository, IAuctionRepository auctionRepository)
    {
        _bidRepository = bidRepository;
        _auctionRepository = auctionRepository;
    }

    public async Task<BidDto> CreateAsync(Guid auctionId, CreateBidDto dto, Guid bidderId)
    {
        var auction = await _auctionRepository.GetByIdAsync(auctionId);
        if (auction == null)
            throw new KeyNotFoundException("Auction not found");

        if (auction.Status != AuctionStatus.Active)
            throw new InvalidOperationException("Auction is not active");

        if (auction.EndDate < DateTime.UtcNow)
            throw new InvalidOperationException("Auction has ended");

        if (dto.Amount <= auction.CurrentPrice)
            throw new InvalidOperationException("Bid must be higher than current price");

        if (auction.OwnerId == bidderId)
            throw new InvalidOperationException("Cannot bid on own auction");

        var bid = new Bid
        {
            Id = Guid.NewGuid(),
            Amount = dto.Amount,
            CreatedAt = DateTime.UtcNow,
            AuctionId = auctionId,
            BidderId = bidderId
        };

        auction.CurrentPrice = dto.Amount;

        await _bidRepository.AddAsync(bid);
        _auctionRepository.Update(auction);
        await _bidRepository.SaveChangesAsync();

        return new BidDto(bid.Id, bid.Amount, bid.CreatedAt, bid.BidderId, "");
    }

    public async Task<IEnumerable<BidDto>> GetByAuctionIdAsync(Guid auctionId)
    {
        var bids = await _bidRepository.GetByAuctionIdAsync(auctionId);
        return bids.Select(b => new BidDto(b.Id, b.Amount, b.CreatedAt, b.BidderId, b.Bidder?.Username ?? ""));
    }
}
