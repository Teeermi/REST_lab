using AuctionSystem.Application.DTOs;
using FluentValidation;

namespace AuctionSystem.Application.Validators;

public class CreateBidValidator : AbstractValidator<CreateBidDto>
{
    public CreateBidValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Kwota oferty musi być większa od 0");
    }
}
