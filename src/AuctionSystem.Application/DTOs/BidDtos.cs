namespace AuctionSystem.Application.DTOs;

public record CreateBidDto(decimal Amount);

public record BidDto(Guid Id, decimal Amount, DateTime CreatedAt, Guid BidderId, string BidderUsername);
