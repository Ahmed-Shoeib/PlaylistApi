using System.Net;
using System.Net.Http.Json;
using PlaylistApi.DTOs;
using Xunit;

namespace PlaylistApi.IntegrationTests;

public class SongsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public SongsIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<int> CreateTestPlaylistAsync()
    {
        var request = new CreatePlaylistRequest
        {
            Name = "Playlist For Song Tests"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/users/1/playlists",
            request);

        response.EnsureSuccessStatusCode();

        var playlist =
            await response.Content.ReadFromJsonAsync<PlaylistResponse>();

        Assert.NotNull(playlist);

        return playlist.Id;
    }

    [Fact]
    public async Task AddSong_ToExistingPlaylist_ReturnsCreatedAndAppearsInPlaylist()
    {
        // Arrange
        var playlistId = await CreateTestPlaylistAsync();

        var request = new CreateSongRequest
        {
            Title = "Integration Test Song",
            Artist = "Test Artist",
            Album = "Test Album",
            DurationInSeconds = 200
        };

        // Act
        var addResponse = await _client.PostAsJsonAsync(
            $"/api/playlists/{playlistId}/songs",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, addResponse.StatusCode);

        var song =
            await addResponse.Content.ReadFromJsonAsync<SongResponse>();

        Assert.NotNull(song);
        Assert.Equal("Integration Test Song", song.Title);
        Assert.Equal("Test Artist", song.Artist);
        Assert.Equal("Test Album", song.Album);
        Assert.Equal(200, song.DurationInSeconds);
        Assert.Equal(playlistId, song.PlaylistId);

        var getResponse = await _client.GetAsync(
            $"/api/playlists/{playlistId}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var playlist =
            await getResponse.Content.ReadFromJsonAsync<PlaylistResponse>();

        Assert.NotNull(playlist);

        Assert.Contains(
            playlist.Songs,
            existingSong =>
                existingSong.Title == "Integration Test Song" &&
                existingSong.Artist == "Test Artist");
    }

    [Fact]
    public async Task AddSong_ToNonExistentPlaylist_ReturnsNotFound()
    {
        // Arrange
        var request = new CreateSongRequest
        {
            Title = "Orphan Song",
            Artist = "Nobody"
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/playlists/999999/songs",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddSong_MissingRequiredArtist_ReturnsBadRequest()
    {
        // Arrange
        var playlistId = await CreateTestPlaylistAsync();

        var request = new CreateSongRequest
        {
            Title = "No Artist Song"
            // Artist remains string.Empty, so [Required] validation should fail.
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            $"/api/playlists/{playlistId}/songs",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}