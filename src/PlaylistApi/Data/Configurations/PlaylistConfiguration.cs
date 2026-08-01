using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlaylistApi.Models;

namespace PlaylistApi.Data.Configurations;

public class PlaylistConfiguration : IEntityTypeConfiguration<Playlist>
{
    public void Configure(EntityTypeBuilder<Playlist> builder)
    {
        builder.ToTable("Playlists");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        // Speeds up "get all playlists for user X" queries
        builder.HasIndex(p => p.UserId);

        builder.HasMany(p => p.Songs)
            .WithOne(s => s.Playlist)
            .HasForeignKey(s => s.PlaylistId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}