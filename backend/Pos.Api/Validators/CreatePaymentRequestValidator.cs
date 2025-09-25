using FluentValidation;
using Pos.Api.Contracts.Requests;

namespace Pos.Api.Validators;

public class CreatePaymentRequestValidator : AbstractValidator<CreatePaymentRequest>
{
    public CreatePaymentRequestValidator()
    {
        RuleFor(x => x.Method).IsInEnum();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.CurrencyCode).Length(3);
        RuleFor(x => x.ExchangeRate).GreaterThan(0);
        RuleFor(x => x.PaidAt).NotEmpty();
        RuleFor(x => x).Must(x => x.InvoiceId.HasValue || x.CustomerId.HasValue)
            .WithMessage("InvoiceId or CustomerId must be provided");
    }
}
