namespace AuctionSystem.Core.Entities;

public class Bid
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }

    public Guid AuctionId { get; set; }
    public Auction Auction { get; set; } = null!;

    public Guid BidderId { get; set; }
    public User Bidder { get; set; } = null!;
}
