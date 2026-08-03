using Microsoft.EntityFrameworkCore;
using PlaylistApi.Data;
using PlaylistApi.Models;

namespace PlaylistApi.Repositories;

public class SongRepository : ISongRepository
{
    private readonly PlaylistDbContext _context;

    public SongRepository(PlaylistDbContext context)
    {
        _context = context;
    }

    public async Task<Song> AddAsync(Song song, CancellationToken cancellationToken)
    {
        _context.Songs.Add(song);
        await _context.SaveChangesAsync(cancellationToken);
        return song;
    }

    public async Task<Song?> GetByIdAsync(int songId, int playlistId, CancellationToken cancellationToken)
    {
        return await _context.Songs
            .FirstOrDefaultAsync(s => s.Id == songId && s.PlaylistId == playlistId, cancellationToken);
    }

    public async Task UpdateAsync(Song song, CancellationToken cancellationToken)
    {
        song.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Song song, CancellationToken cancellationToken)
    {
        _context.Songs.Remove(song);
        await _context.SaveChangesAsync(cancellationToken);
    }
}