using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CDCatalogModel;

namespace CDCatalogTests
{
    [TestClass]
    public class ModelUnitTests
    {
        #region Helper Methods

        private Song getValidSong()
        {
            return new Song 
            {
                Id = 1,
                Title = "Test Song",
                TrackLength = 60,
                Rating = 10,
                ArtistId = 1,
                GenreId = 1,
                AlbumId = 1,
                TrackNumber = 1,
                Url = "http://www.song.com",
                Album = getValidAlbum(),
                Artist = getValidArtist(),
                Genre = getValidGenre()
            };
        }

        private Song getInvalidSong()
        {
            return new Song
            {
                Id = -1,
                Title = null,
                TrackLength = -10,
                Rating = Int32.MinValue,
                ArtistId = -1,
                GenreId = -1,
                AlbumId = -1,
                TrackNumber = -1,
                Url = "",
                Album = getInvalidAlbum(),
                Artist = getInvalidArtist(),
                Genre = getInvalidGenre()
            };
        }

        private Genre getValidGenre()
        {
            return new Genre 
            {
                Id = 1,
                Name = "My Genre",
                Albums = null,
                Songs = null
            };
        }

        private Genre getInvalidGenre()
        {
            return new Genre 
            {
                Id = -1,
                Name = null,
                Albums = null,
                Songs = null
            };
        }

        private Artist getValidArtist()
        {
            return new Artist 
            {
                Id = 1,
                Name = "My Artist",
                Albums = null,
                Songs = null
            };
        }

        private Artist getInvalidArtist()
        {
            return new Artist
            {
                Id = -1,
                Name = null,
                Albums = null,
                Songs = null
            };
        }

        private Album getValidAlbum()
        {
            return new Album
            {
                Id = 1,
                Title = "My Album",
                Year = 1999,
                Rating = 10,
                ArtistId = 1,
                GenreId = 1,
                Artist = getValidArtist(),
                Genre = getValidGenre(),
                Songs = null
            };
        }

        private Album getInvalidAlbum()
        {
            return new Album
            {
                Id = -1,
                Title = null,
                Year = -1000,
                Rating = Int32.MinValue,
                ArtistId = -1,
                GenreId = -1,
                Artist = getInvalidArtist(),
                Genre = getInvalidGenre(),
                Songs = null
            };
        }

        #endregion

        #region Helper Method Tests

        [TestMethod]
        public void validSongIsValid()
        {
            Assert.IsTrue(getValidSong().IsValid);
        }

        [TestMethod]
        public void validAlbumIsValid()
        {
            Assert.IsTrue(getValidAlbum().IsValid);
        }

        [TestMethod]
        public void validArtistIsValid()
        {
            Assert.IsTrue(getValidArtist().IsValid);
        }

        [TestMethod]
        public void validGenreIsValid()
        {
            Assert.IsTrue(getValidGenre().IsValid);
        }

        [TestMethod]
        public void invalidSongIsInvalid()
        {
            Assert.IsFalse(getInvalidSong().IsValid);
        }

        [TestMethod]
        public void invalidAlbumIsInvalid()
        {
            Assert.IsFalse(getInvalidAlbum().IsValid);
        }

        [TestMethod]
        public void invalidArtistIsInvalid()
        {
            Assert.IsFalse(getInvalidArtist().IsValid);
        }

        [TestMethod]
        public void invalidGenreIsInvalid()
        {
            Assert.IsFalse(getInvalidGenre().IsValid);
        }

        #endregion

        #region Song IsValid Property Tests

        [TestMethod]
        public void validSongIdIsGreaterThanOrEqualToZero()
        {
            Song song = getValidSong();
            song.Id = 0;              Assert.IsTrue(song.IsValid);
            song.Id = 1;              Assert.IsTrue(song.IsValid);
            song.Id = Int32.MaxValue; Assert.IsTrue(song.IsValid);
            /*****************************************************/
            song.Id = -1;             Assert.IsFalse(song.IsValid);
            song.Id = Int32.MinValue; Assert.IsFalse(song.IsValid);
        }

        [TestMethod]
        public void validSongTitleIsNotNullOrEmpty()
        {
            Song song = getValidSong();
            song.Title = "";   Assert.IsFalse(song.IsValid);
            song.Title = null; Assert.IsFalse(song.IsValid);
        }

        [TestMethod]
        public void validSongTrackLengthIsGreaterThanZero()
        {
            Song song = getValidSong();
            song.TrackLength = 1;               Assert.IsTrue(song.IsValid);
            song.TrackLength = Int32.MaxValue;  Assert.IsTrue(song.IsValid);
            /***************************************************************/
            song.TrackLength = 0;               Assert.IsFalse(song.IsValid);
            song.TrackLength = -1;              Assert.IsFalse(song.IsValid);
            song.TrackLength = Int32.MinValue;  Assert.IsFalse(song.IsValid);
        }

