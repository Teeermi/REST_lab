using AuctionSystem.Application.DTOs;
using AuctionSystem.Application.Interfaces;
using AuctionSystem.Application.Services;
using AuctionSystem.Core.Entities;
using AuctionSystem.Core.Interfaces;
using Moq;

namespace AuctionSystem.Tests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _repoMock;
    private readonly Mock<IJwtService> _jwtMock;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _repoMock = new Mock<IUserRepository>();
        _jwtMock = new Mock<IJwtService>();
        _jwtMock.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("fake-token");
        _service = new UserService(_repoMock.Object, _jwtMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailExists_ThrowsInvalidOperationException()
    {
        _repoMock.Setup(r => r.EmailExistsAsync("taken@test.com")).ReturnsAsync(true);

        var dto = new RegisterUserDto("taken@test.com", "user", "password");

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RegisterAsync(dto));
    }

    [Fact]
    public async Task RegisterAsync_WhenValid_ReturnsAuthResponseWithToken()
    {
        _repoMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

        var dto = new RegisterUserDto("new@test.com", "newuser", "password123");
        var result = await _service.RegisterAsync(dto);

        Assert.Equal("fake-token", result.Token);
        Assert.Equal("new@test.com", result.User.Email);
        Assert.Equal("newuser", result.User.Username);
        _repoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_HashesPasswordBeforeSaving()
    {
        _repoMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

        User? savedUser = null;
        _repoMock.Setup(r => r.AddAsync(It.IsAny<User>()))
            .Callback<User>(u => savedUser = u);

        var dto = new RegisterUserDto("a@b.com", "u", "plaintext");
        await _service.RegisterAsync(dto);

        Assert.NotNull(savedUser);
        Assert.NotEqual("plaintext", savedUser!.PasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify("plaintext", savedUser.PasswordHash));
    }

    [Fact]
    public async Task LoginAsync_WhenUserNotFound_ThrowsUnauthorized()
    {
        _repoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.LoginAsync(new LoginDto("none@test.com", "pass")));
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordWrong_ThrowsUnauthorized()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "a@b.com",
            Username = "u",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpass")
        };
        _repoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.LoginAsync(new LoginDto(user.Email, "wrongpass")));
    }

    [Fact]
    public async Task LoginAsync_WhenCredentialsValid_ReturnsToken()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "a@b.com",
            Username = "u",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("secret123")
        };
        _repoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        var result = await _service.LoginAsync(new LoginDto(user.Email, "secret123"));

        Assert.Equal("fake-token", result.Token);
        Assert.Equal(user.Id, result.User.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenFound_ReturnsUserDto()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "a@b.com",
            Username = "u",
            CreatedAt = DateTime.UtcNow
        };
        _repoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);

        var result = await _service.GetByIdAsync(user.Id);

        Assert.NotNull(result);
        Assert.Equal(user.Email, result!.Email);
        Assert.Equal(user.Username, result.Username);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPagedResult()
    {
        var users = new List<User>
        {
            new() { Id = Guid.NewGuid(), Email = "a@b.com", Username = "u1", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Email = "c@d.com", Username = "u2", CreatedAt = DateTime.UtcNow }
        };
        _repoMock.Setup(r => r.GetPagedAsync(1, 10)).ReturnsAsync((users, 2));

        var result = await _service.GetAllAsync(1, 10);

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
    }

    [Fact]
    public async Task UpdateAsync_WhenDifferentUser_ThrowsUnauthorized()
    {
        var id = Guid.NewGuid();
        var otherId = Guid.NewGuid();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.UpdateAsync(id, new UpdateUserDto("newname"), otherId));
    }

    [Fact]
    public async Task UpdateAsync_WhenSameUser_UpdatesUsername()
    {
        var id = Guid.NewGuid();
        var user = new User { Id = id, Username = "old", Email = "a@b.com" };
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(user);

        var result = await _service.UpdateAsync(id, new UpdateUserDto("newname"), id);

        Assert.NotNull(result);
        Assert.Equal("newname", result!.Username);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenDifferentUser_ThrowsUnauthorized()
    {
        var id = Guid.NewGuid();
        var otherId = Guid.NewGuid();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.DeleteAsync(id, otherId));
    }

    [Fact]
    public async Task DeleteAsync_WhenSameUser_RemovesUser()
    {
        var id = Guid.NewGuid();
        var user = new User { Id = id };
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(user);

        var deleted = await _service.DeleteAsync(id, id);

        Assert.True(deleted);
        _repoMock.Verify(r => r.Remove(user), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
