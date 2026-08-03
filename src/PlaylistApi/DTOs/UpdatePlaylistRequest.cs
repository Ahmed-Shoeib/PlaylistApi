using System.ComponentModel.DataAnnotations;

namespace PlaylistApi.DTOs;

public class UpdatePlaylistRequest
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}