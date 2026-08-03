using PlaylistApi.DTOs;
using PlaylistApi.Exceptions;
using PlaylistApi.Models;
using PlaylistApi.Repositories;

namespace PlaylistApi.Services;

public class SongService : ISongService
{
    private readonly ISongRepository _songRepository;
    private readonly IPlaylistRepository _playlistRepository;

    public SongService(ISongRepository songRepository, IPlaylistRepository playlistRepository)
    {
        _songRepository = songRepository;
        _playlistRepository = playlistRepository;
    }

    public async Task<SongResponse> AddSongToPlaylistAsync(
        int playlistId, CreateSongRequest request, CancellationToken cancellationToken)
    {
        var playlistExists = await _playlistRepository.ExistsAsync(playlistId, cancellationToken);
        if (!playlistExists)
        {
            throw new NotFoundException($"Playlist with id {playlistId} was not found.");
        }

        var song = new Song
        {
            Title = request.Title,
            Artist = request.Artist,
            Album = request.Album,
            DurationInSeconds = request.DurationInSeconds,
            PlaylistId = playlistId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _songRepository.AddAsync(song, cancellationToken);

        return MapToResponse(created);
    }

    private static SongResponse MapToResponse(Song song)
    {
        return new SongResponse
        {
            Id = song.Id,
            Title = song.Title,
            Artist = song.Artist,
            Album = song.Album,
            DurationInSeconds = song.DurationInSeconds,
            PlaylistId = song.PlaylistId,
            CreatedAt = song.CreatedAt,
            UpdatedAt = song.UpdatedAt
        };
    }
}