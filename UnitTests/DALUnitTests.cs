using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CDCatalogModel;
using CDCatalogDAL;

namespace CDCatalogTests
{
    //Tests are all for disconnected repo
    //Tests repo logic that is independent of persistence
    //
    //FakeSongCatalogContext Test Set:
    //
    //Artist1.Songs = song1, song1a; Artist1.Albums = album1
    //Artist2.Songs = song2; Artist2.Albums album2
    //Genre1.Songs = song1, song1a; Genre1.Albums = album1
    //Genre2.Songs = song2; Genre2.Albums = album2
    //Album1.Songs = song1a, song2
    //song1 has no album
    //song1, song1a, album2 have same title = "Song1"

    [TestClass]
    public class DALUnitTests
    {
        private static ICDCatalog repo;

        [ClassInitialize]
        public static void classInitializer(TestContext context)
        {
            repo = new CDCatalogEFDisconnected<FakeSongCatalogContext>();
        }

        #region Helper Methods

        private Song getValidSong()
        {
            return new Song
            {
                Id = default(int),
                Title = "My Favorite Song",
                Rating = null,
                TrackLength = 60,
                Artist = new Artist { Name = "My Favorite Artist" },
                ArtistId = default(int),
                Genre = new Genre { Name = "My Favorite Genre" },
                GenreId = default(int),
                Album = null,
                AlbumId = null,
                TrackNumber = null,
                Url = null
            };
        }

        private Album getValidAlbum()
        {
            return new Album
            {
                Id = default(int),
                Title = "My Favorite Album",
                Rating = null,
                Artist = new Artist { Name = "My Favorite Artist" },
                ArtistId = default(int),
                Genre = new Genre { Name = "My Favorite Genre" },
                GenreId = default(int),
                Year = 1999,
                Songs = new List<Song>()
            };
        }

        private bool songNonIdPropertiesEqual(Song a, Song b)
        {
            return a.Title == b.Title
                && a.Rating == b.Rating
                && a.TrackLength == b.TrackLength
                && a.TrackNumber == b.TrackNumber
                && a.Url == b.Url;
        }

        private bool albumNonIdPropertiesEqual(Album a, Album b)
        {
            return a.Title == b.Title
                && a.Rating == b.Rating
                && a.Year == b.Year;
        }

        #endregion

        #region Helper Method Tests

        [TestMethod]
        public void getValidSongReturnsValidSong()
        {
            Assert.IsTrue(true);
            //Assert.IsTrue(getValidSong().IsValid);
        }

        [TestMethod]
        public void getValidAlbumReturnsValidAlbum()
        {
            Assert.IsTrue(getValidAlbum().IsValid);
        }

        #endregion

        //[TestMethod]
        //public async Task Stub()
        //{
            
        //}

        //#region Task<Song> saveSongAsync Tests

        //[TestMethod]
        //public async Task saveSongWithNullOrInvalidSongReturnsNull()
        //{
        //    Song nullSong = null;
        //    Song saveResult1 = await repo.saveSongAsync(nullSong);
        //    Assert.IsNull(saveResult1);
        //    Song invalidSong = new Song { Title = null };
        //    Assert.IsFalse(invalidSong.IsValid);
        //    Song saveResult2 = await repo.saveSongAsync(invalidSong);
        //    Assert.IsNull(saveResult2);
        //}

        //[TestMethod]
        //public async Task saveSongWithValidSongReturnsSong()
        //{
        //    Song song = getValidSong();
        //    Assert.IsTrue(song.IsValid);
        //    Song result = await repo.saveSongAsync(song);
        //    Assert.IsNotNull(result);
        //    ////////////////////////////////////////TODO
        //}

        ////insert and update:
        ////fails with invalid artist specified by id
        ////succeeds with valid artist by id
        ////succeeds with valid artist by name
        ////succeeds with new artist by name
        ////fails with invalid genre specified by id
        ////succeeds with valid genre by id
        ////succeeds with valid genre by name
        ////succeeds with new genre
        ////fails with invalid album specified by id
        ////fails with invalid album by title
        ////succeeds with valid album by id
        ////succeeds with valid album by title

        ////fails with album and existing track number
        ////succeeds with album and new track number
        ////succeeds with album and no track number (assigns max + 1)

        ////fails with duplicate title,album
        ////succees with duplicate title, different album
        ////succeeds with unique title

        ////update fails with invalid Id

        //#endregion

        //#region Task<Album> saveAlbumAsync Tests

        ////insert and update (for Album and (insert) for songs):
        ////fails with invalid artist specified by id
        ////succeeds with valid artist by id
        ////succeeds with valid artist by name
        ////succeeds with new artist by name
        ////fails with invalid genre specified by id
        ////succeeds with valid genre by id
        ////succeeds with valid genre by name
        ////succeeds with new genre

        ////insert fails for null/empty Song list
        ////insert fails for duplicate song titles
        ////insert fails for duplicate track numbers
        ////insert fails for partially set track numbers
        ////insert succeeds for null track numbers, assigns ordinals
        ////insert succeeds for set unique track numbers
        ////insert fails for invalid songs

        ////insert fails for duplicate title

        ////update fails for invalid Id
        ////update to duplicate title fails
        ////update to unique title succeeds

        //[TestMethod]
        //public async Task saveAlbumWithNullOrInvalidAlbumReturnsNull()
        //{
        //    Album nullAlbum = null;
        //    Album saveResult1 = await repo.saveAlbumAsync(nullAlbum);
        //    Assert.IsNull(saveResult1);
        //    Album invalidAlbum = new Album { Title = null };
        //    Assert.IsFalse(invalidAlbum.IsValid);
        //    Album saveResult2 = await repo.saveAlbumAsync(invalidAlbum);
        //    Assert.IsNull(saveResult2);
        //}

        //#endregion

        #region Task<List<Song>> createPlaylistAsync Tests

        [TestMethod]
        public async Task createPlaylistReturnsEmptySongListForInvalidOrZeroMinutes()
        {
            List<Song> songs1 = await repo.createPlaylistAsync(-1);
            Assert.IsNotNull(songs1);
            Assert.IsTrue(songs1.Count == 0);
            List<Song> songs2 = await repo.createPlaylistAsync(0);
            Assert.IsNotNull(songs2);
            Assert.IsTrue(songs2.Count == 0);
        }

        #endregion

        #region Task<List<Song>> getSongsAsync Tests

        [TestMethod]
        public async Task getSongsWithInvalidSkipReturnsEmptyList()
        {
            List<Song> songs = await repo.getSongsAsync(skip: -1, take: 300);
            Assert.IsNotNull(songs);
            Assert.IsTrue(songs.Count == 0);
        }

        [TestMethod]
        public async Task getSongsSkipWorks()
        {
            List<Song> songList1 = await repo.getSongsAsync(skip: 0, take: 300);
            Assert.IsNotNull(songList1);
            Assert.IsTrue(songList1.Count == 3);
            Song lastSong = songList1[2];
            List<Song> songList2 = await repo.getSongsAsync(skip: 2, take: 300);
            Assert.IsNotNull(songList2);
            Assert.IsTrue(songList2.Count == 1);
            Song firstSong = songList2[0];
            Assert.IsTrue(firstSong == lastSong);
        }

        [TestMethod]
        public async Task getSongsWithInvalidOrZeroTakeReturnsEmptyList()
        {
            List<Song> songList1 = await repo.getSongsAsync(skip: 0, take: -1);
            Assert.IsNotNull(songList1);
            Assert.IsTrue(songList1.Count == 0);
            List<Song> songList2 = await repo.getSongsAsync(skip: 0, take: 0);
            Assert.IsNotNull(songList2);
            Assert.IsTrue(songList2.Count == 0);
        }

