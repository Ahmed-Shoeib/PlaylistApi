using PlaylistApi.Models;

namespace PlaylistApi.Repositories;

public interface ISongRepository
{
    Task<Song> AddAsync(Song song, CancellationToken cancellationToken);
    Task<Song?> GetByIdAsync(int songId, int playlistId, CancellationToken cancellationToken);
    Task UpdateAsync(Song song, CancellationToken cancellationToken);
    Task DeleteAsync(Song song, CancellationToken cancellationToken);
}