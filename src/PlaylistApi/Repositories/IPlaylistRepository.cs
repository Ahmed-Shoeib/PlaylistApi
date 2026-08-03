using PlaylistApi.Models;

namespace PlaylistApi.Repositories;

public interface IPlaylistRepository
{
    Task<Playlist> AddAsync(Playlist playlist, CancellationToken cancellationToken);
    Task<Playlist?> GetByIdWithSongsAsync(int id, CancellationToken cancellationToken);
    Task<List<Playlist>> GetByUserIdAsync(int userId, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
    Task<Playlist?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task UpdateAsync(Playlist playlist, CancellationToken cancellationToken);
    Task DeleteAsync(Playlist playlist, CancellationToken cancellationToken);
}