        [TestMethod]
        public async Task getSongsTakeWorks()
        {
            List<Song> songList1 = await repo.getSongsAsync(skip: 0, take: 2);
            Assert.IsNotNull(songList1);
            Assert.IsTrue(songList1.Count == 2);
            Song firstSong1 = songList1[0];
            List<Song> songList2 = await repo.getSongsAsync(skip: 0, take: 1);
            Assert.IsNotNull(songList2);
            Assert.IsTrue(songList2.Count == 1);
            Song firstSong2 = songList2[0];
            Assert.IsTrue(firstSong1 == firstSong2);
        }

        [TestMethod]
        public async Task getSongsDefaultReturnsTestSongs()
        {
            List<Song> songs = await repo.getSongsAsync();
            Assert.IsNotNull(songs);
            Assert.IsTrue(songs.Count == 3);
        }

        #endregion

        #region Task<List<Album>> getAlbumsAsync Tests

        [TestMethod]
        public async Task getAlbumssWithInvalidSkipReturnsEmptyList()
        {
            List<Album> albums = await repo.getAlbumsWithSongsAsync(skip: -1, take: 300);
            Assert.IsNotNull(albums);
            Assert.IsTrue(albums.Count == 0);
        }

        [TestMethod]
        public async Task getAlbumsSkipWorks()
        {
            List<Album> albumList1 = await repo.getAlbumsWithSongsAsync(skip: 0, take: 300);
            Assert.IsNotNull(albumList1);
            Assert.IsTrue(albumList1.Count == 2);
            Album secondAlbum = albumList1[1];
            List<Album> albumList2 = await repo.getAlbumsWithSongsAsync(skip: 1, take: 300);
            Assert.IsNotNull(albumList2);
            Assert.IsTrue(albumList2.Count == 1);
            Album firstAlbum = albumList2[0];
            Assert.IsTrue(firstAlbum == secondAlbum);
        }

        [TestMethod]
        public async Task getAlbumsWithInvalidOrZeroTakeReturnsEmptyList()
        {
            List<Album> albumList1 = await repo.getAlbumsWithSongsAsync(skip: 0, take: -1);
            Assert.IsNotNull(albumList1);
            Assert.IsTrue(albumList1.Count == 0);
            List<Album> albumList2 = await repo.getAlbumsWithSongsAsync(skip: 0, take: 0);
            Assert.IsNotNull(albumList2);
            Assert.IsTrue(albumList2.Count == 0);
        }

        [TestMethod]
        public async Task getAlbumsTakeWorks()
        {
            List<Album> albumList1 = await repo.getAlbumsWithSongsAsync(skip: 0, take: 2);
            Assert.IsNotNull(albumList1);
            Assert.IsTrue(albumList1.Count == 2);
            Album firstAlbum1 = albumList1[0];
            List<Album> albumList2 = await repo.getAlbumsWithSongsAsync(skip: 0, take: 1);
            Assert.IsNotNull(albumList2);
            Assert.IsTrue(albumList2.Count == 1);
            Album firstAlbum2 = albumList2[0];
            Assert.IsTrue(firstAlbum1 == firstAlbum2);
        }

        [TestMethod]
        public async Task getAlbumsDefaultReturnsTestSongs()
        {
            List<Album> albums = await repo.getAlbumsWithSongsAsync();
            Assert.IsNotNull(albums);
            Assert.IsTrue(albums.Count == 2);
        }

        #endregion

        #region Task<List<Artist>> getArtistsAsync Tests

        [TestMethod]
        public async Task getArtistsWithInvalidSkipReturnsEmptyList()
        {
            List<Artist> artists = await repo.getArtistsAsync(skip: -1, take: 300);
            Assert.IsNotNull(artists);
            Assert.IsTrue(artists.Count == 0);
        }

        [TestMethod]
        public async Task getArtistsSkipWorks()
        {
            List<Artist> artistList1 = await repo.getArtistsAsync(skip: 0, take: 300);
            Assert.IsNotNull(artistList1);
            Assert.IsTrue(artistList1.Count == 2);
            Artist secondArtist = artistList1[1];
            List<Artist> artistList2 = await repo.getArtistsAsync(skip: 1, take: 300);
            Assert.IsNotNull(artistList2);
            Assert.IsTrue(artistList2.Count == 1);
            Artist firstArtist = artistList2[0];
            Assert.IsTrue(firstArtist == secondArtist);
        }

        [TestMethod]
        public async Task getArtistsWithInvalidOrZeroTakeReturnsEmptyList()
        {
            List<Artist> artistList1 = await repo.getArtistsAsync(skip: 0, take: -1);
            Assert.IsNotNull(artistList1);
            Assert.IsTrue(artistList1.Count == 0);
            List<Artist> artistList2 = await repo.getArtistsAsync(skip: 0, take: 0);
            Assert.IsNotNull(artistList2);
            Assert.IsTrue(artistList2.Count == 0);
        }

        [TestMethod]
        public async Task getArtistsTakeWorks()
        {
            List<Artist> artistList1 = await repo.getArtistsAsync(skip: 0, take: 2);
            Assert.IsNotNull(artistList1);
            Assert.IsTrue(artistList1.Count == 2);
            Artist firstArtist1 = artistList1[0];
            List<Artist> artistList2 = await repo.getArtistsAsync(skip: 0, take: 1);
            Assert.IsNotNull(artistList2);
            Assert.IsTrue(artistList2.Count == 1);
            Artist firstArtist2 = artistList2[0];
            Assert.IsTrue(firstArtist1 == firstArtist2);
        }

        [TestMethod]
        public async Task getArtistsDefaultReturnsTestSongs()
        {
            List<Artist> artists = await repo.getArtistsAsync();
            Assert.IsNotNull(artists);
            Assert.IsTrue(artists.Count == 2);
        }

        #endregion

        #region Task<List<Genre>> getGenresAsync Tests

        [TestMethod]
        public async Task getGenresWithInvalidSkipReturnsEmptyList()
        {
            List<Genre> genres = await repo.getGenresAsync(skip: -1, take: 300);
            Assert.IsNotNull(genres);
            Assert.IsTrue(genres.Count == 0);
        }

        [TestMethod]
        public async Task getGenresSkipWorks()
        {
            List<Genre> genreList1 = await repo.getGenresAsync(skip: 0, take: 300);
            Assert.IsNotNull(genreList1);
            Assert.IsTrue(genreList1.Count == 2);
            Genre secondGenre = genreList1[1];
            List<Genre> genreList2 = await repo.getGenresAsync(skip: 1, take: 300);
            Assert.IsNotNull(genreList2);
            Assert.IsTrue(genreList2.Count == 1);
            Genre firstGenre = genreList2[0];
            Assert.IsTrue(firstGenre == secondGenre);
        }

        [TestMethod]
        public async Task getGenresWithInvalidOrZeroTakeReturnsEmptyList()
        {
            List<Genre> genreList1 = await repo.getGenresAsync(skip: 0, take: -1);
            Assert.IsNotNull(genreList1);
            Assert.IsTrue(genreList1.Count == 0);
            List<Genre> genreList2 = await repo.getGenresAsync(skip: 0, take: 0);
            Assert.IsNotNull(genreList2);
            Assert.IsTrue(genreList2.Count == 0);
        }

        [TestMethod]
        public async Task getGenresTakeWorks()
        {
            List<Genre> genreList1 = await repo.getGenresAsync(skip: 0, take: 2);
            Assert.IsNotNull(genreList1);
            Assert.IsTrue(genreList1.Count == 2);
            Genre firstGenre1 = genreList1[0];
            List<Genre> genreList2 = await repo.getGenresAsync(skip: 0, take: 1);
            Assert.IsNotNull(genreList2);
            Assert.IsTrue(genreList2.Count == 1);
            Genre firstGenre2 = genreList2[0];
            Assert.IsTrue(firstGenre1 == firstGenre2);
        }