        [TestMethod]
        public void validSongHasRatingNullOrOneToTen()
        {
            Song song = getValidSong();
            song.Rating = null;             Assert.IsTrue(song.IsValid);
            for(int i = 1; i <= 10; ++i)
            {
                song.Rating = i;            Assert.IsTrue(song.IsValid);
            }
            /***********************************************************/
            song.Rating = 11;               Assert.IsFalse(song.IsValid);
            song.Rating = Int32.MaxValue;   Assert.IsFalse(song.IsValid);
            song.Rating = 0;                Assert.IsFalse(song.IsValid);
            song.Rating = -1;               Assert.IsFalse(song.IsValid);
            song.Rating = Int32.MinValue;   Assert.IsFalse(song.IsValid);
        }

        [TestMethod]
        public void validSongHasArtistIdGreaterThanOrEqualToZero()
        {
            Song song = getValidSong();
            song.ArtistId = 0;               Assert.IsTrue(song.IsValid);
            song.ArtistId = 1;               Assert.IsTrue(song.IsValid);
            song.ArtistId = Int32.MaxValue;
            song.Artist.Id = Int32.MaxValue; Assert.IsTrue(song.IsValid);
            /***********************************************************/
            song.Artist = getValidArtist();
            song.ArtistId = -1;              Assert.IsFalse(song.IsValid);
            song.ArtistId = Int32.MinValue;  Assert.IsFalse(song.IsValid);
            song.Artist = null;
            song.ArtistId = -1;              Assert.IsFalse(song.IsValid);
            song.ArtistId = Int32.MinValue;  Assert.IsFalse(song.IsValid);
        }

        [TestMethod]
        public void validSongHasNullOrValidArtist()
        {
            Song song = getValidSong();
            song.Artist = null;               Assert.IsTrue(song.IsValid);
            song.Artist = getValidArtist();   Assert.IsTrue(song.IsValid);
            /*************************************************************/
            song.Artist = getInvalidArtist(); Assert.IsFalse(song.IsValid);
        }

        [TestMethod]
        public void validSongHasArtistIdGreaterThanZeroOrValidArtist()
        {
            Song song = getValidSong();
            song.ArtistId = 1; song.Artist = getValidArtist();   Assert.IsTrue(song.IsValid);
            song.ArtistId = 0; song.Artist = getValidArtist();   Assert.IsTrue(song.IsValid);
            song.ArtistId = 1; song.Artist = null;               Assert.IsTrue(song.IsValid);
            song.ArtistId = 1; song.Artist = getValidArtist();
            song.Artist.Id = 0;                                  Assert.IsTrue(song.IsValid);
            /*******************************************************************************/
            song.ArtistId = 0; song.Artist = null;               Assert.IsFalse(song.IsValid);   
        }

        [TestMethod]
        public void validSongHasMatchingArtistIdsIfBothAssigned()
        {
            Song song = getValidSong();
            song.ArtistId = 1;
            song.Artist = getValidArtist();
            song.Artist.Id = 1;                 Assert.IsTrue(song.IsValid);
            /***************************************************************/
            song.Artist.Id = 2;                 Assert.IsFalse(song.IsValid);
        }
        
        [TestMethod]
        public void validSongHasGenreIdGreaterThanOrEqualToZero()
        {
            Song song = getValidSong();
            song.GenreId = 0;                Assert.IsTrue(song.IsValid);
            song.GenreId = 1;                Assert.IsTrue(song.IsValid);
            song.GenreId = Int32.MaxValue;
            song.Genre.Id = Int32.MaxValue;  Assert.IsTrue(song.IsValid);
            /***********************************************************/
            song.Genre = getValidGenre(); 
            song.GenreId = -1;               Assert.IsFalse(song.IsValid);
            song.GenreId = Int32.MinValue;   Assert.IsFalse(song.IsValid);
        }

        [TestMethod]
        public void validSongHasNullOrValidGenre()
        {
            Song song = getValidSong();
            song.Genre = null;              Assert.IsTrue(song.IsValid);
            song.Genre = getValidGenre();   Assert.IsTrue(song.IsValid);
            /***********************************************************/
            song.Genre = getInvalidGenre(); Assert.IsFalse(song.IsValid);
        }

        [TestMethod]
        public void validSongHasGenreIdGreaterThanZeroOrValidGenre()
        {
            Song song = getValidSong();
            song.GenreId = 1; song.Genre = getValidGenre();     Assert.IsTrue(song.IsValid);
            song.GenreId = 1; song.Genre = null;                Assert.IsTrue(song.IsValid);
            song.GenreId = 1; song.Genre = getValidGenre();
            song.Genre.Id = 0;                                  Assert.IsTrue(song.IsValid);
            song.GenreId = 0; song.Genre = getValidGenre();     Assert.IsTrue(song.IsValid);
            /*******************************************************************************/
            song.GenreId = 0; song.Genre = getInvalidGenre();   Assert.IsFalse(song.IsValid);
        }

