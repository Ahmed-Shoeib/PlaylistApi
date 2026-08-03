using System.ComponentModel.DataAnnotations;

namespace PlaylistApi.DTOs;

public class CreateSongRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Artist { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Album { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "DurationInSeconds must be a positive number.")]
    public int? DurationInSeconds { get; set; }
}