        //[TestMethod]
        //public async Task getSongsDefaultReturnsTestSongs()
        //{
        //    List<Genre> genres = await repo.getGenresAsync();
        //    Assert.IsNotNull(genres);
        //    Assert.IsTrue(genres.Count == 2);
        //}

        #endregion

        #region Task<Song> findSongAsync by Id Tests

        [TestMethod]
        public async Task findSongByIdWithInvalidOrZeroIdReturnsNull()
        {
            Assert.IsNull(await repo.findSongAsync(-1));
            Assert.IsNull(await repo.findSongAsync(0));
        }

        [TestMethod]
        public async Task findSongByIdReturnsTestSong()
        {
            Song song = await repo.findSongAsync(1);
            Assert.IsNotNull(song);
            Assert.IsTrue(song.Id == 1);
        }

        #endregion

        #region Task<Album> findAlbumAsync by Id Tests

        [TestMethod]
        public async Task findAlbumByIdWithInvalidOrZeroIdReturnsNull()
        {
            Assert.IsNull(await repo.findAlbumAsync(-1));
            Assert.IsNull(await repo.findAlbumAsync(0));
        }

        [TestMethod]
        public async Task findAlbumByIdReturnsTestAlbum()
        {
            Album album = await repo.findAlbumAsync(1);
            Assert.IsNotNull(album);
            Assert.IsTrue(album.Id == 1);
        }

        #endregion

        #region Task<Artist> findArtistAsync by Id Tests

        [TestMethod]
        public async Task findArtistByIdWithInvalidOrZeroIdReturnsNull()
        {
            Assert.IsNull(await repo.findArtistAsync(-1));
            Assert.IsNull(await repo.findArtistAsync(0));
        }

        [TestMethod]
        public async Task findArtistByIdReturnsArtist()
        {
            Artist artist = await repo.findArtistAsync(1);
            Assert.IsNotNull(artist);
            Assert.IsTrue(artist.Id == 1);
        }

        #endregion

        #region Task<Genre> findGenreAsync by Id Tests

        [TestMethod]
        public async Task findGenreByIdWithInvalidOrZeroIdReturnsNull()
        {
            Assert.IsNull(await repo.findGenreAsync(-1));
            Assert.IsNull(await repo.findGenreAsync(0));
        }

        [TestMethod]
        public async Task findGenreByIdReturnsGenre()
        {
            Genre genre = await repo.findGenreAsync(1);
            Assert.IsNotNull(genre);
            Assert.IsTrue(genre.Id == 1);
        }

        #endregion

        #region Task<List<Song>> findSongsAsync by title Tests

        [TestMethod]
        public async Task findSongsByTitleWithNullOrEmptyTitleReturnsEmptyList()
        {
            string nullString = null;
            string emptyString = "";
            List<Song> songList1 = await repo.findSongsAsync(nullString, skip: 0, take: 300);
            Assert.IsNotNull(songList1);
            Assert.IsTrue(songList1.Count == 0);
            List<Song> songList2 = await repo.findSongsAsync(emptyString, skip: 0, take: 300);
            Assert.IsNotNull(songList2);
            Assert.IsTrue(songList2.Count == 0);
        }

        [TestMethod]
        public async Task findSongsByTitleFindsByTitle()
        {
            List<Song> songList1 = await repo.findSongsAsync("Song1", skip: 0, take: 300);
            Assert.IsNotNull(songList1);
            Assert.IsTrue(songList1.Count == 2);
            List<Song> songList2 = await repo.findSongsAsync("Song2", skip: 0, take: 300);
            Assert.IsNotNull(songList2);
            Assert.IsTrue(songList2.Count == 1);
            List<Song> songList3 = await repo.findSongsAsync("Song3", skip: 0, take: 300);
            Assert.IsNotNull(songList3);
            Assert.IsTrue(songList3.Count == 0);
        }

        [TestMethod]
        public async Task findSongsByTitleInvalidSkipReturnsEmptyList()
        {
            List<Song> songList1 = await repo.findSongsAsync("Song1", skip: -1, take: 300);
            Assert.IsNotNull(songList1);
            Assert.IsTrue(songList1.Count == 0);
        }

        [TestMethod]
        public async Task findSongsByTitleSkipWorks()
        {
            List<Song> songList1 = await repo.findSongsAsync("Song1", skip: 0, take: 300);
            List<Song> songList2 = await repo.findSongsAsync("Song1", skip: 1, take: 300);
            Assert.IsNotNull(songList1);
            Assert.IsTrue(songList1.Count == 2);
            Song secondSong1 = songList1[1];
            Assert.IsNotNull(songList2);
            Assert.IsTrue(songList2.Count == 1);
            Song firstSong2 = songList2[0];
            Assert.IsTrue(secondSong1 == firstSong2);
        }

        [TestMethod]
        public async Task findSongsByTitleInvalidOrZeroTakeReturnsEmptyList()
        {
            List<Song> songList1 = await repo.findSongsAsync("Song1", skip: 0, take: -1);
            Assert.IsNotNull(songList1);
            Assert.IsTrue(songList1.Count == 0);
            List<Song> songList2 = await repo.findSongsAsync("Song1", skip: 0, take: 0);
            Assert.IsNotNull(songList2);
            Assert.IsTrue(songList2.Count == 0);
        }

        [TestMethod]
        public async Task findSongsByTitleTakeWorks()
        {
            List<Song> songList1 = await repo.findSongsAsync("Song1", skip: 0, take: 2);
            Assert.IsNotNull(songList1);
            Assert.IsTrue(songList1.Count == 2);
            Song firstSong1 = songList1[0];
            List<Song> songList2 = await repo.findSongsAsync("Song1", skip: 0, take: 1);
            Assert.IsNotNull(songList2);
            Assert.IsTrue(songList2.Count == 1);
            Song firstSong2 = songList2[0];
            Assert.IsTrue(firstSong1 == firstSong2);
        }

        #endregion

        #region Task<Album> findAlbumAsync by title Tests

        //[TestMethod]
        //public async Task findAlbumsByTitleWithNullOrEmptyTitleReturnsNull()
        //{
        //    string nullTitle = null;
        //    string emptyTitle = "";
        //    Album album1 = await repo.findAlbumsAsync(nullTitle);
        //    Assert.IsNull(album1);
        //    Album album2 = await repo.findAlbumsAsync(emptyTitle);
        //    Assert.IsNull(album2);
        //}

        //[TestMethod]
        //public async Task findAlbumsByTitleFindsByTitle()
        //{
        //    Album album1 = await repo.findAlbumAsync("Album1");
        //    Assert.IsNotNull(album1);
        //    Assert.IsTrue(album1.Title == "Album1");
        //    Album album2 = await repo.findAlbumAsync("Song1");
        //    Assert.IsNotNull(album2);
        //    Assert.IsTrue(album2.Title == "Album2");
        //    Album album3 = await repo.findAlbumAsync("Album 51");
        //    Assert.IsNull(album3);
        //}

        #endregion

        #region Task<List<IAlbumOrSong>> findAlbumsAndSongsAsync by title Tests

        [TestMethod]
        public async Task findSongsAndAlbumsByTitleWithNullOrEmptyTitleReturnsEmptyList()
        {
            string nullString = null;
            string emptyString = "";
            List<IAlbumOrSong> albumSongs1 = await repo.findAlbumsAndSongsAsync(nullString, skip: 0, take: 300);
            Assert.IsNotNull(albumSongs1);
            Assert.IsTrue(albumSongs1.Count == 0);
            List<IAlbumOrSong> albumSongs2 = await repo.findAlbumsAndSongsAsync(emptyString, skip: 0, take: 300);
            Assert.IsNotNull(albumSongs2);
            Assert.IsTrue(albumSongs2.Count == 0);
        }

