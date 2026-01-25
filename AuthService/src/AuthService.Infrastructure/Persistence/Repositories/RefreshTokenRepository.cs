using AuthService.Application.Abstractions.Repositories;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Persistence.Repositories;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AuthDbContext _context;

    public RefreshTokenRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetByTokenAsync(
        string token,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .RefreshTokens.Where(rt => rt.Token == token && !rt.IsRevoked && !rt.IsExpired())
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default
    )
    {
        await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default
    )
    {
        refreshToken.Revoke();
        _context.RefreshTokens.Update(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeAllByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        var tokens = await _context
            .RefreshTokens.Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.Revoke();
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
