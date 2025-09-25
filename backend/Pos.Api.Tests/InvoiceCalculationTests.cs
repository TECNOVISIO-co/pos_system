using FluentAssertions;
using Pos.Domain.Entities;
using Pos.Domain.Enums;
using Xunit;

namespace Pos.Api.Tests;

public class InvoiceCalculationTests
{
    [Fact]
    public void AddItem_ShouldCalculateTotalsWithDiscountAndTax()
    {
        var tax = new Tax("IVA19", "IVA 19%", 19m);
        var invoice = new Invoice("INV-1001", Guid.NewGuid(), DateTimeOffset.UtcNow, "COP");

        invoice.AddItem(Guid.NewGuid(), "Producto de prueba", 2, 100m, 10m, new[] { tax });
        invoice.RecalculateTotals();

        invoice.Subtotal.Should().Be(180m);
        invoice.DiscountTotal.Should().Be(20m);
        invoice.TaxTotal.Should().Be(34.2m);
        invoice.Total.Should().Be(214.2m);
        invoice.BalanceDue.Should().Be(214.2m);
    }

    [Fact]
    public void RegisterPayment_ShouldUpdateBalanceAndStatus()
    {
        var invoice = new Invoice("INV-1002", Guid.NewGuid(), DateTimeOffset.UtcNow, "COP");
        var tax = new Tax("IVA19", "IVA 19%", 19m);
        invoice.AddItem(Guid.NewGuid(), "Producto", 1, 100m, 0m, new[] { tax });
        invoice.Post();

        invoice.RegisterPayment(Guid.NewGuid(), PaymentMethod.Cash, 50m, DateTimeOffset.UtcNow, "COP");
        invoice.TotalPaid.Should().Be(50m);
        invoice.Status.Should().Be(InvoiceStatus.PartiallyPaid);
        invoice.BalanceDue.Should().Be(invoice.Total - 50m);

        invoice.RegisterPayment(Guid.NewGuid(), PaymentMethod.Card, invoice.BalanceDue, DateTimeOffset.UtcNow, "COP");
        invoice.Status.Should().Be(InvoiceStatus.Paid);
        invoice.BalanceDue.Should().Be(0m);
    }
}
