using PlaylistApi.DTOs;
using PlaylistApi.Exceptions;
using PlaylistApi.Models;
using PlaylistApi.Repositories;

namespace PlaylistApi.Services;

public class PlaylistService : IPlaylistService
{
    private readonly IPlaylistRepository _playlistRepository;
    private readonly IUserRepository _userRepository;

    public PlaylistService(IPlaylistRepository playlistRepository, IUserRepository userRepository)
    {
        _playlistRepository = playlistRepository;
        _userRepository = userRepository;
    }

    public async Task<PlaylistResponse> CreatePlaylistAsync(
        int userId, CreatePlaylistRequest request, CancellationToken cancellationToken)
    {
        var userExists = await _userRepository.ExistsAsync(userId, cancellationToken);
        if (!userExists)
        {
            throw new NotFoundException($"User with id {userId} was not found.");
        }

        var playlist = new Playlist
        {
            Name = request.Name,
            Description = request.Description,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _playlistRepository.AddAsync(playlist, cancellationToken);

        return MapToResponse(created);
    }

    public async Task<List<PlaylistSummaryResponse>> GetUserPlaylistsAsync(
        int userId, CancellationToken cancellationToken)
    {
        var userExists = await _userRepository.ExistsAsync(userId, cancellationToken);
        if (!userExists)
        {
            throw new NotFoundException($"User with id {userId} was not found.");
        }

        var playlists = await _playlistRepository.GetByUserIdAsync(userId, cancellationToken);

        return playlists.Select(MapToSummaryResponse).ToList();
    }

    public async Task<PlaylistResponse> GetPlaylistByIdAsync(
        int playlistId, CancellationToken cancellationToken)
    {
        var playlist = await _playlistRepository.GetByIdWithSongsAsync(playlistId, cancellationToken);
        if (playlist is null)
        {
            throw new NotFoundException($"Playlist with id {playlistId} was not found.");
        }

        return MapToResponse(playlist);
    }

    public async Task<PlaylistResponse> UpdatePlaylistAsync(
        int playlistId, UpdatePlaylistRequest request, CancellationToken cancellationToken)
    {
        var playlist = await _playlistRepository.GetByIdAsync(playlistId, cancellationToken);
        if (playlist is null)
        {
            throw new NotFoundException($"Playlist with id {playlistId} was not found.");
        }

        playlist.Name = request.Name;
        playlist.Description = request.Description;

        await _playlistRepository.UpdateAsync(playlist, cancellationToken);

        var updated = await _playlistRepository.GetByIdWithSongsAsync(playlistId, cancellationToken);
        return MapToResponse(updated!);
    }

    public async Task DeletePlaylistAsync(int playlistId, CancellationToken cancellationToken)
    {
        var playlist = await _playlistRepository.GetByIdAsync(playlistId, cancellationToken);
        if (playlist is null)
        {
            throw new NotFoundException($"Playlist with id {playlistId} was not found.");
        }

        await _playlistRepository.DeleteAsync(playlist, cancellationToken);
    }

    private static PlaylistResponse MapToResponse(Playlist playlist)
    {
        return new PlaylistResponse
        {
            Id = playlist.Id,
            Name = playlist.Name,
            Description = playlist.Description,
            UserId = playlist.UserId,
            CreatedAt = playlist.CreatedAt,
            UpdatedAt = playlist.UpdatedAt,
            Songs = playlist.Songs.Select(s => new SongResponse
            {
                Id = s.Id,
                Title = s.Title,
                Artist = s.Artist,
                Album = s.Album,
                DurationInSeconds = s.DurationInSeconds,
                PlaylistId = s.PlaylistId,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            }).ToList()
        };
    }

    private static PlaylistSummaryResponse MapToSummaryResponse(Playlist playlist)
    {
        return new PlaylistSummaryResponse
        {
            Id = playlist.Id,
            Name = playlist.Name,
            Description = playlist.Description,
            SongCount = playlist.Songs.Count,
            CreatedAt = playlist.CreatedAt
        };
    }
}