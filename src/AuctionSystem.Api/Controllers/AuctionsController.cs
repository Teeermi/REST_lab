using System.Security.Claims;
using AuctionSystem.Application.DTOs;
using AuctionSystem.Application.Interfaces;
using AuctionSystem.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuctionsController : ControllerBase
{
    private readonly IAuctionService _auctionService;
    private readonly IBidService _bidService;

    public AuctionsController(IAuctionService auctionService, IBidService bidService)
    {
        _auctionService = auctionService;
        _bidService = bidService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<AuctionDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Category? category = null,
        [FromQuery] AuctionStatus? status = null,
        [FromQuery] string? sortBy = null)
    {
        var result = await _auctionService.GetAllAsync(page, pageSize, category, status, sortBy);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AuctionDetailDto>> GetById(Guid id)
    {
        var auction = await _auctionService.GetByIdAsync(id);
        if (auction == null) return NotFound();
        return Ok(auction);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<AuctionDto>> Create([FromBody] CreateAuctionDto dto)
    {
        var ownerId = GetCurrentUserId();
        var auction = await _auctionService.CreateAsync(dto, ownerId);
        return CreatedAtAction(nameof(GetById), new { id = auction.Id }, auction);
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AuctionDto>> Update(Guid id, [FromBody] UpdateAuctionDto dto)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var result = await _auctionService.UpdateAsync(id, dto, currentUserId);
            if (result == null) return NotFound();
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var deleted = await _auctionService.DeleteAsync(id, currentUserId);
            if (!deleted) return NotFound();
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet("{id:guid}/bids")]
    public async Task<ActionResult<IEnumerable<BidDto>>> GetBids(Guid id)
    {
        var bids = await _bidService.GetByAuctionIdAsync(id);
        return Ok(bids);
    }

    [Authorize]
    [HttpPost("{id:guid}/bids")]
    public async Task<ActionResult<BidDto>> CreateBid(Guid id, [FromBody] CreateBidDto dto)
    {
        try
        {
            var bidderId = GetCurrentUserId();
            var bid = await _bidService.CreateAsync(id, dto, bidderId);
            return CreatedAtAction(nameof(GetBids), new { id }, bid);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private Guid GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return Guid.Parse(claim!.Value);
    }
}
