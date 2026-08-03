using PlaylistApi.DTOs;

namespace PlaylistApi.Services;

public interface IPlaylistService
{
    Task<PlaylistResponse> CreatePlaylistAsync(int userId, CreatePlaylistRequest request, CancellationToken cancellationToken);
    Task<List<PlaylistSummaryResponse>> GetUserPlaylistsAsync(int userId, CancellationToken cancellationToken);
    Task<PlaylistResponse> GetPlaylistByIdAsync(int playlistId, CancellationToken cancellationToken);
    Task<PlaylistResponse> UpdatePlaylistAsync(int playlistId, UpdatePlaylistRequest request, CancellationToken cancellationToken);
    Task DeletePlaylistAsync(int playlistId, CancellationToken cancellationToken);
}