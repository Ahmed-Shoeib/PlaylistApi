using System.ComponentModel.DataAnnotations;

namespace PlaylistApi.Models;

public class Playlist
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public int UserId { get; set; }

    // Navigation: the User this playlist belongs to
    public User User { get; set; } = null!;

    // Navigation: the songs in this playlist
    public ICollection<Song> Songs { get; set; } = new List<Song>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}