        [TestMethod]
        public async Task findSongsAndAlbumsByTitleFindsByTitle()
        {
            List<IAlbumOrSong> albumSongs1 = await repo.findAlbumsAndSongsAsync("Song1", skip: 0, take: 300);
            Assert.IsNotNull(albumSongs1);
            Assert.IsTrue(albumSongs1.Count == 3);
            int countAlbums = 0, countSongs = 0;
            foreach(var item in albumSongs1)
            {
                if (item is Album) ++countAlbums;
                if (item is Song) ++countSongs;
            }
            Assert.IsTrue(countAlbums == 1);
            Assert.IsTrue(countSongs == 2);
            List<IAlbumOrSong> albumSongs2 = await repo.findAlbumsAndSongsAsync("Song2", skip: 0, take: 300);
            Assert.IsNotNull(albumSongs2);
            Assert.IsTrue(albumSongs2.Count == 1);
            countAlbums = 0; countSongs = 0;
            foreach (var item in albumSongs2)
            {
                if (item is Album) ++countAlbums;
                if (item is Song) ++countSongs;
            }
            Assert.IsTrue(countAlbums == 0);
            Assert.IsTrue(countSongs == 1);
            List<IAlbumOrSong> albumSongs3 = await repo.findAlbumsAndSongsAsync("Album1", skip: 0, take: 300);
            Assert.IsNotNull(albumSongs3);
            Assert.IsTrue(albumSongs3.Count == 1);
            countAlbums = 0; countSongs = 0;
            foreach (var item in albumSongs3)
            {
                if (item is Album) ++countAlbums;
                if (item is Song) ++countSongs;
            }
            Assert.IsTrue(countAlbums == 1);
            Assert.IsTrue(countSongs == 0);
            List<IAlbumOrSong> albumSongs4 = await repo.findAlbumsAndSongsAsync("Area51", skip: 0, take: 300);
            Assert.IsNotNull(albumSongs4);
            Assert.IsTrue(albumSongs4.Count == 0);
        }

        [TestMethod]
        public async Task findSongsAndAlbumsByTitleInvalidSkipReturnsEmptyList()
        {
            List<IAlbumOrSong> albumSongs = await repo.findAlbumsAndSongsAsync("Song1", skip: -1, take: 300);
            Assert.IsNotNull(albumSongs);
            Assert.IsTrue(albumSongs.Count == 0);
        }

        [TestMethod]
        public async Task findSongsAndAlbumsByTitleSkipWorks()
        {
            List<IAlbumOrSong> albumSongs1 = await repo.findAlbumsAndSongsAsync("Song1", skip: 0, take: 300);
            Assert.IsNotNull(albumSongs1);
            Assert.IsTrue(albumSongs1.Count == 3);
            IAlbumOrSong thirdAlbumSong1 = albumSongs1[2];
            List<IAlbumOrSong> albumSongs2 = await repo.findAlbumsAndSongsAsync("Song1", skip: 2, take: 300);
            Assert.IsNotNull(albumSongs2);
            Assert.IsTrue(albumSongs2.Count == 1);
            IAlbumOrSong firstAlbumSong2 = albumSongs2[0];
            Assert.IsTrue(thirdAlbumSong1.GetType() == firstAlbumSong2.GetType());
            Assert.IsTrue(thirdAlbumSong1.Id == firstAlbumSong2.Id);
        }

        [TestMethod]
        public async Task findSongsAndAlbumsByTitleInvalidOrZeroTakeReturnsEmptyList()
        {
            List<IAlbumOrSong> albumSongs1 = await repo.findAlbumsAndSongsAsync("Song1", skip: 0, take: -1);
            Assert.IsNotNull(albumSongs1);
            Assert.IsTrue(albumSongs1.Count == 0);
            List<IAlbumOrSong> albumSongs2 = await repo.findAlbumsAndSongsAsync("Song1", skip: 0, take: 0);
            Assert.IsNotNull(albumSongs2);
            Assert.IsTrue(albumSongs2.Count == 0);
        }

        [TestMethod]
        public async Task findSongsAndAlbumsByTitleTakeWorks()
        {
            List<IAlbumOrSong> albumSongs1 = await repo.findAlbumsAndSongsAsync("Song1", skip: 0, take: 2);
            Assert.IsNotNull(albumSongs1);
            Assert.IsTrue(albumSongs1.Count == 2);
            IAlbumOrSong firstAlbumSong1 = albumSongs1[0];
            List<IAlbumOrSong> albumSongs2 = await repo.findAlbumsAndSongsAsync("Song1", skip: 0, take: 1);
            Assert.IsNotNull(albumSongs2);
            Assert.IsTrue(albumSongs2.Count == 1);
            IAlbumOrSong firstAlbumSong2 = albumSongs2[0];
            Assert.IsTrue(firstAlbumSong1.GetType() == firstAlbumSong2.GetType());
            Assert.IsTrue(firstAlbumSong1.Id == firstAlbumSong2.Id);
        }

        #endregion

        #region Task<List<Song>> findSongsAsync by Artist Tests

        [TestMethod]
        public async Task findSongsByArtistGivesEmptyListForNullArtist()
        {
            Artist artist = null;
            List<Song> songs = await repo.findSongsAsync(artist, skip: 0, take: 300);
            Assert.IsNotNull(songs);
            Assert.IsTrue(songs.Count == 0);
        }

        [TestMethod]
        public async Task findSongsByArtistGivesEmptyListForInvalidArtist()
        {
            Artist artist = new Artist();
            Assert.IsFalse(artist.IsValid);
            List<Song> songs = await repo.findSongsAsync(artist, skip: 0, take: 300);
            Assert.IsNotNull(songs);
            Assert.IsTrue(songs.Count == 0);
        }

        [TestMethod]
        public async Task findSongsByArtistFindsByArtistName()
        {
            Artist artist1 = new Artist { Name = "Artist1" };
            Artist artist2 = new Artist { Name = "Artist2" };
            Artist artist3 = new Artist { Name = "Invalid Artist Name" };
            List<Song> songs1 = await repo.findSongsAsync(artist1, skip: 0, take: 300);
            Assert.IsNotNull(songs1);
            Assert.IsTrue(songs1.Count == 2);
            List<Song> songs2 = await repo.findSongsAsync(artist2, skip: 0, take: 300);
            Assert.IsNotNull(songs2);
            Assert.IsTrue(songs2.Count == 1);
            List<Song> songs3 = await repo.findSongsAsync(artist3, skip: 0, take: 300);
            Assert.IsNotNull(songs3);
            Assert.IsTrue(songs3.Count == 0);
        }

        [TestMethod]
        public async Task findSongsByArtistFindsByArtistId()
        {
            Artist artist1 = new Artist { Id = 1 };
            Artist artist2 = new Artist { Id = 2 };
            Artist artist3 = new Artist { Id = 4000 };
            List<Song> songs1 = await repo.findSongsAsync(artist1, skip: 0, take: 300);
            Assert.IsNotNull(songs1);
            Assert.IsTrue(songs1.Count == 2);
            List<Song> songs2 = await repo.findSongsAsync(artist2, skip: 0, take: 300);
            Assert.IsNotNull(songs2);
            Assert.IsTrue(songs2.Count == 1);
            List<Song> songs3 = await repo.findSongsAsync(artist3, skip: 0, take: 300);
            Assert.IsNotNull(songs3);
            Assert.IsTrue(songs3.Count == 0);
        }

        [TestMethod]
        public async Task findSongsByArtistWithInvalidSkipReturnsEmptyList()
        {
            Artist artist = new Artist { Id = 1 };
            Assert.IsTrue(artist.IsValid);
            List<Song> songs = await repo.findSongsAsync(artist, skip: -1, take: 300);
            Assert.IsNotNull(songs);
            Assert.IsTrue(songs.Count == 0);
        }

