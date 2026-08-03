using Moq;
using PlaylistApi.DTOs;
using PlaylistApi.Exceptions;
using PlaylistApi.Models;
using PlaylistApi.Repositories;
using PlaylistApi.Services;
using Xunit;

namespace PlaylistApi.UnitTests;

public class PlaylistServiceTests
{
    private readonly Mock<IPlaylistRepository> _playlistRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly PlaylistService _sut;

    public PlaylistServiceTests()
    {
        _sut = new PlaylistService(_playlistRepositoryMock.Object, _userRepositoryMock.Object);
    }

    [Fact]
    public async Task CreatePlaylistAsync_UserExists_CreatesAndReturnsPlaylist()
    {
        // Arrange
        var userId = 1;
        var request = new CreatePlaylistRequest { Name = "Road Trip", Description = "Long drive songs" };

        _userRepositoryMock
            .Setup(r => r.ExistsAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _playlistRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Playlist>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Playlist p, CancellationToken _) =>
            {
                p.Id = 42;
                return p;
            });

        // Act
        var result = await _sut.CreatePlaylistAsync(userId, request, CancellationToken.None);

        // Assert
        Assert.Equal(42, result.Id);
        Assert.Equal("Road Trip", result.Name);
        Assert.Equal("Long drive songs", result.Description);
        Assert.Equal(userId, result.UserId);
        Assert.Empty(result.Songs);

        _playlistRepositoryMock.Verify(
            r => r.AddAsync(It.Is<Playlist>(p => p.Name == "Road Trip" && p.UserId == userId), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreatePlaylistAsync_UserDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var userId = 999;
        var request = new CreatePlaylistRequest { Name = "Road Trip" };

        _userRepositoryMock
            .Setup(r => r.ExistsAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.CreatePlaylistAsync(userId, request, CancellationToken.None));

        _playlistRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Playlist>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetUserPlaylistsAsync_UserExists_ReturnsPlaylistSummaries()
    {
        // Arrange
        var userId = 1;
        var playlists = new List<Playlist>
        {
            new()
            {
                Id = 1, Name = "Playlist A", UserId = userId,
                Songs = new List<Song> { new() { Id = 1, Title = "Song 1", Artist = "Artist 1" } }
            },
            new()
            {
                Id = 2, Name = "Playlist B", UserId = userId,
                Songs = new List<Song>()
            }
        };

        _userRepositoryMock
            .Setup(r => r.ExistsAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _playlistRepositoryMock
            .Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(playlists);

        // Act
        var result = await _sut.GetUserPlaylistsAsync(userId, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].SongCount);
        Assert.Equal(0, result[1].SongCount);
    }

    [Fact]
    public async Task GetUserPlaylistsAsync_UserDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var userId = 999;

        _userRepositoryMock
            .Setup(r => r.ExistsAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.GetUserPlaylistsAsync(userId, CancellationToken.None));
    }

    [Fact]
    public async Task GetPlaylistByIdAsync_PlaylistExists_ReturnsPlaylist()
    {
        // Arrange
        var playlistId = 5;
        var playlist = new Playlist
        {
            Id = playlistId,
            Name = "Existing Playlist",
            UserId = 1,
            Songs = new List<Song>
            {
                new() { Id = 10, Title = "Song 1", Artist = "Artist 1", PlaylistId = playlistId }
            }
        };

        _playlistRepositoryMock
            .Setup(r => r.GetByIdWithSongsAsync(playlistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(playlist);

        // Act
        var result = await _sut.GetPlaylistByIdAsync(playlistId, CancellationToken.None);

        // Assert
        Assert.Equal(playlistId, result.Id);
        Assert.Single(result.Songs);
        Assert.Equal("Song 1", result.Songs[0].Title);
    }

    [Fact]
    public async Task GetPlaylistByIdAsync_PlaylistDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var playlistId = 999;

        _playlistRepositoryMock
            .Setup(r => r.GetByIdWithSongsAsync(playlistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Playlist?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.GetPlaylistByIdAsync(playlistId, CancellationToken.None));
    }

    [Fact]
    public async Task UpdatePlaylistAsync_PlaylistExists_UpdatesNameAndDescription()
    {
        // Arrange
        var playlistId = 3;
        var request = new UpdatePlaylistRequest { Name = "New Name", Description = "New Description" };

        var existingPlaylist = new Playlist
        {
            Id = playlistId,
            Name = "Old Name",
            Description = "Old Description",
            UserId = 1
        };

        var reloadedPlaylist = new Playlist
        {
            Id = playlistId,
            Name = "New Name",
            Description = "New Description",
            UserId = 1,
            Songs = new List<Song>()
        };

        _playlistRepositoryMock
            .Setup(r => r.GetByIdAsync(playlistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPlaylist);

        _playlistRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Playlist>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _playlistRepositoryMock
            .Setup(r => r.GetByIdWithSongsAsync(playlistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reloadedPlaylist);

        // Act
        var result = await _sut.UpdatePlaylistAsync(playlistId, request, CancellationToken.None);

        // Assert
        Assert.Equal("New Name", result.Name);
        Assert.Equal("New Description", result.Description);

        _playlistRepositoryMock.Verify(
            r => r.UpdateAsync(It.Is<Playlist>(p => p.Name == "New Name"), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdatePlaylistAsync_PlaylistDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var playlistId = 999;
        var request = new UpdatePlaylistRequest { Name = "New Name" };

        _playlistRepositoryMock
            .Setup(r => r.GetByIdAsync(playlistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Playlist?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.UpdatePlaylistAsync(playlistId, request, CancellationToken.None));

        _playlistRepositoryMock.Verify(
            r => r.UpdateAsync(It.IsAny<Playlist>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task DeletePlaylistAsync_PlaylistExists_DeletesPlaylist()
    {
        // Arrange
        var playlistId = 7;
        var existingPlaylist = new Playlist { Id = playlistId, Name = "To Delete", UserId = 1 };

        _playlistRepositoryMock
            .Setup(r => r.GetByIdAsync(playlistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPlaylist);

        _playlistRepositoryMock
            .Setup(r => r.DeleteAsync(existingPlaylist, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.DeletePlaylistAsync(playlistId, CancellationToken.None);

        // Assert
        _playlistRepositoryMock.Verify(
            r => r.DeleteAsync(existingPlaylist, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeletePlaylistAsync_PlaylistDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var playlistId = 999;

        _playlistRepositoryMock
            .Setup(r => r.GetByIdAsync(playlistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Playlist?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.DeletePlaylistAsync(playlistId, CancellationToken.None));

        _playlistRepositoryMock.Verify(
            r => r.DeleteAsync(It.IsAny<Playlist>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}