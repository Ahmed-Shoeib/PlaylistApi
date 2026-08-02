using Microsoft.AspNetCore.Mvc;
using PlaylistApi.DTOs;
using PlaylistApi.Services;

namespace PlaylistApi.Controllers;

[ApiController]
public class PlaylistsController : ControllerBase
{
    private readonly IPlaylistService _playlistService;

    public PlaylistsController(IPlaylistService playlistService)
    {
        _playlistService = playlistService;
    }

    /// <summary>
    /// Creates a new playlist for the specified user.
    /// </summary>
    [HttpPost("api/users/{userId:int}/playlists")]
    [ProducesResponseType(typeof(PlaylistResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PlaylistResponse>> CreatePlaylist(
        int userId,
        [FromBody] CreatePlaylistRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _playlistService.CreatePlaylistAsync(userId, request, cancellationToken);

        return CreatedAtAction(
            nameof(GetPlaylistById),
            new { playlistId = result.Id },
            result);
    }

    /// <summary>
    /// Gets all playlists belonging to the specified user.
    /// </summary>
    [HttpGet("api/users/{userId:int}/playlists")]
    [ProducesResponseType(typeof(List<PlaylistSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<PlaylistSummaryResponse>>> GetUserPlaylists(
        int userId,
        CancellationToken cancellationToken)
    {
        var result = await _playlistService.GetUserPlaylistsAsync(userId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a single playlist, including its songs.
    /// </summary>
    [HttpGet("api/playlists/{playlistId:int}")]
    [ProducesResponseType(typeof(PlaylistResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PlaylistResponse>> GetPlaylistById(
        int playlistId,
        CancellationToken cancellationToken)
    {
        var result = await _playlistService.GetPlaylistByIdAsync(playlistId, cancellationToken);
        return Ok(result);
    }
}