        [TestMethod]
        public void validSongHasMatchingGenreIdsIfBothAssigned()
        {
            Song song = getValidSong();
            song.GenreId = 1;
            song.Genre = getValidGenre();
            song.Genre.Id = 1;              Assert.IsTrue(song.IsValid);
            /***********************************************************/
            song.Genre.Id = 2;              Assert.IsFalse(song.IsValid);
        }

        [TestMethod]
        public void validSongHasNullOrNonEmptyUrl()
        {
            Song song = getValidSong();
            song.Url = null;                    Assert.IsTrue(song.IsValid);
            song.Url = "h";                     Assert.IsTrue(song.IsValid);
            song.Url = "http://www.songs.com";  Assert.IsTrue(song.IsValid);
            /****************************************************************/
            song.Url = "";                      Assert.IsFalse(song.IsValid);
        }

        [TestMethod]
        public void validSongHasAlbumIdGreaterThanZeroOrNull()
        {
            Song song = getValidSong();
            song.Album = null;
            song.AlbumId = null;
            song.TrackNumber = null;        Assert.IsTrue(song.IsValid);
            song.Album = getValidAlbum();
            song.Album.Id = 1;
            song.AlbumId = 1;
            song.TrackNumber = 1;           Assert.IsTrue(song.IsValid);
            song.Album.Id = Int32.MaxValue;
            song.AlbumId = Int32.MaxValue;  Assert.IsTrue(song.IsValid);
            /***********************************************************/
            song.Album.Id = 0;
            song.AlbumId = 0;               Assert.IsFalse(song.IsValid);
            song.AlbumId = -1;
            song.Album.Id = -1;             Assert.IsFalse(song.IsValid);
            song.AlbumId = Int32.MinValue;
            song.Album.Id = Int32.MinValue; Assert.IsFalse(song.IsValid);
        }

        [TestMethod]
        public void validSongHasNullOrValidAlbum()
        {
            Song song = getValidSong();
            song.Album = null;
            song.AlbumId = null;
            song.TrackNumber = null;        Assert.IsTrue(song.IsValid);
            song.Album = getValidAlbum();
            song.AlbumId = 1;
            song.Album.Id = 1;
            song.TrackNumber = 1;           Assert.IsTrue(song.IsValid);
            /***********************************************************/
            song.AlbumId = null;
            song.Album = getInvalidAlbum(); Assert.IsFalse(song.IsValid);
        }

        [TestMethod]
        public void validSongAlbumIdsMatchIfBothSet()
        {
            Song song = getValidSong();
            song.AlbumId = 1;
            song.Album = getValidAlbum();
            song.Album.Id = 1;              Assert.IsTrue(song.IsValid);
            /***********************************************************/
            song.Album.Id = 2;              Assert.IsFalse(song.IsValid);
        }

        [TestMethod]
        public void validSongValidIfOnlyAlbumIdOrAlbumSet()
        {
            Song song = getValidSong();
            song.AlbumId = null;
            song.Album = getValidAlbum();   Assert.IsTrue(song.IsValid);
            song.AlbumId = 1;
            song.Album.Id = 0;              Assert.IsTrue(song.IsValid);
            song.AlbumId = 1;
            song.Album = null;              Assert.IsTrue(song.IsValid);
        }

        [TestMethod]
        public void validSongValidIfNoAlbum()
        {
            Song song = getValidSong();
            song.Album = null;
            song.AlbumId = null;
            song.TrackNumber = null;       Assert.IsTrue(song.IsValid);
        }

        [TestMethod]
        public void validSongHasNullOrNonZeroTrackNumber()
        {
            Song song = getValidSong();
            song.TrackNumber = null;
            song.Album = null;
            song.AlbumId = null;                Assert.IsTrue(song.IsValid);
            song.Album = getValidAlbum();
            song.AlbumId = 1;
            song.TrackNumber = 1;               Assert.IsTrue(song.IsValid);
            song.TrackNumber = Int32.MaxValue;  Assert.IsTrue(song.IsValid);
            /****************************************************************/
            song.TrackNumber = 0;               Assert.IsFalse(song.IsValid);
            song.TrackNumber = -1;              Assert.IsFalse(song.IsValid);
            song.TrackNumber = Int32.MinValue;  Assert.IsFalse(song.IsValid);
        }

        [TestMethod]
        public void validSongWithAlbumHasValidTrackNumber()
        {
            Song song = getValidSong();
            song.Album = getValidAlbum();
            song.TrackNumber = 1;           Assert.IsTrue(song.IsValid);
            song.Album = null;
            song.AlbumId = 1;               Assert.IsTrue(song.IsValid);
            /**********************************************************/
            song.TrackNumber = null;        Assert.IsFalse(song.IsValid);
            song.AlbumId = 0;
            song.Album = getValidAlbum();   Assert.IsFalse(song.IsValid);
        }

