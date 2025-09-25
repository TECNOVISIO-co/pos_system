using FluentValidation;
using Pos.Api.Contracts.Requests;
using Pos.Domain.Entities;
using Pos.Infrastructure.Repositories;

namespace Pos.Api.Services;

public class PaymentApplicationService : IPaymentApplicationService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreatePaymentRequest> _validator;

    public PaymentApplicationService(IPaymentRepository paymentRepository, IUnitOfWork unitOfWork, IValidator<CreatePaymentRequest> validator)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Payment> CreateAsync(CreatePaymentRequest request, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var payment = new Payment(request.InvoiceId, request.CustomerId, request.Method, request.Amount, request.PaidAt, request.CurrencyCode);
        payment.SetExchangeRate(request.ExchangeRate);
        payment.SetReference(request.Reference);
        payment.SetNotes(request.Notes);

        await _paymentRepository.AddAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return payment;
    }
}
