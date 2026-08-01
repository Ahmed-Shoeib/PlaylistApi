using System.ComponentModel.DataAnnotations;

namespace PlaylistApi.Models;

public class Song
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Artist { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Album { get; set; }

    public int? DurationInSeconds { get; set; }

    public int PlaylistId { get; set; }

    // Navigation: the Playlist this song belongs to
    public Playlist Playlist { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}