        [TestMethod]
        public void validSongWithoutAlbumHasNullTrackNumber()
        {
            Song song = getValidSong();
            song.Album = null;
            song.AlbumId = null;
            song.TrackNumber = null;    Assert.IsTrue(song.IsValid);
            /*******************************************************/
            song.TrackNumber = 1;       Assert.IsFalse(song.IsValid);
        }

        #endregion

        #region Song Equals,==,!=,GetHashCode Tests

        [TestMethod]
        public void songUnaryEqualsHandlesNull()
        {
            Song song1 = getValidSong();
            Song song2 = null;              Assert.IsFalse(song1.Equals(song2));
        }

        [TestMethod]
        public void songUnaryEqualsComparesValues()
        {
            Song song1 = getValidSong();
            Song song2 = getValidSong(); Assert.IsTrue(song1.Equals(song2));
                                         Assert.IsTrue(song2.Equals(song1));
        }

        [TestMethod]
        public void songUnaryEqualsHandlesSelfAssertion()
        {
            Song song1 = getValidSong();
            Song song2 = song1;             Assert.IsTrue(song1.Equals(song2));
                                            Assert.IsTrue(song2.Equals(song1));
        }

        [TestMethod]
        public void songBinaryEqualsHandlesNulls()
        {
            Song song1 = null;
            Song song2 = null;      Assert.IsTrue(song1 == song2);
            /*****************************************************/
            song1 = null;
            song2 = getValidSong(); Assert.IsFalse(song1 == song2);
                                    Assert.IsFalse(song2 == song1);
        }

        [TestMethod]
        public void songBinaryEqualsComparesValues()
        {
            Song song1 = getValidSong();
            Song song2 = getValidSong();    Assert.IsTrue(song1 == song2);
                                            Assert.IsTrue(song2 == song1);
            /*************************************************************/
            song2 = getInvalidSong();       Assert.IsFalse(song1 == song2);
                                            Assert.IsFalse(song2 == song1);
        }

        [TestMethod]
        public void songBinaryEqualsHandlesSelfAssertion()
        {
            Song song1 = getValidSong();
            Song song2 = song1;             Assert.IsTrue(song1 == song2);
                                            Assert.IsTrue(song2 == song1);
        }

        [TestMethod]
        public void songBinaryNotEqualsHandlesNulls()
        {
            Song song1 = null;
            Song song2 = getValidSong(); Assert.IsTrue(song1 != song2);
                                         Assert.IsTrue(song2 != song1);
            /**********************************************************/
            song1 = null;
            song2 = null;                Assert.IsFalse(song1 != song2);
        }

        [TestMethod]
        public void songBinaryNotEqualsComparesValues()
        {
            Song song1 = getValidSong();
            Song song2 = getInvalidSong();  Assert.IsTrue(song1 != song2);
                                            Assert.IsTrue(song2 != song1);
            /*************************************************************/
            song2 = getValidSong();         Assert.IsFalse(song1 != song2);
                                            Assert.IsFalse(song2 != song1);
        }

        [TestMethod]
        public void songBinaryNotEqualsHandlesSelfAssertion()
        {
            Song song1 = getValidSong();
            Song song2 = song1;             Assert.IsFalse(song1 != song2);
                                            Assert.IsFalse(song2 != song1);
        }

        [TestMethod]
        public void songGetHashCodeReturnsIntWithOrWithoutTitle()
        {
            Song song1 = getValidSong(); Assert.IsInstanceOfType(song1.GetHashCode(), typeof(int));
            song1.Title = null;          Assert.IsInstanceOfType(song1.GetHashCode(), typeof(int));
        }

        #endregion

        #region Album IsValid Property Tests

        [TestMethod]
        public void validAlbumHasIdGreaterThanOrEqualToZero()
        {
            Album album = getValidAlbum();
            album.Id = 0;               Assert.IsTrue(album.IsValid);
            album.Id = 1;               Assert.IsTrue(album.IsValid);
            album.Id = Int32.MaxValue;  Assert.IsTrue(album.IsValid);
            /********************************************************/
            album.Id = -1;              Assert.IsFalse(album.IsValid);
            album.Id = Int32.MinValue;  Assert.IsFalse(album.IsValid);
        }

        [TestMethod]
        public void validAlbumTitleNotNullOrEmpty()
        {
            Album album = getValidAlbum();
            album.Title = "a";          Assert.IsTrue(album.IsValid);
            album.Title = "My Title";   Assert.IsTrue(album.IsValid);
            /*******************************************************/
            album.Title = "";           Assert.IsFalse(album.IsValid);
            album.Title = null;         Assert.IsFalse(album.IsValid);
        }

