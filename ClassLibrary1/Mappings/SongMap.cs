using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using CDCatalogModel;

namespace CDCatalogDAL
{
    public class songMap : EntityTypeConfiguration<Song>
    {
        public songMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Url)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("songs");
            this.Property(t => t.Id).HasColumnName("id");
            this.Property(t => t.Title).HasColumnName("title");
            this.Property(t => t.TrackLength).HasColumnName("track_length");
            this.Property(t => t.Rating).HasColumnName("rating");
            this.Property(t => t.ArtistId).HasColumnName("artist_id");
            this.Property(t => t.GenreId).HasColumnName("genre_id");
            this.Property(t => t.AlbumId).HasColumnName("album_id");
            this.Property(t => t.TrackNumber).HasColumnName("track_number");
            this.Property(t => t.Url).HasColumnName("url");

            // Relationships
            this.HasOptional(t => t.Album)
                .WithMany(t => t.Songs)
                .HasForeignKey(d => d.AlbumId);
            this.HasRequired(t => t.Artist)
                .WithMany(t => t.Songs)
                .HasForeignKey(d => d.ArtistId);
            this.HasRequired(t => t.Genre)
                .WithMany(t => t.Songs)
                .HasForeignKey(d => d.GenreId);

            //not mapped
            this.Ignore(t => t.DisplayTitle);
            this.Ignore(t => t.DisplayTrackLength);
            this.Ignore(t => t.IsValid);
        }
    }
}
