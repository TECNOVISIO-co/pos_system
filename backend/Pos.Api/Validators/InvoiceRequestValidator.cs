using FluentValidation;
using Pos.Api.Contracts.Requests;

namespace Pos.Api.Validators;

public class InvoiceRequestValidator : AbstractValidator<InvoiceRequest>
{
    public InvoiceRequestValidator()
    {
        RuleFor(x => x.InvoiceNumber).NotEmpty();
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.WarehouseId).NotEmpty();
        RuleFor(x => x.IssuedAt).NotEmpty();
        RuleFor(x => x.CurrencyCode).Length(3);
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).SetValidator(new InvoiceItemRequestValidator());
        RuleForEach(x => x.Payments).SetValidator(new PaymentRequestValidator()).When(x => x.Payments != null);
    }
}

public class InvoiceItemRequestValidator : AbstractValidator<InvoiceItemRequest>
{
    public InvoiceItemRequestValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DiscountRate).InclusiveBetween(0, 100);
    }
}