        [TestMethod]
        public void validAlbumHasYearGreaterThanOrEqualToSixteenHundredAndLessThanToday()
        {
            Album album = getValidAlbum();
            album.Year = 1600;                  Assert.IsTrue(album.IsValid);
            album.Year = 1999;                  Assert.IsTrue(album.IsValid);
            album.Year = DateTime.Now.Year;     Assert.IsTrue(album.IsValid);
            /****************************************************************/
            album.Year = DateTime.Now.Year + 1; Assert.IsFalse(album.IsValid);
            album.Year = Int32.MaxValue;        Assert.IsFalse(album.IsValid);
            album.Year = 1599;                  Assert.IsFalse(album.IsValid);
            album.Year = 0;                     Assert.IsFalse(album.IsValid);
            album.Year = -1;                    Assert.IsFalse(album.IsValid);
            album.Year = Int32.MinValue;        Assert.IsFalse(album.IsValid);
        }

        [TestMethod]
        public void validAlbumHasRatingOneToTenOrNull()
        {
            Album album = getValidAlbum();
            album.Rating = null;            Assert.IsTrue(album.IsValid);
            for(int i = 1; i <= 10; ++i)
            {
                album.Rating = i;           Assert.IsTrue(album.IsValid);
            }
            /***************************************************/
            album.Rating = 11;              Assert.IsFalse(album.IsValid);
            album.Rating = Int32.MaxValue;  Assert.IsFalse(album.IsValid);
            album.Rating = 0;               Assert.IsFalse(album.IsValid);
            album.Rating = -1;              Assert.IsFalse(album.IsValid);
            album.Rating = Int32.MinValue;  Assert.IsFalse(album.IsValid);
        }

        [TestMethod]
        public void validAlbumHasArtistIdGreaterThanOrEqualToZero()
        {
            Album album = getValidAlbum();
            album.ArtistId = 0;
            album.Artist.Id = 1;                Assert.IsTrue(album.IsValid);
            album.ArtistId = 1;                 Assert.IsTrue(album.IsValid);
            album.ArtistId = Int32.MaxValue;
            album.Artist.Id = Int32.MaxValue;   Assert.IsTrue(album.IsValid);
            /****************************************************************/
            album.ArtistId = -1;
            album.Artist.Id = 0;                Assert.IsFalse(album.IsValid);
            album.ArtistId = Int32.MinValue;    Assert.IsFalse(album.IsValid);
        }

        [TestMethod]
        public void validAlbumHasNullOrValidArtist()
        {
            Album album = getValidAlbum();
            album.ArtistId = 1;
            album.Artist = null;                Assert.IsTrue(album.IsValid);
            album.Artist = getValidArtist();    Assert.IsTrue(album.IsValid);
            album.Artist.Id = 0;                Assert.IsTrue(album.IsValid);
            /****************************************************************/
            album.Artist = getInvalidArtist();  Assert.IsFalse(album.IsValid);
        }

        [TestMethod]
        public void validAlbumHasValidArtistIdOrArtist()
        {
            Album album = getValidAlbum();
            album.ArtistId = 1;
            album.Artist = getValidArtist();
            album.Artist.Id = 1;                Assert.IsTrue(album.IsValid);
            album.ArtistId = 0;                 Assert.IsTrue(album.IsValid);
            album.ArtistId = 1;
            album.Artist = null;                Assert.IsTrue(album.IsValid);
            /****************************************************************/
            album.ArtistId = 0;
            album.Artist = null;                Assert.IsFalse(album.IsValid);
        }

        [TestMethod]
        public void validAlbumArtistIdsMatchIfBothSet()
        {
            Album album = getValidAlbum();
            album.ArtistId = 1;
            album.Artist = getValidArtist();
            album.Artist.Id = 1;                Assert.IsTrue(album.IsValid);
            /****************************************************************/
            album.Artist.Id = 2;                Assert.IsFalse(album.IsValid);
        }

        [TestMethod]
        public void validAlbumHasGenreIdGreaterThanOrEqualToZero()
        {
            Album album = getValidAlbum();
            album.GenreId = 0;
            album.Genre.Id = 1;              Assert.IsTrue(album.IsValid);
            album.GenreId = 1;
            album.Genre.Id = 1;              Assert.IsTrue(album.IsValid);
            album.GenreId = Int32.MaxValue;
            album.Genre.Id = Int32.MaxValue; Assert.IsTrue(album.IsValid);
            /*************************************************************/
            album.GenreId = -1;
            album.Genre.Id = 0;             Assert.IsFalse(album.IsValid);
            album.GenreId = Int32.MinValue;
            album.Genre.Id = 0;             Assert.IsFalse(album.IsValid);
        }

