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
}