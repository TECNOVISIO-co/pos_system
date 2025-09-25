using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.Api.Contracts.Requests;
using Pos.Api.Contracts.Responses;
using Pos.Api.Validators;
using Pos.Domain.Entities;
using Pos.Infrastructure.Repositories;

namespace Pos.Api.Controllers;

[ApiController]
[Route("api/v1/sync")]
[Authorize]
public class SyncController : ControllerBase
{
    private readonly ISyncRepository _syncRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<SyncPushRequest> _validator;

    public SyncController(ISyncRepository syncRepository, IUnitOfWork unitOfWork, IValidator<SyncPushRequest> validator)
    {
        _syncRepository = syncRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    [HttpGet("changes")]
    [ProducesResponseType(typeof(SyncChangesResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetChanges([FromQuery] string entity, [FromQuery] long? since, [FromQuery] int page = 1, [FromQuery] int size = 100, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        size = Math.Clamp(size, 1, 500);
        var result = await _syncRepository.GetChangesAsync(entity, since, page, size, cancellationToken);
        var nextCursor = result.Items.Any() ? result.Items.Max(c => c.Id) : since;
        var response = new SyncChangesResponse(result.Items.Select(c => new SyncChangeResponse(c.Id, c.EntityName, c.EntityId, c.Operation, c.Payload, c.CreatedAt)).ToList(), nextCursor, result.Total);
        return Ok(response);
    }

    [HttpPost("changes")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> PushChanges([FromBody] SyncPushRequest request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var entries = request.Changes.Select(change =>
        {
            var payload = change.Data.HasValue ? JsonSerializer.Serialize(change.Data.Value) : string.Empty;
            return new SyncLogEntry(request.Entity, change.EntityId, change.Operation, "client", payload);
        }).ToList();

        await _syncRepository.AddEntriesAsync(entries, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Accepted();
    }
}