        [TestMethod]
        public void validAlbumHasNullOrValidGenre()
        {
            Album album = getValidAlbum();
            album.Genre = null;                     Assert.IsTrue(album.IsValid);
            album.Genre = getValidGenre();          Assert.IsTrue(album.IsValid);
            /********************************************************************/
            album.Genre = getInvalidGenre();        Assert.IsFalse(album.IsValid);
        }

        [TestMethod]
        public void validAlbumHasValidGenreIdOrGenre()
        {
            Album album = getValidAlbum();
            album.GenreId = 1;
            album.Genre = getValidGenre();
            album.Genre.Id = 1;                     Assert.IsTrue(album.IsValid);
            album.GenreId = 0;                      Assert.IsTrue(album.IsValid);
            album.GenreId = 1;
            album.Genre = null;                     Assert.IsTrue(album.IsValid);
            /********************************************************************/
            album.GenreId = 0;
            album.Genre = null;                     Assert.IsFalse(album.IsValid);
        }

        [TestMethod]
        public void validAlbumGenreIdsMatchIfBothSet()
        {
            Album album = getValidAlbum();
            album.GenreId = 1;
            album.Genre = getValidGenre();
            album.Genre.Id = 1;                     Assert.IsTrue(album.IsValid);
            /********************************************************************/
            album.Genre.Id = 2;                     Assert.IsFalse(album.IsValid);
        }

        #endregion

        #region Album Equals,==,!=,GetHashCode Tests

        [TestMethod]
        public void albumUnaryEqualsHandlesNulls()
        {
            Album album1 = getValidAlbum();
            Album album2 = null;               Assert.IsFalse(album1.Equals(album2));
        }

        [TestMethod]
        public void albumUnaryEqualsComparesValues()
        {
            Album album1 = getValidAlbum();
            Album album2 = getValidAlbum(); Assert.IsTrue(album1.Equals(album2));
                                            Assert.IsTrue(album2.Equals(album1));
        }

        [TestMethod]
        public void albumUnaryEqualsHandlesSelfAssertion()
        {
            Album album1 = getValidAlbum();
            Album album2 = album1;          Assert.IsTrue(album1.Equals(album2));
                                            Assert.IsTrue(album2.Equals(album1));
        }

        [TestMethod]
        public void albumBinaryEqualsHandlesNulls()
        {
            Album album1 = null;
            Album album2 = null;      Assert.IsTrue(album1 == album2);
                                      Assert.IsTrue(album2 == album1);
            /*********************************************************/
            album1 = null;
            album2 = getValidAlbum(); Assert.IsFalse(album1 == album2);
                                      Assert.IsFalse(album2 == album1);
        }

        [TestMethod]
        public void albumBinaryEqualsComparesValues()
        {
            Album album1 = getValidAlbum();
            Album album2 = getValidAlbum(); Assert.IsTrue(album1 == album2);
                                            Assert.IsTrue(album2 == album1);
            /***************************************************************/
            album2 = getInvalidAlbum();     Assert.IsFalse(album1 == album2);
                                            Assert.IsFalse(album2 == album1);
        }

        [TestMethod]
        public void albumBinaryEqualsHandlesSelfAssertion()
        {
            Album album1 = getValidAlbum();
            Album album2 = album1;          Assert.IsTrue(album1 == album2);
                                            Assert.IsTrue(album2 == album1);
        }

        [TestMethod]
        public void albumBinaryNotEqualsHandlesNulls()
        {
            Album album1 = null;
            Album album2 = getValidAlbum(); Assert.IsTrue(album1 != album2);
                                            Assert.IsTrue(album2 != album1);
            /***************************************************************/
            album1 = null;
            album2 = null;                  Assert.IsFalse(album1 != album2);
                                            Assert.IsFalse(album2 != album1);
        }

        [TestMethod]
        public void albumBinaryNotEqualsComparesValues()
        {
            Album album1 = getValidAlbum();
            Album album2 = getInvalidAlbum(); Assert.IsTrue(album1 != album2);
                                              Assert.IsTrue(album2 != album1);
            /*****************************************************************/
            album2 = getValidAlbum();         Assert.IsFalse(album1 != album2);
                                              Assert.IsFalse(album2 != album1);
        }

        [TestMethod]
        public void albumBinaryNotEqualsHandlesSelfAssertion()
        {
            Album album1 = getValidAlbum();
            Album album2 = album1;            Assert.IsFalse(album1 != album2);
                                              Assert.IsFalse(album2 != album1);
        }

        [TestMethod]
        public void albumGetHashCodeWorksWithOrWithoutName()
        {
            Album album1 = getValidAlbum(); Assert.IsInstanceOfType(album1.GetHashCode(), typeof(int));                           
            album1.Title = null;            Assert.IsInstanceOfType(album1.GetHashCode(), typeof(int));
        }

