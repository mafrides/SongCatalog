using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using CDCatalogModel;

namespace CDCatalogDAL
{
    public partial class SongCatalogContext : DbContext, ISongCatalogContext
    {
        static SongCatalogContext()
        {
            Database.SetInitializer<SongCatalogContext>(null);
        }

        public SongCatalogContext()
            : base("Name=SongCatalogContext")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.UseDatabaseNullSemantics = false;
        }
        
        public DbSet<Album> Albums { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Song> Songs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AlbumMap());
            modelBuilder.Configurations.Add(new ArtistMap());
            modelBuilder.Configurations.Add(new GenreMap());
            modelBuilder.Configurations.Add(new songMap());
        }


        public void SetModified(object entity)
        {
            this.Entry(entity).State = EntityState.Modified;
        }

        public void SetAdded(object entity)
        {
            this.Entry(entity).State = EntityState.Added;
        }

        public void SetDeleted(object entity)
        {
            this.Entry(entity).State = EntityState.Deleted;
        }
    }
}
