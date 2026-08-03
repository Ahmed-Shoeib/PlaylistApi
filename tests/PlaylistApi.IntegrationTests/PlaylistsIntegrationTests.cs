using System.Net;
using System.Net.Http.Json;
using PlaylistApi.DTOs;
using Xunit;

namespace PlaylistApi.IntegrationTests;

public class PlaylistsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PlaylistsIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreatePlaylist_ForExistingUser_ReturnsCreated()
    {
        // Arrange
        var request = new CreatePlaylistRequest { Name = "Integration Test Playlist", Description = "Created by a test" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/1/playlists", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var created = await response.Content.ReadFromJsonAsync<PlaylistResponse>();
        Assert.NotNull(created);
        Assert.Equal("Integration Test Playlist", created!.Name);
        Assert.Equal(1, created.UserId);
        Assert.Empty(created.Songs);
    }

    [Fact]
    public async Task CreatePlaylist_ForNonExistentUser_ReturnsNotFound()
    {
        // Arrange
        var request = new CreatePlaylistRequest { Name = "Should Not Be Created" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/99999/playlists", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreatePlaylist_MissingRequiredName_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreatePlaylistRequest { Description = "No name provided" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/1/playlists", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetUserPlaylists_ForExistingUser_ReturnsOkWithList()
    {
        // Arrange — create a playlist first so we know at least one exists
        await _client.PostAsJsonAsync("/api/users/2/playlists",
            new CreatePlaylistRequest { Name = "Bob's Playlist" });

        // Act
        var response = await _client.GetAsync("/api/users/2/playlists");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var playlists = await response.Content.ReadFromJsonAsync<List<PlaylistSummaryResponse>>();
        Assert.NotNull(playlists);
        Assert.Contains(playlists!, p => p.Name == "Bob's Playlist");
    }

    [Fact]
    public async Task GetUserPlaylists_ForNonExistentUser_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/users/99999/playlists");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetPlaylistById_ExistingPlaylist_ReturnsOkWithDetails()
    {
        // Arrange
        var createResponse = await _client.PostAsJsonAsync("/api/users/1/playlists",
            new CreatePlaylistRequest { Name = "Detail Test Playlist" });
        var created = await createResponse.Content.ReadFromJsonAsync<PlaylistResponse>();

        // Act
        var response = await _client.GetAsync($"/api/playlists/{created!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var playlist = await response.Content.ReadFromJsonAsync<PlaylistResponse>();
        Assert.NotNull(playlist);
        Assert.Equal("Detail Test Playlist", playlist!.Name);
    }

    [Fact]
    public async Task GetPlaylistById_NonExistentPlaylist_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/playlists/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}