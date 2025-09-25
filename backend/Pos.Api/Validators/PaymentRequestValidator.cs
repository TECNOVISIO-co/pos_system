using FluentValidation;
using Pos.Api.Contracts.Requests;

namespace Pos.Api.Validators;

public class PaymentRequestValidator : AbstractValidator<PaymentRequest>
{
    public PaymentRequestValidator()
    {
        RuleFor(x => x.Method).IsInEnum();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.CurrencyCode).Length(3);
        RuleFor(x => x.PaidAt).NotEmpty();
    }
}
