using AuctionSystem.Core.Entities;

namespace AuctionSystem.Core.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<bool> EmailExistsAsync(string email);
    Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(int page, int pageSize);
}
