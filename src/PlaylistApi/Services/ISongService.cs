using PlaylistApi.DTOs;

namespace PlaylistApi.Services;

public interface ISongService
{
    Task<SongResponse> AddSongToPlaylistAsync(int playlistId, CreateSongRequest request, CancellationToken cancellationToken);
}