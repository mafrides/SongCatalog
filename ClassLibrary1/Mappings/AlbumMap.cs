using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using CDCatalogModel;

namespace CDCatalogDAL
{
    public class AlbumMap : EntityTypeConfiguration<Album>
    {
        public AlbumMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("albums");
            this.Property(t => t.Id).HasColumnName("id");
            this.Property(t => t.Title).HasColumnName("title");
            this.Property(t => t.Year).HasColumnName("year");
            this.Property(t => t.Rating).HasColumnName("rating");
            this.Property(t => t.ArtistId).HasColumnName("artist_id");
            this.Property(t => t.GenreId).HasColumnName("genre_id");

            // Relationships
            this.HasRequired(t => t.Artist)
                .WithMany(t => t.Albums)
                .HasForeignKey(d => d.ArtistId);
            this.HasRequired(t => t.Genre)
                .WithMany(t => t.Albums)
                .HasForeignKey(d => d.GenreId);

            //not mapped
            this.Ignore(t => t.DisplayTitle);
            this.Ignore(t => t.DisplayTrackLength);
            this.Ignore(t => t.IsValid);
        }
    }
}
