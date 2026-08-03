using Moq;
using PlaylistApi.DTOs;
using PlaylistApi.Exceptions;
using PlaylistApi.Models;
using PlaylistApi.Repositories;
using PlaylistApi.Services;
using Xunit;

namespace PlaylistApi.UnitTests;

public class SongServiceTests
{
    private readonly Mock<ISongRepository> _songRepositoryMock = new();
    private readonly Mock<IPlaylistRepository> _playlistRepositoryMock = new();
    private readonly SongService _sut;

    public SongServiceTests()
    {
        _sut = new SongService(_songRepositoryMock.Object, _playlistRepositoryMock.Object);
    }

    [Fact]
    public async Task AddSongToPlaylistAsync_PlaylistExists_CreatesAndReturnsSong()
    {
        // Arrange
        var playlistId = 1;
        var request = new CreateSongRequest
        {
            Title = "Highway Star",
            Artist = "Deep Purple",
            Album = "Machine Head",
            DurationInSeconds = 361
        };

        _playlistRepositoryMock
            .Setup(r => r.ExistsAsync(playlistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _songRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Song>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Song s, CancellationToken _) =>
            {
                s.Id = 100;
                return s;
            });

        // Act
        var result = await _sut.AddSongToPlaylistAsync(playlistId, request, CancellationToken.None);

        // Assert
        Assert.Equal(100, result.Id);
        Assert.Equal("Highway Star", result.Title);
        Assert.Equal(playlistId, result.PlaylistId);

        _songRepositoryMock.Verify(
            r => r.AddAsync(It.Is<Song>(s => s.Title == "Highway Star" && s.PlaylistId == playlistId), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task AddSongToPlaylistAsync_PlaylistDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var playlistId = 999;
        var request = new CreateSongRequest { Title = "Some Song", Artist = "Some Artist" };

        _playlistRepositoryMock
            .Setup(r => r.ExistsAsync(playlistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.AddSongToPlaylistAsync(playlistId, request, CancellationToken.None));

        _songRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Song>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateSongAsync_SongExists_UpdatesAndReturnsSong()
    {
        // Arrange
        var playlistId = 1;
        var songId = 10;
        var request = new UpdateSongRequest
        {
            Title = "Highway Star (Live)",
            Artist = "Deep Purple",
            Album = "Made in Japan",
            DurationInSeconds = 400
        };

        var existingSong = new Song
        {
            Id = songId,
            Title = "Highway Star",
            Artist = "Deep Purple",
            PlaylistId = playlistId
        };

        _songRepositoryMock
            .Setup(r => r.GetByIdAsync(songId, playlistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingSong);

        _songRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Song>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.UpdateSongAsync(playlistId, songId, request, CancellationToken.None);

        // Assert
        Assert.Equal("Highway Star (Live)", result.Title);
        Assert.Equal("Made in Japan", result.Album);
        Assert.Equal(400, result.DurationInSeconds);
    }

    [Fact]
    public async Task UpdateSongAsync_SongDoesNotExistInPlaylist_ThrowsNotFoundException()
    {
        // Arrange
        var playlistId = 1;
        var songId = 999;
        var request = new UpdateSongRequest { Title = "Doesn't matter", Artist = "Doesn't matter" };

        _songRepositoryMock
            .Setup(r => r.GetByIdAsync(songId, playlistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Song?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.UpdateSongAsync(playlistId, songId, request, CancellationToken.None));

        _songRepositoryMock.Verify(
            r => r.UpdateAsync(It.IsAny<Song>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteSongAsync_SongExists_DeletesSong()
    {
        // Arrange
        var playlistId = 1;
        var songId = 10;
        var existingSong = new Song { Id = songId, Title = "To Delete", Artist = "Artist", PlaylistId = playlistId };

        _songRepositoryMock
            .Setup(r => r.GetByIdAsync(songId, playlistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingSong);

        _songRepositoryMock
            .Setup(r => r.DeleteAsync(existingSong, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.DeleteSongAsync(playlistId, songId, CancellationToken.None);

        // Assert
        _songRepositoryMock.Verify(
            r => r.DeleteAsync(existingSong, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteSongAsync_SongDoesNotExistInPlaylist_ThrowsNotFoundException()
    {
        // Arrange
        var playlistId = 1;
        var songId = 999;

        _songRepositoryMock
            .Setup(r => r.GetByIdAsync(songId, playlistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Song?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.DeleteSongAsync(playlistId, songId, CancellationToken.None));

        _songRepositoryMock.Verify(
            r => r.DeleteAsync(It.IsAny<Song>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}