        #endregion

        #region Artist IsValid Property Tests

        [TestMethod]
        public void validArtistHasIdGreaterThanOrEqualToZero()
        {
            Artist artist = getValidArtist();
            artist.Id = 0;              Assert.IsTrue(artist.IsValid);
            artist.Id = 1;              Assert.IsTrue(artist.IsValid);
            artist.Id = Int32.MaxValue; Assert.IsTrue(artist.IsValid);
            /*********************************************************/
            artist.Id = -1;             Assert.IsFalse(artist.IsValid);
            artist.Id = Int32.MinValue; Assert.IsFalse(artist.IsValid);
        }

        [TestMethod]
        public void validArtistNameNotNullOrEmpty()
        {
            Artist artist = getValidArtist();
            artist.Name = "a";          Assert.IsTrue(artist.IsValid);
            artist.Name = "My Artist";  Assert.IsTrue(artist.IsValid);
            /*********************************************************/
            artist.Name = "";           Assert.IsFalse(artist.IsValid);
            artist.Name = null;         Assert.IsFalse(artist.IsValid);
        }

        #endregion

        #region Artist Equals,==,!=,GetHashCode Tests

        [TestMethod]
        public void artistUnaryEqualsHandlesNulls()
        {
            Artist artist1 = getValidArtist();
            Artist artist2 = null;              Assert.IsFalse(artist1.Equals(artist2));
        }

        [TestMethod]
        public void artistUnaryEqualComparesValues()
        {
            Artist artist1 = getValidArtist();
            Artist artist2 = getValidArtist();  Assert.IsTrue(artist1.Equals(artist2));
                                                Assert.IsTrue(artist2.Equals(artist1));
        }

        [TestMethod]
        public void artistUnaryEqualsHandlesSelfAssertion()
        {
            Artist artist1 = getValidArtist();
            Artist artist2 = artist1;           Assert.IsTrue(artist1.Equals(artist2));
                                                Assert.IsTrue(artist2.Equals(artist1));
        }

        [TestMethod]
        public void artistBinaryEqualsHandlesNulls()
        {
            Artist artist1 = null;
            Artist artist2 = null;              Assert.IsTrue(artist1 == artist2);
            /*********************************************************************/
            artist1 = getValidArtist();
            artist2 = null;                     Assert.IsFalse(artist1 == artist2);
                                                Assert.IsFalse(artist2 == artist1);
        }

        [TestMethod]
        public void artistBinaryEqualsComparesValues()
        {
            Artist artist1 = getValidArtist();
            Artist artist2 = getValidArtist();  Assert.IsTrue(artist1 == artist2);
                                                Assert.IsTrue(artist2 == artist1);
            /*********************************************************************/
            artist2 = getInvalidArtist();       Assert.IsFalse(artist1 == artist2);
                                                Assert.IsFalse(artist2 == artist1);
        }

        [TestMethod]
        public void artistBinaryEqualsHandleSelfAssertion()
        {
            Artist artist1 = getValidArtist();
            Artist artist2 = artist1;           Assert.IsTrue(artist1 == artist2);
                                                Assert.IsTrue(artist2 == artist1);
        }

        [TestMethod]
        public void artistBinaryNotEqualsHandlesNulls()
        {
            Artist artist1 = getValidArtist();
            Artist artist2 = null;              Assert.IsTrue(artist1 != artist2);
                                                Assert.IsTrue(artist2 != artist1);
            /*********************************************************************/
            artist1 = null;
            artist2 = null;                     Assert.IsFalse(artist1 != artist2);
                                                Assert.IsFalse(artist2 != artist1);
        }

        [TestMethod]
        public void artistBinaryNotEqualsComparesValues()
        {
            Artist artist1 = getValidArtist();
            Artist artist2 = getInvalidArtist(); Assert.IsTrue(artist1 != artist2);
                                                 Assert.IsTrue(artist2 != artist1);
            /**********************************************************************/
            artist2 = getValidArtist();          Assert.IsFalse(artist1 != artist2);
                                                 Assert.IsFalse(artist2 != artist1);
        }

        [TestMethod]
        public void artistBinaryNotEqualsHandlesSelfAssertion()
        {
            Artist artist1 = getValidArtist();
            Artist artist2 = artist1;           Assert.IsFalse(artist1 != artist2);
                                                Assert.IsFalse(artist2 != artist1);
        }

        [TestMethod]
        public void artistGetHashCodeWorksWithOrWithoutName()
        {
            Artist artist1 = getValidArtist(); Assert.IsInstanceOfType(artist1.GetHashCode(), typeof(int));
            artist1.Name = null;               Assert.IsInstanceOfType(artist1.GetHashCode(), typeof(int));
        }

        #endregion

        #region Genre IsValid Property Tests

