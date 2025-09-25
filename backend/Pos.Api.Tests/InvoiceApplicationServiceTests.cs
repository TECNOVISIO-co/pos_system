using FluentAssertions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Pos.Api.Contracts.Requests;
using Pos.Api.Services;
using Pos.Api.Validators;
using Pos.Domain.Entities;
using Pos.Domain.Enums;
using Pos.Infrastructure.Data;
using Pos.Infrastructure.Repositories;
using Xunit;

namespace Pos.Api.Tests;

public class InvoiceApplicationServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldPersistInvoiceAndReduceStock()
    {
        var options = new DbContextOptionsBuilder<PosDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new PosDbContext(options);
        var warehouse = new Warehouse("WH-01", "Principal");
        context.Warehouses.Add(warehouse);
        var tax = new Tax("IVA19", "IVA 19%", 19m);
        context.Taxes.Add(tax);
        var product = new Product("SKU-1", "Producto", 120m);
        product.AssignTax(tax.Id, 1);
        context.Products.Add(product);
        var stock = new WarehouseStock(warehouse.Id, product.Id);
        stock.SetAbsolute(50m, 0);
        context.WarehouseStocks.Add(stock);
        await context.SaveChangesAsync();

        var invoiceRepository = new InvoiceRepository(context);
        var paymentRepository = new PaymentRepository(context);
        var warehouseStockRepository = new WarehouseStockRepository(context);
        var unitOfWork = new UnitOfWork(context);
        IValidator<InvoiceRequest> validator = new InvoiceRequestValidator();
        var service = new InvoiceApplicationService(context, invoiceRepository, paymentRepository, warehouseStockRepository, unitOfWork, validator);

        var request = new InvoiceRequest(
            "INV-2001",
            Guid.NewGuid(),
            warehouse.Id,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow.AddDays(5),
            "COP",
            "",
            new List<InvoiceItemRequest> { new(product.Id, 5m, 120m, 0m) },
            new List<PaymentRequest>
            {
                new(PaymentMethod.Cash, 300m, DateTimeOffset.UtcNow, "COP", "CAJA")
            }
        );

        var invoice = await service.CreateAsync(request);

        var storedInvoices = await context.Invoices.CountAsync();
        storedInvoices.Should().Be(1);
        invoice.Total.Should().BeGreaterThan(0);
        invoice.Items.Should().HaveCount(1);
        var updatedStock = await context.WarehouseStocks.FirstAsync();
        updatedStock.OnHand.Should().Be(45m);
        updatedStock.Reserved.Should().Be(0m);
        invoice.TotalPaid.Should().Be(300m);
    }
}
