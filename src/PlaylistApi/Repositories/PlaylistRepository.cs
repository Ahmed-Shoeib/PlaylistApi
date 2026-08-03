using Microsoft.EntityFrameworkCore;
using PlaylistApi.Data;
using PlaylistApi.Models;

namespace PlaylistApi.Repositories;

public class PlaylistRepository : IPlaylistRepository
{
    private readonly PlaylistDbContext _context;

    public PlaylistRepository(PlaylistDbContext context)
    {
        _context = context;
    }

    public async Task<Playlist> AddAsync(Playlist playlist, CancellationToken cancellationToken)
    {
        _context.Playlists.Add(playlist);
        await _context.SaveChangesAsync(cancellationToken);
        return playlist;
    }

    public async Task<Playlist?> GetByIdWithSongsAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Playlists
            .Include(p => p.Songs)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<List<Playlist>> GetByUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        return await _context.Playlists
            .Include(p => p.Songs)
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Playlists
            .AsNoTracking()
            .AnyAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Playlist?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Playlists
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(Playlist playlist, CancellationToken cancellationToken)
    {
        playlist.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Playlist playlist, CancellationToken cancellationToken)
    {
        _context.Playlists.Remove(playlist);
        await _context.SaveChangesAsync(cancellationToken);
    }
}