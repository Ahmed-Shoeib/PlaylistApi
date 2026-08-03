using Microsoft.AspNetCore.Mvc;
using PlaylistApi.DTOs;
using PlaylistApi.Services;

namespace PlaylistApi.Controllers;

[ApiController]
public class SongsController : ControllerBase
{
    private readonly ISongService _songService;

    public SongsController(ISongService songService)
    {
        _songService = songService;
    }

    /// <summary>
    /// Adds a new song to the specified playlist.
    /// </summary>
    [HttpPost("api/playlists/{playlistId:int}/songs")]
    [ProducesResponseType(typeof(SongResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SongResponse>> AddSong(
        int playlistId,
        [FromBody] CreateSongRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _songService.AddSongToPlaylistAsync(playlistId, request, cancellationToken);

        return CreatedAtAction(
            actionName: nameof(PlaylistsController.GetPlaylistById),
            controllerName: "Playlists",
            new { playlistId = result.PlaylistId },
            result);
    }

    /// <summary>
    /// Updates an existing song within a playlist.
    /// </summary>
    [HttpPut("api/playlists/{playlistId:int}/songs/{songId:int}")]
    [ProducesResponseType(typeof(SongResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SongResponse>> UpdateSong(
        int playlistId,
        int songId,
        [FromBody] UpdateSongRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _songService.UpdateSongAsync(playlistId, songId, request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Removes a song from a playlist.
    /// </summary>
    [HttpDelete("api/playlists/{playlistId:int}/songs/{songId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSong(
        int playlistId,
        int songId,
        CancellationToken cancellationToken)
    {
        await _songService.DeleteSongAsync(playlistId, songId, cancellationToken);
        return NoContent();
    }
}