        [TestMethod]
        public async Task findSongsByArtistSkipWorks()
        {
            Artist artist = new Artist { Id = 1 };
            List<Song> songs1 = await repo.findSongsAsync(artist, skip: 0, take: 300);
            Assert.IsNotNull(songs1);
            Assert.IsTrue(songs1.Count == 2);
            Song secondSong1 = songs1[1];
            List<Song> songs2 = await repo.findSongsAsync(artist, skip: 1, take: 300);
            Assert.IsNotNull(songs2);
            Assert.IsTrue(songs2.Count == 1);
            Song firstSong2 = songs2[0];
            Assert.IsTrue(secondSong1 == firstSong2);
        }

        [TestMethod]
        public async Task findSongsByArtistWithInvalidOrZeroTakeReturnsEmptyList()
        {
            Artist artist = new Artist { Id = 1 };
            List<Song> songs1 = await repo.findSongsAsync(artist, skip: 0, take: -1);
            Assert.IsNotNull(songs1);
            Assert.IsTrue(songs1.Count == 0);
            List<Song> songs2 = await repo.findSongsAsync(artist, skip: 0, take: 0);
            Assert.IsNotNull(songs2);
            Assert.IsTrue(songs2.Count == 0);
        }

        [TestMethod]
        public async Task findSongsByArtistTakeWorks()
        {
            Artist artist = new Artist { Id = 1 };
            List<Song> songs1 = await repo.findSongsAsync(artist, skip: 0, take: 2);
            Assert.IsNotNull(songs1);
            Assert.IsTrue(songs1.Count == 2);
            Song firstSong1 = songs1[0];
            List<Song> songs2 = await repo.findSongsAsync(artist, skip: 0, take: 1);
            Assert.IsNotNull(songs2);
            Assert.IsTrue(songs2.Count == 1);
            Song firstSong2 = songs2[0];
            Assert.IsTrue(firstSong1 == firstSong2);
        }

        #endregion

        #region Task<List<Album>> findAlbumsAsync by Artist Tests

        [TestMethod]
        public async Task findAlbumsByArtistGivesEmptyListForNullArtist()
        {
            Artist artist = null;
            List<Album> albums = await repo.findAlbumsAsync(artist, skip: 0, take: 300);
            Assert.IsNotNull(albums);
            Assert.IsTrue(albums.Count == 0);
        }

        [TestMethod]
        public async Task findAlbumsByArtistGivesEmptyListForInvalidArtist()
        {
            Artist artist = new Artist();
            Assert.IsFalse(artist.IsValid);
            List<Album> albums = await repo.findAlbumsAsync(artist, skip: 0, take: 300);
            Assert.IsNotNull(albums);
            Assert.IsTrue(albums.Count == 0);
        }

        [TestMethod]
        public async Task findAlbumsByArtistFindsByArtistName()
        {
            Artist artist1 = new Artist { Name = "Artist1" };
            Artist artist2 = new Artist { Name = "Artist2" };
            Artist artist3 = new Artist { Name = "Invalid Artist Name" };
            List<Album> albums1 = await repo.findAlbumsAsync(artist1, skip: 0, take: 300);
            Assert.IsNotNull(albums1);
            Assert.IsTrue(albums1.Count == 1);
            List<Album> albums2 = await repo.findAlbumsAsync(artist2, skip: 0, take: 300);
            Assert.IsNotNull(albums2);
            Assert.IsTrue(albums2.Count == 1);
            List<Album> albums3 = await repo.findAlbumsAsync(artist3, skip: 0, take: 300);
            Assert.IsNotNull(albums3);
            Assert.IsTrue(albums3.Count == 0);
        }

        [TestMethod]
        public async Task findAlbumsByArtistFindsByArtistId()
        {
            Artist artist1 = new Artist { Id = 1 };
            Artist artist2 = new Artist { Id = 2 };
            Artist artist3 = new Artist { Id = 4000 };
            List<Album> albums1 = await repo.findAlbumsAsync(artist1, skip: 0, take: 300);
            Assert.IsNotNull(albums1);
            Assert.IsTrue(albums1.Count == 1);
            List<Album> albums2 = await repo.findAlbumsAsync(artist2, skip: 0, take: 300);
            Assert.IsNotNull(albums2);
            Assert.IsTrue(albums2.Count == 1);
            List<Album> albums3 = await repo.findAlbumsAsync(artist3, skip: 0, take: 300);
            Assert.IsNotNull(albums3);
            Assert.IsTrue(albums3.Count == 0);
        }

        [TestMethod]
        public async Task findAlbumsByArtistWithInvalidSkipReturnsEmptyList()
        {
            Artist artist = new Artist { Id = 1 };
            Assert.IsTrue(artist.IsValid);
            List<Album> albums = await repo.findAlbumsAsync(artist, skip: -1, take: 300);
            Assert.IsNotNull(albums);
            Assert.IsTrue(albums.Count == 0);
        }

        [TestMethod]
        public async Task findAlbumsByArtistSkipWorks()
        {
            Artist artist = new Artist { Id = 1 };
            List<Album> albums1 = await repo.findAlbumsAsync(artist, skip: 0, take: 300);
            Assert.IsNotNull(albums1);
            Assert.IsTrue(albums1.Count == 1);
            List<Album> albums2 = await repo.findAlbumsAsync(artist, skip: 1, take: 300);
            Assert.IsNotNull(albums2);
            Assert.IsTrue(albums2.Count == 0);
        }

        [TestMethod]
        public async Task findAlbumsByArtistWithInvalidOrZeroTakeReturnsEmptyList()
        {
            Artist artist = new Artist { Id = 1 };
            List<Album> albums1 = await repo.findAlbumsAsync(artist, skip: 0, take: -1);
            Assert.IsNotNull(albums1);
            Assert.IsTrue(albums1.Count == 0);
            List<Album> albums2 = await repo.findAlbumsAsync(artist, skip: 0, take: 0);
            Assert.IsNotNull(albums2);
            Assert.IsTrue(albums2.Count == 0);
        }

        [TestMethod]
        public async Task findAlbumsByArtistTakeWorks()
        {
            //Only one in test set, not very good test
            Artist artist = new Artist { Id = 1 };
            List<Album> albums1 = await repo.findAlbumsAsync(artist, skip: 0, take: 2);
            Assert.IsNotNull(albums1);
            Assert.IsTrue(albums1.Count == 1);
            Album firstAlbum1 = albums1[0];
            List<Album> albums2 = await repo.findAlbumsAsync(artist, skip: 0, take: 1);
            Assert.IsNotNull(albums2);
            Assert.IsTrue(albums2.Count == 1);
            Album firstAlbum2 = albums2[0];
            Assert.IsTrue(firstAlbum1 == firstAlbum2);
        }

        #endregion

        #region Task<List<IAlbumOrSong>> findAlbumsAndSongsAsync by Artist Tests

        [TestMethod]
        public async Task findAlbumsAndSongsByArtistGivesEmptyListForNullArtist()
        {
            Artist artist = null;
            List<IAlbumOrSong> albumSongs = await repo.findAlbumsAndSongsAsync(artist, skip: 0, take: 300);
            Assert.IsNotNull(albumSongs);
            Assert.IsTrue(albumSongs.Count == 0);
        }

        [TestMethod]
        public async Task findAlbumsAndSongsByArtistGivesEmptyListForInvalidArtist()
        {
            Artist artist = new Artist();
            Assert.IsFalse(artist.IsValid);
            List<IAlbumOrSong> albumSongs = await repo.findAlbumsAndSongsAsync(artist, skip: 0, take: 300);
            Assert.IsNotNull(albumSongs);
            Assert.IsTrue(albumSongs.Count == 0);
        }

