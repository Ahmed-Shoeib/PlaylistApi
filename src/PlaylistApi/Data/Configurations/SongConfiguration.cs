using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlaylistApi.Models;

namespace PlaylistApi.Data.Configurations;

public class SongConfiguration : IEntityTypeConfiguration<Song>
{
    public void Configure(EntityTypeBuilder<Song> builder)
    {
        builder.ToTable("Songs");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Artist)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Album)
            .HasMaxLength(200);

        // Speeds up "get all songs for playlist X" queries
        builder.HasIndex(s => s.PlaylistId);
    }
}