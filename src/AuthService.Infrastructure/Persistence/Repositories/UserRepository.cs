using AuthService.Application.Abstractions.Repositories;
using AuthService.Domain.Entities;
using AuthService.Domain.ValueObjects;
using AuthService.Infrastructure.Persistence.Context;
using BaitaHora.Domain.Features.Common.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AuthDbContext _context;

    public UserRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context
            .Users.Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(
        Email email,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .Users.Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Email.Value == email.Value, cancellationToken);
    }

    public async Task<User?> GetByUsernameAsync(
        Username username,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .Users.Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Username.Value == username.Value, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(
        Email email,
        CancellationToken cancellationToken = default
    )
    {
        return await _context.Users.AnyAsync(u => u.Email.Value == email.Value, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