        [TestMethod]
        public async Task findAlbumsAndSongsByArtistFindsByArtistName()
        {
            Artist artist1 = new Artist { Name = "Artist1" };
            Artist artist2 = new Artist { Name = "Artist2" };
            Artist artist3 = new Artist { Name = "Invalid Artist Name" };
            List<IAlbumOrSong> albumSongs1 = await repo.findAlbumsAndSongsAsync(artist1, skip: 0, take: 300);
            Assert.IsNotNull(albumSongs1);
            Assert.IsTrue(albumSongs1.Count == 3);
            int songCount = 0, albumCount = 0;
            foreach(var albumSong in albumSongs1)
            {
                if (albumSong is Album) ++albumCount;
                if (albumSong is Song) ++songCount;
            }
            Assert.IsTrue(songCount == 2);
            Assert.IsTrue(albumCount == 1);
            List<IAlbumOrSong> albumSongs2 = await repo.findAlbumsAndSongsAsync(artist2, skip: 0, take: 300);
            Assert.IsNotNull(albumSongs2);
            Assert.IsTrue(albumSongs2.Count == 2);
            songCount = 0; albumCount = 0;
            foreach(var albumSong in albumSongs2)
            {
                if (albumSong is Album) ++albumCount;
                if (albumSong is Song) ++songCount;
            }
            Assert.IsTrue(songCount == 1);
            Assert.IsTrue(albumCount == 1);
            List<IAlbumOrSong> albumSongs3 = await repo.findAlbumsAndSongsAsync(artist3, skip: 0, take: 300);
            Assert.IsNotNull(albumSongs3);
            Assert.IsTrue(albumSongs3.Count == 0);
        }

        [TestMethod]
        public async Task findAlbumsAndSongsByArtistFindsByArtistId()
        {
            Artist artist1 = new Artist { Id = 1 };
            Artist artist2 = new Artist { Id = 2 };
            Artist artist3 = new Artist { Id = 4000 };
            List<IAlbumOrSong> albumSongs1 = await repo.findAlbumsAndSongsAsync(artist1, skip: 0, take: 300);
            Assert.IsNotNull(albumSongs1);
            Assert.IsTrue(albumSongs1.Count == 3);
            int songCount = 0, albumCount = 0;
            foreach (var albumSong in albumSongs1)
            {
                if (albumSong is Album) ++albumCount;
                if (albumSong is Song) ++songCount;
            }
            Assert.IsTrue(songCount == 2);
            Assert.IsTrue(albumCount == 1);
            List<IAlbumOrSong> albumSongs2 = await repo.findAlbumsAndSongsAsync(artist2, skip: 0, take: 300);
            Assert.IsNotNull(albumSongs2);
            Assert.IsTrue(albumSongs2.Count == 2);
            songCount = 0; albumCount = 0;
            foreach (var albumSong in albumSongs2)
            {
                if (albumSong is Album) ++albumCount;
                if (albumSong is Song) ++songCount;
            }
            Assert.IsTrue(songCount == 1);
            Assert.IsTrue(albumCount == 1);
            List<IAlbumOrSong> albumSongs3 = await repo.findAlbumsAndSongsAsync(artist3, skip: 0, take: 300);
            Assert.IsNotNull(albumSongs3);
            Assert.IsTrue(albumSongs3.Count == 0);
        }

        [TestMethod]
        public async Task findAlbumsAndSongsByArtistWithInvalidSkipReturnsEmptyList()
        {
            Artist artist = new Artist { Id = 1 };
            Assert.IsTrue(artist.IsValid);
            List<IAlbumOrSong> albumSongs = await repo.findAlbumsAndSongsAsync(artist, skip: -1, take: 300);
            Assert.IsNotNull(albumSongs);
            Assert.IsTrue(albumSongs.Count == 0);
        }

        [TestMethod]
        public async Task findAlbumsAndSongsByArtistSkipWorks()
        {
            Artist artist = new Artist { Id = 1 };
            List<IAlbumOrSong> albumSongs1 = await repo.findAlbumsAndSongsAsync(artist, skip: 0, take: 300);
            Assert.IsNotNull(albumSongs1);
            Assert.IsTrue(albumSongs1.Count == 3);
            IAlbumOrSong lastAlbumSong1 = albumSongs1[2];
            List<IAlbumOrSong> albumSongs2 = await repo.findAlbumsAndSongsAsync(artist, skip: 2, take: 300);
            Assert.IsNotNull(albumSongs2);
            Assert.IsTrue(albumSongs2.Count == 1);
            IAlbumOrSong firstAlbumSong2 = albumSongs2[0];
            Assert.IsTrue(lastAlbumSong1.GetType() == firstAlbumSong2.GetType());
            Assert.IsTrue(lastAlbumSong1.Id == firstAlbumSong2.Id);
        }

        [TestMethod]
        public async Task findAlbumsAndSongsByArtistWithInvalidOrZeroTakeReturnsEmptyList()
        {
            Artist artist = new Artist { Id = 1 };
            List<IAlbumOrSong> albumSongs1 = await repo.findAlbumsAndSongsAsync(artist, skip: 0, take: -1);
            Assert.IsNotNull(albumSongs1);
            Assert.IsTrue(albumSongs1.Count == 0);
            List<IAlbumOrSong> albumSongs2 = await repo.findAlbumsAndSongsAsync(artist, skip: 0, take: 0);
            Assert.IsNotNull(albumSongs2);
            Assert.IsTrue(albumSongs2.Count == 0);
        }

        [TestMethod]
        public async Task findAlbumsAndSongsByArtistTakeWorks()
        {
            Artist artist = new Artist { Id = 1 };
            List<IAlbumOrSong> albumSongs1 = await repo.findAlbumsAndSongsAsync(artist, skip: 0, take: 2);
            Assert.IsNotNull(albumSongs1);
            Assert.IsTrue(albumSongs1.Count == 2);
            IAlbumOrSong firstAlbumSong1 = albumSongs1[0];
            List<IAlbumOrSong> albumSongs2 = await repo.findAlbumsAndSongsAsync(artist, skip: 0, take: 1);
            Assert.IsNotNull(albumSongs2);
            Assert.IsTrue(albumSongs2.Count == 1);
            IAlbumOrSong firstAlbumSong2 = albumSongs2[0];
            Assert.IsTrue(firstAlbumSong1.GetType() == firstAlbumSong2.GetType());
            Assert.IsTrue(firstAlbumSong1.Id == firstAlbumSong2.Id);
        }

        #endregion

        #region Task<List<Song>> findSongsAsync by Genre Tests

        [TestMethod]
        public async Task findSongsByGenreGivesEmptyListForNullArtist()
        {
            Genre genre = null;
            List<Song> songs = await repo.findSongsAsync(genre, skip: 0, take: 300);
            Assert.IsNotNull(songs);
            Assert.IsTrue(songs.Count == 0);
        }

        [TestMethod]
        public async Task findSongsByGenreGivesEmptyListForInvalidArtist()
        {
            Genre genre = new Genre();
            Assert.IsFalse(genre.IsValid);
            List<Song> songs = await repo.findSongsAsync(genre, skip: 0, take: 300);
            Assert.IsNotNull(songs);
            Assert.IsTrue(songs.Count == 0);
        }

        [TestMethod]
        public async Task findSongsByGenreFindsByArtistName()
        {
            Genre genre1 = new Genre { Name = "Genre1" };
            Genre genre2 = new Genre { Name = "Genre2" };
            Genre genre3 = new Genre { Name = "Invalid Genre Name" };
            List<Song> songs1 = await repo.findSongsAsync(genre1, skip: 0, take: 300);
            Assert.IsNotNull(songs1);
            Assert.IsTrue(songs1.Count == 2);
            List<Song> songs2 = await repo.findSongsAsync(genre2, skip: 0, take: 300);
            Assert.IsNotNull(songs2);
            Assert.IsTrue(songs2.Count == 1);
            List<Song> songs3 = await repo.findSongsAsync(genre3, skip: 0, take: 300);
            Assert.IsNotNull(songs3);
            Assert.IsTrue(songs3.Count == 0);
        }

