using AuctionSystem.Application.DTOs;
using AuctionSystem.Application.Services;
using AuctionSystem.Core.Entities;
using AuctionSystem.Core.Enums;
using AuctionSystem.Core.Interfaces;
using Moq;

namespace AuctionSystem.Tests;

public class BidServiceTests
{
    private readonly Mock<IBidRepository> _bidRepoMock;
    private readonly Mock<IAuctionRepository> _auctionRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly BidService _service;

    public BidServiceTests()
    {
        _bidRepoMock = new Mock<IBidRepository>();
        _auctionRepoMock = new Mock<IAuctionRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _service = new BidService(_bidRepoMock.Object, _auctionRepoMock.Object, _userRepoMock.Object);
    }

    [Fact]
    public async Task CreateBid_WhenAuctionNotFound_ThrowsKeyNotFoundException()
    {
        _auctionRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Auction?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.CreateAsync(Guid.NewGuid(), new CreateBidDto(100), Guid.NewGuid())
        );
    }

    [Fact]
    public async Task CreateBid_WhenAuctionNotActive_ThrowsInvalidOperationException()
    {
        var auction = new Auction
        {
            Id = Guid.NewGuid(),
            Status = AuctionStatus.Ended,
            CurrentPrice = 50
        };

        _auctionRepoMock.Setup(r => r.GetByIdAsync(auction.Id))
            .ReturnsAsync(auction);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CreateAsync(auction.Id, new CreateBidDto(100), Guid.NewGuid())
        );
    }

    [Fact]
    public async Task CreateBid_WhenBidTooLow_ThrowsInvalidOperationException()
    {
        var auction = new Auction
        {
            Id = Guid.NewGuid(),
            Status = AuctionStatus.Active,
            CurrentPrice = 100,
            EndDate = DateTime.UtcNow.AddDays(1),
            OwnerId = Guid.NewGuid()
        };

        _auctionRepoMock.Setup(r => r.GetByIdAsync(auction.Id))
            .ReturnsAsync(auction);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CreateAsync(auction.Id, new CreateBidDto(50), Guid.NewGuid())
        );

        Assert.Contains("higher", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateBid_WhenOwnerBids_ThrowsInvalidOperationException()
    {
        var ownerId = Guid.NewGuid();
        var auction = new Auction
        {
            Id = Guid.NewGuid(),
            Status = AuctionStatus.Active,
            CurrentPrice = 100,
            EndDate = DateTime.UtcNow.AddDays(1),
            OwnerId = ownerId
        };

        _auctionRepoMock.Setup(r => r.GetByIdAsync(auction.Id))
            .ReturnsAsync(auction);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CreateAsync(auction.Id, new CreateBidDto(150), ownerId)
        );

        Assert.Contains("own", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateBid_WhenValid_ReturnsCreatedBid()
    {
        var auction = new Auction
        {
            Id = Guid.NewGuid(),
            Status = AuctionStatus.Active,
            CurrentPrice = 100,
            EndDate = DateTime.UtcNow.AddDays(1),
            OwnerId = Guid.NewGuid()
        };

        var bidderId = Guid.NewGuid();

        _auctionRepoMock.Setup(r => r.GetByIdAsync(auction.Id))
            .ReturnsAsync(auction);
        _userRepoMock.Setup(r => r.GetByIdAsync(bidderId))
            .ReturnsAsync(new User { Id = bidderId, Username = "bidder1" });

        var result = await _service.CreateAsync(auction.Id, new CreateBidDto(150), bidderId);

        Assert.Equal(150, result.Amount);
        Assert.Equal(bidderId, result.BidderId);
        Assert.Equal("bidder1", result.BidderUsername);
        _bidRepoMock.Verify(r => r.AddAsync(It.IsAny<Bid>()), Times.Once);
        _bidRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
