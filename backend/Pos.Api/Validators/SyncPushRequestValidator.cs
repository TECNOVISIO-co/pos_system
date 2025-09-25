using FluentValidation;
using Pos.Api.Contracts.Requests;

namespace Pos.Api.Validators;

public class SyncPushRequestValidator : AbstractValidator<SyncPushRequest>
{
    public SyncPushRequestValidator()
    {
        RuleFor(x => x.Entity).NotEmpty();
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.BatchId).NotEmpty();
        RuleFor(x => x.Changes).NotEmpty();
        RuleForEach(x => x.Changes).SetValidator(new SyncChangeRequestValidator());
    }
}

public class SyncChangeRequestValidator : AbstractValidator<SyncChangeRequest>
{
    public SyncChangeRequestValidator()
    {
        RuleFor(x => x.Operation).IsInEnum();
    }
}