        [TestMethod]
        public async Task findSongsByGenreFindsByArtistId()
        {
            Genre genre1 = new Genre { Id = 1 };
            Genre genre2 = new Genre { Id = 2 };
            Genre genre3 = new Genre { Id = 4000 };
            List<Song> songs1 = await repo.findSongsAsync(genre1, skip: 0, take: 300);
            Assert.IsNotNull(songs1);
            Assert.IsTrue(songs1.Count == 2);
            List<Song> songs2 = await repo.findSongsAsync(genre2, skip: 0, take: 300);
            Assert.IsNotNull(songs2);
            Assert.IsTrue(songs2.Count == 1);
            List<Song> songs3 = await repo.findSongsAsync(genre3, skip: 0, take: 300);
            Assert.IsNotNull(songs3);
            Assert.IsTrue(songs3.Count == 0);
        }

        [TestMethod]
        public async Task findSongsByGenreWithInvalidSkipReturnsEmptyList()
        {
            Genre genre = new Genre { Id = 1 };
            Assert.IsTrue(genre.IsValid);
            List<Song> songs = await repo.findSongsAsync(genre, skip: -1, take: 300);
            Assert.IsNotNull(songs);
            Assert.IsTrue(songs.Count == 0);
        }

        [TestMethod]
        public async Task findSongsByGenreSkipWorks()
        {
            Genre genre = new Genre { Id = 1 };
            List<Song> songs1 = await repo.findSongsAsync(genre, skip: 0, take: 300);
            Assert.IsNotNull(songs1);
            Assert.IsTrue(songs1.Count == 2);
            Song secondSong1 = songs1[1];
            List<Song> songs2 = await repo.findSongsAsync(genre, skip: 1, take: 300);
            Assert.IsNotNull(songs2);
            Assert.IsTrue(songs2.Count == 1);
            Song firstSong2 = songs2[0];
            Assert.IsTrue(secondSong1 == firstSong2);
        }

        [TestMethod]
        public async Task findSongsByGenreWithInvalidOrZeroTakeReturnsEmptyList()
        {
            Genre genre = new Genre { Id = 1 };
            List<Song> songs1 = await repo.findSongsAsync(genre, skip: 0, take: -1);
            Assert.IsNotNull(songs1);
            Assert.IsTrue(songs1.Count == 0);
            List<Song> songs2 = await repo.findSongsAsync(genre, skip: 0, take: 0);
            Assert.IsNotNull(songs2);
            Assert.IsTrue(songs2.Count == 0);
        }

        [TestMethod]
        public async Task findSongsByGenreTakeWorks()
        {
            Genre genre = new Genre { Id = 1 };
            List<Song> songs1 = await repo.findSongsAsync(genre, skip: 0, take: 2);
            Assert.IsNotNull(songs1);
            Assert.IsTrue(songs1.Count == 2);
            Song firstSong1 = songs1[0];
            List<Song> songs2 = await repo.findSongsAsync(genre, skip: 0, take: 1);
            Assert.IsNotNull(songs2);
            Assert.IsTrue(songs2.Count == 1);
            Song firstSong2 = songs2[0];
            Assert.IsTrue(firstSong1 == firstSong2);
        }

        #endregion

        #region Task<List<Album>> findAlbumsAsync by Genre Tests

        [TestMethod]
        public async Task findAlbumsByGenreGivesEmptyListForNullArtist()
        {
            Genre genre = null;
            List<Album> albums = await repo.findAlbumsAsync(genre, skip: 0, take: 300);
            Assert.IsNotNull(albums);
            Assert.IsTrue(albums.Count == 0);
        }

        [TestMethod]
        public async Task findAlbumsByGenreGivesEmptyListForInvalidArtist()
        {
            Genre genre = new Genre();
            Assert.IsFalse(genre.IsValid);
            List<Album> albums = await repo.findAlbumsAsync(genre, skip: 0, take: 300);
            Assert.IsNotNull(albums);
            Assert.IsTrue(albums.Count == 0);
        }

        [TestMethod]
        public async Task findAlbumsByGenreFindsByArtistName()
        {
            Genre genre1 = new Genre { Name = "Genre1" };
            Genre genre2 = new Genre { Name = "Genre2" };
            Genre genre3 = new Genre { Name = "Invalid Genre Name" };
            List<Album> albums1 = await repo.findAlbumsAsync(genre1, skip: 0, take: 300);
            Assert.IsNotNull(albums1);
            Assert.IsTrue(albums1.Count == 1);
            List<Album> albums2 = await repo.findAlbumsAsync(genre2, skip: 0, take: 300);
            Assert.IsNotNull(albums2);
            Assert.IsTrue(albums2.Count == 1);
            List<Album> albums3 = await repo.findAlbumsAsync(genre3, skip: 0, take: 300);
            Assert.IsNotNull(albums3);
            Assert.IsTrue(albums3.Count == 0);
        }

        [TestMethod]
        public async Task findAlbumsByGenreFindsByArtistId()
        {
            Genre genre1 = new Genre { Id = 1 };
            Genre genre2 = new Genre { Id = 2 };
            Genre genre3 = new Genre { Id = 4000 };
            List<Album> albums1 = await repo.findAlbumsAsync(genre1, skip: 0, take: 300);
            Assert.IsNotNull(albums1);
            Assert.IsTrue(albums1.Count == 1);
            List<Album> albums2 = await repo.findAlbumsAsync(genre2, skip: 0, take: 300);
            Assert.IsNotNull(albums2);
            Assert.IsTrue(albums2.Count == 1);
            List<Album> albums3 = await repo.findAlbumsAsync(genre3, skip: 0, take: 300);
            Assert.IsNotNull(albums3);
            Assert.IsTrue(albums3.Count == 0);
        }

        [TestMethod]
        public async Task findAlbumsByGenreWithInvalidSkipReturnsEmptyList()
        {
            Genre genre = new Genre { Id = 1 };
            Assert.IsTrue(genre.IsValid);
            List<Album> albums = await repo.findAlbumsAsync(genre, skip: -1, take: 300);
            Assert.IsNotNull(albums);
            Assert.IsTrue(albums.Count == 0);
        }

        [TestMethod]
        public async Task findAlbumsByGenreSkipWorks()
        {
            Genre genre = new Genre { Id = 1 };
            List<Album> albums1 = await repo.findAlbumsAsync(genre, skip: 0, take: 300);
            Assert.IsNotNull(albums1);
            Assert.IsTrue(albums1.Count == 1);
            List<Album> albums2 = await repo.findAlbumsAsync(genre, skip: 1, take: 300);
            Assert.IsNotNull(albums2);
            Assert.IsTrue(albums2.Count == 0);
        }

        [TestMethod]
        public async Task findAlbumsByGenreWithInvalidOrZeroTakeReturnsEmptyList()
        {
            Genre genre = new Genre { Id = 1 };
            List<Album> albums1 = await repo.findAlbumsAsync(genre, skip: 0, take: -1);
            Assert.IsNotNull(albums1);
            Assert.IsTrue(albums1.Count == 0);
            List<Album> albums2 = await repo.findAlbumsAsync(genre, skip: 0, take: 0);
            Assert.IsNotNull(albums2);
            Assert.IsTrue(albums2.Count == 0);
        }

        [TestMethod]
        public async Task findAlbumsByGenreTakeWorks()
        {
            //Only one in test set, not very good test
            Genre genre = new Genre { Id = 1 };
            List<Album> albums1 = await repo.findAlbumsAsync(genre, skip: 0, take: 2);
            Assert.IsNotNull(albums1);
            Assert.IsTrue(albums1.Count == 1);
            Album firstAlbum1 = albums1[0];
            List<Album> albums2 = await repo.findAlbumsAsync(genre, skip: 0, take: 1);
            Assert.IsNotNull(albums2);
            Assert.IsTrue(albums2.Count == 1);
            Album firstAlbum2 = albums2[0];
            Assert.IsTrue(firstAlbum1 == firstAlbum2);
        }

