using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pos.Api.Auth;
using Pos.Api.Contracts.Requests;
using Pos.Api.Contracts.Responses;
using Pos.Api.Validators;
using Pos.Domain.Entities;
using Pos.Infrastructure.Data;

namespace Pos.Api.Services;

public class AuthService : IAuthService
{
    private readonly PosDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IValidator<LoginRequest> _validator;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public AuthService(PosDbContext context, IJwtTokenService jwtTokenService, IValidator<LoginRequest> validator)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _validator = validator;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var user = await _context.Users.Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive && u.DeletedAt == null, cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verification == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var roleIds = user.Roles.Select(r => r.RoleId).ToList();
        var roles = await _context.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.Code).ToListAsync(cancellationToken);
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        return new LoginResponse(accessToken, refreshToken, roles.FirstOrDefault() ?? "user");
    }
}
