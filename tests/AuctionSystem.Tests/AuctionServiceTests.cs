using AuctionSystem.Application.DTOs;
using AuctionSystem.Application.Services;
using AuctionSystem.Core.Entities;
using AuctionSystem.Core.Enums;
using AuctionSystem.Core.Interfaces;
using Moq;

namespace AuctionSystem.Tests;

public class AuctionServiceTests
{
    private readonly Mock<IAuctionRepository> _repoMock;
    private readonly AuctionService _service;

    public AuctionServiceTests()
    {
        _repoMock = new Mock<IAuctionRepository>();
        _service = new AuctionService(_repoMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        _repoMock.Setup(r => r.GetWithBidsAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Auction?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenFound_ReturnsAuctionDetail()
    {
        var auction = new Auction
        {
            Id = Guid.NewGuid(),
            Title = "Test Auction",
            Description = "Test Description",
            Category = Category.Electronics,
            StartingPrice = 100,
            CurrentPrice = 150,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(7),
            Status = AuctionStatus.Active,
            OwnerId = Guid.NewGuid(),
            Owner = new User { Username = "testuser" },
            Bids = new List<Bid>()
        };

        _repoMock.Setup(r => r.GetWithBidsAsync(auction.Id))
            .ReturnsAsync(auction);

        var result = await _service.GetByIdAsync(auction.Id);

        Assert.NotNull(result);
        Assert.Equal("Test Auction", result.Title);
        Assert.Equal(150, result.CurrentPrice);
    }

    [Fact]
    public async Task CreateAsync_SetsCorrectInitialValues()
    {
        var ownerId = Guid.NewGuid();
        var dto = new CreateAuctionDto(
            "New Auction",
            "Description",
            Category.Fashion,
            100,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(7)
        );

        _repoMock.Setup(r => r.GetWithBidsAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => new Auction
            {
                Id = id,
                Title = dto.Title,
                Description = dto.Description,
                Category = dto.Category,
                StartingPrice = dto.StartingPrice,
                CurrentPrice = dto.StartingPrice,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = AuctionStatus.Active,
                OwnerId = ownerId,
                Owner = new User { Username = "owner" },
                Bids = new List<Bid>()
            });

        var result = await _service.CreateAsync(dto, ownerId);

        Assert.Equal("New Auction", result.Title);
        Assert.Equal(100, result.CurrentPrice);
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Auction>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenNotOwner_ThrowsUnauthorized()
    {
        var ownerId = Guid.NewGuid();
        var otherId = Guid.NewGuid();

        var auction = new Auction
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId
        };

        _repoMock.Setup(r => r.GetByIdAsync(auction.Id))
            .ReturnsAsync(auction);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.DeleteAsync(auction.Id, otherId)
        );
    }
}