        [TestMethod]
        public void validGenreHasIdGreaterThanOrEqualToZero()
        {
            Genre genre = getValidGenre();
            genre.Id = 0;               Assert.IsTrue(genre.IsValid);
            genre.Id = 1;               Assert.IsTrue(genre.IsValid);
            genre.Id = Int32.MaxValue;  Assert.IsTrue(genre.IsValid);
            /********************************************************/
            genre.Id = -1;              Assert.IsFalse(genre.IsValid);
            genre.Id = Int32.MinValue;  Assert.IsFalse(genre.IsValid);
        }

        [TestMethod]
        public void validGenreNameNotNullOrEmpty()
        {
            Genre genre = getValidGenre();
            genre.Name = "a";           Assert.IsTrue(genre.IsValid);
            genre.Name = "My Genre";    Assert.IsTrue(genre.IsValid);
            /********************************************************/
            genre.Name = "";            Assert.IsFalse(genre.IsValid);
            genre.Name = null;          Assert.IsFalse(genre.IsValid);
        }

        #endregion

        #region Genre Equals,==,!=,GetHashCode Tests

        [TestMethod]
        public void genreUnaryEqualsHandlesNulls()
        {
            Genre genre1 = getValidGenre();
            Genre genre2 = null;            Assert.IsFalse(genre1.Equals(genre2));
        }

        [TestMethod]
        public void genreUnaryEqualsComparesValues()
        {
            Genre genre1 = getValidGenre();
            Genre genre2 = getValidGenre(); Assert.IsTrue(genre1.Equals(genre2));
                                            Assert.IsTrue(genre2.Equals(genre1));
            /********************************************************************/
            genre2 = getInvalidGenre();     Assert.IsFalse(genre1.Equals(genre2));
                                            Assert.IsFalse(genre2.Equals(genre1));
        }

        [TestMethod]
        public void genreUnaryEqualsHandlesSelfAssertion()
        {
            Genre genre1 = getValidGenre();
            Genre genre2 = genre1;          Assert.IsTrue(genre1.Equals(genre2));
                                            Assert.IsTrue(genre2.Equals(genre1));
        }

        [TestMethod]
        public void genreBinaryEqualsHandlesNulls()
        {
            Genre genre1 = null;
            Genre genre2 = null;        Assert.IsTrue(genre1 == genre2);
            /***********************************************************/
            genre1 = null;
            genre2 = getInvalidGenre(); Assert.IsFalse(genre1 == genre2);
                                        Assert.IsFalse(genre2 == genre1);
        }

        [TestMethod]
        public void genreBinaryEqualsComparesValues()
        {
            Genre genre1 = getValidGenre();
            Genre genre2 = getValidGenre(); Assert.IsTrue(genre1 == genre2);
                                            Assert.IsTrue(genre2 == genre1);
            /***************************************************************/
            genre2 = getInvalidGenre();     Assert.IsFalse(genre1 == genre2);
                                            Assert.IsFalse(genre2 == genre1);
        }

        [TestMethod]
        public void genreBinaryEqualsHandlesSelfAssertion()
        {
            Genre genre1 = getValidGenre();
            Genre genre2 = genre1;          Assert.IsTrue(genre1 == genre2);
                                            Assert.IsTrue(genre2 == genre1);
        }

        [TestMethod]
        public void genreBinaryNotEqualsHandlesNulls()
        {
            Genre genre1 = getValidGenre();
            Genre genre2 = null;            Assert.IsTrue(genre1 != genre2);
                                            Assert.IsTrue(genre2 != genre1);
            /***************************************************************/
            genre1 = null;
            genre2 = null;                  Assert.IsFalse(genre1 != genre2);
                                            Assert.IsFalse(genre2 != genre1);
        }

        [TestMethod]
        public void genreBinaryNotEqualsComparesValues()
        {
            Genre genre1 = getValidGenre();
            Genre genre2 = getInvalidGenre(); Assert.IsTrue(genre1 != genre2);
                                              Assert.IsTrue(genre2 != genre1);
            /*****************************************************************/
            genre2 = getValidGenre();         Assert.IsFalse(genre1 != genre2);
                                              Assert.IsFalse(genre2 != genre1);
        }

        [TestMethod]
        public void genreBinaryNotEqualsHandlesSelfAssertion()
        {
            Genre genre1 = getValidGenre();
            Genre genre2 = genre1;          Assert.IsFalse(genre1 != genre2);
                                            Assert.IsFalse(genre2 != genre1);
        }

        [TestMethod]
        public void genreGetHashCodeWorksWithOrWithoutName()
        {
            Genre genre1 = getValidGenre(); Assert.IsInstanceOfType(genre1.GetHashCode(), typeof(int));
            genre1.Name = null;             Assert.IsInstanceOfType(genre1.GetHashCode(), typeof(int));
        }

        #endregion
    }
}
