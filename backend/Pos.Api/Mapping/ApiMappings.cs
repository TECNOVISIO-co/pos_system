using Pos.Api.Contracts.Responses;
using Pos.Domain.Entities;

namespace Pos.Api.Mapping;

public static class ApiMappings
{
    public static ProductResponse ToResponse(this Product product)
        => new(product.Id, product.Sku, product.Name, product.Description, product.Price, product.Cost, product.IsActive, product.UpdatedAt);

    public static CustomerResponse ToResponse(this Customer customer)
        => new(customer.Id, customer.Code, customer.Name, customer.DocumentType, customer.DocumentNumber, customer.Email, customer.Phone, customer.CreditLimit, customer.AvailableCredit, customer.UpdatedAt);

    public static TaxResponse ToResponse(this Tax tax)
        => new(tax.Id, tax.Code, tax.Name, tax.Rate, tax.Scope, tax.IsCompound);

    public static PriceListResponse ToResponse(this PriceList priceList)
        => new(priceList.Id, priceList.Code, priceList.Name, priceList.CurrencyCode, priceList.IsDefault);

    public static InvoiceCreatedResponse ToResponse(this Invoice invoice)
        => new(invoice.Id, invoice.InvoiceNumber, invoice.Total, invoice.BalanceDue);
}