        #endregion

        #region Task<List<IAlbumOrSong>> findAlbumsAndSongsAsync by Genre Tests

        [TestMethod]
        public async Task findAlbumsAndSongsByGenreGivesEmptyListForNullArtist()
        {
            Genre genre = null;
            List<IAlbumOrSong> albumSongs = await repo.findAlbumsAndSongsAsync(genre, skip: 0, take: 300);
            Assert.IsNotNull(albumSongs);
            Assert.IsTrue(albumSongs.Count == 0);
        }

        [TestMethod]
        public async Task findAlbumsAndSongsByGenreGivesEmptyListForInvalidArtist()
        {
            Genre genre = new Genre();
            Assert.IsFalse(genre.IsValid);
            List<IAlbumOrSong> albumSongs = await repo.findAlbumsAndSongsAsync(genre, skip: 0, take: 300);
            Assert.IsNotNull(albumSongs);
            Assert.IsTrue(albumSongs.Count == 0);
        }

        [TestMethod]
        public async Task findAlbumsAndSongsByGenreFindsByArtistName()
        {
            Genre genre1 = new Genre { Name = "Genre1" };
            Genre genre2 = new Genre { Name = "Genre2" };
            Genre genre3 = new Genre { Name = "Invalid Genre Name" };
            List<IAlbumOrSong> albumSongs1 = await repo.findAlbumsAndSongsAsync(genre1, skip: 0, take: 300);
            Assert.IsNotNull(albumSongs1);
            Assert.IsTrue(albumSongs1.Count == 3);
            int songCount = 0, albumCount = 0;
            foreach (var albumSong in albumSongs1)
            {
                if (albumSong is Album) ++albumCount;
                if (albumSong is Song) ++songCount;
            }
            Assert.IsTrue(songCount == 2);
            Assert.IsTrue(albumCount == 1);
            List<IAlbumOrSong> albumSongs2 = await repo.findAlbumsAndSongsAsync(genre2, skip: 0, take: 300);
            Assert.IsNotNull(albumSongs2);
            Assert.IsTrue(albumSongs2.Count == 2);
            songCount = 0; albumCount = 0;
            foreach (var albumSong in albumSongs2)
            {
                if (albumSong is Album) ++albumCount;
                if (albumSong is Song) ++songCount;
            }
            Assert.IsTrue(songCount == 1);
            Assert.IsTrue(albumCount == 1);
            List<IAlbumOrSong> albumSongs3 = await repo.findAlbumsAndSongsAsync(genre3, skip: 0, take: 300);
            Assert.IsNotNull(albumSongs3);
            Assert.IsTrue(albumSongs3.Count == 0);
        }

        [TestMethod]
        public async Task findAlbumsAndSongsByGenreFindsByArtistId()
        {
            Genre genre1 = new Genre { Id = 1 };
            Genre genre2 = new Genre { Id = 2 };
            Genre genre3 = new Genre { Id = 4000 };
            List<IAlbumOrSong> albumSongs1 = await repo.findAlbumsAndSongsAsync(genre1, skip: 0, take: 300);
            Assert.IsNotNull(albumSongs1);
            Assert.IsTrue(albumSongs1.Count == 3);
            int songCount = 0, albumCount = 0;
            foreach (var albumSong in albumSongs1)
            {
                if (albumSong is Album) ++albumCount;
                if (albumSong is Song) ++songCount;
            }
            Assert.IsTrue(songCount == 2);
            Assert.IsTrue(albumCount == 1);
            List<IAlbumOrSong> albumSongs2 = await repo.findAlbumsAndSongsAsync(genre2, skip: 0, take: 300);
            Assert.IsNotNull(albumSongs2);
            Assert.IsTrue(albumSongs2.Count == 2);
            songCount = 0; albumCount = 0;
            foreach (var albumSong in albumSongs2)
            {
                if (albumSong is Album) ++albumCount;
                if (albumSong is Song) ++songCount;
            }
            Assert.IsTrue(songCount == 1);
            Assert.IsTrue(albumCount == 1);
            List<IAlbumOrSong> albumSongs3 = await repo.findAlbumsAndSongsAsync(genre3, skip: 0, take: 300);
            Assert.IsNotNull(albumSongs3);
            Assert.IsTrue(albumSongs3.Count == 0);
        }

        [TestMethod]
        public async Task findAlbumsAndSongsByGenreWithInvalidSkipReturnsEmptyList()
        {
            Genre genre = new Genre { Id = 1 };
            Assert.IsTrue(genre.IsValid);
            List<IAlbumOrSong> albumSongs = await repo.findAlbumsAndSongsAsync(genre, skip: -1, take: 300);
            Assert.IsNotNull(albumSongs);
            Assert.IsTrue(albumSongs.Count == 0);
        }

        [TestMethod]
        public async Task findAlbumsAndSongsByGenreSkipWorks()
        {
            Genre genre = new Genre { Id = 1 };
            List<IAlbumOrSong> albumSongs1 = await repo.findAlbumsAndSongsAsync(genre, skip: 0, take: 300);
            Assert.IsNotNull(albumSongs1);
            Assert.IsTrue(albumSongs1.Count == 3);
            IAlbumOrSong lastAlbumSong1 = albumSongs1[2];
            List<IAlbumOrSong> albumSongs2 = await repo.findAlbumsAndSongsAsync(genre, skip: 2, take: 300);
            Assert.IsNotNull(albumSongs2);
            Assert.IsTrue(albumSongs2.Count == 1);
            IAlbumOrSong firstAlbumSong2 = albumSongs2[0];
            Assert.IsTrue(lastAlbumSong1.GetType() == firstAlbumSong2.GetType());
            Assert.IsTrue(lastAlbumSong1.Id == firstAlbumSong2.Id);
        }

        [TestMethod]
        public async Task findAlbumsAndSongsByGenreWithInvalidOrZeroTakeReturnsEmptyList()
        {
            Genre genre = new Genre { Id = 1 };
            List<IAlbumOrSong> albumSongs1 = await repo.findAlbumsAndSongsAsync(genre, skip: 0, take: -1);
            Assert.IsNotNull(albumSongs1);
            Assert.IsTrue(albumSongs1.Count == 0);
            List<IAlbumOrSong> albumSongs2 = await repo.findAlbumsAndSongsAsync(genre, skip: 0, take: 0);
            Assert.IsNotNull(albumSongs2);
            Assert.IsTrue(albumSongs2.Count == 0);
        }

        [TestMethod]
        public async Task findAlbumsAndSongsByGenreTakeWorks()
        {
            Genre genre = new Genre { Id = 1 };
            List<IAlbumOrSong> albumSongs1 = await repo.findAlbumsAndSongsAsync(genre, skip: 0, take: 2);
            Assert.IsNotNull(albumSongs1);
            Assert.IsTrue(albumSongs1.Count == 2);
            IAlbumOrSong firstAlbumSong1 = albumSongs1[0];
            List<IAlbumOrSong> albumSongs2 = await repo.findAlbumsAndSongsAsync(genre, skip: 0, take: 1);
            Assert.IsNotNull(albumSongs2);
            Assert.IsTrue(albumSongs2.Count == 1);
            IAlbumOrSong firstAlbumSong2 = albumSongs2[0];
            Assert.IsTrue(firstAlbumSong1.GetType() == firstAlbumSong2.GetType());
            Assert.IsTrue(firstAlbumSong1.Id == firstAlbumSong2.Id);
        }

        #endregion
    }
}
