using PlaylistApi.Models;

namespace PlaylistApi.Repositories;

public interface ISongRepository
{
    Task<Song> AddAsync(Song song, CancellationToken cancellationToken);
}