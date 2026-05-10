using AuctionSystem.Core.Entities;

namespace AuctionSystem.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}
