using PlaylistApi.DTOs;

namespace PlaylistApi.Services;

public interface IPlaylistService
{
    Task<PlaylistResponse> CreatePlaylistAsync(int userId, CreatePlaylistRequest request, CancellationToken cancellationToken);
    Task<List<PlaylistSummaryResponse>> GetUserPlaylistsAsync(int userId, CancellationToken cancellationToken);
    Task<PlaylistResponse> GetPlaylistByIdAsync(int playlistId, CancellationToken cancellationToken);
}