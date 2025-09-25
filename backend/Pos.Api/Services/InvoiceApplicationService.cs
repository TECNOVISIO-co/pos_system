using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Pos.Api.Contracts.Requests;
using Pos.Domain.Entities;
using Pos.Infrastructure.Data;
using Pos.Infrastructure.Repositories;

namespace Pos.Api.Services;

public class InvoiceApplicationService : IInvoiceApplicationService
{
    private readonly PosDbContext _context;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IWarehouseStockRepository _warehouseStockRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<InvoiceRequest> _validator;

    public InvoiceApplicationService(
        PosDbContext context,
        IInvoiceRepository invoiceRepository,
        IPaymentRepository paymentRepository,
        IWarehouseStockRepository warehouseStockRepository,
        IUnitOfWork unitOfWork,
        IValidator<InvoiceRequest> validator)
    {
        _context = context;
        _invoiceRepository = invoiceRepository;
        _paymentRepository = paymentRepository;
        _warehouseStockRepository = warehouseStockRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Invoice> CreateAsync(InvoiceRequest request, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var existing = await _invoiceRepository.GetByNumberAsync(request.InvoiceNumber, cancellationToken);
        if (existing is not null)
        {
            throw new InvalidOperationException($"Invoice {request.InvoiceNumber} already exists");
        }

        var invoice = new Invoice(request.InvoiceNumber, request.CustomerId, request.IssuedAt, request.CurrencyCode);
        invoice.SetWarehouse(request.WarehouseId);
        invoice.SetDueDate(request.DueAt);
        invoice.SetNotes(request.Notes);

        foreach (var itemRequest in request.Items)
        {
            var product = await _context.Products
                .Include(p => p.Taxes)
                .FirstOrDefaultAsync(p => p.Id == itemRequest.ProductId, cancellationToken)
                ?? throw new InvalidOperationException($"Product {itemRequest.ProductId} not found");

            var taxIds = product.Taxes.Select(t => t.TaxId).ToList();
            var taxes = await _context.Taxes.Where(t => taxIds.Contains(t.Id)).ToListAsync(cancellationToken);

            var stock = await _warehouseStockRepository.GetAsync(request.WarehouseId, product.Id, cancellationToken);
            if (stock is null)
            {
                throw new InvalidOperationException($"Stock for product {product.Sku} not initialised in warehouse");
            }

            if (!product.AllowNegativeStock && stock.Available < itemRequest.Quantity)
            {
                throw new InvalidOperationException($"Insufficient stock for product {product.Sku}");
            }

            invoice.AddItem(product.Id, product.Name, itemRequest.Quantity, itemRequest.UnitPrice, itemRequest.DiscountRate, taxes);
            stock.Adjust(-itemRequest.Quantity);
            await _warehouseStockRepository.UpdateAsync(stock, cancellationToken);
        }

        if (request.Payments is not null)
        {
            foreach (var payment in request.Payments)
            {
                var entity = invoice.RegisterPayment(request.CustomerId, payment.Method, payment.Amount, payment.PaidAt, payment.CurrencyCode, payment.Reference);
                await _paymentRepository.AddAsync(entity, cancellationToken);
            }
        }

        invoice.Post();
        await _invoiceRepository.AddAsync(invoice, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return invoice;
    }
}
