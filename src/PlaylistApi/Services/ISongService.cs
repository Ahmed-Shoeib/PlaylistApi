using PlaylistApi.DTOs;

namespace PlaylistApi.Services;

public interface ISongService
{
    Task<SongResponse> AddSongToPlaylistAsync(int playlistId, CreateSongRequest request, CancellationToken cancellationToken);
    Task<SongResponse> UpdateSongAsync(int playlistId, int songId, UpdateSongRequest request, CancellationToken cancellationToken);
    Task DeleteSongAsync(int playlistId, int songId, CancellationToken cancellationToken);
}