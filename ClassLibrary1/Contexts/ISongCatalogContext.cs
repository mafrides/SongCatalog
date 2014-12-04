using System;
using System.Data.Entity;
using System.Threading.Tasks;
using CDCatalogModel;

namespace CDCatalogDAL
{
    public interface ISongCatalogContext : IDisposable
    {
        DbSet<Album> Albums { get; }
        DbSet<Artist> Artists { get; }
        DbSet<Genre> Genres { get; }
        DbSet<Song> Songs { get; }

        int SaveChanges();
        Task<int> SaveChangesAsync();

        void SetModified(object entity);
        void SetAdded(object entity);
        void SetDeleted(object entity);
    }
}
