using System.Threading.Tasks;
using System.Collections.Generic;
using CDCatalogModel;

namespace CDCatalogDAL
{
    public interface ICDCatalog
    {
        #region Count

        Task<int> getSongCountAsync();
        Task<int> getAlbumCountAsync();
        Task<int> getArtistCountAsync();
        Task<int> getGenreCountAsync();

        #endregion

        #region Create

        Task<Song> insertSongAsync(Song song);
        Task<Album> insertAlbumAsync(Album album);

        #endregion

        #region Read

        Task<List<Song>> createPlaylistAsync(int minutes);

        Task<List<Song>> getSongsAsync(int skip = 0, int take = 300);
        Task<List<Album>> getAlbumsWithSongsAsync(int skip = 0, int take = 300);
        Task<List<IAlbumOrSong>> getAlbumsAndSongsAsync(int skip = 0, int take = 300);
        Task<List<Album>> getAlbumsAsync(int skip = 0, int take = 300);
        Task<List<Artist>> getArtistsAsync(int skip = 0, int take = 300);
        Task<List<Genre>> getGenresAsync(int skip = 0, int take = 300);

        Task<Song> findSongAsync(int Id);
        Task<Album> findAlbumAsync(int Id);
        Task<Artist> findArtistAsync(int Id);
        Task<Genre> findGenreAsync(int Id);

        Task<List<Song>> findSongsAsync(string title, int skip = 0, int take = 300);
        Task<List<Album>> findAlbumsAsync(string title, int skip = 0, int take = 300);
        Task<List<IAlbumOrSong>> findAlbumsAndSongsAsync(string title, int skip = 0, int take = 300);

        Task<List<Song>> findSongsAsync(Artist artist, int skip = 0, int take = 300);
        Task<List<Album>> findAlbumsAsync(Artist artist, int skip = 0, int take = 300);
        Task<List<IAlbumOrSong>> findAlbumsAndSongsAsync(Artist artist, int skip = 0, int take = 300);

        Task<List<Song>> findSongsAsync(Genre genre, int skip = 0, int take = 300);
        Task<List<Album>> findAlbumsAsync(Genre genre, int skip = 0, int take = 300);
        Task<List<IAlbumOrSong>> findAlbumsAndSongsAsync(Genre genre, int skip = 0, int take = 300);

        #endregion

        #region Update

        Task<Song> updateSongAsync(Song song);
        Task<Album> updateAlbumAsync(Album album);

        #endregion

        #region Delete

        Task removeSongAsync(Song song);
        Task removeSongsAsync(List<Song> songs);
        Task removeAlbumAsync(Album album);
        Task removeAlbumWithSongsAsync(Album album);
        Task removeAlbumsWithSongsAsync(List<Album> albums);

        Task removeAlbumsWithoutSongsAsync();
        Task removeArtistsWithoutSongsAsync();
        Task removeGenresWithoutSongsAsync();

        #endregion
    }
}
