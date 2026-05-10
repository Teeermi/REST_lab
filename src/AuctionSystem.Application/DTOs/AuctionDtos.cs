using AuctionSystem.Core.Enums;

namespace AuctionSystem.Application.DTOs;

public record CreateAuctionDto(
    string Title,
    string Description,
    Category Category,
    decimal StartingPrice,
    DateTime StartDate,
    DateTime EndDate
);

public record UpdateAuctionDto(
    string? Title,
    string? Description,
    Category? Category,
    DateTime? EndDate
);

public record AuctionDto(
    Guid Id,
    string Title,
    string Description,
    Category Category,
    decimal StartingPrice,
    decimal CurrentPrice,
    DateTime StartDate,
    DateTime EndDate,
    AuctionStatus Status,
    Guid OwnerId,
    string OwnerUsername
);

public record AuctionDetailDto(
    Guid Id,
    string Title,
    string Description,
    Category Category,
    decimal StartingPrice,
    decimal CurrentPrice,
    DateTime StartDate,
    DateTime EndDate,
    AuctionStatus Status,
    Guid OwnerId,
    string OwnerUsername,
    IEnumerable<BidDto> Bids
);

public record PagedResultDto<T>(IEnumerable<T> Items, int TotalCount, int Page, int PageSize);
