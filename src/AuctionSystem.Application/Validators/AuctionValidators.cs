using AuctionSystem.Application.DTOs;
using FluentValidation;

namespace AuctionSystem.Application.Validators;

public class CreateAuctionValidator : AbstractValidator<CreateAuctionDto>
{
    public CreateAuctionValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Tytuł jest wymagany")
            .MaximumLength(200).WithMessage("Tytuł może mieć maksymalnie 200 znaków");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Opis jest wymagany");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Nieprawidłowa kategoria");

        RuleFor(x => x.StartingPrice)
            .GreaterThan(0).WithMessage("Cena wywoławcza musi być większa od 0");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate).WithMessage("Data zakończenia musi być późniejsza niż data rozpoczęcia");
    }
}

public class UpdateAuctionValidator : AbstractValidator<UpdateAuctionDto>
{
    public UpdateAuctionValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(200).WithMessage("Tytuł może mieć maksymalnie 200 znaków")
            .When(x => !string.IsNullOrEmpty(x.Title));

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Nieprawidłowa kategoria")
            .When(x => x.Category.HasValue);
    }
}
