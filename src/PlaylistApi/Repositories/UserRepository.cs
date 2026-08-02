using Microsoft.EntityFrameworkCore;
using PlaylistApi.Data;
using PlaylistApi.Models;

namespace PlaylistApi.Repositories;

public class UserRepository : IUserRepository
{
    private readonly PlaylistDbContext _context;

    public UserRepository(PlaylistDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == id, cancellationToken);
    }
}