using System;
using System.Collections.Generic;
using System.Linq;
using CDCatalogModel;
using CDCatalogDAL;

namespace CDCatalogAPI
{
    public static class CDCatalog
    {
        #region Setting Repository is Required

        public static ICDCatalogRepository Repository { get; set; }

        #endregion

        #region Song and Album Extension Methods

        //Insert or Update
        public static Song save(this Song song)
        {
            return song.IsValid ? Repository.saveSong(song) : null;
        }
        public static Album save(this Album album)
        {
            foreach(var song in album.Songs)
            {
                if (!song.IsValid) return null;
            }
            return album.IsValid ? Repository.saveAlbum(album) : null;
        }

        public static bool remove(this Song song)
        {
            return song.Id <= 0 ? false : Repository.removeSong(song);
        }
        public static bool remove(this Album album)
        {
            return album.Id <= 0 ? false : Repository.removeAlbum(album);
        }
        public static bool removeWithSongs(this Album album)
        {
            return album.Id <= 0 ? false : Repository.removeAlbumWithSongs(album);
        }

        //Ratings 1-10
        public static Song rate(this Song song, int rating)
        {
            rating = rating > 10 ? 10 : rating < 1 ? 1 : rating;
            if (!song.IsValid) return null;
            song.Rating = rating;
            return Repository.saveSong(song);
        }
        public static Song unrate(this Song song)
        {
            if (!song.IsValid) return null;
            song.Rating = null;
            return Repository.saveSong(song);
        }
        public static Album rate(this Album album, int rating)
        {
            rating = rating > 10 ? 10 : rating < 1 ? 1 : rating;
            if (!album.IsValid) return null;
            album.Rating = rating;
            return Repository.saveAlbum(album);
        }
        public static Album unrate(this Album album)
        {
            if (!album.IsValid) return null;
            album.Rating = null;
            return Repository.saveAlbum(album);
        }

        #endregion

        #region Catalog Static Methods

        public static List<Song> findSongs(string title, int skip = 0, int take = 300)
        {
            skip = skip < 0 ? 0 : skip;
            take = take < 0 ? 0 : take;
            return String.IsNullOrEmpty(title) ? new List<Song>() :
                Repository.getSongs().Where(s => s.Title.ToLower().Contains(title.ToLower()))
                                     .Skip(skip).Take(take).ToList();
        }
        public static List<Album> findAlbums(string title, int skip = 0, int take = 300)
        {
            skip = skip < 0 ? 0 : skip == Int32.MaxValue ? skip - 1 : skip;
            take = take < 0 ? 0 : take;
            return String.IsNullOrEmpty(title) ? new List<Album>() :
                Repository.getAlbumsWithSongs().Where(a => a.Title.ToLower().Contains(title.ToLower()))
                                               .Skip(skip).Take(take).ToList();
        }
        public static List<Song> findSongs(Artist artist, int skip = 0, int take = 300)
        {
            skip = skip < 0 ? 0 : skip == Int32.MaxValue ? skip - 1 : skip;
            take = take < 0 ? 0 : take;
            return !artist.IsValid ? new List<Song>() :
                Repository.getSongs().Where(s => s.Artist == artist)
                                     .Skip(skip).Take(take).ToList();
        }
        public static List<Album> findAlbums(Artist artist, int skip = 0, int take = 300)
        {
            skip = skip < 0 ? 0 : skip == Int32.MaxValue ? skip - 1 : skip;
            take = take < 0 ? 0 : take;
            return !artist.IsValid ? new List<Album>() :
                Repository.getAlbumsWithSongs().Where(a => a.Artist == artist)
                                               .Skip(skip).Take(take).ToList();
        }
        public static List<Song> findSongs(Genre genre, int skip = 0, int take = 300)
        {
            skip = skip < 0 ? 0 : skip == Int32.MaxValue ? skip - 1 : skip;
            take = take < 0 ? 0 : take;
            return !genre.IsValid ? new List<Song>() :
                Repository.getSongs().Where(s => s.Genre == genre)
                                     .Skip(skip).Take(take).ToList();
        }
        public static List<Album> findAlbums(Genre genre, int skip = 0, int take = 300)
        {
            skip = skip < 0 ? 0 : skip == Int32.MaxValue ? skip - 1 : skip;
            take = take < 0 ? 0 : take;
            return !genre.IsValid ? new List<Album>() :
                Repository.getAlbumsWithSongs().Where(a => a.Genre == genre)
                                               .Skip(skip).Take(take).ToList();
        }

        public static List<Song> getSongs(int skip = 0, int take = 300)
        {
            return Repository.getSongs().Skip(skip).Take(take).ToList();
        }
        public static List<Album> getAlbums(int skip = 0, int take = 300)
        {
            return Repository.getAlbums().Skip(skip).Take(take).ToList();
        }
        public static List<Artist> getArtists(int skip = 0, int take = 300)
        {
            return Repository.getArtists().Skip(skip).Take(take).ToList();
        }
        public static List<Genre> getGenres(int skip = 0, int take = 300)
        {
            return Repository.getGenres().Skip(skip).Take(take).ToList();
        }

        public static int SongCount
        {
            get 
            {
                return Repository.getSongs().Count();
            }
        }
        public static int AlbumCount
        {
            get 
            {
                return Repository.getAlbums().Count();
            }
        }
        public static int ArtistCount
        {
            get
            {
                return Repository.getArtists().Count();
            }
        }
        public static int GenreCount
        {
            get
            {
                return Repository.getGenres().Count();
            }
        }

        public static List<Song> createPlaylist(int minutes)
        {
            throw new NotImplementedException();
        }

        public static void removeAlbumsWithoutSongs()
        {     
            Repository.removeAlbumsWithoutSongs();
        }

        public static void removeArtistsWithoutSongs()
        {
            Repository.removeArtistsWithoutSongs();
        }

        public static void removeGenresWithoutSongs()
        {
            Repository.removeGenresWithoutSongs();
        }

        #endregion
    }
}
