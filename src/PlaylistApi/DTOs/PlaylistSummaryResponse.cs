namespace PlaylistApi.DTOs;

/// <summary>
/// A lightweight playlist representation used for list endpoints,
/// so we don't send every song's full details when listing many playlists.
/// </summary>
public class PlaylistSummaryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SongCount { get; set; }
    public DateTime CreatedAt { get; set; }
}