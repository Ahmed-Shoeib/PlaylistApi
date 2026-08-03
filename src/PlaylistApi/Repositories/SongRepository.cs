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
}