using BankingApi.Application.DTOs;
using FluentValidation;

namespace BankingApi.Application.Validators;

public class TransferRequestValidator : AbstractValidator<TransferRequest>
{
    public TransferRequestValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.FromAccountId)
            .NotEmpty().WithMessage("'From' account ID is required.");

        RuleFor(x => x.ToAccountId)
            .NotEmpty().WithMessage("'To' account ID is required.");

        RuleFor(x => x)
            .Must(x => x.FromAccountId != x.ToAccountId)
            .WithMessage("Cannot transfer to the same account.");
    }
}
